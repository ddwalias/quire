# EquationControl lane checkpoint: visualInputScriptable group

- Owner: dispatched worker `task_2585780f24e4` (Orca session `orca_session_id:claude_32e98c9e_919d_4823_b7ed_5529f2b64c32`)
- Branch: `feature/eq-visualinputscriptable`, worktree `C:/Users/DuyDang/orca/workspaces/Quire/eq-visualinputscriptable`
- Files: `src/Quire.Controls.EquationControl/Generated/Mirror/` `Controls.Internal.VisualInputScriptableControlNode.g.cs`, `Controls.Internal.VisualInputScriptableControlNodeBase.g.cs`, `Controls.Internal.VisualInputScriptableControlNodeEditor.g.cs`
- Reference: `PYTHONUTF8=1 python tools/reference/reference.py Ptc.<Type> --assembly Ptc.Controls.EquationControl --il` (Release IL; Mathcad `Ptc.` names)

## Classification

| Type | Stub | Status | Reason |
|---|---|---|---|
| VisualInputScriptableControlNode | `TypeOfThis`, `_visual`, `xamlBorder`, `xamlGrid`, `xamlToggle`, `xamlScrollViewer`, `_contentLoaded` | unblocked | field declarations move to hand-written partial |
| VisualInputScriptableControlNode | `.cctor` | unblocked | sets `TypeOfThis`, calls `CommonRoutedEvents.ContentChangedEvent.AddOwner(TypeOfThis)` |
| VisualInputScriptableControlNode | `.ctor()` | unblocked | calls `InitializeComponent()` |
| VisualInputScriptableControlNode | `.ctor(string, MathNode, VisualMathRegionNode)` | unblocked | initializes fields, event subscriptions, calls `init()`, `InitializeComponent()` |
| VisualInputScriptableControlNodeEditor | `BORDER_THICKNESS`, `_caret`, `_border`, `CaretStateProperty` | unblocked | field declarations and initializers move to hand-written partial |
| VisualInputScriptableControlNodeEditor | `CaretState` get/set | unblocked | dependency property wrapper |
| VisualInputScriptableControlNodeEditor | `.ctor(VisualMathNode, VisualMathRegionNode)` | unblocked | calls base ctor, initializes `_caret`, `_border`, adds to `VisualChildren`, `base.Add()` |
| VisualInputScriptableControlNodeEditor | `OnContextMenuOpening` | unblocked | plain IL (`_ = ContextMenu;`) |
| VisualInputScriptableControlNodeEditor | `OnCaretColorChanged`, `UpdateCaretBlinking` | unblocked | plain IL (`WpfUtils.CreateSolidWpfBrush`, `WpfUtils.Blink`) |
| VisualInputScriptableControlNodeEditor | `MeasureOverride`, `ArrangeOverride`, `OnPreviewKeyDown`, `GetDesiredTransform` | unblocked | plain IL |
| VisualInputScriptableControlNodeEditor | `HandleCaretPositionChanged`, `OnCaretStateChanged` | unblocked | plain IL (switch on `CaretState`) |
| VisualInputScriptableControlNodeBase | Instance fields (`_availableWidthCache`, `_availableHeightCache`, `_contextMenu`, `_eq`, `_headingsVisibility`, `_effectiveBaselineY`, `_matrixInfo`, `_mathNode`, `_rowHeading`, `_columnHeading`, `_rowIndices`, `_columnIndices`, `_leftBracket`, `_rightBracket`, `_opacity`, `_matrixGrid`, `_scrollViewer`, `_grid`, `_border`, `_toggle`, `_inputControl`, `_rowHeadingTop`, `_columnHeadingLeft`, `_matrixTop`, `_matrixLeft`, `_matrixContentTop`, `_matrixContentLeft`, `_children`, `_region`, `_scriptableInfo`) | unblocked | field declarations move to hand-written partial |
| VisualInputScriptableControlNodeBase | Static fields / constants (`TypeOfThis`, `ContentChangedEvent`, `RequestActivateEquationForComboBoxNodeEvent`, `pastingAttachedPropertyDelay`, `headingFontSizeSmallerBy`, `headingFontOpacity`, `headingCellHorizontalPadding`, `rowHeadingName`, `VISUAL_NAME`) | unblocked | static fields and constants move to hand-written partial |
| VisualInputScriptableControlNodeBase | Simple properties (`ViewModel`, `BlockFocus`, `IsAdornerFocused`, `ResizeAdorner`, `OriginalSize`, `EffectiveBaselineY`, `EffectiveScriptBaselineY`, `TextBaselineY`, `RowCount`, `ColumnCount`, `Cells`, `RowHeading`, `ColumnHeading`, `NodeInfo`, `MatrixHeadingRowIndices`, `MatrixHeadingColumnIndices`, `ErrorCell`, `ErrorNode`, `HeadingsVisibility`, `this[int]`, `LeftFence`, `RightFence`, `NodeChildren`, `DesiredSizeWithoutFences`, `Horizontal`, `MathChildren`, `Children`, `IsEditorFrameVisible`, `InputControl`, `DataExchangeModel`, `IVisualNode.Loaded`, `IVisualNode.IsLoaded`) | unblocked | standard property accessors matching original IL |
| VisualInputScriptableControlNodeBase | Lifecycle & initialization (`.ctor`, `.cctor`, `init`, `InitMathRegion`, `InitNodeInfo`, `InitInputControl`, `InitializeVisibility`, `ConstructContextMenu`, `Dispose`, `Finalize`) | unblocked | plain IL |
| VisualInputScriptableControlNodeBase | Command handlers & events (`BindCommands`, `CanAddInputs`, `CanRemoveInputs`, `HandleOpenEditingMode`, `HandleAddInputs`, `HandleRemoveInputs`, `HandleAddOutput`, `OnControlLostKeyboardFocusOrDataChanged`, `HandleCommitNeeded`, `HandleModelPropertyChanged`, `GetEquationControl`, `LoadedHandler`, `HandleCalculatableStateChanged`, `HandleMathFormatChanged`, `HandleIORegionChanged`, `HandleSExpressionChanged`, `OnContextMenuOpening`, `HandleGotFocus`, `HandleLostFocus`, `HandleMouseDown`, `NavigateInto`) | unblocked | plain IL |
| VisualInputScriptableControlNodeBase | Layout, hit testing & matrix headings (`HookupMatrixResizeWidget`, `UpdateOnBorderResize`, `MeasureStaticLayout`, `TryDragIntoHiddenPart`, `GetNodeAtPoint`, `IsResizeIndicator`, `SetupResizeIndicatorBehaviour`, `ResizeIndicatorOnLoaded`, `MeasureOverride`, `ArrangeOverride`, `ArrangeStaticLayout`, `SetParensVisibility`, `CreateHeading`, `ArrangeHeadingContent`, `GetMatrixRowOrColumnIndices`, `SetHeadingsCellsVisibility`, `PopulateMatrixHeadings`, `BuildChildren`, `ConstructMatrixGrid`, `CreateTextNode`, `CheckIfSpecialCellNeeded`, `GetIndexOfNode`) | unblocked | plain IL; lambda in `SetupResizeIndicatorBehaviour` matches original closure ordinal |

