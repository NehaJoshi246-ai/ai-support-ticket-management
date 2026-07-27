# API Contract

Base URL: `/api`  
Content-Type: `application/json`  
Enums: PascalCase strings in JSON (`JsonStringEnumConverter`).

## Implementation status

| Endpoint | Status |
|----------|--------|
| `GET /api/health` | ✅ Implemented |
| Ticket CRUD + PATCH status | ✅ Implemented |
| Comments GET/POST | ✅ Implemented |
| `GET /api/users` | ⬜ **Not implemented** |

---

## Health

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/health` | `{ "status": "ok" }` |

---

## Users (read-only) — **planned, not implemented**

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/users` | List seeded users |

Until implemented, use seed user ids 1–10 (see `database/setup-notes.md`) or inspect SQLite `Users` table.

---

## Tickets

| Method | Path | Status |
|--------|------|--------|
| GET | `/api/tickets` | ✅ List, newest by `CreatedAt` |
| GET | `/api/tickets/{id}` | ✅ |
| POST | `/api/tickets` | ✅ Creates as `Open` |
| PUT | `/api/tickets/{id}` | ✅ Fields only; **no status** |
| PATCH | `/api/tickets/{id}/status` | ✅ Lifecycle only |

### Ticket response (actual shape)

```json
{
  "id": 1,
  "title": "Cannot reset password",
  "description": "Password reset email never arrives...",
  "priority": "High",
  "status": "Open",
  "assignedToId": 4,
  "assignedToName": "Alex Agent",
  "createdById": 1,
  "createdByName": "Sam Customer",
  "createdAt": "2026-07-01T09:00:00+00:00"
}
```

**Note:** `allowedNextStatuses` is **not** on the response yet. Frontend should mirror `TransitionMap` or add a field in a future API change.

### POST `/api/tickets`

```json
{
  "title": "Cannot log in",
  "description": "User sees 500 after password reset",
  "priority": "High",
  "createdById": 1,
  "assignedToId": null
}
```

- **201** — created ticket (`status` = `Open`, `createdAt` set server-side).
- **400** — `ValidationProblemDetails` with per-field errors.

### PUT `/api/tickets/{id}`

```json
{
  "title": "Updated title",
  "description": "Updated description",
  "priority": "Critical",
  "assignedToId": 2
}
```

- **200** — updated ticket.
- **400** if `status` property present (directs to PATCH endpoint).
- **404** if ticket not found.

### PATCH `/api/tickets/{id}/status`

```json
{
  "status": "InProgress"
}
```

| Current | Allowed `status` |
|---------|------------------|
| Open | InProgress, Cancelled |
| InProgress | Resolved, Cancelled |
| Resolved | Closed |
| Closed | _(none)_ |
| Cancelled | _(none)_ |

- **200** — updated ticket; same status → idempotent 200.
- **409** — invalid transition (`InvalidTransitionException`).
- **404** — ticket not found.

**409 example:**

```json
{
  "title": "Invalid status transition",
  "detail": "Cannot transition from Open to Resolved. Allowed next statuses from Open: InProgress, Cancelled.",
  "status": 409,
  "fromStatus": "Open",
  "toStatus": "Resolved",
  "allowedNextStatuses": ["InProgress", "Cancelled"]
}
```

---

## Comments

| Method | Path | Status |
|--------|------|--------|
| GET | `/api/tickets/{ticketId}/comments` | ✅ Oldest first |
| POST | `/api/tickets/{ticketId}/comments` | ✅ |

### Comment response

```json
{
  "id": 1,
  "ticketId": 2,
  "body": "Reproduced on Safari 17.4...",
  "createdById": 5,
  "createdByName": "Morgan Agent",
  "createdAt": "2026-07-01T12:00:00+00:00"
}
```

### POST body

```json
{
  "body": "Reproduced in staging.",
  "createdById": 4
}
```

- **201** — created comment.
- **400** — empty body / bad user id (field errors).
- **404** — ticket does not exist.

---

## Error summary (as implemented)

| Status | When |
|--------|------|
| 400 | DTO validation (`ValidationProblemDetails`) |
| 404 | Ticket not found; comment on missing ticket |
| 409 | Invalid status transition |
| 500 | Unhandled exceptions (e.g. unmapped `DbUpdateException`) |

---

## Service mapping (actual classes)

| Concern | Class | Project |
|---------|-------|---------|
| Ticket CRUD (fields) | `TicketService` | Api |
| Status transition | `TicketStatusTransitionService` | Api |
| Comments | `TicketCommentService` | Api |
| Rules | `TransitionMap` | Domain |
| Persistence | `AppDbContext` | Infrastructure |

No interfaces/DI abstractions — concrete scoped services registered in `Program.cs`.
