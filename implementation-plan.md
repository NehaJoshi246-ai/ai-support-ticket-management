# Implementation Plan

## Phase 0 — Repository staging ✅

- Root docs, `src/`, `tests/`, `database/`, `ai-prompts/`, `.gitignore`.

## Phase 1 — Design ✅ (current)

- Assessment scope locked: User (seeded), Ticket, Comments.
- Layered backend layout, SQLite, dedicated status endpoint documented.
- See `design-notes.md`, `data-model.md`, `api-contract.md`, `ui-flow.md`.

## Phase 2 — Backend solution

### 2.1 Scaffold solution

1. Create `src/SupportTicketManagement.sln`.
2. Add projects:
   - `Api` — ASP.NET Core Web API
   - `Application` — class library
   - `Domain` — class library
   - `Infrastructure` — class library
3. Wire project references: Api → Application + Infrastructure; Application → Domain; Infrastructure → Domain + Application (if needed for DI registration only — prefer Infrastructure → Domain).

### 2.2 Domain layer

1. Add entities: `User`, `Ticket`, `TicketComment`.
2. Add enums: `TicketStatus`, `TicketPriority`.
3. Add `TicketStatusTransitionRules` with allowed-next map.
4. Add domain exceptions: `InvalidStatusTransitionException`, `NotFoundException`.

### 2.3 Infrastructure layer

1. Add `AppDbContext` + entity configurations.
2. Configure **SQLite** connection string in `appsettings.Development.json`.
3. Create initial migration.
4. Implement `UserSeed` (2–3 sample users).
5. Register DbContext in `Program.cs`.

### 2.4 Application layer

1. Define DTOs (ticket, comment, user, status transition request).
2. Implement services:
   - `TicketService` — create, get, list, update (excludes status)
   - `TicketStatusTransitionService` — PATCH logic + rule enforcement
   - `TicketCommentService` — list/create comments for a ticket
   - `UserService` — list seeded users
3. Register services in DI.

### 2.5 API layer

1. Thin controllers delegating to services:
   - `TicketsController`
   - `TicketCommentsController` (nested route)
   - `UsersController`
2. Map exceptions to HTTP status codes.
3. Enable CORS for React dev server.
4. Swagger in Development.

**Deliverable:** API runnable locally; tickets, status transitions, comments, users all working against SQLite.

## Phase 3 — Frontend

1. Scaffold React app at `src/frontend/ticket-ui/` (Vite + TypeScript recommended).
2. Add `api/` modules for users, tickets, comments.
3. Build pages:
   - Ticket list
   - Create ticket
   - Ticket detail (field form + status actions + comment thread)
4. Filter status actions per state machine.
5. Configure dev proxy or env var for API base URL.

**Deliverable:** End-to-end flows from UI through REST to SQLite.

## Phase 4 — Integration tests (`tests/`)

1. Add test project referencing Api + Infrastructure.
2. Use `WebApplicationFactory` with SQLite in-memory or test file DB.
3. Cover:
   - User seed + list
   - Ticket create/list/get/update
   - Valid and invalid status transitions via `PATCH …/status`
   - PUT rejects status field
   - Comment list/create
4. Record results in `test-results.md`.

## Phase 5 — Review & delivery

1. Self-review → `code-review-notes.md`.
2. Fixes → `review-fixes.md`.
3. Update `pr-description.md`, `reflection.md`, `final-ai-usage-summary.md`.

---

## Suggested build order (first vertical slice)

| Step | Task |
|------|------|
| 1 | Solution + Domain entities/enums/rules |
| 2 | Infrastructure DbContext, migration, user seed |
| 3 | `TicketService` + `TicketsController` (GET list, GET by id, POST) |
| 4 | `TicketStatusTransitionService` + PATCH endpoint + transition tests |
| 5 | PUT update (no status) + assignment |
| 6 | `TicketCommentService` + nested comment routes |
| 7 | React list + create + detail with status actions and comments |
| 8 | Remaining integration tests + docs |

## Definition of done (MVP)

- [ ] Layered backend under `src/backend/` with thin controllers.
- [ ] SQLite persistence with seeded users.
- [ ] Ticket CRUD fields via REST; status **only** via `PATCH /api/tickets/{id}/status`.
- [ ] Comments on tickets via nested REST routes.
- [ ] React UI implements flows in `ui-flow.md`.
- [ ] Integration tests pass; results logged.
