# UI Flow

React SPA under `src/frontend/ticket-ui/` calling the REST API defined in `api-contract.md`.

## Primary screens

| Screen | Purpose |
|--------|---------|
| **Ticket list** | Browse tickets; show status, priority, assignee, creator |
| **Create ticket** | New ticket form |
| **Ticket detail** | View/edit fields, transition status, read/add comments |

## API usage by screen

| Action | HTTP call |
|--------|-----------|
| Load users (dropdowns) | `GET /api/users` |
| List tickets | `GET /api/tickets` |
| Open ticket | `GET /api/tickets/{id}` |
| Create ticket | `POST /api/tickets` |
| Save field changes | `PUT /api/tickets/{id}` |
| Change status | `PATCH /api/tickets/{id}/status` |
| Load comments | `GET /api/tickets/{id}/comments` |
| Add comment | `POST /api/tickets/{id}/comments` |

Field edits and status changes use **separate** API calls — the UI should not bundle status into the save form.

## Status UI rules

Show only valid next statuses for the current ticket:

| Current status | Actions shown |
|----------------|---------------|
| Open | **Start** (→ InProgress), **Cancel** (→ Cancelled) |
| InProgress | **Resolve** (→ Resolved), **Cancel** (→ Cancelled) |
| Resolved | **Close** (→ Closed) |
| Closed | Status actions hidden / disabled |
| Cancelled | Status actions hidden / disabled |

Each action calls `PATCH /api/tickets/{id}/status` with the target status, then refreshes the ticket.

## Happy paths

### Create ticket

1. User opens **Create Ticket**.
2. Form: title, description, priority, **Created by** (user dropdown), optional **Assign to**.
3. Submit → `POST /api/tickets`.
4. Redirect to ticket detail or list; new ticket shows status **Open**.

### Browse tickets

1. **Ticket list** loads via `GET /api/tickets`.
2. Row click → detail page via `GET /api/tickets/{id}`.
3. Detail also loads comments: `GET /api/tickets/{id}/comments`.

### Edit ticket fields

1. On detail, user edits title, description, priority, or assignee.
2. **Save** → `PUT /api/tickets/{id}` (no status in payload).
3. UI refreshes ticket from response.

### Transition status

1. User clicks a status action (e.g. **Start work**).
2. UI calls `PATCH /api/tickets/{id}/status` with `{ "status": "InProgress" }`.
3. On success, refresh ticket and re-render available actions.
4. On **400**, show server message (should be rare if actions are filtered).

### Add comment

1. On detail, user types in comment box and selects author (or uses current context user).
2. Submit → `POST /api/tickets/{id}/comments`.
3. Append new comment to thread (or reload comment list).

## Ticket detail layout (draft)

```
┌─────────────────────────────────────────────┐
│ Title                          [status badge]│
│ Priority · Assignee · Created by · Date     │
├─────────────────────────────────────────────┤
│ Description (editable)                       │
│ [Save changes]                               │
├─────────────────────────────────────────────┤
│ Status actions: [Start] [Cancel]  …          │
├─────────────────────────────────────────────┤
│ Comments                                     │
│   • Alex — "Reproduced in staging…"         │
│   [Add comment]                              │
└─────────────────────────────────────────────┘
```

## Error / empty states

| State | Behavior |
|-------|----------|
| No tickets | Empty list message + link to create |
| Ticket not found | 404 page or inline error |
| Validation errors | Inline on create/edit forms |
| Invalid transition | Toast/banner with API `detail` |
| Terminal ticket | Disable status actions; field edit may still be allowed |

## Navigation

```
Ticket List ──► Create Ticket
     │
     └──► Ticket Detail
              ├── Save fields (PUT)
              ├── Status actions (PATCH …/status)
              └── Comment thread (GET/POST …/comments)
```

## Frontend module layout (aligns with backend separation)

```
src/frontend/ticket-ui/src/
├── api/
│   ├── client.ts          # base fetch, error handling
│   ├── tickets.ts
│   ├── comments.ts
│   └── users.ts
├── pages/
├── components/
└── types/
```

Each `api/*.ts` module mirrors one backend resource — keeps pages free of URL/binding details.
