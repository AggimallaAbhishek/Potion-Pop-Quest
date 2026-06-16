# Potion Pop Quest QA Checklist

Use this checklist before level tuning, Android/WebGL builds, or portfolio capture.

1. Open the project in Unity `6000.3.17f1`.
2. Stop Play Mode, run `Assets > Refresh`, and clear the Console.
3. Run `Window > General > Test Runner`.
4. Run all EditMode tests, then all PlayMode tests.
5. Run `Potion Pop Quest > QA > Run Level QA Simulator`.
6. Review Console output and the exported reports in `Library/PotionPopQuestQa`.
7. Manually smoke test menu, level select, settings, level intro, HUD, hint, boosters, pause, win, lose, replay, next level, shop, daily reward, and reset progress.
8. Confirm there are zero red Console errors before building.

Economy note:

- Coin packages, lives, boosters, shop, and daily rewards are test economy systems only.
- They are not real purchases and must not be presented as real IAP until platform purchase validation exists.

Level tuning targets:

- Levels 1-7: roughly 70%-95% simulator win rate.
- Levels 8, 15, and 20 may be harder, but should stay above roughly 55%-60%.
- All levels should report `0` stuck boards.

Platform checks:

- Android: portrait layout, safe area, touch input, back button, save persistence, audio settings, and repeated level restarts.
- WebGL: browser load, local save persistence, first-click music unlock, and no browser console errors.
