# Test Results

## Summary

| Date | Suite | Passed | Failed | Skipped | Notes |
|------|-------|--------|--------|---------|-------|
| — | Integration | — | — | — | Test project not created yet |

## Latest run

- **Command:** _pending_
- **Environment:** .NET 8, SQLite
- **Outcome:** No automated tests in repository.

## Manual smoke (API)

| Check | Result | Notes |
|-------|--------|-------|
| `dotnet build src/SupportTickets.sln` | ✅ | Builds clean |
| `GET /api/health` | ✅ | Manual |
| `GET /api/tickets` | ✅ | Returns seed + created tickets |
| `PATCH` valid/invalid status | ✅ | 200 / 409 |
| POST comment missing ticket | ✅ | 404 (not 500) |

Record formal integration test output here when `tests/` project exists.
