#!/usr/bin/env python3
"""Regenerate SESSION-INDEX.md and readable markdown from raw JSONL transcripts."""

from __future__ import annotations

import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RAW_DIR = ROOT / "raw"
READABLE_DIR = ROOT / "readable"
INDEX_PATH = ROOT / "SESSION-INDEX.md"

TIMESTAMP_RE = re.compile(r"<timestamp>(.*?)</timestamp>", re.DOTALL)
USER_QUERY_RE = re.compile(r"<user_query>\s*(.*?)\s*</user_query>", re.DOTALL)


def extract_user_turn(text: str) -> tuple[str | None, str]:
    ts_match = TIMESTAMP_RE.search(text)
    timestamp = ts_match.group(1).strip() if ts_match else None
    uq_match = USER_QUERY_RE.search(text)
    if uq_match:
        body = uq_match.group(1).strip()
    else:
        body = text.strip()
    return timestamp, body


def extract_assistant_text(content: list[dict]) -> tuple[str, int]:
    parts: list[str] = []
    tool_count = 0
    for block in content:
        if block.get("type") == "text" and block.get("text"):
            parts.append(block["text"].strip())
        elif block.get("type") == "tool_use":
            tool_count += 1
    return "\n\n".join(parts), tool_count


def first_line_preview(text: str, max_len: int = 120) -> str:
    line = text.replace("\n", " ").strip()
    if len(line) <= max_len:
        return line
    return line[: max_len - 1] + "…"


def process_session(jsonl_path: Path) -> dict:
    user_turns: list[dict] = []
    readable_lines: list[str] = [
        f"# Readable transcript: {jsonl_path.name}",
        "",
        f"Source: `raw/{jsonl_path.name}` (JSONL — canonical raw record).",
        "",
        "Assistant tool calls are summarized as counts; open the JSONL for full tool inputs/outputs.",
        "",
        "---",
        "",
    ]
    turn = 0

    with jsonl_path.open(encoding="utf-8") as f:
        for line_no, line in enumerate(f, 1):
            line = line.strip()
            if not line:
                continue
            obj = json.loads(line)
            role = obj.get("role")
            content = obj.get("message", {}).get("content", [])

            if role == "user":
                text = content[0].get("text", "") if content else ""
                timestamp, body = extract_user_turn(text)
                turn += 1
                user_turns.append(
                    {
                        "turn": turn,
                        "line": line_no,
                        "timestamp": timestamp,
                        "preview": first_line_preview(body),
                        "body": body,
                    }
                )
                readable_lines.append(f"## Turn {turn} — User")
                if timestamp:
                    readable_lines.append(f"**When:** {timestamp}")
                readable_lines.append("")
                readable_lines.append(body)
                readable_lines.append("")

            elif role == "assistant":
                text, tool_count = extract_assistant_text(content)
                readable_lines.append(f"### Assistant (JSONL line {line_no})")
                if tool_count:
                    readable_lines.append(f"*{tool_count} tool call(s) in raw transcript.*")
                readable_lines.append("")
                if text:
                    readable_lines.append(text)
                else:
                    readable_lines.append("_No assistant text in this record (tool-only turn)._")
                readable_lines.append("")
                readable_lines.append("---")
                readable_lines.append("")

    return {
        "file": jsonl_path.name,
        "bytes": jsonl_path.stat().st_size,
        "user_turns": user_turns,
        "readable": "\n".join(readable_lines),
    }


def build_index(sessions: list[dict]) -> str:
    lines = [
        "# Session index (generated)",
        "",
        "Chronological user prompts extracted from raw JSONL transcripts.",
        "Regenerate: `python ai-prompts/transcripts/scripts/export-transcripts.py`",
        "",
        "## Sessions",
        "",
        "| File | Size | User turns | Notes |",
        "|------|------|------------|-------|",
    ]

    notes = {
        "session-84f66eb1-primary.jsonl": "Primary Cursor session through Jul 27 (integration tests, reflection)",
        "session-e7026414-fork.jsonl": "Parallel/fork session; overlaps early prompts; includes commit-and-push turns",
    }

    for s in sessions:
        lines.append(
            f"| [`raw/{s['file']}`](raw/{s['file']}) | {s['bytes']:,} bytes | {len(s['user_turns'])} | {notes.get(s['file'], '')} |"
        )

    lines.extend(["", "## User prompts by session", ""])

    for s in sessions:
        lines.append(f"### {s['file']}")
        lines.append("")
        lines.append("| Turn | Line | When | Preview |")
        lines.append("|------|------|------|---------|")
        for t in s["user_turns"]:
            when = t["timestamp"] or "—"
            preview = t["preview"].replace("|", "\\|")
            lines.append(
                f"| {t['turn']} | {t['line']} | {when} | {preview} |"
            )
        lines.append("")

    return "\n".join(lines)


def main() -> None:
    READABLE_DIR.mkdir(parents=True, exist_ok=True)

    sessions = []
    for jsonl_path in sorted(RAW_DIR.glob("*.jsonl")):
        data = process_session(jsonl_path)
        sessions.append(data)
        readable_path = READABLE_DIR / jsonl_path.name.replace(".jsonl", ".md")
        readable_path.write_text(data["readable"], encoding="utf-8")

    INDEX_PATH.write_text(build_index(sessions), encoding="utf-8")
    print(f"Wrote {INDEX_PATH}")
    for s in sessions:
        print(f"  {s['file']}: {len(s['user_turns'])} user turns")


if __name__ == "__main__":
    main()
