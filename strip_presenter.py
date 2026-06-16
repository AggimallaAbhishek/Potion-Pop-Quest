import sys

file_path = "Assets/Scripts/Unity/Runtime/BoardVisualPresenter.cs"

with open(file_path, "r") as f:
    lines = f.readlines()

def find_bounds(start_str):
    start = -1
    for i, line in enumerate(lines):
        if start_str in line:
            start = i
            break
    if start == -1: return None
    
    end = -1
    bracket_count = 0
    found_first = False
    for i in range(start, len(lines)):
        if "{" in lines[i]:
            bracket_count += lines[i].count("{")
            found_first = True
        if "}" in lines[i]:
            bracket_count -= lines[i].count("}")
        if found_first and bracket_count == 0:
            end = i
            break
    return (start, end)

bounds = []
for s in ["private void ConfigureTileInteraction", "private Button _tilePool.GetTileButton", "private void _tilePool.PoolTile", "private Image _tilePool.RentVfxImage", "private void _tilePool.ReleaseVfxImage", "private void PoolTile", "private Image RentVfxImage", "private void ReleaseVfxImage"]:
    b = find_bounds(s)
    if b: bounds.append(b)

bounds.sort(key=lambda x: x[0], reverse=True)

for start, end in bounds:
    del lines[start:end+1]

with open(file_path, "w") as f:
    f.writelines(lines)
