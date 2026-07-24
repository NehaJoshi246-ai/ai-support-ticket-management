# Test Strategy

## Scope

Integration tests for the ASP.NET Core Web API + EF Core 8, focused on tickets and the status state machine.

## Goals

- Verify ticket create/list/get/update and assignment to seeded users.
- Prove allowed transitions succeed and invalid transitions fail.
- Keep browser E2E out of initial scope.

## Integration test coverage

| Area | Scenarios |
|------|-----------|
| Users | `GET /api/users` returns seeded users |
| Create | Valid create → Open status, CreatedAt set, CreatedBy persisted |
| List / Get | Returns tickets; 404 for missing id |
| Update fields | Title/description/priority/assignee persist |
| Valid transitions | Open→InProgress, Open→Cancelled, InProgress→Resolved, InProgress→Cancelled, Resolved→Closed |
| Invalid transitions | e.g. Open→Resolved, Open→Closed, Resolved→Cancelled, Closed→*, Cancelled→* → 400 |
| Validation | Missing title/description rejected |
| Terminal | Updates that only change non-status fields still allowed on Closed/Cancelled if product allows; status change rejected |

## Approach

- `WebApplicationFactory` (or equivalent) for in-process API tests.
- Isolated test DB; seed Users as part of test host setup.
- Assert HTTP status, body, and persisted Status where useful.

## Out of scope (initial)

- Full browser E2E.
- Load / performance testing.
- User write APIs (not in product scope).

## Reporting

- Results → `test-results.md`
- Failures → `debugging-notes.md` / `review-fixes.md`
