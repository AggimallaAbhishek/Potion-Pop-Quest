import sys

file_path = "Assets/Scripts/Unity/Runtime/BoardVisualPresenter.cs"

with open(file_path, "r") as f:
    content = f.read()

# 1. Replace fields
content = content.replace(
    "private readonly Stack<Button> _tileButtonPool = new Stack<Button>();\n        private readonly Stack<Image> _vfxImagePool = new Stack<Image>();",
    "private readonly BoardTilePool _tilePool;\n        private readonly BoardInputHandler _inputHandler;"
)

content = content.replace(
    "private Action<GridPosition> _tilePressed;\n        private Action<GameSfxCue> _playSfx;",
    ""
)

# 2. Constructor
content = content.replace(
    "_fontProvider = fontProvider ?? (() => null);\n        }",
    "_fontProvider = fontProvider ?? (() => null);\n            _tilePool = new BoardTilePool(_iconFactory);\n            _inputHandler = new BoardInputHandler();\n        }"
)

# 3. Configure
content = content.replace(
    "_tilePressed = tilePressed;\n            _playSfx = playSfx;",
    "_inputHandler.Configure(tilePressed, playSfx);"
)

# 4. GetTileButton -> _tilePool.GetTileButton()
content = content.replace("GetTileButton()", "_tilePool.GetTileButton()")

# 5. PoolTile -> _tilePool.PoolTile
content = content.replace("PoolTile(rect)", "_tilePool.PoolTile(rect)")
content = content.replace("PoolTile(existing)", "_tilePool.PoolTile(existing)")

# 6. RentVfxImage
content = content.replace("RentVfxImage(name, color, size, anchoredPosition)", "_tilePool.RentVfxImage(name, color, size, anchoredPosition, _boardRoot)")

# 7. ReleaseVfxImage
content = content.replace("ReleaseVfxImage(image)", "_tilePool.ReleaseVfxImage(image, _boardRoot)")

# 8. ConfigureTileInteraction -> _inputHandler.ConfigureTileInteraction
content = content.replace("ConfigureTileInteraction(position, rect, cell)", "_inputHandler.ConfigureTileInteraction(position, rect, cell)")


with open(file_path, "w") as f:
    f.write(content)