Blocked by LoadComponent/XAML: None in these files (`InitializeComponent` and `Connect` on `VisualInputScriptableControlNode` were already implemented in `src/Quire.Controls.EquationControl/Mirrored/Controls.Internal.VisualInputScriptableControlNode.cs` on main, preserving the 14 `LoadComponent` calls as instructed).
No member is blocked by a dynamic script engine, a SymbolDescriptor constructor, or a type-map ownership gap.

## Batches

Batches are planned in dependency order:
1. `VisualInputScriptableControlNodeEditor`: complete type (fields, CaretState, constructors, layout overrides, input, caret blinking).
2. `VisualInputScriptableControlNode`: fields, constructors (`.ctor()`, `.ctor(string, MathNode, VisualMathRegionNode)`), and `.cctor`.
3. `VisualInputScriptableControlNodeBase`: fields, constants, properties, lifecycle, commands, event handlers, layout, and matrix headings.

## Remaining

- No members blocked.

## Results

- Baseline: PASS, 1733 checked (`.mirror-cache/release-il-4c8b60c9212a/release-il.json`).
- Batch 1 (VisualInputScriptableControlNodeEditor complete type): PASS, 1746 checked (`.mirror-cache/release-il-e849e1ef0e42/release-il.json`). Shapes: `OnPreviewKeyDown` uses `switch (e.Key)` with `Key.Up` and `Key.Down` which Roslyn lowers to branch and pop/pop matching Mathcad; `OnCaretStateChanged` visibility uses `CaretState == LeftMargin || CaretState == RightMargin ? Visible : Hidden`.
- Batch 2 (VisualInputScriptableControlNode complete type): PASS, 1749 checked (`.mirror-cache/release-il-95a4067d3ef3/release-il.json`). Shapes: constructors, `.cctor`, and field declarations in Mathcad order matching original Release IL.
- Batch 3 (VisualInputScriptableControlNodeBase complete type): PASS, 1876 checked (`.mirror-cache/release-il-b839828ed356/release-il.json`). Shapes: `SetParensVisibility` uses `parensVisibility == Visibility.Visible ? 1 : 0` lowering to `ceq; conv.r8`; `CreateTextNode` types local as `IVisualTextNode` to call interface setters and emit `isinst DependencyObject`; `ResizeIndicatorOnLoaded` uses guard clause `if (visualTextNode == null || !IsResizeIndicator(visualTextNode)) return;` matching branch layout; `NodeChildren` uses `_matrixGrid.Children.CopyTo((UIElement[])array, 0)` matching local variable slots; `PopulateMatrixHeadings` reuses hoisted `Label element` and integer subexpression locals.
