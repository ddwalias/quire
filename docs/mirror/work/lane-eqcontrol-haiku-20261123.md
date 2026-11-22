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
