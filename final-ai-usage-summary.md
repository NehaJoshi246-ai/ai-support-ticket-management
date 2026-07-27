# Final AI Usage Summary

Honest record of how AI was used on this project — including prompts that failed, docs that drifted from code, and corrections that came from human review.

---

## How AI was used (by phase)

| Phase | AI role | Human role |
|-------|---------|------------|
| Repo staging | Generated doc scaffolds and folder layout | Specified assessment stack and file list |
| Requirements / design | Drafted API contract, state machine, layered architecture | Locked assessment option and transition rules |
| Backend | Scaffolded solution, endpoints, validation, seed | Ran builds, smoke tests, steered naming (`SupportTickets.*`) |
| Debugging | Traced 404 vs 500 paths; wrote `debugging-notes.md` | Reported symptom; validated repro before accepting fix narrative |
| Code review | Listed service risks + **rejected** weak findings | Asked for list-only review; owns fix priority |
| Docs | Multiple full-doc sync passes | **"Update all file according to code"** — forced alignment |

---

## Prompt iteration log (chronological)

Each row: what was asked → what happened → correction if any.

| # | Prompt / intent | AI outcome | Failure or course correction |
|---|-----------------|------------|------------------------------|
| 1 | Create root docs before code | 17 markdown files + folder plan | ✅ As intended |
| 2 | Add `src/`, `tests/`, `database/`, `ai-prompts/` tree | Folders + prompt stubs created | ✅ |
| 3 | `.gitignore` for .NET + React | Standard ignore file | ✅ |
| 4 | Lock assessment entities + state machine | Updated requirements/design docs | ✅ Scope stable afterward |
| 5 | Design layered API + SQLite + separate PATCH status | Rich `design-notes.md` with `backend/Application/` layout | ⚠️ **Design ahead of implementation** — code never added Application project |
| 6 | Scaffold .NET solution (Api, Domain, Infrastructure) | `SupportTickets.sln`, 3 projects, SQLite | ⚠️ Simpler than design doc (`SupportTickets.Api` not `backend/Api`) |
| 7 | Build failed: `Migrate()` not found | Added `using Microsoft.EntityFrameworkCore` | 🔧 Tooling fix |
| 8 | `dotnet-ef` missing | Installed global tool | 🔧 Environment fix |
| 9 | Build failed: API process locking DLLs | Stopped running API before rebuild | 🔧 Local dev friction |
| 10 | Seed 10 users + sample tickets | `DataSeeder` in migration | ✅ |
| 11 | Ticket CRUD + field validation | `TicketService`, DTOs, 400 ProblemDetails | ✅ |
| 12 | Status via dedicated PATCH, not PUT | `TransitionMap`, `InvalidTransitionException`, 409 | ✅ Human chose **409 over 400** for invalid transitions |
| 13 | Comment endpoints + 404 if ticket missing | Controller try/catch + `EnsureTicketExistsAsync` | ✅ |
| 14 | Frontend: list, detail, create, loading states | Started Vite scaffold; **interrupted** | ❌ **No `src/frontend/` in repo** — largest incomplete deliverable |
| 15 | "Create schema" | Added `schema.sql` + `schema.md` | ↩️ User: **"undo last changes"** — files removed; schema stays EF migration only |
| 16 | Comment POST → 500, expected 404 | Investigated; current code returns 404 on repro | ⚠️ User paste of error was empty (`[paste]`); doc captures *likely* 500 causes if check/catch missing |
| 17 | Review services — **don't fix**, list only | `code-review-notes.md` with accepted + **rejected** items | ✅ Human ownership signal |
| 18 | README setup outline for clean machine | Section stubs in `readme.md` | ⬜ Commands still placeholders for human |
| 19 | `database/setup-notes.md` fill-in | Connection string, migrate, seed, persistence | ✅ |
| 20 | Integration tests — list 25 transition pairs first | Full matrix table for human marking | ⬜ Tests not written yet — deliberate pause |
| 21 | **"Update all file according to code"** | Mass doc sync: removed ghost Users API, Application layer, frontend-as-done | 🔧 **Major correction** — docs had described aspirational architecture |
| 22 | Update `planning.md` | Phase checklist synced to repo | ✅ |
| 23 | Capture prompt iteration including failures | This document | ✅ |

---

## Failures (what went wrong)

### 1. Documentation drift

**Symptom:** `design-notes.md`, `api-contract.md`, `implementation-plan.md` described:

