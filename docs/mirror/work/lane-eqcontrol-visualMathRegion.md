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

## Batch 2: simple members

- Members: `Size` (get), `TextBaselineY` (override get), `Register_EQUATION_WRAPPING_DEBUG_Handlers` (`[Conditional]`, empty), `WriteImageTo`, `IVisualMathRegionNode.Focus`, `HandleProhibitMouseClickPropertyChanged`.
- Shapes tried: `Size` reads `ActualHeight` into a local before `ActualWidth` (Mathcad order); other members matched on the first candidate.
- Release IL: PASS, 1676 checked (`.mirror-cache/release-il-83e439d1b902/release-il.json`).

## Batch 3: event forwarders

- Members: `FireCopy`, `FirePaste`, `FireCut` (both overloads each), `FireShowRegionHistory`, `FireCanPaste`, `FireRequestRestoreInsertionPoint`, `FireSetIsReplacingPreviouseOperator(bool, out bool)`, `FireCanTraverse`, `FireTraversalRequest`, `FireCanPerformReplace`, `FireRestoreFromHistory`.
- Shapes tried: the first candidate put `e.Handled = true` after the `if` for `FireCopy`/`FirePaste`/`FireCut`/`FireShowRegionHistory` and FAILED; Mathcad sets `Handled` inside the null-checked block. Traversal checks written as `direction == X && Event != null` (Mathcad branches on the null test with the direction test in one chain) matched.
- Release IL: PASS, 1690 checked (`.mirror-cache/release-il-fc2924bfe416/release-il.json`). Earlier candidate: FAIL (6 members) at `.mirror-cache/release-il-6f7e66c8517d`.

## Batch 4: small members

- Members: `HandleWarningDisplayFilterChanged` (static, `is` pattern and `?.Invoke()`), `GetNodeAtPoint` (internal override), `OnVisualParentChanged`, `GetTransformBasedOnNumericNodeHeight`, `HandleMathFormatChanged`, `HandleLabeledIdFormatDictionaryChanged`, `IsExtendedFontEqualsToBackUp`, `Dispose`.
- Shapes tried: `IsExtendedFontEqualsToBackUp` with `if (!Equals) return true;` FAILED (branch sense inverted); `if (Equals) return revision != ...; return true;` matched.
- Release IL: PASS, 1704 checked (`.mirror-cache/release-il-88f55d1324ce/release-il.json`).

## Batch 5: text input, insert-function forwarders, adorners, root node, mouse click

- Members: `OnTextInput` (override, `foreach` over `e.Text`), `FireInsertFunction` (both overloads, `?.Invoke`), `GetAdorners`, `OnRootNodeChanged`, `SimulateMouseClick`.
- Shapes tried: `SimulateMouseClick` as `if (n == 1) … else if (n == 2) …` FAILED; Mathcad dispatches with two `beq` tests and a shared return, which a two-case `switch` with `break` matched.
- Release IL: PASS, 1711 checked (`.mirror-cache/release-il-dcd69ce9f7f5/release-il.json`).

## Batch 6: explicit Loaded event, ShowVisualHistory

- Members: `IVisualMathRegionNode.Loaded` explicit event (`_loadedEvent` with `Delegate.Combine`/`Delegate.Remove`), `ShowVisualHistory`.
- Release IL: PASS, 1721 checked (`.mirror-cache/release-il-d72bda39513a/release-il.json`). Branch merged with `main` first; the gate compared against newer `main` code from other lanes.

## Blocked after batch 6

- `FireCanSwitchToLineBreakOperator`: Mathcad IL uses `[DevComponents.WpfRibbon]DevComponents.WpfRibbon.IButtonCommandParameter`. `Quire.Controls.EquationControl` does not reference that assembly, so the type cannot be named. Needs a project reference (outside this lane; maintainer). Stub kept.
