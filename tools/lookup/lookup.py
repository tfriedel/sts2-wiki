#!/usr/bin/env python3
"""sts2-lookup: Quick reference for STS2 enemies and cards.

Usage:
    lookup enemy <name>       Look up enemy (moves, pattern, HP)
    lookup card <name>        Look up card (cost, effects, target)
    lookup encounter <name>   Look up encounter setup
    lookup -h, --help         Show help

Options:
    --pattern   Show only move pattern (enemies)
    --json      Output raw JSON
    -f, --fuzzy Fuzzy match (Levenshtein)
"""

import json
import os
import re
import sys
from pathlib import Path
from typing import Optional

DATA_ROOT = Path(__file__).resolve().parent.parent.parent / "data"


def _version_key(name: str) -> tuple[int, ...]:
    """Sort key for "vMAJOR.MINOR.PATCH" directory names."""
    return tuple(int(p) for p in re.findall(r"\d+", name))


def _latest_version_dir() -> Path:
    """Pick the highest semver-style directory under data/.

    Override with STS2_VERSION (e.g. STS2_VERSION=v0.101.0) when you need
    to query an older snapshot.
    """
    override = os.environ.get("STS2_VERSION")
    if override:
        return DATA_ROOT / override
    versions = [p for p in DATA_ROOT.iterdir() if p.is_dir() and p.name.startswith("v")]
    if not versions:
        raise SystemExit(f"No version directories found under {DATA_ROOT}")
    return max(versions, key=lambda p: _version_key(p.name))


DATA_DIR = _latest_version_dir()

# ── fuzzy matching ──────────────────────────────────────────────

def levenshtein(a: str, b: str) -> int:
    if len(a) < len(b):
        return levenshtein(b, a)
    if len(b) == 0:
        return len(a)
    prev = list(range(len(b) + 1))
    for i, ca in enumerate(a, 1):
        curr = [i]
        for j, cb in enumerate(b, 1):
            subst = prev[j] + (0 if ca == cb else 1)
            ins = curr[j - 1] + 1
            del_ = prev[j] + 1
            curr.append(min(subst, ins, del_))
        prev = curr
    return prev[-1]


