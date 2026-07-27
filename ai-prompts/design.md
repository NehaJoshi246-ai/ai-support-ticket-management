# AI Prompts — Design

## Purpose

Architecture, API contract, and data model design prompts and outcomes.

## Prompts used

### Design pack

```
Using requirements-analysis.md and acceptance-criteria.md, draft design-notes.md,
api-contract.md, data-model.md, and ui-flow.md for the ticket management system.
```

### Layered API + SQLite

```
ASP.NET Core Web API + EF Core SQLite + React.
Thin controllers; TransitionMap; PATCH /api/tickets/{id}/status separate from PUT.
```

## Outcomes (as built vs planned)

| Planned | Actual |
|---------|--------|
| `backend/Api`, `Application`, `Domain`, `Infrastructure` | `SupportTickets.Api` (includes Services), Domain, Infrastructure |
| `TicketStatusTransitionRules` | `TransitionMap` |
| `InvalidStatusTransitionException` | `InvalidTransitionException` |
| `UsersController` | Not implemented |
| `allowedNextStatuses` on ticket JSON | Not on `TicketResponse` yet |
| React `src/frontend/ticket-ui` | Not scaffolded |

## Key design docs (synced to code)

- [design-notes.md](../design-notes.md) — actual layout and endpoints
- [api-contract.md](../api-contract.md) — implemented vs pending
- [data-model.md](../data-model.md) — entities and enums
- [ui-flow.md](../ui-flow.md) — target UI against current API
