#!/usr/bin/env python3
"""
Stitch folders of still frames into Desktop Gremlin sprite sheets.

Point it at a folder full of animation folders (flat or grouped) and it will:

  * map each folder name onto the name the engine actually wants
    (RunUpLeft -> upLeft, RunDown -> runDown, ...)
  * file each sheet into the right group folder (Run / Walk / Emotes / Actions)
  * derive ONE shared square crop box across every frame of every animation,
    because the engine crops all sheets with a single WIDTH/HEIGHT/COLUMN grid
  * write config.txt with the frame counts filled in

    python build_sheets.py raw/ out/ --map emote5=click --copy idle=runIdle

Output drops straight into SpriteSheet/Gremlins/<Name>/.
"""

import argparse
import math
import os
import re
import shutil
import sys

try:
    from PIL import Image
except ImportError:
    sys.exit("Pillow is required:  pip install Pillow")

FRAME_EXTS = (".png", ".tga", ".webp", ".bmp")

# canonical engine name -> group folder.  Source of truth: SpriteManager._fileNameMap
# for the filenames, the PlayAnimation call sites for the folders.
CANON = {
    "runUp": "Run", "runDown": "Run", "runLeft": "Run", "runRight": "Run",
    "upLeft": "Run", "upRight": "Run", "downLeft": "Run", "downRight": "Run",
    "walkUp": "Walk", "walkDown": "Walk", "walkLeft": "Walk", "walkRight": "Walk",
    "emote1": "Emotes", "emote2": "Emotes", "emote3": "Emotes", "emote4": "Emotes",
    "idle": "Actions", "runIdle": "Actions", "hover": "Actions", "grab": "Actions",
    "sleep": "Actions", "intro": "Actions", "outro": "Actions", "click": "Actions",
}

# Engine drops the "run" prefix on diagonals; UmaViewer exports tend to keep it.
ALIASES = {
    "runupleft": "upLeft", "runupright": "upRight",
    "rundownleft": "downLeft", "rundownright": "downRight",
    "walkupleft": "upLeft", "walkupright": "upRight",
    "walkdownleft": "downLeft", "walkdownright": "downRight",
    "wait": "idle", "stand": "idle", "idle1": "idle",
}

CONFIG_LAYOUT = [
    ("Run", ["RUNUP", "RUNDOWN", "RUNLEFT", "RUNRIGHT"]),
    ("Run Diagonal", ["UPLEFT", "UPRIGHT", "DOWNLEFT", "DOWNRIGHT"]),
    ("Emotes", ["EMOTE1", "EMOTE2", "EMOTE3", "EMOTE4"]),
    ("Walk", ["WALKDOWN", "WALKLEFT", "WALKRIGHT", "WALKUP"]),
    ("Actions", ["GRAB", "HOVER", "IDLE", "INTRO", "CLICK",
                 "OUTRO", "PAT", "RUNIDLE", "SLEEP"]),
]


def natural_key(path):
    """Sort frame_2.png before frame_10.png."""
    name = os.path.basename(path)
    return [int(p) if p.isdigit() else p.lower() for p in re.split(r"(\d+)", name)]


def resolve(raw_name, overrides):
    """Map a folder name onto a canonical engine animation name."""
    if raw_name in overrides:
        return overrides[raw_name]
    key = re.sub(r"[^a-z0-9]", "", raw_name.lower())
    if key in overrides:
        return overrides[key]
    if key in ALIASES:
        return ALIASES[key]
    for canon in CANON:
        if re.sub(r"[^a-z0-9]", "", canon.lower()) == key:
            return canon
    return None


def discover(root, overrides):
    """Walk the input tree, returning (canonical, raw_name, frames) plus misses."""
    found, unknown = [], []
    for dirpath, dirnames, filenames in os.walk(root):
        frames = sorted(
            (os.path.join(dirpath, f) for f in filenames
             if f.lower().endswith(FRAME_EXTS)),
            key=natural_key,
        )
        if not frames:
            continue
        raw = os.path.basename(dirpath)
        canon = resolve(raw, overrides)
        if canon is None:
            unknown.append((raw, len(frames)))
        else:
            found.append((canon, raw, frames))
    found.sort(key=lambda x: (CANON[x[0]], x[0]))
    return found, unknown


