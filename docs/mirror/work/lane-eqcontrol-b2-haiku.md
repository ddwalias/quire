# Lane: Quire.Controls.EquationControl, half B2 (eqcontrol-b2-haiku)

- Owner: eqcontrol-b2-haiku worker (Claude, claude-haiku-5-5)
- Branch: `mirror/eqcontrol-b2-haiku-20261119` (from `main` 0b541ddcd)
- Scope: generated stubs in `src/Quire.Controls.EquationControl/Generated/Mirror` from
  `Controls.Internal.VisualMathRegionNode.g.cs` through `Controls.SpecTableView.g.cs` (21 files with stubs).

## Type: Controls.Internal.VisualMathRegionNode

- Landed exact (Release IL): `OnKeyDown`, `OnRender`, `HandleMouseDown`.
  Report: `.mirror-cache/release-il-2204e94f9d35/release-il.json` (gate on the committed tree).
- Parked, with the blocker found:
  - `AutomateKeypress`: Mathcad concatenates `string + char` through `String.Concat(object, object)`; Roslyn emits
    `Concat(string, string)` / `ReadOnlySpan` `Equals`. Two shapes tried (named string locals, literal list); the concat form remains.
  - `OnMouseMove`: field access differs at the `DragIntoMatrixHiddenPart` null-check (event field load order). One shape tried.
  - `SetWarnings`: Mathcad stores the `RubberBandHighlighter` local in slot 0; Roslyn's `foreach` variable takes slot 0. Not reordered.
  - `SetError(IEngineError, IList<IVisualMathNode>)`: branch layout differs at the `error.ResourceString`/display-text checks (brfalse sense).
  - `FireCanInsertInputControl`: pattern-variable local slot order (`stloc.3` vs `stloc.2`).
  - `RecursiveRemoveOfEditors`: local slot / loop shape differs at the `Children` enumeration (`stloc.s 9` vs `stloc.2`).
  - `FireCanSwitchToLineBreakOperator`: needs `DevComponents.WpfRibbon.IButtonCommandParameter`; the project does not reference it (also recorded in `lane-eqcontrol-visualMathRegion.md`).
  - `OnInput`, `HandleLostFocus`: null/false ternary and `||` layouts (also in `lane-eqcontrol-visualMathRegion.md`).
  - Lambda or method-group closure members (`RefreshView`, `SetupCommands`, `RemoveWarnings`, `BuildAndExecPasteCmd`, `BuildAndExecCutCmd`, `HandleGotFocus`, `BackUpLabeledIdDictionary`, `IsLabeledIdDictionaryEqualToBackUp`, both constructors): need the `<>c` ordinal placeholders (AGENTS.md); not started.
  - `.cctor` DependencyProperty fields (`TypeOfThis`, `RootNodeProperty`, `LocationProperty`, `RootPaddingProperty`): stay stubs.
- Remaining in this type: 42 stub members (of 45; 3 landed).

## Landed
- `fbb7462ce` (on main; branch commit `16a2a6ae1`): VisualMathRegionNode OnKeyDown, OnRender, HandleMouseDown. Gate PASS, landing receipt `C:\Projects\Quire\.mirror-cacheeceipts\gate-il-f6c175bd5f66.json`.

## Landed in the second dispatch (main, Release IL gate PASS per landing)
- VisualSubscriptOperatorNode: ctor, ModifyChildMathFormat, OnSecondOperandChanged, TextBaselineY, EffectiveBaselineY, EffectiveScriptBaselineY, HandleMathFormatChanged; fields moved to hand-written.
  Parked: CoreElements (iterator; state-machine ordinal), MeasureOverride (struct-temp local slot 4 vs 3), ArrangeOverride (dup/stack shape), GetNodeAtPoint (`new Rect` ternary as `call` on a local).
