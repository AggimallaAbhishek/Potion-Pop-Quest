import sys

file_path = "Assets/Scripts/Unity/Runtime/GeneratedGameUi.cs"

ranges_to_delete = [
    (725, 750), # BuildMainMenu
    (399, 419), # ShowSettings
    (301, 397), # CreateLevelCardPrefab
    (243, 299), # ShowLevelSelect
    (236, 241)  # ShowMainMenu
]

with open(file_path, "r") as f:
    lines = f.readlines()

for start, end in ranges_to_delete:
    del lines[start-1:end]

with open(file_path, "w") as f:
    f.writelines(lines)
