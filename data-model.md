# Data Model

## Overview

EF Core 8 over **SQLite** for the Support Ticket Management assessment option.

Entities: **User** (seeded), **Ticket**, **TicketComment**.

## Entities

### User (seeded only)

| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, identity |
| Name | string(100) | required |
| Email | string(200) | required, unique |

Populated via Infrastructure seed (`UserSeed`). No runtime user writes.

### Ticket

| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, identity |
| Title | string(200) | required |
| Description | string(4000) | required |
| Priority | int / enum | Low, Medium, High, Critical |
| Status | int / enum | see state machine |
| AssignedToId | int | FK → User, nullable |
| CreatedById | int | FK → User, required |
| CreatedAt | DateTimeOffset | required, set on insert |

**Defaults on create:** `Status = Open`, `CreatedAt = UtcNow`.

**Not updatable:** `Id`, `CreatedById`, `CreatedAt`.

**Not via general update:** `Status` — changed only through the status transition service/endpoint.

### TicketComment

| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, identity |
| TicketId | int | FK → Ticket, required, cascade delete |
| Body | string(4000) | required |
| CreatedById | int | FK → User, required |
| CreatedAt | DateTimeOffset | required, set on insert |

Comments are append-only in v1 (no edit/delete unless scope expands).

## Enums

### TicketStatus

| Value | Notes |
|-------|-------|
| Open | Initial state |
| InProgress | Work started |
| Resolved | Fix/work complete, awaiting close |
| Closed | Terminal |
| Cancelled | Terminal |

### TicketPriority

`Low` | `Medium` | `High` | `Critical`

## Status state machine

```
Open ──► InProgress ──► Resolved ──► Closed
  │            │
  └────────────┴──► Cancelled
```

| From | Allowed next |
|------|--------------|
| Open | InProgress, Cancelled |
| InProgress | Resolved, Cancelled |
| Resolved | Closed |
| Closed | _(none)_ |
| Cancelled | _(none)_ |

Implemented in `Domain/Rules/TicketStatusTransitionRules.cs` and enforced by `TicketStatusTransitionService`.

## Relationships

```
User 1───* Ticket        (CreatedBy)
User 1───* Ticket        (AssignedTo)
User 1───* TicketComment (CreatedBy)
Ticket 1───* TicketComment
```

## ER diagram

```
┌─────────┐       ┌─────────┐       ┌────────────────┐
│  User   │◄──────│ Ticket  │──────►│ TicketComment  │
└─────────┘       └─────────┘       └────────────────┘
   ▲                  │
   │                  │
   └── CreatedBy / AssignedTo / Comment author
```

## EF Core / SQLite notes

- **DbContext:** `AppDbContext` in `Infrastructure/Data/`.
- **Configurations:** fluent API per entity (`TicketConfiguration`, etc.).
- **Connection:** `Data Source=support-tickets.db` (path in `appsettings.Development.json`).
- **Migrations:** under `Infrastructure/Migrations/`.
- **Indexes:** `Tickets(Status)`, `Tickets(Priority)`, `Tickets(AssignedToId)`, `TicketComments(TicketId)`.
- **Delete behavior:** cascade delete comments when a ticket is removed (if delete is ever added; otherwise unused).

## Seed & schema artifacts

| Location | Purpose |
|----------|---------|
| `database/schema/` | Optional SQL dumps / notes |
| `database/seed-data/` | Reference seed JSON/scripts |
| `Infrastructure/Seed/UserSeed.cs` | Runtime EF seed for users |
