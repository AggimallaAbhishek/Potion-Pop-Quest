# Potion Pop Quest QA And Build Checklist

This checklist is the baseline for the portfolio/playtest vertical slice.

## Verification Order

1. Stop Play Mode in Unity.
2. Select `Assets > Refresh`.
3. Clear the Console.
4. Run `Window > General > Test Runner > EditMode > Run All`.
5. Run `Window > General > Test Runner > PlayMode > Run All`.
6. Run `Potion Pop Quest > Run Level QA Simulator`.
7. Run `Potion Pop Quest > Configure Build Settings`.
8. Build Android development APK and WebGL development build.
9. Smoke test Android, WebGL, and editor gameplay.

## Level QA Acceptance

- Levels 1-20: `0` stuck boards.
- Levels 1-7: roughly `70%-95%` win rate.
- Hard levels 8, 15, and 20: above roughly `55%-60%`.
- Record exported QA reports from `Library/PotionPopQuestQa`.

## Android Smoke Test

| Area | Expected |
| --- | --- |
| Install | APK installs without warnings beyond development-build notices. |
| Launch | App starts in portrait and reaches main menu. |
| Safe area | Top economy bar does not overlap status bar or notch. |
| Touch | Menu buttons, level cards, board tiles, hint, boosters, and settings respond. |
| Gameplay | Valid swaps decrement moves; invalid swaps do not. |
| Persistence | Save progress and settings persist after app restart. |
| Audio | Music/SFX toggles and sliders persist. |
| Back | Android Back opens settings/pause-style flow during gameplay and exits modals first. |
| Stability | No red Console/logcat errors during 10 minutes of play. |

## WebGL Smoke Test

| Area | Expected |
| --- | --- |
| Load | Browser reaches main menu without console errors. |
| First gesture audio | Music starts only after first click/tap. |
| Gameplay | Level select and level start work. |
| Save | Progress persists after browser refresh. |
| UI scale | Layout is readable at `960x600`, `1280x720`, and a mobile-like narrow viewport. |
| Compatibility | Unsupported platform features are guarded and do not throw. |

## Manual 20-Level Pass

For every level:

- Start level from level select.
- Dismiss intro.
- Make one invalid swap and confirm no move loss.
- Use hint once.
- Complete or fail naturally.
- Confirm win/lose modal has a clear primary action.
- Confirm replay and next-level flows do not corrupt progress.

Special focus:

- Level 4: line potion creation.
- Level 5: wooden box damage/break.
- Level 7: dark tile clearing.
- Level 8: bomb potion creation.
- Level 10 and 20: larger cascades and final next-level routing.

## Portfolio Capture

- Main menu screenshot.
- Level select screenshot.
- Level 1 gameplay screenshot.
- Potion creation screenshot.
- Obstacle level screenshot.
- Win modal screenshot.
- 20-30 second gameplay clip showing swap, match, cascade, powerup, and win/lose feedback.
