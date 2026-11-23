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

## Probe — UpdateShowNumbers closure ordinal (parked; no source change)

- Mathcad: `<UpdateShowNumbers>b__67_0` (members.tsv `+<>c` row 77; `UpdateShowNumbers` is method row 40, token 0x06000031).
- Quire, current main: `<UpdateShowNumbers>b__97_0` (the stub sits in `Generated/`, which compiles before `Mirrored/`).
- To reach 67, exactly 66 methods must precede it in compile order. That needs a compile-first hand-written file declaring the Mathcad members before it as placeholders, in Mathcad order, and removing their generated stubs. This is a broad reorder of the class, so the group is parked per the task rule.
- Still open, all lambda-bearing: UpdateShowNumbers/Ticks/AxisExpressions, GetNumberFormatsForRegions, GetProofingLanguageForRegions, GetMatrixFormatsForRegions, GetCalcIsDisabled, GetOleObjectAutoResizeState, GetPlotCameraType, SetTextFormatPropertyValueWithCommand, HandleCollectionChanged.

## Batch 7 — DirectRenderingAlignmentGridBorder static DependencyProperty initializer

- `TypeOfThis`, the ten static DependencyProperty fields and the static constructor moved to `Mirrored/` (stubs removed from `.g.cs`). No lambdas; callbacks are method groups. `GridCellSizeProperty` uses `AlignmentGrid.GridCellSizeProperty.AddOwner`.
- Release IL: PASS (1276 checked, 0 failures).
- Left as stubs: `BuildTileDrawingGeometry` (contains a `UpTo` lambda; closure-ordinal blocker as for RegionCollectionWithProperty), and the instance pen fields.

## Batch 8 — HeaderFooterZombie IsAssociatedWithPage and static PageNumber property

- `_typeOfThis` and `PageNumberProperty` moved to `Mirrored/` as field initializers (stubs removed). `IsAssociatedWithPage` implemented (no closures). Release IL: PASS (1278 checked, 0 failures).
- Left as XAML-generated stubs: `InitializeComponent`, `IComponentConnector.Connect`, `_contentLoaded`.
- Survey, closure-free types checked: `RegionFactory` (large serialization chain, not attempted), `RegionPersistentData` (Accept parked), `WhiteboardItemZombie` (constructor and `HandleZombieLoaded` use lambdas).

## Batch 9 — RegionFactory.CreateFailedRegionPersistentData, CreateTemplatedTextBoxTypedPersistentData

- Both members implemented in `Mirrored/` (stubs removed). The failed-region image URI keeps Mathcad's `Ptc.Controls.Core` resource path, because the IL compares the string constant (the mirrored `FlowDocumentFamilyConverter` does the same). The enum dispatch is a `switch` statement; the switch expression differed at the first local (`stloc.1` vs Mathcad `stloc.0`).
- Release IL: PASS (1280 checked, 0 failures).
- Left as stubs: `ConvertToIRegionType`, `ConvertFromIRegionType`, `CreatePlotTypedPersistentData` and the rest of `RegionFactory` (not attempted in this batch).

## Batch 10 — RegionFactory.CreatePlotTypedPersistentData

- Implemented in `Mirrored/` as the `is` chain (no closures; stub removed). Release IL: PASS (1281 checked, 0 failures).
- Left as stubs: `ConvertToIRegionType`, `ConvertFromIRegionType` (not attempted in this batch).

## Parked — RegionFactory.ConvertToIRegionType (code reverted; stub kept)

- Release IL first difference after the decompiled-shape edits: `first difference at #2 (Quire has 137 entries, Mathcad 139): Quire brfalse 126, Mathcad brfalse 128`. Type operations match after these edits: `castclass IPersistentDataProvider` before `GetRegionPersistentData`, `isinst IHasPreSerialize` twice, no `DependencyObject` castclass before `GetBookmarkTag` or `GetIsRegionBorderVisible`; `string.Format` with `((object)control).GetType()` for the exception.
- Edits that were needed: `conversionAnnotations != null ? conversionAnnotations.ToStrings() : null` (the `?.` form emitted `call` where Mathcad emits `callvirt`); `((IPersistentDataProvider)control).GetRegionPersistentData(...)` (pattern variable differed); `(regionPersistentData as IHasPreSerialize).PreSerialize()`.
- Unresolved: Mathcad's body is 2 instructions longer before the throw block; not located. Next step: diff the full sequence with `MethodAudit --diff` for `RegionFactory::ConvertToIRegionType`.
- `ConvertFromIRegionType`: not attempted (larger chain of type tests).

## Batch 11 — RegionFactory.ConvertFromIRegionType

- Implemented in `Mirrored/` (stub removed from `.g.cs`). The shape is the `if`/`else if` `is` chain in Mathcad's order, the `ProtectedItemType` unwrap, and the `ISupportCustomInitialization`, `ISolveBlockControl` and `PlotControlBase` post-checks. The `istextblock` ternary picks `Paragraph` or `TextRegion`.
- Build fix: `Quire.Controls.Include` (`IncludeWorksheetControl`) is reached through the generated `Mirror.props` closure, so only a `using` was needed. The added `System.Windows.Documents` using made `Paragraph` ambiguous, so it was removed and the plain name resolves to `Quire.Controls.Text.Paragraph`.
- Release IL: PASS (1282 checked, 1282 exact, 0 failures).
- Left as stubs: `ConvertToIRegionType` (parked, batch 10 note).
