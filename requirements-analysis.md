# Requirements Analysis

## Overview

Build a support ticket management system with:

- **Backend:** ASP.NET Core Web API
- **Frontend:** React
- **Database:** EF Core 8
- **Testing:** Integration tests

## Functional requirements

_To be refined from product brief / assignment._

1. Create, view, update, and manage support tickets.
2. Assign tickets to agents / users as applicable.
3. Track ticket status and priority.
4. Persist ticket data via EF Core 8.
5. Expose REST APIs consumed by the React UI.
6. Cover critical flows with integration tests.

## Non-functional requirements

- Clear API contract and consistent error handling.
- Maintainable project structure (API, data, UI, tests).
- Documented design decisions and test results.

## Out of scope (initial)

_TBD — capture exclusions as requirements are clarified._

## Open questions

- Authentication / authorization model?
- Ticket lifecycle states and transitions?
- Roles (customer, agent, admin)?
- Search, filtering, and pagination requirements?
