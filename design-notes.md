# Design Notes

## Goals

- Deliver the assessment **Support Ticket Management** option with a maintainable split between API, business logic, persistence, and UI.
- Keep controllers thin; put rules (especially status transitions) in a service layer.
- Use EF Core 8 with **SQLite** for local persistence.
- Expose a REST API consumed by a **React** SPA.

## System overview

```
React SPA (src/frontend)
        │  REST / JSON
        ▼
ASP.NET Core Web API (src/backend/Api)
        │  calls interfaces
        ▼
Application services (src/backend/Application)
        │  uses entities / rules
        ▼
Domain (src/backend/Domain)
        │
        ▼
Infrastructure — EF Core + SQLite (src/backend/Infrastructure)
```

## Proposed `src/` layout

```
src/
├── SupportTicketManagement.sln
│
├── backend/
│   ├── Api/                              # HTTP surface only
│   │   ├── Controllers/
│   │   │   ├── TicketsController.cs
│   │   │   ├── TicketCommentsController.cs
│   │   │   └── UsersController.cs
│   │   ├── Middleware/                   # Optional global exception handling
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── Application/                      # Use cases + orchestration
│   │   ├── DTOs/
│   │   │   ├── Tickets/
│   │   │   ├── Comments/
│   │   │   └── Users/
│   │   ├── Interfaces/
│   │   │   ├── ITicketService.cs
│   │   │   ├── ITicketCommentService.cs
│   │   │   ├── ITicketStatusTransitionService.cs
│   │   │   └── IUserService.cs
│   │   ├── Services/
│   │   │   ├── TicketService.cs
│   │   │   ├── TicketCommentService.cs
│   │   │   ├── TicketStatusTransitionService.cs
│   │   │   └── UserService.cs
│   │   └── Mapping/                      # Entity ↔ DTO mapping (manual or Mapster)
│   │
│   ├── Domain/                           # Core model + invariants
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Ticket.cs
│   │   │   └── TicketComment.cs
│   │   ├── Enums/
│   │   │   ├── TicketStatus.cs
│   │   │   └── TicketPriority.cs
│   │   ├── Rules/
│   │   │   └── TicketStatusTransitionRules.cs   # Allowed next-status map
│   │   └── Exceptions/
│   │       ├── InvalidStatusTransitionException.cs
│   │       └── NotFoundException.cs
│   │
│   └── Infrastructure/                   # Persistence + seeding
│       ├── Data/
│       │   ├── AppDbContext.cs
│       │   └── Configurations/           # IEntityTypeConfiguration per entity
│       ├── Migrations/
│       └── Seed/
│           └── UserSeed.cs
│
└── frontend/
    └── ticket-ui/                        # React app (Vite or CRA)
        └── src/
            ├── api/                      # fetch wrappers per resource
            ├── components/
            ├── pages/
            │   ├── TicketListPage.tsx
            │   ├── TicketCreatePage.tsx
            │   └── TicketDetailPage.tsx
            ├── hooks/
            └── types/                    # TS types mirroring API DTOs
```

### Layer responsibilities

| Layer | Owns | Does not own |
|-------|------|--------------|
| **Api** | Routing, HTTP status codes, binding request/response DTOs | Business rules, EF queries |
| **Application** | Ticket/comment workflows, validation orchestration, DTO mapping | HTTP concerns, DbContext details |
| **Domain** | Entities, enums, transition rules, domain exceptions | Database access |
| **Infrastructure** | DbContext, migrations, SQLite connection, seed data | HTTP or UI logic |

Controllers should delegate immediately to application services and translate domain exceptions to HTTP responses (e.g. invalid transition → 400, not found → 404).

## Status transition design

Status changes are **not** accepted on the general ticket update endpoint. They go through a dedicated action:

`PATCH /api/tickets/{id}/status`

This keeps field edits and lifecycle moves separate:

- **TicketService** — create, read, list, update title/description/priority/assignee.
- **TicketStatusTransitionService** — load ticket, ask `TicketStatusTransitionRules` if the move is allowed, persist new status, return updated ticket.

The rules live in Domain (`TicketStatusTransitionRules`); the service applies them and throws `InvalidStatusTransitionException` when rejected.

```
Open → InProgress | Cancelled
InProgress → Resolved | Cancelled
Resolved → Closed
```

Closed and Cancelled are terminal.

## Comments design

Comments are child records on a ticket — useful for support notes during In Progress / Resolved work.

- Stored as `TicketComment` with FK to `Ticket` and `User` (author).
- Listed and created via nested routes under a ticket.
- Comments do not change ticket status; status still uses the dedicated PATCH endpoint.

## Persistence

- **Provider:** SQLite (single file, e.g. `support-tickets.db`, under backend or `database/`).
- **ORM:** EF Core 8 with fluent configurations in Infrastructure.
- **Users:** seeded on startup or via migration seed; read-only through API.

## Frontend design

- React SPA talks to the API over REST (JSON).
- Central `api/` module per resource (`tickets`, `comments`, `users`).
- Ticket detail page combines:
  - field edit form → `PUT /api/tickets/{id}`
  - status action buttons/dropdown → `PATCH /api/tickets/{id}/status`
  - comment thread → `GET/POST /api/tickets/{id}/comments`
- UI only offers valid next statuses; server remains authoritative.

## API surface (summary)

See `api-contract.md` for full request/response shapes.

| Resource | Endpoints |
|----------|-----------|
| Users | `GET /api/users` |
| Tickets | `GET/POST /api/tickets`, `GET/PUT /api/tickets/{id}`, `PATCH /api/tickets/{id}/status` |
| Comments | `GET/POST /api/tickets/{ticketId}/comments` |

## Decisions log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2026-07-24 | Assessment option: Support Ticket Management | Assignment scope |
| 2026-07-24 | Users seeded only | No user write APIs |
| 2026-07-24 | Status via dedicated PATCH endpoint | Explicit validation path, thin controllers |
| 2026-07-24 | Business logic in Application services | Clean separation from HTTP layer |
| 2026-07-24 | SQLite for local dev | Simple single-file persistence |
| 2026-07-24 | Comments as nested ticket resource | Support notes without coupling to status updates |
| 2026-07-24 | General PUT excludes status | Prevents accidental lifecycle bypass |

## Alternatives considered

- **Status on general PUT** — rejected; dedicated endpoint makes transition validation explicit and testable.
- **Fat controllers** — rejected; services keep rules reusable for integration tests.
- **Repository layer** — deferred; services can use `AppDbContext` directly until complexity warrants abstraction.
