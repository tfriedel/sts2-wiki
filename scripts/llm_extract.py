#!/usr/bin/env python3
"""LLM-based game data extraction using Claude Agent SDK.

Replaces regex-based extraction with an LLM agent that reads decompiled
C# source code, produces structured JSON, and reviews the generated
wiki pages for accuracy.
"""

import argparse
import hashlib
import json
import os
import re
import sys
from pathlib import Path

import anyio
from claude_agent_sdk import ClaudeAgentOptions, ResultMessage, query

PROJECT_ROOT = Path(__file__).resolve().parent.parent

# Maps entity type to the decompiled source subdirectory
ENTITY_TYPE_DIRS: dict[str, str] = {
    "events": "MegaCrit.Sts2.Core.Models.Events",
}

# Maps entity type to the localization file name
ENTITY_TYPE_LOC: dict[str, str] = {
    "events": "events",
}

CACHE_PATH = PROJECT_ROOT / "data" / ".llm_cache.json"
PROMPTS_DIR = PROJECT_ROOT / "scripts" / "prompts"
MODEL = "claude-sonnet-4-6"


def load_cache() -> dict[str, object]:
    if CACHE_PATH.exists():
        result: dict[str, object] = json.loads(CACHE_PATH.read_text())
        return result
    return {}


def save_cache(cache: dict[str, object]) -> None:
    CACHE_PATH.parent.mkdir(parents=True, exist_ok=True)
    CACHE_PATH.write_text(json.dumps(cache, indent=2) + "\n")


def compute_cache_key(source_content: str, loc_entries: str, prompt_content: str) -> str:
    combined = source_content + "\n---LOC---\n" + loc_entries + "\n---PROMPT---\n" + prompt_content
    return hashlib.sha256(combined.encode()).hexdigest()


def load_localization(loc_dir: str, loc_type: str) -> dict[str, str]:
    loc_path = os.path.join(loc_dir, f"{loc_type}.json")
    if not os.path.exists(loc_path):
        return {}
    with open(loc_path) as f:
        result: dict[str, str] = json.load(f)
        return result


def get_loc_entries_for_entity(loc_data: dict[str, str], class_name: str) -> dict[str, str]:
    """Extract localization entries relevant to a specific entity."""
    key = re.sub(r"([a-z])([A-Z])", r"\1_\2", class_name).upper()
    entries = {k: v for k, v in loc_data.items() if k.startswith(f"{key}.")}
    if not entries:
        for k, v in loc_data.items():
            if k.upper().startswith(class_name.upper() + "."):
                entries[k] = v
    return entries


def slugify(name: str) -> str:
    s = name.lower()
    s = re.sub(r"[^a-z0-9]+", "-", s)
    return s.strip("-")


def is_event_class(content: str) -> bool:
    """Check if this .cs file defines an EventModel subclass."""
    return ": EventModel" in content and ": AncientEventModel" not in content


def get_shared_events(decompiled_dir: str) -> set[str]:
    """Find event class names in ModelDb.AllSharedEvents."""
    model_db_path = os.path.join(decompiled_dir, "MegaCrit.Sts2.Core.Models", "ModelDb.cs")
    shared: set[str] = set()
    if os.path.exists(model_db_path):
        with open(model_db_path) as f:
            content = f.read()
        m = re.search(
            r"AllSharedEvents.*?new\s+EventModel\[.*?\{(.*?)\}\)",
            content,
            re.DOTALL,
        )
        if m:
            for em in re.finditer(r"Event<(\w+)>\(\)", m.group(1)):
                shared.add(em.group(1))
    return shared


def get_act_names(decompiled_dir: str) -> list[str]:
    acts_dir = os.path.join(decompiled_dir, "MegaCrit.Sts2.Core.Models.Acts")
    if not os.path.isdir(acts_dir):
        return []
    return sorted(f.removesuffix(".cs") for f in os.listdir(acts_dir) if f.endswith(".cs"))


def get_act_events(decompiled_dir: str) -> dict[str, list[str]]:
    acts_dir = os.path.join(decompiled_dir, "MegaCrit.Sts2.Core.Models.Acts")
    result: dict[str, list[str]] = {}
    if not os.path.isdir(acts_dir):
        return result
    for fname in os.listdir(acts_dir):
        if not fname.endswith(".cs"):
            continue
        act_name = fname.removesuffix(".cs")
        with open(os.path.join(acts_dir, fname)) as f:
            content = f.read()
        for m in re.finditer(r"ModelDb\.Event<(\w+)>\(\)", content):
            event_class = m.group(1)
            if event_class not in result:
                result[event_class] = []
            if act_name not in result[event_class]:
                result[event_class].append(act_name)
    return result


