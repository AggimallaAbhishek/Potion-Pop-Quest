# Potion Pop Quest Implementation Notes

## Architecture

The project is split into a pure C# core and a Unity adapter layer.

- `PotionPopQuest.Core` has no UnityEngine dependency. It owns board state, rules, matching, gravity, scoring, goals, potions, obstacles, save models, and the 10-level fallback catalog.
- `PotionPopQuest.Unity` adapts that core into Unity runtime behavior: logging, level loading, local save persistence, generated UGUI screens, and the game controller.
- `PotionPopQuest.Editor` adds a Unity menu item to create the MVP scene and register it in build settings.
- `TileIconFactory` generates temporary ingredient, obstacle, and potion icons at runtime so the board is visual before final art is ready.

## Gameplay Flow

The session flow is:

`Tile input -> swap validation -> match or potion activation -> clear/damage -> gravity/refill -> cascades -> goal/score update -> win/lose check -> UI refresh`

Invalid swaps are reversed and do not consume a move. Valid swaps consume one move. Cascades are capped at 20 passes to prevent infinite board resolution.

UI feedback currently uses generated UGUI animations: selected-tile outline, invalid-swap board shake, match/cascade/potion board pulse, tile pop-in, and modal intro transitions.

## MVP Data

The MVP data is available in two places:

- `Assets/Resources/Levels/mvp_levels.json` for Unity runtime tuning.
- `MvpLevelCatalog` as a code fallback if JSON loading fails.

The JSON format mirrors the requested public interfaces: level number, board size, moves, active ingredients, goals, star thresholds, and obstacle positions.

## Verification Gaps In This Environment

The source implementation is complete enough to open in Unity, but this shell does not have `unity` or `dotnet` on PATH. Required follow-up verification inside Unity:

- Compile scripts after package resolution.
- Run EditMode tests.
- Create and open the MVP scene from the Unity menu.
- Run `Potion Pop Quest > Configure Build Settings`.
- Build WebGL and run browser smoke checks.
- Build Android APK/AAB and test touch input on device/emulator.
