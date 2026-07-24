# Code Review Notes

**Scope:** `TicketService`, `TicketStatusTransitionService`  
**Date:** 2026-07-24  
**Reviewer:** Self-review (with AI-assisted pass)  
**Goal:** Identify real issues to fix — not style preferences.

---

## Review checklist

- [x] Validation gaps (service vs controller)
- [x] Null-reference risks
- [x] Concurrent update / lost-update scenarios
- [x] Exception types that become 500s
- [x] Mapping / response consistency
- [ ] Fixes applied (tracked in `review-fixes.md`)

---

## Work list — accepted findings

Work through these in priority order.

### Critical / high

| # | Severity | Finding | Where | Why it matters |
|---|----------|---------|-------|----------------|
| H1 | **High** | **No optimistic concurrency** on `Ticket` | `Ticket` entity, `UpdateAsync`, `TransitionAsync` | Two concurrent requests load the same ticket, each mutates a different field (e.g. A patches status Open→InProgress, B patches Open→Cancelled). Both read `Open`; both transitions are valid from that snapshot; **last `SaveChanges` wins** and can skip intermediate states or overwrite a field update. No `RowVersion` / concurrency token → no `DbUpdateConcurrencyException`. |
| H2 | **High** | **Lost field updates when status + PUT race** | `TicketService.UpdateAsync` + `TicketStatusTransitionService.TransitionAsync` | EF tracks the full entity. Concurrent PUT (title) and PATCH (status) on the same ticket: whichever saves second writes **all** scalar columns from its in-memory snapshot, potentially reverting the other request's change. |
| H3 | **Medium** | **`Enum.Parse` / `TicketPriorityParser.Parse` can throw unhandled** | `TransitionAsync` line 32; `CreateAsync` / `UpdateAsync` | Controller DTO validation normally runs first, but the service layer does not defend itself. Direct service calls or validation bypass → `ArgumentException` → **500** instead of 400. |
| H4 | **Medium** | **User existence check is TOCTOU** | `EnsureUserExistsAsync` in `TicketService` | User validated with `AnyAsync`, then ticket saved later. If user row disappears between check and insert (unlikely with seed-only users, possible in tests), FK `Restrict` on `CreatedById` → `DbUpdateException` → **500**. |

### Medium / low

| # | Severity | Finding | Where | Why it matters |
|---|----------|---------|-------|----------------|
| M1 | **Medium** | **Null `request` not guarded in services** | All public methods on both services | `[ApiController]` validates before the action runs, but services are public and testable. `request.Title.Trim()` or `request.Status.Trim()` → **NullReferenceException** → 500 if called without a body. |
| M2 | **Medium** | **`Map` assumes `CreatedBy` is loaded** | `TicketService.Map` line 114: `ticket.CreatedBy.Name` | `Include(t => t.CreatedBy)` is used in queries today. If a future code path calls `Map` without Include, or DB integrity breaks, → NRE. `CreatedBy` is `null!` on entity — compiler won't warn. |
| M3 | **Medium** | **Duplicated response mapping; transition path omits shared mapper** | `TicketStatusTransitionService.LoadResponseAsync` vs `TicketService.Map` | `LoadResponseAsync` hand-builds `TicketResponse` and does not use `TicketService.Map`. Any field added to the contract (e.g. `allowedNextStatuses` per API design) can be **missing from PATCH responses only** — frontend drift. |
| M4 | **Low** | **Post-save reload uses null-forgiving `!`** | `CreateAsync` / `UpdateAsync`: `(await GetByIdAsync(...))!` | After insert/update, reload could theoretically return null (ticket deleted in another request). Unlikely; would throw on next access rather than returning a clean 404. |
| M5 | **Low** | **SQLite write concurrency unhandled** | All `SaveChangesAsync` | Under parallel integration tests or load, SQLite can throw on locked database. No retry or translated error — surfaces as 500. Acceptable for local assessment; note for production. |
| M6 | **Low** | **`GetAllAsync` loads entire table into memory** | `TicketService.GetAllAsync` | Not a correctness bug at assessment scale; will not scale. Double sort (SQL by Id, memory by `CreatedAt`) is a SQLite workaround — document or add pagination later. |

### Behaviour to confirm (not necessarily bugs)

| # | Topic | Current behaviour | Decision needed |
|---|-------|-------------------|-----------------|
| B1 | Same-status PATCH | `current == target` → 200 no-op | Correct for idempotent API; ensure integration tests expect **Pass**, not 409. |
| B2 | Terminal status transitions | `Closed` / `Cancelled` → any change throws `InvalidTransitionException` | Correct per state machine. |
| B3 | Clear assignee on PUT | `assignedToId: null` clears assignment | Matches nullable FK; confirm product intent. |

