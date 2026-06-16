import sys

file_path = "Assets/Scripts/Unity/Runtime/GeneratedGameUi.cs"

ranges_to_delete = [
    (1526, 1530),
    (1520, 1524),
    (1500, 1518),
    (1478, 1498),
    (1220, 1293),
    (1204, 1218),
    (1182, 1202),
    (1170, 1180),
    (1133, 1166),
    (1116, 1131),
    (1101, 1114),
    (1048, 1068),
    (1011, 1046),
    (233, 250)
]

with open(file_path, "r") as f:
    lines = f.readlines()

for start, end in ranges_to_delete:
    del lines[start-1:end]

with open(file_path, "w") as f:
    f.writelines(lines)
