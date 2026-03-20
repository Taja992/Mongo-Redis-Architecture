#!/usr/bin/env bash
# rename-project.sh — rename this template to a new project name
#
# Usage:   bash rename-project.sh <NewProjectName>
# Example: bash rename-project.sh MyAwesomeApp
#
# Replaces all occurrences of "Template"/"template" across file contents,
# then renames files and folders to match.

set -euo pipefail

if [ -z "${1:-}" ]; then
  echo "Usage: bash rename-project.sh <NewProjectName>"
  echo "Example: bash rename-project.sh MyAwesomeApp"
  exit 1
fi

# Normalize input: capitalize first letter, derive lowercase variant
NEW="${1^}"
NEW_LOWER="${NEW,,}"
OLD="Template"
OLD_LOWER="template"

if [ "$NEW" = "$OLD" ]; then
  echo "New name is the same as the old name. Nothing to do."
  exit 0
fi

# Directories/files to skip during processing
SKIP="(\.git|/bin/|/obj/|/node_modules/|/\.next/|rename-project\.sh)"

echo "Renaming: '$OLD' → '$NEW'  |  '$OLD_LOWER' → '$NEW_LOWER'"
echo ""

# ── Step 1: Replace text content in all text files ──────────────────────────
echo "Step 1: Replacing text in files..."

grep -rIl -E "($OLD|$OLD_LOWER)" . \
  | grep -Ev "$SKIP" \
  | while IFS= read -r file; do
      sed -i "s/$OLD/$NEW/g; s/$OLD_LOWER/$NEW_LOWER/g" "$file"
      echo "  updated: $file"
    done

# ── Step 2: Rename files and directories (depth-first avoids path collisions)
echo ""
echo "Step 2: Renaming files and directories..."

find . -depth \( -name "*${OLD}*" -o -name "*${OLD_LOWER}*" \) \
  | grep -Ev "$SKIP" \
  | while IFS= read -r path; do
      base="$(basename "$path")"
      dir="$(dirname "$path")"
      new_base="${base//$OLD/$NEW}"
      new_base="${new_base//$OLD_LOWER/$NEW_LOWER}"
      if [ "$base" != "$new_base" ]; then
        mv "$path" "$dir/$new_base"
        echo "  renamed: $path  →  $dir/$new_base"
      fi
    done

echo ""
echo "Done. Project renamed to '$NEW'."
echo ""
echo "Next steps:"
echo "  dotnet build                         # verify backend compiles"
echo "  cd web && pnpm install && pnpm build # verify frontend compiles"
echo "  Update README.md with your project description"
