# Pull Request Description

## Summary

Support Ticket Management assessment — ASP.NET Core 8 Web API with EF Core SQLite backend for tickets, status transitions, and comments.

## What's in this PR

### Backend ✅

- `SupportTickets.Api`, `SupportTickets.Domain`, `SupportTickets.Infrastructure`
- Ticket CRUD (fields via PUT; status via dedicated PATCH)
- `TransitionMap` state machine; invalid transitions → 409
- Nested comment endpoints with ticket existence checks → 404
- Seed: 10 users, 5 tickets (all statuses), 2 comments
- SQLite auto-migrate on startup; CORS + Swagger in Development

### Not in this PR yet

- `GET /api/users`
- React frontend (`src/frontend/`)
- Integration test project (`tests/`)
- Review fixes from `code-review-notes.md`

## Test plan

- [x] `dotnet build src/SupportTickets.sln`
- [x] Manual: GET tickets, POST ticket, PATCH status (valid/invalid), POST comment
- [ ] Integration test suite
- [ ] UI smoke tests

## Docs

Root markdown and `ai-prompts/` aligned with implemented code.
