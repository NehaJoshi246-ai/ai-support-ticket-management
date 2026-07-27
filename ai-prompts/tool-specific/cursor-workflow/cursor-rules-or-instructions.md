# Cursor Rules / Instructions

## Working agreements (aligned with codebase)

1. Solution is `src/SupportTickets.sln` — projects `Api`, `Domain`, `Infrastructure` (not `backend/Application`).
2. Business logic in `SupportTickets.Api/Services/` today — thin controllers in `Controllers/`.
3. Status changes **only** through `TicketStatusTransitionService` + `PATCH .../status`.
4. Rules in `SupportTickets.Domain/Rules/TransitionMap.cs`.
5. Do not document `GET /api/users` as done until `UsersController` exists.
6. Update root docs when API or model changes.

## Before frontend work

- Implement or stub `GET /api/users` for dropdowns.
- CORS already enabled in `Program.cs`.
- API dev URL: `http://localhost:5189`.

## Before integration tests

- See 25-pair transition matrix in planning conversation.
- Expect 409 for invalid transitions, not 400.

## Doc sync

After substantive changes, update: `api-contract.md`, `acceptance-criteria.md`, `implementation-plan.md`, `ai-prompts/planning.md`.

After new Cursor sessions, copy JSONL into `ai-prompts/transcripts/raw/` and run `python ai-prompts/transcripts/scripts/export-transcripts.py`.
