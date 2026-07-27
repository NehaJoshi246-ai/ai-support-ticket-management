# Reflection

## What went well

- Assessment scope stayed narrow: seeded users, ticket CRUD, explicit status machine, SQLite — enough to demo end-to-end API behaviour without auth or workflow engines.
- Dedicated `PATCH /api/tickets/{id}/status` made lifecycle rules testable in isolation from field updates.
- Field-level **400** validation and **409** for illegal transitions give clients actionable errors (not generic 500s).
- `DataSeeder` via EF `HasData` delivers demo-ready data on first migration — no separate seed script to run.
- **25/25** integration tests now lock the transition matrix to `TransitionMap` in code.

---

## Decision trade-offs (concrete)

Each row: what we chose, what we rejected, and why — with evidence where available.

### 1. Status changes: dedicated PATCH vs PUT body

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **PATCH `/status` only** (chosen) | Clear HTTP semantics; PUT cannot accidentally change lifecycle; easy to test matrix | Extra endpoint; UI needs two calls for “edit fields + change status” | **Chosen** — `UpdateTicketRequest` explicitly rejects `status` on PUT with 400 |
| PUT with `status` field | One request for “save everything” | Easy to bypass `TransitionMap`; harder to return rich 409 | Rejected |

**Evidence:** `TicketsController` exposes `PATCH {id}/status`; `UpdateTicketRequest` validation message directs clients to the PATCH route.

### 2. Invalid transition: 409 Conflict vs 400 Bad Request

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **409 + ProblemDetails extensions** (chosen) | Signals “valid JSON, invalid business state”; can attach `fromStatus`, `toStatus`, `allowedNextStatuses` | Some clients treat 409 as “retry later” (misuse) | **Chosen** — human decision during implementation |
| 400 validation error | Familiar for form APIs | Blurs input shape errors with lifecycle rules | Rejected |

**Evidence:** Invalid `Open → Resolved` returns 409 with extensions populated (asserted in 15 matrix tests). Example shape:

```json
{
  "title": "Invalid status transition",
  "status": 409,
  "fromStatus": "Open",
  "toStatus": "Resolved",
  "allowedNextStatuses": ["InProgress", "Cancelled"]
}
```

### 3. Same-status PATCH: 200 no-op vs 409

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **200 idempotent no-op** (chosen) | Safe for retries and “set status to current” UI | Slightly less strict than “reject unchanged” | **Chosen** — `TransitionAsync` returns early when `current == target` |
| 409 “already in status” | Explicit rejection | Noisy for dropdowns that re-submit current value | Rejected |

**Evidence:** 5 matrix pairs `(from == to)` expect **200**; all pass in `TicketStatusTransitionMatrixTests`.

### 4. Layering: services in Api vs separate Application project

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **Services in `SupportTickets.Api`** (chosen) | Faster scaffold; fewer projects for assessment | Api layer owns orchestration; harder to reuse without referencing Api | **Chosen** for speed |
| `backend/Application/` + interfaces | Clean dependency direction; matches enterprise templates | Extra project, DI wiring, doc drift when not built | Designed in `design-notes.md` but **not implemented** |

