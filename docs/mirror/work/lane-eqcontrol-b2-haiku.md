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

## Remaining

- About 1,500 member-level stubs across the 21 files in this half, as of this checkpoint. Only `VisualMathRegionNode` has been worked.
- Next: the other 20 files in order (`VisualVariantLineBinaryOperatorNode`, `SpecTableControl`, `SpecTableView` are the largest).
