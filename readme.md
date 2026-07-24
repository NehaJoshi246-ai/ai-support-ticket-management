# AI Support Ticket Management

Support ticket management system with ASP.NET Core Web API, React frontend, EF Core 8, and integration tests.

## Stack

| Layer | Technology |
|-------|------------|
| Backend | ASP.NET Core Web API |
| Frontend | React |
| Database | EF Core 8 |
| Testing | Integration Tests |

## Documentation

| Document | Purpose |
|----------|---------|
| [candidate-info.md](candidate-info.md) | Candidate / project ownership info |
| [tool-workflow.md](tool-workflow.md) | Tools and AI-assisted workflow |
| [requirements-analysis.md](requirements-analysis.md) | Requirements breakdown |
| [acceptance-criteria.md](acceptance-criteria.md) | Acceptance criteria |
| [implementation-plan.md](implementation-plan.md) | Implementation plan |
| [design-notes.md](design-notes.md) | Architecture and design decisions |
| [api-contract.md](api-contract.md) | API contract |
| [data-model.md](data-model.md) | Data model |
| [ui-flow.md](ui-flow.md) | UI flows |
| [test-strategy.md](test-strategy.md) | Test strategy |
| [test-results.md](test-results.md) | Test results |
| [debugging-notes.md](debugging-notes.md) | Debugging notes |
| [code-review-notes.md](code-review-notes.md) | Code review notes |
| [review-fixes.md](review-fixes.md) | Fixes from review |
| [pr-description.md](pr-description.md) | Pull request description |
| [reflection.md](reflection.md) | Reflection |
| [final-ai-usage-summary.md](final-ai-usage-summary.md) | Final AI usage summary |

## Getting started

Local setup for a clean machine with **.NET 8 SDK** and **Node.js** only.  
See [database/setup-notes.md](database/setup-notes.md) for database details.

### 1. Prerequisites

- .NET SDK version: _[e.g. 8.0.x — fill after verifying]_
- Node.js version: _[e.g. 20 LTS — fill after verifying]_
- Optional: `dotnet-ef` global tool (only if applying migrations manually)
- OS notes: _[any Windows/macOS/Linux caveats]_

### 2. Clone the repository

- Clone URL / branch
- Working directory name

### 3. Backend (ASP.NET Core API)

- Restore & build the solution (`src/SupportTickets.sln`)
- Where config lives (`appsettings.json`, optional `appsettings.Development.json`)
- Run the API project (`src/SupportTickets.Api`)
- Default URL / Swagger: _[e.g. http://localhost:5189/swagger]_
- Note: migrations run automatically on startup; SQLite file created on first run

### 4. Database (SQLite)

- Pointer to [database/setup-notes.md](database/setup-notes.md)
- One-line summary: no separate DB install; file `support-tickets.db` beside the API
- How to reset local data (delete `.db` file and restart)

### 5. Frontend (React)

- App location: _[e.g. `src/frontend/ticket-ui` when present]_
- Install dependencies (`npm install` / `npm ci`)
- API base URL / dev proxy / `.env` variable name
- Run dev server
- Default UI URL: _[e.g. http://localhost:5173]_

### 6. Verify it works

- API health or Swagger smoke check
- `GET /api/users` returns seeded users
- `GET /api/tickets` returns sample tickets
- UI loads ticket list

### 7. Tests (optional)

- Integration test project location: _[e.g. `tests/`]_
- Command to run tests
- Note on test database (in-memory / separate SQLite file)

### 8. Common issues

- Port already in use
- CORS / wrong API URL in frontend
- Missing `support-tickets.db` (first run not completed)
- `dotnet-ef` not found (only needed for manual migrations)

### 9. Project layout (quick reference)

```
src/
  SupportTickets.sln
  SupportTickets.Api/          # Web API entry point
  SupportTickets.Domain/       # Entities, enums, TransitionMap
  SupportTickets.Infrastructure/ # EF Core, migrations, seed
  frontend/ticket-ui/          # React app (when present)
tests/                         # Integration tests
database/                      # Setup notes, schema, seed reference
```
