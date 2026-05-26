#!/usr/bin/env python3
"""Extract room / shop / rest-site / event / Neow / treasure art from the STS2 PCK.

Companion to ``extract_images.py`` (which handles atlases/sprites). This pulls
*standalone* VRAM textures used as full-scene art for the workbench's game-like
decision screens. Each logical ``foo.png`` has a ``foo.png.import`` sidecar in
the PCK whose ``path[.fmt]="res://....ctex"`` points at the decodable texture.

Output mirrors the workbench's served-art convention under ``OUTPUT/``:
    events/<slug>.png         per-event illustration  (key: event slug)
    merchant/<char>_shop.png  shopkeeper              (key: character)
    rest_site/restsite_<char>.png rest-site scene     (key: character)
    rooms/neow.png            Neow room background
    rooms/treasure_act{1,2,3}.png treasure room backgrounds

Large source textures are downscaled so the longest edge is <= MAX_EDGE.
Idempotent: existing outputs are skipped unless --force.
"""

from __future__ import annotations

import argparse
import os
import re
import struct
import sys
from pathlib import Path

from PIL import Image

# Reuse the .ctex decoder (WebP/PNG/BC1/BC3/BC7) from the sibling extractor.
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from extract_images import decode_ctex  # noqa: E402

MAX_EDGE = 1600


def read_pck_index(pck_path: str) -> dict[str, tuple[int, int]]:
    """Return {logical_path: (absolute_offset, size)} for every file in the PCK."""
    index: dict[str, tuple[int, int]] = {}
    with open(pck_path, "rb") as f:
        assert f.read(4) == b"GDPC", "not a Godot PCK"
        fmt_version = struct.unpack("<I", f.read(4))[0]
        f.read(16)  # engine version
        file_base = struct.unpack("<Q", f.read(8))[0]
        directory_offset = struct.unpack("<Q", f.read(8))[0] if fmt_version >= 3 else 0
        f.read(64)  # reserved
        if fmt_version >= 3:
            f.seek(directory_offset)
        file_count = struct.unpack("<I", f.read(4))[0]
        for _ in range(file_count):
            path_len = struct.unpack("<I", f.read(4))[0]
            path = f.read(path_len).rstrip(b"\x00").decode("utf-8")
            offset = struct.unpack("<q", f.read(8))[0]
            size = struct.unpack("<q", f.read(8))[0]
            f.read(16 + 4)  # md5 + flags
            index[path] = (file_base + offset, size)
    return index


def make_reader(pck_path: str, index: dict[str, tuple[int, int]]):
    def read(path: str) -> bytes | None:
        if path not in index:
            return None
        offset, size = index[path]
        with open(pck_path, "rb") as f:
            f.seek(offset)
            return f.read(size)

    return read


def resolve_ctex(import_text: str) -> str | None:
    """Pick the best .ctex referenced by a .png.import (prefer BPTC > S3TC > plain)."""
    paths = re.findall(r'path[^=]*="res://([^"]+\.ctex)"', import_text)
    if not paths:
        return None
    paths.sort(key=lambda p: (0 if "bptc" in p else 1 if "s3tc" in p else 2))
    return paths[0]


def downscale(img: Image.Image, max_edge: int = MAX_EDGE) -> Image.Image:
    longest = max(img.width, img.height)
    if longest <= max_edge:
        return img
    scale = max_edge / longest
    return img.resize((round(img.width * scale), round(img.height * scale)), Image.LANCZOS)


def output_for(source: str) -> str | None:
    """Map a PCK source path (no .import suffix) to an output rel path, or None to skip.

    Only per-event illustrations (``images/events/<slug>.png``) are extracted as
    flat scenes. Room / shopkeeper / rest-site / Neow / treasure art is *Spine
    skeletal animation* (Godot-wrapped ``.tres`` skeletons + packed atlas pages),
    not flat textures, so decoding the ``.ctex`` yields an unusable atlas sheet
    rather than a composited scene. Those screens use themed dark backgrounds +
    per-asset art instead. See the design spec's "art gap" note.

    Args:
        source: e.g. ``images/events/abyssal_baths.png``.
    """
    if source.startswith("images/events/"):
        rest = source[len("images/events/") :]
        if "/" in rest:
            return None  # sub-asset (e.g. crystal_sphere/*), not the main illustration
        return f"events/{rest}"
    return None


def main() -> None:
    parser = argparse.ArgumentParser(description="Extract room/event/shop art from the STS2 PCK")
    parser.add_argument("pck_path")
    parser.add_argument("output_dir")
    parser.add_argument("--force", action="store_true", help="re-extract even if output exists")
    args = parser.parse_args()

    pck_path = os.path.expanduser(args.pck_path)
    out_root = Path(args.output_dir)
    index = read_pck_index(pck_path)
    read = make_reader(pck_path, index)

    extracted = 0
    skipped = 0
    failed = 0
    by_category: dict[str, int] = {}
    for path in index:
        if not path.endswith(".png.import"):
            continue
        source = path[: -len(".import")]
        rel_out = output_for(source)
        if rel_out is None:
            continue
        out_path = out_root / rel_out
        category = rel_out.split("/")[0]
        if out_path.exists() and not args.force:
            skipped += 1
            continue
        import_text = (read(path) or b"").decode("utf-8", "replace")
        ctex = resolve_ctex(import_text)
        if not ctex or ctex not in index:
            print(f"  no .ctex for {source}")
            failed += 1
            continue
        ctex_data = read(ctex)
        img = decode_ctex(ctex_data) if ctex_data else None
        if img is None:
            print(f"  decode failed: {source}")
            failed += 1
            continue
        img = downscale(img)
        out_path.parent.mkdir(parents=True, exist_ok=True)
        img.save(out_path, "PNG")
        extracted += 1
        by_category[category] = by_category.get(category, 0) + 1

    print(
        f"\nextracted {extracted} (skipped {skipped}, failed {failed}); "
        + ", ".join(f"{k}={v}" for k, v in sorted(by_category.items()))
    )


if __name__ == "__main__":
    main()
