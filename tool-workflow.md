# Tool Workflow

## Tools used

| Tool | Purpose |
|------|---------|
| Cursor / AI assistant | Planning, backend implementation, docs |
| .NET 8 SDK | `SupportTickets.Api`, Domain, Infrastructure |
| EF Core 8 + SQLite | Local persistence |
| `dotnet-ef` | Migrations (optional; API auto-migrates) |
| Node.js | React frontend (planned) |
| Git | Version control |

## Workflow (actual progress)

1. ✅ Stage repository docs and folders
2. ✅ Lock assessment requirements and design
3. ✅ Scaffold `SupportTickets.sln` (Api, Domain, Infrastructure)
4. ✅ Implement ticket, status, comment APIs
5. ⬜ Add `GET /api/users`
6. ⬜ React frontend
7. ⬜ Integration tests + state machine matrix
8. ⬜ Review fixes and delivery docs

## Conventions

- Docs updated to match code (not aspirational Application layer paths).
- Status changes only via `PATCH /api/tickets/{id}/status`.
- Log AI usage in `final-ai-usage-summary.md` (includes failures and course corrections) and prompts under `ai-prompts/`.
