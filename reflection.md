# Reflection

## What went well

- Clear assessment scope (state machine, seeded users, SQLite) kept backend focused.
- Dedicated PATCH status endpoint made transition rules testable and explicit.
- Field-level 400 validation and 409 for bad transitions give useful API errors.
- `DataSeeder` provides immediate demo data across all ticket statuses.

## What was challenging

- Documentation drifted ahead of code (Application layer, Users API, frontend paths).
- SQLite `DateTimeOffset` ordering required in-memory sort on list.
- Concurrency and duplicated mapping flagged in review but not yet fixed.

## What I would do differently

- Add `GET /api/users` alongside ticket endpoints before UI work.
- Single `TicketResponseMapper` from the start.
- Integration test project early, especially the 25-pair transition matrix.

## Key learnings

- Keep docs synced with actual project names (`SupportTickets.*` vs planned `backend/` layout).
- Distinguish 400 (validation) vs 409 (business rule) for status transitions.
- Explicit ticket-existence check before comment insert avoids FK 500s.
