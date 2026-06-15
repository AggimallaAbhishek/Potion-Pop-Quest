using System.Collections.Generic;
using System.IO;
using System.Linq;
using PotionPopQuest.Core;
using PotionPopQuest.Unity;
using UnityEditor;
using UnityEngine;

namespace PotionPopQuest.Unity.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        private const string JsonPath = "Assets/Resources/Levels/mvp_levels.json";

        private LevelCatalogJson _catalog;
        private int _selectedLevelIndex = 0;
        private Vector2 _scrollPos;

        private ObstacleType _paintObstacle = ObstacleType.None;

        [MenuItem("Potion Pop Quest/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditorWindow>("Level Editor");
        }

        private void OnEnable()
        {
            LoadCatalog();
        }

        private void LoadCatalog()
        {
            if (File.Exists(JsonPath))
            {
                string json = File.ReadAllText(JsonPath);
                _catalog = JsonUtility.FromJson<LevelCatalogJson>(json);
                if (_catalog.levels == null) _catalog.levels = new LevelJson[0];
            }
            else
            {
                _catalog = new LevelCatalogJson { levels = new LevelJson[0] };
            }
        }

        private void SaveCatalog()
        {
            if (_catalog != null)
            {
                string json = JsonUtility.ToJson(_catalog, true);
                File.WriteAllText(JsonPath, json);
                AssetDatabase.Refresh();
                Debug.Log("Saved mvp_levels.json");
            }
        }

        private void OnGUI()
        {
            if (_catalog == null || _catalog.levels == null)
            {
                GUILayout.Label("Loading...");
                return;
            }

            EditorGUILayout.BeginHorizontal();
            
            // LEFT PANEL: Level Selector & Data
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            DrawLeftPanel();
            EditorGUILayout.EndVertical();

            // RIGHT PANEL: Grid & Palette
            EditorGUILayout.BeginVertical();
            DrawRightPanel();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawLeftPanel()
        {
            GUILayout.Label("Level Editor", EditorStyles.boldLabel);

            if (GUILayout.Button("Load Data")) LoadCatalog();
            if (GUILayout.Button("Save Data", GUILayout.Height(30))) SaveCatalog();

            EditorGUILayout.Space();

            if (_catalog.levels.Length == 0)
            {
                if (GUILayout.Button("Create First Level"))
                {
                    _catalog.levels = new[] { CreateNewLevel(1) };
                }
                return;
            }

            var levelNames = _catalog.levels.Select(l => l.displayName ?? $"Level {l.levelNumber}").ToArray();
            _selectedLevelIndex = EditorGUILayout.Popup("Select Level", _selectedLevelIndex, levelNames);

            if (GUILayout.Button("Add New Level"))
            {
                var list = _catalog.levels.ToList();
                int nextNum = list.Count > 0 ? list.Max(l => l.levelNumber) + 1 : 1;
                list.Add(CreateNewLevel(nextNum));
                _catalog.levels = list.ToArray();
                _selectedLevelIndex = list.Count - 1;
            }

            EditorGUILayout.Space();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            var level = _catalog.levels[_selectedLevelIndex];

            level.levelNumber = EditorGUILayout.IntField("Level Number", level.levelNumber);
            level.displayName = EditorGUILayout.TextField("Display Name", level.displayName);
            level.gridWidth = EditorGUILayout.IntField("Grid Width", level.gridWidth);
            level.gridHeight = EditorGUILayout.IntField("Grid Height", level.gridHeight);
            level.moves = EditorGUILayout.IntField("Moves", level.moves);
            level.tutorialLevel = EditorGUILayout.Toggle("Tutorial Level", level.tutorialLevel);

            EditorGUILayout.Space();
            GUILayout.Label("Star Thresholds", EditorStyles.boldLabel);
            if (level.starThresholds == null) level.starThresholds = new StarThresholdJson();
            level.starThresholds.oneStar = EditorGUILayout.IntField("One Star", level.starThresholds.oneStar);
            level.starThresholds.twoStars = EditorGUILayout.IntField("Two Stars", level.starThresholds.twoStars);
            level.starThresholds.threeStars = EditorGUILayout.IntField("Three Stars", level.starThresholds.threeStars);

            EditorGUILayout.Space();
            DrawActiveIngredients(level);

            EditorGUILayout.Space();
            DrawGoals(level);

            EditorGUILayout.EndScrollView();
            
            if (GUILayout.Button("Delete Current Level", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Delete Level", "Are you sure you want to delete this level?", "Yes", "No"))
                {
                    var list = _catalog.levels.ToList();
                    list.RemoveAt(_selectedLevelIndex);
                    _catalog.levels = list.ToArray();
                    _selectedLevelIndex = Mathf.Clamp(_selectedLevelIndex - 1, 0, _catalog.levels.Length - 1);
                }
            }
        }

        private void DrawActiveIngredients(LevelJson level)
        {
            GUILayout.Label("Active Ingredients", EditorStyles.boldLabel);
            if (level.activeIngredients == null) level.activeIngredients = new string[0];
            
            var list = level.activeIngredients.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                list[i] = EditorGUILayout.TextField(list[i]);
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    list.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Ingredient"))
            {
                list.Add(IngredientType.RedHerb.ToString());
            }
            level.activeIngredients = list.ToArray();
        }

        private void DrawGoals(LevelJson level)
        {
            GUILayout.Label("Goals", EditorStyles.boldLabel);
            if (level.goals == null) level.goals = new GoalJson[0];

            var list = level.goals.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                list[i].goalType = EditorGUILayout.TextField("Goal Type", list[i].goalType);
                list[i].goalItem = EditorGUILayout.TextField("Item", list[i].goalItem);
                list[i].goalAmount = EditorGUILayout.IntField("Amount", list[i].goalAmount);
                if (GUILayout.Button("Remove Goal"))
                {
                    list.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add Goal"))
            {
                list.Add(new GoalJson { goalType = GoalType.CollectIngredient.ToString(), goalItem = IngredientType.RedHerb.ToString(), goalAmount = 10 });
            }
            level.goals = list.ToArray();
        }

        private void DrawRightPanel()
        {
            if (_catalog.levels.Length == 0) return;
            var level = _catalog.levels[_selectedLevelIndex];

            GUILayout.Label("Obstacle Palette", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            _paintObstacle = (ObstacleType)EditorGUILayout.EnumPopup("Paint:", _paintObstacle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GUILayout.Label("Grid (8x8)", EditorStyles.boldLabel);

            if (level.obstacles == null) level.obstacles = new ObstacleJson[0];

            var obstaclesMap = new Dictionary<Vector2Int, ObstacleJson>();
            foreach (var obs in level.obstacles)
            {
                obstaclesMap[new Vector2Int(obs.row, obs.column)] = obs;
            }

            float cellSize = 40f;
            for (int r = 0; r < 8; r++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int c = 0; c < 8; c++)
                {
                    var pos = new Vector2Int(r, c);
                    obstaclesMap.TryGetValue(pos, out var currentObs);

                    string btnText = ".";
                    Color defaultColor = GUI.color;
                    
                    if (currentObs != null)
                    {
                        if (currentObs.type == ObstacleType.WoodenBox.ToString())
                        {
                            btnText = "W";
                            GUI.color = Color.yellow;
                        }
                        else if (currentObs.type == ObstacleType.DarkTile.ToString())
                        {
                            btnText = "D";
                            GUI.color = Color.magenta;
                        }
                        else if (currentObs.type == ObstacleType.StoneBlock.ToString())
                        {
                            btnText = "S";
                            GUI.color = Color.gray;
                        }
                        else
                        {
                            btnText = "O";
                        }
                    }

                    if (GUILayout.Button(btnText, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                    {
                        if (_paintObstacle == ObstacleType.None)
                        {
                            obstaclesMap.Remove(pos);
                        }
                        else
                        {
                            obstaclesMap[pos] = new ObstacleJson
                            {
                                row = r,
                                column = c,
                                type = _paintObstacle.ToString(),
                                healthOverride = 0
                            };
                        }
                        
                        level.obstacles = obstaclesMap.Values.ToArray();
                    }
                    GUI.color = defaultColor;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space();
            GUILayout.Label("Legend:\n . = Empty\n W = Wooden Box\n D = Dark Tile\n S = Stone Block");
        }

        private LevelJson CreateNewLevel(int number)
        {
            return new LevelJson
            {
                levelNumber = number,
                displayName = $"Level {number}",
                gridWidth = 8,
                gridHeight = 8,
                moves = 15,
                tutorialLevel = false,
                activeIngredients = new[] { "RedHerb", "BlueCrystal", "GreenLeaf", "YellowStarDust", "PurpleMushroom" },
                starThresholds = new StarThresholdJson { oneStar = 5000, twoStars = 10000, threeStars = 15000 },
                goals = new[] { new GoalJson { goalType = GoalType.CollectIngredient.ToString(), goalItem = IngredientType.RedHerb.ToString(), goalAmount = 10 } },
                obstacles = new ObstacleJson[0]
            };
        }
    }
}
