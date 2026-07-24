# Design Notes

## Goals

- Simple, maintainable ticket management architecture.
- Clear separation between API, data access, and UI.
- Testable APIs via integration tests.

## Proposed architecture

```
React (UI)
    │
    ▼
ASP.NET Core Web API
    │
    ▼
EF Core 8 → Database
```

## Backend design

- Controllers / minimal APIs for ticket resources.
- EF Core entities and DbContext for persistence.
- DTOs for request/response to keep API contract stable.
- Centralized validation and problem-details style errors where practical.

## Frontend design

- React app with ticket list, create, and detail/update views.
- Thin API client layer.
- Status and priority presented clearly in the UI.

## Decisions log

| Date | Decision | Rationale |
|------|----------|-----------|
| | | |

## Alternatives considered

_TBD as implementation choices are made (e.g., auth, hosting DB, state management)._
