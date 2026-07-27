# Review Fixes

## Fixes applied

| Date | Finding (from code-review-notes) | Fix | Verified |
|------|----------------------------------|-----|----------|
| | | | |

## Outstanding

| ID | Finding | Priority | Status |
|----|---------|----------|--------|
| H1 | No optimistic concurrency on Ticket | High | Open |
| H2 | PUT + PATCH lost updates | High | Open |
| H3 | Enum.Parse can 500 if validation bypassed | Medium | Open |
| M1 | Null request not guarded in services | Medium | Open |
| M3 | Duplicated TicketResponse mapping | Medium | Open |
| — | `GET /api/users` missing | Medium | Open |
| — | `allowedNextStatuses` not on ticket JSON | Low | Open |

## Notes

Apply fixes incrementally; re-run manual smoke and integration tests when added.
