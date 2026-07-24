# Database Setup Notes

## Overview

EF Core 8 persistence for Support Ticket Management.

## Entities

- **User** — seeded only
- **Ticket** — Id, Title, Description, Priority, Status, AssignedTo, CreatedBy, CreatedAt

## Status values

Open, InProgress, Resolved, Closed, Cancelled

## Local setup

_TBD after solution scaffolding._

1. Configure connection string.
2. Apply migrations.
3. Ensure Users are seeded (startup seed and/or `seed-data/`).

## Schema

See `schema/` for schema artifacts.

## Seed data

See `seed-data/` for sample users used for CreatedBy / AssignedTo.
