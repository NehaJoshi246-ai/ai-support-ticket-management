# AI Prompts — Planning

## Purpose

Prompts used for requirements analysis, acceptance criteria, implementation planning, and phase tracking.  
Update this file when scope or completion status changes.

---

## Project snapshot

| Item | Value |
|------|--------|
| **Option** | Support Ticket Management (assessment) |
| **Stack** | ASP.NET Core Web API, React, EF Core 8 + SQLite, integration tests |
| **Solution** | `src/SupportTickets.sln` |
| **API** | `src/SupportTickets.Api` |
| **Domain** | `src/SupportTickets.Domain` |
| **Infrastructure** | `src/SupportTickets.Infrastructure` |

---

## Scope (locked)

### Entities

- **User** — seeded only (no write APIs)
- **Ticket** — Id, Title, Description, Priority, Status, AssignedTo, CreatedBy, CreatedAt
- **TicketComment** — nested under tickets

### Status state machine

```
Open → InProgress | Cancelled
InProgress → Resolved | Cancelled
Resolved → Closed
```

- Status changes **only** via `PATCH /api/tickets/{id}/status`
- Invalid transitions → **409 Conflict** (`InvalidTransitionException`)
- Field updates via `PUT /api/tickets/{id}` (no status field)

---

## Phase status

| Phase | Status | Notes |
|-------|--------|-------|
| 0 — Repo staging | ✅ Done | Root docs, folders, `.gitignore` |
| 1 — Requirements & design | ✅ Done | `requirements-analysis.md`, `design-notes.md`, `api-contract.md`, `data-model.md`, `ui-flow.md` |
| 2 — Backend | 🟡 Mostly done | See checklist below |
| 3 — Frontend | ⬜ Not started | `src/frontend/ticket-ui/` not scaffolded |
| 4 — Integration tests | ⬜ Not started | `tests/` empty |
| 5 — Review & delivery | 🟡 In progress | `code-review-notes.md` written; fixes pending |

### Backend checklist

| Item | Status |
|------|--------|
| Solution scaffold (Api, Domain, Infrastructure) | ✅ |
| Entities, enums, `TransitionMap` | ✅ |
| SQLite + `AppDbContext` + migrations | ✅ |
| Seed: 10 users, 5 tickets, 2 comments | ✅ |
| `GET/POST` tickets, `GET` by id, `PUT` update | ✅ |
| `PATCH /api/tickets/{id}/status` | ✅ |
| `GET/POST /api/tickets/{id}/comments` | ✅ |
| `GET /api/users` (for UI dropdowns) | ⬜ Not implemented yet |
| README setup commands filled in | ⬜ Outline only |
| Review fixes (concurrency, mapping, etc.) | ⬜ See `code-review-notes.md` |

---

## Prompts (historical)

### Repository staging

```
Create the required root documentation structure for a support ticket management system
(ASP.NET Core Web API, React, EF Core 8, integration tests) before any application code.
```

### Requirements & plan

```
Based on the project stack and staged docs, refine requirements-analysis.md,
acceptance-criteria.md, and implementation-plan.md for a support ticket system.
```

### Assessment capture

```
Support management system assessment option.
Entities: User (seeded only); Ticket(id, title, description, priority, status,
assigned to, created by, created at).
Status state machine: Open → In Progress | Cancelled;
In Progress → Resolved | Cancelled; Resolved → Closed.
Update requirements and design docs accordingly.
```

### Design session (layered API)

```
ASP.NET Core Web API + EF Core SQLite + React.
Thin controllers; business logic in services; TransitionMap for status rules.
Dedicated PATCH /api/tickets/{id}/status — not general PUT.
Document folder structure in design-notes.md and api-contract.md.
```

---

## Prompts (next — use when ready)

### Users API (blocking frontend forms)

```
Add GET /api/users returning seeded users (id, name, email, role).
Thin UsersController + UserService. Register in DI. Match api-contract.md.
```

### Frontend scaffold

```
Scaffold React + TypeScript (Vite) at src/frontend/ticket-ui/.
Pages: ticket list (search + status filter), detail (comments + status dropdown),
create form. Use allowedNextStatuses from ticket API for status options.
Loading, empty, and error states on every page. CORS already enabled on API.
```

### Integration tests — state machine matrix

```
Before writing tests, list all 25 (fromStatus, toStatus) pairs for review.
Then parameterized WebApplicationFactory tests: valid → 200, invalid → 409.
Record results in test-results.md.
```

### README setup (fill commands)

```
Fill readme.md Getting started sections with exact commands for clean machine:
.NET 8 SDK + Node only. Backend run, optional frontend, verify smoke checks.
```

### Review fixes

```
Work through code-review-notes.md priority list: concurrency token on Ticket,
shared TicketResponse mapper, service-layer validation guards.
Log resolutions in review-fixes.md.
```

---

## Outcomes

### Completed

- Root documentation structure and assessment scope documented.
- Design: layered backend, SQLite, dedicated status endpoint, comment nesting.
- Backend MVP: tickets CRUD (fields), status transitions, comments, seed data.
- `database/setup-notes.md` — connection string, migrations, seed, persistence.
- `debugging-notes.md` — comment 404 vs 500 investigation.
- `code-review-notes.md` — TicketService / transition service review with rejected findings.
- `final-ai-usage-summary.md` — **prompt iteration log with failures and course corrections**.

### Remaining

- `GET /api/users`
- React frontend (`ui-flow.md`)
- Integration tests (`test-strategy.md` + transition matrix)
- Review fixes from `code-review-notes.md`
- Delivery docs: `pr-description.md`, `reflection.md`, `final-ai-usage-summary.md`

---

## Key doc links

| Doc | Use when planning |
|-----|-------------------|
| [requirements-analysis.md](../requirements-analysis.md) | Scope / out of scope |
| [implementation-plan.md](../implementation-plan.md) | Phase detail |
| [acceptance-criteria.md](../acceptance-criteria.md) | Definition of done |
| [api-contract.md](../api-contract.md) | Endpoint shapes |
| [ui-flow.md](../ui-flow.md) | Frontend screens |
| [test-strategy.md](../test-strategy.md) | Test coverage |
| [database/setup-notes.md](../database/setup-notes.md) | Local DB setup |