---

## Self-review pass (going through the list)

| # | Verdict | Notes |
|---|---------|-------|
| H1 | **Accept** | Reproducible with two parallel PATCH requests from `Open`. No `[Timestamp]` on `Ticket`, no concurrency configuration in `TicketConfiguration`. |
| H2 | **Accept** | Classic read-modify-write race. Fix options: row version, or targeted SQL `UPDATE` for single columns, or serializable transaction (heavy). |
| H3 | **Accept** | `AllowedTicketStatus` / `AllowedTicketPriority` on DTOs help at the HTTP layer only. Service should not assume controller always ran. |
| H4 | **Accept** | Low probability with seeded users; still a real gap for robust error mapping. |
| M1 | **Accept** | Defensive `ArgumentNullException.ThrowIfNull(request)` is cheap insurance. |
| M2 | **Accept** | FK + Include make this safe on happy path; fragile if `Map` is reused. |
| M3 | **Accept** | Confirmed `LoadResponseAsync` duplicates mapping and is not shared with `TicketService.Map`. |
| M4 | **Defer** | Edge case; optional guard clause. |
| M5 | **Defer** | Out of scope for SQLite assessment MVP. |
| M6 | **Defer** | Performance, not correctness. |

---

## Rejected findings (and why)

These came up during review but were **intentionally not added** to the work list.

| Suggestion | Why rejected |
|------------|--------------|
| **"Move services to Application layer"** | Valid architecture note from `design-notes.md`, but not a runtime defect. Refactor when layering is in scope — not a TicketService bug. |
| **"Add repository abstraction over DbContext"** | Indirection without a current test/mocking pain point. YAGNI for this assessment. |
| **"Inject `ITicketService` into transition service"** | Coupling concern only. No functional bug. |
| **"Same-status transition should return 409"** | Product choice. Current idempotent 200 is documented and reasonable for PATCH. Would change API contract / test matrix. |
| **"Reject whitespace-only title after Trim()"** | Controller `[Required(AllowEmptyStrings = false)]` already blocks `""`. A title of `"   "` becomes empty after Trim in service — **borderline**, but could add service trim-validation; rejected as duplicate of DTO concern unless fuzzing bypasses MVC validation. *Revisit if you add minimal API or non-controller callers.* |
| **"Null-forgiving `CreatedBy` on entity is wrong"** | Required FK makes null CreatedBy a data corruption case, not normal flow. Fix data / Include, not entity nullability alone. |
| **"Use `DateTime.UtcNow` vs `DateTimeOffset`"** | `DateTimeOffset.UtcNow` is used consistently on create. Not a bug. |
| **"TransitionMap should live in service not Domain"** | Placement is design preference. Rules are correctly consulted before save. |
| **"DbContext should be `IAppDbContext` interface"** | Testability pattern, not a production defect. Integration tests can use `WebApplicationFactory`. |
| **"Missing auth on who can transition status"** | Out of assessment scope (no auth model). Not a service-layer bug today. |
| **"Validate user role before assign / create"** | Business rule not in requirements. Users are seeded; any user can be creator/assignee per current API contract. |

---

## Recommended fix order (for `review-fixes.md`)

1. **H1 + H2** — Add `byte[] RowVersion` or `uint Version` to `Ticket`, configure concurrency token, catch `DbUpdateConcurrencyException` → 409 with clear message.
2. **M3** — Single `TicketResponseMapper` used by both services; include `allowedNextStatuses` from `TransitionMap.GetAllowedNext` on every ticket response.
3. **H3 + M1** — Guard null request; wrap parse failures in domain `ValidationException`.
4. **H4** — Catch `DbUpdateException` for FK violations → 400/404 as appropriate (or rely on existence checks in same transaction).
5. **M2** — Null-conditional or explicit throw if `CreatedBy` missing in `Map`.

---

## Reviewer comments

- Strongest real gap for an assessment demo: **concurrency** (H1/H2). State machine tests on a single thread will all pass while parallel requests can still corrupt lifecycle.
- Validation is **controller-heavy, service-light** — fine for thin assessment, but services should not throw raw `ArgumentException` / NRE.
- Transition service correctly uses `TransitionMap` before save and throws **`InvalidTransitionException`** (mapped to 409 in controller) — that path looks sound for single-threaded use.
- No code changes made in this review pass; findings only.
