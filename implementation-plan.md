# Implementation Plan

## Phase 0 — Repository staging ✅

Root docs, `src/`, `tests/`, `database/`, `ai-prompts/`, `.gitignore`.

## Phase 1 — Design ✅

Assessment scope, API contract, data model, UI flows documented.

## Phase 2 — Backend ✅ (mostly)

### Done

| Item | Location |
|------|----------|
| Solution `src/SupportTickets.sln` | Api + Domain + Infrastructure |
| Entities: User, Ticket, TicketComment | `SupportTickets.Domain` |
| Enums: TicketStatus, TicketPriority, UserRole | Domain |
| `TransitionMap`, domain exceptions | Domain |
| `AppDbContext`, configurations, migration | Infrastructure |
| Seed: 10 users, 5 tickets, 2 comments | `DataSeeder` |
| `TicketService` — list, get, create, update | Api/Services |
| `TicketStatusTransitionService` — PATCH logic | Api/Services |
| `TicketCommentService` — list, add | Api/Services |
| Controllers + validation + CORS + Swagger | Api |
| Auto-migrate on startup | `Program.cs` |

### Remaining backend

- [ ] `GET /api/users` + `UserService` (needed for UI dropdowns)
- [ ] Review fixes from `code-review-notes.md` (concurrency, shared mapper)
- [ ] Optional: `allowedNextStatuses` on `TicketResponse`

## Phase 3 — Frontend ⬜

- [ ] Scaffold `src/frontend/ticket-ui/` (Vite + React + TS)
- [ ] List (search + status filter), detail (comments + status), create form
- [ ] Loading / empty / error states
- [ ] Dev proxy → `http://localhost:5189`

## Phase 4 — Integration tests ⬜

- [ ] Test project under `tests/`
- [ ] `WebApplicationFactory` + SQLite test DB
- [ ] Ticket API + comment API + 25-pair transition matrix
- [ ] Record in `test-results.md`

## Phase 5 — Review & delivery 🟡

- [x] `code-review-notes.md`, `debugging-notes.md`
- [ ] `review-fixes.md` applied
- [ ] `pr-description.md`, `reflection.md`, `final-ai-usage-summary.md`

---

## Build order (actual progress)

| Step | Task | Status |
|------|------|--------|
| 1 | Domain + Infrastructure + migration + seed | ✅ |
| 2 | Ticket GET/POST/PUT | ✅ |
| 3 | PATCH status + TransitionMap | ✅ |
| 4 | Comments GET/POST | ✅ |
| 5 | GET /api/users | ⬜ |
| 6 | React UI | ⬜ |
| 7 | Integration tests | ⬜ |

## Definition of done (MVP)

- [x] SQLite + seeded data
- [x] Ticket field CRUD via REST
- [x] Status **only** via `PATCH /api/tickets/{id}/status`
- [x] Comments nested under tickets
- [ ] `GET /api/users`
- [ ] React UI per `ui-flow.md`
- [ ] Integration tests pass
