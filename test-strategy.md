# Test Strategy

## Scope

Integration tests for `SupportTickets.Api` + EF Core SQLite.  
**Status:** No test project in `tests/` yet — strategy only.

## Test host

- `WebApplicationFactory<Program>` (or custom entry point).
- SQLite: separate file per run or `:memory:` with shared connection.
- Seed data from migration or test fixture.

## Coverage plan

### Tickets

| Scenario | Expected |
|----------|----------|
| POST valid | 201, status Open, persisted |
| POST missing title | 400 field errors |
| POST bad priority | 400 |
| GET list | 200, includes seed + created |
| GET by id | 200 / 404 |
| PUT update fields | 200, status unchanged |
| PUT with status field | 400 |

### Status transitions (25-pair matrix)

- Valid forward transitions → **200**
- Invalid transitions → **409** with `fromStatus`, `toStatus`, `allowedNextStatuses`
- Same status → **200** idempotent
- Document matrix in test code from `TransitionMap`

### Comments

| Scenario | Expected |
|----------|----------|
| GET on existing ticket | 200 list |
| GET on missing ticket | 404 |
| POST valid | 201 |
| POST empty body | 400 |
| POST on missing ticket | 404 |

### Users (when implemented)

- GET `/api/users` returns 10 seeded users.

## Out of scope (initial)

- Browser E2E
- Load testing
- Auth

## Reporting

- Results → `test-results.md`
- Failures → `debugging-notes.md`

## Known code risks to test after fixes

- Concurrent PUT + PATCH (no row version today) — see `code-review-notes.md`