def fuzzy_match(query: str, candidates: list[str]) -> Optional[str]:
    """Find best match by Levenshtein distance. Threshold scales with query length."""
    q_lower = query.lower()
    threshold = max(2, len(q_lower) // 2)
    best = None
    best_dist = threshold
    for c in candidates:
        d = levenshtein(q_lower, c.lower())
        if d <= best_dist:
            best_dist = d
            best = c
            if d == 0:
                break
    return best


# ── data loading ────────────────────────────────────────────────

def list_json_files(subdir: str) -> list[str]:
    d = DATA_DIR / subdir
    if not d.is_dir():
        return []
    return sorted(f.stem for f in d.glob("*.json"))


def load_json(subdir: str, name: str) -> dict:
    d = DATA_DIR / subdir / f"{name}.json"
    if not d.is_file():
        return {}
    return json.loads(d.read_text())


# ── output formatters ───────────────────────────────────────────

def format_enemy(data: dict, pattern_only: bool = False) -> str:
    if not data:
        return "Not found."

    title = data.get("title", data.get("class_name", "?"))
    hp = data.get("min_hp", "?")
    max_hp = data.get("max_hp", "")
    if str(hp) == str(max_hp):
        hp_str = f"HP {hp}"
    else:
        hp_str = f"HP {hp}-{max_hp}"

    spawn = data.get("powers_on_spawn", [])
    powers_str = f"  Spawns with: {', '.join(spawn)}" if spawn else ""

    moves = data.get("moves", [])
    move_lines = []
    for m in moves:
        title_m = m.get("title", m.get("id", "?"))
        intents = m.get("intents", [])
        effects = m.get("effects", [])
        parts = []
        for intent in intents:
            t = intent.get("type", "")
            if t == "attack":
                dmg = intent.get("damage", "?")
                hits = intent.get("hits", 1)
                if hits > 1:
                    parts.append(f"{dmg}dmg x{hits}")
                else:
                    parts.append(f"{dmg}dmg")
            elif t == "multi_attack":
                dmg = intent.get("damage", "?")
                hits = intent.get("hits", 1)
                parts.append(f"{dmg}dmg x{hits}")
            elif t == "buff":
                parts.append("buff")
            elif t == "debuff":
                parts.append("debuff")
            elif t == "block":
                amt = intent.get("amount", intent.get("value", "?"))
                parts.append(f"{amt}blk")
            elif t == "summon":
                parts.append("summon")
        intent_str = " + ".join(parts)
        effect_str = "; ".join(effects)
        if intent_str:
            move_lines.append(f"  {title_m}: {intent_str} → {effect_str}")
        else:
            move_lines.append(f"  {title_m}: {effect_str}")

    if pattern_only:
        return f"{data.get('move_pattern', 'Unknown pattern')}"

    pattern = data.get("move_pattern", "Unknown pattern")
    notes = data.get("notes", "")

    lines = [f"\n{title}  {hp_str}"]
    if spawn:
        lines.append(powers_str)
    lines.append(f"\nMoves:")
    lines.extend(move_lines)
    lines.append(f"\nPattern: {pattern}")
    if notes and notes != pattern:
        lines.append(f"\nNotes: {notes}")
    return "\n".join(lines)


def format_card(data: dict) -> str:
    if not data:
        return "Not found."

    title = data.get("title", data.get("class_name", "?"))
    cost = data.get("energy_cost", "?")
    rarity = data.get("rarity", "")
    card_type = data.get("type", "")
    target = data.get("target", "")
    desc = data.get("description", "")
    upg = data.get("upgraded_description", "")
    character = data.get("character", "")

    header = f"\n{title}"
    if character:
        header += f"  [{character}]"
    header += f"  Cost {cost}"
    if rarity:
        header += f"  {rarity}"
    if target:
        header += f"  →{target}"

    lines = [header]
    lines.append(f"  {desc}")
    if upg:
        lines.append(f"  (upgraded: {upg})")

    notes = data.get("notes", "")
    if notes:
        lines.append(f"\n  Notes: {notes}")

    # Show vars
    vars_ = data.get("vars", [])
    if vars_ and "vars" not in desc.lower():
        for v in vars_:
            vtype = v.get("type", "")
            base = v.get("base_value", "")
            upg_v = v.get("upgraded_value", "")
            if vtype == "Damage":
                lines.append(f"  Damage: {base} (upgraded: {upg_v})")

    return "\n".join(lines)


def format_encounter(data: dict) -> str:
    if not data:
        return "Not found."
    title = data.get("title", data.get("class_name", "?"))
    room = data.get("room_type", "")
    monsters = data.get("monsters", [])
    total = data.get("total_monsters", "")
    notes = data.get("notes", "")
    tags = data.get("tags", [])

    lines = [f"\n{title}  [{room}]" f"  ({total} monster{'s' if total != 1 else ''})"]
    if monsters:
        lines.append(f"  Monsters: {', '.join(monsters)}")
    if tags:
        lines.append(f"  Tags: {', '.join(tags)}")
    if notes:
        lines.append(f"\n  Notes: {notes}")
    return "\n".join(lines)


# ── main ────────────────────────────────────────────────────────

def find_file(name: str, subdir: str, fuzzy: bool) -> Optional[str]:
    candidates = list_json_files(subdir)
    if not candidates:
        return None
    # exact match first
    if name in candidates:
        return name
    # case-insensitive
    name_lower = name.lower()
    for c in candidates:
        if c.lower() == name_lower:
            return c
    # contains match ("ink" -> Inklet, Inklets)
    if len(name) >= 3:
        for c in candidates:
            if name_lower in c.lower():
                return c
    if fuzzy:
        matched = fuzzy_match(name, candidates)
        if matched:
            return matched
    return None


def main():
    import argparse

    parser = argparse.ArgumentParser(
        description="Quick lookup for STS2 enemies and cards",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="Examples:\n  lookup enemy Nibbit\n  lookup card Bash\n  lookup enemy Fogmog --pattern\n  lookup card Conflagration -f\n"
    )
    parser.add_argument("category", choices=["enemy", "card", "encounter", "list"],
                        help="Category to look up, or 'list' to show all")
    parser.add_argument("name", nargs="?", default=None, help="Name of the entity (required unless category=list)")
    parser.add_argument("--pattern", action="store_true",
                       help="Show only move pattern (enemies)")
    parser.add_argument("--json", action="store_true",
                       help="Output raw JSON")
    parser.add_argument("-f", "--fuzzy", action="store_true",
                       help="Fuzzy match")
    args = parser.parse_args()

    subdir_map = {
        "enemy": "monsters",
        "card": "cards",
        "encounter": "encounters",
        "list": "monsters",
    }
    subdir = subdir_map[args.category]

    if args.category == "list":
        # Determine subdir from name if given (e.g., list card, list enemy)
        if args.name and args.name in subdir_map:
            subdir = subdir_map[args.name]
        candidates = list_json_files(subdir)
        if args.name and args.name not in subdir_map:
            matches = [c for c in candidates if args.name.lower() in c.lower()]
            if matches:
                print("\n".join(matches))
            else:
                print(f"No matches for '{args.name}'")
        else:
            print(f"{len(candidates)} items")
            print("\n".join(candidates))
        return

    if not args.name:
        print("Error: name is required (or use 'list' category)", file=sys.stderr)
        sys.exit(1)

    name = find_file(args.name, subdir, args.fuzzy)
    if not name:
        print(f"Not found: '{args.name}'", file=sys.stderr)
        # Show closest matches
        candidates = list_json_files(subdir)
        matches = sorted(
            [(c, levenshtein(args.name.lower(), c.lower())) for c in candidates],
            key=lambda x: x[1]
        )[:5]
        if matches:
            print("Closest matches:", ", ".join(f"{m[0]}({m[1]})" for m in matches), file=sys.stderr)
        sys.exit(1)

    if args.json:
        print(json.dumps(load_json(subdir, name), indent=2))
        return

    data = load_json(subdir, name)
    if args.category == "enemy":
        print(format_enemy(data, args.pattern))
    elif args.category == "card":
        print(format_card(data))
    elif args.category == "encounter":
        print(format_encounter(data))


if __name__ == "__main__":
    main()
