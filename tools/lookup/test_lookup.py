"""Tests for tools.lookup.lookup.

Run from repo root:
    uv run python -m unittest tools.lookup.test_lookup
"""

import json
import unittest
from pathlib import Path
from tempfile import TemporaryDirectory

from tools.lookup import lookup


def _write(path: Path, payload: dict) -> None:
    path.write_text(json.dumps(payload))


class FindFilesTests(unittest.TestCase):
    """Cover the resolution paths in lookup.find_files using a synthetic data dir."""

    def setUp(self) -> None:
        self._tmp = TemporaryDirectory()
        root = Path(self._tmp.name)
        (root / "monsters").mkdir()
        (root / "cards").mkdir()
        (root / "encounters").mkdir()

        # Size-variant slime
        _write(
            root / "monsters" / "LeafSlimeS.json",
            {
                "class_name": "LeafSlimeS",
                "title": "Leaf Slime (S)",
                "min_hp": 12,
                "max_hp": 16,
                "moves": [],
            },
        )
        _write(
            root / "monsters" / "LeafSlimeM.json",
            {
                "class_name": "LeafSlimeM",
                "title": "Leaf Slime (M)",
                "min_hp": 33,
                "max_hp": 36,
                "moves": [],
            },
        )
        # Multi-variant family
        for variant in ("Egg", "Nectar", "Rock", "Silk"):
            _write(
                root / "monsters" / f"Bowlbug{variant}.json",
                {
                    "class_name": f"Bowlbug{variant}",
                    "title": f"Bowlbug ({variant})",
                    "min_hp": 20,
                    "max_hp": 25,
                    "moves": [],
                },
            )
        # Title differs from class_name
        _write(
            root / "monsters" / "Architect.json",
            {
                "class_name": "Architect",
                "title": "The Architect",
                "min_hp": 9999,
                "max_hp": 9999,
                "moves": [],
            },
        )
        # Distinct stem to test substring behaviour
        _write(
            root / "monsters" / "Inklet.json",
            {
                "class_name": "Inklet",
                "title": "Inklet",
                "min_hp": 5,
                "max_hp": 8,
                "moves": [],
            },
        )
        # Card and encounter for cross-category coverage
        _write(
            root / "cards" / "Strike.json",
            {
                "class_name": "Strike",
                "title": "Strike",
                "energy_cost": 1,
            },
        )
        _write(
            root / "encounters" / "AxebotsNormal.json",
            {
                "class_name": "AxebotsNormal",
                "title": "Bot Buddies",
                "monsters": ["Axebot", "Axebot"],
            },
        )

        self._orig_data_dir = lookup.DATA_DIR
        lookup.DATA_DIR = root
        lookup._TITLE_INDEX_CACHE.clear()

    def tearDown(self) -> None:
        lookup.DATA_DIR = self._orig_data_dir
        lookup._TITLE_INDEX_CACHE.clear()
        self._tmp.cleanup()

    # ── class_name (file stem) resolution ──────────────────────────

    def test_exact_class_name(self) -> None:
        self.assertEqual(lookup.find_files("LeafSlimeS", "monsters", False), ["LeafSlimeS"])

    def test_case_insensitive_class_name(self) -> None:
        self.assertEqual(lookup.find_files("leafslimes", "monsters", False), ["LeafSlimeS"])

    # ── human title resolution (issue #2 main ask) ────────────────

    def test_exact_title_with_size_suffix(self) -> None:
        self.assertEqual(lookup.find_files("Leaf Slime (S)", "monsters", False), ["LeafSlimeS"])

    def test_title_case_insensitive(self) -> None:
        self.assertEqual(lookup.find_files("the architect", "monsters", False), ["Architect"])

    def test_base_name_returns_both_size_variants(self) -> None:
        result = lookup.find_files("Leaf Slime", "monsters", False)
        self.assertEqual(sorted(result), ["LeafSlimeM", "LeafSlimeS"])

    def test_base_name_returns_all_named_variants(self) -> None:
        result = lookup.find_files("Bowlbug", "monsters", False)
        self.assertEqual(
            sorted(result),
            ["BowlbugEgg", "BowlbugNectar", "BowlbugRock", "BowlbugSilk"],
        )

    def test_base_name_case_insensitive(self) -> None:
        result = lookup.find_files("leaf slime", "monsters", False)
        self.assertEqual(sorted(result), ["LeafSlimeM", "LeafSlimeS"])

    # ── substring fallbacks ───────────────────────────────────────

    def test_substring_class_name(self) -> None:
        # "ink" → Inklet via stem substring
        self.assertEqual(lookup.find_files("ink", "monsters", False), ["Inklet"])

    def test_substring_in_title_only(self) -> None:
        # "architec" with no fuzzy: stem substring picks Architect first
        self.assertIn("Architect", lookup.find_files("architec", "monsters", False))

    # ── fuzzy ─────────────────────────────────────────────────────

    def test_fuzzy_typo_class_name(self) -> None:
        self.assertEqual(lookup.find_files("Architct", "monsters", True), ["Architect"])

    def test_fuzzy_typo_human_title(self) -> None:
        # "Leaf Slim" is closer to "leaf slime" (base) than to any class_name
        result = lookup.find_files("Leaf Slim", "monsters", True)
        self.assertEqual(sorted(result), ["LeafSlimeM", "LeafSlimeS"])

    # ── misses ────────────────────────────────────────────────────

    def test_not_found_without_fuzzy(self) -> None:
        self.assertEqual(lookup.find_files("Definitely Not A Monster", "monsters", False), [])

    def test_unknown_subdir(self) -> None:
        self.assertEqual(lookup.find_files("anything", "doesnotexist", True), [])

    # ── cross-category ────────────────────────────────────────────

    def test_card_exact(self) -> None:
        self.assertEqual(lookup.find_files("Strike", "cards", False), ["Strike"])

    def test_encounter_resolves_by_title(self) -> None:
        self.assertEqual(lookup.find_files("Bot Buddies", "encounters", False), ["AxebotsNormal"])


