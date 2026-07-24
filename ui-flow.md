# UI Flow

## Primary screens (draft)

1. **Ticket list** — browse tickets with status/priority indicators.
2. **Create ticket** — form to submit a new ticket.
3. **Ticket detail / edit** — view and update a single ticket.

## Happy paths

### Create ticket

1. User opens Create Ticket.
2. Enters title, description, priority.
3. Submits form → `POST /api/tickets`.
4. On success, redirect to list or detail with confirmation.

### View tickets

1. User opens Ticket List.
2. UI loads `GET /api/tickets`.
3. User selects a ticket → detail view via `GET /api/tickets/{id}`.

### Update ticket

1. User opens ticket detail.
2. Changes status, priority, or fields as allowed.
3. Saves → `PUT/PATCH /api/tickets/{id}`.
4. UI refreshes with updated data.

## Error / empty states

- Empty list message when no tickets exist.
- Validation errors shown on create/update forms.
- Not-found and API failure messaging on detail/list.

## Navigation (draft)

```
Ticket List ──► Create Ticket
     │
     └──► Ticket Detail / Edit
```
