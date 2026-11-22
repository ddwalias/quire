# Lane Quire.Controls.Worksheet (haiku-worksheet-20261123)

- Owner: Claude worker (task_9a74b07c793c)
- Branch: `feature/haiku-worksheet-20261123` (worktree `C:/Users/DuyDang/orca/workspaces/Quire/haiku-worksheet-20261123`), from `main` be8d3e6b6

## Batch 1 — CombinedInvalidConverter (Convert)

- Members: `CombinedInvalidConverter.Convert` (stub removed from `.g.cs`, implemented in `Mirrored/`). `ConvertBack` was already hand-written.
- Shape: `values[i] is bool f && f` pattern tests, combined with bool `&`. Release IL: PASS (gate, `--type Quire.Controls.Worksheet.CombinedInvalidConverter`).
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: see the batch receipt.

## Batch 2 — WorksheetControlExtensions.SeparateOverlappedItems

- Member: `SeparateOverlappedItems(this WorksheetControl, Dimension)` (stub removed from `.g.cs`, implemented in `Mirrored/`). Shape: `(IInputElement)(ActiveItem as IMathboardControl) ?? (IsContentActive ? control : ...)`; the `if`/`else` form differed at the `dup` of the null test. Release IL: PASS (1264 checked, 0 failures).

## Parked — Accept overrides (`visitor as IVisitorFuncOfRegionPersistentData ?? throw ...`)

- `WorksheetPageBreakPersistentData`, `TemplatedTextBoxPersistentData`, `SpecTableRegionPersistentData` (and the parked ones in the earlier checkpoint): Mathcad emits `isinst; dup; brtrue.s` with **no** `pop` on the null path; Roslyn emits `pop` for `?? throw`, and the `is`-pattern and switch-expression forms use a local. First difference at #3 (Quire 14 entries, Mathcad 13). Not reshaped further; stubs kept. Shared across all Accept overrides in the assembly.

## Batch 3 — CollapsibleAreaHeightConverter (Convert, ConvertBack, constructor) + ProjectReference

- `Quire.Controls.Worksheet.csproj` gains a direct `ProjectReference` to `Quire.Controls.CollapsibleArea` (hand-written csproj, not `Generated/Mirror.props`). Evaluated closure of CollapsibleArea (Mirror, Calculation.Core, Controls.Core, Controls.Whiteboard, Core) does not reach Worksheet: acyclic.
- Members: `CollapsibleAreaHeightConverter` constructor, `Convert` and `ConvertBack` (stubs removed from `.g.cs`, implemented in `Mirrored/`). Shape: `worksheetPageLayout.GridCellSize.Height` read directly; a named `Size` local differed (Mathcad stores the rvalue struct in a temp).
- Release IL: PASS (1266 checked, 0 failures).
