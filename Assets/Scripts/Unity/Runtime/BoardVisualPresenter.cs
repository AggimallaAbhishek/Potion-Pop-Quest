using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PotionPopQuest.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class BoardVisualPresenter
    {
        private const float BoardSizePortrait = 820f;
        private const float BoardSizeLandscape = 640f;
        private const float BoardSizeFallback = 720f;
        private const float MinBoardSize = 560f;
        private const float MaxBoardSize = 840f;
        private const int BoardPadding = 24;

        private readonly IGameLogger _logger;
        private readonly TileIconFactory _iconFactory;
        private readonly Func<Font> _fontProvider;
        private readonly Dictionary<GridPosition, RectTransform> _tileViews = new Dictionary<GridPosition, RectTransform>();
        private readonly Dictionary<GridPosition, BoardCellSnapshot> _viewCells = new Dictionary<GridPosition, BoardCellSnapshot>();
        private readonly Stack<Button> _tileButtonPool = new Stack<Button>();
        private readonly Stack<Image> _vfxImagePool = new Stack<Image>();
        private readonly List<Outline> _selectionOutlines = new List<Outline>();

        private RectTransform _boardRoot;
        private RectTransform _floatingLayer;
        private Action<GridPosition> _tilePressed;
        private Action<GameSfxCue> _playSfx;
        private int _width = 8;
        private int _height = 8;
        private float _boardSize = BoardSizeFallback;
        private float _cellSize = 78f;
        private float _spacing = 8f;

        public BoardVisualPresenter(IGameLogger logger, TileIconFactory iconFactory, Func<Font> fontProvider)
        {
            _logger = logger ?? new NullGameLogger();
            _iconFactory = iconFactory ?? new TileIconFactory();
            _fontProvider = fontProvider ?? (() => null);
        }

        public IReadOnlyDictionary<GridPosition, RectTransform> TileViews => _tileViews;

        public void Configure(
            RectTransform boardRoot,
            RectTransform floatingLayer,
            Action<GridPosition> tilePressed,
            Action<GameSfxCue> playSfx)
        {
            _boardRoot = boardRoot;
            _floatingLayer = floatingLayer;
            _tilePressed = tilePressed;
            _playSfx = playSfx;
        }

        public bool TryGetTile(GridPosition position, out RectTransform rect)
        {
            return _tileViews.TryGetValue(position, out rect) && rect != null;
        }

        public void Render(BoardSnapshot snapshot, GridPosition? selectedTile, UiFeedbackCue feedbackCue)
        {
            if (snapshot == null || _boardRoot == null)
            {
                return;
            }

            ConfigureBoardFrame(snapshot.Width, snapshot.Height);
            ClearSelectionOutlines();
            ReleaseAllTiles();

            foreach (var position in snapshot.AllPositions())
            {
                var rect = CreateTileView(position, snapshot.GetCell(position));
                if (feedbackCue != UiFeedbackCue.None)
                {
                    rect.gameObject.AddComponent<UiTileAnimator>().PlayIntro((position.Row * snapshot.Width + position.Column) * 0.0025f, feedbackCue);
                }
            }

            ApplySelection(selectedTile);
            KeepFloatingLayerOnTop();
        }

        public IEnumerator Play(IReadOnlyList<BoardAnimationEvent> events, BoardSnapshot finalSnapshot)
        {
            if (events == null || events.Count == 0)
            {
                SyncToSnapshot(finalSnapshot, null);
                yield break;
            }

            var previousWasSwap = false;
            for (var index = 0; index < events.Count; index++)
            {
                var animationEvent = events[index];
                if (animationEvent.Kind == BoardAnimationEventKind.TileDropped || animationEvent.Kind == BoardAnimationEventKind.TileSpawned)
                {
                    var batch = new List<BoardAnimationEvent> { animationEvent };
                    while (index + 1 < events.Count
                           && (events[index + 1].Kind == BoardAnimationEventKind.TileDropped || events[index + 1].Kind == BoardAnimationEventKind.TileSpawned)
                           && events[index + 1].CascadeIndex == animationEvent.CascadeIndex)
                    {
                        index++;
                        batch.Add(events[index]);
                    }

                    yield return PlayMovementBatch(batch);
                    previousWasSwap = false;
                    continue;
                }

                switch (animationEvent.Kind)
                {
                    case BoardAnimationEventKind.Swap:
                        yield return Swap(animationEvent.From, animationEvent.To);
                        previousWasSwap = true;
                        break;
                    case BoardAnimationEventKind.InvalidSwap:
                        if (previousWasSwap)
                        {
                            yield return Swap(animationEvent.From, animationEvent.To);
                        }
                        else
                        {
                            yield return ShakePositions(animationEvent.Positions);
                        }

                        previousWasSwap = false;
                        break;
                    case BoardAnimationEventKind.CascadeStarted:
                        yield return new WaitForSecondsRealtime(GameplayPresentationConfig.CascadeDelay);
                        previousWasSwap = false;
                        break;
                    case BoardAnimationEventKind.Clear:
                        yield return ClearPositions(animationEvent.Positions);
                        previousWasSwap = false;
                        break;
                    case BoardAnimationEventKind.PotionCreated:
                        yield return CreatePotion(animationEvent);
                        previousWasSwap = false;
                        break;
                    case BoardAnimationEventKind.PotionActivated:
                        yield return PotionBurst(animationEvent);
                        yield return ReleaseAffectedIngredientViews(animationEvent.Positions);
                        previousWasSwap = false;
                        break;
                    case BoardAnimationEventKind.ObstacleDamaged:
                        yield return FlashPositions(animationEvent.Positions, new Color(1f, 0.58f, 0.28f, 1f));
                        previousWasSwap = false;
                        break;
                    case BoardAnimationEventKind.ObstacleDestroyed:
                        yield return DestroyObstacles(animationEvent.Positions);
                        previousWasSwap = false;
                        break;
                    case BoardAnimationEventKind.Win:
                    case BoardAnimationEventKind.Lose:
                        yield return Pulse(_boardRoot, 1.035f, GameplayPresentationConfig.BoardPulseDuration);
                        previousWasSwap = false;
                        break;
                    case BoardAnimationEventKind.BoardShuffled:
                        yield return Shuffle(animationEvent);
                        previousWasSwap = false;
                        break;
                }
            }

            SyncToSnapshot(finalSnapshot, null);
        }

        public void SyncToSnapshot(BoardSnapshot snapshot, GridPosition? selectedTile)
        {
            if (snapshot == null || _boardRoot == null)
            {
                return;
            }

            ConfigureBoardFrame(snapshot.Width, snapshot.Height);
            ClearSelectionOutlines();

            var validPositions = new HashSet<GridPosition>(snapshot.AllPositions());
            foreach (var position in _tileViews.Keys.Where(position => !validPositions.Contains(position)).ToArray())
            {
                ReleaseTile(position);
            }

            foreach (var position in snapshot.AllPositions())
            {
                var cell = snapshot.GetCell(position);
                if (!_tileViews.TryGetValue(position, out var rect) || rect == null)
                {
                    CreateTileView(position, cell);
                    continue;
                }

                ConfigureTileRect(rect, position);
                UpdateTileContent(rect, position, cell);
            }

            ApplySelection(selectedTile);
            KeepFloatingLayerOnTop();
        }

        private IEnumerator Swap(GridPosition first, GridPosition second)
        {
            if (!TryGetTile(first, out var firstRect) || !TryGetTile(second, out var secondRect))
            {
                _logger.Warn(LogCategory.UI, $"Could not animate swap {first} -> {second}; one or both tile views were missing.");
                yield break;
            }

            var firstStart = CellPosition(first);
            var secondStart = CellPosition(second);
            yield return TileTweenRunner.MovePair(
                firstRect,
                firstStart,
                secondStart,
                secondRect,
                secondStart,
                firstStart,
                GameplayPresentationConfig.SwapDuration,
                TileTweenEase.EaseInOutCubic,
                1.05f);

            _tileViews[first] = secondRect;
            _tileViews[second] = firstRect;
            if (TrySwapCellSnapshots(first, second))
            {
                ConfigureTileInteraction(first, secondRect, _viewCells[first]);
                ConfigureTileInteraction(second, firstRect, _viewCells[second]);
            }
            else
            {
                _logger.Warn(LogCategory.UI, $"Could not update swapped cell snapshots for {first} -> {second}; final board sync will recover.");
            }

            KeepFloatingLayerOnTop();
        }

        private IEnumerator ClearPositions(IReadOnlyList<GridPosition> positions)
        {
            var targets = positions
                .Where(position => _tileViews.ContainsKey(position))
                .ToArray();
            if (targets.Length == 0)
            {
                yield break;
            }

            // White flash frame on each tile before the pop
            foreach (var position in targets)
            {
                if (_tileViews.TryGetValue(position, out var rect) && rect != null)
                {
                    var img = rect.GetComponent<Image>();
                    if (img != null)
                    {
                        img.color = UiColorPalette.ClearFlash;
                    }
                }
            }

            yield return new WaitForSecondsRealtime(0.03f);

            yield return ScaleFadeWithEasing(targets, Vector3.one, Vector3.one * 1.24f, 1f, 0f, GameplayPresentationConfig.ClearPopDuration);

            var origins = new List<Vector2>();
            foreach (var position in targets)
            {
                if (_tileViews.TryGetValue(position, out var rect) && rect != null)
                {
                    origins.Add(rect.anchoredPosition);
                }

                ReleaseTile(position);
            }

            yield return PlaySparkBursts(origins, targets.Length);
        }

        private IEnumerator ScaleFadeWithEasing(IReadOnlyList<GridPosition> positions,
            Vector3 startScale, Vector3 endScale, float startAlpha, float endAlpha, float duration)
        {
            var targets = positions
                .Where(position => _tileViews.TryGetValue(position, out var rect) && rect != null)
                .Select(position => _tileViews[position])
                .ToArray();
            yield return TileTweenRunner.ScaleFade(targets, startScale, endScale, startAlpha, endAlpha, duration, TileTweenEase.EaseOutBack);
        }

        private IEnumerator ReleaseAffectedIngredientViews(IReadOnlyList<GridPosition> positions)
        {
            var targets = positions
                .Where(position => _viewCells.TryGetValue(position, out var cell) && cell.HasIngredient && cell.AcceptsIngredient)
                .Where(position => _tileViews.ContainsKey(position))
                .ToArray();
            if (targets.Length == 0)
            {
                yield break;
            }

            yield return ScaleFade(targets, Vector3.one, Vector3.one * 1.12f, 1f, 0f, GameplayPresentationConfig.ClearPopDuration);
            foreach (var position in targets)
            {
                ReleaseTile(position);
            }
        }

        private IEnumerator CreatePotion(BoardAnimationEvent animationEvent)
        {
            foreach (var position in animationEvent.Positions)
            {
                if (!TryGetTile(position, out var rect))
                {
                    continue;
                }

                _viewCells.TryGetValue(position, out var existing);
                var updated = new BoardCellSnapshot(animationEvent.Ingredient, animationEvent.Potion, existing.Obstacle, existing.ObstacleHealth);
                UpdateTileContent(rect, position, updated);
            }

            yield return FlashPositions(animationEvent.Positions, PotionColor(animationEvent.Potion));
        }

        private IEnumerator DestroyObstacles(IReadOnlyList<GridPosition> positions)
        {
            var targets = positions
                .Where(position => _tileViews.ContainsKey(position))
                .ToArray();
            if (targets.Length == 0)
            {
                yield break;
            }

            yield return ScaleFade(targets, Vector3.one, Vector3.one * 1.18f, 1f, 0f, GameplayPresentationConfig.ClearPopDuration);
            foreach (var position in targets)
            {
                ReleaseTile(position);
            }
        }

        private IEnumerator PlayMovementBatch(IReadOnlyList<BoardAnimationEvent> batch)
        {
            var movements = new List<TileMotion>();
            var spawned = new List<TileMotion>();

            foreach (var animationEvent in batch)
            {
                if (animationEvent.Kind == BoardAnimationEventKind.TileDropped)
                {
                    if (!_tileViews.TryGetValue(animationEvent.From, out var rect) || rect == null)
                    {
                        _logger.Warn(LogCategory.Drop, $"Missing tile view for drop {animationEvent.From} -> {animationEvent.To}; final board sync will recover.");
                        continue;
                    }

                    if (!_viewCells.TryGetValue(animationEvent.From, out var cell))
                    {
                        _logger.Warn(LogCategory.Drop, $"Missing cell snapshot for drop {animationEvent.From} -> {animationEvent.To}; final board sync will recover.");
                        continue;
                    }

                    movements.Add(new TileMotion(rect, animationEvent.From, animationEvent.To, CellPosition(animationEvent.From), CellPosition(animationEvent.To), cell));
                    continue;
                }

                if (animationEvent.Kind == BoardAnimationEventKind.TileSpawned)
                {
                    var cell = new BoardCellSnapshot(animationEvent.Ingredient, PotionType.None, ObstacleType.None, 0);
                    var rect = CreateTileView(animationEvent.To, cell, register: false);
                    var end = CellPosition(animationEvent.To);
                    var rowDistance = Mathf.Max(1, animationEvent.To.Row - animationEvent.From.Row);
                    var start = end + new Vector2(0f, CellPitch * rowDistance);
                    rect.anchoredPosition = start;
                    rect.localScale = Vector3.one * 0.72f;
                    spawned.Add(new TileMotion(rect, animationEvent.From, animationEvent.To, start, end, cell));
                }
            }

            var allMotions = movements.Concat(spawned).ToArray();
            if (allMotions.Length > 0)
            {
                yield return MoveTiles(allMotions, MovementDuration(allMotions), TileTweenEase.EaseOutBounce);
            }

            ApplyMovementMappings(movements, spawned);

            foreach (var movement in spawned)
            {
                movement.Rect.localScale = Vector3.one;
            }

            KeepFloatingLayerOnTop();
        }

        private IEnumerator Shuffle(BoardAnimationEvent animationEvent)
        {
            if (animationEvent.Movements.Count == 0)
            {
                _logger.Warn(LogCategory.Board, "BoardShuffled event had no movement metadata; using fallback pulse.");
                yield return Pulse(_boardRoot, 1.04f, GameplayPresentationConfig.BoardPulseDuration);
                yield break;
            }

            var motions = new List<TileMotion>();
            foreach (var movement in animationEvent.Movements)
            {
                if (!_tileViews.TryGetValue(movement.From, out var rect) || rect == null)
                {
                    _logger.Warn(LogCategory.Board, $"Missing tile view for shuffle {movement.From} -> {movement.To}; final board sync will recover.");
                    continue;
                }

                if (!_viewCells.TryGetValue(movement.From, out var cell))
                {
                    _logger.Warn(LogCategory.Board, $"Missing cell snapshot for shuffle {movement.From} -> {movement.To}; final board sync will recover.");
                    continue;
                }

                motions.Add(new TileMotion(rect, movement.From, movement.To, CellPosition(movement.From), CellPosition(movement.To), cell));
            }

            yield return Pulse(_boardRoot, 1.025f, GameplayPresentationConfig.BoardPulseDuration * 0.5f);
            yield return MoveTiles(motions, GameplayPresentationConfig.SpawnDropDuration, TileTweenEase.EaseInOutCubic);
            ApplyMovementMappings(motions);
        }

        private RectTransform CreateTileView(GridPosition position, BoardCellSnapshot cell, bool register = true)
        {
            var button = GetTileButton();
            var rect = button.GetComponent<RectTransform>();
            button.gameObject.SetActive(true);
            button.transform.SetParent(_boardRoot, false);
            ConfigureTileRect(rect, position);
            UpdateTileContent(rect, position, cell, register);
            if (register)
            {
                _tileViews[position] = rect;
                _viewCells[position] = cell;
            }

            KeepFloatingLayerOnTop();
            return rect;
        }

        private void ConfigureTileRect(RectTransform rect, GridPosition position)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(_cellSize, _cellSize);
            rect.anchoredPosition = CellPosition(position);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }

        private void UpdateTileContent(RectTransform rect, GridPosition position, BoardCellSnapshot cell, bool registerCell = true)
        {
            ClearChildren(rect);
            foreach (var animator in rect.GetComponents<UiTileAnimator>())
            {
                UnityEngine.Object.Destroy(animator);
            }

            foreach (var group in rect.GetComponents<CanvasGroup>())
            {
                group.alpha = 1f;
            }

            var image = rect.GetComponent<Image>();
            // Use rounded-corner sprite for candy-style tiles
            image.sprite = _iconFactory.GetRoundedRectSprite(14);
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 3f;
            image.color = UiColorPalette.CellColor(cell);
            image.raycastTarget = true;

            // ── Bottom shadow gradient (3D depth) ──
            CreateDepthLayer(rect, "BottomShadow",
                new Vector2(0.02f, 0f), new Vector2(0.98f, 0.22f),
                UiColorPalette.TileBaseGradientBottom);

            // ── Inner shadow (bottom-right darkening) ──
            CreateDepthLayer(rect, "InnerShadow",
                new Vector2(0.05f, 0f), new Vector2(1f, 0.95f),
                UiColorPalette.TileInnerShadow);

            // ── Top specular highlight band (glossy candy shine) ──
            CreateDepthLayer(rect, "TopHighlight",
                new Vector2(0.08f, 0.84f), new Vector2(0.92f, 0.98f),
                UiColorPalette.TileTopHighlight);

            // ── Top gradient (lighter top edge for 3D look) ──
            CreateDepthLayer(rect, "TopGradient",
                new Vector2(0.04f, 0.90f), new Vector2(0.96f, 1f),
                UiColorPalette.TileBaseGradientTop);

            ConfigureTileInteraction(position, rect, cell);

            if (cell.Obstacle == ObstacleType.DarkTile)
            {
                // Vignette overlay for obstacle tiles
                CreateDepthLayer(rect, "ObstacleVignette",
                    new Vector2(0f, 0f), new Vector2(1f, 1f),
                    new Color(0f, 0f, 0f, 0.12f));
                CreateIconImage(rect, _iconFactory.GetObstacleSprite(ObstacleType.DarkTile), new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.94f), new Color(1f, 1f, 1f, 0.70f));
            }

            if (cell.BlocksIngredientSpace)
            {
                // Vignette for inset look
                CreateDepthLayer(rect, "ObstacleVignette",
                    new Vector2(0f, 0f), new Vector2(0.08f, 1f),
                    new Color(0f, 0f, 0f, 0.10f));
                CreateDepthLayer(rect, "ObstacleVignette2",
                    new Vector2(0.92f, 0f), new Vector2(1f, 1f),
                    new Color(0f, 0f, 0f, 0.10f));
                CreateIconImage(rect, _iconFactory.GetObstacleSprite(cell.Obstacle), new Vector2(0.10f, 0.10f), new Vector2(0.90f, 0.90f), Color.white);
                CreateAnchoredText(rect, cell.ObstacleHealth.ToString(), 22, TextAnchor.LowerRight);

                // Damage crack indicator for low-HP obstacles
                if (cell.ObstacleHealth <= 1 && cell.Obstacle == ObstacleType.StoneBlock)
                {
                    CreateDepthLayer(rect, "CrackOverlay",
                        new Vector2(0.25f, 0.15f), new Vector2(0.75f, 0.85f),
                        new Color(0f, 0f, 0f, 0.18f));
                }

                if (registerCell)
                {
                    _viewCells[position] = cell;
                }

                return;
            }

            // ── Radial glow behind the ingredient icon (soft light emission) ──
            if (cell.Ingredient != IngredientType.None)
            {
                var glowColor = UiColorPalette.IngredientColorLight(cell.Ingredient);
                // Larger, softer outer glow
                CreateDepthLayer(rect, "RadialGlowOuter",
                    new Vector2(0.12f, 0.12f), new Vector2(0.88f, 0.88f),
                    UiColorPalette.WithAlpha(glowColor, 0.10f));
                // Inner bright glow
                CreateDepthLayer(rect, "RadialGlowInner",
                    new Vector2(0.22f, 0.22f), new Vector2(0.78f, 0.78f),
                    UiColorPalette.WithAlpha(glowColor, 0.16f));
                CreateIconImage(rect, _iconFactory.GetIngredientSprite(cell.Ingredient), new Vector2(0.10f, 0.10f), new Vector2(0.90f, 0.90f), Color.white);
            }

            if (cell.Potion != PotionType.None)
            {
                // Pulsing potion glow
                var potionColor = PotionColor(cell.Potion);
                CreateDepthLayer(rect, "PotionGlowOuter",
                    new Vector2(0.42f, 0.42f), new Vector2(1.08f, 1.08f),
                    UiColorPalette.WithAlpha(potionColor, 0.26f));
                CreateDepthLayer(rect, "PotionGlowInner",
                    new Vector2(0.50f, 0.50f), new Vector2(1.02f, 1.02f),
                    UiColorPalette.WithAlpha(potionColor, 0.38f));
                CreateIconImage(rect, _iconFactory.GetPotionSprite(cell.Potion), new Vector2(0.55f, 0.55f), new Vector2(0.98f, 0.98f), Color.white);
            }

            if (registerCell)
            {
                _viewCells[position] = cell;
            }
        }

        /// <summary>Creates a visual depth layer overlay on a tile.</summary>
        private static void CreateDepthLayer(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var layerObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            layerObject.transform.SetParent(parent, false);
            var layerRect = layerObject.GetComponent<RectTransform>();
            layerRect.anchorMin = anchorMin;
            layerRect.anchorMax = anchorMax;
            layerRect.offsetMin = Vector2.zero;
            layerRect.offsetMax = Vector2.zero;
            var layerImage = layerObject.GetComponent<Image>();
            layerImage.color = color;
            layerImage.raycastTarget = false;
        }

        private void ConfigureTileInteraction(GridPosition position, RectTransform rect, BoardCellSnapshot cell)
        {
            var button = rect.GetComponent<Button>();
            var image = rect.GetComponent<Image>();
            button.targetGraphic = image;
            button.interactable = cell.CanMoveIngredient;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                _playSfx?.Invoke(GameSfxCue.Tap);
                _tilePressed?.Invoke(position);
            });
        }

        private Button GetTileButton()
        {
            if (_tileButtonPool.Count > 0)
            {
                return _tileButtonPool.Pop();
            }

            var buttonObject = new GameObject("Tile", typeof(RectTransform), typeof(Image), typeof(Button));
            return buttonObject.GetComponent<Button>();
        }

        private void ReleaseTile(GridPosition position)
        {
            if (!_tileViews.TryGetValue(position, out var rect) || rect == null)
            {
                _tileViews.Remove(position);
                _viewCells.Remove(position);
                return;
            }

            _tileViews.Remove(position);
            _viewCells.Remove(position);
            PoolTile(rect);
        }

        private void ReleaseAllTiles()
        {
            foreach (var rect in _tileViews.Values.ToArray())
            {
                if (rect != null)
                {
                    PoolTile(rect);
                }
            }

            _tileViews.Clear();
            _viewCells.Clear();
        }

        private void PoolTile(RectTransform rect)
        {
            ClearChildren(rect);
            ClearTileOutlines(rect);
            foreach (var animator in rect.GetComponents<UiTileAnimator>())
            {
                UnityEngine.Object.Destroy(animator);
            }

            foreach (var group in rect.GetComponents<CanvasGroup>())
            {
                group.alpha = 1f;
            }

            var button = rect.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            rect.localScale = Vector3.one;
            rect.gameObject.SetActive(false);
            _tileButtonPool.Push(button);
        }

        private void ApplyMovementMappings(IReadOnlyList<TileMotion> motions, IReadOnlyList<TileMotion> spawnedMotions = null)
        {
            var moving = motions ?? Array.Empty<TileMotion>();
            var spawned = spawnedMotions ?? Array.Empty<TileMotion>();
            var incomingRects = new HashSet<RectTransform>(
                moving.Concat(spawned)
                    .Where(motion => motion.Rect != null)
                    .Select(motion => motion.Rect));
            var nextViews = new Dictionary<GridPosition, RectTransform>(_tileViews);
            var nextCells = new Dictionary<GridPosition, BoardCellSnapshot>(_viewCells);
            foreach (var motion in moving)
            {
                nextViews.Remove(motion.From);
                nextCells.Remove(motion.From);
            }

            foreach (var motion in moving)
            {
                ApplyMotionMapping(nextViews, nextCells, incomingRects, motion);
            }

            foreach (var motion in spawned)
            {
                ApplyMotionMapping(nextViews, nextCells, incomingRects, motion);
            }

            _tileViews.Clear();
            _viewCells.Clear();
            foreach (var item in nextViews)
            {
                _tileViews[item.Key] = item.Value;
            }

            foreach (var item in nextCells)
            {
                _viewCells[item.Key] = item.Value;
            }

            foreach (var item in _tileViews)
            {
                item.Value.anchoredPosition = CellPosition(item.Key);
                if (_viewCells.TryGetValue(item.Key, out var cell))
                {
                    ConfigureTileInteraction(item.Key, item.Value, cell);
                }
                else
                {
                    _logger.Warn(LogCategory.UI, $"Tile view at {item.Key} had no cell snapshot after movement mapping; final board sync will recover.");
                }
            }
        }

        private void ApplyMotionMapping(
            IDictionary<GridPosition, RectTransform> nextViews,
            IDictionary<GridPosition, BoardCellSnapshot> nextCells,
            ISet<RectTransform> incomingRects,
            TileMotion motion)
        {
            if (motion.Rect == null)
            {
                return;
            }

            if (nextViews.TryGetValue(motion.To, out var existing) && existing != null && existing != motion.Rect)
            {
                _logger.Warn(LogCategory.Drop, $"Replacing an existing tile view at {motion.To} during movement mapping; final board sync will recover if this was unexpected.");
                if (!incomingRects.Contains(existing))
                {
                    PoolTile(existing);
                }
            }

            nextViews[motion.To] = motion.Rect;
            nextCells[motion.To] = motion.Cell;
        }

        private bool TrySwapCellSnapshots(GridPosition first, GridPosition second)
        {
            if (!_viewCells.TryGetValue(first, out var firstCell) || !_viewCells.TryGetValue(second, out var secondCell))
            {
                return false;
            }

            _viewCells[first] = secondCell;
            _viewCells[second] = firstCell;
            return true;
        }

        private void ApplySelection(GridPosition? selectedTile)
        {
            if (!selectedTile.HasValue || !TryGetTile(selectedTile.Value, out var rect))
            {
                return;
            }

            var outline = rect.gameObject.AddComponent<Outline>();
            outline.effectColor = UiColorPalette.SelectionGlow;
            outline.effectDistance = new Vector2(5, -5);
            _selectionOutlines.Add(outline);

            // Outer glow ring behind the selected tile (pulsing)
            var glowObject = new GameObject("SelectionGlow", typeof(RectTransform), typeof(Image));
            glowObject.transform.SetParent(rect, false);
            var glowRect = glowObject.GetComponent<RectTransform>();
            glowRect.anchorMin = new Vector2(-0.12f, -0.12f);
            glowRect.anchorMax = new Vector2(1.12f, 1.12f);
            glowRect.offsetMin = Vector2.zero;
            glowRect.offsetMax = Vector2.zero;
            glowRect.SetAsFirstSibling();
            var glowImage = glowObject.GetComponent<Image>();
            glowImage.sprite = _iconFactory.GetRoundedRectSprite(14);
            glowImage.type = Image.Type.Sliced;
            glowImage.pixelsPerUnitMultiplier = 3f;
            glowImage.color = UiColorPalette.WithAlpha(UiColorPalette.SelectionGlow, 0.30f);
            glowImage.raycastTarget = false;

            // Inner highlight glow
            var innerGlow = new GameObject("SelectionInnerGlow", typeof(RectTransform), typeof(Image));
            innerGlow.transform.SetParent(rect, false);
            var innerRect = innerGlow.GetComponent<RectTransform>();
            innerRect.anchorMin = new Vector2(0.05f, 0.05f);
            innerRect.anchorMax = new Vector2(0.95f, 0.95f);
            innerRect.offsetMin = Vector2.zero;
            innerRect.offsetMax = Vector2.zero;
            innerRect.SetAsFirstSibling();
            innerGlow.GetComponent<Image>().color = UiColorPalette.WithAlpha(UiColorPalette.SelectionGlow, 0.12f);
            innerGlow.GetComponent<Image>().raycastTarget = false;

            // Start continuous bounce pulse
            var animator = rect.gameObject.GetComponent<UiTileAnimator>();
            if (animator == null)
            {
                animator = rect.gameObject.AddComponent<UiTileAnimator>();
            }

            animator.PlayIntro(0f, UiFeedbackCue.Match);
        }

        private void ClearSelectionOutlines()
        {
            foreach (var outline in _selectionOutlines)
            {
                if (outline != null)
                {
                    UnityEngine.Object.Destroy(outline);
                }
            }

            _selectionOutlines.Clear();
        }

        private void ClearTileOutlines(RectTransform rect)
        {
            foreach (var outline in rect.GetComponents<Outline>())
            {
                UnityEngine.Object.Destroy(outline);
            }
        }

        private void ConfigureBoardFrame(int width, int height)
        {
            _width = Math.Max(1, width);
            _height = Math.Max(1, height);
            var portrait = Screen.height >= Screen.width;
            var boardSize = portrait ? BoardSizePortrait : BoardSizeLandscape;
            if (Screen.width <= 0 || Screen.height <= 0)
            {
                boardSize = BoardSizeFallback;
            }

            _boardSize = Mathf.Clamp(boardSize, MinBoardSize, MaxBoardSize);
            _boardRoot.sizeDelta = new Vector2(_boardSize, _boardSize);
            var layoutElement = _boardRoot.GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                layoutElement.preferredWidth = _boardSize;
                layoutElement.preferredHeight = _boardSize;
            }

            _spacing = _width >= 8 ? 8f : 10f;
            var inner = _boardSize - BoardPadding * 2f - _spacing * (_width - 1);
            _cellSize = Mathf.Floor(inner / _width);
        }

        private Vector2 CellPosition(GridPosition position)
        {
            var pitch = CellPitch;
            var x = -_boardSize * 0.5f + BoardPadding + _cellSize * 0.5f + position.Column * pitch;
            var y = _boardSize * 0.5f - BoardPadding - _cellSize * 0.5f - position.Row * pitch;
            return new Vector2(x, y);
        }

        private float CellPitch => _cellSize + _spacing;

        private void KeepFloatingLayerOnTop()
        {
            if (_floatingLayer != null)
            {
                _floatingLayer.SetAsLastSibling();
            }
        }

        private IEnumerator ShakePositions(IReadOnlyList<GridPosition> positions)
        {
            var targets = positions
                .Where(position => _tileViews.TryGetValue(position, out var rect) && rect != null)
                .Select(position => _tileViews[position])
                .ToArray();
            var starts = targets.ToDictionary(target => target, target => target.anchoredPosition);
            var duration = GameplayPresentationConfig.InvalidShakeDuration;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var offset = Mathf.Sin(elapsed * 120f) * Mathf.Lerp(10f, 0f, elapsed / duration);
                foreach (var target in targets)
                {
                    if (target != null)
                    {
                        target.anchoredPosition = starts[target] + new Vector2(offset, 0f);
                    }
                }

                yield return null;
            }

            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.anchoredPosition = starts[target];
                }
            }
        }

        private IEnumerator FlashPositions(IReadOnlyList<GridPosition> positions, Color color)
        {
            var targets = positions
                .Where(position => _tileViews.TryGetValue(position, out var rect) && rect != null)
                .Select(position => _tileViews[position])
                .ToArray();
            if (targets.Length == 0)
            {
                yield break;
            }

            foreach (var target in targets)
            {
                var image = target.GetComponent<Image>();
                if (image != null)
                {
                    image.color = color;
                }
            }

            yield return ScaleTiles(targets, Vector3.one, Vector3.one * 1.12f, GameplayPresentationConfig.ClearPopDuration * 0.5f);
            yield return ScaleTiles(targets, Vector3.one * 1.12f, Vector3.one, GameplayPresentationConfig.ClearPopDuration * 0.5f);

            foreach (var position in positions)
            {
                if (TryGetTile(position, out var rect) && _viewCells.TryGetValue(position, out var cell))
                {
                    rect.GetComponent<Image>().color = CellColor(cell);
                }
            }
        }

        private IEnumerator ScaleFade(IReadOnlyList<GridPosition> positions, Vector3 startScale, Vector3 endScale, float startAlpha, float endAlpha, float duration)
        {
            var targets = positions
                .Where(position => _tileViews.TryGetValue(position, out var rect) && rect != null)
                .Select(position => _tileViews[position])
                .ToArray();
            yield return ScaleFade(targets, startScale, endScale, startAlpha, endAlpha, duration);
        }

        private static IEnumerator ScaleFade(IReadOnlyList<RectTransform> targets, Vector3 startScale, Vector3 endScale, float startAlpha, float endAlpha, float duration)
        {
            var groups = targets.ToDictionary(target => target, EnsureCanvasGroup);
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Smooth(Mathf.Clamp01(elapsed / duration));
                foreach (var target in targets)
                {
                    if (target == null)
                    {
                        continue;
                    }

                    target.localScale = Vector3.LerpUnclamped(startScale, endScale, t);
                    if (groups.TryGetValue(target, out var group) && group != null)
                    {
                        group.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                    }
                }

                yield return null;
            }
        }

        private IEnumerator PlaySparkBursts(IReadOnlyList<Vector2> origins, int clearedTileCount)
        {
            if (_boardRoot == null || origins == null || origins.Count == 0)
            {
                yield break;
            }

            var positions = origins.Take(GameplayPresentationConfig.MaxSparkBurstTiles).ToArray();
            var sparksPerTile = clearedTileCount > GameplayPresentationConfig.MaxSparkBurstTiles
                ? GameplayPresentationConfig.LargeCascadeSparkCount
                : GameplayPresentationConfig.SparkCount;
            var directions = new[]
            {
                new Vector2(1f, 1f).normalized,
                new Vector2(-1f, 1f).normalized,
                new Vector2(1f, -1f).normalized,
                new Vector2(-1f, -1f).normalized,
                new Vector2(0f, 1f),
                new Vector2(0f, -1f)
            };

            var active = new List<Image>();
            var starts = new List<Vector2>();
            var activeDirections = new List<Vector2>();
            foreach (var origin in positions)
            {
                for (var index = 0; index < Mathf.Min(sparksPerTile, directions.Length); index++)
                {
                    var spark = RentVfxImage("Spark", UiColorPalette.ClearGlow, new Vector2(8f, 8f), origin);
                    active.Add(spark);
                    starts.Add(origin);
                    activeDirections.Add(directions[index]);
                }
            }

            KeepFloatingLayerOnTop();
            var elapsed = 0f;
            while (elapsed < GameplayPresentationConfig.SparkLifetime)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / GameplayPresentationConfig.SparkLifetime);
                var eased = EasingFunctions.EaseOutQuart(t);
                for (var index = 0; index < active.Count; index++)
                {
                    var spark = active[index];
                    if (spark == null)
                    {
                        continue;
                    }

                    var rect = spark.rectTransform;
                    rect.anchoredPosition = starts[index] + activeDirections[index] * GameplayPresentationConfig.SparkSpeed * eased;
                    rect.sizeDelta = Vector2.Lerp(new Vector2(8f, 8f), new Vector2(3f, 3f), eased);
                    spark.color = UiColorPalette.WithAlpha(UiColorPalette.ClearGlow, 1f - eased);
                }

                yield return null;
            }

            foreach (var image in active)
            {
                ReleaseVfxImage(image);
            }
        }

        private Image RentVfxImage(string name, Color color, Vector2 size, Vector2 anchoredPosition)
        {
            Image image;
            if (_vfxImagePool.Count > 0)
            {
                image = _vfxImagePool.Pop();
                image.gameObject.name = name;
                image.gameObject.SetActive(true);
            }
            else
            {
                var effectObject = new GameObject(name, typeof(RectTransform), typeof(Image));
                image = effectObject.GetComponent<Image>();
            }

            image.transform.SetParent(_boardRoot, false);
            image.sprite = _iconFactory.GetPillSprite();
            image.color = color;
            image.raycastTarget = false;
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            return image;
        }

        private void ReleaseVfxImage(Image image)
        {
            if (image == null)
            {
                return;
            }

            image.gameObject.SetActive(false);
            image.transform.SetParent(_boardRoot, false);
            if (_vfxImagePool.Count >= GameplayPresentationConfig.MaxActiveVfxImages)
            {
                UnityEngine.Object.Destroy(image.gameObject);
                return;
            }

            _vfxImagePool.Push(image);
        }

        private IEnumerator MoveTiles(IReadOnlyList<TileMotion> motions, float duration, TileTweenEase ease)
        {
            var tweenMotions = motions
                .Select(motion => new TileTweenMotion(motion.Rect, motion.Start, motion.End, squashOnLand: true))
                .ToArray();
            yield return TileTweenRunner.MoveMany(tweenMotions, duration, ease);
        }

        private static float MovementDuration(IReadOnlyList<TileMotion> motions)
        {
            var maxRows = motions.Count == 0 ? 1 : motions.Max(motion => Mathf.Abs(motion.To.Row - motion.From.Row));
            return Mathf.Min(
                GameplayPresentationConfig.DropMaxDuration,
                GameplayPresentationConfig.DropDuration + maxRows * GameplayPresentationConfig.DropDurationPerRow);
        }

        private static IEnumerator ScaleTiles(IReadOnlyList<RectTransform> targets, Vector3 start, Vector3 end, float duration)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Smooth(Mathf.Clamp01(elapsed / duration));
                foreach (var target in targets)
                {
                    if (target != null)
                    {
                        target.localScale = Vector3.LerpUnclamped(start, end, t);
                    }
                }

                yield return null;
            }
        }

        private IEnumerator PotionBurst(BoardAnimationEvent animationEvent)
        {
            if (_boardRoot == null)
            {
                yield break;
            }

            yield return Pulse(_boardRoot, 1.018f, GameplayPresentationConfig.PotionAnticipationDuration);
            if (animationEvent.Potion == PotionType.LineHorizontal || animationEvent.Potion == PotionType.LineVertical)
            {
                yield return Beam(animationEvent.Potion == PotionType.LineHorizontal);
                yield break;
            }

            var color = PotionColor(animationEvent.Potion);
            var burst = RentVfxImage("Potion Burst", new Color(color.r, color.g, color.b, 0.6f), Vector2.one * (animationEvent.Potion == PotionType.Bomb ? 140f : 220f), Vector2.zero);
            var rect = burst.rectTransform;
            rect.sizeDelta = Vector2.one * (animationEvent.Potion == PotionType.Bomb ? 160f : 240f);
            KeepFloatingLayerOnTop();

            yield return TileTweenRunner.Scale(rect, Vector3.one * 0.35f, Vector3.one * 1.5f, GameplayPresentationConfig.PotionBurstDuration, TileTweenEase.EaseOutBack);
            ReleaseVfxImage(burst);
        }

        private IEnumerator Beam(bool horizontal)
        {
            var beam = RentVfxImage("Line Potion Beam", new Color(0.74f, 0.94f, 1f, 0.72f), horizontal ? new Vector2(_boardRoot.rect.width, 18f) : new Vector2(18f, _boardRoot.rect.height), Vector2.zero);
            var rect = beam.rectTransform;
            KeepFloatingLayerOnTop();

            yield return TileTweenRunner.Scale(rect, horizontal ? new Vector3(0.08f, 1f, 1f) : new Vector3(1f, 0.08f, 1f), Vector3.one, GameplayPresentationConfig.BeamDuration, TileTweenEase.EaseOutCubic);
            ReleaseVfxImage(beam);
        }

        private static IEnumerator Pulse(RectTransform target, float scale, float duration)
        {
            yield return TileTweenRunner.Scale(target, Vector3.one, Vector3.one * scale, duration * 0.5f, TileTweenEase.EaseOutQuart);
            yield return TileTweenRunner.Scale(target, Vector3.one * scale, Vector3.one, duration * 0.5f, TileTweenEase.EaseOutCubic);
        }

        private static IEnumerator Scale(RectTransform target, Vector3 start, Vector3 end, float duration)
        {
            var elapsed = 0f;
            target.localScale = start;
            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Smooth(Mathf.Clamp01(elapsed / duration));
                target.localScale = Vector3.LerpUnclamped(start, end, t);
                yield return null;
            }

            if (target != null)
            {
                target.localScale = end;
            }
        }

        private static CanvasGroup EnsureCanvasGroup(RectTransform target)
        {
            var group = target.GetComponent<CanvasGroup>();
            return group != null ? group : target.gameObject.AddComponent<CanvasGroup>();
        }

        private void CreateIconImage(Transform parent, Sprite sprite, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var iconObject = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(parent, false);
            var rect = iconObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = iconObject.GetComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.preserveAspect = true;
            image.raycastTarget = false;
        }

        private Text CreateAnchoredText(Transform parent, string text, int size, TextAnchor anchor)
        {
            var labelObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(parent, false);
            var label = labelObject.GetComponent<Text>();
            label.text = text;
            label.font = _fontProvider();
            label.color = Color.white;
            label.fontSize = size;
            label.alignment = anchor;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 12;
            label.resizeTextMaxSize = size;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = new Vector2(8, 6);
            label.rectTransform.offsetMax = new Vector2(-8, -6);
            label.raycastTarget = false;
            return label;
        }

        private static void ClearChildren(Transform parent)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i).gameObject;
                child.SetActive(false);
                UnityEngine.Object.Destroy(child);
            }
        }

        private static Color CellColor(BoardCellSnapshot cell)
        {
            return UiColorPalette.CellColor(cell);
        }

        private static Color PotionColor(PotionType potion)
        {
            return UiColorPalette.PotionColor(potion);
        }

        private static float Smooth(float t)
        {
            return t * t * (3f - 2f * t);
        }

        private readonly struct TileMotion
        {
            public TileMotion(RectTransform rect, GridPosition from, GridPosition to, Vector2 start, Vector2 end, BoardCellSnapshot cell)
            {
                Rect = rect;
                From = from;
                To = to;
                Start = start;
                End = end;
                Cell = cell;
            }

            public RectTransform Rect { get; }
            public GridPosition From { get; }
            public GridPosition To { get; }
            public Vector2 Start { get; }
            public Vector2 End { get; }
            public BoardCellSnapshot Cell { get; }
        }
    }
}
