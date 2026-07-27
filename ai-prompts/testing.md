# AI Prompts — Testing

## Purpose

Integration test planning and execution prompts.

## Status

No test project in repo yet. Strategy in [test-strategy.md](../test-strategy.md).

## Prompts (planned)

### Transition matrix review

```
List all 25 (fromStatus, toStatus) pairs for review before writing tests.
Valid → 200, invalid → 409, same-status → 200 idempotent.
```

### Integration test scaffold

```
WebApplicationFactory for SupportTickets.Api with SQLite test database.
Cover tickets, comments, status matrix per test-strategy.md.
```

## Outcomes

- 25-pair matrix documented in conversation (pending test code)
- [test-results.md](../test-results.md) — manual smoke only so far

## Manual smoke verified

- Build succeeds
- GET tickets, PATCH valid/invalid status, comment 404 on missing ticket
