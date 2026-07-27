# AI Prompts — Code Review

## Purpose

Self-review of service layer with accepted and rejected findings.

## Prompt used

```
Review TicketService and TicketStatusTransitionService for validation gaps,
null refs, and concurrent update risks. Do not fix — list for human review.
Include rejected suggestions with rationale.
```

## Outcomes

Full review: [code-review-notes.md](../code-review-notes.md)

**Accepted highlights:** no concurrency token, PUT+PATCH races, duplicated mapper, missing users API.

**Rejected highlights:** mandatory Application layer move, same-status 409, repository pattern without need.

## Follow-up

Track fixes in [review-fixes.md](../review-fixes.md) — none applied yet.

## Iteration note

Human explicitly blocked auto-fix — part of ownership story in [final-ai-usage-summary.md](../final-ai-usage-summary.md).
