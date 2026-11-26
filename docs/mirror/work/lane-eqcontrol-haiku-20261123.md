# Lane: Quire.Controls.EquationControl (eqcontrol-haiku-20261123)

- Owner: dispatched EquationControl worker (Claude, claude-haiku-5-5), task_e6f53e959f67
- Branch: `feature/haiku-equationcontrol-20261123` (worktree from main a046f0a59)
- Scope: `Quire.Controls.EquationControl` only; one type per batch, hand-written partials in `Mirrored/`.
- Reference: `PYTHONUTF8=1 python tools/reference/reference.py Ptc.<Type> --assembly Ptc.Controls.EquationControl --il`

## Batches

### Batch 1: Controls.Internal.VisualSymbolNode (leaf members)

- Members exact: `TextBaselineY`, `GlyphGap`, `EffectiveBaselineY`, `EffectiveScriptBaselineY`, `LeftMargin`, `RightMargin`, `GetXCoordinate`, `ArrangeStaticLayout`, `GetNodeAtPoint`, `ICloneable.Clone`.
- Gate (project-level, no `--type`): PASS; 3329 checked (`.mirror-cache/release-il-ae210befc875/release-il.json`).

### Batch 2: Controls.Internal.VisualSymbolNode (rest of the type)

- Members: all remaining members, including fields, `IsMonoLineProperty` (.cctor), `HorizontalMargin`, `ScreenInkBounds`, `Geometry`, `SymbolBounds`, `Get/SetIsMonoLine`, four constructors, `OnVisualParentChanged`, `OnShowOperatorOnlyWhenActiveChanged`, `OnRender`, `MeasureStaticLayout`, `UpdateText`, `Clone`.
- Shapes that needed the Release IL form:
  - `OnShowOperatorOnlyWhenActiveChanged` and `UpdateText`: Mathcad uses `brtrue` to a body after a `ret`, so both are written as guard clauses (`if (!value) return;`, `if (_symbol == null) return;`).
  - `UpdateText`: `_baseline = glyphRun.BaselineOrigin.Y;` (a named `Point` local shifts the local slot and the `ldarg.0` order).
  - `IsMonoLineProperty`: `new FrameworkPropertyMetadata(false)` without the `(PropertyMetadata)(object)` cast (the cast emits a `castclass` Mathcad does not have).
- Gate (project-level): PASS; 3347 checked (`.mirror-cache/release-il-0a75a2e838d0/release-il.json`).
- Generated file `Generated/Mirror/Controls.Internal.VisualSymbolNode.g.cs` now has an empty class body (all stubs moved to the hand-written partial).

## Remaining

- See the next section for the batch queue; none claimed beyond this list.

### Batch 3: Controls.Internal.VisualOutfixNodeEditor (fields, accessors, property handlers, focus/remove, measure-free members)

