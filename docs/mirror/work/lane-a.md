# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Members: `IO.PackageOperationsProvider.CreateNewImagePart`, `MouseEventGenerator.NormalizeCoordinates(Point, out int, out int)`, `MouseEventGenerator.GenerateMouseClick(Point, int)`, `MouseEventGenerator.SendMouseInput(int, int, SendMouseInputFlags)`, `PersistentData.ProtectableTypesRegistry.GetXmlSerializer(string, bool)` landed.
- Release IL report: `.mirror-cache/release-il-08bacfa0574e/release-il.json` — PASS, 2,987 exact, 554 unfinished; no mismatches.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