- `backend/Application/` project and `ITicketService` interfaces
- `UsersController` / `GET /api/users` as if shipped
- `allowedNextStatuses` on ticket JSON
- `TicketStatusTransitionRules` (actual code: `TransitionMap`)

**Cause:** Design prompts ran before implementation; partial features discussed in chat never committed.

**Correction:** User prompt #21 — full doc pass against `src/`. `cursor-rules-or-instructions.md` now says not to document Users API until it exists.

### 2. Frontend not delivered

**Symptom:** `ui-flow.md` and acceptance criteria implied UI; no React app in tree.

**Cause:** `npm create vite` + long-running shell tasks interrupted; priority stayed on API.

**Correction:** Docs now mark frontend ⬜; `ui-flow.md` lists which API endpoints exist today.

### 3. Schema files reverted

**Symptom:** User asked for schema, then undo.

**Cause:** Likely preferred EF migrations as single source of truth, or unwanted duplicate of migration SQL.

**Correction:** `database/schema/` back to `.gitkeep` only; `setup-notes.md` points to migrations.

### 4. Shell / environment friction

- First `dotnet build` after scaffold: missing EF using, file locks from running API.
- `sqlite3` CLI not on Windows PATH — used Python for DB row counts.
- Background API tasks exited when process killed — expected, not a code bug.

### 5. Incomplete error report loop

User reported comment → 500 with `[paste]` placeholder but no stack trace. AI repro showed 404 on current code. **Debugging-notes** documents both outcomes so future readers know to check exception *type* first.

---

## Course corrections (human steering)

| Human input | Why it mattered |
|-------------|-----------------|
| Assessment option + explicit state machine | Stopped generic ticket CRUD scope creep |
| PATCH status **separate** from PUT | Enforced in code and tests plan |
| "Don't fix, just list" for code review | Produced rejected findings — shows judgment, not blind merge |
| "List transition pairs before tests" | Avoided AI dumping 25 tests without review |
| "Undo last changes" (schema) | Reverted unwanted artifact quickly |
| "Update all file according to code" | Fixed systematic doc/code mismatch |
| 409 vs 400 for invalid transitions | Documented HTTP semantics choice in API contract |

---

## Rejected AI suggestions (ownership)

From `code-review-notes.md` — intentionally **not** turned into tasks:

| Suggestion | Why rejected |
|------------|--------------|
| Move services to Application layer immediately | Architecture preference; not a runtime bug |
| Repository pattern over DbContext | YAGNI for assessment size |
| Same-status PATCH → 409 | Idempotent 200 is valid API design |
| Mandatory `CreatedBy` null-forgiving entity change | Data corruption case, not normal path |

---

## What worked well

- **Docs-before-code** staging made assessment intent clear to reviewers.
- **TransitionMap** in Domain + dedicated PATCH endpoint — easy to explain and test.
- **Field-level 400** via `ValidationProblemDetails` — good API UX without custom middleware.
- **DataSeeder** across all statuses — instant demo data on first run.
- **Iterative debugging doc** — teaches 404 vs 500 without rewriting controller blindly.
- **Doc sync pass** — single prompt realigned 15+ files to `SupportTickets.*` reality.

---

## What AI did not do (still open)

- React UI (`src/frontend/ticket-ui/`)
- `GET /api/users`
- Integration test project + 25-pair matrix automation
- Review fixes: concurrency token, shared `TicketResponse` mapper
- Filled README command blocks (outline only)
- `allowedNextStatuses` on live API response

---

## Assessment (meta)

AI was strongest at **scaffolding and API implementation** — hours of boilerplate (entities, EF config, controllers, validation) in one session. It was weakest at **keeping docs and repo in sync** without an explicit human audit; the design conversation created a parallel architecture that did not match the simpler three-project solution.

**Sign of ownership:** asking for failures in this summary, undoing schema, rejecting review items, demanding docs match code, and pausing integration tests until the transition matrix is human-approved.

---

## Related artifacts

| File | Contents |
|------|----------|
| [ai-prompts/planning.md](ai-prompts/planning.md) | Phase status + next prompts |
| [debugging-notes.md](debugging-notes.md) | Comment 404 investigation |
| [code-review-notes.md](code-review-notes.md) | Accepted/rejected findings |
| [ai-prompts/code-review.md](ai-prompts/code-review.md) | Review prompt + outcomes |
| [reflection.md](reflection.md) | Personal takeaways |