**Evidence:** `TicketService`, `TicketStatusTransitionService` live under `src/SupportTickets.Api/Services/`. Doc sync pass (#21) removed ghost Application layer from docs.

### 5. Transition rules: static `TransitionMap` vs state machine class

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **Static dictionary `TransitionMap`** (chosen) | Trivial to read; matrix tests import same source | No per-role rules; no transition metadata (reason codes) | **Chosen** — matches assessment rules exactly |
| `TicketStatusTransitionRules` / OO state machine | Extensible for roles, side effects | Overkill for 5 states and 5 forward edges | Rejected for scope |

**Evidence:** `TransitionMap.Allowed` has exactly 5 forward edges; matrix data derives expected outcomes from `CanTransition`.

### 6. Persistence: SQLite file vs SQL Server

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **SQLite file** `support-tickets.db` (chosen) | Zero install; portable; EF migrations work | No `DateTimeOffset` ORDER BY in SQL; weak write concurrency | **Chosen** — assessment requirement |
| SQL Server / PostgreSQL | Better concurrency and date ordering | Setup friction on clean machine | Rejected for demo friction |

**Evidence:** `TicketService.GetAllAsync` orders by `Id` in SQL then `CreatedAt` in memory — direct SQLite workaround documented in service comment.

### 7. Seed: EF `HasData` in migration vs runtime seeder

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **HasData in migration** (chosen) | Seed applies with `Migrate()`; reproducible | Does not re-run on every startup; IDs fixed in migration | **Chosen** |
| `IDataSeeder` on startup | Can refresh demo data | Duplicate risk; harder to diff | Rejected |
| Standalone `schema.sql` | Visible SQL for reviewers | Duplicates EF model; user **reverted** schema files | Rejected (prompt #15 undo) |

**Evidence:** `DataSeeder.cs` — 10 users (4 roles), 5 tickets (one per status), 2 comments. `POST /api/tickets` returns seed + created tickets on `GET /api/tickets` (manual smoke ✅).

### 8. Missing ticket on comment POST: explicit 404 vs FK 500

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **`EnsureTicketExistsAsync` + catch** (chosen) | Correct HTTP semantics; stable for clients | Duplicated check pattern per controller | **Chosen** |
| Rely on FK constraint | Less code | `DbUpdateException` → **500** | Rejected |

**Evidence:** `POST /api/tickets/99999/comments` → **404** with `"Ticket with id 99999 was not found."` (see `debugging-notes.md`). Manual smoke ✅.

### 9. Integration test host: shared `:memory:` connection vs file per run

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **Singleton `SqliteConnection` + `:memory:`** (chosen) | Fast; no file cleanup | Must disable parallel tests; must `RemoveAll` DbContext registrations | **Chosen** |
| Unique file per test | Isolation | Slower; path hygiene | Rejected |

**Evidence:** `SupportTicketsWebApplicationFactory` + `StatusTransitionMatrixCollection` with `DisableParallelization`. Initial failures (404 on PATCH) traced to wrong DbContext registration and JSON `id` not deserializing (camelCase) — fixed before green run.

### 10. Concurrency token on `Ticket`

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **No row version** (current) | Simple entity; fast assessment | Lost updates on concurrent PUT + PATCH (H1/H2 in review) | **Deferred** — documented in `code-review-notes.md`, open in `review-fixes.md` |
| `[Timestamp]` / `RowVersion` | EF throws `DbUpdateConcurrencyException` | Migration + client must send token | Not implemented |

**Evidence:** Review accepted H1/H2; no fix applied yet. Acceptable for single-user demo; not acceptable for multi-agent production without follow-up.

### 11. Frontend vs users API vs integration tests (time allocation)

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **API + matrix tests first** (chosen) | Proves core assessment (state machine) automatically | No React demo; `GET /api/users` still missing for dropdowns | **Chosen** when frontend scaffold interrupted |
| Frontend next | Visible stakeholder demo | Dropdowns need users endpoint; more moving parts | Deferred — `src/frontend/` absent |
| Users API before UI | Unblocks create/assign forms | Less visible than UI screenshots | Still outstanding |

**Evidence:** Acceptance criteria — frontend ⬜, users API ⬜, testing ✅ (25 matrix tests).

---

## Demo evidence (reproducible)

### Automated

| Check | Command / location | Result |
|-------|-------------------|--------|
| Full transition matrix | `dotnet test tests/SupportTickets.IntegrationTests` | **25 passed, 0 failed** (2026-07-27) |
| Valid transitions | Matrix tests | 5 × **200** |
| Same-status no-op | Matrix tests | 5 × **200** |
| Invalid transitions | Matrix tests | 15 × **409** + extension fields |
| Solution build | `dotnet build src/SupportTickets.sln` | ✅ clean (manual smoke) |

### Manual API smoke

| Request | Expected | Observed |
|---------|----------|----------|
| `GET /api/health` | 200 | ✅ |
| `GET /api/tickets` | 200, seed + new tickets | ✅ |
| `PATCH` valid transition | 200 | ✅ |
| `PATCH` invalid transition | 409 | ✅ |
| `POST` comment on missing ticket | 404 (not 500) | ✅ |

### Seed snapshot (first run after migrate)

| Entity | Count | Purpose |
|--------|-------|---------|
| Users | 10 | Customers, Agents, Leads, Admins — assignee dropdown data (API list not yet exposed) |
| Tickets | 5 | One per status: Open, InProgress, Resolved, Closed, Cancelled |
| Comments | 2 | On InProgress and Resolved tickets |

### Operational friction (honest)

| Issue | Symptom | Workaround |
|-------|---------|------------|
| API process locking DLLs | `CS2012` on `dotnet test` / build | Stop running `SupportTickets.Api` before build |
| Slow NuGet on first test restore | Long restore with retries | Normal on cold machine; packages cached afterward |

---

## What was challenging

- **Documentation drift:** Design described `Application/`, `UsersController`, `allowedNextStatuses` on ticket JSON before code existed. Required explicit “sync docs to code” pass.
- **SQLite limitations:** `DateTimeOffset` sort and write locking — worked around in list query, noted for concurrency.
- **Integration test wiring:** Wrong DbContext after `ConfigureTestServices` and camelCase JSON deserialization (`id` → `0`) caused misleading 404s until factory and `JsonSerializerOptions` were fixed.
- **Incomplete UI:** Largest gap for a visual demo; API and tests prove backend assessment criteria.

---

## What I would do differently

1. **`GET /api/users` before any UI** — create/assign forms need it; seed data already exists.
2. **Single `TicketResponse` mapper** from day one — avoids PATCH vs GET field drift (M3 in review).
3. **Integration test project in the same PR as PATCH status** — matrix would have caught DbContext/JSON issues earlier.
4. **`[Timestamp]` on `Ticket`** if demonstrating concurrent agents — even one integration test for lost update would document the trade-off.
5. **Docs tagged “implemented” vs “planned”** in headers — reduces sync churn.

---

## Key learnings

- **400 vs 409:** Shape/validation → 400; lifecycle rule violation → 409 with machine-readable extensions. Clients and tests can branch on status code.
- **Don’t use FK errors as API control flow** — always validate existence in the service; map domain exceptions in controllers (or one filter).
- **Keep docs tied to `src/` paths** — `SupportTickets.*` vs planned `backend/` layout caused repeated confusion.
- **Test host must replace EF registration completely** — `RemoveAll<DbContextOptions<AppDbContext>>` and `RemoveAll<AppDbContext>`; partial removal leaves file DB and flaky 404s.
- **Match API JSON options in tests** — `PropertyNameCaseInsensitive` + `JsonStringEnumConverter` for `ReadFromJsonAsync`.
- **Idempotent same-status PATCH** simplifies UI and accounts for 5 of 25 matrix cases — worth deciding explicitly, not by accident.

---

## Current gaps (for next iteration)

Tracked in `review-fixes.md` and acceptance criteria:

- `GET /api/users` — not implemented
- `allowedNextStatuses` on `TicketResponse` JSON — only on 409 today
- Optimistic concurrency (H1/H2) — open
- React frontend — not started
