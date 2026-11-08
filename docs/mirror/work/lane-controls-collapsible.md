# Lane CollArea — Quire.Controls.CollapsibleArea

- Owner: Orca task `task_2aa5b47d434a` (maintainer-assigned; no WORK.md lane row).
- Branch: `mirror/controls-collapsible-20261108`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/controls-collapsible-20261108`.
- Baseline (main `2b04ba9f`): 16 exact, 209 unfinished.

## Batch 1 — converter, dictionary, XAML data object, commands

- Members: `AreaHeaderConverter` (`Instance`, `.cctor`, `Convert`, `ConvertBack`, `.ctor`); `AreaProtectionDictionary` (`_contentLoaded`, `.ctor`, `InitializeComponent`, `IComponentConnector.Connect`); `AreaTypeXamlCopyDataObject` (auto-properties `IsCollapsed`, `IsBorderVisible`, `ProtectionDataWrapper`, `.ctor`); `ToggleBorderCollapseStateCommand` (all members); `ToggleCollapseStateCommand` (all members).
- Hand-written: `src/Quire.Controls.CollapsibleArea/Mirrored/Controls.CollapsibleArea.<Type>.cs`; stubs removed from the matching `Generated/Mirror/*.g.cs`.
- Release IL: PASS — 38 checked, 38 exact, 0 failures (`--project Quire.Controls.CollapsibleArea`). 188 unfinished.
- Shapes that matched on first try: `public virtual` on converter `Convert`/`ConvertBack`; plain `public void InitializeComponent()` (no `virtual`) and explicit `IComponentConnector.Connect`; field initializer for `Instance`; `(DateTime?)null` ternary arm in `Convert`; auto-properties for the XAML data object.
- Remaining mismatches in this batch: none.

## Batch 2 — serialization strategies, interface declarations

- Members: `CollapsibleAreaSerializationStrategy` (fields `_control`, `_regionCreator`, `.ctor`, `ConvertToRegionPersistantData`, `SerializeRegionsEpilog`); `CollapsibleAreaDeserializationStrategy` (`_control`, `.ctor`, `RegionFactory` get, `DeserializeRegionsEpilog`); `ICollapsibleAreaModel` (10 declarations); `ICollapsibleAreaViewModel` (18 accessor/method declarations).
- Hand-written: `Mirrored/Controls.CollapsibleArea.<Type>.cs`; stubs removed from `Generated/Mirror/*.g.cs`. Interface declarations are abstract in the partial (`{ get; set; }`, no bodies), as in `Controls.Text.IWhiteboardItemInTextWrapper`.
- `DeserializeRegionsEpilog` had a non-stub generated pass-through (`=> base.DeserializeRegionsEpilog(...)`) in the .g.cs; it was removed because the hand-written override replaces it.
- Release IL: PASS — 72 checked, 0 failures. Interface declarations are checked as declaration-only (exact). 154 unfinished.
- Shapes that matched first try: `protected readonly` for the `family initonly` field; `DebugTools.FailAssertion((string)null)` (ILSpy's `null` is ambiguous with the `Exception` overload); `region.Location = GetLocation(item)` order; `new CollapsibleAreaViewModel Clone();` in the interface.
- Remaining mismatches in this batch: none.

## Batch 3 — CollapsibleAreaModel

- Members: all 26 model members (fields `_isCollapsed`, `_isBorderVisible = true`, nested `PropertyNames` constants, `CanToggleCollapseState`, `IsCollapsed`, `IsBorderVisible`, `ProtectionData`, `HasOpenDescendantOleObjects`, `PrintViewport`, `IsInPrinting` override, `PropertyChanged` event, `.ctor`, `ToggleCollapseState`, `ToggleBorderVisibleState`, `GetPersistentData`, `SetPersistentData`, `SetProtection`, `RemoveProtection`, `Clone`, `CopyStateTo`, `InitializeChildManager`, `FirePropertyChanged`, `ShouldCloneItem`).
- Hand-written: `Mirrored/Controls.CollapsibleArea.CollapsibleAreaModel.cs`; 25 declarations removed from `Generated/Mirror/Controls.CollapsibleArea.CollapsibleAreaModel.g.cs`.
- Shape tried and rejected: block-`if (x != value) { ... }` setters for `IsCollapsed` and `IsBorderVisible`. Gate: "Release IL differs" (`beq.s` to exit vs Mathcad's `bne.un.s` + `ret`). Kept instead: `if (x == value) return;` early return with a one-line comment (both setters).
- Release IL: PASS — 67 checked, 67 exact, 0 failures. Model has 26 exact, 0 stubs.
- Remaining mismatches in this batch: none.

## Batch 4 — CollapsibleAreaViewModel (75 of 77 members exact)

- Members: all ViewModel members, including the nested `PropertyNames` (constants), nested `CollapsibleAreaResizeData`, `.cctor` (`CollapsedAreaHeight`), and the closures in `.ctor`, `BindCommands`, `HandleItemAdded`, `HandleCollapsibleStateChanged`, `ErrorPropertyChangedHandler`.
- Hand-written: `Mirrored/Controls.CollapsibleArea.CollapsibleAreaViewModel.cs` (one partial, members in Mathcad metadata order). The generated `.g.cs` keeps only the class header; the nested types moved to the end of the hand-written class with their `MirrorOf` attributes.
- Compile-order rule found by the gate: a closure's `DisplayClass{N}` / lambda ordinal is `1 +` the number of counted members declared before the method in source order. Counted: methods, accessors, constructors, properties (1 each), fields including backing fields and static fields. Not counted: `const` fields. Nested types count where they are declared, so they go after the methods. Mathcad source order is the metadata token order of its methods (a property's accessors sit at the property's declaration point; `private set` and `set` may precede `get`), and each field sits before the members that follow it.
- Release IL: PASS — 151 checked, 151 exact, 0 failures; 63 unfinished in the project (Control 61, ViewModel 2).
- Shapes tried and kept (matched): early-return setters (`AreaHasErrors`, `ProtectionOperationsEnabled`, as in the Model); `MinimumHeight = FindMinimumSize().Height;` (one statement); `insertionPoint.Offset(0.0, -Grid.GridCellSize.Height)` (Release IL uses `neg`); `IsAncestorOf(visual)` with no cast; `RestoreResizeData` with `resizeData as CollapsibleAreaResizeData`, null check and early return, then `OldSize`.
- Unfinished in this batch (placeholders, `=> throw global::Quire.Mirror.MirrorStub.NotImplemented();`):
  - `OnToggleProtectState(object)`: the `func` lambda (`<>c__DisplayClass53_0::<OnToggleProtectState>b__0`) has `IsNullOrEmpty<char>` followed by `ldc.i4.0; cgt.un; ret` in Release IL. Shapes tried: `return str.IsNullOrEmpty();` (no `cgt.un`), `return str.IsNullOrEmpty() != false;` (C# folds it away). Not matched.

## Batch 5 — CollapsibleAreaControl and constant holders

- Members: all Control members (fields, auto-properties, events, the `.ctor` closures, `HideFromCalculationContext`, `PrintViewport`, `ControlWidth`/`ControlMinHeight` handlers, `InitializeComponent`/`Connect`), plus the constant-only `AreaHeaderKeys`, `CollapsibleAreaConstants` and `ProductInfo.Constants` (constants and constructors moved to hand-written partials, MirrorStub markers removed).
- Hand-written: `Mirrored/Controls.CollapsibleArea.CollapsibleAreaControl.cs` (one partial, members in Mathcad metadata order), `Mirrored/Controls.CollapsibleArea.AreaHeaderKeys.cs`, `...CollapsibleAreaConstants.cs`, `Mirrored/ProductInfo.Constants.cs`. The Control's generated `.g.cs` keeps only the class header.
- Compile-order facts confirmed by the gate: a constant field counts toward a closure ordinal (`DragBorderName` and `ToggleButtonCanvasName` are counted). `_messageBoxMngr` sits right before the `MessageBoxManager` property, after the ctor. `_contentLoaded` and `InitializeComponent`/`Connect` are the XAML partial's members, last in order.
- Shapes that matched: `if (d as T)`-style handlers as `T x = d as T; if (x == null) return;`; `if (_contentLoaded) return;` in `InitializeComponent`; `this` without the `(DependencyObject)(object)` cast for `AttachedProperties`; `HandlePrintViewportChanged`'s `if (isInPrintCommon.HasValue)` branch before the per-item `foreach`.
- Release IL: PASS — 221 checked, 221 exact, 0 failures. Project-wide: 221 exact, 5 unfinished, 0 MirrorStub markers left in `Generated/Mirror`.
- Remaining unfinished members (all placeholders, `=> throw global::Quire.Mirror.MirrorStub.NotImplemented();` unless noted):
  - `CollapsibleAreaControl.PrintViewport` setter: Release IL uses `ldarg`-free `HasValue` value-materialization (`beq.s`, `ldc.i4.0`, `br.s`, then `brfalse.s`). Tried: block `if (A || B) return/set`, `if (sameState) return;`; both emit `bne.un` jump form. Not matched.
  - `CollapsibleAreaControl.HideFromCalculationContext` setter: Release IL is `bne.un.s` + `ret` for `if (HideFromCalculationContext == value) return;` and then the nested closures; Quire emits `ceq`/`ldc.i4.0`/`ceq`/`brtrue` for the same test. Not matched; the two closures (`<>c__DisplayClass76_0`) are inside the placeholder.
  - `CollapsibleAreaViewModel.OnToggleProtectState` (see batch 4): the `func` lambda `cgt.un` shape.
  - `CollapsibleAreaViewModel.HandleGridCellSizeChanged`: pass-through as on main (see batch 4).

## Batch 6 — HandlePrintViewportChanged

- Member: `CollapsibleAreaControl.HandlePrintViewportChanged` (internal static).
- Shape that matched: the same branch order as batch 5 (`if (isInPrintCommon.HasValue) {Each} else {foreach}`), with **no `return;` inside the `foreach`**. The `return` made Roslyn emit an extra `ret` before the outer `else` (leave target mismatch at `leave.s` / `brfalse` offsets); removing it gives Mathcad's single epilogue.
- Release IL: PASS — 226 checked, 226 exact, 0 failures.
- Remaining placeholders: `Control.PrintViewport` setter, `Control.HideFromCalculationContext` setter, `ViewModel.OnToggleProtectState`, and `ViewModel.HandleGridCellSizeChanged` (pass-through). Shapes tried for each are recorded in the sections below.

## Batch 7 — ViewModel.HandleGridCellSizeChanged

- Member: `CollapsibleAreaViewModel.HandleGridCellSizeChanged` (was a base pass-through on main).
- Shape that matched: `base.HandleGridCellSizeChanged(...)`, the `Each` lambda, then `MinimumHeight = StrictMinimumSize.Height;` as one statement. The local form `Size strictMinimumSize = StrictMinimumSize; MinimumHeight = strictMinimumSize.Height;` gave Mathcad's `this` push in the wrong place (`ldarg.0` after the stloc) and bound the `StrictMinimumSize` read to `WhiteboardViewModel::get_StrictMinimumSize`. The one-statement form binds to `CollapsibleAreaViewModel::get_StrictMinimumSize`, as Mathcad does.
- Release IL: PASS — 229 checked, 229 exact, 0 failures.

## Batch 8 — attempts on the three remaining placeholders (no shape found)

Gate: `release_il.py --project Quire.Controls.CollapsibleArea` (G) or a scratch build + `ilspycmd` dump (D). Each is a single Mathcad member; the other placeholders stay as they are.

- `CollapsibleAreaControl.PrintViewport` setter. Mathcad leaves the unchanged-check on the stack (`beq.s` into the comparison arm, `ldc.i4.0; br.s`, then one `brfalse.s` and an early `ret`).
  1. (G) `bool unchanged = A.HasValue == B.HasValue && (!A.HasValue || A.GetValueOrDefault() == B.GetValueOrDefault()); if (unchanged) return;` — `bne.un.s` at the first compare, Mathcad `beq.s`.
  2. (G) `if (A.HasValue != B.HasValue ? false : (...)) return;` — Roslyn rewrites `c ? false : Y` to `!c && Y`, `bne.un` again.
  3. (G) `if (A == B) return;` on `Rect?` — the lifted operator adds `ldloc.0/stloc.1/stloc.2` temporaries that Mathcad does not have.
  4. (G) `bool unchanged = A.HasValue != B.HasValue ? false : (...); if (unchanged) return;` — `bne.un.s`, plus a bool local Mathcad does not have.
  5. (G) explicit `goto` layout matching the branch order — Roslyn chains the jumps into `bne.un`.
- `CollapsibleAreaControl.HideFromCalculationContext` setter. Mathcad materializes the comparison (`ceq; ldc.i4.0; ceq; brtrue.s`) before the early `ret`.
  1. (G) `if (HideFromCalculationContext == value) return;` (the original body) — `bne.un.s 2` where Mathcad has `ceq; ldc.i4.0; ceq; brtrue.s`.
  2. (D) `if (!(HideFromCalculationContext != value)) return;` — folds to the same `bne.un.s`.
- `CollapsibleAreaViewModel.OnToggleProtectState` (method exact on its own; its closure `<>c__DisplayClass53_0::<OnToggleProtectState>b__0` is not). Mathcad's lambda falls through `IsNullOrEmpty<char>` with `ldc.i4.0; cgt.un; ret` (the lambda is compiler-generated, so it cannot stay a placeholder while the method lands).
  1. (G) `return str.IsNullOrEmpty();` — no `ldc.i4.0; cgt.un`.
  2. (G) `return str.IsNullOrEmpty() != false;` — still differs; (D) the dump shows Roslyn folds it to the plain call.
  3. (D) `bool isNullOrEmpty = str.IsNullOrEmpty(); return isNullOrEmpty;` — folds as well.

Conclusion: the three members need compare/bool values materialized on the stack that C# source forms did not produce here. No shape passed the gate; they stay placeholders. Unfinished in the project: 3 (PrintViewport setter, HideFromCalculationContext setter, OnToggleProtectState). Release IL: 229 checked, 229 exact, 0 failures.

## Remaining (not yet started)
- Nothing implementable is left in this assembly. The 5 unfinished members above need a shape not yet found; the next step is the gate on those shapes, not new members.
