# API Contract

Base URL: `/api`  
Content-Type: `application/json`

## Conventions

- Controllers are thin; services enforce business rules.
- **Status changes are not allowed on `PUT /api/tickets/{id}`** — use `PATCH /api/tickets/{id}/status`.
- Enum values in JSON use PascalCase strings (e.g. `"InProgress"`, `"Low"`).
- Timestamps are ISO-8601 UTC.

---

## Users (read-only)

Seeded users for creator, assignee, and comment author selection.

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/users` | List all seeded users |

### Response `200`

```json
[
  { "id": 1, "name": "Alex Agent", "email": "alex@example.com" }
]
```

No POST/PUT/DELETE.

---

## Tickets

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/tickets` | List tickets (newest first) |
| GET | `/api/tickets/{id}` | Get ticket by id |
| POST | `/api/tickets` | Create ticket (status = Open) |
| PUT | `/api/tickets/{id}` | Update ticket fields (not status) |
| PATCH | `/api/tickets/{id}/status` | Transition ticket status |

Optional query on list (later): `?status=Open&priority=High&assignedToId=2`

### Ticket response

```json
{
  "id": 1,
  "title": "Cannot log in",
  "description": "User sees 500 after password reset",
  "priority": "High",
  "status": "Open",
  "assignedToId": 2,
  "assignedToName": "Alex Agent",
  "createdById": 1,
  "createdByName": "Sam Support",
  "createdAt": "2026-07-24T08:00:00Z"
}
```

### POST `/api/tickets` — create

**Request**

```json
{
  "title": "Cannot log in",
  "description": "User sees 500 after password reset",
  "priority": "High",
  "createdById": 1,
  "assignedToId": null
}
```

**Response `201`** — ticket body (status = `Open`, `createdAt` set by server).

**Rules**

- `title`, `description`, `priority`, `createdById` required.
- `assignedToId` optional.
- Client must not send `status` or `createdAt`.

### PUT `/api/tickets/{id}` — update fields

**Request**

```json
{
  "title": "Updated title",
  "description": "Updated description",
  "priority": "Critical",
  "assignedToId": 2
}
```

**Response `200`** — updated ticket.

**Rules**

- Updatable: `title`, `description`, `priority`, `assignedToId`.
- **Not accepted:** `status`, `createdById`, `createdAt`.
- If request includes `status`, return **400** with message directing client to the status endpoint.

### PATCH `/api/tickets/{id}/status` — transition status

**Request**

```json
{
  "status": "InProgress"
}
```

**Response `200`** — ticket with new status.

**Allowed transitions**

| Current status | Request `status` may be |
|----------------|---------------------------|
| Open | InProgress, Cancelled |
| InProgress | Resolved, Cancelled |
| Resolved | Closed |
| Closed | _(none — 400)_ |
| Cancelled | _(none — 400)_ |

**Errors**

- **400** — invalid transition (e.g. Open → Resolved).
- **404** — ticket not found.

Example error body:

```json
{
  "title": "Invalid status transition",
  "detail": "Cannot transition from Open to Resolved. Allowed: InProgress, Cancelled.",
  "status": 400
}
```

---

## Comments (nested under ticket)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/tickets/{ticketId}/comments` | List comments for a ticket (oldest first) |
| POST | `/api/tickets/{ticketId}/comments` | Add a comment |

### Comment response

```json
{
  "id": 10,
  "ticketId": 1,
  "body": "Reproduced in staging; investigating auth middleware.",
  "createdById": 2,
  "createdByName": "Alex Agent",
  "createdAt": "2026-07-24T09:15:00Z"
}
```

### POST `/api/tickets/{ticketId}/comments`

**Request**

```json
{
  "body": "Reproduced in staging; investigating auth middleware.",
  "createdById": 2
}
```

**Response `201`** — created comment.

**Rules**

- Ticket must exist (**404** if not).
- `body` and `createdById` required.
- Comments do not change ticket status.

---

## Common error responses

| Status | When |
|--------|------|
| 400 | Validation failure, invalid status transition, status sent on PUT |
| 404 | Ticket, comment context, or referenced user not found |
| 500 | Unexpected server error |

Validation errors should return field-level detail where practical (ProblemDetails shape).

---

## Service mapping (implementation hint)

| Endpoint | Application service |
|----------|---------------------|
| Ticket CRUD (except status) | `ITicketService` |
| PATCH status | `ITicketStatusTransitionService` |
| Comments | `ITicketCommentService` |
| Users list | `IUserService` |
