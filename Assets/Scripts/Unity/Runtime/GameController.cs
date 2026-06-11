using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private bool verboseLogs;
        [SerializeField] private LevelDefinition[] levelDefinitions = Array.Empty<LevelDefinition>();
        [Header("Audio Feedback")]
        [SerializeField] private AudioClip tapSfx;
        [SerializeField] private AudioClip invalidSwapSfx;
        [SerializeField] private AudioClip matchSfx;
        [SerializeField] private AudioClip cascadeSfx;
        [SerializeField] private AudioClip potionSfx;
        [SerializeField] private AudioClip linePotionSfx;
        [SerializeField] private AudioClip bombPotionSfx;
        [SerializeField] private AudioClip lightningPotionSfx;
        [SerializeField] private AudioClip obstacleBreakSfx;
        [SerializeField] private AudioClip winSfx;
        [SerializeField] private AudioClip loseSfx;

        private PPQLogger _logger;
        private IReadOnlyList<LevelData> _levels;
        private ISaveRepository _saveRepository;
        private SaveData _saveData;
        private GeneratedGameUi _ui;
        private GameSession _session;
        private GridPosition? _selectedTile;
        private int _currentLevelNumber = 1;
        private AudioSource _audioSource;
        private bool _inputLocked;

        private void Start()
        {
            ConfigureRuntime();
            _logger = new PPQLogger(verboseLogs);
            _levels = new LevelCatalogLoader(_logger).LoadLevels(levelDefinitions).OrderBy(level => level.LevelNumber).ToArray();
            _saveRepository = new PlayerPrefsSaveRepository(_logger);
            _saveData = _saveRepository.Load();
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;

            _ui = new GeneratedGameUi(_logger);
            _ui.Build(
                transform,
                StartFirstUnlockedLevel,
                ShowLevelSelect,
                ShowSettings,
                QuitGame,
                StartLevel,
                HandleTilePressed,
                RestartCurrentLevel,
                StartNextLevel,
                ShowMainMenu,
                ResetProgress,
                ToggleMusic,
                ToggleSfx,
                PlaySfx);

            ShowMainMenu();
        }

        public void ShowMainMenu()
        {
            _selectedTile = null;
            _ui.ShowMainMenu();
        }

        private void ShowLevelSelect()
        {
            _selectedTile = null;
            _ui.ShowLevelSelect(_levels, _saveData.highestUnlockedLevel, StarsForLevel);
        }

        private void ShowSettings()
        {
            _ui.ShowSettings(_saveData.musicEnabled, _saveData.sfxEnabled);
        }

        private void StartFirstUnlockedLevel()
        {
            StartLevel(Mathf.Clamp(_saveData.highestUnlockedLevel, 1, _levels.Count));
        }

        private void StartLevel(int levelNumber)
        {
            var level = _levels.FirstOrDefault(item => item.LevelNumber == levelNumber);
            if (level == null)
            {
                _logger.Warn(LogCategory.Board, $"Level {levelNumber} does not exist.");
                return;
            }

            if (levelNumber > _saveData.highestUnlockedLevel)
            {
                _logger.Warn(LogCategory.UI, $"Level {levelNumber} is locked.");
                return;
            }

            _currentLevelNumber = levelNumber;
            _selectedTile = null;
            _session = new GameSession(level, random: new SystemRandomSource(), logger: _logger);
            _ui.ShowGame(_session, _selectedTile);
            _ui.ShowLevelIntro(_session);
        }

        private void RestartCurrentLevel()
        {
            StartLevel(_currentLevelNumber);
        }

        private void StartNextLevel()
        {
            var next = _currentLevelNumber + 1;
            if (_levels.Any(level => level.LevelNumber == next))
            {
                StartLevel(next);
                return;
            }

            ShowLevelSelect();
        }

        private void HandleTilePressed(GridPosition position)
        {
            if (_inputLocked || _session == null || _session.State != GameSessionState.Playing)
            {
                return;
            }

            if (!_selectedTile.HasValue)
            {
                _selectedTile = position;
                _ui.ShowGame(_session, _selectedTile, "Select an adjacent tile to swap.");
                return;
            }

            var first = _selectedTile.Value;
            if (first == position)
            {
                _selectedTile = null;
                _ui.ShowGame(_session, _selectedTile);
                return;
            }

            var result = _session.TrySwap(first, position);
            _selectedTile = null;
            StartCoroutine(ResolveMove(result));
        }

        private IEnumerator ResolveMove(MoveResult result)
        {
            _inputLocked = true;

            if (!result.ValidMove)
            {
                PlaySfx(GameSfxCue.InvalidSwap);
                yield return _ui.PlayMoveResult(_session, _selectedTile, result, UiFeedbackCue.InvalidSwap);
                _inputLocked = false;
                yield break;
            }

            var feedback = FeedbackFor(result);
            PlaySfx(SfxFor(result));
            yield return _ui.PlayMoveResult(_session, _selectedTile, result, feedback);

            if (_session.State == GameSessionState.Won)
            {
                CompleteCurrentLevel();
                PlaySfx(GameSfxCue.Win);
                _ui.ShowWin(_session, HasNextLevel());
            }
            else if (_session.State == GameSessionState.Lost)
            {
                PlaySfx(GameSfxCue.Lose);
                _ui.ShowLose(_session);
            }

            _inputLocked = false;
        }

        private void CompleteCurrentLevel()
        {
            SaveProgressService.ApplyLevelCompleted(_saveData, _currentLevelNumber, _session.Score, _session.Stars, HasNextLevel());
            _saveRepository.Save(_saveData);
        }

        private bool HasNextLevel()
        {
            return _levels.Any(level => level.LevelNumber == _currentLevelNumber + 1);
        }

        private int StarsForLevel(int levelNumber)
        {
            return _saveData.levelProgress.FirstOrDefault(progress => progress.levelNumber == levelNumber)?.stars ?? 0;
        }

        private void ToggleMusic(bool enabled)
        {
            _saveData.musicEnabled = enabled;
            _saveRepository.Save(_saveData);
        }

        private void ToggleSfx(bool enabled)
        {
            _saveData.sfxEnabled = enabled;
            _saveRepository.Save(_saveData);
        }

        private void ResetProgress()
        {
            _saveRepository.Reset();
            _saveData = new SaveData();
            ShowMainMenu();
        }

        private void QuitGame()
        {
            _logger.Log(LogCategory.UI, "Quit requested.");
            Application.Quit();
        }

        private static void ConfigureRuntime()
        {
            Application.targetFrameRate = 60;
            if (Application.isMobilePlatform)
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
                Screen.orientation = ScreenOrientation.Portrait;
            }
        }

        private static UiFeedbackCue FeedbackFor(MoveResult result)
        {
            if (result.CreatedPotions.Count > 0)
            {
                return UiFeedbackCue.Potion;
            }

            if (result.Cascades > 0)
            {
                return UiFeedbackCue.Cascade;
            }

            return UiFeedbackCue.Match;
        }

        private static GameSfxCue SfxFor(MoveResult result)
        {
            if (result.AnimationEvents.Any(item => item.Kind == BoardAnimationEventKind.ObstacleDestroyed))
            {
                return GameSfxCue.ObstacleBreak;
            }

            var potionEvent = result.AnimationEvents.FirstOrDefault(item =>
                item.Kind == BoardAnimationEventKind.PotionCreated
                || item.Kind == BoardAnimationEventKind.PotionActivated);
            if (potionEvent != null)
            {
                switch (potionEvent.Potion)
                {
                    case PotionType.LineHorizontal:
                    case PotionType.LineVertical:
                        return GameSfxCue.LinePotion;
                    case PotionType.Bomb:
                        return GameSfxCue.BombPotion;
                    case PotionType.Lightning:
                        return GameSfxCue.LightningPotion;
                    default:
                        return GameSfxCue.Potion;
                }
            }

            return result.Cascades > 0 ? GameSfxCue.Cascade : GameSfxCue.Match;
        }

        private void PlaySfx(GameSfxCue cue)
        {
            if (_saveData != null && !_saveData.sfxEnabled)
            {
                return;
            }

            var clip = ClipFor(cue);
            if (clip == null || _audioSource == null)
            {
                return;
            }

            _audioSource.PlayOneShot(clip);
        }

        private AudioClip ClipFor(GameSfxCue cue)
        {
            switch (cue)
            {
                case GameSfxCue.Tap:
                    return tapSfx;
                case GameSfxCue.InvalidSwap:
                    return invalidSwapSfx;
                case GameSfxCue.Match:
                    return matchSfx;
                case GameSfxCue.Cascade:
                    return cascadeSfx;
                case GameSfxCue.Potion:
                    return potionSfx;
                case GameSfxCue.LinePotion:
                    return linePotionSfx != null ? linePotionSfx : potionSfx;
                case GameSfxCue.BombPotion:
                    return bombPotionSfx != null ? bombPotionSfx : potionSfx;
                case GameSfxCue.LightningPotion:
                    return lightningPotionSfx != null ? lightningPotionSfx : potionSfx;
                case GameSfxCue.ObstacleBreak:
                    return obstacleBreakSfx != null ? obstacleBreakSfx : matchSfx;
                case GameSfxCue.Win:
                    return winSfx;
                case GameSfxCue.Lose:
                    return loseSfx;
                default:
                    return null;
            }
        }
    }
}
