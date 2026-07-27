# Acceptance Criteria

## Documentation staging

- [x] Required root documentation files exist.
- [x] Docs aligned with implemented code (this pass).

## Domain (implemented in code)

- [x] `User` seeded only; no user write APIs.
- [x] `User.Role` enum: Customer, Agent, Lead, Admin.
- [x] `Ticket` fields: Id, Title, Description, Priority, Status, AssignedTo, CreatedBy, CreatedAt.
- [x] `TicketComment` with TicketId, Body, CreatedById, CreatedAt.
- [x] New tickets start as **Open**.
- [x] Status transitions via `TransitionMap`:
  - Open → InProgress | Cancelled
  - InProgress → Resolved | Cancelled
  - Resolved → Closed
- [x] Invalid transitions return **409 Conflict** (not 400).
- [x] Closed and Cancelled are terminal.

## Backend (ASP.NET Core Web API)

- [x] Ticket endpoints: GET list, GET by id, POST, PUT (no status).
- [x] `PATCH /api/tickets/{id}/status` for lifecycle only.
- [x] Comment endpoints: GET/POST `/api/tickets/{ticketId}/comments`.
- [x] Assignment via `assignedToId` on create/update.
- [x] EF Core 8 + SQLite persistence.
- [x] Field-level 400 validation on create/update/comment bodies.
- [x] `GET /api/health` smoke endpoint.
- [ ] `GET /api/users` — **not implemented** (documented in api-contract, pending).
- [ ] API fully matches `api-contract.md` until users endpoint exists.

## Frontend (React)

- [ ] Not started (`src/frontend/` absent).

## Testing

- [x] Integration test project: `tests/SupportTickets.IntegrationTests/`
- [x] State machine matrix tests — 25 pairs via `TicketStatusTransitionMatrixTests`
- [x] Results recorded in `test-results.md`

## Delivery

- [x] `code-review-notes.md` — self-review of services.
- [x] `debugging-notes.md` — comment 404 investigation.
- [ ] `review-fixes.md` — fixes from review not applied.
- [x] `reflection.md` — trade-offs and demo evidence.
- [x] `final-ai-usage-summary.md` — iteration log + link to raw transcripts.
