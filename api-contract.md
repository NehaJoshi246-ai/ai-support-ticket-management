# API Contract

Base URL: `/api` (tentative)

## Resources

### Tickets

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/tickets` | List tickets |
| GET | `/api/tickets/{id}` | Get ticket by id |
| POST | `/api/tickets` | Create ticket |
| PUT / PATCH | `/api/tickets/{id}` | Update ticket |
| DELETE | `/api/tickets/{id}` | Delete ticket (if in scope) |

## Ticket payload (draft)

```json
{
  "id": "guid-or-int",
  "title": "string",
  "description": "string",
  "status": "Open | InProgress | Resolved | Closed",
  "priority": "Low | Medium | High | Critical",
  "createdAt": "ISO-8601",
  "updatedAt": "ISO-8601",
  "assigneeId": "optional"
}
```

## Create request (draft)

```json
{
  "title": "string (required)",
  "description": "string (required)",
  "priority": "Low | Medium | High | Critical"
}
```

## Error responses (draft)

| Status | When |
|--------|------|
| 400 | Validation failure |
| 404 | Ticket not found |
| 500 | Unexpected server error |

## Notes

- Finalize field types, enums, filtering, and pagination after requirements are locked.
- Keep this document in sync with implemented controllers/endpoints.
