# Database Setup Notes

## Database choice

| Item | Value |
|------|--------|
| **Provider** | SQLite (local assessment / dev only — not production) |
| **ORM** | EF Core 8 |
| **DbContext** | `SupportTickets.Infrastructure.Data.AppDbContext` |
| **Migrations** | `src/SupportTickets.Infrastructure/Migrations/` |

SQLite was chosen for zero external install: a single file database, no Docker or server process. Suitable for local development and integration tests.

---

## Connection string

Configured in `src/SupportTickets.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=support-tickets.db"
  }
}
```

**Notes:**

- `Data Source=support-tickets.db` is a **relative path**. The file is created in the **process working directory** when the API runs (typically `src/SupportTickets.Api/` when using `dotnet run` on the Api project).
- No username, password, or API keys — nothing secret to commit.
- For a custom local path (optional), use e.g. `Data Source=../database/support-tickets.db` or an absolute path. Prefer `appsettings.Development.json` (gitignored) for machine-specific overrides — do not commit real credentials if you later switch providers.

**Example local override** (`appsettings.Development.json`, not committed):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=./data/support-tickets.db"
  }
}
```

---

## Schema & migrations

Initial migration: `20260724083521_InitialCreate`

Tables:

| Table | Purpose |
|-------|---------|
| `Users` | Seeded users (read-only via API) |
| `Tickets` | Support tickets |
| `TicketComments` | Comments on tickets |
| `__EFMigrationsHistory` | EF migration tracking |

**Apply migrations manually** (optional — the API also migrates on startup):

```bash
# From repository root; requires dotnet-ef global tool
dotnet ef database update \
  --project src/SupportTickets.Infrastructure/SupportTickets.Infrastructure.csproj \
  --startup-project src/SupportTickets.Api/SupportTickets.Api.csproj
```

**Add a new migration** after model changes:

```bash
dotnet ef migrations add <MigrationName> \
  --project src/SupportTickets.Infrastructure/SupportTickets.Infrastructure.csproj \
  --startup-project src/SupportTickets.Api/SupportTickets.Api.csproj \
  --output-dir Migrations
```

**Automatic migrate on startup:** `Program.cs` calls `db.Database.Migrate()` when the API starts, so a fresh clone gets schema applied on first run without a separate step.

---

## Seed data

Seed is defined in `src/SupportTickets.Infrastructure/Seed/DataSeeder.cs` and applied via EF Core **`HasData`** in the initial migration (not a runtime script).

### What is seeded

| Entity | Count | Description |
|--------|-------|-------------|
| **Users** | 10 | 3 Customers, 3 Agents, 2 Leads, 2 Admins (fixed ids 1–10) |
| **Tickets** | 5 | One per lifecycle state: Open, InProgress, Resolved, Closed, Cancelled |
| **TicketComments** | 2 | On the InProgress and Resolved sample tickets |

### Sample users (ids for forms / API)

| Id | Name | Role |
|----|------|------|
| 1–3 | Sam / Riley / Casey Customer | Customer |
| 4–6 | Alex / Morgan / Taylor Agent | Agent |
| 7–8 | Jordan / Quinn Lead | Lead |
| 9–10 | Avery / Blake Admin | Admin |

### Sample tickets

| Id | Title | Status |
|----|-------|--------|
| 1 | Cannot reset password | Open |
| 2 | Dashboard charts blank on Safari | InProgress |
| 3 | Invoice PDF download fails | Resolved |
| 4 | Typo on billing FAQ page | Closed |
| 5 | Duplicate notification emails | Cancelled |

Reference copies may also be kept under `database/seed-data/` for documentation; the **source of truth** for runtime seed is `DataSeeder.cs` baked into the migration.

**Important:** `HasData` seed runs when the migration is **first applied**. It does **not** re-run on every API restart.

---

## Restart & persistence

### Normal restart (data kept)

1. Stop the API (Ctrl+C).
2. Start the API again.
3. The `support-tickets.db` file remains on disk.
4. `Migrate()` runs but applies **no pending migrations** if schema is up to date.
5. **All tickets, comments, and user-created data persist** across restarts.

### Fresh clone / first run (empty machine)

1. No `support-tickets.db` file exists yet (file is gitignored).
2. Start the API → `Migrate()` creates the file, applies `InitialCreate`, inserts seed users/tickets/comments.
3. You immediately have sample data to browse.

### Reset to seed-only state (wipe local data)

1. Stop the API.
2. Delete the SQLite file (and WAL sidecars if present):
   - `src/SupportTickets.Api/support-tickets.db`
   - `support-tickets.db-shm`, `support-tickets.db-wal` (if they exist)
3. Start the API → migration reapplies → seed data restored.

### What is *not* persisted in git

- `*.db`, `*.db-shm`, `*.db-wal` are in `.gitignore`.
- Each developer (and CI agent) gets their own local file on first run.

---

## Troubleshooting

| Symptom | Likely cause | Action |
|---------|--------------|--------|
| Empty DB after clone | Haven't run API yet | Start API once (auto-migrate) |
| Schema out of date | Model changed, no migration | Add migration + `database update` |
| `FOREIGN KEY constraint failed` | Invalid user/ticket id in request | Use seeded user ids; ensure ticket exists |
| Locked database errors | Parallel writes to SQLite | Retry; avoid heavy concurrent writes locally |
| Wrong DB file used | Relative path + different cwd | Run from Api project dir or set absolute path in Development config |

---

## Related paths

| Path | Purpose |
|------|---------|
| `database/schema/` | Optional SQL dumps / schema notes |
| `database/seed-data/` | Optional reference seed artifacts |
| `src/SupportTickets.Infrastructure/Migrations/` | EF migration C# files |
| `src/SupportTickets.Infrastructure/Seed/DataSeeder.cs` | Seed definition |
