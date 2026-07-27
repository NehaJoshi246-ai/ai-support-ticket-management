# Database Schema

SQLite schema for the Support Ticket Management system.  
**Runtime source of truth:** EF Core migrations in `src/SupportTickets.Infrastructure/Migrations/`.  
This folder is a human-readable reference and optional manual bootstrap via `schema.sql`.

---

## ER diagram

```
┌─────────────────────────────────────────────────────────────────┐
│ Users                                                           │
│  PK Id          INTEGER                                         │
│      Name       TEXT NOT NULL                                   │
│      Email      TEXT NOT NULL UNIQUE                            │
│      Role       INTEGER NOT NULL                                │
└────────────┬───────────────────────────────┬────────────────────┘
             │                               │
             │ CreatedById (required)        │ AssignedToId (optional)
             │ ON DELETE RESTRICT            │ ON DELETE SET NULL
             ▼                               ▼
┌─────────────────────────────────────────────────────────────────┐
│ Tickets                                                         │
│  PK Id           INTEGER                                        │
│      Title        TEXT NOT NULL (max 200)                       │
│      Description  TEXT NOT NULL (max 4000)                      │
│      Priority     INTEGER NOT NULL                              │
│      Status       INTEGER NOT NULL                              │
│  FK  AssignedToId INTEGER NULL → Users.Id                       │
│  FK  CreatedById  INTEGER NOT NULL → Users.Id                   │
│      CreatedAt    TEXT NOT NULL (DateTimeOffset ISO-8601)     │
└────────────────────────────┬────────────────────────────────────┘
                             │ TicketId
                             │ ON DELETE CASCADE
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│ TicketComments                                                  │
│  PK Id          INTEGER                                         │
│  FK TicketId    INTEGER NOT NULL → Tickets.Id                   │
│      Body       TEXT NOT NULL (max 4000)                        │
│  FK CreatedById INTEGER NOT NULL → Users.Id                     │
│      CreatedAt  TEXT NOT NULL (DateTimeOffset ISO-8601)         │
└─────────────────────────────────────────────────────────────────┘
```

---

## Tables

### Users

| Column | SQLite type | Nullable | Notes |
|--------|-------------|----------|-------|
| Id | INTEGER | NO | PK, autoincrement |
| Name | TEXT | NO | max 100 chars (EF) |
| Email | TEXT | NO | unique index |
| Role | INTEGER | NO | see UserRole enum |

### Tickets

| Column | SQLite type | Nullable | Notes |
|--------|-------------|----------|-------|
| Id | INTEGER | NO | PK, autoincrement |
| Title | TEXT | NO | max 200 |
| Description | TEXT | NO | max 4000 |
| Priority | INTEGER | NO | see TicketPriority |
| Status | INTEGER | NO | see TicketStatus; changed via PATCH only |
| AssignedToId | INTEGER | YES | FK → Users.Id |
| CreatedById | INTEGER | NO | FK → Users.Id |
| CreatedAt | TEXT | NO | ISO-8601 `DateTimeOffset` |

### TicketComments

| Column | SQLite type | Nullable | Notes |
|--------|-------------|----------|-------|
| Id | INTEGER | NO | PK, autoincrement |
| TicketId | INTEGER | NO | FK → Tickets.Id |
| Body | TEXT | NO | max 4000 |
| CreatedById | INTEGER | NO | FK → Users.Id |
| CreatedAt | TEXT | NO | ISO-8601 `DateTimeOffset` |

---

## Indexes

| Index | Table | Column(s) | Unique |
|-------|-------|-----------|--------|
| IX_Users_Email | Users | Email | yes |
| IX_Tickets_AssignedToId | Tickets | AssignedToId | no |
| IX_Tickets_CreatedById | Tickets | CreatedById | no |
| IX_Tickets_Priority | Tickets | Priority | no |
| IX_Tickets_Status | Tickets | Status | no |
| IX_TicketComments_TicketId | TicketComments | TicketId | no |
| IX_TicketComments_CreatedById | TicketComments | CreatedById | no |

---

## Enums (stored as INTEGER)

### UserRole

| Value | Name |
|-------|------|
| 0 | Customer |
| 1 | Agent |
| 2 | Lead |
| 3 | Admin |

### TicketPriority

| Value | Name |
|-------|------|
| 0 | Low |
| 1 | Medium |
| 2 | High |
| 3 | Critical |

### TicketStatus

| Value | Name |
|-------|------|
| 0 | Open |
| 1 | InProgress |
| 2 | Resolved |
| 3 | Closed |
| 4 | Cancelled |

**Status transitions** (enforced in application code, not DB):

```
Open → InProgress | Cancelled
InProgress → Resolved | Cancelled
Resolved → Closed
```

---

## Foreign keys & delete behavior

| FK | On delete |
|----|-----------|
| Tickets.CreatedById → Users | RESTRICT |
| Tickets.AssignedToId → Users | SET NULL |
| TicketComments.TicketId → Tickets | CASCADE |
| TicketComments.CreatedById → Users | RESTRICT |

---

## Files

| File | Purpose |
|------|---------|
| `schema.sql` | DDL only — tables, indexes, FKs |
| `schema.md` | This document |

## Applying schema manually (optional)

Normally the API applies migrations on startup. To inspect DDL only:

```bash
sqlite3 support-tickets.db < database/schema/schema.sql
```

For a full dev database with seed data, run the API once or use `dotnet ef database update` (see [setup-notes.md](../setup-notes.md)).
