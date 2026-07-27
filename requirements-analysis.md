# Requirements Analysis

## Assessment option

**Support Ticket Management System**

## Stack (as implemented)

| Layer | Technology | Location |
|-------|------------|----------|
| Backend | ASP.NET Core 8 Web API | `src/SupportTickets.Api` |
| Domain | Entities, enums, `TransitionMap` | `src/SupportTickets.Domain` |
| Persistence | EF Core 8 + SQLite | `src/SupportTickets.Infrastructure` |
| Frontend | React | **Not implemented yet** |
| Testing | Integration tests | **Not implemented yet** (`tests/` empty) |

## Entities (implemented)

### User (seeded only)

- Seeded via `DataSeeder` in migration `InitialCreate` (10 users with roles).
- **No** `GET /api/users` endpoint yet — clients must use known seed ids or add that endpoint before UI dropdowns.

### Ticket

| Field | Implementation |
|-------|----------------|
| Id | `int`, PK, identity |
| Title | `string`, max 200, required |
| Description | `string`, max 4000, required |
| Priority | `TicketPriority` enum |
| Status | `TicketStatus` enum; default `Open` on create |
| AssignedToId | FK → User, nullable |
| CreatedById | FK → User, required |
| CreatedAt | `DateTimeOffset`, set on create |

### TicketComment (implemented)

| Field | Implementation |
|-------|----------------|
| Id | `int`, PK |
| TicketId | FK → Ticket, cascade delete |
| Body | `string`, max 4000, required |
| CreatedById | FK → User |
| CreatedAt | `DateTimeOffset` |

## Status state machine

Enforced in `TransitionMap` + `TicketStatusTransitionService`; invalid moves throw `InvalidTransitionException` → HTTP **409**.

```
Open → InProgress | Cancelled
InProgress → Resolved | Cancelled
Resolved → Closed
```

| From | Allowed next |
|------|--------------|
| Open | InProgress, Cancelled |
| InProgress | Resolved, Cancelled |
| Resolved | Closed |
| Closed | Terminal |
| Cancelled | Terminal |

Same-status PATCH is treated as idempotent **200** (no-op).

## Functional requirements

| # | Requirement | Status |
|---|-------------|--------|
| 1 | Create tickets (Open, CreatedAt set server-side) | ✅ API |
| 2 | List / view tickets with assignee and creator names | ✅ API |
| 3 | Assign / reassign via `AssignedToId` | ✅ API |
| 4 | Update fields via PUT; status via PATCH only | ✅ API |
| 5 | EF Core 8 + SQLite + seed data | ✅ |
| 6 | Comments on tickets | ✅ API |
| 7 | React UI | ⬜ Not started |
| 8 | Integration tests | ⬜ Not started |

## Non-functional requirements (implemented)

- Field-level **400** validation via `ValidationProblemDetails` on DTOs.
- Invalid status transition → **409** with `fromStatus`, `toStatus`, `allowedNextStatuses` in ProblemDetails extensions.
- Missing ticket / comment context → **404** `ProblemDetails`.
- CORS enabled for local React dev.
- Swagger in Development.
- Auto-migrate on API startup.

## Out of scope (still)

- User registration / CRUD APIs.
- Auth / authorization.
- Ticket delete (use Cancelled).
- `GET /api/users` — planned for UI but **not coded yet**.

## Resolved (no longer open)

- **Priority values:** `Low`, `Medium`, `High`, `Critical`.
- **AssignedTo on create:** optional (`null` allowed).
- **CreatedBy:** client sends `createdById` from seeded users (no auth).