class TitleIndexTests(unittest.TestCase):
    """Direct checks on build_title_index — caching and base-name stripping."""

    def setUp(self) -> None:
        self._tmp = TemporaryDirectory()
        root = Path(self._tmp.name)
        (root / "monsters").mkdir()
        _write(root / "monsters" / "Foo.json", {"class_name": "Foo", "title": "Foo (Variant)"})
        _write(root / "monsters" / "Bar.json", {"class_name": "Bar", "title": "Bar"})
        _write(root / "monsters" / "Bad.json", {"class_name": "Bad"})  # no title
        self._orig_data_dir = lookup.DATA_DIR
        lookup.DATA_DIR = root
        lookup._TITLE_INDEX_CACHE.clear()

    def tearDown(self) -> None:
        lookup.DATA_DIR = self._orig_data_dir
        lookup._TITLE_INDEX_CACHE.clear()
        self._tmp.cleanup()

    def test_title_index_full_and_base(self) -> None:
        title_index, base_index = lookup.build_title_index("monsters")
        self.assertEqual(title_index["foo (variant)"], ["Foo"])
        self.assertEqual(title_index["bar"], ["Bar"])
        # Base name (variant suffix stripped) only registered when distinct.
        self.assertEqual(base_index["foo"], ["Foo"])
        self.assertNotIn("bar", base_index)

    def test_title_index_skips_missing_titles(self) -> None:
        title_index, _ = lookup.build_title_index("monsters")
        self.assertNotIn("bad", title_index)
        self.assertNotIn("", title_index)

    def test_title_index_is_cached(self) -> None:
        first = lookup.build_title_index("monsters")
        second = lookup.build_title_index("monsters")
        self.assertIs(first, second)


if __name__ == "__main__":
    unittest.main()
