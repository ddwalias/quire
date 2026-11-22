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

## Batch 4 — ControlFactory.CreateCollapsibleAreaControl; RegionCollectionWithProperty text-format handlers

- `ControlFactory.CreateCollapsibleAreaControl`: `new CollapsibleAreaControl(gridCellSize) { RegionFactory = RegionFactory.Instance }`, now possible with the CollapsibleArea reference.
- `RegionCollectionWithProperty`: the static `HandleTextFormatChanged(DependencyObject, DependencyPropertyChangedEventArgs)` and the instance `HandleTextFormatChanged(TextFormat, TextFormat)`. Both have no closures.
- Release IL: PASS (1269 checked, 0 failures).
- Left for later: the static constructor and `DependencyProperty` fields of RegionCollectionWithProperty (they reference `TextFormat.TextFormatProperty.AddOwner` and the `PlotCommonProperties`/`AttachedProperties` owners), the constructor, and `OnTextFormattingChanged` (`using (new CommandScope)` with pattern loops, not attempted).

## Batch 5 — RegionCollectionWithProperty initialization, constructor, OnTextFormattingChanged

- Static `DependencyProperty` fields and `_typeOfThis` declared in `Mirrored/` (stubs removed from `.g.cs`), with the static constructor (`AddOwner` on `TextFormat`, `AttachedProperties` and `PlotCommonProperties` owners).
- Constructor: `_regions` from `DebugTools.AssertNotNull(...).Passed` ternary, `RangeChanged` hook, initial `HandleCollectionChanged`. The instance field `_currentProofingLanguage = XmlLanguage.Empty` initializer is required by Mathcad's constructor IL (first difference was `XmlLanguage.get_Empty`).
- `OnTextFormattingChanged`: `EnvironmentScope` guard, `using (new CommandScope(...))`, pattern `foreach` with `continue`. Exact as written.
- Release IL: PASS (1272 checked, 0 failures).
- Left: `GetTextFormat`, `GetCalcIsDisabled` and the other `Get*ForRegions` helpers, `SetTextFormatPropertyValueWithCommand`, `UpdateShowNumbers`/`UpdateShowTicks` and the `HandleCollectionChanged` body (not attempted in this batch).

## Batch 6 — RegionCollectionWithProperty GetTextFormat, GetPlotBackgroundType, GetPlotsCommonState

- Release IL: PASS (1275 checked, 0 failures).
- `GetTextFormat`: the decompiled `region as X` / `is X` shape matched; the pattern-variable form (`is IRichTextBase richText`) differed at the first type test.
- Parked, lambda-bearing helpers (reverted to stubs): `UpdateShowNumbers`, `UpdateShowTicks`, `UpdateShowAxisExpressions`, `GetNumberFormatsForRegions`, `GetProofingLanguageForRegions`, `GetMatrixFormatsForRegions`, `GetCalcIsDisabled`, `GetOleObjectAutoResizeState`. Shared blocker: the Quire lambda names are `<UpdateShowNumbers>b__97_0` and `<GetCalcIsDisabled>b__...` with method ordinals about 30 higher than Mathcad's (`<UpdateShowNumbers>b__67_0`, `<GetCalcIsDisabled>b__77_0`), so the Mathcad closure names (`<>c` members and `<>c__DisplayClass105_0`) have no counterpart. The Generated stubs for members Mathcad places earlier are compiled before the Mirrored partial. A placeholder layout in Mathcad order (AGENTS: unmarked placeholder stubs) is needed before these can land.
