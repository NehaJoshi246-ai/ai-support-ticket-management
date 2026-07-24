# Implementation Plan

## Phase 0 — Repository staging (current)

1. Create required root documentation files.
2. Confirm structure matches assignment expectations.

## Phase 1 — Requirements & design

1. Finalize requirements analysis and acceptance criteria.
2. Define data model (`data-model.md`).
3. Define API contract (`api-contract.md`).
4. Define UI flows (`ui-flow.md`).
5. Capture design notes (`design-notes.md`).

## Phase 2 — Backend

1. Scaffold ASP.NET Core Web API solution.
2. Configure EF Core 8 (entities, DbContext, migrations).
3. Implement ticket APIs and validation.
4. Add seed data if needed for local/dev.

## Phase 3 — Frontend

1. Scaffold React app.
2. Wire API client and ticket list/detail/create/update screens.
3. Align UI with documented flows.

## Phase 4 — Testing & hardening

1. Add integration tests per `test-strategy.md`.
2. Run tests; record results in `test-results.md`.
3. Debug issues; note findings in `debugging-notes.md`.

## Phase 5 — Review & delivery

1. Self/code review → `code-review-notes.md`.
2. Apply fixes → `review-fixes.md`.
3. Write `pr-description.md`, `reflection.md`, `final-ai-usage-summary.md`.

## Suggested order of first code work

1. Solution + EF Core model
2. Ticket API endpoints
3. React ticket list + create
4. Integration tests for create/list/get/update
