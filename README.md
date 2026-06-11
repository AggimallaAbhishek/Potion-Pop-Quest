# Potion Pop Quest

Potion Pop Quest is a Unity 2D match-3 puzzle MVP for Android-first development. The game follows the attached development document: an 8x8 ingredient board, limited moves, goals, stars, 10 MVP levels, line/bomb/lightning potions, wooden boxes, dark magic tiles, local save progress, and touch-friendly UI.

## Tech Stack

- Engine: Unity 6.3 LTS
- Language: C#
- UI: Unity UGUI generated at runtime
- Data: `Resources` JSON catalog with ScriptableObject-compatible definitions
- Persistence: `PlayerPrefs` repository behind `ISaveRepository`
- Tests: Unity Test Framework EditMode tests
- Primary target: Android APK/AAB
- Secondary verification target: WebGL build for browser smoke tests

## Current Implementation

- Pure gameplay logic lives in `Assets/Scripts/Core`.
- Unity adapter/runtime code lives in `Assets/Scripts/Unity`.
- MVP level data lives in `Assets/Resources/Levels/mvp_levels.json`.
- Unit tests live in `Assets/Tests/EditMode`.
- The runtime bootstraps itself with `RuntimeInitializeOnLoadMethod`, so it can run from a simple scene.

## First Unity Setup

1. Open this folder in Unity Hub with Unity 6.3 LTS.
2. Wait for packages to resolve and scripts to compile.
3. Run `Potion Pop Quest > Create MVP Scene` from the Unity menu.
4. Open `Assets/Scenes/GameScene.unity`.
5. Press Play.

The generated UI includes main menu, level select, game board, HUD, settings, win, and lose flows.

For Android/WebGL defaults, also run:

```text
Potion Pop Quest > Configure Build Settings
```

## Tests

Run the EditMode tests from Unity Test Runner:

- `Window > General > Test Runner`
- Select `EditMode`
- Run `PotionPopQuest.Tests`

The tests cover board generation, match classification, obstacle clearing, dark tile clearing, and goal tracking.
They also cover move-count behavior, save progress updates, and PlayerPrefs save/load persistence.

For manual level QA, use:

```text
Potion Pop Quest > QA > Unlock All MVP Levels
Potion Pop Quest > QA > Reset Local Progress
```

These editor actions let you test levels 1-10 without replaying the full unlock sequence.

## Android Build

1. Install Android Build Support through Unity Hub.
2. Open `File > Build Profiles`.
3. Add or select Android.
4. Use APK for local device testing or AAB for Google Play.
5. Build and run on a physical device or emulator.

## Notes

- No backend, ads, IAP, cloud save, or leaderboard is included in the MVP.
- The UI uses generated placeholder ingredient and obstacle icons; final hand-made art can replace these later.
- Debug logs use `[PotionPopQuest][Category]` prefixes and are disabled by default on `GameController`.
- SFX hooks are wired on `GameController`; assign clips in the Inspector for tap, swap failure, matches, cascades, potions, win, and lose.
- This environment does not currently expose `unity` or `dotnet` on PATH, so compile/build/browser verification must be completed inside Unity.
