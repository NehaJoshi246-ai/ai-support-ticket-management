# Session index (generated)

Chronological user prompts extracted from raw JSONL transcripts.
Regenerate: `python ai-prompts/transcripts/scripts/export-transcripts.py`

## Sessions

| File | Size | User turns | Notes |
|------|------|------------|-------|
| [`raw/session-84f66eb1-primary.jsonl`](raw/session-84f66eb1-primary.jsonl) | 397,363 bytes | 26 | Primary Cursor session through Jul 27 (integration tests, reflection) |
| [`raw/session-e7026414-fork.jsonl`](raw/session-e7026414-fork.jsonl) | 249,233 bytes | 46 | Parallel/fork session; overlaps early prompts; includes commit-and-push turns |

## User prompts by session

### session-84f66eb1-primary.jsonl

| Turn | Line | When | Preview |
|------|------|------|---------|
| 1 | 1 | Friday, Jul 24, 2026, 12:26 PM (UTC+5:30) | I am creating a project support ticket management system : Backend: ASP.NET Core Web API Frontend: React  Database: EF … |
| 2 | 5 | Friday, Jul 24, 2026, 12:33 PM (UTC+5:30) | create folder  src/ tests/ database/ database/setup-notes.md database/schema/ database/seed-data/ ai-prompts/ ai-prompt… |
| 3 | 10 | Friday, Jul 24, 2026, 12:40 PM (UTC+5:30) | add .gitignore appropriate for a .net core + reactproject (bin/ obje/, node_modules/, ./env appsettings.development.jso… |
| 4 | 13 | Friday, Jul 24, 2026, 1:26 PM (UTC+5:30) | Support management system option from my assessment.  Entities: User (seeded only) Ticket(id,title, description, priori… |
| 5 | 17 | Friday, Jul 24, 2026, 1:41 PM (UTC+5:30) | Let's design : I'm doing ASP.NET Core Web API for the backend, EF Core with SQLite for local persistence, and a React f… |
| 6 | 21 | Friday, Jul 24, 2026, 1:49 PM (UTC+5:30) | Scaffold the .NET solution now inside src/. I want a Web API project called SupportTickets.Api, a class library Support… |
| 7 | 37 | Friday, Jul 24, 2026, 2:03 PM (UTC+5:30) | Add the User, Ticket, and Comment entities to SupportTickets.Domain based on the fields I gave you earlier.  Set up the… |
| 8 | 47 | Friday, Jul 24, 2026, 2:07 PM (UTC+5:30) | Implement the ticket endpoints: POST to create, GET list, GET by id for detail, PUT to update title/description/priorit… |
| 9 | 62 | Friday, Jul 24, 2026, 2:13 PM (UTC+5:30) | Now the status transition endpoint. I want a TransitionMap that defines exactly which status can move to which — Open→I… |
| 10 | 67 | Friday, Jul 24, 2026, 2:31 PM (UTC+5:30) | Add comment endpoints — POST to add a comment to a ticket, GET to list comments for a ticket. Validate the ticket exist… |
| 11 | 71 | Friday, Jul 24, 2026, 2:35 PM (UTC+5:30) | Now the frontend. I need: a ticket list page with search box and status filter, a ticket detail page showing the ticket… |
| 12 | 77 | Friday, Jul 24, 2026, 4:19 PM (UTC+5:30) | I need integration tests proving the state machine — every valid transition should succeed, every invalid one should be… |
| 13 | 80 | Friday, Jul 24, 2026, 4:22 PM (UTC+5:30) | I'm getting a 500 when adding a comment to a ticket ID that doesn't exist — I expected a 404. Here's the controller act… |
| 14 | 90 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 15 | 92 | Friday, Jul 24, 2026, 4:28 PM (UTC+5:30) | Review my TicketService and TransitionService classes for real issues — missing validation, possible null refs, anythin… |
| 16 | 98 | Friday, Jul 24, 2026, 4:29 PM (UTC+5:30) | Help me draft a README setup section for someone cloning this with a clean machine — just the .NET SDK and Node install… |
| 17 | 103 | Monday, Jul 27, 2026, 12:40 PM (UTC+5:30) | Create schema |
| 18 | 107 | Monday, Jul 27, 2026, 1:26 PM (UTC+5:30) | undo last changes |
| 19 | 110 | Monday, Jul 27, 2026, 1:43 PM (UTC+5:30) | update planning.md file |
| 20 | 116 | Monday, Jul 27, 2026, 1:49 PM (UTC+5:30) | update all file according to code |
| 21 | 127 | Monday, Jul 27, 2026, 2:00 PM (UTC+5:30) | Capture real prompt iteration including failures and course corrections |
| 22 | 131 | Monday, Jul 27, 2026, 2:02 PM (UTC+5:30) | Implement the 25-pair state-machine matrix as WebApplicationFactory tests |
| 23 | 139 | Monday, Jul 27, 2026, 2:02 PM (UTC+5:30) | Implement the 25-pair state-machine matrix as WebApplicationFactory tests |
| 24 | 165 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 25 | 171 | Monday, Jul 27, 2026, 2:40 PM (UTC+5:30) | Deepen reflection with concrete decision trade-offs and demo evidence |
| 26 | 178 | Monday, Jul 27, 2026, 2:42 PM (UTC+5:30) | Preserve raw prompt/response transcripts for full traceability |

### session-e7026414-fork.jsonl

| Turn | Line | When | Preview |
|------|------|------|---------|
| 1 | 1 | Friday, Jul 24, 2026, 12:26 PM (UTC+5:30) | I am creating a project support ticket management system : Backend: ASP.NET Core Web API Frontend: React  Database: EF … |
| 2 | 5 | Friday, Jul 24, 2026, 12:33 PM (UTC+5:30) | create folder  src/ tests/ database/ database/setup-notes.md database/schema/ database/seed-data/ ai-prompts/ ai-prompt… |
| 3 | 10 | Friday, Jul 24, 2026, 12:40 PM (UTC+5:30) | add .gitignore appropriate for a .net core + reactproject (bin/ obje/, node_modules/, ./env appsettings.development.jso… |
| 4 | 13 | Friday, Jul 24, 2026, 1:26 PM (UTC+5:30) | Support management system option from my assessment.  Entities: User (seeded only) Ticket(id,title, description, priori… |
| 5 | 17 | Friday, Jul 24, 2026, 1:41 PM (UTC+5:30) | Let's design : I'm doing ASP.NET Core Web API for the backend, EF Core with SQLite for local persistence, and a React f… |
| 6 | 21 | Friday, Jul 24, 2026, 1:49 PM (UTC+5:30) | Scaffold the .NET solution now inside src/. I want a Web API project called SupportTickets.Api, a class library Support… |
| 7 | 37 | Friday, Jul 24, 2026, 2:03 PM (UTC+5:30) | Add the User, Ticket, and Comment entities to SupportTickets.Domain based on the fields I gave you earlier.  Set up the… |
| 8 | 47 | Friday, Jul 24, 2026, 2:07 PM (UTC+5:30) | Implement the ticket endpoints: POST to create, GET list, GET by id for detail, PUT to update title/description/priorit… |
| 9 | 62 | Friday, Jul 24, 2026, 2:13 PM (UTC+5:30) | Now the status transition endpoint. I want a TransitionMap that defines exactly which status can move to which — Open→I… |
| 10 | 67 | Friday, Jul 24, 2026, 2:31 PM (UTC+5:30) | Add comment endpoints — POST to add a comment to a ticket, GET to list comments for a ticket. Validate the ticket exist… |
| 11 | 71 | Friday, Jul 24, 2026, 2:35 PM (UTC+5:30) | Now the frontend. I need: a ticket list page with search box and status filter, a ticket detail page showing the ticket… |
| 12 | 77 | Friday, Jul 24, 2026, 3:41 PM (UTC+5:30) | I need integration tests proving the state machine — every valid transition should succeed, every invalid one should be… |
| 13 | 81 | Friday, Jul 24, 2026, 3:43 PM (UTC+5:30) | Execute the selected diff-tab commit action. |
| 14 | 94 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 15 | 95 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 16 | 96 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 17 | 97 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 18 | 100 | Friday, Jul 24, 2026, 3:58 PM (UTC+5:30) | Execute the selected diff-tab commit-and-push action. |
| 19 | 106 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 20 | 107 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 21 | 108 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 22 | 111 | Friday, Jul 24, 2026, 4:52 PM (UTC+5:30) | Execute the selected diff-tab commit-and-push action. |
| 23 | 116 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 24 | 117 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 25 | 118 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 26 | 121 | Monday, Jul 27, 2026, 1:13 PM (UTC+5:30) | Execute the selected diff-tab commit-and-push action. |
| 27 | 127 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 28 | 128 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 29 | 129 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 30 | 130 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 31 | 133 | Monday, Jul 27, 2026, 1:27 PM (UTC+5:30) | Execute the selected diff-tab commit-and-push action. |
| 32 | 139 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 33 | 140 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 34 | 141 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 35 | 144 | Monday, Jul 27, 2026, 1:44 PM (UTC+5:30) | Execute the selected diff-tab commit-and-push action. |
| 36 | 150 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 37 | 151 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 38 | 152 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 39 | 155 | Monday, Jul 27, 2026, 1:53 PM (UTC+5:30) | Execute the selected diff-tab commit-and-push action. |
| 40 | 160 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 41 | 161 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 42 | 162 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 43 | 165 | Monday, Jul 27, 2026, 2:34 PM (UTC+5:30) | Execute the selected diff-tab commit-and-push action. |
| 44 | 171 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 45 | 172 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
| 46 | 173 | — | Briefly inform the user about the task result and perform any follow-up actions (if needed). If there's no follow-ups n… |
