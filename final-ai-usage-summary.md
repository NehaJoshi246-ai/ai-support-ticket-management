# Final AI Usage Summary

## How AI was used

| Phase | Usage |
|-------|---------|
| Repository staging | Root docs, folders, `.gitignore` |
| Requirements / design | Assessment scope, API contract, layered design (partially simplified in code) |
| Backend implementation | Solution scaffold, entities, migrations, seed, ticket/comment/status APIs |
| Debugging | Comment 404 vs 500 analysis → `debugging-notes.md` |
| Code review | TicketService / transition service review with rejected findings |
| Docs sync | Align all markdown with implemented code |

## Prompts / collaboration

- Staged docs before code per assignment workflow.
- Scaffolded `SupportTickets.*` solution with SQLite.
- Implemented validation, PATCH status, TransitionMap, 409 responses.
- Drafted README setup outline and `database/setup-notes.md`.
- Transition test matrix (25 pairs) for future integration tests.

## What AI did not do

- React frontend (planned, not built).
- Integration test project.
- `GET /api/users` endpoint.
- Review fixes (concurrency token, shared mapper).

## Assessment

AI accelerated scaffolding and API implementation. Human ownership shown by rejecting some review suggestions, documenting 409 vs 400, and syncing docs to **actual** code structure (services in Api project, no Users endpoint yet).

_Update at project completion with test results and UI delivery._
