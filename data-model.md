# Data Model

## Overview

EF Core 8 persistence for support tickets (and related entities as needed).

## Entities (draft)

### Ticket

| Field | Type | Notes |
|-------|------|-------|
| Id | Guid / int | Primary key |
| Title | string | Required |
| Description | string | Required |
| Status | enum / string | Open, InProgress, Resolved, Closed |
| Priority | enum / string | Low, Medium, High, Critical |
| CreatedAt | DateTimeOffset | Set on create |
| UpdatedAt | DateTimeOffset | Updated on change |
| AssigneeId | Guid / int? | Optional FK if users exist |

### User / Agent (optional)

| Field | Type | Notes |
|-------|------|-------|
| Id | Guid / int | Primary key |
| Name | string | Display name |
| Email | string | Unique if used for login |

## Relationships

- Ticket may optionally belong to one Assignee (User).
- Additional entities (comments, attachments, history) TBD based on scope.

## EF Core notes

- DbContext: e.g. `AppDbContext` / `TicketDbContext`.
- Migrations for schema evolution.
- Indexes on Status / Priority if filtering is common.

## ER sketch

```
User 1───* Ticket (optional assignee)
```
