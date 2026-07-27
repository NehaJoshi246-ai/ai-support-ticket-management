# UI Flow

**Status:** Not implemented — no `src/frontend/` yet.  
This document describes the **target** UI against the **current** API.

## API available today

| Action | Endpoint | Status |
|--------|----------|--------|
| Health check | `GET /api/health` | ✅ |
| List tickets | `GET /api/tickets` | ✅ |
| Ticket detail | `GET /api/tickets/{id}` | ✅ |
| Create ticket | `POST /api/tickets` | ✅ |
| Update fields | `PUT /api/tickets/{id}` | ✅ |
| Change status | `PATCH /api/tickets/{id}/status` | ✅ |
| List comments | `GET /api/tickets/{id}/comments` | ✅ |
| Add comment | `POST /api/tickets/{id}/comments` | ✅ |
| List users (dropdowns) | `GET /api/users` | ⬜ **Build API first or hardcode seed ids** |

Default API URL (dev): `http://localhost:5189` (`launchSettings.json`).

## Target screens

### 1. Ticket list

- Load `GET /api/tickets`.
- **Search** — client-side filter on title/description.
- **Status filter** — dropdown (All, Open, InProgress, …).
- Show priority, status, assignee, creator.
- States: loading spinner, empty message, error banner.

### 2. Create ticket

- Fields: title, description, priority, createdById, optional assignedToId.
- Submit `POST /api/tickets`.
- Redirect to detail on success; show field errors from 400 response.

### 3. Ticket detail

- Load ticket + `GET /api/tickets/{id}/comments`.
- **Edit block:** title, description, priority, assignee → `PUT` (no status).
- **Status dropdown:** only valid next statuses (mirror `TransitionMap` until API adds `allowedNextStatuses`).
- **Comments:** thread + add form → `POST` comment.
- Terminal tickets: disable status control.

## Status UI rules (mirror `TransitionMap`)

| Current | Show options |
|---------|----------------|
| Open | InProgress, Cancelled |
| InProgress | Resolved, Cancelled |
| Resolved | Closed |
| Closed | None |
| Cancelled | None |

Each change → `PATCH /api/tickets/{id}/status`. On **409**, show `detail` and `allowedNextStatuses` from response.

## Error handling

| Case | UI behavior |
|------|-------------|
| List/detail load fails | Error state + retry |
| Empty ticket list | Message + link to create |
| 400 validation | Inline field errors |
| 404 ticket | Not-found view |
| 409 bad transition | Banner with server message |

## Planned frontend layout

```
src/frontend/ticket-ui/
  src/
    api/          client.ts, tickets.ts, comments.ts, users.ts
    pages/        TicketListPage, TicketCreatePage, TicketDetailPage
    components/   StatusBadge, CommentThread, ...
    types/        mirror API DTOs
```

CORS is already enabled on the API for browser dev.
