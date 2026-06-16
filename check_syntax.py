import os
import glob

def check_braces(file_path):
    with open(file_path, "r") as f:
        content = f.read()
    
    stack = []
    lines = content.split('\n')
    for i, line in enumerate(lines):
        for char in line:
            if char == '{':
                stack.append(('{', i+1))
            elif char == '}':
                if not stack or stack[-1][0] != '{':
                    print(f"Error in {file_path}: Mismatched closing brace at line {i+1}")
                    return False
                stack.pop()
    
    if stack:
        print(f"Error in {file_path}: Unclosed opening brace(s) at line(s) {[item[1] for item in stack]}")
        return False
    
    return True

files = glob.glob("Assets/Scripts/Unity/Runtime/*.cs")
for file in files:
    check_braces(file)
print("Check complete.")