- VisualTextSpan: ArrangeOverride, backing fields. Parked: MeasureOverride, UpdateChildren (closure `Each` and foreach/lambda).
- VisualTextRun: ctor, MathFormat, Baseline, SuperscriptBaseline, CharacterWidths, Font, HandleMathFormatChanged, MeasureOverride, OnRender. Parked: Text and UpdateText (`!=`/`==` branch sense: Mathcad uses op_Equality with brfalse; ctor-free idioms tried, both mismatch).
- VisualStringNode: Text, TextBaselineY, HighlightRect, TextPropertyChanged, FireTextChanged, MeasureStaticLayout, OnRender, ComputeAlignmentBox, GetNodeAtPoint. Parked: UpdateText (FormattedText argument load order, `Font.Foreground` position), ScreenInkBounds (not attempted), static ctor (DependencyProperty).
- VisualMultiLineAligningBinaryOperatorNode: ctor, ComputeTopLineBaseLine, CurrentBaseLine, FirstNodeAndOperandWidth, HasFirstOperandAtLeft/Above. Parked: IsMultiline (bool? assignment-expression temp slot), GetSymbolRectangle (pattern-variable local slot), ComputeOperatorRectangle (struct local read via ldloca), ComputeCurrentValues, MeasureStaticLayout, ArrangeStaticLayout (use base `_operatorExtendedRectangle`, not yet mirrored), GetFirst/SecondChildRectangle (need base static `GetOffsetAndExcludeRectangles`, not yet mirrored).
- VisualMultiLineWrappingBinaryOperatorNode: IsMultiline, WrappingSign, HasFirstOperandAbove/AtLeft, ComputeOperatorRectangle. Parked: ctor (base `.ctor` call differs at #0), ComputeWrappingSignRectangle (local slot), CoreElementsTopLine (iterator), MeasureStaticLayout, ArrangeStaticLayout, GetVisualMathNodeAtPoint, GetFirst/SecondChildRectangle (base static helper).
- VisualSymbolicEvaluationNode: indexer (with string.Format for the exception message; the interpolated-string handler differs), Layout, NodeChildren, SymEvalSymbol, SymEvalSymbolTopLeft, TextBaselineY, EffectiveBaselineY, EffectiveScriptBaselineY, NavigatableParts, SetVisibilityOfOperatorSymbol, GetNodeAtPoint. Parked: ctor (field initializers with the static descriptor), CoreElements (iterator), MeasureStaticLayout and ArrangeStaticLayout (lambda `Each`/`EachWithIndex`).

Blocker classes seen in this dispatch: lambda closure ordinals (`<>c`), iterator state machines, Roslyn temp/local slot ordering for struct member reads and pattern variables, `!=` vs `==` branch sense for string comparisons, and base helpers that are still stubs.
Project reference: `DevComponents.WpfRibbon` is still not referenced (FireCanSwitchToLineBreakOperator, VisualMathRegionNode).

## Third dispatch: SpecTableView (placeholder device)
- All 103 SpecTableView members moved into `Mirrored/Controls.SpecTableView.cs` in Mathcad order, with `[MirrorStub]` removed from the generated file (now an empty class body).
- Exact (Release IL, not in the mismatch list): constructor, command binding and editing-command members, the cut/copy/clear/delete handlers, traversal and focus helpers, AddChild, Clear, ActiveCell/ActiveCells/SelectedCells/IsSelected, the visibility helpers, selection and mouse handlers, SelectCells, GetStartEndCellIndices, FireCellAdded, the event fire members.
- Unmarked placeholders (blocker named): `CellCount` and `MathFormat`/`DeserializeLabeledIdFormat`/`DeserializeMathFormat` (auto-property backing fields and base call), `VerticalAlignmentOffset` (constant shape `2.0` vs `0.0` in the first comparison), `GetChildrenInRowMajorOrder` (iterator: Quire names the state machine `d__107`, Mathcad `d__89`; the placeholder device does not shift it, so the ordinal offset is 18 in the hand-written partial), `OnRender` (pen local slot 5 vs 1), `HandleDeleteCommand`, `HandleCellCommandWithDirectionParameter`, `HandleTraverseForward`, `HandleBackwardsTraverse` (`brtrue`/`brfalse` sense of the null-test), `UpdateAdornerLocationToSelectedCells` (local slot 7 vs 0), `ClearOvertypeAdorner` (`callvirt` vs `call` on AdornerLayer.Remove), `SetDeserializedMathFormatData` (branch sense of the null test).
- SpecTableControl: not started in the third dispatch (92 stubs; decompile is 1299 lines, with closures in `HideFromCalculationContext` and `SearchableRegions`; needs the placeholder device).

## Remaining

- Stub attributes left in the 21 files at this checkpoint: 678 (was 763 before the second dispatch).
- Not started: VisualVariantLineBinaryOperatorNode (70), VisualMatrixNode (56), VisualTextNodeEditor (41), VisualOutfixNodeEditor (35),
  VisualSymbolNode (34), VisualNthRootOperatorNode (29), VisualNthRootNodeEditor (29), VisualSymbolicEvaluationNodeEditor (27),
  VisualTextNode (26), VisualStepRangeOperatorNodeEditor (24), VisualPlaceholderNode (22), VisualScoper (20), SpecTableControl (92), SpecTableView (103).
- The landed VisualMathRegionNode still has 24 stubs (lambda, `.ctor`, DP, and blocker-class members listed above).
- The lane stopped here; the remaining members are not claimed.