def union_alpha_bbox(animations):
    """Union of the non-transparent bounds across every frame of every animation."""
    box, canvas = None, None
    for canon, _, frames in animations:
        for path in frames:
            with Image.open(path) as im:
                im = im.convert("RGBA")
                if canvas is None:
                    canvas = im.size
                elif im.size != canvas:
                    sys.exit(
                        f"\nFrame size mismatch in '{canon}': {im.size} != {canvas}.\n"
                        "Every animation must be rendered at one resolution with a "
                        "locked camera, or the shared crop box is meaningless."
                    )
                bb = im.getbbox()
            if bb is None:
                continue
            box = bb if box is None else (
                min(box[0], bb[0]), min(box[1], bb[1]),
                max(box[2], bb[2]), max(box[3], bb[3]),
            )
    if box is None:
        sys.exit("Every frame is fully transparent - check your alpha export.")
    return box, canvas


def squarify(box, canvas, pad):
    l, t, r, b = box
    l, t = max(0, l - pad), max(0, t - pad)
    r, b = min(canvas[0], r + pad), min(canvas[1], b + pad)
    side = min(max(r - l, b - t), canvas[0], canvas[1])
    cx, cy = (l + r) / 2, (t + b) / 2
    l = max(0, min(int(round(cx - side / 2)), canvas[0] - side))
    t = max(0, min(int(round(cy - side / 2)), canvas[1] - side))
    return (l, t, l + side, t + side)


