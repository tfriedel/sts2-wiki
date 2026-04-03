# Encounter Data Extraction - ByrdonisElite

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Extract ByrdonisElite encounter data from decompiled C# source, write JSON schema-compliant data file, generate wiki page, and verify rendering.

**Architecture:** Read C# source → extract room type, weak status, monster list, tags → write JSON → run page generator → build Astro site → review rendered HTML → fix any issues.

**Tech Stack:** Python (extraction), JSON (data format), Astro (site generation), ilspycmd (decompiled source)

---

### Task 1: Read and Analyze ByrdonisElite Source

**Files:**
- Read: `/Users/drmaciver/Projects/sts2-wiki/decompiled/v0.101.0/MegaCrit.Sts2.Core.Models.Encounters/ByrdonisElite.cs`

- [ ] **Step 1: Read the ByrdonisElite.cs source file**

Read `/Users/drmaciver/Projects/sts2-wiki/decompiled/v0.101.0/MegaCrit.Sts2.Core.Models.Encounters/ByrdonisElite.cs` to extract:
- `RoomType` property (should be `RoomType.Elite`)
- `IsWeak` property boolean value
- `GenerateMonsters()` method - identify all `ModelDb.Monster<ClassName>()` calls
- `Tags` property - list all `EncounterTag.*` values
- Any special mechanics or conditional spawning logic

Expected: Understand complete encounter structure

- [ ] **Step 2: Document extracted values**

Note the following for JSON creation:
- **class_name**: "ByrdonisElite"
- **title**: "Byrdonis" (from localization BYRDONIS_ELITE.title)
- **room_type**: (Extract from RoomType property - should be "Elite")
- **is_weak**: (Boolean from IsWeak property)
- **monsters**: (Array of class names from GenerateMonsters())
- **total_monsters**: (Count of spawned monsters, may differ from unique types)
- **tags**: (Array of tag names)
- **notes**: (Any special mechanics if applicable)

---

### Task 2: Write JSON Data File

**Files:**
- Create: `/Users/drmaciver/Projects/sts2-wiki/data/v0.101.0/encounters/ByrdonisElite.json`

- [ ] **Step 1: Create ByrdonisElite.json with schema-compliant data**

Write the following JSON structure with values extracted from Task 1:

```json
{
  "class_name": "ByrdonisElite",
  "title": "Byrdonis",
  "room_type": "Elite",
  "is_weak": false,
  "monsters": [],
  "total_monsters": 0,
  "tags": [],
  "notes": ""
}
```

Replace empty values with actual extracted data from the C# source analysis.

- [ ] **Step 2: Verify JSON is valid**

Run: `python -c "import json; json.load(open('/Users/drmaciver/Projects/sts2-wiki/data/v0.101.0/encounters/ByrdonisElite.json'))" && echo "Valid JSON"`

Expected: Output "Valid JSON" with no errors

---

### Task 3: Generate Page and Build Site

**Files:**
- Input: `/Users/drmaciver/Projects/sts2-wiki/data/v0.101.0/encounters/ByrdonisElite.json`
- Generated: `/Users/drmaciver/Projects/sts2-wiki/site/src/content/encounters/byrdonis.md`
- Output: `/Users/drmaciver/Projects/sts2-wiki/site/dist/encounters/byrdonis/index.html`

- [ ] **Step 1: Run the encounter page generator**

From `/Users/drmaciver/Projects/sts2-wiki`:

```bash
uv run python -m scripts.generate_encounters data/v0.101.0 site/src/content/encounters
```

Expected: Script completes without errors, generates Markdown file in site/src/content/encounters/byrdonis.md

- [ ] **Step 2: Build the Astro site**

From `/Users/drmaciver/Projects/sts2-wiki`:

```bash
cd site && npm run build
```

Expected: Build succeeds, HTML output created at site/dist/encounters/byrdonis/index.html

---

### Task 4: Review Rendered Page and Fix Issues

**Files:**
- Review: `/Users/drmaciver/Projects/sts2-wiki/site/dist/encounters/byrdonis/index.html`
- Fix: `/Users/drmaciver/Projects/sts2-wiki/data/v0.101.0/encounters/ByrdonisElite.json` (data issues) or `/Users/drmaciver/Projects/sts2-wiki/overrides/encounters/byrdonis.md` (template issues)

- [ ] **Step 1: Read the rendered HTML page**

Read `/Users/drmaciver/Projects/sts2-wiki/site/dist/encounters/byrdonis/index.html` and verify:
- Title displays as "Byrdonis"
- Room type shows as "Elite"
- All monsters are listed correctly
- All encounter tags are displayed
- No blank/empty fields
- No formatting errors

Expected: Page renders correctly with all data visible

- [ ] **Step 2: Check for issues**

If any problems found (missing data, formatting issues, broken links):
- **Data problems** (wrong values, missing monsters, incorrect tags): Update `/Users/drmaciver/Projects/sts2-wiki/data/v0.101.0/encounters/ByrdonisElite.json`, regenerate, and rebuild
- **Template/display problems** (formatting, styling): Create override at `/Users/drmaciver/Projects/sts2-wiki/overrides/encounters/byrdonis.md`

Re-run generator and build to verify fixes.

- [ ] **Step 3: Confirm completion**

When page renders correctly with all data visible and properly formatted, task is complete.

---

## Plan Verification

✅ **Spec Coverage:**
- Reading source file → Task 1
- Extracting JSON schema fields → Task 1, Step 2
- Writing data file → Task 2
- Running page generator → Task 3, Step 1
- Building site → Task 3, Step 2
- Reading rendered HTML → Task 4, Step 1
- Fixing issues → Task 4, Step 2

✅ **No Placeholders:** All code blocks, commands, and expected outputs are complete

✅ **Type Consistency:** JSON schema matches provided spec, file paths are exact, commands are complete
