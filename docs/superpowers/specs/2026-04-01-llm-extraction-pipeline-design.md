# LLM-Based Game Data Extraction Pipeline

## Problem

The current extraction pipeline uses ~200 regex patterns across 13 Python scripts to parse decompiled C# source code into structured JSON. This approach:

- Silently misses information that doesn't match known patterns (e.g., DollRoom's act restriction, multi-attack hit counts)
- Can't represent mechanics that don't fit the fixed JSON schema
- Requires ongoing maintenance as the game code evolves
- Produces errors that are only caught when a human reads the wiki page

## Solution

Replace regex-based extraction with an LLM agent (Claude Sonnet) that reads decompiled source code, produces structured data, and reviews the generated wiki pages — with the ability to fix problems it finds.

## Architecture

### Per-Entity Agent

Each game entity (event, monster, card, etc.) is processed by a single coherent agent that maintains context across the full pipeline for that entity:

1. **Read** the decompiled .cs source file and relevant localization entries
2. **Write** structured JSON data to `data/{version}/{type}.json` (or a per-entity file)
3. **Trigger** page regeneration from the updated data
4. **Review** the rendered page against the source code
5. **Fix** any problems by updating data, creating/editing override files, or both
6. **Repeat** review until satisfied (max 2 fix iterations, then escalate)

The agent keeps context across all steps, so it remembers what it saw in the source when reviewing the page.

### Orchestrator

A Python script (`scripts/llm_extract.py`) manages the pipeline:

- Walks the decompiled source directory for a given entity type
- Computes cache keys and skips unchanged entities
- Spins up one agent conversation per entity
- Provides tools for the agent to interact with the repo
- Collects results and reports changes

### Agent Tools

The agent gets these tools:

- **`read_file(path)`** — read any file in the repo (source, localization, data, pages, rendered HTML)
- **`write_data(path, content)`** — write to data files or override files
- **`rebuild_page(entity_type, slug)`** — runs the generator for a specific entity and returns the rendered page content
- **`report_issue(description)`** — escalate to human when the agent can't fix a problem

Path constraints enforced by the orchestrator: writes only to `data/` and `overrides/`, reads anywhere in the repo.

### Per-Page Overrides

When the agent finds something that can't be represented in the JSON schema, it creates an override file:

```
overrides/events/doll-room.md
overrides/monsters/fake-merchant.md
```

These contain additional content (notes, corrections, extra sections) that the page generator merges into the output. The agent creates and edits these as needed.

This replaces the current `scripts/monster_notes.py` pattern with a general-purpose mechanism.

### Prompts

Each entity type has a prompt file (`scripts/prompts/extract_{type}.md`) that tells the agent:

- What kind of entity it's processing
- The target JSON schema with field descriptions
- What to look for in the source code
- What the localization data contains
- Examples of correct output for representative entities
- Common pitfalls to watch for
- What a good wiki page looks like for this entity type

These prompts are versioned and included in the cache key hash.

### Caching

Cache key = hash of (source file content + relevant localization entries + prompt file content).

Stored in `data/.llm_cache.json` as a mapping from `(entity_type, class_name)` to `{cache_key, last_processed}`.

When any input changes (source code, localization, or prompt), the entity is reprocessed. A `--force` flag reprocesses everything.

## Incremental Rollout

The LLM pipeline is built alongside the existing regex pipeline, not as a replacement. For each entity type:

1. Build the prompt and test on a few representative entities
2. Compare LLM output against existing data to find improvements and regressions
3. Once confident, switch that entity type to LLM extraction
4. Delete the corresponding `extract_{type}.py` script

### Starting with Events

Events are the most variable entity type and the most problematic with regex extraction. Start with:

1. Doll Room — the event that prompted this redesign
2. A few other events of varying complexity (simple, medium, complex)
3. All events

Then extend to monsters, enchantments, and the rest.

## What Stays the Same

- Astro templates and content.config.ts schema (extended as needed)
- The site build process
- The justfile orchestration (new targets added)
- The localization extraction (extract_pck.py)
- The decompilation step
- The image extraction

## What Changes

- `extract_*.py` scripts → LLM agents (gradual migration per entity type)
- `monster_notes.py` → generalized override system
- Hardcoded enrichments in `generate_events.py` → LLM review + overrides
- Data files may gain new fields as the LLM extracts richer information

## Open Questions

- Should the per-entity data be individual JSON files or entries in the existing combined files (e.g., `data/v0.101.0/events.json`)? Individual files are simpler for caching but the existing system uses combined files.
- How should the override files be formatted? Plain markdown that gets appended? YAML frontmatter overrides? A mix?
- Should the agent have access to other entities' data for cross-referencing (e.g., an event that references a relic needs to know the relic's title)?
