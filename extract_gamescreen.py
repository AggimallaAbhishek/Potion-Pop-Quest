import sys

file_path = "Assets/Scripts/Unity/Runtime/GeneratedGameUi.cs"
out_path = "Assets/Scripts/Unity/Runtime/GeneratedGameUi.GameScreen.cs"

start_line = 240
end_line = 838

with open(file_path, "r") as f:
    lines = f.readlines()

extracted = lines[start_line-1:end_line]

out_lines = [
    "using System;\n",
    "using System.Collections;\n",
    "using System.Collections.Generic;\n",
    "using System.Linq;\n",
    "using PotionPopQuest.Core;\n",
    "using TMPro;\n",
    "using UnityEngine;\n",
    "using UnityEngine.UI;\n",
    "\n",
    "namespace PotionPopQuest.Unity\n",
    "{\n",
    "    public sealed partial class GeneratedGameUi\n",
    "    {\n"
]
out_lines.extend(extracted)
out_lines.append("    }\n")
out_lines.append("}\n")

with open(out_path, "w") as f:
    f.writelines(out_lines)

del lines[start_line-1:end_line]

with open(file_path, "w") as f:
    f.writelines(lines)
