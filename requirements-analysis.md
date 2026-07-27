# Requirements Analysis

## Assessment option

**Support Ticket Management System**

## Stack

- **Backend:** ASP.NET Core Web API
- **Frontend:** React
- **Database:** EF Core 8
- **Testing:** Integration tests

## Entities

### User (seeded only)

- Users exist in the database via seed data only.
- No user create/update/delete UI or APIs required (read for assignment is enough).

### Ticket

| Field | Description |
|-------|-------------|
| Id | Primary key |
| Title | Ticket title |
| Description | Ticket body |
| Priority | Ticket priority |
| Status | Lifecycle status (state machine enforced) |
| AssignedTo | FK to User (assignee) |
| CreatedBy | FK to User (creator) |
| CreatedAt | Creation timestamp |

## Status state machine

Allowed transitions only:

```
Open ──────────────► In Progress
Open ──────────────► Cancelled
In Progress ───────► Resolved
In Progress ───────► Cancelled
Resolved ──────────► Closed
```

| From | Allowed next statuses |
|------|------------------------|
| Open | In Progress, Cancelled |
| In Progress | Resolved, Cancelled |
| Resolved | Closed |
| Closed | _(terminal — no further transitions)_ |
| Cancelled | _(terminal — no further transitions)_ |

Invalid transitions must be rejected by the API.

## Functional requirements

1. Create tickets with title, description, priority; set CreatedBy and CreatedAt; initial status **Open**.
2. List and view tickets (including assignee and creator).
3. Assign / reassign tickets to seeded users (`AssignedTo`).
4. Update ticket fields as allowed; change status only via valid transitions.
5. Persist via EF Core 8; seed Users for local/dev/test.
6. React UI for ticket list, create, detail/update, assignment, and valid status actions.
7. Integration tests for CRUD-ish ticket flows and status transition rules.

## Non-functional requirements

- Clear API contract and consistent validation/error responses.
- State machine rules enforced server-side (UI should only offer valid next statuses).
- Maintainable layout: `src/`, `tests/`, `database/`, root docs.

## Out of scope

- User registration / CRUD (users are seeded only).
- Arbitrary status jumps (e.g. Open → Resolved, Closed → Open).
- Comments, attachments, or audit history (unless later required).

## Open questions

- Exact priority enum values (e.g. Low / Medium / High / Critical)?
- Is `AssignedTo` required on create or optional until later?
- Auth model for identifying `CreatedBy`, or select from seeded users in UI?
