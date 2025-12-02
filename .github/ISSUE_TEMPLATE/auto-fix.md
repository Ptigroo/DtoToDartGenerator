---
name: Auto-Fix Request
about: Request an automated fix (title must start with "auto")
title: "auto: "
labels: auto-fix
assignees: ''
---

## Description

<!-- Describe what you want to be automatically fixed or implemented -->

## Expected Behavior

<!-- What should happen after the fix is applied? -->

## Current Behavior (if applicable)

<!-- What is currently happening? -->

## Additional Context

<!-- Any additional information, code snippets, or references that would help -->

---

**Note:** Issues with titles starting with "auto" (case-insensitive) will automatically trigger a workflow that:
1. Creates a working branch
2. Scaffolds a plan file
3. Opens a draft pull request

The draft PR can then receive AI-authored or manual commits to implement the actual fix.
