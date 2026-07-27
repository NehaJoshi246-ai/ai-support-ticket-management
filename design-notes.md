# Design Notes

## Goals

- Support Ticket Management assessment with clear separation: HTTP controllers, services, domain rules, EF persistence.
- SQLite for local dev; React SPA planned but not yet built.

## Implemented architecture

```
React SPA (planned: src/frontend/ticket-ui)
        │  REST / JSON
        ▼
SupportTickets.Api
  ├── Controllers/     TicketsController, TicketCommentsController, HealthController
  ├── Services/      TicketService, TicketStatusTransitionService, TicketCommentService
  ├── DTOs/          Request/response + validation attributes
  └── Validation/    AllowedTicketPriority, AllowedTicketStatus parsers
        │
        ▼
SupportTickets.Domain
  ├── Entities/      User, Ticket, TicketComment
  ├── Enums/         TicketStatus, TicketPriority, UserRole
  ├── Rules/         TransitionMap
  └── Exceptions/    NotFoundException, InvalidTransitionException, ValidationException
        │
        ▼
SupportTickets.Infrastructure
  ├── Data/          AppDbContext, fluent configurations
  ├── Migrations/    InitialCreate
  └── Seed/          DataSeeder (HasData in migration)
```

**Note:** Services live in the **Api** project today, not a separate Application layer. Domain has no EF references.

## Actual `src/` layout

```
src/
├── SupportTickets.sln
├── SupportTickets.Api/
│   ├── Controllers/
│   │   ├── TicketsController.cs
│   │   ├── TicketCommentsController.cs
│   │   └── HealthController.cs
│   ├── Services/
│   │   ├── TicketService.cs
│   │   ├── TicketStatusTransitionService.cs
│   │   └── TicketCommentService.cs
│   ├── DTOs/
│   ├── Validation/
│   └── Program.cs
├── SupportTickets.Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── Rules/TransitionMap.cs
│   └── Exceptions/
└── SupportTickets.Infrastructure/
    ├── Data/
    ├── Migrations/
    └── Seed/DataSeeder.cs
```

`src/frontend/` — **not present yet**.

## Status transition design (as built)

- **PUT** rejects `status` field (DTO `IValidatableObject` → 400).
- **PATCH** ` /api/tickets/{id}/status` body: `{ "status": "InProgress" }`.
- `TicketStatusTransitionService` checks `TransitionMap`; throws `InvalidTransitionException` → controller maps to **409** with:
  - `detail` — human-readable message with from/to and allowed list
  - extensions: `fromStatus`, `toStatus`, `allowedNextStatuses`
- Same from/to status → **200** idempotent no-op.

## Validation design (as built)

| Layer | Mechanism |
|-------|-----------|
| HTTP / DTO | Data annotations + custom `AllowedTicketPriority` / `AllowedTicketStatus` |
| Controller | `ValidationProblemDetails` for model state; catch domain `ValidationException` |
| Service | `EnsureUserExistsAsync` → `ValidationException` on bad user id |
| Missing entity | `NotFoundException` → 404 in controllers |

## API surface (implemented)

| Method | Path | Controller |
|--------|------|------------|
| GET | `/api/health` | HealthController |
| GET | `/api/tickets` | TicketsController |
| GET | `/api/tickets/{id}` | TicketsController |
| POST | `/api/tickets` | TicketsController |
| PUT | `/api/tickets/{id}` | TicketsController |
| PATCH | `/api/tickets/{id}/status` | TicketsController |
| GET | `/api/tickets/{ticketId}/comments` | TicketCommentsController |
| POST | `/api/tickets/{ticketId}/comments` | TicketCommentsController |

**Not implemented:** `GET /api/users`

## Persistence

- SQLite file: `support-tickets.db` (relative to API working directory).
- Connection: `appsettings.json` → `Data Source=support-tickets.db`.
- `Program.cs` calls `db.Database.Migrate()` on startup.

## Seed data (`DataSeeder`)

- 10 users (3 Customer, 3 Agent, 2 Lead, 2 Admin).
- 5 tickets covering all statuses.
- 2 comments on InProgress and Resolved tickets.

## Known gaps (see `code-review-notes.md`)

- No optimistic concurrency on `Ticket`.
- `TicketResponse` mapping duplicated in transition service vs `TicketService.Map`.
- No `allowedNextStatuses` on ticket JSON yet — frontend should use `TransitionMap` mirror or add to API.
- Services lack null-guard on request parameters.

## Decisions log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2026-07-24 | Assessment: Support Ticket Management | Assignment |
| 2026-07-24 | SQLite + EF Core 8 | Local assessment simplicity |
| 2026-07-24 | Services in Api project | Faster scaffold; Application layer deferred |
| 2026-07-24 | `TransitionMap` in Domain | Single rule source |
| 2026-07-24 | Invalid transition → 409 | Business rule conflict, not malformed input |
| 2026-07-24 | `InvalidTransitionException` with allowed list in message | Frontend-friendly errors |

## Alternatives not used

- Separate Application project — deferred.
- `GET /api/users` — planned, not built.
- Status on general PUT — rejected.
