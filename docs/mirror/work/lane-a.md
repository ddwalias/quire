# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Members: `IO.PackageOperationsProvider.CreateNewImagePart`, `MouseEventGenerator.NormalizeCoordinates(Point, out int, out int)`, `MouseEventGenerator.GenerateMouseClick(Point, int)`, `MouseEventGenerator.SendMouseInput(int, int, SendMouseInputFlags)`, `PersistentData.ProtectableTypesRegistry.GetXmlSerializer(string, bool)`, `PersistentData.ProtectableTypesRegistry.Register(string, Type)`, `PersistentData.ProtectableTypesRegistry..cctor`, `PersistentData.AreaType.GetXmlSerializerStatic(bool)`, `PersistentData.OleUtils.IsMicrosoftEquationGuid(Guid)`, `PersistentData.OleUtils.IsSupportedExcelGuid(Guid)`, `PersistentData.OleUtils.GetExcelSheetGuid(string)`, `Properties.SettingsHelper.GetMathcadPrimeLicenseConfigValues(string[])` landed.
- Release IL report: `.mirror-cache/release-il-79fea1e1d9ab/release-il.json` — PASS, 2,999 exact, 548 unfinished; no mismatches.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
