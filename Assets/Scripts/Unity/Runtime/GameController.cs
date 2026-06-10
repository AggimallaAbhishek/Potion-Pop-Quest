using System;
using System.Collections.Generic;
using System.Linq;
using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private bool verboseLogs = true;
        [SerializeField] private LevelDefinition[] levelDefinitions = Array.Empty<LevelDefinition>();

        private PPQLogger _logger;
        private IReadOnlyList<LevelData> _levels;
        private ISaveRepository _saveRepository;
        private SaveData _saveData;
        private GeneratedGameUi _ui;
        private GameSession _session;
        private GridPosition? _selectedTile;
        private int _currentLevelNumber = 1;

        private void Start()
        {
            _logger = new PPQLogger(verboseLogs);
            _levels = new LevelCatalogLoader(_logger).LoadLevels(levelDefinitions).OrderBy(level => level.LevelNumber).ToArray();
            _saveRepository = new PlayerPrefsSaveRepository(_logger);
            _saveData = _saveRepository.Load();

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
                ToggleSfx);

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
            if (_session == null || _session.State != GameSessionState.Playing)
            {
                return;
            }

            if (!_selectedTile.HasValue)
            {
                _selectedTile = position;
                _ui.ShowGame(_session, _selectedTile);
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
            _ui.ShowGame(_session, _selectedTile, result.Message);

            if (!result.ValidMove)
            {
                return;
            }

            if (_session.State == GameSessionState.Won)
            {
                CompleteCurrentLevel();
                _ui.ShowWin(_session, HasNextLevel());
            }
            else if (_session.State == GameSessionState.Lost)
            {
                _ui.ShowLose(_session);
            }
        }

        private void CompleteCurrentLevel()
        {
            var progress = _saveData.GetOrCreateLevelProgress(_currentLevelNumber);
            progress.bestScore = Math.Max(progress.bestScore, _session.Score);
            progress.stars = Math.Max(progress.stars, _session.Stars);

            if (HasNextLevel())
            {
                _saveData.highestUnlockedLevel = Math.Max(_saveData.highestUnlockedLevel, _currentLevelNumber + 1);
            }

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
    }
}