def render(frames, box, size, cols, quantize, colors, out_path):
    rows = math.ceil(len(frames) / cols)
    sheet = Image.new("RGBA", (cols * size, rows * size), (0, 0, 0, 0))
    for i, path in enumerate(frames):
        with Image.open(path) as im:
            im = im.convert("RGBA")
            if box:
                im = im.crop(box)
            if im.size != (size, size):
                im = im.resize((size, size), Image.LANCZOS)
            sheet.paste(im, ((i % cols) * size, (i // cols) * size))
    os.makedirs(os.path.dirname(out_path), exist_ok=True)
    if quantize:
        sheet = sheet.quantize(colors=colors, method=Image.FASTOCTREE)
    sheet.save(out_path, optimize=True)
    return sheet.size, rows


def parse_pairs(items, flag):
    out = {}
    for item in items or []:
        if "=" not in item:
            sys.exit(f"{flag} expects src=dst, got '{item}'")
        src, dst = item.split("=", 1)
        out[src.strip()] = dst.strip()
    return out


def build(args):
    overrides = parse_pairs(args.map, "--map")
    copies = parse_pairs(args.copy, "--copy")
    reverses = parse_pairs(args.reverse, "--reverse")

    animations, unknown = discover(args.input, overrides)
    if not animations:
        sys.exit(f"No frame folders found under {args.input}")

    # --reverse intro=outro : synthesise a new animation from reversed frames
    by_name = {c: f for c, _, f in animations}
    for src, dst in reverses.items():
        if src not in by_name:
            sys.exit(f"--reverse {src}={dst}: no animation named '{src}'")
        if dst not in CANON:
            sys.exit(f"--reverse {src}={dst}: '{dst}' is not an engine animation")
        animations.append((dst, f"{src} (reversed)", list(reversed(by_name[src]))))

    total = sum(len(f) for _, _, f in animations)
    print(f"Found {len(animations)} animations, {total} frames\n")

    if unknown:
        print("  ! Unrecognized folders (skipped) - use --map NAME=engineName:")
        for raw, n in unknown:
            print(f"      {raw}  ({n} frames)")
        print()

    if args.crop:
        raw_box, canvas = union_alpha_bbox(animations)
        box = squarify(raw_box, canvas, args.pad)
        print(f"Source canvas   : {canvas[0]}x{canvas[1]}")
        print(f"Union alpha bbox: {raw_box}")
        print(f"Shared crop box : {box}  "
              f"({box[2]-box[0]}x{box[3]-box[1]}, scaled to {args.size}x{args.size})\n")
    else:
        box = None

    counts, written = {}, {}
    for canon, raw, frames in animations:
        group = CANON[canon]
        out_path = os.path.join(args.output, group, f"{canon}.png")
        (w, h), rows = render(frames, box, args.size, args.columns,
                              args.quantize, args.colors, out_path)
        counts[canon.upper()] = len(frames)
        written[canon] = out_path
        note = "" if raw == canon else f"   <- {raw}"
        kb = os.path.getsize(out_path) // 1024
        print(f"  {group}/{canon}.png".ljust(28) +
              f"{w}x{h}  {len(frames):>3} frames  {rows} rows  {kb:>5}KB{note}")

    for src, dst in copies.items():
        if src not in written:
            sys.exit(f"--copy {src}={dst}: '{src}' was not built")
        if dst not in CANON:
            sys.exit(f"--copy {src}={dst}: '{dst}' is not an engine animation")
        dst_path = os.path.join(args.output, CANON[dst], f"{dst}.png")
        os.makedirs(os.path.dirname(dst_path), exist_ok=True)
        shutil.copyfile(written[src], dst_path)
        counts[dst.upper()] = counts[src.upper()]
        print(f"  {CANON[dst]}/{dst}.png".ljust(28) +
              f"copied from {src}.png ({counts[dst.upper()]} frames)")

    write_config(args, counts)
    report_missing(counts)


def write_config(args, counts):
    lines = []
    for title, keys in CONFIG_LAYOUT:
        lines.append(f"//{title}")
        lines += [f"{k}={counts.get(k, 0)}" for k in keys]
        lines.append("")
    lines += ["//SpriteSheet", f"WIDTH={args.size}", f"HEIGHT={args.size}",
              f"COLUMN={args.columns}", ""]
    os.makedirs(args.output, exist_ok=True)
    path = os.path.join(args.output, "config.txt")
    with open(path, "w", newline="\r\n") as f:
        f.write("\n".join(lines))
    print(f"\nWrote {path}")


def report_missing(counts):
    missing = [k for _, keys in CONFIG_LAYOUT for k in keys
               if k not in counts and k != "PAT"]
    if missing:
        print(f"\n  Missing ({len(missing)}), left at 0: {', '.join(missing)}")
        if "OUTRO" in missing:
            print("  ! OUTRO missing: tray 'Stylish Close' will not shut the app down.")
        if "IDLE" in missing:
            print("  ! IDLE missing: this is the default resting animation.")
    else:
        print("\n  All 23 reachable animations present (PAT is dead in this build).")


def main():
    p = argparse.ArgumentParser(
        description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    p.add_argument("input", help="folder containing your animation folders")
    p.add_argument("output", help="destination SpriteSheet/Gremlins/<Name> folder")
    p.add_argument("--size", type=int, default=325, help="frame size px (default 325)")
    p.add_argument("--columns", type=int, default=10, help="frames per row (default 10)")
    p.add_argument("--pad", type=int, default=8, help="px padding around the crop")
    p.add_argument("--no-crop", dest="crop", action="store_false",
                   help="frames are already tight and aligned")
    p.add_argument("--map", action="append", metavar="RAW=NAME",
                   help="rename a folder, e.g. --map emote5=click")
    p.add_argument("--copy", action="append", metavar="SRC=DST",
                   help="duplicate a built sheet, e.g. --copy idle=runIdle")
    p.add_argument("--reverse", action="append", metavar="SRC=DST",
                   help="build a sheet from reversed frames, e.g. --reverse intro=outro")
    p.add_argument("--quantize", action="store_true",
                   help="8-bit palette PNGs, matching the shipped Cafe sheets")
    p.add_argument("--colors", type=int, default=128, help="palette size (default 128)")
    build(p.parse_args())


if __name__ == "__main__":
    main()
