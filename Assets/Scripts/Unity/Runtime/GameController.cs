using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PotionPopQuest.Core;
using UnityEngine;
using UnityEngine.Audio;

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
        [SerializeField] private AudioClip musicLoop;
        [Header("Audio Routing")]
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;

        private PPQLogger _logger;
        private IReadOnlyList<LevelData> _levels;
        private ISaveRepository _saveRepository;
        private SaveData _saveData;
        private GeneratedGameUi _ui;
        private GameSession _session;
        private GridPosition? _selectedTile;
        private int _currentLevelNumber = 1;
        private readonly BoardMoveFinder _moveFinder = new BoardMoveFinder();
        private readonly Dictionary<GameSfxCue, float> _lastCueTimes = new Dictionary<GameSfxCue, float>();
        private AudioSource _sfxSource;
        private AudioSource _musicSource;
        private bool _inputLocked;
        private bool _hintVisible;
        private float _idleHintTimer;
        private bool _musicGestureUnlocked;
        private float _nextEconomyUiRefreshTime;

        private void Start()
        {
            ConfigureRuntime();
            _logger = new PPQLogger(verboseLogs);
            _levels = new LevelCatalogLoader(_logger).LoadLevels(levelDefinitions).OrderBy(level => level.LevelNumber).ToArray();
            _saveRepository = new PlayerPrefsSaveRepository(_logger);
            _saveData = _saveRepository.Load();
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            ApplyAudioRouting();
            LoadFallbackAudioClips();
            ApplyMusicState();

            _ui = new GeneratedGameUi(_logger);
            _ui.Build(transform, new GeneratedGameUiActions
            {
                Play = StartFirstUnlockedLevel,
                ShowLevels = ShowLevelSelect,
                ShowSettings = ShowSettings,
                Quit = QuitGame,
                StartLevel = StartLevel,
                TilePressed = HandleTilePressed,
                HintRequested = RequestHint,
                Restart = RestartCurrentLevel,
                NextLevel = StartNextLevel,
                MainMenu = ShowMainMenu,
                ResetProgress = ResetProgress,
                ToggleMusic = ToggleMusic,
                ToggleSfx = ToggleSfx,
                SetMusicVolume = SetMusicVolume,
                SetSfxVolume = SetSfxVolume,
                ToggleVibration = ToggleVibration,
                LevelIntroDismissed = DismissLevelIntro,
                PlaySfx = PlaySfx,
                BuyLivesPressed = HandleBuyLives,
                HammerBoosterPressed = RequestHammerBooster,
                ShuffleBoosterPressed = RequestShuffleBooster,
                ShowShop = ShowShop,
                CloseShop = CloseShop,
                BuyCoinPackage = BuyCoinPackage,
                ClaimDailyReward = ClaimDailyReward
            });

            UpdateEconomyUi();
            ShowMainMenu();
        }

        private bool _hammerModeActive;

        private void Update()
        {
            if (_saveData != null)
            {
                if (EconomyManager.ProcessLifeRegeneration(_saveData))
                {
                    _saveRepository.Save(_saveData);
                    UpdateEconomyUi();
                }
                else if (Time.unscaledTime >= _nextEconomyUiRefreshTime)
                {
                    UpdateEconomyUi();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_hammerModeActive)
                {
                    CancelHammerMode();
                    return;
                }
                HandleBackRequested();
                return;
            }

            if (_inputLocked || _hintVisible || _session == null || _session.State != GameSessionState.Playing)
            {
                return;
            }

            _idleHintTimer += Time.unscaledDeltaTime;
            if (_idleHintTimer >= GameplayPresentationConfig.AutoHintDelay)
            {
                RequestHint();
            }
        }

        public void ShowMainMenu()
        {
            ClearHintState();
            _selectedTile = null;
            _ui.ShowMainMenu();
            CheckAndShowDailyReward();
        }

        private void ShowLevelSelect()
        {
            ClearHintState();
            _selectedTile = null;
            _ui.ShowLevelSelect(_levels, _saveData.highestUnlockedLevel, StarsForLevel);
        }

        private void ShowSettings()
        {
            ClearHintState();
            _ui.ShowSettings(
                _saveData.musicEnabled,
                _saveData.sfxEnabled,
                _saveData.musicVolume,
                _saveData.sfxVolume,
                _saveData.vibrationEnabled);
        }

        private void StartFirstUnlockedLevel()
        {
            StartLevel(Mathf.Clamp(_saveData.highestUnlockedLevel, 1, _levels.Count));
        }

        private void StartLevel(int levelNumber)
        {
            if (_saveData.currentLives <= 0)
            {
                _logger.Log(LogCategory.UI, "Out of lives. Prompting shop.");
                ShowShop();
                return;
            }

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
            ClearHintState();
            _selectedTile = null;
            _session = new GameSession(level, random: new SystemRandomSource(), logger: _logger);
            _inputLocked = true;
            _ui.ShowGame(_session, _selectedTile);
            _ui.ShowLevelIntro(_session);
            _ui.ShowTutorial(level);
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

            ClearHintState();

            if (_hammerModeActive)
            {
                _hammerModeActive = false;
                _saveData.hammerBoosters--;
                _saveRepository.Save(_saveData);
                UpdateEconomyUi();
                StartCoroutine(ResolveMove(_session.UseHammer(position)));
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

        private void RequestHint()
        {
            if (_inputLocked || _session == null || _session.State != GameSessionState.Playing)
            {
                return;
            }

            _idleHintTimer = 0f;
            if (_moveFinder.TryFindValidMove(_session.Board, out var move))
            {
                _ui.ShowHint(move);
                _hintVisible = true;
                _logger.Log(LogCategory.UI, $"Hint shown for {move.First} -> {move.Second}.");
                return;
            }

            _logger.Log(LogCategory.Board, "Hint requested but no valid move exists; attempting board shuffle.");
            ClearHintState();
            StartCoroutine(ResolveMove(_session.TryShuffleIfNeeded()));
        }

        private IEnumerator ResolveMove(MoveResult result)
        {
            _inputLocked = true;
            try
            {
                if (!result.ValidMove)
                {
                    PlaySfx(GameSfxCue.InvalidSwap);
                    yield return _ui.PlayMoveResult(_session, _selectedTile, result, UiFeedbackCue.InvalidSwap);
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
                    EconomyManager.TryConsumeLife(_saveData);
                    _saveRepository.Save(_saveData);
                    UpdateEconomyUi();
                    PlaySfx(GameSfxCue.Lose);
                    _ui.ShowLose(_session);
                }
            }
            finally
            {
                _idleHintTimer = 0f;
                _inputLocked = false;
            }
        }

        private void ClearHintState()
        {
            _hintVisible = false;
            _idleHintTimer = 0f;
            _ui?.ClearHint();
        }

        private void CompleteCurrentLevel()
        {
            SaveProgressService.ApplyLevelCompleted(_saveData, _currentLevelNumber, _session.Score, _session.Stars, HasNextLevel());
            _saveRepository.Save(_saveData);
            UpdateEconomyUi();
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
            ApplyMusicState();
        }

        private void ToggleSfx(bool enabled)
        {
            _saveData.sfxEnabled = enabled;
            _saveRepository.Save(_saveData);
        }

        private void SetMusicVolume(float volume)
        {
            _saveData.musicVolume = Mathf.Clamp01(volume);
            _saveRepository.Save(_saveData);
            ApplyMusicState();
        }

        private void SetSfxVolume(float volume)
        {
            _saveData.sfxVolume = Mathf.Clamp01(volume);
            _saveRepository.Save(_saveData);
            if (_sfxSource != null)
            {
                _sfxSource.volume = _saveData.sfxVolume;
            }
        }

        private void ToggleVibration(bool enabled)
        {
            _saveData.vibrationEnabled = enabled;
            _saveRepository.Save(_saveData);
            _logger.Log(LogCategory.UI, $"Vibration placeholder set to {enabled}.");
        }

        private void DismissLevelIntro()
        {
            _inputLocked = false;
            _idleHintTimer = 0f;
            _logger.Log(LogCategory.UI, $"Level {_currentLevelNumber} intro dismissed.");
        }

        private void ResetProgress()
        {
            _saveRepository.Reset();
            _saveData = new SaveData();
            ApplyMusicState();
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

        private void HandleBackRequested()
        {
            ClearHintState();
            if (_ui != null && _ui.IsModalOpen())
            {
                _ui.CloseTopModal();
                return;
            }

            if (_session != null && _session.State == GameSessionState.Playing)
            {
                ShowSettings();
                return;
            }

            ShowMainMenu();
        }

        private void UpdateEconomyUi()
        {
            if (_ui == null || _saveData == null)
            {
                return;
            }

            _ui.UpdateEconomy(_saveData.currentLives, EconomyManager.GetSecondsUntilNextLife(_saveData), _saveData.coins, _saveData.hammerBoosters, _saveData.shuffleBoosters);
            _nextEconomyUiRefreshTime = Time.unscaledTime + 1f;
        }

        private void HandleBuyLives()
        {
            if (EconomyManager.TryPurchaseLives(_saveData))
            {
                _saveRepository.Save(_saveData);
                UpdateEconomyUi();
                _logger.Log(LogCategory.UI, "Bought lives.");
            }
        }

        private void RequestHammerBooster()
        {
            if (_session == null || _session.State != GameSessionState.Playing) return;

            if (_saveData.hammerBoosters > 0)
            {
                _hammerModeActive = true;
                _ui.ShowGame(_session, null, "Select a tile to smash!");
            }
            else if (EconomyManager.TryPurchaseBooster(_saveData, BoosterType.Hammer))
            {
                _saveRepository.Save(_saveData);
                UpdateEconomyUi();
                _hammerModeActive = true;
                _ui.ShowGame(_session, null, "Select a tile to smash!");
            }
        }

        private void CancelHammerMode()
        {
            _hammerModeActive = false;
            _ui.ShowGame(_session, null, "");
        }

        private void RequestShuffleBooster()
        {
            if (_session == null || _session.State != GameSessionState.Playing) return;

            if (_saveData.shuffleBoosters > 0)
            {
                _saveData.shuffleBoosters--;
                _saveRepository.Save(_saveData);
                StartCoroutine(ResolveMove(_session.ForceShuffle()));
                UpdateEconomyUi();
            }
            else if (EconomyManager.TryPurchaseBooster(_saveData, BoosterType.Shuffle))
            {
                _saveData.shuffleBoosters--; // Consume it immediately
                _saveRepository.Save(_saveData);
                StartCoroutine(ResolveMove(_session.ForceShuffle()));
                UpdateEconomyUi();
            }
        }

        private void CheckAndShowDailyReward()
        {
            if (EconomyManager.CheckDailyRewardAvailable(_saveData))
            {
                _ui.ShowDailyReward();
            }
        }

        private void ShowShop()
        {
            _ui.ShowShop();
        }

        private void CloseShop()
        {
            // Update UI in case anything changed
            UpdateEconomyUi();
        }

        private void BuyCoinPackage(int amount)
        {
            EconomyManager.PurchaseCoinPackage(_saveData, amount);
            _saveRepository.Save(_saveData);
            UpdateEconomyUi();
            _logger.Log(LogCategory.UI, $"Purchased {amount} coins.");
        }

        private void ClaimDailyReward()
        {
            var reward = EconomyManager.ClaimDailyReward(_saveData);
            _saveRepository.Save(_saveData);
            UpdateEconomyUi();
            _logger.Log(LogCategory.UI, $"Claimed daily reward of {reward} coins.");
        }

        private static UiFeedbackCue FeedbackFor(MoveResult result)
        {
            if (result.AnimationEvents.Any(item => item.Kind == BoardAnimationEventKind.BoardShuffled))
            {
                return UiFeedbackCue.Cascade;
            }

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

            if (result.AnimationEvents.Any(item => item.Kind == BoardAnimationEventKind.BoardShuffled))
            {
                return GameSfxCue.Cascade;
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
            UnlockWebGlMusicAfterGesture();
            if (_saveData != null && !_saveData.sfxEnabled)
            {
                return;
            }

            if (_lastCueTimes.TryGetValue(cue, out var lastTime)
                && Time.unscaledTime - lastTime < GameplayPresentationConfig.RepeatedCueCooldown)
            {
                return;
            }

            var clip = ClipFor(cue);
            if (clip == null || _sfxSource == null)
            {
                return;
            }

            _lastCueTimes[cue] = Time.unscaledTime;
            _sfxSource.volume = _saveData != null ? Mathf.Clamp01(_saveData.sfxVolume) : GameplayPresentationConfig.DefaultSfxVolume;
            _sfxSource.pitch = PitchFor(cue);
            _sfxSource.PlayOneShot(clip);
            _sfxSource.pitch = 1f;
        }

        private void UnlockWebGlMusicAfterGesture()
        {
            if (_musicGestureUnlocked)
            {
                return;
            }

            _musicGestureUnlocked = true;
            ApplyMusicState();
        }

        private static float PitchFor(GameSfxCue cue)
        {
            switch (cue)
            {
                case GameSfxCue.Match:
                case GameSfxCue.Cascade:
                case GameSfxCue.Tap:
                    return UnityEngine.Random.Range(0.94f, 1.06f);
                default:
                    return 1f;
            }
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

        private void LoadFallbackAudioClips()
        {
            tapSfx = tapSfx != null ? tapSfx : Resources.Load<AudioClip>("Audio/SFX/tap");
            invalidSwapSfx = invalidSwapSfx != null ? invalidSwapSfx : Resources.Load<AudioClip>("Audio/SFX/invalid_swap");
            matchSfx = matchSfx != null ? matchSfx : Resources.Load<AudioClip>("Audio/SFX/match");
            cascadeSfx = cascadeSfx != null ? cascadeSfx : Resources.Load<AudioClip>("Audio/SFX/cascade");
            potionSfx = potionSfx != null ? potionSfx : Resources.Load<AudioClip>("Audio/SFX/potion");
            linePotionSfx = linePotionSfx != null ? linePotionSfx : Resources.Load<AudioClip>("Audio/SFX/line_potion");
            bombPotionSfx = bombPotionSfx != null ? bombPotionSfx : Resources.Load<AudioClip>("Audio/SFX/bomb_potion");
            lightningPotionSfx = lightningPotionSfx != null ? lightningPotionSfx : Resources.Load<AudioClip>("Audio/SFX/lightning_potion");
            obstacleBreakSfx = obstacleBreakSfx != null ? obstacleBreakSfx : Resources.Load<AudioClip>("Audio/SFX/obstacle_break");
            winSfx = winSfx != null ? winSfx : Resources.Load<AudioClip>("Audio/SFX/win");
            loseSfx = loseSfx != null ? loseSfx : Resources.Load<AudioClip>("Audio/SFX/lose");
            musicLoop = musicLoop != null ? musicLoop : Resources.Load<AudioClip>("Audio/Music/potion_lab_loop");
        }

        private void ApplyAudioRouting()
        {
            if (_musicSource != null && musicMixerGroup != null)
            {
                _musicSource.outputAudioMixerGroup = musicMixerGroup;
            }

            if (_sfxSource != null && sfxMixerGroup != null)
            {
                _sfxSource.outputAudioMixerGroup = sfxMixerGroup;
            }
        }

        private void ApplyMusicState()
        {
            if (_musicSource == null)
            {
                return;
            }

            if (_saveData != null && _saveData.musicEnabled && musicLoop != null)
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer && !_musicGestureUnlocked)
                {
                    _musicSource.Stop();
                    return;
                }

                if (_musicSource.clip != musicLoop)
                {
                    _musicSource.clip = musicLoop;
                }

                _musicSource.volume = Mathf.Clamp01(_saveData.musicVolume);
                if (!_musicSource.isPlaying)
                {
                    _musicSource.Play();
                }

                return;
            }

            _musicSource.Stop();
        }
    }
}