async def process_entity(
    class_name: str,
    source_path: str,
    entity_type: str,
    version: str,
    loc_entries: dict[str, str],
    prompt_content: str,
    act_info: str,
    cache: dict[str, object],
    force: bool,
) -> None:
    """Process a single entity through the LLM agent."""
    with open(source_path) as f:
        source_content = f.read()

    cache_key = compute_cache_key(
        source_content,
        json.dumps(loc_entries, sort_keys=True),
        prompt_content,
    )

    entity_cache_key = f"{entity_type}:{class_name}"
    if not force and entity_cache_key in cache:
        cached = cache[entity_cache_key]
        if isinstance(cached, dict) and cached.get("cache_key") == cache_key:
            print(f"  Skipping {class_name} (cached)")
            return

    print(f"Processing {class_name}...")

    # Build the prompt for the agent
    agent_prompt = f"""{prompt_content}

---

## Entity to Process

**Type:** {entity_type}
**Class:** {class_name}
**Version:** {version}
**Project root:** {PROJECT_ROOT}

## Source Code

The source file is at: {source_path}

## Localization Data

```json
{json.dumps(loc_entries, indent=2)}
```

## Act Assignment Info

{act_info}

## File Paths

- Per-entity data file: data/{version}/{entity_type}/{class_name}.json
- Override file (if needed): overrides/{entity_type}/{{slug}}.md
- Generated page will be at: site/src/content/{entity_type}/{{slug}}.md
- Rendered HTML will be at: site/dist/{entity_type}/{{slug}}/index.html

## Instructions

1. Read the source file to understand the event fully
2. Write the JSON data file using the Write tool
3. Run the page generator and site build using Bash:
   ```
   uv run python -m scripts.generate_{entity_type} \
     data/{version} site/src/content/{entity_type}
   cd site && npm run build
   ```
4. Read the rendered HTML to review the page
5. Fix any problems by updating the data file or creating an override, then rebuild
6. When satisfied, say "Done"
"""

    async for message in query(
        prompt=agent_prompt,
        options=ClaudeAgentOptions(
            cwd=str(PROJECT_ROOT),
            model=MODEL,
            allowed_tools=["Read", "Write", "Edit", "Bash", "Glob", "Grep"],
            permission_mode="bypassPermissions",
            max_turns=30,
        ),
    ):
        if isinstance(message, ResultMessage):
            print(f"  Agent result: {message.result[:200] if message.result else '(empty)'}")

    # Update cache
    cache[entity_cache_key] = {
        "cache_key": cache_key,
        "last_processed": str(Path(source_path).stat().st_mtime),
    }
    save_cache(cache)


async def async_main() -> None:
    parser = argparse.ArgumentParser(description="LLM-based game data extraction")
    parser.add_argument("--type", required=True, choices=list(ENTITY_TYPE_DIRS.keys()))
    parser.add_argument("--version", default="v0.101.0")
    parser.add_argument("--entity", help="Process a single entity by class name")
    parser.add_argument("--force", action="store_true", help="Ignore cache")
    args = parser.parse_args()

    entity_type = args.type
    version = args.version
    decompiled_dir = str(PROJECT_ROOT / "decompiled" / version)
    loc_dir = str(PROJECT_ROOT / "extracted" / version / "localization" / "eng")

    # Load prompt
    prompt_path = PROMPTS_DIR / f"extract_{entity_type.rstrip('s')}.md"
    if not prompt_path.exists():
        print(f"Error: prompt not found at {prompt_path}")
        sys.exit(1)
    prompt_content = prompt_path.read_text()

    # Load localization
    loc_type = ENTITY_TYPE_LOC[entity_type]
    loc_data = load_localization(loc_dir, loc_type)

    # Load shared events and act assignments
    shared_events = get_shared_events(decompiled_dir)
    act_events = get_act_events(decompiled_dir)
    act_names = get_act_names(decompiled_dir)

    # Find source files
    source_dir = os.path.join(decompiled_dir, ENTITY_TYPE_DIRS[entity_type])
    if not os.path.isdir(source_dir):
        print(f"Error: source directory not found: {source_dir}")
        sys.exit(1)

    cache = load_cache()

    for fname in sorted(os.listdir(source_dir)):
        if not fname.endswith(".cs"):
            continue
        class_name = fname.removesuffix(".cs")

        if args.entity and class_name != args.entity:
            continue

        source_path = os.path.join(source_dir, fname)
        with open(source_path) as f:
            content = f.read()

        if entity_type == "events" and not is_event_class(content):
            continue

        loc_entries = get_loc_entries_for_entity(loc_data, class_name)

        # Determine act info
        if class_name in shared_events:
            all_acts = ", ".join(act_names)
            act_info = (
                "This is a SHARED event (ModelDb.AllSharedEvents), available "
                f"in all acts: {all_acts}. However, check IsAllowed() for "
                "act restrictions that may limit which acts it actually appears in."
            )
        elif class_name in act_events:
            acts = act_events[class_name]
            act_info = f"This event is specific to: {', '.join(acts)}"
        else:
            act_info = "No act assignment found."

        await process_entity(
            class_name=class_name,
            source_path=source_path,
            entity_type=entity_type,
            version=version,
            loc_entries=loc_entries,
            prompt_content=prompt_content,
            act_info=act_info,
            cache=cache,
            force=args.force,
        )

    print("\nDone.")


def main() -> None:
    anyio.run(async_main)


if __name__ == "__main__":
    main()
