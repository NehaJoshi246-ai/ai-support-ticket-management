# Project Context (Cursor Workflow)

## Project

AI Support Ticket Management

## Stack

- Backend: ASP.NET Core Web API
- Frontend: React
- Database: EF Core 8
- Testing: Integration tests

## Repository layout (target)

```
src/                  Application source (API + React)
tests/                Integration tests
database/             Setup notes, schema, seed data
ai-prompts/           Prompt logs by phase
*.md                  Planning / design / delivery docs at root
```

## Assessment option

Support Ticket Management

- **User:** seeded only
- **Ticket:** Id, Title, Description, Priority, Status, AssignedTo, CreatedBy, CreatedAt
- **Status machine:** Open→InProgress|Cancelled; InProgress→Resolved|Cancelled; Resolved→Closed

## Current phase

Requirements/design locked from assessment option; application code next.

## Constraints

- Prefer docs and structure before coding.
- Keep API, data model, UI, and tests aligned with root markdown contracts.
- Enforce status transitions server-side.
- Record AI usage in `final-ai-usage-summary.md` and prompts under `ai-prompts/`.
