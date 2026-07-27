# AI Prompts — Debugging

## Purpose

Diagnose runtime issues with AI assistance.

## Session: Comment POST → 500 vs 404

**Problem:** Expected 404 when posting comment to non-existent ticket.

**Investigation:** Traced `TicketCommentsController` → `TicketCommentService.EnsureTicketExistsAsync` → `NotFoundException` catch.

**Likely causes documented:**
- Unhandled `DbUpdateException` if existence check skipped (FK failure)
- Uncaught `NotFoundException` if no controller catch

**Validated:** Current code returns 404 for ticket id 99999.

**Full write-up:** [debugging-notes.md](../debugging-notes.md)

## Outcomes

- Root cause framework for 500 vs 404 on nested resources
- Confirmed fix path: explicit existence check + controller catch
