# LLM Extraction Pipeline — TODO List

## Status Key
- [ ] Not started
- [x] Complete
- [~] In progress

## Events
- [x] Build orchestrator (scripts/llm_extract.py)
- [x] Write event extraction prompt
- [x] Process all EventModel (regular) events: v0.101.0 (58/58), v0.100.0 (59/59), v0.99.1 (59/59), v0.98.2 (59/59) — 235/235 total ✓
- [ ] Note: AncientEventModel (9 per version) handled separately via extract-ancients script
- [ ] Remove old extract_events.py regex script (once all versions migrated)
- [ ] Remove _EVENT_ENRICHMENTS dict from generate_events.py (once LLM data covers all)

## Monsters
- [x] Write monster extraction prompt (scripts/prompts/extract_monster.md)
- [x] Add "monsters" to ENTITY_TYPE_DIRS in llm_extract.py
- [x] Adapt generate_monsters.py to read per-entity JSON + overrides
- [x] Process all monsters for v0.101.0 (121/121 — 100%)
- [x] Process all monsters for v0.100.0 (120/120 — 100%)
- [x] Review and fix issues (1 issue: DeprecatedMonster 0 HP, expected)
- [x] Process monsters for v0.99.1 (121/121 — 100%) and v0.98.2 (120/120 — 100%) — 482/482 total (100%)

## Enchantments
- [x] Write enchantment extraction prompt
- [x] Add "enchantments" to ENTITY_TYPE_DIRS
- [x] Adapt generate_enchantments.py for per-entity JSON + overrides
- [x] Process all enchantments for v0.101.0
- [x] Review and fix issues (0 issues)

## Cards
- [x] Write card extraction prompt
- [x] Add "cards" to ENTITY_TYPE_DIRS
- [x] Adapt generate_cards.py for per-entity JSON + overrides
- [x] Process all 577 cards for v0.101.0
- [x] Review and fix issues (1 broken JSON manually fixed; 2 cards written manually after timeout; referenced_powers re-slugify via power_titles lookup)

## Relics
- [x] Write relic extraction prompt
- [x] Add "relics" to ENTITY_TYPE_DIRS
- [x] Adapt generate_relics.py for per-entity JSON + overrides
- [x] Process all 291 relics for v0.101.0
- [x] Review and fix issues (0 issues)

## Powers
- [x] Write power extraction prompt
- [x] Add "powers" to ENTITY_TYPE_DIRS
- [x] Adapt generate_powers.py for per-entity JSON + overrides
- [x] Process all 244 powers for v0.101.0
- [x] Review and fix issues (0 issues; {Amount} placeholders in smart_description are intentional runtime vars)

## Potions
- [x] Write potion extraction prompt
- [x] Add "potions" to ENTITY_TYPE_DIRS
- [x] Adapt generate_potions.py for per-entity JSON + overrides
- [x] Process all 64 potions for v0.101.0
- [x] Review and fix issues (0 issues)

## Encounters
- [x] Write encounter extraction prompt
- [x] Add "encounters" to ENTITY_TYPE_DIRS
- [x] Adapt generate_encounters.py for per-entity JSON + overrides
- [x] Process all encounters for v0.101.0
- [x] Review and fix issues (0 issues)

## Infrastructure
- [x] Optimize: skip per-entity site rebuild (--skip-build flag, ~15x faster)
- [x] Add justfile targets for LLM extraction
- [x] Update the sts2-new-patch skill to include LLM extraction steps
- [ ] Clean up: remove old regex extraction scripts once all types migrated
- [x] Validate all per-entity data against content.config.ts schemas (`just check-content` runs `astro sync`)
