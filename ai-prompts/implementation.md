# AI Prompts — Implementation

## Purpose

Backend (and planned frontend) implementation prompts and outcomes.

## Prompts used

### Backend scaffold

```
Scaffold SupportTickets.Api, SupportTickets.Domain, SupportTickets.Infrastructure
under src/ with EF Core 8 SQLite, entities, migration, seed.
```

### Ticket endpoints

```
POST/GET list/GET id/PUT for tickets with field-level 400 validation.
PUT must not accept status.
```

### Status transition

```
PATCH /api/tickets/{id}/status with TransitionMap.
InvalidTransitionException → 409 with from/to/allowed list in ProblemDetails.
```

### Comments

```
GET/POST /api/tickets/{ticketId}/comments.
404 if ticket missing; require non-empty body.
```

## Outcomes ✅

| Component | File(s) |
|-----------|---------|
| Solution | `src/SupportTickets.sln` |
| Tickets API | `TicketsController`, `TicketService` |
| Status API | `TicketsController.Patch`, `TicketStatusTransitionService` |
| Comments API | `TicketCommentsController`, `TicketCommentService` |
| Domain rules | `TransitionMap`, `InvalidTransitionException` |
| Seed | `DataSeeder` — 10 users, 5 tickets, 2 comments |
| Health | `HealthController` |

## Not implemented ⬜

- `GET /api/users`
- `src/frontend/ticket-ui/`
- `tests/` integration project
- Application layer separation (services remain in Api)

## Next implementation prompts

```
Add GET /api/users (UserService, UsersController).
Mirror api-contract.md user response with Role enum.
```

```
Scaffold Vite React TS at src/frontend/ticket-ui per ui-flow.md.
```
