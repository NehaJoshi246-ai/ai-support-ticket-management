# AI Support Ticket Management

Support ticket management system — ASP.NET Core Web API backend (implemented), React frontend and integration tests (planned).

## Stack (current)

| Layer | Technology | Status |
|-------|------------|--------|
| API | ASP.NET Core 8 (`SupportTickets.Api`) | ✅ |
| Domain | `SupportTickets.Domain` | ✅ |
| Database | EF Core 8 + SQLite | ✅ |
| Frontend | React | ⬜ Not started |
| Tests | Integration tests | ⬜ Not started |

## Documentation

| Document | Purpose |
|----------|---------|
| [requirements-analysis.md](requirements-analysis.md) | Scope and requirements |
| [acceptance-criteria.md](acceptance-criteria.md) | Done / not done checklist |
| [implementation-plan.md](implementation-plan.md) | Phase tracking |
| [design-notes.md](design-notes.md) | Architecture (as built) |
| [api-contract.md](api-contract.md) | REST endpoints |
| [data-model.md](data-model.md) | Entities and enums |
| [ui-flow.md](ui-flow.md) | Target UI flows |
| [database/setup-notes.md](database/setup-notes.md) | SQLite setup |
| [test-strategy.md](test-strategy.md) | Test plan |
| [code-review-notes.md](code-review-notes.md) | Service review |
| [debugging-notes.md](debugging-notes.md) | Debug log |

## Getting started

See [database/setup-notes.md](database/setup-notes.md) for DB details.

### Prerequisites

- .NET 8 SDK
- Node.js (for frontend when added)
- Optional: `dotnet-ef` for manual migrations

### Backend (implemented)

```bash
cd src
dotnet build SupportTickets.sln
dotnet run --project SupportTickets.Api
```

- Swagger: `http://localhost:5189/swagger` (Development)
- SQLite file: `src/SupportTickets.Api/support-tickets.db` (created on first run)
- Migrations apply automatically on startup

### Verify API

```bash
curl http://localhost:5189/api/health
curl http://localhost:5189/api/tickets
```

### Frontend

Not in repo yet — planned at `src/frontend/ticket-ui/`.

### Tests

Not in repo yet — planned under `tests/`.

## Project layout

```
src/
  SupportTickets.sln
  SupportTickets.Api/           # Controllers, Services, DTOs
  SupportTickets.Domain/        # Entities, TransitionMap, exceptions
  SupportTickets.Infrastructure/ # EF Core, migrations, seed
tests/                          # (empty — integration tests planned)
database/                       # setup-notes, schema placeholder
ai-prompts/                     # Prompt log by phase
```

## API quick reference

| Method | Path |
|--------|------|
| GET | `/api/health` |
| GET/POST | `/api/tickets` |
| GET/PUT | `/api/tickets/{id}` |
| PATCH | `/api/tickets/{id}/status` |
| GET/POST | `/api/tickets/{id}/comments` |

`GET /api/users` — not implemented yet.
