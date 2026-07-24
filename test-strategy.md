# Test Strategy

## Scope

Primary focus: **integration tests** for the ASP.NET Core Web API and EF Core persistence.

## Goals

- Verify ticket API endpoints against a real (or test) database pipeline.
- Catch contract, validation, and persistence regressions early.
- Keep unit tests optional unless complexity warrants them.

## Integration test coverage (planned)

| Area | Scenarios |
|------|-----------|
| Create | Valid create returns 201/200 and persists |
| List | Returns created tickets |
| Get by id | Existing ticket; 404 for missing |
| Update | Status/priority/field updates persist |
| Validation | Missing title/description rejected |
| Delete | Soft/hard delete behavior if in scope |

## Approach

- Use ASP.NET Core WebApplicationFactory (or equivalent) for in-process API tests.
- Prefer isolated test database / EF Core provider suitable for CI.
- Seed minimal data per test; clean up or use unique fixtures.
- Assert HTTP status, response body shape, and DB state where useful.

## Out of scope (initial)

- Full E2E browser automation (unless later required).
- Load / performance testing.

## Reporting

- Record runs and outcomes in `test-results.md`.
- Note failures and fixes in `debugging-notes.md` / `review-fixes.md`.
