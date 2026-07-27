# AI session transcripts

Full traceability for human prompts and AI responses on this project.

## What is preserved

| Artifact | Path | Purpose |
|----------|------|---------|
| **Raw JSONL** (canonical) | [`raw/`](raw/) | One JSON object per line: `role`, `message` (user text + assistant text + tool calls) |
| **Readable markdown** | [`readable/`](readable/) | Same turns in prose; tool calls summarized |
| **Session index** | [`SESSION-INDEX.md`](SESSION-INDEX.md) | Turn-by-turn table with line numbers into JSONL |
| **Curated summary** | [`../final-ai-usage-summary.md`](../../final-ai-usage-summary.md) | Failures, course corrections, rejected suggestions |
| **Phase prompts** | [`../planning.md`](../planning.md), [`../implementation.md`](../implementation.md), etc. | Intent per phase (not every chat turn) |

## Sessions

| File | Session ID | Span |
|------|------------|------|
| [`raw/session-84f66eb1-primary.jsonl`](raw/session-84f66eb1-primary.jsonl) | `84f66eb1-…` | Jul 24–27, 2026 — primary thread (staging → API → tests → reflection) |
| [`raw/session-e7026414-fork.jsonl`](raw/session-e7026414-fork.jsonl) | `e7026414-…` | Jul 24–27, 2026 — fork/parallel thread (overlaps early work; extra commit/push turns) |

Cursor stores live copies under the IDE agent-transcripts folder; files in `raw/` are **repo snapshots** for reviewers who clone without Cursor metadata.

## JSONL format (per line)

```json
{
  "role": "user" | "assistant",
  "message": {
    "content": [
      { "type": "text", "text": "<timestamp>…</timestamp>\n<user_query>…</user_query>" },
      { "type": "tool_use", "name": "Shell", "input": { … } }
    ]
  }
}
```

- **User** records: timestamp + full prompt (may include system reminders in older exports).
- **Assistant** records: narrative text plus tool invocations (file writes, shell commands, etc.).

## How to use

1. **Find a decision** — open [`SESSION-INDEX.md`](SESSION-INDEX.md), locate the turn preview, note JSONL **line** number.
2. **Read raw** — open the matching `raw/*.jsonl` at that line (or use `readable/*.md` for quick reading).
3. **Cross-check summary** — [`final-ai-usage-summary.md`](../../final-ai-usage-summary.md) prompt # column maps to the same chronology.

## Regenerate after new exports

Copy updated JSONL from Cursor into `raw/`, then:

```bash
python ai-prompts/transcripts/scripts/export-transcripts.py
```

This refreshes `SESSION-INDEX.md` and `readable/*.md` without editing them by hand.

## Policy

- Do **not** edit raw JSONL after export (treat as immutable audit log).
- If a prompt contained secrets, redact in a **new** file and note the redaction in this README — do not rewrite history silently.
- Summaries (`final-ai-usage-summary.md`, `reflection.md`) are interpretive; **raw JSONL is the source of truth** for what was actually asked and returned.
