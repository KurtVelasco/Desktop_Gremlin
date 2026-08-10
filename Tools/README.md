# Tools

## build_sheets.py

Stitches folders of still frames into sprite sheets in the layout the engine
expects, and writes the character's `config.txt` with frame counts filled in.

```
pip install Pillow
python Tools/build_sheets.py <frames-folder> <output-folder> [options]
```

### Why the shared crop matters

The engine crops every sheet for a character with a single grid definition —
`WIDTH`, `HEIGHT` and `COLUMN` from that character's `config.txt`
(see `SpriteManager.PlayAnimation`). There is no per-animation offset. So every
sheet must share one frame size and one pivot, or the character will jump
between animations.

The script enforces this: it scans every frame of every animation first, takes
the union of the non-transparent bounding boxes, squares it, and renders all
sheets against that one box. Frames must be rendered at a single resolution
with a locked camera — it will refuse to continue if resolutions differ.

### Naming

Input folder names are mapped onto the names the engine actually wants, so
capture exports can be used as-is:

| Export name                        | Engine name              |
|------------------------------------|--------------------------|
| `RunUpLeft`, `RunDownRight`, ...   | `upLeft`, `downRight`    |
| `RunDown`, `WalkUp`, ...           | `runDown`, `walkUp`      |
| `wait`, `stand`, `idle1`           | `idle`                   |

Casing and separators are ignored when matching. Anything unrecognized is
reported and skipped rather than silently dropped — remap it with `--map`.

Sheets are filed into `Run/`, `Walk/`, `Emotes/` and `Actions/` automatically.

### Options

| Option | Purpose |
|---|---|
| `--map RAW=NAME` | Rename a folder, e.g. `--map emote5=click` |
| `--copy SRC=DST` | Duplicate a built sheet, e.g. `--copy idle=runIdle` |
| `--reverse SRC=DST` | Build from reversed frames, e.g. `--reverse intro=outro` |
| `--size N` | Frame size in px (default 325) |
| `--columns N` | Frames per row (default 10) |
| `--pad N` | Padding around the crop (default 8) |
| `--no-crop` | Frames are already tight and aligned |
| `--quantize` | Write 8-bit palette PNGs, matching the existing sheets |

### Example

```
python Tools/build_sheets.py raw/ Desktop_Gremlin/SpriteSheet/Gremlins/MyChar/ \
    --map emote5=click --copy idle=runIdle --reverse intro=outro --quantize
```

### Frame counts and timing

Playback advances exactly one frame per timer tick, and the timer runs at
`SPRITE_FRAMERATE` (see `AnimationController`). At the default of 60 that makes
60 frames equal one second, so capturing at 60fps keeps timing 1:1 with the
source clip.

`idle`, `runIdle`, `hover`, `grab`, `sleep`, `emote1` and `emote3` loop, so they
need seamless loop points. `intro`, `outro`, `click`, `emote2` and `emote4` play
once and reset.

### Notes

- `outro` is required for a clean exit. Shutdown fires when the animation
  completes, so without the sheet the tray's "Stylish Close" leaves the app
  running.
- `pat`, `idle2`, `jumpScare`, `poof`, `fireLeft`, `fireRight` and `reload`
  appear in the sprite map or config parser but no code path reaches them.
  Leave them at 0.
- New assets must be added to `Desktop_Gremlin.csproj` as `<Content>` entries
  with `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>`, or they
  will not be copied next to the executable.
- Large sheets are re-decoded from disk every tick unless `ALLOW_CACHE = true`
  in the main `config.txt`.
