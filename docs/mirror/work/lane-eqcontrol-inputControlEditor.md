# EquationControl lane checkpoint: inputControlEditor group

- Owner: dispatched worker `task_f9d5b7ccf815` (Orca session `orca_session_id:d3c29520-723f-4bea-8583-c05b23337e40`)
- Branch: `feature/eq-inputcontroleditor`, worktree `C:/Users/DuyDang/orca/workspaces/Quire/eq-inputcontroleditor`
- Files: `src/Quire.Controls.EquationControl/Generated/Mirror/` `Controls.InputControls.IInputControlEditorViewModel.g.cs`, `Controls.InputControls.InputControlEditor.g.cs`, `Controls.InputControls.InputControlEditorViewModel.g.cs`, `Controls.Internal.VisualInputControlEditor.g.cs`
- Reference: `PYTHONUTF8=1 python tools/reference/reference.py Ptc.<Type> --assembly Ptc.Controls.EquationControl --il` (Release IL; Mathcad `Ptc.` names)

## Classification

| Type | Stub | Status | Reason |
|---|---|---|---|
| IInputControlEditorViewModel | 14 interface members (get/set, ApplyScript, CancelScript, OnPreviewKeyDown) | unblocked | abstract declarations, no body |
| InputControlEditor | `.ctor` | unblocked | calls `InitializeComponent` (unfinished callee does not block the caller); lambda `<.ctor>b__1_0` depends on closure ordinal |
| InputControlEditor | `PostInitializeComponent` | unblocked | plain IL |
| InputControlEditor | `ButtonFileOpen_OnClick`, `ButtonFileSave_OnClick` | unblocked | OpenFileDialog/SaveFileDialog, no dynamic engine |
| InputControlEditor | `InputControlEditor_OnClosing` | unblocked | plain IL |
| InputControlEditor | `InitializeComponent`, `IComponentConnector.Connect` | blocked: LoadComponent/XAML | keep as is |
| InputControlEditor | `_viewModel`, `TabControl`, `cbLanguage`, `tbScript`, `gridPropertyEditor`, `_contentLoaded` | field declarations | XAML-named fields; left in place |
| InputControlEditorViewModel | `.ctor` | unblocked | field initializers (`_applyVisibility`, `_scriptingEditorEnabled`, `_isPythonIsUnavailable`, `cachedInputScript`) must move into the hand-written partial |
| VisualInputControlEditor | `CaretState` get/set, `CaretStateProperty` (`.cctor`) | unblocked | static field initializer moves to the hand-written partial |
| VisualInputControlEditor | `.ctor` x2, `BORDER_THICKNESS` initializer | unblocked | calls base ctor, Caret/Border; `BORDER_THICKNESS = 1` initializer moves |
| VisualInputControlEditor | `Add`, `Remove`, `OnCaretColorChanged`, `UpdateCaretBlinking` | unblocked | plain IL |
| VisualInputControlEditor | `MeasureOverride`, `ArrangeOverride`, `GetDesiredTransform`, `HandleCaretPositionChanged`, `OnCaretStateChanged` | unblocked | plain IL (switch on CaretState) |

Blocked by LoadComponent/XAML: `InputControlEditor.InitializeComponent`, `InputControlEditor.Connect`.
No member is blocked by a dynamic script engine, a SymbolDescriptor constructor or a type-map ownership gap.

## Batches

Batches are listed in landing order; each gate result is recorded below as it lands.

1. IInputControlEditorViewModel interface declarations.
2. InputControlEditor: `PostInitializeComponent`, `ButtonFileOpen_OnClick`, `ButtonFileSave_OnClick`, `InputControlEditor_OnClosing`, `.ctor`.
3. InputControlEditorViewModel `.ctor` with the four field initializers.
4. VisualInputControlEditor: `CaretState`, `CaretStateProperty`, `Add`, `Remove`, `OnCaretColorChanged`, `UpdateCaretBlinking`.
5. VisualInputControlEditor: both constructors and `BORDER_THICKNESS`.
6. VisualInputControlEditor: `MeasureOverride`, `ArrangeOverride`, `GetDesiredTransform`, `HandleCaretPositionChanged`, `OnCaretStateChanged`.

## Remaining

- Blocked: `InitializeComponent`, `Connect` (LoadComponent/XAML).
- Results per batch are appended below.

## Results

- Batch 1 (IInputControlEditorViewModel declarations): 20 interface members checked, shapes: abstract declarations without bodies, matching the original. Release IL: PASS, 1691 checked (`.mirror-cache/release-il-d90324581b6f/release-il.json`).
- Batch 2 (InputControlEditor constructor, PostInitializeComponent, ButtonFileOpen/ButtonFileSave/OnClosing): PASS, 1682 checked (`.mirror-cache/release-il-000b9136d147/release-il.json`). Shapes: the lambda ordinal needs `_viewModel` (a counted field) before the constructor, so the fields move to the hand-written partial in Mathcad order; `dialog.ShowDialog().Value` (not `== true`, which emits GetValueOrDefault); the reader keeps a null-checked `finally` calling `IDisposable.Dispose` directly (a `using` block adds a copy local). `InitializeComponent` and `Connect` are now unmarked placeholders in the hand-written partial after the XAML fields, still throwing (blocked by XAML).
- Batch 3 (InputControlEditorViewModel constructor): PASS, 1683 checked (`.mirror-cache/release-il-81bc2e397b9a/release-il.json`). Shapes: the four fields with initializers (`_applyVisibility`, `_scriptingEditorEnabled`, `_isPythonIsUnavailable`, `cachedInputScript`) move to the hand-written partial in IL order; `ResourceProvider<InputControlEditorResourceDirectory>.Instance` as the base argument, `_model.PropertyChanged += HandleModelPropertyChanged` then `Init()`.
