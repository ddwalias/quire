# EquationControl lane checkpoint: VisualMathRegionNode

- Owner: dispatched EquationControl worker (visualMathRegion group).
- Branch: `feature/eq-visualmathregion` (created from `main` at `c757fc2fc`).
- File: `src/Quire.Controls.EquationControl/Generated/Mirror/Controls.Internal.VisualMathRegionNode.g.cs`, implemented in `src/Quire.Controls.EquationControl/Mirrored/Controls.Internal.VisualMathRegionNode.cs`.
- Reference: `Ptc.Controls.Internal.VisualMathRegionNode` in `Ptc.Controls.EquationControl`, Release IL.
- Baseline gate (`--type Quire.Controls.Internal.VisualMathRegionNode`): `.mirror-cache/gate-classify`-style report at `scratchpad/gate-classify/release-il.json`. The type has 385 method records: 128 exact hand-written members, 192 exact stub-marked members (96 events, add/remove accessors), 65 stub-marked members that are not exact.

## Classification

Unblocked:
- U1 events: 96 `public event` declarations marked `[MirrorStub]`. Their add/remove accessors are already exact in Release IL; the declarations move to the hand-written partial and the markers are removed.
- U2 instance fields: `_isLogicalFocusWithinScriptableControl`, `_isSymbolicEvalKeywordsShown`, `_activeGestureManager`, `_defaultGestureManager`, `_emptyGestureManager`, `_limitSideGestureManager`, `_visualFactory`, `_message`, `_highlight`, `_historyPopup`, `_mouseDownPosition`, `DragHysteresis` (const), `_isDragging`, `_previouseKeys`, `_loadedEvent`, `GetErrorDisplayText`, `_previousLabeledIdFormat`, `_previousLabeledIdFormatRevisionNumber`, `_visibleWarningMessage`, `_lhsOfDefinitionVisibility`, `SExpressionComplexIndicator` (const).
- U3 simple methods (no closure, no XAML, no script engine): `get_Size`, `get_TextBaselineY`, `Register_EQUATION_WRAPPING_DEBUG_Handlers`, `HandleProhibitMouseClickPropertyChanged`, `WriteImageTo`, `IVisualMathRegionNode.Focus`, `OnTextInput`, `OnKeyDown`, `OnMouseMove`, `OnRender`, `MeasureStaticLayout`, `ArrangeStaticLayout`, `MeasureResizableChildNode`, `HandleMouseDown`, `OnInput`, `FireRequestRestoreInsertionPoint`, `SetWarnings`, `HandleLostFocus`, `FireCanSwitchToLineBreakOperator`, `CanConvertToTextRegion`, `IsTextRegionEnabled`, `FireSetIsReplacingPreviouseOperator`, `FireCanInsertInputControl`, `FireInsertFunction` (x2), `FirePaste` (x2), `FireCanPaste`, `FireCopy` (x2), `FireCut` (x2), `FireShowRegionHistory`, `OnRootNodeChanged`, `SetError` (x2), `HandleWarningDisplayFilterChanged`, `GetAdorners`, `ShowVisualHistory`, `RecursiveRemoveOfEditors`, `FireRestoreFromHistory`, `GetNodeAtPoint`, `OnVisualParentChanged`, `FireCanTraverse`, `FireTraversalRequest`, `FireCanPerformReplace`, `GetTransformBasedOnNumericNodeHeight`, `SimulateMouseClick`, `HandleMathFormatChanged`, `HandleLabeledIdFormatDictionaryChanged`, `IsExtendedFontEqualsToBackUp`, `Dispose`, `AutomateKeypress`, `RefreshView` (see blocked below if its lambda ordinal does not match).
- U4 explicit event accessors `IVisualMathRegionNode.add_Loaded`/`remove_Loaded` (no marker; body in `.g.cs`).
- U5 static initializers: `TypeOfThis`, `RootNodeProperty`, `LocationProperty`, `RootPaddingProperty`, and the type initializer (`.cctor`) that registers them.

Blocked, reason: closure or lambda ordinal (`<>9__N_M`, `DisplayClass` names depend on compile order of earlier lambdas in the type):
- `.ctor()` and `.ctor(GestureManager, DelegateFunction1)` (closure, method-group cache `<>O`).
- `HandleGotFocus`, `BuildAndExecPasteCmd`, `BuildAndExecCutCmd`, `IsLabeledIdDictionaryEqualToBackUp` (closures).
- `SetupCommands`, `RemoveWarnings`, `BackUpLabeledIdDictionary` (lambda caches and ldftn).
- `RefreshView` (`<>9__187_0` ordinal).
Plan for these: placeholder stubs in Mathcad order (AGENTS.md) or a compile-order device, only after the unblocked members above are exact.

Not blocked by the listed reasons: no `LoadComponent`/XAML loads, no dynamic script engine, no `SymbolDescriptor` constructor and no type-map ownership conflict in this type.

## Batches

1. U1 events and U2 instance fields (declarations only; accessors already exact).
2. U3 simple methods, grouped by owner-visible behaviour (small batches).
3. U5 static initializers and `.cctor`.
4. Blocked closure group after placeholder ordering.

## Gate

- Release IL baseline (before batch 1): FAIL on this type's 65 non-exact stub members, as listed above; exact members unchanged.

## Batch 1: events and instance fields

- Members: 96 `public event` declarations and 21 instance/const fields of `VisualMathRegionNode` (moved from the `.g.cs` stubs to the hand-written partial; markers removed in the same change).
- Shapes: declarations only; accessors already exact. No closure or ordinal concerns.
- Release IL: PASS, 1670 checked (`.mirror-cache/release-il-03c6dd47b771/release-il.json`).
- Remaining: static fields and `.cctor` (U5), simple methods (U3), blocked closure group.
