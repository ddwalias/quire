# Lane Quire.Controls.Worksheet, half A

- Owner: Claude worker (task_b5ca4c6fcc1d)
- Branch: `mirror/worksheet-a-haiku-20261119` (from `main` 457672a0c); worktree `C:/Users/DuyDang/orca/workspaces/Quire/worksheet-a`
- Half A: Generated stub files up to and including `Controls.Worksheet.RtfItemModel.g.cs`, plus `Controls.Core.RegionCollectionWithProperty.g.cs` (27 files, 446 stub attributes at start).

## Landed

- None. No batch passed the Release IL gate.

## Parked (one focused attempt each)

- `*PersistentData.Accept` override (`ChartContainer`, `Container`, `ExcelContainer`, `ImageRegion`, `IncludeRegion`, `MathRegion`, `OleObject`, `PlotRegion`, `RichTextBase`): `visitor as IVisitorFuncOfRegionPersistentData ?? throw ...` gives the Mathcad IL shape except for one extra `pop` on the null path that Roslyn emits and Mathcad does not. Gate: `first difference at #3 (Quire has 14 entries, Mathcad 13)`. Stubs kept.
- `CollapsibleAreaHeightConverter` (`Convert`, `ConvertBack`, constructor): the body needs `Quire.Controls.CollapsibleArea.CollapsibleAreaControl` (`isinst`). `Quire.Controls.Worksheet.csproj` does not reference `Quire.Controls.CollapsibleArea.csproj`. Adding that reference is a csproj change outside this lane. Blocked.
- `ControlFactory.CreateCollapsibleAreaControl`: constructs `CollapsibleAreaControl`, same missing reference. Blocked.
- `CombinedInvalidConverter.Convert`: parked in batch 1 (see `lane-worksheet-haiku.md`).
- `PageNumberControl` (`GetRegionPersistentData`, `IsValidTemplateKey`, constructor, `_typeOfThis`): uses compiler-generated display classes (`<>c__DisplayClass10_0`, `<>c__DisplayClass17_0`) and a static constructor with a `HandleTagChanged` method group. Needs closure ordinals; not attempted further.
- `DirectRenderingAlignmentGridBorder`: 11 static dependency properties, pens and `BuildTileDrawingGeometry`. Static initializer shape not checked; not attempted.
- `RtfItemModel`, `MultiWorksheetItemCopyVisitor`, `SaveAsRtfWorksheetItemsVisitor`, `MultiWorksheetItemRtfCopyVisitorBase`, `RtfItemWrapperHelper` users: blocked by NRtfTree licence (D7). Parked.

## State

- Release IL: not run for a passing batch. The last two gate runs (Accept batch, converter batch) both failed as described above, and both were reverted.
- Remaining stubs in half A: 446 (unchanged).
