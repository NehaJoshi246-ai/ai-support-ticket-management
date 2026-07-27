# Project Context (Cursor Workflow)

## Project

AI Support Ticket Management — assessment option.

## Stack (as implemented)

| Piece | Path / tech |
|-------|-------------|
| API | `src/SupportTickets.Api` — ASP.NET Core 8 |
| Domain | `src/SupportTickets.Domain` |
| EF + SQLite | `src/SupportTickets.Infrastructure` |
| Frontend | **Not started** |
| Tests | **Not started** (`tests/` empty) |

## Domain rules

- **User:** seeded only (10 users, `UserRole` enum)
- **Ticket:** Id, Title, Description, Priority, Status, AssignedToId, CreatedById, CreatedAt
- **TicketComment:** nested under tickets
- **TransitionMap:** Open→InProgress|Cancelled; InProgress→Resolved|Cancelled; Resolved→Closed
- Status via `PATCH /api/tickets/{id}/status` only; invalid → **409**

## API implemented

```
GET  /api/health
GET  /api/tickets, GET/PUT/POST /api/tickets/{id}
PATCH /api/tickets/{id}/status
GET/POST /api/tickets/{ticketId}/comments
```

**Missing:** `GET /api/users`

## Services (in Api project)

- `TicketService` — field CRUD
- `TicketStatusTransitionService` — PATCH status
- `TicketCommentService` — comments

## Current phase

Backend MVP done; next: users endpoint, frontend, integration tests, review fixes.

## Constraints

- Docs must match code (no fictional Application layer in instructions).
- SQLite local only; `support-tickets.db` gitignored.
- Record work in `ai-prompts/` and `final-ai-usage-summary.md`.
