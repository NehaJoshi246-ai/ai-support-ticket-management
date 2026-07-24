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

## Current phase

Repository structure staging — documentation and folders before application code.

## Constraints

- Prefer docs and structure before coding.
- Keep API, data model, UI, and tests aligned with root markdown contracts.
- Record AI usage in `final-ai-usage-summary.md` and prompts under `ai-prompts/`.
