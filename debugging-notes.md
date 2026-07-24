# Debugging Notes

## Issue: POST comment on missing ticket returns 500 instead of 404

**Date:** 2026-07-24  
**Endpoint:** `POST /api/tickets/{ticketId}/comments`  
**Repro:** Comment on a ticket id that does not exist (e.g. `99999`).

---

### Problem

Adding a comment to a non-existent ticket should return **404 Not Found** with a clear message (`Ticket with id {id} was not found.`). Instead the API returns **500 Internal Server Error**, which usually means an **unhandled exception** reached ASP.NET Core’s developer exception page / generic error handler.

Expected flow:

1. Service checks ticket exists.
2. If not → throw `NotFoundException`.
3. Controller catches it → return `404` + `ProblemDetails`.

---

### Investigation

#### 1. Read the controller — is 404 wired up?

`TicketCommentsController.Create` has:

```csharp
try
{
    var created = await _comments.AddAsync(ticketId, request, cancellationToken);
    return CreatedAtAction(nameof(List), new { ticketId }, created);
}
catch (NotFoundException ex)
{
    return TicketNotFound(ex);
}
```

So **if** `NotFoundException` is thrown and is the **same type** the `catch` expects (`SupportTickets.Domain.Exceptions.NotFoundException`), the client should get 404 — not 500.

A 500 here usually means one of:

| Likelihood | What actually blew up | Why it becomes 500 |
|------------|----------------------|-------------------|
| **High** | `DbUpdateException` on `SaveChangesAsync` | FK constraint: comment references missing `TicketId`. No `catch` for EF exceptions → unhandled. |
| **High** | `NotFoundException` thrown but **not caught** | Early version without `try/catch`, or wrong exception type in `catch`. |
| Medium | `NullReferenceException` on null `request` | Would need ticket check to run first; less likely for “missing ticket” specifically. |
| Low | `CreatedAtAction` throws | Only on **success** (201) path — not when ticket is missing. |

#### 2. Read the service — does it check before insert?

`TicketCommentService.AddAsync` calls `EnsureTicketExistsAsync` **before** `SaveChanges`:

```csharp
await EnsureTicketExistsAsync(ticketId, cancellationToken);
// ...
_db.Comments.Add(comment);
await _db.SaveChangesAsync(cancellationToken);
```

`EnsureTicketExistsAsync` throws `NotFoundException` when `AnyAsync` is false.

**If that check is missing, commented out, or never reached**, EF still tries to insert a row with an invalid `TicketId`. SQLite enforces the FK (`TicketComment` → `Ticket`) and throws `DbUpdateException` — classic **500**, not 404.

#### 3. Check the console / stack trace (what to look for)

You didn’t paste the error yet, but the **exception type** tells you which branch failed:

- **`NotFoundException`** + 500 → controller isn’t catching it (or a different `NotFoundException` type).
- **`DbUpdateException`** / `SQLite Error 19: FOREIGN KEY constraint failed` → insert ran without a successful existence check (or check was bypassed).
- **`InvalidOperationException`** mentioning `CreatedAtAction` / `No route matches` → success path only; unrelated to missing ticket.

#### 4. Quick mental model

```
POST /comments
    │
    ├─ Ticket exists? ──no──► throw NotFoundException ──► catch ──► 404 ✓
    │
    └─ yes ──► SaveChanges ──► 201 ✓

If existence check skipped:
    SaveChanges ──► FK fails ──► DbUpdateException ──► 500 ✗
```

---

### How AI helped

- Traced the request path: `TicketCommentsController` → `TicketCommentService.AddAsync` → `EnsureTicketExistsAsync` / `SaveChangesAsync`.
- Compared **intended** design (explicit 404) vs **common failure** (FK violation 500 when pre-check is absent).
- Ran a local repro against the current codebase:

  ```http
  POST http://127.0.0.1:5189/api/tickets/99999/comments
  Content-Type: application/json

  {"body":"test","createdById":1}
  ```

  **Result:** `404` with body  
  `{"title":"Ticket not found","status":404,"detail":"Ticket with id 99999 was not found."}`

- Conclusion: current repo code has the fix in place; a 500 likely means either an older build without the check/catch, or the real exception is `DbUpdateException` (not `NotFoundException`).

---

### What I personally validated

- [x] `EnsureTicketExistsAsync` runs **before** insert in `TicketCommentService.AddAsync`.
- [x] It throws `SupportTickets.Domain.Exceptions.NotFoundException`.
- [x] Controller `catch (NotFoundException ex)` matches that type (same namespace via `using SupportTickets.Domain.Exceptions`).
- [x] Live POST to ticket `99999` returns **404** (not 500) on current code.
- [ ] **Your turn:** paste the console stack trace and confirm exception type (`NotFoundException` vs `DbUpdateException`).

---

### Final fix

**Root cause (most likely):** Missing or bypassed ticket-existence check → EF FK failure on save → unhandled `DbUpdateException` → 500.

**Fix (two parts — both needed):**

1. **Service:** Check ticket exists and throw domain `NotFoundException` before `Add` / `SaveChanges`:

   ```csharp
   private async Task EnsureTicketExistsAsync(int ticketId, CancellationToken ct)
   {
       if (!await _db.Tickets.AnyAsync(t => t.Id == ticketId, ct))
           throw new NotFoundException($"Ticket with id {ticketId} was not found.");
   }
   ```

2. **Controller:** Catch that exception and map to HTTP 404 (not 500):

   ```csharp
   catch (NotFoundException ex)
   {
       return NotFound(new ProblemDetails
       {
           Title = "Ticket not found",
           Detail = ex.Message,
           Status = StatusCodes.Status404NotFound
       });
   }
   ```

**Do not rely on FK alone** for API semantics — database errors should become intentional HTTP status codes in the service/controller layer.

**Optional hardening:** Add a global exception filter or middleware that maps `NotFoundException` → 404 everywhere so individual controllers can’t forget the `catch`.

---

## Log

| Date | Issue | Root cause | Resolution |
|------|-------|------------|------------|
| 2026-07-24 | POST comment on missing ticket → 500 | Likely unhandled `DbUpdateException` (FK) or uncaught `NotFoundException` | `EnsureTicketExistsAsync` + controller `catch (NotFoundException)` → 404 |

## Common areas to watch

- EF Core migrations vs model drift.
- CORS between React and Web API during local dev.
- Status/priority enum serialization mismatches.
- Integration test database isolation / leftover state.
- **Using DB constraint errors as control flow** — always validate existence in the service and return 404 explicitly.
