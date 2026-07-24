# Acceptance Criteria

## Documentation staging

- [x] Required root documentation files exist before application code.
- [x] Assessment entities and status state machine captured in requirements/design docs.

## Domain

- [ ] `User` is seeded only; no user write APIs required.
- [ ] `Ticket` has: Id, Title, Description, Priority, Status, AssignedTo, CreatedBy, CreatedAt.
- [ ] New tickets start as **Open**.
- [ ] Status transitions enforce:
  - Open → In Progress | Cancelled
  - In Progress → Resolved | Cancelled
  - Resolved → Closed
- [ ] Invalid status transitions return a client error (e.g. 400).
- [ ] Closed and Cancelled are terminal.

## Backend (ASP.NET Core Web API)

- [ ] API endpoints match `api-contract.md`.
- [ ] Ticket create / list / get / update work end-to-end.
- [ ] Assignment to seeded users works (`AssignedTo`).
- [ ] EF Core 8 persists and retrieves ticket data correctly.
- [ ] Validation and error responses are consistent.

## Frontend (React)

- [ ] UI flows match `ui-flow.md`.
- [ ] Users can create tickets and manage assignment/status through the UI.
- [ ] Status control only offers **valid next** statuses for the current ticket.
- [ ] Priority and status are visible on list and detail.
- [ ] Basic responsive layout works on desktop and mobile.

## Testing

- [ ] Integration tests cover ticket APIs and allowed/rejected status transitions per `test-strategy.md`.
- [ ] Test results are recorded in `test-results.md`.

## Delivery

- [ ] Code review notes and fixes documented.
- [ ] PR description, reflection, and AI usage summary completed.
