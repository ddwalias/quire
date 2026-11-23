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
