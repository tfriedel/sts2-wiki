# FairyInABottle Potion Extraction Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Extract FairyInABottle potion data from decompiled Slay the Spire 2 C# source and generate correct wiki page content.

**Architecture:** Read the source C# class to understand potion properties (rarity, usage, target, effects), resolve any template variables in the description using the decompiled code, write validated JSON to the data directory, then run the generation and build pipeline to verify the page renders correctly.

**Tech Stack:** Decompiled C#, JSON schema, Python extraction scripts, Astro site generator

---

## File Structure

Files to create/modify:
- Create: `data/v0.101.0/potions/FairyInABottle.json` (potion metadata)
- Modify: May need `overrides/potions/fairy-in-a-bottle.md` if page generation needs adjustments
- Generated: `site/src/content/potions/fairy-in-a-bottle.md` (auto-generated from JSON)
- Built: `site/dist/potions/fairy-in-a-bottle/index.html` (rendered output for review)

---

## Tasks

### Task 1: Read and Analyze the Source C# File

**Files:**
- Read: `decompiled/v0.101.0/MegaCrit.Sts2.Core.Models.Potions/FairyInABottle.cs`

- [ ] **Step 1: Read the source file to understand the potion**

Run: `cat decompiled/v0.101.0/MegaCrit.Sts2.Core.Models.Potions/FairyInABottle.cs`

Extract and document:
- Class name: `FairyInABottle`
- Rarity property value and enum mapping
- Usage property (PotionUsage enum value)
- Target property (TargetType enum value)
- Localization key (usually matches class name or similar pattern)
- Any special mechanics or effects described in the code
- Dynamic variables that need resolution

- [ ] **Step 2: Check for any localization key dependencies**

The description is typically in format `{LOC_KEY}.description`. If localization data is empty (as shown), we may need to infer from code or add a note that localization is missing.

---

### Task 2: Extract and Resolve Potion Data

**Files:**
- Create: `data/v0.101.0/potions/FairyInABottle.json`

- [ ] **Step 1: Map source properties to JSON schema**

Using the source code analysis from Task 1, create the JSON data structure. Key mappings:
```json
{
  "class_name": "FairyInABottle",
  "title": "[From localization {LOC_KEY}.title or inferred from class name]",
  "rarity": "[From Rarity property: Common|Uncommon|Rare|Event|Token|None]",
  "usage": "[From Usage property: Drink|Throw|Apply|Unknown]",
  "target": "[From Target property: Self|Single|AllEnemy|etc]",
  "description": "[From localization, with {VarName} templates resolved]",
  "notes": "[Optional: complex mechanics explanation]"
}
```

- [ ] **Step 2: Resolve template variables in description**

If the description contains template variables like `{VarName}`:
- Look for DynamicVars in the source code that define these values
- Replace with actual values or descriptive text in square brackets
- Example: `{damage}` → `[12 damage]` or actual value if found in code
- NEVER leave raw `{VarName}` syntax in final output

- [ ] **Step 3: Write the JSON data file**

Write the complete JSON to: `data/v0.101.0/potions/FairyInABottle.json`

Expected format (example):
```json
{
  "class_name": "FairyInABottle",
  "title": "Fairy in a Bottle",
  "rarity": "Rare",
  "usage": "Drink",
  "target": "Self",
  "description": "Grants [2 temporary HP] and [1 draw] next turn.",
  "notes": "Optional: add if there are special interactions or complex mechanics"
}
```

- [ ] **Step 4: Validate JSON structure**

Run: `python3 -c "import json; json.load(open('data/v0.101.0/potions/FairyInABottle.json'))" && echo "Valid JSON"`

Expected: Valid JSON output with no errors

---

### Task 3: Generate Page Content and Build Site

**Files:**
- Generate: `site/src/content/potions/fairy-in-a-bottle.md`
- Build: `site/dist/potions/fairy-in-a-bottle/index.html`

- [ ] **Step 1: Run the potion page generator**

Run: `uv run python -m scripts.generate_potions data/v0.101.0 site/src/content/potions`

Expected: Script completes without errors, file `site/src/content/potions/fairy-in-a-bottle.md` is created

- [ ] **Step 2: Build the Astro site**

Run: `cd site && npm run build`

Expected: Build completes successfully, no errors. Output includes `site/dist/potions/fairy-in-a-bottle/index.html`

---

### Task 4: Review and Verify Rendered Output

**Files:**
- Read: `site/dist/potions/fairy-in-a-bottle/index.html`

- [ ] **Step 1: Read the rendered HTML page**

Run: `cat site/dist/potions/fairy-in-a-bottle/index.html | head -100`

Verify in the HTML:
- Rarity badge displays correctly (Common/Uncommon/Rare/Event/Token)
- Usage type is shown (Drink/Throw/Apply)
- Target description is present and correct
- Description text appears with NO raw `{VarName}` syntax
- Title matches the "Fairy in a Bottle" or similar expected name

- [ ] **Step 2: Identify any rendering issues**

Check for:
- Missing or malformed fields
- Unresolved template variables (raw `{...}` syntax in rendered output)
- Broken links or references
- Formatting issues in description text

---

### Task 5: Fix Issues if Needed (Conditional)

**Files:**
- Modify: `data/v0.101.0/potions/FairyInABottle.json` or
- Create: `overrides/potions/fairy-in-a-bottle.md` (if data file needs override)

- [ ] **Step 1: If issues found, update the JSON data file**

If description has unresolved variables or incorrect values:
1. Update `data/v0.101.0/potions/FairyInABottle.json`
2. Re-run generator: `uv run python -m scripts.generate_potions data/v0.101.0 site/src/content/potions`
3. Re-build site: `cd site && npm run build`
4. Re-verify rendered output

OR

- [ ] **Step 2: If structural changes needed, create override file**

If generated page needs override (e.g., custom formatting for complex mechanics):
1. Create `overrides/potions/fairy-in-a-bottle.md` with custom Markdown
2. Rebuild site: `cd site && npm run build`
3. Verify rendered output

- [ ] **Step 3: Repeat until page renders correctly**

Continue the read → fix → generate → build → verify cycle until output is correct.

---

### Task 6: Final Commit

**Files:**
- Commit: `data/v0.101.0/potions/FairyInABottle.json`
- (Do NOT commit: `decompiled/`, `extracted/`, `site/dist/`, `site/src/content/potions/fairy-in-a-bottle.md`)

- [ ] **Step 1: Stage the data file**

Run: `git add data/v0.101.0/potions/FairyInABottle.json`

- [ ] **Step 2: Create commit**

Run: `git commit -m "data: extract FairyInABottle potion from v0.101.0"`

Expected: Commit created successfully

- [ ] **Step 3: Verify nothing unintended was committed**

Run: `git show --stat`

Expected: Only `data/v0.101.0/potions/FairyInABottle.json` in the commit

---

## Self-Review Checklist

✓ Spec coverage: All requirements addressed (extract data, validate JSON, generate page, verify rendering, fix issues)
✓ File paths: Exact paths specified throughout
✓ Commands: Complete commands with expected output
✓ No placeholders: Every step has concrete actions and code
✓ Testing strategy: HTML verification serves as functional test
✓ Commit discipline: Clear, single-purpose commit at end