- Exact: fields `_leftFence`, `_rightFence`, `_highlighter`, `_firstFenceOffset`; `NodeSide`, `EditMode`, `CaretOpacity`; the five static `On*PropertyChanged` handlers; `GetFocus`, `Remove`, `BindToSettings`, `UpdateTypeover`, `RemoveHighlight`, `GetFence`, `UpdateCaretBlinking`, `SetParensEditorVisibility`, `SetCaretType`, `OnNodeSideChanged`, `OnCaretColorChanged`, `OnCaretOpacityChanged`, `OnScopeColorChanged`.
- Parked (kept as `[MirrorStub]` in the generated file): `MeasureOverride` (first difference at #37, a `visualOutfixNode?.DesiredSizeWithoutFences ?? ...` branch), `ArrangeOverride` (`is VisualOutfixNode { Horizontal: false }` pattern local slot), `UpdateParens` (first difference at #3 on the base field load), `BindHighlighterProperties` (switch shape at #2).
- Still stubs: the five `Property` DependencyProperty fields and the constructor (static initializer `.cctor` not yet written).
- Gate (project-level): PASS; 3369 checked (`.mirror-cache/release-il-afff70c54927/release-il.json`).

### Batch 4: Controls.Internal.VisualOutfixNodeEditor (static initializer and constructor)

- Exact: `.cctor` (five DependencyProperty registrations; `FrameworkPropertyMetadata` locals with `AffectsRender` as statement assignments, `+=` for callbacks) and the constructor (the transform is an if/else assignment to a `GeneralTransform` local; the ternary expression layout gave `brtrue` where Mathcad has `brfalse`).
- Parked: `MeasureOverride`, `ArrangeOverride`, `UpdateParens`, `BindHighlighterProperties` remain `[MirrorStub]` in the generated file.
- Gate (project-level): PASS; 3371 checked (`.mirror-cache/release-il-fd8e88d60a52/release-il.json`).

### Batch 5: Controls.Internal.VisualTextNodeEditor (accessors, caret, highlight, handlers; no static initializer)

- Exact: fields `_caret`, `_highlightBox`, `_highlightX`, `_idAlignmentBox`, `_highlightBrush` (initializers); `CaretPosition`, `Text`, `CharacterWidths`, `Geometry`; the static caret/highlight handlers; `BindProperties`, `HookupEvents`, `UnhookEvents`, `Remove`, `InitializeCaret`, `InitializeHighlightBox`, `SetSelection`, `UnsetSelection`, `SetCaretType`, `OnCaretColorChanged`, `UpdateCaretBlinking`, `OnCaretPositionChanged`, `OnTextChanged`, `GlyphIndexToX`, `UpdateCaretPosition`, `UpdateMeasure`, `ArrangeOverride`, `OnRender`, the three `OnEditNodeMouse*` handlers, `OnHighlightColorChanged`, `OnHighlightOpacityChanged`.
- Parked: `MeasureOverride` (Mathcad loads `_highlightBox` before the `_caret.Line.DesiredSize` temp; the C# shapes tried either leave an extra local or reorder the field loads).
- Still stubs: the three `*Property` DependencyProperty fields, `CreateDependencyProperty`, and the constructor (static initializer not yet written).
- Gate (project-level): PASS; 3403 checked (`.mirror-cache/release-il-7b3e7f0b6705/release-il.json`).

### Batch 6: Controls.Internal.VisualTextNodeEditor (static initializer, factory, constructor)

- Exact: `.cctor` (three DP registrations through `CreateDependencyProperty`), `CreateDependencyProperty`, and the constructor. Remaining stub: `MeasureOverride` (parked, see batch 5).
- Gate (project-level): PASS; 3406 checked (`.mirror-cache/release-il-1dfc1bf596c8/release-il.json`).

### Batch 7: Controls.Internal.VisualMatrixNode (fields, accessors, bracket/heading helpers)

- Fields moved to the hand-written partial (the existing `ResizeAdorner`, `ErrorCell`, `OriginalSize`, `RowCount`, `Cells`, heading and baseline members from main stay as they were).
- Exact: `ColumnCount`, `NodeInfo`, `ErrorNode`, `HeadingsVisibility`, `this[int]`, `LeftFence`, `RightFence`, `DesiredSizeWithoutFences`, `Horizontal`, `SetParensVisibility`, `CreateBrackets`, `CreateHeading`.
- Parked (kept as `[MirrorStub]`): `NodeChildren` (an extra local slot in the `UIElement[]` copy), `OnMouseMove` and `LoadedHandler` (Mathcad calls `SetMatrixForResizeAdorner`/`UpdateMatrixForResizeAdorner` with `callvirt`; the Quire method is emitted `call`, which is an `EquationControl` member virtuality difference, not a matrix-node one), `TryDragIntoHiddenPart` (branch sense of the `ViewportSize == FullSize || IsInResult` guard at #13).
- Still stubs: the constructor, `MeasureOverride`, `ArrangeOverride`, `GetNodeAtPoint`, `HookupMatrixResizeWidget`, the resize-indicator helpers (closure in `SetupResizeIndicatorBehaviour`), `ArrangeHeadingContent`, `GetMatrixRowOrColumnIndices`, `ConstructMatrixGrid`, `CreateTextNode`, `CheckIfSpecialCellNeeded`, `GetIndexOfNode`, `SetHeadingsCellsVisibility`, `PopulateMatrixHeadings`, `BuildChildren`.
- Gate (project-level): PASS; 3419 checked (`.mirror-cache/release-il-58423d9f557b/release-il.json`).

### Batch 8: Controls.Internal.VisualMatrixNode (OnMouseMove, LoadedHandler)

- Resolved the batch 7 parked pair. Mathcad's `OnMouseMove` and `LoadedHandler` call `EquationControl::SetMatrixForResizeAdorner` and `UpdateMatrixForResizeAdorner` through `callvirt` after an explicit `brfalse` null check on a local. The earlier `?.` form emitted `call`. The target methods are `internal` (`assembly`) and non-virtual in both builds, so this was a source-shape difference, not a mirror-wide virtuality mismatch.
- `OnMouseMove` also needed guard clauses (`if (ResizeAdorner != null && DevAssert(...).Passed) return;`, then `if (LeftButton == Pressed) return;`) to match Mathcad's branch layout.
- Gate (project-level): PASS; 3421 checked (`.mirror-cache/release-il-e6a31d7292b4/release-il.json`).

### Batch 9: Controls.Internal.VisualMatrixNode (heading indices, visibility, special cells, index lookup)

- Exact: `GetMatrixRowOrColumnIndices`, `SetHeadingsCellsVisibility`, `BuildChildren`, `CheckIfSpecialCellNeeded` (the ellipsis cells are U+22F1, U+2026 and U+22EE, which Mathcad uses), `GetIndexOfNode`.
- Parked: `PopulateMatrixHeadings` (heading string constants, `"\n?"`/`" ?\n"` with Mathcad's ellipsis characters, and a local slot order), `ConstructMatrixGrid` (branch layout at #26), `CreateTextNode` (the `visualTextNode is DependencyObject` cast sits at a different stack position than the `as` form).
- Gate (project-level): PASS; 3426 checked (`.mirror-cache/release-il-89628e571ba0/release-il.json`).

### Batch 10: Controls.Internal.VisualMatrixNode (IsResizeIndicator)

- Exact: `IsResizeIndicator` (Mathcad compares against U+2026, U+22EE, U+22F0 and returns the U+22F1 comparison).
- Parked: `ResizeIndicatorOnLoaded` (the `sender as` shape differs at #4), `ArrangeHeadingContent` (the `for` loop and `text != null && IndexOf` branch layout differ at #11; the heading ellipsis test is U+22EE, which is now in the candidate).
- Gate (project-level): PASS; 3427 checked (`.mirror-cache/release-il-5a6ee6722eb1/release-il.json`).

### Batch 11: Controls.Internal.VisualMatrixNode (GetNodeAtPoint, HookupMatrixResizeWidget)

- Exact: `GetNodeAtPoint` (hit-test branches as written), `HookupMatrixResizeWidget` (the null check is nested positively, `if (equationControl != null) { ... }`, because Mathcad's `brfalse` jumps to the end; a guard return emitted `brtrue`).
- Gate (project-level): PASS; 3429 checked (`.mirror-cache/release-il-44da2f95b648/release-il.json`).

### Batch 12 attempt: Controls.Internal.VisualMatrixNode (MeasureOverride, ArrangeOverride)

- No member landed; both are parked as `[MirrorStub]` and the attempted code was discarded.
- `MeasureOverride`: at #4 Mathcad stores into local slot 12 (its earlier locals occupy 0–11), while the C# form uses slot 0; the body compiles but the local layout does not match.
- `ArrangeOverride`: the `visibility` ternary with `HeadingsVisibility = ...` as an assignment expression matches the first branch but leaves the arm one instruction larger (`brfalse` 17 vs 16), and the body is 160 vs 155 instructions. The if/else and inverted-ternary forms did not resolve it.
- Gate on the attempted tree with both parked: PASS; 3429 checked (`.mirror-cache/release-il-cdd9decef00a/release-il.json`), which is the same exact set as before this attempt.

### Batch 13: Controls.Internal.VisualMatrixNode (constructor)

- Exact: `VisualMatrixNode()` (the `Name = VISUAL_MATRIX_NAME` read, `_opacity` initializer, heading and bracket setup, `Loaded += LoadedHandler`).
- Still parked/untried: `SetupResizeIndicatorBehaviour` (a lambda closure whose `<>c` and `b__` ordinals are not yet checked), `MeasureOverride`, `ArrangeOverride` (parked, see batch 12).
- Gate (project-level): PASS; 3430 checked (`.mirror-cache/release-il-aa57582cb7b8/release-il.json`).

### Batch 14: Controls.SpecTableControl (small independent group)

- Exact: `GetHelpContext`, `DataRows`, `GetActiveColumn`, `FirePropertyChanged`, `ContainsError` (foreach with a pattern variable).
- Gate (project-level): PASS; 3435 checked (`.mirror-cache/release-il-15761d609404/release-il.json`).
- Dependency recorded: `VisualMatrixNode.SetupResizeIndicatorBehaviour` is a lambda closure (`MouseDown +=` with a `<>c` cache); it stays a stub until the closure ordinal layout is verified, and it does not block the rest of the matrix type.

### Batch 15: Controls.SpecTableControl (active-cell accessors, custom-initialization, error count)

- Exact: `ActiveCell`, `ActiveCellCoordinate`, `ActiveCellSExpression` (explicit null check, not `??`), `CurrentError` (positive null check on the interface cast), `InCustomInitialization`, `ErrorCount`.
- Left as in main: the existing `CalcIsDisabled` (already mirrored), `HideFromCalculationContext` and `EngineResults` (closure and LINQ shapes, parked for now).
- Gate (project-level): PASS; 3442 checked (`.mirror-cache/release-il-2dcb1423561d/release-il.json`).

### Batch 16: Controls.SpecTableControl (static initializer)

- Exact: `.cctor` (type token, default-style and whiteboard `IsActive` overrides, `SynchronizationStatus`/`CalculatingStatus` owners, IO tag override, `CalcIsDisabled` owner with its change handler, and the `ContentChanged` routed-event owner). The five static fields moved to the hand-written partial.
- Still parked/untried: `HideFromCalculationContext` and `EngineResults` (closure and LINQ shapes).
- Gate (project-level): PASS; 3443 checked (`.mirror-cache/release-il-895c54aed7c0/release-il.json`).

### Batch 17 probes (no landing)

- `SpecTableControl` event move (8 field-like events): gate PASS, but `add_Changed`/`remove_Changed` are "unchanged and never matched Mathcad (declaration differs)", so the move was reverted rather than landing unverified accessors.
- `EngineResults` (LINQ `Select` lambda): Release IL differs at the closure. Mathcad names the lambda `<get_EngineResults>b__202_0` in `SpecTableControl+<>c`; Quire's ordinal is different, so the `ldftn` and the `<>c` member have no Mathcad counterpart. The ordinal would need the 201 Mathcad members before it as placeholders in order; reverted, not landed.

### Batch 18: Controls.InputControls.ComboBoxTableModel (row/column index properties, title, private constructor, cell lookup, table resize)

- Exact: `TableTitle`, `SelectedTableRowIndex`, `SelectedNameValuesRowIndex`, the public constructor (`: this(2, 2)`), the private `(rows, columns)` constructor (guard-clause shape: Mathcad's `brfalse` skips the body on the assertion's `Failed`), private `GetCellAt`, `AreCellPositionsValid`, `UpdateTableSize`.
- Still stubs: `GetColumnCells`, `UpdateComboBoxTable`, `IsValid`, `InsertRow`/`InsertColumn`, and the range helpers. Each uses a lambda or iterator, so they are closures I have not verified yet.
- Gate (project-level): PASS; 3454 checked (`.mirror-cache/release-il-daaf9e75b317/release-il.json`).

### Batch 19: Controls.InputControls.EditableComboBoxViewModel (placeholder and table sizes, midpoints, constructor, placeholder measurement)

- Exact: `PlaceHolderBoxSize` (guard-clause setter), `MidPointBetweenFirstAndSecondRow` and `...Column` (two guard clauses: the assertion first, then the equality check), `TableSize` (guard-clause setter), the constructor (`: base(tableModel)` and `SetupCommandBindings()`), and `UpdatePlaceHolderBoxSize` (the placeholder measurement).
- The existing `SelectedCells`, `ClearSelection` and `CreateSelection` in the partial stay as they were. I did not replace them with the batch copies.
- Still stubs: `AllCells`, the three events (same accessor-declaration concern as SpecTableControl), `ExpandComboBoxRowsColumns`, `PasteSelectedValues`, the paste/copy and delete helpers, and `ProcessConsecutiveIntervalsInReverseOrderAndNotify` (uses lambdas or iterators).
- Gate (project-level): PASS; 3464 checked (`.mirror-cache/release-il-943ca0b12d75/release-il.json`).

### Batch 20: Controls.InputControls.EditableComboBoxViewModel (interval helper, selected-cell clear)

- Exact: `ProcessConsecutiveIntervalsInReverseOrderAndNotify` (internal static; no lambdas inside, only the caller-supplied delegates) and `RemoveOrClearSelectedCells` (its `DeleteColumns`/`DeleteRows` calls are still stubs, but the branch and the flag shapes match).
- Still stubs, with the first mismatch location where tried: `CopyEditMode` (not attempted yet; uses `WpfClipboard` and `ComboBoxInputControlData`), `DeleteRows`/`DeleteColumns`/`DeleteEmptyRows`/`DeleteEmptyColumns` (lambda closures into the interval helper), `SetupCommandBindings` (command lambdas), `PasteEditMode` and the paste helpers.
- Gate (project-level): PASS; 3466 checked (`.mirror-cache/release-il-ee007d1c8ca3/release-il.json`).

### Batch 21: Controls.InputControls.EditableComboBoxViewModel (CopyEditMode)

- Exact: `CopyEditMode`. The first attempt (`new ComboBoxInputControlData(...).GetMatrixSExprFromSelection(...)`) emitted `call` where Mathcad emits `callvirt`; storing the data object in a local before the call matches.
- Gate (project-level): PASS; 3467 checked (`.mirror-cache/release-il-f82ac9b274f4/release-il.json`).

### Batch 22: Controls.InputControls.EditableComboBoxViewModel (TryReadMathNodeFromClipboard) — landed as a0d7e2768

- Gate (project-level): PASS; 3468 checked, zero failures.

### Batch 23: Controls.InputControls.EditableComboBoxViewModel (PasteEditMode)

- Exact: `PasteEditMode(DiscreteRect)`. The guard clause shape matches Mathcad's `brtrue` after `ret`.
- The `if (discreteRect.HasValue)` form with a named `DiscreteRect? discreteRect` local put the nullable in slot 1 (Mathcad: slot 2, `valueOrDefault` in slot 1). The `is DiscreteRect pastedRect` pattern form matches the slot layout and the `GetValueOrDefault` shape.
- Gate (project-level): PASS; 3469 checked (`.mirror-cache/release-il-afea7ba59d29/release-il.json`).
- Still stubs: `AllCells`, the three events, `ExpandComboBoxRowsColumns`, `PasteSelectedValues`, `TextBox_OnPaste`, `SetupCommandBindings`, `DeleteRows`/`DeleteColumns`/`DeleteEmptyRows`/`DeleteEmptyColumns`.

### Batch 24: Controls.InputControls.EditableComboBoxViewModel (row/column expansion, paste into selection, text-box paste)

- Exact: `ExpandComboBoxRowsColumns` (two `while` loops, which Roslyn emits bottom-tested like Mathcad), `PasteSelectedValues` (struct `FullSize` temp in slot 7, the `DevAssert` bounds check, and the `CellsAdded?.Invoke` null-conditional), and `TextBox_OnPaste` (the `is ICellData` pattern in the if-else shape, and the `MATRIX` branch with `return`).
- Gate (project-level): PASS; 3472 checked (`.mirror-cache/release-il-bff432f43703/release-il.json`).
- Still stubs: `AllCells`, the three events, `SetupCommandBindings` (command lambdas), `DeleteRows`/`DeleteColumns` (lambdas capturing the flags), `DeleteEmptyRows`/`DeleteEmptyColumns` (nested lambdas). All five use closures whose `<>c`/`<>c__DisplayClass` and `b__` ordinals are not yet checked.

### Batch 25: Controls.InputControls.EditableComboBoxViewModel (CellsAdded, RowsDeleted, ColumnsDeleted events)

- Exact: the three field-like events, moved from the generated stub to the hand-written partial. Their add/remove accessors are the Roslyn `CompareExchange` loop (the Mathcad backing field is `CellsAdded`, `RowsDeleted`, `ColumnsDeleted`), and all six accessors are in the checked set.
- Gate (project-level): PASS; 3472 checked, zero failures (`.mirror-cache/release-il-932ffaddd1b5/release-il.json`).

### Closure probe (no landing): AllCells

- `AllCells => TableModel.AllRows.SelectMany((ComboBoxRow row) => row.AllCells)` is exact in body shape (`ldsfld <>9__N_0`, `ldftn <get_AllCells>b__N_0`, `dup/stsfld`, `SelectMany`), but the closure ordinal differs: Mathcad `<>c::<get_AllCells>b__7_0`, Quire `<>c::<get_AllCells>b__56_0` (the Mathcad field is `<>9__7_0`). The method's position in the merged type decides the number, so the Quire ordinal moves with every member before it.
- Reverted, not landed. Evidence: `.mirror-cache/release-il-df314d65fdda/release-il.json`.
- Mathcad ordinals for the other closures in this type, for the record: `SetupCommandBindings` `b__46_0..3`; `DeleteRows` display class `61_0`; `DeleteColumns` `63_0`; `DeleteEmptyRows` `73_0` and `b__73_2`; `DeleteEmptyColumns` `74_0..2`.

### Still stubs (closure-gated, not landed)

- `AllCells` (ordinal 7), `SetupCommandBindings` (46), `DeleteRows` (display class 61), `DeleteColumns` (63), `DeleteEmptyRows` (73), `DeleteEmptyColumns` (74). Each needs the Mathcad ordinal reproduced by placeholder members in Mathcad order before it, which is the closure-ordinal blocker in the lane notes.

### Batch 26: Controls.InputControls.EditableComboBoxViewModel (AllCells closure, with ordinal placeholders)

- Exact: `AllCells` (`SelectMany` closure `<get_AllCells>b__7_0`, ordinal 7) and the constructor, which moves to the top of the hand-written partial so that it precedes `AllCells` as in Mathcad's member order.
- Ordinal finding: a lambda's Roslyn ordinal is 1 plus the number of members declared before its method in the merged type. Generated stubs and fields come first (the generated `.g.cs` is compiled first), so the five stub methods in the generated file pushed `AllCells` to 11 and then 56. Moving those five methods into the hand-written partial as unmarked placeholders (`=> throw ...NotImplemented()`) in Mathcad order brought the count to 7.
- Placeholders now in the partial, in Mathcad order: `SetupCommandBindings` (after `TextBox_OnPaste`), `DeleteRows` (after `CanDeleteRow`), `DeleteColumns` (after `CanDeleteColumn`), `DeleteEmptyRows` and `DeleteEmptyColumns` (after `ProcessConsecutiveIntervalsInReverseOrderAndNotify`). Each of their closures still needs its ordinal checked when implemented: `SetupCommandBindings` is Mathcad `b__46_0..3`, `DeleteRows` display class `61_0`, `DeleteColumns` `63_0`, `DeleteEmptyRows` `73_0`, and `DeleteEmptyColumns` `74_0..2`.
- Gate (project-level): PASS; 3476 checked, zero failures (`.mirror-cache/release-il-6bc583f323e2/release-il.json`).
- Still stubs (placeholders): `SetupCommandBindings`, `DeleteRows`, `DeleteColumns`, `DeleteEmptyRows`, `DeleteEmptyColumns`. Their ordinals depend on the members declared before each one in Mathcad order, which the placeholders in the partial now provide only approximately, so each needs its own probe.

### Probe (no landing): SetupCommandBindings closure ordinal

- With the body implemented (four instance lambdas, `CreateCommandBinding` with `ApplicationCommands.Copy`/`Paste`), Quire names them `<SetupCommandBindings>b__55_0..3`; Mathcad names them `b__46_0..3`. The `ldftn` targets and the `CreateCommandBinding` shape otherwise match.
- Moving the three events (with their six accessors) and the `DeleteRows`/`DeleteColumns` placeholders after `SetupCommandBindings` drops Quire to `b__44`. So the events and the two delete placeholders account for 11 of the 55 preceding members. Mathcad needs `b__46`, which is two members short of the Quire count, and the partial's other members before `SetupCommandBindings` (`GetCellAt`, `CanDeleteRow`, `GetTableRect`, `RemoveOrClearSelectedCells` neighbours) are not yet in Mathcad order.
- Reverted, not landed. The fix is to put the whole partial in Mathcad's member order (token order in `EditableComboBoxViewModel.members.tsv`, with events and fields counted), not a single move. Evidence is in the probe logs `.mirror-cache/release-il-50b97015ac23/release-il.json` (55) and `.mirror-cache/release-il-3b47ae357624/release-il.json` (44).

### Status at this checkpoint

- Landed for this type: batches 22–26 (TryReadMathNodeFromClipboard, PasteEditMode, ExpandComboBoxRowsColumns/PasteSelectedValues/TextBox_OnPaste, the three events, AllCells). The project gate is PASS with 3476 checked.
- Remaining for this type, all closure- or ordinal-gated: `SetupCommandBindings`, `DeleteRows`, `DeleteColumns`, `DeleteEmptyRows`, `DeleteEmptyColumns`. They stay as unmarked placeholders in the partial, in Mathcad order.

### Batch 27: Controls.InputControls.EditableComboBoxViewModel (SetupCommandBindings, DeleteRows, DeleteColumns, DeleteEmptyRows, DeleteEmptyColumns)

- Closure ordinal rule, now verified on all five members: a lambda's ordinal `N` is `methodIndex + 5 + properties + events`, where `methodIndex` is the 0-based index of the containing method in Mathcad's token order (`members.tsv`, ctor = 0), `5` is the generated fields, `properties` counts the property declarations up to and including the method's own property (get and set count once), and `events` counts the event declarations before the method. Checks: `AllCells` 1+5+1 = 7; `SetupCommandBindings` 30+5+11 = 46; `DeleteRows` 42+5+11+3 = 61; `DeleteColumns` 44+5+11+3 = 63; `DeleteEmptyRows` 54+5+11+3 = 73; `DeleteEmptyColumns` 55+5+11+3 = 74.
- Layout: the partial is in Mathcad token order (ctor, AllCells, the property declarations, the methods up to `TextBox_OnPaste`, `SetupCommandBindings`, the events, then the rest). Pure reorder, no member added or removed.
- Gate (project-level): PASS; 3498 checked, zero failures (`.mirror-cache/release-il-d6bb9413c9e2/release-il.json`). Earlier exact rows (AllCells, the events, PasteEditMode, ExpandComboBoxRowsColumns, PasteSelectedValues, TextBox_OnPaste) remain exact.
- Regeneration: `dotnet build` of `Quire.Componentsmgd` (Release) and `dotnet run -c Release --project tools/Quire.Tools.MirrorGen -- emit` leave `Controls.InputControls.EditableComboBoxViewModel.g.cs` byte-identical (zero diff). The emit also rewrites about 330 unrelated generated files on this worktree, which were reverted; that drift is outside this lane.
- Dependency note: `DeleteEmptyColumns` calls `TableModel.GetColumnCells`, which is still a generated stub in `ComboBoxTableModel`. The member is Release-IL exact; at run time it reaches that stub until `ComboBoxTableModel.GetColumnCells` is mirrored.

### Batch 28: Controls.InputControls.ComboBoxTableModel (member order, explicit GetCellAt, removal, range helpers)

- Member order: the hand-written partial is in Mathcad token order, and every method stub from the generated file is now a placeholder in the partial in that order (so the generated file keeps only the four fields). The closure rule from batch 27 holds for this type: a lambda ordinal is `methodIndex + 4 fields + properties + events`, with `methodIndex` counted from the first constructor as 0. Checks: `IsValid` 18+4+8 = 30, `InsertRow` 21+4+8 = 33, `RemoveRow` 35, `RemoveColumn` 39, `ClearSelectedCellsContent` 41, `AreAllSelectedCellsEmpty` 42, `CellsInRange` 44 (display class), `TrimWhitespacesInUnitCells` 47, `UpdateCellsPositionForShiftedNameValuesRows` 36 (display class), `GetColumnCells` 26 (display class).
- Exact, new: `IComboBoxTableModel.GetCellAt` (explicit wrapper), `IsValid`, `RemoveRow`, `RemoveColumn`, `ClearSelectedCellsContent`, `AreAllSelectedCellsEmpty`, `TrimWhitespacesInUnitCells`, `CellsInRange`, `TakeSubsequence<T>`, `UpdateCellsPositionForShiftedNameValuesRows`, `UpdateCellsInRangeValues`.
- Shapes that the Release IL requires: `cell.Row -= 1` (not `cell.Row--` or `--cell.Row`, which store a temp), and the `if (DevAssert(...).Failed) return;` guard for `UpdateCellsInRangeValues` (`brfalse` past the return).
- Gate (project-level): PASS; 3521 checked, zero failures (`.mirror-cache/release-il-fbe3a9a25729/release-il.json`).
- Regeneration: `dotnet run -c Release --project tools/Quire.Tools.MirrorGen -- emit` leaves `Controls.InputControls.ComboBoxTableModel.g.cs` byte-identical (zero diff). Its unrelated drift on the worktree was reverted.

#### Blocker: GetColumnCells (not landed)

- Its Mathcad body is `if (!DevAssert(...).Failed) { IEnumerable<IReadonlyCellData> ... = AllRows.Select(...); return ...; } return Enumerable.Empty<...>();`, with a display class `26_0` that captures `columnIndex`, and Mathcad stores the Select result in local 1 (`stloc.1; ldloc.1; ret`).
- Roslyn in Release returns the Select result directly, with no stloc/ldloc, in every form tried: a named local, a local declared before the `if`, a direct return, and both block orders. The first difference is at #20 (`brtrue.s` target off by two bytes), and the entry count is 33 against Mathcad's 35.
- It stays a placeholder in the partial. `DeleteEmptyColumns` still reaches it at run time.
- Left open: `UpdateComboBoxTable` (display classes `29_0`/`29_1`), `InsertRow` (lambda `<>c` `33_0`), `InsertColumn` (`37_0`). Their ordinals are predicted by the rule above, and each needs its own gate probe.

### Batch 29: Controls.InputControls.ComboBoxTableModel (InsertRow, InsertColumn, UpdateComboBoxTable)

- Exact: `InsertRow` (lambda `cell.Row += 1`, `<>c` `33_0`), `InsertColumn` (lambda `cell.Column += 1`, `<>c` `37_0`), and `UpdateComboBoxTable` (display classes `29_0` and `29_1` for the captured `data` and `dataRowsList`, with the `ValueTuple` row list and `DebugTools.IsDebug`). The partial gains `using Quire.EquationEditor.Tree;` for the adapter type.
- Gate (project-level): PASS; 3531 checked, zero failures (`.mirror-cache/release-il-8c2536ad0c9b/release-il.json`).
- Regeneration: the generated file is unchanged (zero diff) after a fresh Release build of `Quire.Controls.EquationControl` and `emit`. A stale project build made `emit` re-add all method stubs, so the build must come first.
- Still placeholders: `GetColumnCells` (the Release IL blocker in batch 28). No other ComboBoxTableModel member remains as a placeholder except that one; every method stub from the generated file is now either exact or the `GetColumnCells` placeholder.

### Batch 30: Controls.Internal.LimitLayoutManager (FindClosestChild, IsBetween)

- Exact: `FindClosestChild` (the override with its loops and `out` writes) and `IsBetween` (leaf). The stubs are removed from the generated file and both are in the hand-written partial.
- Shape note: the decompiled `double x`, `double y` and `Rect val` locals are not in Mathcad's IL. Mathcad reads `hitPoint.X` and `LimitExpressionRect.Right` directly; `Rect val = ...` with `val.Right` put an extra local in the candidate.
- Gate (project-level): PASS; 3533 checked, zero failures (`.mirror-cache/release-il-8f545cd3fe11/release-il.json`).

### Batch 31: Controls.Internal.Caret (static fields and static initializer)

- Exact: `CARET_WIDTH` (const), the three static `DependencyProperty` fields (`ColorProperty`, `BlinkProperty`, `BlinkPeriodProperty`) and `.cctor`. The `Blink` metadata is an object initializer in the `Register` argument list. Mathcad loads the type tokens before `new FrameworkPropertyMetadata()`, so the metadata cannot be a local assigned first (that order gave a call-order mismatch).
- Gate (project-level): PASS; 3534 checked, zero failures (`.mirror-cache/release-il-7e9ecbb748de/release-il.json`).

### Parked (not landed): LimitLayoutManager siblings

- `ConjugateLayoutManager.FindClosestChild` (and `VectorizationLayoutManager`, same size): Mathcad reads `hitPoint.X` before `OperatorSymbolRect` in the `X < Left` test, and it merges the `X < 0` and `X < Left` returns into one block. The `||` form matches the read order and the branch, but leaves `.maxstack 2` against Mathcad's `3`. A `Rect` local declared before the `X` read matches the maxstack question no better and changes the call order. Not landed.

### Batch 32: Controls.Internal.VisualTextRun (Text, PropertyChanged)

- Exact: the `Text` property (getter returns `_text`; the setter returns early when the value is unchanged, then sets `_text` and calls `UpdateText()`) and the `PropertyChanged` event. Both stub pairs are removed from the generated file, and both are in the hand-written partial with the `System.ComponentModel` using.
- Still a stub: `UpdateText` (protected virtual). Its Mathcad body is not attempted in this batch.
- Gate (project-level): PASS; 3536 checked, zero failures (`.mirror-cache/release-il-fa11980eb547/release-il.json`).

### Batch 33: Controls.Internal.VisualTextRun.UpdateText

- Exact: `UpdateText` (protected virtual), in Mathcad token order after `OnRender`. The guard clauses match Mathcad's `brtrue`/`brfalse` layout, `Font` is read through the property at each use, and the `TextDecorations` local is loaded before the `FormattedText` construction. The baseline is one expression with `SafeComputeInkBoundingBox(...).Height`, since Mathcad loads `_baseline` before the `Rect`.
- The generated stub pair is removed from the `.g.cs`. The `Text` setter now calls an exact `UpdateText`.
- Gate (project-level): PASS; 3537 checked, zero failures (`.mirror-cache/release-il-e1a5a1155742/release-il.json`); `UpdateText` moved from generated stub to exact.

### Batch 34: Controls.Internal.VisualTextSpan (MeasureOverride, PropertyChanged)

- Exact: `MeasureOverride` (the `Children` loop with the `is IVisualText` pattern, the `_layout` arrange, the `SafeComputeInkBoundingBox(...).Height` baseline, and the `foreach` with the `Each` lambda). It moves from generated stub to exact.
- Ordinal layout: the lambda was `<MeasureOverride>b__32_0` and Mathcad has `b__30_0`. The compiler counts the members before the method, so `HandleMathFormatChanged` and `GetSubscriptFont` moved above `MeasureOverride` (Mathcad order), and the generated `UpdateChildren` stub, which the compiler orders first, became an unmarked placeholder after `ArrangeOverride`. The `PropertyChanged` event moved to the end of the class. With that order the lambda is `b__30_0`.
- `PropertyChanged` was already checked as exact before this batch (its accessors had no verdict change). The partial now declares it, and its stub pair is gone from the `.g.cs`.
- Correction: this entry first called `UpdateChildren` "Each-free" and a placeholder with a layout-only body. That was wrong: its Mathcad body has a `foreach` with an `Each` lambda (`b__32_0`). Batch 35 makes it exact.
- Gate (project-level): PASS; 3539 checked, zero failures (`.mirror-cache/release-il-209d61aa0a54/release-il.json`).

### Batch 35: Controls.Internal.VisualTextSpan.UpdateChildren

- Exact: `UpdateChildren` (protected virtual), in Mathcad token order after `ArrangeOverride`. The `Children`/`Inlines` guard, the `foreach` over `Inlines` with the `Run` and `Span` branches, the `Subscript` ternary item type, the `defaultSubscriptItem` layout item, and the `Each` lambda `b__32_0` (ordinal set by the member order from batch 34) all match.
- Shape note: the `Span` pattern variable is declared at loop-body scope, with `if (inline is Span span)` after the `Run` block's `continue`. An `else if` put `span` in a nested scope, which gave `visualTextRun` the wrong local slot (`stloc.s 6` against Mathcad's `stloc.s 7`). Keep this shape; the comment in the code says why.
- The partial gains `using Quire.Wpf;` for `Subscript`. The stub pair was already removed from the `.g.cs` in batch 34.
- Gate (project-level): PASS; 3541 checked, zero failures (`.mirror-cache/release-il-2238edebf955/release-il.json`); `UpdateChildren` moved from generated stub to exact.

### Batch 36: Controls.SpecTableControl (CurrentIdLabel, CurrentDisplayIdLabel, Activate, FireRequestRemove)

- Exact: the `CurrentIdLabel` and `CurrentDisplayIdLabel` properties (the setter maps `IdLabels.None` to `null` and fires `PropertyChanged`), `Activate(bool)` (`ExecuteOnLoad(this, DelegateFocus)`, a method group with no closure), and `FireRequestRemove()` (the try/catch around `_currentCell = ActiveCell`, then `RequestRemove(this, Direction.Left)`). Their stub pairs are removed from the `.g.cs`; each moved from generated stub to exact.
- Not in this batch: `DelegateFocus` (its `Focus` lambda is a closure whose ordinal depends on the member layout) and `PersistentData` with its region helpers. The `GetColumnCells` blocker in `ComboBoxTableModel` is unchanged.
- Gate (project-level): PASS; 3547 checked, zero failures (`.mirror-cache/release-il-31d5d5c66afc/release-il.json`).

### Batch 37: Controls.EquationCommands (static command fields and static constructor)

- Cause: every command field in `EquationCommands` was a generated `MirrorStub` with no initializer, so `InsertOperator` (and the other commands) stayed null. The SolveBlock harness reached `SetupCommandBindings` with a null `InsertOperator`.
- Exact: the static constructor (`.cctor`), which assigns all 61 command fields in Mathcad order (`_typeOfThis`, the `RoutedUICommand`s and `RoutedCommand`s, the input gestures, and the two class input bindings). The 61 field declarations move from the `.g.cs` into the partial without the stub attribute, as batch 31 did for `Caret`.
- Usings added for the helper types: `Quire.Wpf` (Gestures, RoutedUICommandWithFunctionalityKey), `Quire.FunctionalitiesLimitation`, `Quire.Controls.Core`, and `System.Windows.Documents` (EditingCommands).
- Gate (project-level): PASS; 3548 checked, zero failures (`.mirror-cache/release-il-9f6bb75ba21b/release-il.json`); the `.cctor` is checked and matches.

### Batch 38: Controls.Internal.VisualIdNode (CaretNormalHeight, CaretNormalOffset, CaretSubscriptHeight, CaretSubscriptOffset)

- Exact: the four caret getters. Each returns 0 when `Children` is empty, reads the first child as a `VisualTextSpan`, and computes from its `Font`, `GetSubscriptFont()` and `SubscriptBaseline`, with `EffectiveBaselineY` for the normal offset. None has a closure. Their stub pairs are removed from the `.g.cs`; each moved from generated stub to exact.
- Gate (project-level): PASS; 3552 checked, zero failures (`.mirror-cache/release-il-2f67e256b9f6/release-il.json`).

### Batch 39: Controls.EquationControl (IsCommitted, IsSelected, IsReadonly, IsScoped, IsTypeover)

- Exact: the five flag getters. `IsCommitted` is `!_model.IsExpressionValueModified`; `IsSelected` calls `WhiteboardDependencyProperties.GetIsSelected(this)`; `IsReadonly` and `IsScoped` / `IsTypeover` (`Scope` and `Typeover` of `EquationEditModeEnum`) read `_model`. None has a closure, and they are appended at the end of the partial, so no existing closure ordinal moves. Their stub pairs are removed from the `.g.cs`.
- The `EquationControl` reference decompile is empty in the cache (`.cs` is 0 bytes), so the IL was read directly from `Ptc.Controls.EquationControl.il`.
- Gate (project-level): PASS; 3557 checked, zero failures (`.mirror-cache/release-il-0c1b730c7c11/release-il.json`).

### Batch 40: Controls.EquationControl (IsScriptableControl, AtLeftEdge, ScopeDirection, CanHideFromCalculationContext, InterceptAndUpdateResult, UseICalculatableToRestoreResult)

- Exact: six getters. `IsScriptableControl` and `AtLeftEdge()` read `_model`; `ScopeDirection` is `_model.ScopeSide`; `CanHideFromCalculationContext`, `InterceptAndUpdateResult` and `UseICalculatableToRestoreResult` return constants (`true`, `false`, `false`). None has a closure, and they are appended at the end of the partial. Their stub pairs are removed from the `.g.cs`.
- Left out on purpose: `ErrorVisible` (Mathcad throws `NotImplementedException`, so mirroring it would be a placeholder, not an exact body) and `ComponentCookieMultiThreaded` (a compiler-generated backing field that needs an auto-property shape).
- Gate (project-level): PASS; 3563 checked, zero failures (`.mirror-cache/release-il-d43be3e9e435/release-il.json`).

### Batch 41: Controls.EquationControl (DataExchangeModel, ErrorSExpression, TypeoverSExpression, CurrentNodeIsNumeric, ViewFullWidth, ViewClippedWidth)

- Exact: `DataExchangeModel` and `TypeoverSExpression` (direct reads of `_model`); `ErrorSExpression` (`if (_model.ErrorNode != null) return ...SExpression; return "";`, two `ret`s as in Mathcad); `CurrentNodeIsNumeric` (`CurrentModeNavigatorState.Current.DescriptorName == "NUMERIC"`); `ViewFullWidth` (`_view.ActualWidth`) and `ViewClippedWidth` (`_view.DesiredSize.Width`, with the temporary `Size` local that Roslyn makes for the struct call). None has a closure, and they are appended at the end of the partial. Their stub pairs are removed from the `.g.cs`.
- Left out: `IsEmbedded` (getter and setter both need mirroring; the getter is a `GetValue` unbox, not yet checked for the setter) and `ErrorCode` (involves `IsNullOrEmpty` and a conditional that needs its own shape probe).
- Gate (project-level): PASS; 3569 checked, zero failures (`.mirror-cache/release-il-bf9d2637edf4/release-il.json`).

### Batch 42: Controls.EquationControl (EquationType, RegionType, CurrentNodeCaretPosition, CurrentNodeIsIdentifier)

- Exact: `EquationType` (a direct read of `_model`); `RegionType` (the constant `"equation"`); `CurrentNodeCaretPosition` (`CurrentModeNavigatorState.CaretPosition.GetValueOrDefault(-1)`, with the struct temporaries Roslyn makes, as in Mathcad); `CurrentNodeIsIdentifier` (`DescriptorName == "IDENTIFIER"`, the same shape as batch 41's `CurrentNodeIsNumeric`). None has a closure; they are appended at the end of the partial, and their stub pairs are removed from the `.g.cs`.
- Left out: `CurrentNodeSExpression`, `CurrentNodeLabel`, `CurrentNodeCaretSide`, `CurrentNodeNodeSide`, `CurrentNodeOperatorPartIndex` (each has a multi-branch shape on `Mode` that needs its own probe), and the matrix getters (they throw `InvalidOperationException` on a non-matrix node, a body that is not yet verified).
- Gate (project-level): PASS; 3573 checked, zero failures (`.mirror-cache/release-il-704846407bba/release-il.json`).

### Batch 43: Controls.EquationControl (IsEmbedded, TextBaselineY, IsAdornedElementReady)

- Exact: `IsEmbedded` (a `GetValue` unbox getter and a `SetValue` setter on the dependency property); `TextBaselineY` (a direct read of `_view.TextBaselineY`); `IsAdornedElementReady` (a `UIElement`-typed local holding `_view`, then `WpfUtils.GetVisualParent(view) != null` under a null guard; Mathcad's local is `System.Windows.UIElement`, so the local is declared with that type and not `var`). None has a closure; they are appended at the end of the partial, and their stub pairs are removed from the `.g.cs`.
- Left out: `ErrorCode` (involves `IsNullOrEmpty` and a conditional that needs its own shape probe).
- Gate (project-level): PASS; 3577 checked, zero failures (`.mirror-cache/release-il-34c4c22c72ba/release-il.json`).

### Batch 44: Controls.EquationControl (ErrorCode, IsShrunk)

- Exact: `ErrorCode` (a `CalculationError` null check, then `!string.IsNullOrEmpty` on its `ErrorCode`, returning that code or `""`; the three `get_CalculationError` calls are repeated, as in Mathcad, since there is no local); `IsShrunk` (`IsResizable && IsExceedsWidthConstrain`, which Roslyn lowers to the `brfalse` shape Mathcad uses). Both are appended at the end of the partial, and their stub pairs are removed from the `.g.cs`.
- Left out: `IsResizable` (reads `ContainsSymbolicEvaluation`, `SymbolicResultsWidth` and a static threshold before a dependency-property path), `ContainsSymbolicEvaluation` (a loop over the evaluation root with a `_map` lookup), and `TextBaselineOffset` (a `FindCommonVisualAncestor` / `TransformToVisual` struct path, the same shape as the `TextBaselineY` fallback).
- Gate (project-level): PASS; 3579 checked, zero failures (`.mirror-cache/release-il-03dafd60c943/release-il.json`).

### Batch 45: Controls.EquationControl (ContainsSymbolicEvaluation, IsResizable)

- Exact: `ContainsSymbolicEvaluation` (the evaluation root is tested with `!= null` as the outer branch, so the null case falls through to one final `return false`; inside, a `||` of the root's descriptor and its first child's descriptor against `"SYMBOLIC_EVALUATION"`). The first form (`if (root == null) return false;` then two returns) gave a `brtrue` where Mathcad has `brfalse`, so the null guard is written as the positive branch. `IsResizable` (`ContainsSymbolicEvaluation && SymbolicResultsWidth > RecommendedSymbolicResultsWidth`, returning `!IsEmbedded`, else `false`); its `ble.un` comparison matches Mathcad. Both are appended at the end of the partial, and their stub pairs are removed from the `.g.cs`.
- Left out: `SymbolicResultsWidth` (stub; not on the path of these two) and `TextBaselineOffset` (needs its own probe).
- Gate (project-level): PASS; 3581 checked, zero failures (`.mirror-cache/release-il-a85e499eaa0b/release-il.json`).

### Batch 46: Controls.EquationControl (SymbolicResultsWidth)

- Exact: `SymbolicResultsWidth` (a `!ContainsSymbolicEvaluation` guard returning `0.0`; a `while` walking down the first child while its descriptor is `SYMBOLIC_EVALUATION`; the third child mapped through `_map.AssociateNode` and `as VisualMathNode`; `TranslatePoint(new Point(0, 0), _view)`; `ViewFullWidth - origin.X`). It is the runtime dependency of `IsResizable` (batch 45) and is now real rather than a stub. Its stub pair is removed from the `.g.cs`; the partial gains `using Quire.Controls.Internal;` for `VisualMathNode`.
- Left out: `TextBaselineOffset` (the `FindCommonVisualAncestor` / `TransformToVisual` path, which has its own probe).
- Gate (project-level): PASS; 3582 checked, zero failures (`.mirror-cache/release-il-b4516b373053/release-il.json`).

### Batch 47: Controls.EquationControl (TextBaselineOffset)

- Exact: `TextBaselineOffset` (a `FindCommonVisualAncestor(_view) == null` guard returning `_view.TextBaselineY`; otherwise `_view.TransformToVisual(this).Transform(point).Y`, where the `Point` is a local assigned inside the argument, `Point point; ... Transform(point = new Point(0, _view.TextBaselineY)).Y`). The plain `new Point(...)` argument gave `newobj` where Mathcad builds the struct in place in a local (`ldloca.s`, `call .ctor`, `ldloc.0`), so the local-assignment form is kept with a one-line comment. Appended at the end of the partial; its stub pair is removed from the `.g.cs`.
- Gate (project-level): PASS; 3583 checked, zero failures (`.mirror-cache/release-il-31e80c182a50/release-il.json`).

### Batch 48: Controls.EquationControl (IsExceedsWidthConstrain)

- Exact: `IsExceedsWidthConstrain` (a `double` local from `IsEmbedded ? ResizeIndicatorWidth : 0.0`, then `ViewFullWidth > ActualWidth && ViewClippedWidth < ViewFullWidth - resizeIndicatorWidth`). Appended at the end of the partial; its stub pair is removed from the `.g.cs`.
- Depends at run time on `ResizeIndicatorWidth` (still a stub); only the embedded branch reads it. Left out: `ResizeIndicatorWidth` (a `ControlTemplate.FindName` / `ColumnDefinition` path, its own probe).
- Gate (project-level): PASS; 3584 checked, zero failures (`.mirror-cache/release-il-5e194eab754a/release-il.json` for the same member on 3879a01dc; re-run on d9ffeae05 at this commit).

### Batch 49: Controls.EquationControl (ResizeIndicatorWidth)

- Exact: `ResizeIndicatorWidth` (`Template.FindName("PART_ResizeIndicatorColumn", this) as ColumnDefinition`; a non-null column returns its `ActualWidth`, otherwise `0.0`). It is the run-time dependency of `IsExceedsWidthConstrain` (batch 48), now implemented instead of a stub. Its stub pair is removed from the `.g.cs`; the partial gains `using System.Windows.Controls;` for `ColumnDefinition`.
- Gate (project-level): PASS; 3585 checked, zero failures (`.mirror-cache/release-il-59128e9b146a/release-il.json`).

### Batch 50: Controls.EquationControl (ErrorVisible, Error, EngineWarnings, Mode, CurrentNode, VerticalAlignmentOffset, CalculatingStatus, SynchronizationStatus)

- Exact: `ErrorVisible` (Mathcad's own body is `throw new NotImplementedException()`, so the member throws the same; the original does not implement it either); `Error` (`_model.CalculationError`); `EngineWarnings` (`_model.EngineWarnings`); `Mode` (`_model.Mode`); `CurrentNode` (`_model.NavigatorState.Current`); `VerticalAlignmentOffset` (`-TextBaselineOffset`); `CalculatingStatus` and `SynchronizationStatus` (`(T)GetValue(TProperty)`, the unbox of the dependency property). The partial gains `using System;`, `using System.Collections.Generic;` and `using Quire.EquationEditor.Tree;`. The eight stub pairs are removed from the `.g.cs`.
- Left out: `ErrorCount` (its body reads `Error` and the `ScriptingError` resource string, with a conditional that needs a shape probe) and `ErrorText` (a delegate invocation with a `dup` branch).
- Gate (project-level): PASS; 3593 checked, zero failures (`.mirror-cache/release-il-41486776a3b6/release-il.json`).

### Batch 51: Controls.EquationControl (ErrorCount, ErrorText)

- Exact: `ErrorCount` (`if (Error == null) return _model.ScriptingError?.ResourceString != null ? 1 : 0; return 1;`; the `return 1` block is last, which is the branch order Mathcad uses, and the first attempt with `if (Error != null) return 1;` first gave a `brfalse` where Mathcad has `brtrue`); `ErrorText` (`GetErrorDisplayText` delegate invoked on `_model.CalculationError`, concatenated with the `ScriptingError?.ResourceString`, else `ErrorCode`). Both are appended at the end of the partial; their stub pairs are removed from the `.g.cs`.
- Gate (project-level): PASS; 3595 checked, zero failures (`.mirror-cache/release-il-cfd1fa01d00b/release-il.json`).

### Batch 52: Controls.EquationControl (ActiveElementOpacity)

- Exact: `ActiveElementOpacity` (`get => (double)GetValue(ActiveElementOpacityProperty)`, `set => SetValue(ActiveElementOpacityProperty, value)`). The dependency-property group is already in place: the hand-written static constructor registers `ActiveElementOpacityProperty` (and `ActiveNonNavigatableElementOpacityProperty`, which has no property on Mathcad's EquationControl), so the static initialization is unchanged. The property's stub pair is removed from the `.g.cs`.
- Gate (project-level): PASS; 3597 checked, zero failures (`.mirror-cache/release-il-1b7ec26bf553/release-il.json`).

### Batch 53: Controls.EquationControl (IsResizableInputControl, CalcIsDisabled, ResultScalingSExpression, ProhibitComboBox, ProhibitInputControl, AllowExpressionsOnly, IsMathFormatHandledLocally, OverrideIdLabelByResolvedForDefinitionLhs, CurrentError, EquationSExpressionWithResolvedLabels)

- Exact: `IsResizableInputControl` and `CalcIsDisabled` (dependency-property get `(bool)GetValue(...)`, set `SetValue(..., value)`; both properties are registered in the hand-written static constructor, which is unchanged); `ResultScalingSExpression`, `ProhibitComboBox`, `ProhibitInputControl`, `AllowExpressionsOnly`, `IsMathFormatHandledLocally`, `OverrideIdLabelByResolvedForDefinitionLhs` (`_model` get and set delegations); `CurrentError` (`Error`); `EquationSExpressionWithResolvedLabels` (`_model` get). Stub pairs removed from the `.g.cs`.
- Left out: `EquationSExpression` (its setter calls `OnSExpressionChanging`, `SetEquationSExpression` and `FireRequestEvaluation`, a separate probe), `CurrentCaretInsertionType` and `CalcIsDisabled`-like attached ones beyond this group.
- Gate (project-level): PASS; 3615 checked, zero failures (`.mirror-cache/release-il-9763d94ad339/release-il.json`).

### Batch 54: Controls.EquationControl (ShowLhsOfDefinitionForXamlSerialization, VisibleLhsOfDefinition, VisibleLhsOfDefinitionForXamlSerialization, CurrentCaretInsertionType, ResultMatrixFormat, DisplayNumberFormat)

- Exact (11 accessors): `ShowLhsOfDefinitionForXamlSerialization` (get `VisibleLhsOfDefinition == Visibility.Visible`; set toggles to `Visibility.Hidden` only when the value is false); `VisibleLhsOfDefinition` (`_model.VisibleLhsOfDefinition`); `VisibleLhsOfDefinitionForXamlSerialization` (get delegates; set toggles on `!=`); `CurrentCaretInsertionType`, `ResultMatrixFormat`, `DisplayNumberFormat` (dependency-property get with unbox or castclass, set `SetValue`). Stub pairs removed from the `.g.cs` with the indentation kept intact.
- Left out: `CurrentNodeCaretInsertionType` (a field read on the `NavigatorState` value, the field name to confirm); `SymbolicEvalKeywordsVisibilityState` (its setter adds and removes region and model event handlers, its own probe).
- Gate (project-level): PASS; 3626 checked, zero failures (`.mirror-cache/release-il-3f8d027b820c/release-il.json`).

### Batch 55: Controls.EquationControl (IsWithinSolveBlock, IsResizingMatrix, ComponentHandle, BookmarksManager, EngineResult, MathFormat)

- Exact (12 accessors): six compiler-generated auto-properties whose Mathcad accessors are plain `ldfld`/`stfld` on `<Name>k__BackingField`. `IsWithinSolveBlock`, `IsResizingMatrix` (private), `ComponentHandle` (`int?`), `BookmarksManager`, `EngineResult` are `{ get; set; }`; `MathFormat` is `{ get; private set; }`. The partial gains `using Quire.Controls.Core.BookmarkTags;`. Stub pairs removed from the `.g.cs` with the indentation kept intact.
- Left out: `ComponentCookieMultiThreaded` (Mathcad has a getter and no setter, so the backing field is written in a constructor; needs its own probe).
- Gate (project-level): PASS; 3638 checked, zero failures (`.mirror-cache/release-il-7677026b336b/release-il.json`).

### Batch 56: Controls.EquationControl (CurrentNestedResultMatrixPosition, CurrentNumberFormat, CurrentMatrixFormat, CurrentMathFormat, LocalMathFormatForXamlSerialization)

- Exact (10 accessors): `CurrentNestedResultMatrixPosition` (private dependency-property get with `castclass` to `IList<TableCoordinate>`, set `SetValue`; registered in the unchanged static constructor); `CurrentNumberFormat` and `CurrentMatrixFormat` (backing-field get; set stores the field and calls `FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(...))`, the fully qualified name keeps Quire's generic `PropertyChangedEventArgs<T>` out of the way); `CurrentMathFormat` (get `MathFormat`; set throws `NotImplementedException`, as Mathcad's own setter does); `LocalMathFormatForXamlSerialization` (get `MathFormat.ToStaticMathFormat()`, set `MathFormat.FromStaticMathFormat(value)`). Stub pairs removed from the `.g.cs` with the indentation kept intact.
- Runtime dependency (not an IL mismatch): `FirePropertyChanged` is still a generated stub, so the `CurrentNumberFormat` and `CurrentMatrixFormat` setters throw `NotImplementedException` at run time until it is implemented.
- Left out: `CalculatableChildren` (the backing-list field name to confirm), `HideFromCalculationContext` (its setter builds an undoable command), `History`, `EquationSExpression`, `SymbolicEvalKeywordsVisibilityState` (own probes).
- Gate (project-level): PASS; 3648 checked, zero failures (`.mirror-cache/release-il-1858c823e46c/release-il.json`).

### Batch 57: Controls.EquationControl (FirePropertyChanged)

- Exact: `FirePropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)` (the standard event raise: copy the `PropertyChanged` event field to a local, and if it is non-null, invoke it with `(this, e)`). This resolves the batch 56 run-time dependency: the `CurrentNumberFormat` and `CurrentMatrixFormat` setters now call a real implementation instead of throwing. The stub pair is removed from the `.g.cs`; the `PropertyChanged` event declaration is unchanged.
- Gate (project-level): PASS; 3649 checked, zero failures (`.mirror-cache/release-il-79b20090027a/release-il.json`).

### Batch 58: Controls.EquationControl (CalculatableChildren, WasSuspendedRegenerate, InCustomInitialization, CurrentIdLabel, CurrentDisplayIdLabel, CurrentNodeCaretInsertionType)

- Exact (10 accessors): `CalculatableChildren` (returns the `_calculatableChildren` list as `IEnumerable<ICalculatable>`, covariant, no IL); `WasSuspendedRegenerate` (static get and set on `EquationPresenter.WasSuspendedRegenerate`); `InCustomInitialization` (`_model` get and set through `ISupportCustomInitialization`); `CurrentIdLabel` and `CurrentDisplayIdLabel` (store the field, then if `GetValueOrDefault() == IdLabels.None` (value 6) clear it to null, then `FirePropertyChanged`, which is now implemented, as in batch 57); `CurrentNodeCaretInsertionType` (`_model.CurrentModeNavigatorState.CaretInsertionType`). Stub pairs removed from the `.g.cs` with indentation intact.
- Gate (project-level): PASS; 3659 checked, zero failures (`.mirror-cache/release-il-2f2593ce5e07/release-il.json`).

### Batch 59: Controls.EquationControl (Algorithm, AlgorithmOptions, EquationXmlNode)

- Exact (5 accessors): `Algorithm` (get `CurrentNode?.Algorithm`; set `CurrentNode.Algorithm = value`, with no null check, as in Mathcad); `AlgorithmOptions` (get `CurrentNode?.AlgorithmOptions`, the same null-safe shape as `Algorithm`); `EquationXmlNode` (get and private set through `_model`). Stub pairs removed from the `.g.cs` with indentation intact.
- Gate (project-level): PASS; 3664 checked, zero failures (`.mirror-cache/release-il-79dd70a5e32a/release-il.json`).

### Batch 60: Controls.EquationControl (CurrentNodeLabel, SelectedOperator)

- Exact (2 accessors): `CurrentNodeLabel` (`Mode == EquationEditModeEnum.Normal && NavigatorState.Current != null` returns the label, else `null`); `SelectedOperator` (step to the parent when the current node is `NoLogicOnlyVisual`, then return `OperatorSExpName`). Stub pairs removed from the `.g.cs` with indentation intact.
- Parked: `MatrixRows` and `MatrixColumns`. The IL is `(MatrixNodeInfo)(GetNearestAncestor("MATRIX") ?? throw ...).NodeInfo`, which Mathcad compiles as `dup; brtrue.s; ldstr; newobj; throw` with no `pop`. Current Roslyn emits `pop` before the throw, so the branch offsets differ by one instruction. The shape is not expressible with a throw expression in current C#; they stay as stubs.
- Left out: `CurrentNodeNodeSide` and `CurrentNodeCaretSide` (a nullable-enum local whose type the IL does not show in the excerpt); `CurrentNodeOperatorPartIndex` (a `Nullable<int>` local with `GetValueOrDefault(-1)`, its own probe).
- Gate (project-level): PASS; 3666 checked, zero failures (`.mirror-cache/release-il-ecf71741b347/release-il.json`).

### Batch 61: Controls.EquationControl (Rl)

- Exact (1 accessor): `Rl` (`if (AllowExpressionsOnly || !_isForEngineCalculation) return MathNodeToRlTransformer.Default.SetRlidAndTransform(_model.EquationRoot); return _rl;`; `Default` is a static readonly field, read with `ldsfld`). Stub pair removed from the `.g.cs`; the partial gains `using Quire.EquationEditor.Tree.Transform;`.
- Parked: `History` (`new List<string>(_model.VisualHistory).ToArray()` is the right source, but Mathcad compiles `ToArray` as `callvirt` on the non-virtual `List<string>.ToArray`, where current Roslyn emits `call`; the instruction differs, so it stays as a stub. The model member is `VisualHistory`, not `History`).
- Gate (project-level): PASS; 3667 checked, zero failures (`.mirror-cache/release-il-496362b95c57/release-il.json`).

### Batch 62: Controls.EquationControl (FirstPropertyForXamlSerialization, WarningsCount)

- Exact (3 accessors): `FirstPropertyForXamlSerialization` (get: `DebugTools.DevAssert(_isForEngineCalculation)` with the result discarded, then `return true`; set: `InCustomInitialization = value`, then `ReadDeserializationFormatsForPaste()`); `WarningsCount` (`_model.Warnings?.Count ?? 0`, using the `dup`/`brtrue` form). Stub pairs removed from the `.g.cs`.
- Runtime dependency (not an IL mismatch): `ReadDeserializationFormatsForPaste` is still a generated stub, so the `FirstPropertyForXamlSerialization` setter throws `NotImplementedException` until that method is implemented.
- Left out: `Warnings` (its getter's lambda is a closure with a compiler-assigned ordinal, `<get_Warnings>b__588_0`, so it needs a compile-order probe).
- Gate (project-level): PASS; 3670 checked, zero failures (`.mirror-cache/release-il-1e6211496e6e/release-il.json`).

### Batch 63: Controls.EquationControl (ReadDeserializationFormatsForPaste, DeserializeMathFormat, DeserializeLabeledIdFormat)

- Exact (3 accessors): `ReadDeserializationFormatsForPaste` (`DebugTools.DevAssert(...)` over `MathFormatForPaste.Instance` and its two document formats, `.Failed` checked for an early return, then sets `DeserializeMathFormat` and `DeserializeLabeledIdFormat` from the instance); `DeserializeMathFormat` setter (`_view.MathFormat.SetPropertiesFrom(value, true)` after the null guard); `DeserializeLabeledIdFormat` setter (`SetValue(AttachedProperties.LabeledIdFormatProperty, value)`, with the local typed `DependencyObject` as Mathcad's IL requires; `var` gave `VisualMathRegionNode` and failed). Stub pairs removed from the `.g.cs`.
- Runtime dependency (not an IL mismatch): `VisualMathNode.MathFormat` is still a generated stub, so the `DeserializeMathFormat` setter throws `NotImplementedException` at run time until that getter is implemented.
- Runtime dependency still parked: `GetColumnCells` (stloc/ldloc elision); its `DeleteEmptyColumns` dependency stays visible.
- Gate (project-level): PASS; 3673 checked, zero failures (`.mirror-cache/release-il-64a12096c843/release-il.json`).

### Batch 64: Controls.Internal.VisualMathNode (MathFormat)

- Exact (1 property, get and private set): `MathFormat` is a non-virtual auto-property (`{ get; private set; }`), in the hand-written partial `src/Quire.Controls.EquationControl/Mirrored/Controls.Internal.VisualMathNode.cs`. Mathcad's getter is `final`, so its callers use `call`; a `virtual` getter made Roslyn emit `callvirt` and failed 32 call-site comparisons. Stub pair removed from `Controls.Internal.VisualMathNode.g.cs`.
- Runtime dependency (not an IL mismatch): the constructor that assigns `MathFormat` (`new MathFormatDobjController(this)` plus the trigger/style setup) is still a no-op stub, so `MathFormat` stays null until the constructor is implemented. `DeserializeMathFormat` (batch 63) will now throw a `NullReferenceException` rather than `NotImplementedException` until then.
- Runtime dependency still parked: `GetColumnCells` (stloc/ldloc elision); its `DeleteEmptyColumns` dependency stays visible.
- Gate (project-level): PASS; 3675 checked, zero failures (`.mirror-cache/release-il-c5df7c9c77d6/release-il.json`).

### Batch 65: Controls.Internal.VisualMathNode (static dependency-property group, OnShowOperatorOnlyWhenActiveChanged)

- Exact (static initialization and both callbacks): `OperatorFontFamilyName`, `IsValidProperty`, `ShowAlignmentPointProperty` (`AffectsRender`), `ShowOperatorOnlyWhenActiveProperty` (`PropertyChangedCallback` to the static callback), and the static constructor that calls `AddOwner` on `EquationControl.ActiveElementOpacityProperty` and `ActiveNonNavigatableElementOpacityProperty` (each `pop`). Static callback `OnShowOperatorOnlyWhenActiveChanged(DependencyObject, DependencyPropertyChangedEventArgs)` (`as` plus null check, then the virtual call with `(bool)e.NewValue`) and the empty `protected virtual OnShowOperatorOnlyWhenActiveChanged(bool)`. `_alignmentPoint` is declared by hand (no IL). Stub pairs removed from the `.g.cs`.
- Not landed, IL-structural: the `VisualMathNode` constructor. Mathcad stores `_alignmentPoint = new Point(0, 0)` (`stfld`) before its call to the base `VisualNode` constructor, and Roslyn always calls the base constructor first. The first difference is at #0 (`call VisualNode::.ctor` vs `newobj Point`). No idiomatic C# form emits the field store before the base call. The constructor stays the generated stub, so `MathFormat` is still null at run time. The rest of the constructor body (Focusable, Trigger/Style, `MathFormatDobjController(this)`) is not verified, because the gate reports only the first difference.
- Runtime dependency (not an IL mismatch): `MathFormat` stays null until the constructor is matched. `DeserializeMathFormat` (batch 63) therefore throws `NullReferenceException` at run time. `IsValidProperty` is now initialized.
- Runtime dependency still parked: `GetColumnCells` (stloc/ldloc elision); its `DeleteEmptyColumns` dependency stays visible.
- Gate (project-level): PASS; 3678 checked, zero failures (`.mirror-cache/release-il-a930dc38eb76/release-il.json`).

### Batch 66: Controls.Internal.VisualMathNode (DepthBelowBaseline)

- Exact (1 getter): `DepthBelowBaseline` => `DesiredSize.Height - TextBaselineY` (`DesiredSize` is copied to a `Size` temporary before `get_Height`, which Roslyn emits for struct property access; `TextBaselineY` is a `callvirt`). Stub pair removed from the `.g.cs`; the body lives in the hand-written partial.
- Runtime dependency (not an IL mismatch): `VisualMathNode.TextBaselineY` is still a generated stub, so `DepthBelowBaseline` throws `NotImplementedException` at run time until `TextBaselineY` is implemented.
- Unchanged: the `VisualMathNode` constructor (IL-structural mismatch, see batch 65); `MathFormat` stays null at run time; `GetColumnCells` stays parked (`DeleteEmptyColumns`).
- Gate (project-level): PASS; 3679 checked, zero failures (`.mirror-cache/release-il-c2d03e88c747/release-il.json`).

### Batch 67: Controls.Internal.VisualMathNode (TextBaselineY)

- Exact (1 virtual getter): `TextBaselineY` (`public virtual`, returns `0.0`; Mathcad IL is `ldc.r8 0.0; ret`). Stub pair removed from the `.g.cs`; the body lives in the hand-written partial. Subclasses that override it (`VisualTextNode`, `VisualPlaceholderNode`, and others) still call `base.TextBaselineY` through the generated stubs, which are unchanged.
- Runtime dependency cleared: `DepthBelowBaseline` (batch 66) no longer throws at run time, because `TextBaselineY` is implemented.
- Unchanged: the `VisualMathNode` constructor (IL-structural mismatch, batch 65); `MathFormat` stays null at run time, so `DeserializeMathFormat` still throws `NullReferenceException`; `GetColumnCells` stays parked (`DeleteEmptyColumns`).
- Gate (project-level): PASS; 3680 checked, zero failures (`.mirror-cache/release-il-cb037d505f48/release-il.json`).

### Batch 68: Controls.Internal.VisualMathNode (IsValid, ShowAlignmentPoint, ShowOperatorOnlyWhenActive)

- Exact (6 accessors, 3 properties): each property is a non-virtual dependency-property wrapper (`get => (bool)GetValue(XProperty)`, `set => SetValue(XProperty, value)`) over the static DPs registered in batch 65. Stub pairs removed from the `.g.cs`; the bodies live in the hand-written partial.
- Unchanged: the `VisualMathNode` constructor (IL-structural mismatch, batch 65); `MathFormat` stays null at run time, so `DeserializeMathFormat` still throws `NullReferenceException`; `GetColumnCells` stays parked (`DeleteEmptyColumns`).
- Gate (project-level): PASS; 3686 checked, zero failures (`.mirror-cache/release-il-72e57c88afa8/release-il.json`).

### Batch 69: Controls.Internal.VisualMathNode (AlignmentPoint, ScreenCenterPoint)

- Exact (2 getters): `AlignmentPoint` (`=> _alignmentPoint`; `_alignmentPoint` is declared by hand, batch 65) and `ScreenCenterPoint` (`center = new Point(ActualWidth / 2, ActualHeight / 2)` as its own statement, then `return PointToScreen(center)`; the statement form emits the in-place constructor on a local before `this` is loaded, which is the Mathcad order). Stub pairs removed from the `.g.cs`.
- Runtime dependency: `AlignmentPoint` returns `_alignmentPoint`, which is `(0, 0)` until the constructor runs (constructor unmatched, batch 65).
- Probe, not landed: `ComboBoxTableModel.GetColumnCells` with a failure-last layout (`if (!DevAssert(...).Failed) { IEnumerable<IReadonlyCellData> cells = AllRows.Select(row => row.GetCellAt(columnIndex)); return cells; } return Enumerable.Empty<IReadonlyCellData>();`). The display class `<>c__DisplayClass26_0` and its lambda now match, and the bounds check matches. The only difference left is the `stloc.1; ldloc.1` pair before `ret`, which Roslyn elides in Release even with an explicitly typed local (33 entries against Mathcad's 35). It stays parked. Patch saved outside the repo.
- Unchanged: the `VisualMathNode` constructor (IL-structural mismatch, batch 65); `MathFormat` stays null at run time, so `DeserializeMathFormat` still throws `NullReferenceException`; `GetColumnCells` stays parked, and its `DeleteEmptyColumns` dependency stays visible.
- Gate (project-level): PASS; 3688 checked, zero failures (`.mirror-cache/release-il-6df6afd3b76e/release-il.json`).

### Batch 70: Controls.Internal.VisualMathNode (HideSubTreeFromClicks, HideFromClicks, IsMonoLine, NodeInfo)

- Exact (8 accessors, 4 members): `HideSubTreeFromClicks` and `HideFromClicks` are plain field accessors over `_hideSubNodesFromClicks` and `_hideFromClicks`, which are declared by hand; `IsMonoLine` is a virtual getter returning `false` and a virtual setter that throws `NotImplementedException` (Mathcad's own setter, so this is exact, not a stub); `NodeInfo` is a virtual auto-property with the compiler backing field `<NodeInfo>k__BackingField`. Stub pairs removed from the `.g.cs`.
- Not landed: `ScreenBounds` (its Mathcad IL stores a `Point` local between two `PointToScreen` calls and then passes it to the `Rect` constructor, the same elision risk as `GetColumnCells`).
- Unchanged: the `VisualMathNode` constructor (IL-structural mismatch, batch 65); `MathFormat` stays null at run time, so `DeserializeMathFormat` still throws `NullReferenceException`; `GetColumnCells` stays parked (`DeleteEmptyColumns`).
- Gate (project-level): PASS; 3696 checked, zero failures (`.mirror-cache/release-il-8b3ae334d189/release-il.json`).

### Batch 71: Controls.Internal.VisualMathNode (InkedHeight, ScreenInkBounds, NavigatableParts)

- Exact (3 members): `InkedHeight` (virtual getter returning `ActualHeight`); `ScreenInkBounds` (virtual getter returning `ScreenBounds`); `NavigatableParts` (virtual getter whose body is `throw new NotImplementedException()`, which is Mathcad's own behaviour, so it is exact and not a stub). Stub pairs removed from the `.g.cs`.
- Runtime dependency (not an IL mismatch): `ScreenInkBounds` calls `ScreenBounds`, which is still a stub, so it throws `NotImplementedException` at run time until `ScreenBounds` is implemented (its Mathcad IL carries the same stloc/ldloc elision risk as `GetColumnCells`).
- Not landed: `CoreElements` and `CoreElementsTopLine` (compiler iterator state machines `<get_CoreElements>d__43` and `d__45`; these need ordinal matching).
- Unchanged: the `VisualMathNode` constructor (IL-structural mismatch, batch 65); `MathFormat` stays null at run time, so `DeserializeMathFormat` still throws `NullReferenceException`; `GetColumnCells` stays parked (`DeleteEmptyColumns`).
- Gate (project-level): PASS; 3699 checked, zero failures (`.mirror-cache/release-il-d4c8ade78187/release-il.json`).

### Batch 72: Controls.Internal.VisualMathNode (SetOperatorFontProperties, FireMeasureOverideCompleted, HandleMathFormatChanged)

- Exact (3 members): `SetOperatorFontProperties` (protected static; sets `FontFamilyName` from `OperatorFontFamilyName` and `IsItalic`, `IsBold`, `IsUnderline` to `false` through `Nullable<bool>`); `FireMeasureOverideCompleted` (`if (MeasureOverideCompleted != null) MeasureOverideCompleted();` on the field-like event's backing field); `HandleMathFormatChanged` (virtual; `DevAssert(_font == null, null).Passed` guard, then clears `_font` unless the property is `HighlightColorProperty`). `_font` is declared by hand. Stub pairs removed from the `.g.cs`.
- Mismatch recorded (ScreenBounds, not landed): the natural form `new Rect(PointToScreen(new Point(0,0)), PointToScreen(new Point(ActualWidth, ActualHeight)))` fails with one difference (17 entries against Mathcad's 19). Mathcad keeps the bottom-right point in a `stloc.0`/`ldloc.0` pair before the `Rect` constructor, and Roslyn elides it in Release. Reordering the calls changes the call order, so it does not match either. `ScreenBounds` stays a stub, and `ScreenInkBounds` (batch 71) therefore throws `NotImplementedException` at run time.
- Not landed: `OperandChanged` (its instance overload is called with `callvirt` on a non-virtual method, which Roslyn emits as `call`); `CoreElements` and `CoreElementsTopLine` (iterator state machines).
- Unchanged: the `VisualMathNode` constructor (IL-structural mismatch, batch 65); `MathFormat` stays null at run time, so `DeserializeMathFormat` still throws `NullReferenceException`; `GetColumnCells` stays parked (`DeleteEmptyColumns`).
- Gate (project-level): PASS; 3702 checked, zero failures (`.mirror-cache/release-il-4de36e0c9fcd/release-il.json`).

### Batch 73: Controls.Internal.VisualMathNode (ModifyChildMathFormat, BuildChildren, ArrangeMathNode, MeasureMathNode, MeasureStaticLayout, ArrangeStaticLayout)

- Exact (6 members): `ModifyChildMathFormat` (virtual, empty body); `BuildChildren` (virtual; bottom-tested `for` loop that assigns `this[i] = children[i]`); `ArrangeMathNode` and `MeasureMathNode` (internal, single `UIElement.Arrange` and `Measure` calls); `MeasureStaticLayout` and `ArrangeStaticLayout` (protected virtual, `base.MeasureOverride` and `base.ArrangeOverride`). Stub pairs removed from the `.g.cs`.
- Runtime dependency (not an IL mismatch): `BuildChildren` assigns through the indexer `this[int]`, whose setter is still a stub, so `BuildChildren` throws `NotImplementedException` at run time until the indexer is implemented.
- Not landed: `ArrangeChild`. Its Mathcad IL calls `ArrangeMathNode` with `callvirt` on a non-virtual method, which Roslyn emits as `call`, so the shape differs. Left for a probe.
- Unchanged: the `VisualMathNode` constructor (IL-structural mismatch, batch 65); `MathFormat` stays null at run time, so `DeserializeMathFormat` still throws `NullReferenceException`; `ScreenBounds` stays a stub (batch 72 mismatch), so `ScreenInkBounds` still throws at run time; `GetColumnCells` stays parked (`DeleteEmptyColumns`).
- Gate (project-level): PASS; 3708 checked, zero failures (`.mirror-cache/release-il-40edb88dbfdc/release-il.json`).

### Batch 74: Controls.Internal.VisualMathNode (indexer this[int])

- Exact (get and set): the base indexer's accessors both `throw new ArgumentOutOfRangeException("index")`, which is Mathcad's own base behaviour (`ldstr "index"; newobj ArgumentOutOfRangeException(string); throw`). The stub pair is removed from the `.g.cs`; the body lives in the hand-written partial.
- Effect on the runtime dependency: `BuildChildren` (batch 73) no longer throws `NotImplementedException`. It now throws `ArgumentOutOfRangeException` for any node that does not override the indexer, which is how Mathcad's base node behaves. The subclass overrides (`VisualMatrixNode`, `VisualTransposeNode` and others) are still generated stubs, so their indexer and `BuildChildren` paths still throw `NotImplementedException` until those overrides are mirrored.
- Unchanged: `ScreenBounds` stays a stub (batch 72 mismatch), so `ScreenInkBounds` still throws at run time; the `VisualMathNode` constructor is unmatched (batch 65), so `MathFormat` stays null and `DeserializeMathFormat` still throws `NullReferenceException`; `GetColumnCells` stays parked (`DeleteEmptyColumns`).
- Gate (project-level): PASS; 3710 checked, zero failures (`.mirror-cache/release-il-6e0cf582db3d/release-il.json`).

### Batch 75: VisualLinearOutfixNode and VisualLinearUnaryNode indexers

- Exact (4 accessors): `VisualLinearOutfixNode.this[int]` (`if (index == 0)` returns or sets `Child`; otherwise `base[index]`); `VisualLinearUnaryNode.this[int]` (same shape with `((VisualUnaryOperatorNodeForEditor)this).Child`, which binds the `callvirt` to the base accessor, the existing technique in `VisualLinearUnaryNode.cs`). The `base[index]` calls match Mathcad's `call VisualMathNode::set_Item`. Stub pairs removed from both `.g.cs` files.
- Correction to the batch 74 notes: the subclass indexer overrides are `[MirrorStub]` members whose generated body forwards to `base[index]`. They are not `NotImplementedException` throws. Forwarding now reaches the base indexer, which throws `ArgumentOutOfRangeException("index")`.
- Still stubs (forward to base): `VisualNthRootOperatorNode.this[int]` and `VisualDerivativeOperatorNodeBase.this[int]`. Their Mathcad setters carry descriptor-based error messages and need a fuller IL read before they can match.
- Runtime dependency: `BuildChildren` on the linear outfix and unary nodes now uses the implemented indexers (`Child` is set when `index == 0`). Other indexes still throw `ArgumentOutOfRangeException` through the base.
- Unchanged: `ScreenBounds` stays a stub (batch 72 mismatch), so `ScreenInkBounds` still throws; the `VisualMathNode` constructor is unmatched (batch 65), so `MathFormat` stays null and `DeserializeMathFormat` still throws `NullReferenceException`; `GetColumnCells` stays parked (`DeleteEmptyColumns`).
- Gate (project-level): PASS; 3714 checked, zero failures (`.mirror-cache/release-il-2ea5a0ee5497/release-il.json`).

### Batch 76: EquationControl+EquationControlChildCalculatable (RlIndex)

- Exact (2 accessors): `RlIndex` is an auto-property of the nested `private partial class EquationControlChildCalculatable` (`<RlIndex>k__BackingField`; get loads the field, set stores it). The hand-written partial is `src/Quire.Controls.EquationControl/Mirrored/Controls.EquationControl.EquationControlChildCalculatable.cs`. Stub pair removed from the `.g.cs`.
- Still open (not landed): the `VisualNthRootOperatorNode` and `VisualDerivativeOperatorNodeBase` indexer setters (batch 75 notes). Their Mathcad bodies compute the scaled font size on the evaluation stack and store a `Nullable<double>` and a `double` in the reverse order (`stloc.1`/`stloc.2`) inside a `DevAssert` expression. The natural C# order is different, so these need a stack-order probe before they can match. Their getters are not useful alone: `_nodeChildren` is set only by the unmatched constructors.
- Parked and unchanged: `ComponentCookieMultiThreaded`, `History`, `Warnings`, `MatrixRows`, `MatrixColumns`, and the `CurrentNode*` group (see batches 60 to 65).
- Runtime dependencies still visible: `ScreenBounds` stub (`ScreenInkBounds` throws); the `VisualMathNode` constructor unmatched (`MathFormat` null, `DeserializeMathFormat` throws `NullReferenceException`, `AlignmentPoint` is `(0, 0)`); `GetColumnCells` parked (`DeleteEmptyColumns`); the NthRoot and Derivative indexer setters forward to base and throw `ArgumentOutOfRangeException`.
- Gate (project-level): PASS; 3716 checked, zero failures (`.mirror-cache/release-il-1bb728559d86/release-il.json`).

### Batch 77: Controls.EquationControl (OnSExpressionChanging)

- Exact (1 method): `OnSExpressionChanging(string)` is a public method that null-checks the field-like `SExpressionChanging` event and invokes it (the same shape as `FireMeasureOverideCompleted`). It is in the hand-written partial `Mirrored/Controls.EquationControl.cs`, and its stub pair is removed from the `.g.cs`.
- Still open: `EquationSExpression`. The getter is an exact forward to `_model.EquationSExpression`, but the setter depends on `FireRequestEvaluation`, which uses compiler-cached `<>c` lambdas (`<>9__430_0`, `<FireRequestEvaluation>b__430_0/1`) and needs ordinal matching. A property cannot be half hand-written, so the whole property stays a stub.
- Resumed-probe status after the machine restart: the NthRoot setter probe patch is preserved in scratchpad (`batch77_nthroot_setter_probe.patch`); it still fails one entry (stack order of the scaled font size), so it stays unmatched. The `VisualNthRootOperatorNode` and `VisualDerivativeOperatorNodeBase` indexer setters forward to base and throw `ArgumentOutOfRangeException`.
- Runtime dependencies still visible: `ScreenBounds` stub (`ScreenInkBounds` throws); the `VisualMathNode` constructor unmatched (`MathFormat` null, `DeserializeMathFormat` throws `NullReferenceException`, `AlignmentPoint` is `(0, 0)`); `GetColumnCells` parked (`DeleteEmptyColumns`); `EquationSExpression` setter stub.
- Gate (project-level): PASS; 3717 checked, zero failures (`.mirror-cache/release-il-78fb53559a0b/release-il.json`).

### Batch 78: Controls.EquationControl (scope, clipboard, navigation and Pi commands)

- Exact (24 members): one-line forwarders to the `_view` (`VisualMathRegionNode`) fire methods, with the same `ldarg.0; ldfld _view; callvirt Fire*; ret` shape: `Cut`, `Copy`, `Paste` (FireCut, FireCopy, FirePaste), `Backspace` (FireDeleteLeft), `Delete` (FireDeleteRight), `ContractScope`, `ExpandScope`, `ExpandScopeLeft` (FireScopeLeft), `ExpandScopeRight` (FireScopeRight), `ExitGroupOnRight`, `FullyExpandScope`, `FullyContractScope`, `TransformCharacter` (FireGreek), `NavigateLeft`, `NavigateLeftByTerm`, `NavigateRight`, `NavigateRightByTerm`, `NavigateUp`, `NavigateDown`, `NavigateRegionEnd`, `NavigateRegionStart`, `ScopeHome` (FireMoveToStart), `ScopeEnd` (FireMoveToEnd); and `Pi`, which calls `AutomateKeypress('π')` (Mathcad's `ldc.i4 960`). Stub pairs removed from the `.g.cs`; the bodies are in the hand-written partial `Mirrored/Controls.EquationControl.cs`.
- Runtime dependency (not an IL mismatch): these forward into the `VisualMathRegionNode` Fire methods, which are still generated stubs, so each command throws `NotImplementedException` at run time until its target is implemented.
- Not landed: `OnCopy` and `OnCut` (not present as stubs in the generated file), `CanNavigate` and `CanNavigateUp`/`CanNavigateDown` (their `CanEx...` and model-argument shapes need a check first).
- Unchanged: `EquationSExpression` setter (needs `FireRequestEvaluation`); the `VisualMathNode` constructor unmatched (`MathFormat` null, `DeserializeMathFormat` throws `NullReferenceException`); `ScreenBounds` stub; `GetColumnCells` parked (`DeleteEmptyColumns`); NthRoot and Derivative indexer setters forward to base and throw `ArgumentOutOfRangeException`.
- Gate (project-level): PASS; 3741 checked, zero failures (`.mirror-cache/release-il-d46bde79b95b/release-il.json`).

### Batch 79: Controls.EquationControl (CanX predicates, InsertUnitMonomial, OnTraverse pair)

- Exact (12 members): the eight private `CanCopy`, `CanCut`, `CanPaste`, `CanPerformColumnOperations`, `CanPerformIdLabelOperations`, `CanPerformLinearNaryOperations`, `CanPerformRowOperations`, `CanNavigateToOppositeOfScope`, each forwarding `(sender, e)` to the hand-written `_view.FireCanX`; `InsertUnitMonomial` (forwards to `FireInsertUnitMonomial`); `OnTraverse(ExecutedRoutedEventArgs, Direction)` (forwards to `FireTraversalRequest`); and `OnTraverseBackward`/`OnTraverseForward`, which pass `Direction.Left` (0) and `Direction.Right` (1) as the Mathcad IL does (`ldc.i4.0` and `ldc.i4.1`). Stub pairs removed from the `.g.cs`; the bodies are in `Mirrored/Controls.EquationControl.cs`.
- Fire-path status (the runtime dependency the coordinator asked for): the `VisualMathRegionNode` Fire methods are hand-written, not stubs (batch 78 notes were corrected). They only act when an event has subscribers, and nothing subscribes yet, because `HookupToEvents` and `BindEditingCommands` are still stubs. Once those are implemented, the path reaches `BuildAndExecCutCmd` and `BuildAndExecPasteCmd` (both still stubs, 25 IL instructions each, private to `VisualMathRegionNode`), which throw `NotImplementedException`.
- Not landed: `BuildAndExecCutCmd` and `BuildAndExecPasteCmd` (not yet probed); `EquationSExpression` setter (needs `FireRequestEvaluation`'s compiler-cached lambdas).
- Unchanged: the `VisualMathNode` constructor unmatched (`MathFormat` null, `DeserializeMathFormat` throws `NullReferenceException`); `ScreenBounds` stub; `GetColumnCells` parked (`DeleteEmptyColumns`); NthRoot and Derivative indexer setters forward to base and throw `ArgumentOutOfRangeException`.
- Gate (project-level): PASS; 3753 checked, zero failures (`.mirror-cache/release-il-de5d0f5f6a3d/release-il.json`).

### Batch 80: Controls.EquationControl (HookupToEvents)

- Exact (1 method): `HookupToEvents` subscribes the 14 model events and the view's `VisualParentChanged` event with `+=` in Mathcad's order (`ldftn` plus `newobj Action` plus `add_X`, the same shape Roslyn emits for a method-group `+=`). Stub pair removed from the `.g.cs`; the body is in `Mirrored/Controls.EquationControl.cs`.
- Run-time risk (not an IL mismatch): the handlers it subscribes to are still stubs (`OnModelRequestEvaluation`, `FocusWhenAble`, `HandleScopeChanged`, `HandleNavigatorStateChanged`, `OnViewVisualParentChanged` and the others). Nothing calls `HookupToEvents` yet (the constructor is still a stub), so there is no run-time effect today. Once the constructor is implemented, any model event raises a stub handler and throws `NotImplementedException`. The two must land together, or the handlers first.
- Still open on the command-subscription path: `BindEditingCommands`, `BindCommands` and `UnBindEditingCommands` (stubs); `BuildAndExecCutCmd` and `BuildAndExecPasteCmd` (stubs with three-lambda display classes, closure-ordinal matching needed).
- Unchanged: the `VisualMathNode` constructor unmatched (`MathFormat` null, `DeserializeMathFormat` throws `NullReferenceException`); `EquationSExpression` setter (needs `FireRequestEvaluation`); `ScreenBounds` stub; `GetColumnCells` parked (`DeleteEmptyColumns`); NthRoot and Derivative indexer setters.
- Gate (project-level): PASS; 3754 checked, zero failures (`.mirror-cache/release-il-28c078653dcb/release-il.json`).

### Batch 81: Controls.EquationControl (ten subscribed handlers)

- Exact (10 members, the small handlers `HookupToEvents` subscribes): `OnModelRequestEvaluation` (`FireRequestEvaluation(true)`); `FireRequestRemove` (`RequestRemove.Invoke(this, direction)`, no null check, as the IL has none); `OnModelEquationChanged` (`_model_EquationChanged()`); `HandleModelIsRestrictedReadonlyChanged` (true unbinds, false binds); `HandleNumericResultChanged` (`UpdateEngineResult(result)` then `ResultChanged?.Invoke`); `FireExpressionValueChanged` (null-checked invoke with `this`); `HandleCalculationErrorChanged` (`UpdateEngineResult(_model.CalculationError)`, `FirePropertyChanged(ErrorChangedEventArgs.GetArgs())`, `UpdateLhsVisibleOnView()`; the argument is unused in the IL); `FireContentChangedEvents` (`FireContentAndRegionChangedEvents(IORegionChangeType.Content)`); `OnViewVisualParentChanged` (returns when `newParent` is null, otherwise `OnAdornedElementReady(this, null)`); `HandleVisibleLhsOfDefinitionChanged` (`FirePropertyChanged(new PropertyChangedEventArgs("VisibleLhsOfDefinition"))`, `FireContentChangedEvents()`, `UpdateLhsVisibleOnView()`). Stub pairs removed from the `.g.cs`; bodies in `Mirrored/Controls.EquationControl.cs`.
- Run-time dependencies (explicit, not IL mismatches): these handlers call stubs, so they still throw `NotImplementedException` when reached: `UpdateEngineResult` (all four overloads), `UpdateLhsVisibleOnView`, `FireRequestEvaluation`, `_model_EquationChanged`, `FireContentAndRegionChangedEvents`, `BindEditingCommands`, `UnBindEditingCommands`. `HookupToEvents` (batch 80) is still uncalled, so nothing reaches these yet.
- Still open: the five larger subscribed handlers (`FocusWhenAble` 52, `HandleNavigatorStateChanged` 88, `HandleScopeChanged` 107, `HandleTypeoverChanged` 39, `HandleWarningOccured` 21); `BindEditingCommands`, `BindCommands`, `UnBindEditingCommands`; `BuildAndExecCutCmd` and `BuildAndExecPasteCmd`.
- Unchanged: the `VisualMathNode` constructor unmatched (`MathFormat` null); `EquationSExpression` setter; `ScreenBounds` stub; `GetColumnCells` parked; NthRoot and Derivative indexer setters.
- Gate (project-level): PASS; 3764 checked, zero failures (`.mirror-cache/release-il-fe0a43c2a62f/release-il.json`).

## Batch 82

- Owner: haiku-equationcontrol-20261123; branch feature/haiku-equationcontrol-20261123 (base main e769dd8fb).
- Members: `HandleTypeoverChanged` (39 IL) implemented in `Mirrored/Controls.EquationControl.cs`; stub pair removed from `Generated/Mirror/Controls.EquationControl.g.cs`.
- Release IL: PASS, 3765 checked, zero failures (`gate_b82b`).
- Remaining mismatches / blockers: `HandleWarningOccured` (21 IL) is left as a stub: its `Warnings?.CollectAsList(w => w.Item1.Original)` lambda is `<HandleWarningOccured>b__674_0` against Mathcad `b__400_0`. The ordinal counts every member in the class including generated stubs (validation.md), so it is parked with the other closure ordinals.
- Runtime dependency: `HandleTypeoverChanged` calls `BringCurrentNodeIntoView` and `GetFormatForMatrix` (both stubs), so typeover changes will still throw until those land. `HandleWarningOccured` calls `UpdateEngineResult(IEnumerable<EngineRedefInfo>)`, which is also a stub.

## Batch 83

- Owner: haiku-equationcontrol-20261123; branch feature/haiku-equationcontrol-20261123 (on batch 82, base main e769dd8fb).
- Members: `HandleNavigatorStateChanged` (88 IL) implemented in `Mirrored/Controls.EquationControl.cs`; stub pair removed from the `.g.cs`. No lambda, so no ordinal issue.
- Release IL: PASS, 3766 checked, zero failures.
- Runtime dependency: `GetNestedPositionForMatrix`, `AreTableCoordinateListsEqual`, `IncreaseEquationWidthIfNessessary`, `BringCurrentNodeIntoView` and `GetFormatForMatrix` are still stubs. `HookupToEvents` is not called yet, so these handlers are not reached at run time.
- Remaining: `HandleScopeChanged` (107 IL) uses `<>c__DisplayClass403_0`/`b__0`, so it is parked on the closure ordinal like `HandleWarningOccured`. `FocusWhenAble` has two closures (`DisplayClass428_0`), also parked.

## Batch 84

- Owner: haiku-equationcontrol-20261123; branch feature/haiku-equationcontrol-20261123 (on batch 83).
- Members: `_model_EquationChanged`, `FireContentAndRegionChangedEvents` (IORegionChanged null-check idiom), `OnVisibleWarningMessageChanged`, `ClearExpressionValueModified`; stub pairs removed from the `.g.cs`.
- Release IL: PASS, 3770 checked, zero failures.
- Remaining: `UpdateLhsVisibleOnView` (106 IL, nested bool/Visibility logic) and the `UpdateEngineResult` overloads are still stubs; `FireContentChangedEvents` and the `OnModelEquationChanged` path now reach the implemented `FireContentAndRegionChangedEvents` and `_model_EquationChanged`.

## Batch 85

- Owner: haiku-equationcontrol-20261123; branch feature/haiku-equationcontrol-20261123 (on batch 84).
- Members: the four `UpdateEngineResult` overloads (`SynchronizationStatus`, `IResultNode`, `EngineError`, `IEnumerable<EngineRedefInfo>`). Each copies `EngineResult` to a local before the constructor call, which the Release IL shows.
- Release IL: PASS, 3774 checked, zero failures.
- Remaining: `UpdateLhsVisibleOnView` (106 IL) is a stub. `HandleWarningOccured` (lambda ordinal) and `HandleScopeChanged`/`FocusWhenAble` (closures) stay parked.

## Batch 86

- Owner: haiku-equationcontrol-20261123; branch feature/haiku-equationcontrol-20261123-next (on batch 85 tip e02337274, base main 22371b2e5).
- Members: `BindCommands`, `BindEditingCommands`, `UnBindEditingCommands`; stub pairs removed from the `.g.cs`.
- Release IL: PASS, 3777 checked, zero failures.
- Runtime dependency: the binding members call `EditableCommandBindings` and `NonEditableCommandBindings`, which are still stubs, so binding throws until those land. Nothing calls `BindCommands` yet.
- Parked: `UpdateLhsVisibleOnView` (106 IL). Its first condition is materialized as a bool (`ldc.i4.1`/`ldc.i4.0`) and tested after the second, which the natural ternary chain does not reproduce. Probe at scratchpad `updatelhs_probe.patch`.

## Batch 87

- Owner: haiku-equationcontrol-20261123; branch feature/haiku-equationcontrol-20261123 (on main c83700b81).
- Members: `AreTableCoordinateListsEqual` (static helper, 78 IL) implemented; stub pair removed from the `.g.cs`. No lambda.
- Release IL: PASS, 3778 checked, zero failures.

## Batch 88

- Members: `GetNestedPositionForMatrix` (193 IL) implemented; stub pair removed. No lambda. The count test is `Count != 0`, which Roslyn emits as the single `brfalse` the IL uses.
- Release IL: PASS, 3779 checked, zero failures.

## Batch 89

- Members: `GetFormatForMatrix` (136 IL) implemented; stub pair removed. No lambda. The `??` ancestor lookup and the foreach early return match.
- Release IL: PASS, 3780 checked, zero failures.

## Batch 90

- Members: `EditableCommandBindings` (378 IL) implemented as `??=` over a `CommandBindingCollection` initializer (37 bindings); stub pair removed. Gate PASS, 3781 checked.
- Parked: `NonEditableCommandBindings` (1213 IL). Its `Undo`/`Redo` handlers are cached in `<>O` as `<1>__`/`<3>__` in Mathcad, because the constructor caches `EngineErrorMessages.GetDisplayMessage` as `<0>__`. The constructor is blocked by its lambda ordinals (`<.ctor>b__152_*`), so our cache index is 0 and does not match. Also blocked on the constructor are the other constructor-only cache names.

## Batch 91

- Owner: haiku-equationcontrol-20261123; branch `haiku-equationcontrol-20261123-b91` (from main 34b5cc435).
- Compile order: every outer-class member now lives in the hand-written partial in Mathcad order (unmarked `throw` placeholders for unfinished members; `Generated/` keeps only the nested-type shells). The constructor ordinal is 152 (`<.ctor>b__152_0`, `<>c::<.ctor>b__152_1/2`), measured on the built assembly.
- Members: the model constructor `.ctor(IEquationModel)` with its three lambdas, and the field initializers it runs (`_rl`, `_calculatableChildren`, `_isSymbolicEvalKeywordsShown`, `_isForEngineCalculation`, the `RequestRemove` and `OnGotFocusEvent` `delegate { }` initializers).
- The `.ctor(string)` and `.ctor(string, bool)` stubs are `MirrorBlocked` with `: this()` / `: this(sExpression)` chains; without the chains their field initializers duplicate the lambdas.
- Release IL: PASS, 3787 checked, zero failures (`.mirror-cache/release-il-ba9fbc30c4ab/release-il.json`).
- Remaining: `UpdateLhsVisibleOnView` (IL shape); the closure-gated stubs listed in batches 83-86; 1211 generated stubs in total.

## Batch 92

- Owner: haiku-equationcontrol-20261123; branch `haiku-equationcontrol-20261123-b92` (stacked on batch 91 `9877dd373`).
- Members: `NonEditableCommandBindings` implemented as `??=` over a 33-binding `CommandBindingCollection` initializer, the same shape as `EditableCommandBindings`; stub removed from the `.g.cs`.
- Release IL: PASS, 3788 checked, zero failures (`.mirror-cache/release-il-4a7c1bad2243/release-il.json`).

## Batch 93

- Owner: haiku-equationcontrol-20261123; branch `haiku-equationcontrol-20261123-b93` (from main 46107d34f).
- Members: `FocusWhenAble` (closure class `DisplayClass428_0`) and `FireRequestEvaluation` (closures `b__430_0/1`), both exact.
- Layout: the four ambiguous fields `_isForEngineCalculation`, `_matrixResizeAdorner`, `_temporaryResultMatrixFormatShouldClear`, `_nestedMatrixPositionWhichFormatShouldClear` now sit after `FireRequestEvaluation`; this is the placement the closure ordinals of `FocusWhenAble` (428) and `FireRequestEvaluation` (430) need.
- Release IL: PASS, 3795 checked, zero failures (`.mirror-cache/release-il-29d4061f4f6b/release-il.json`).
- Open: `HandleWarningOccured` measures closure 401 against Mathcad 400 (one non-method member too many before it, in the 275-297 band); kept as a stub. `HandleScopeChanged`, `IncreaseEquationWidthIfNessessary`, `BringCurrentNodeIntoView`, `Warnings` (588) and `UpdateLhsVisibleOnView` are not implemented yet.

## Batch 94

- Owner: haiku-equationcontrol-20261123; branch `haiku-equationcontrol-20261123-b94` (from main 1555c7d21, rebased onto main before landing).
- Members: `HandleWarningOccured` (closure `b__400_0`), `HandleScopeChanged` (`DisplayClass403_0`), `IncreaseEquationWidthIfNessessary` (`DisplayClass409_0`), `BringCurrentNodeIntoView` (`DisplayClass417_0`, `b__0/1`), `Warnings` (`b__588_0`). All exact.
- Layout: `_currentNumberFormat` moved after `BringCurrentNodeIntoView` (closure ordinals need it there); `_isForEngineCalculation` and `_matrixResizeAdorner` moved between `EditableCommandBindings` and `SymbolicEvalKeywordsVisibilityState`; `_temporaryResultMatrixFormatShouldClear` and `_nestedMatrixPositionWhichFormatShouldClear` moved after `Warnings`.
- Shapes: `BringCurrentNodeIntoView` uses a positive `if (IsKeyboardFocusWithin)` block so the display class is created after the check; `IncreaseEquationWidthIfNessessary` uses a guard `if (!IsShrunk) return;` and `DesiredSize.Width` without a named `Size` local.
- Open: `UpdateLhsVisibleOnView` stays a stub. The Mathcad IL keeps the `num` value on the evaluation stack while `flag` is computed (no local for `num`); both the named-local form and the reference ternary give a Release IL difference, so it is not landed in this batch.
- Release IL: PASS, 3809 checked, zero failures (`.mirror-cache/release-il-5c2accdf8924/release-il.json`).

## Batch 95

- Owner: haiku-equationcontrol-20261123; branch `haiku-equationcontrol-20261123-b95` (from main cba91d406).
- Members (closure-free, implemented in place, no layout change): `HighlightedOperands`, `CurrentNodeNodeSide`, `CurrentNodeOperatorPartIndex`, `CurrentNodeCaretSide`, `CurrentNodeSExpression`, `ActiveMatrixCell`, `MatrixRows`, `MatrixColumns`, `IsMatrixResized`, `Commit`, `SetSynchronizationStatus`, `SetCalculatingStatus`, `SetResult`, `SetError`, `SetScriptingEngineError`, `SetIdentifierLabels`, `SetAutoSelectAlgorithm`, `SetRedefinitionWarnings`, `FireCalculatableAdded/Removed/Moved`, `SetMatrixOrigin`, `IncrementComponentCookieMultiThreaded`, `FireAlgorithmChanged`, `IsSuitableForInputTaggingPrivate`, `IsSuitableForOutputTaggingPrivate`, `ActivateAndScopeIdentifierPrivate`, `GetDefinitionSExpression`, `ToggleInput`, `SetContextMenu`, `HandleRequestChangeToTextBox`, `IsChangeToTextBoxReady`, `ActivateEquationForComboBoxNode`, `OnViewDeActivated`, `OnViewOnActivated`, `HandleRequestToAllowContextMenu`, `FireMeasureOverideCompleted`, `OnRootNodeChanged`, `FireRootNodeChanged`, `OnTraversedFocusEvent`, `StartResizingMatrix`, `StopResizingMatrix`, `HandleExpressionValueChanged`, `ViewOnIsVisibleChanged`, `AnimateShow`, `AnimateHide`, `GetRegionHistoryCount`, `UnhookFromModelEvents`, `DetachFromModel`, `FireRequestEvaluationControlOnly`, `RemoveMatrixResizeAdorner`, `RemoveMatrixResizeAdornerIfNotNeeded`, `NavigateLeftIfNeccessary`.
- Shapes: `MatrixRows`/`MatrixColumns` use a null-check block (the `?? throw` form differs); `NavigateLeftIfNeccessary` uses three guard returns; `FireCalculatable*` and `FireMeasureOverideCompleted` keep the null-check `if` form.
- Release IL: PASS, 3862 checked, zero failures (`.mirror-cache/release-il-735bcece9896/release-il.json`).
- Open: `FocusViewAndUpdateNavigatorStateIfNeeded` (local function), `CreateWithTextInput`, `GetRegionXmlNode`, `SetRegionXmlNode`, `GetRegionPersistentData`, `SetRegionPersistentData`, and the remaining ~220 placeholders are not yet attempted.

## Batch 96

- Owner: haiku-equationcontrol-20261123; branch `haiku-equationcontrol-20261123-b96` (from main 2a3e909e3).
- Members: command handlers `OnCopy`, `OnCut`, `OnPaste`, `OnBackspace`, `OnDelete`, `OnContractScope`, `OnExpandScope`, `OnFullyContractScope`, `OnFullyExpandScope`, `OnReplace`, `OnInsertPi`, `OnInsertInfinity`, `OnInsertOperator`, `OnSwitchToLineBreakOperator`, `OnInsertEvaluation`, `OnInsertSymbolicEvaluation`, `OnInsertValue`-not included; `OnInsertString`, `OnInsertKeyword`, `OnInsertModifier`, `OnToggleSubscript`, `OnToggleVisibleLhsOfDefinition`, `OnNavigateUp/Down/Left/Right`, `OnToggleUnitIdLabel`, `OnToggleIdLabel`, `OnUnsetIdLabel`, `OnSetIdLabel`, `OnToggleHideFromCalculationContext`, `OnInsertRow`, `OnInsertMatrixColumn`, `OnDeleteRow`, `OnDeleteMatrixColumn`, `OnClearCells`, `OnSplitLinearNaryLine`, `OnRegionHistory`, `OnScopeLeft/Right`, `OnTransformCharacter`, the `OnNavigateTo*` and `OnNavigateLeft/RightByTerm` handlers, `OnConvertToOperator`, `PostDeleteNodeAction`; predicates `CanNavigateUp/Down/Left/Right`, `CanClearMathFormat`, `CanSwitchToLineBreakOperator`, `CanInsertInputControl`, `CanInsertFunction`, `CanInsertKeywordOrModifier`, `CanReplace`, `CanNavigate`, `CanReturn`, `CanTraverse`; `ExecuteClearMathFormat`, `ExecuteIncreaseFontSize`, `ExecuteDecreaseFontSize`, `ExecuteChangeFontColor`, `ExecuteChangeHighlightColor`.
- Shapes: `OnInsertOperator`, `OnInsertKeyword`, `OnInsertModifier` use guard returns (`if (!(e.Parameter is string n)) return;`, `if (text.IsNullOrEmpty()) return;`); the Release IL requires the guard form.
- Release IL: PASS, 3929 checked, zero failures (`.mirror-cache/release-il-070f6ecb5c93/release-il.json`).
- Open: `OnCommit`, `OnSetNumberFormatField`, `OnSetMatrixFormatField`, `OnInsertInputControl`, `OnInsertMatrix`, `OnInsertId`, `OnInsertFunction`, `OpenComboBoxEditingMode`, `CanClearResultFormat`, `CanToggleVisibleLhsOfDefinition` (cast-shaped bodies), not yet attempted or not matched.

## Batch 97

- Owner: haiku-equationcontrol-20261123; branch `haiku-equationcontrol-20261123-b97` (from main 2fcc0387d).
- Members: `ScopeTo`, `ScopeLhs`, `Keypress`, `SetResultMatrixSize`, `SetResultMatrixOffset`, `ToggleSymbolicKeywordsVisibility`, `IsAlgorithmSupportOptions`, `EditNodeOnCaretSide`, `EditNodeOnNodeSide`, `IsResultMatrixFormatIndexVisible`.
- Release IL: PASS, 3939 checked, zero failures (`.mirror-cache/release-il-eb9908eaaefa/release-il.json`).
- Open: `FixCaretToRightMostPositionAfterClipping` stays a stub (one extra instruction in the Mathcad body, not matched by the temp-local or inline forms); `XFromNavigatorState`, `ToggleAlgorithmOption` (nested-if shape), and the rest of the remaining placeholders are not yet attempted.

## Batch 98

- Owner: haiku-equationcontrol-20261123; branch `haiku-equationcontrol-20261123-b98` (from main 711dad224).
- Members: properties `SymbolicEvalKeywordsVisibilityState`, `VisibleMatrixCellRange`, `EquationSExpression`; handlers `CanPerformCearCellsOperation`, `CanTraverseForward`, `CanTraverseBackward`, `CanToggleVisibleLhsOfDefinition`, `OnClearResultFormat`, `OnInsertFunction` (guard form on `as`), `OnInsertMatrix`, `OnInsertInputControl`, `OpenComboBoxEditingMode`.
- Parked: `CanClearResultFormat` (Mathcad reads `DisplayNumberFormatProperty` without the `DependencyObject` cast that the source needs), `OnInsertId` (the Mathcad null-check order, `p != null && p.Name != null`, has not matched in three shapes).
- Release IL: PASS, 3953 checked, zero failures (`.mirror-cache/release-il-e30b9215a3eb/release-il.json`).
