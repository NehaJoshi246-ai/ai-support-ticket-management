# Data Model

## Overview

EF Core 8 over **SQLite**.  
Projects: `SupportTickets.Domain` (entities), `SupportTickets.Infrastructure` (DbContext, migrations, seed).

## Entities

### User (seeded only)

| Column | Type | Notes |
|--------|------|-------|
| Id | int | PK, identity |
| Name | string(100) | required |
| Email | string(200) | required, unique index |
| Role | int | `UserRole` enum |

**Seed:** 10 users in migration via `DataSeeder`. No runtime writes.

### Ticket

| Column | Type | Notes |
|--------|------|-------|
| Id | int | PK, identity |
| Title | string(200) | required |
| Description | string(4000) | required |
| Priority | int | `TicketPriority` |
| Status | int | `TicketStatus` |
| AssignedToId | int? | FK → User, SET NULL on delete |
| CreatedById | int | FK → User, RESTRICT on delete |
| CreatedAt | DateTimeOffset | TEXT in SQLite |

Indexes: `Status`, `Priority`, `AssignedToId`, `CreatedById`.

### TicketComment

| Column | Type | Notes |
|--------|------|-------|
| Id | int | PK |
| TicketId | int | FK → Ticket, CASCADE delete |
| Body | string(4000) | required |
| CreatedById | int | FK → User, RESTRICT |
| CreatedAt | DateTimeOffset | TEXT in SQLite |

Index: `TicketId`, `CreatedById`.

## Enums (`SupportTickets.Domain.Enums`)

### TicketStatus

`Open` (0) | `InProgress` (1) | `Resolved` (2) | `Closed` (3) | `Cancelled` (4)

### TicketPriority

`Low` (0) | `Medium` (1) | `High` (2) | `Critical` (3)

### UserRole

`Customer` (0) | `Agent` (1) | `Lead` (2) | `Admin` (3)

## Status state machine

Implemented in `Domain/Rules/TransitionMap.cs`, enforced by `TicketStatusTransitionService`.

```
Open → InProgress | Cancelled
InProgress → Resolved | Cancelled
Resolved → Closed
```

## Relationships

```
User 1───* Ticket (CreatedBy)
User 1───* Ticket (AssignedTo, optional)
User 1───* TicketComment (CreatedBy)
Ticket 1───* TicketComment
```

## EF Core details

| Item | Location |
|------|----------|
| DbContext | `Infrastructure/Data/AppDbContext.cs` |
| Configurations | `Infrastructure/Data/Configurations/*.cs` |
| Migration | `20260724083521_InitialCreate` |
| Seed | `Infrastructure/Seed/DataSeeder.cs` (HasData) |
| Connection | `Data Source=support-tickets.db` in `appsettings.json` |

## JSON serialization

Enums serialize as **PascalCase strings** (`JsonStringEnumConverter` in `Program.cs`).

## Artifacts

| Path | Purpose |
|------|---------|
| `database/setup-notes.md` | Local DB setup |
| `database/schema/` | Placeholder (no SQL dump committed) |
| `database/seed-data/` | Placeholder for reference exports |
