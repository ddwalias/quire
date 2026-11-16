# Quire.Controls.EquationControl lane checkpoint

- Owner: dispatched EquationControl worker
- Branch: `feature/equationcontrol-stubs-agy-20261116`
- Selected members:
  - `Controls.Internal.MatrixLayoutManager`: `.ctor()`, `.ctor(MatrixLayoutManager)`, `ExtractVisualTextWidth`
  - `Controls.Internal.LayoutItem`: `.ctor(Size)`, `.ctor(Size, double)`, `ToString`
  - `Controls.Internal.TextLayoutItem`: `ItemType` get/set, `Size` get/set, `TextBaselineOffset` get/set, `CapsHeight` get/set, `XHeight` get/set, `ToString`, `.ctor`
  - `Controls.VisualDescriptorTableBase`: `PostInitializeComponent`, `UpdateElementUids`, `UpdateFunctionalityDictionary`, `.ctor`
  - `Controls.VisualDescriptorTableBase.InitializationInfo`: `FunctionalityDictionaryInitialized` get/set, `.ctor`
  - `Controls.InputControls.EditableComboBoxLayoutManager`: `.ctor()`, `.ctor(EditableComboBoxLayoutManager)`
  - `Controls.EquationCommands`: `GetCommandKeyGestures`, `GetCommandFirstKeyGesture`
  - `Controls.EquationCommands.CommitCommandParameter`: `IsAsynchronous` get, `IsForce` get/set, `IsForceControlOnly` get/set, `.ctor(bool)`, `.ctor(bool, bool, bool)`
  - `Controls.MathcadSettings`: `HandleFocusForIme` get/set, `PropertyChanged` event
  - `Controls.EquationControl.TestHelper`: `SetEquationXmlNode`, `.ctor`
- Parked members:
  - `Controls.Internal.ConjugateLayoutManager.FindClosestChild`: parked due to Roslyn compiler emitting `.maxstack 2` while Mathcad 12 binary has `.maxstack 3` on identical IL instructions.
  - `Controls.Internal.VectorizationLayoutManager.FindClosestChild`: parked due to Roslyn compiler emitting `.maxstack 2` while Mathcad 12 binary has `.maxstack 3` on identical IL instructions.
- Release IL: PASS; 2465 checked (0 failures, 2541 unfinished stubs remaining).
