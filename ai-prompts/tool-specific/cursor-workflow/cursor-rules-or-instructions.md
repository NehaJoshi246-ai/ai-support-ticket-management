# Cursor Rules / Instructions

## Working agreements for this project

1. Keep root documentation and `ai-prompts/` updated when decisions change.
2. Put application code under `src/`; tests under `tests/`; DB artifacts under `database/`.
3. Do not invent product scope beyond agreed requirements docs without calling it out.
4. Match implemented APIs to `api-contract.md` and entities to `data-model.md`.
5. Prefer integration tests for API + EF Core critical paths.
6. Log debugging and review findings in the corresponding root markdown files.

## Suggested Cursor habits

- Start design/implementation chats with a pointer to `project-context.md`.
- Paste or reference the relevant phase prompt file (`planning.md`, `design.md`, etc.).
- After substantive AI help, update `final-ai-usage-summary.md`.

## Optional `.cursor/rules`

_If project rules are added later, summarize them here and keep this file in sync._
