# LLM Extraction Pipeline — TODO List

## Status Key
- [ ] Not started
- [x] Complete
- [~] In progress

## Events
- [x] Build orchestrator (scripts/llm_extract.py)
- [x] Write event extraction prompt
- [x] Process all 58 events for v0.101.0
- [ ] Process events for v0.100.0, v0.99.1, v0.98.2
- [ ] Remove old extract_events.py regex script (once all versions migrated)
- [ ] Remove _EVENT_ENRICHMENTS dict from generate_events.py (once LLM data covers all)

## Monsters
- [ ] Write monster extraction prompt (scripts/prompts/extract_monster.md)
- [ ] Add "monsters" to ENTITY_TYPE_DIRS in llm_extract.py
- [ ] Adapt generate_monsters.py to read per-entity JSON + overrides
- [ ] Process all monsters for v0.101.0
- [ ] Review and fix issues
- [ ] Process monsters for older versions

## Enchantments
- [ ] Write enchantment extraction prompt
- [ ] Add "enchantments" to ENTITY_TYPE_DIRS
- [ ] Adapt generate_enchantments.py for per-entity JSON + overrides
- [ ] Process all enchantments for v0.101.0
- [ ] Review and fix issues

## Cards
- [ ] Write card extraction prompt
- [ ] Add "cards" to ENTITY_TYPE_DIRS
- [ ] Adapt generate_cards.py for per-entity JSON + overrides
- [ ] Process all 575 cards for v0.101.0 (batch processing — may need parallelism)
- [ ] Review and fix issues

## Relics
- [ ] Write relic extraction prompt
- [ ] Add "relics" to ENTITY_TYPE_DIRS
- [ ] Adapt generate_relics.py for per-entity JSON + overrides
- [ ] Process all 290 relics for v0.101.0
- [ ] Review and fix issues

## Powers
- [ ] Write power extraction prompt
- [ ] Add "powers" to ENTITY_TYPE_DIRS
- [ ] Adapt generate_powers.py for per-entity JSON + overrides
- [ ] Process all 260 powers for v0.101.0
- [ ] Review and fix issues

## Potions
- [ ] Write potion extraction prompt
- [ ] Add "potions" to ENTITY_TYPE_DIRS
- [ ] Adapt generate_potions.py for per-entity JSON + overrides
- [ ] Process all 64 potions for v0.101.0
- [ ] Review and fix issues

## Encounters
- [ ] Write encounter extraction prompt
- [ ] Add "encounters" to ENTITY_TYPE_DIRS
- [ ] Adapt generate_encounters.py for per-entity JSON + overrides
- [ ] Process all encounters for v0.101.0
- [ ] Review and fix issues

## Infrastructure
- [ ] Add justfile targets for LLM extraction
- [ ] Update the sts2-new-patch skill to include LLM extraction steps
- [ ] Clean up: remove old regex extraction scripts once all types migrated
- [ ] Validate all per-entity data against content.config.ts schemas
