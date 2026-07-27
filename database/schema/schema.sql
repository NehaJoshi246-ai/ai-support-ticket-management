-- Support Ticket Management — SQLite schema
-- Generated from EF Core migration: 20260724083521_InitialCreate
-- Source of truth for runtime: src/SupportTickets.Infrastructure/Migrations/

PRAGMA foreign_keys = ON;

-- ---------------------------------------------------------------------------
-- Users (seeded only — no runtime write APIs)
-- Role: 0=Customer, 1=Agent, 2=Lead, 3=Admin
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Users" (
    "Id"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "Name"  TEXT    NOT NULL,
    "Email" TEXT    NOT NULL,
    "Role"  INTEGER NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");

-- ---------------------------------------------------------------------------
-- Tickets
-- Priority: 0=Low, 1=Medium, 2=High, 3=Critical
-- Status:   0=Open, 1=InProgress, 2=Resolved, 3=Closed, 4=Cancelled
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Tickets" (
    "Id"           INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "Title"        TEXT    NOT NULL,
    "Description"  TEXT    NOT NULL,
    "Priority"     INTEGER NOT NULL,
    "Status"       INTEGER NOT NULL,
    "AssignedToId" INTEGER NULL,
    "CreatedById"  INTEGER NOT NULL,
    "CreatedAt"    TEXT    NOT NULL,
    CONSTRAINT "FK_Tickets_Users_AssignedToId"
        FOREIGN KEY ("AssignedToId") REFERENCES "Users" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Tickets_Users_CreatedById"
        FOREIGN KEY ("CreatedById") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS "IX_Tickets_AssignedToId" ON "Tickets" ("AssignedToId");
CREATE INDEX IF NOT EXISTS "IX_Tickets_CreatedById"  ON "Tickets" ("CreatedById");
CREATE INDEX IF NOT EXISTS "IX_Tickets_Priority"     ON "Tickets" ("Priority");
CREATE INDEX IF NOT EXISTS "IX_Tickets_Status"       ON "Tickets" ("Status");

-- ---------------------------------------------------------------------------
-- TicketComments
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "TicketComments" (
    "Id"          INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "TicketId"    INTEGER NOT NULL,
    "Body"        TEXT    NOT NULL,
    "CreatedById" INTEGER NOT NULL,
    "CreatedAt"   TEXT    NOT NULL,
    CONSTRAINT "FK_TicketComments_Tickets_TicketId"
        FOREIGN KEY ("TicketId") REFERENCES "Tickets" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TicketComments_Users_CreatedById"
        FOREIGN KEY ("CreatedById") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS "IX_TicketComments_TicketId"    ON "TicketComments" ("TicketId");
CREATE INDEX IF NOT EXISTS "IX_TicketComments_CreatedById" ON "TicketComments" ("CreatedById");

-- ---------------------------------------------------------------------------
-- EF Core migration history (created automatically by Migrate())
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId"    TEXT NOT NULL PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);
