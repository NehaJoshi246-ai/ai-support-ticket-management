# Test Results

## Summary

| Date | Suite | Passed | Failed | Skipped | Notes |
|------|-------|--------|--------|---------|-------|
| 2026-07-27 | Integration (status matrix) | 25 | 0 | 0 | `TicketStatusTransitionMatrixTests` |

## Latest run

- **Command:** `dotnet test tests/SupportTickets.IntegrationTests/SupportTickets.IntegrationTests.csproj`
- **Environment:** .NET 8, SQLite `:memory:` via `WebApplicationFactory`
- **Outcome:** 25/25 passed — full `(fromStatus, toStatus)` matrix

### Matrix breakdown

| Category | Count | HTTP |
|----------|-------|------|
| Valid forward transitions | 5 | 200 |
| Same-status no-op | 5 | 200 |
| Invalid transitions | 15 | 409 with `fromStatus`, `toStatus`, `allowedNextStatuses` |

## Manual smoke (API)

| Check | Result | Notes |
|-------|--------|-------|
| `dotnet build src/SupportTickets.sln` | ✅ | Builds clean |
| `GET /api/health` | ✅ | Manual |
| `GET /api/tickets` | ✅ | Returns seed + created tickets |
| `PATCH` valid/invalid status | ✅ | 200 / 409 |
| POST comment missing ticket | ✅ | 404 (not 500) |
