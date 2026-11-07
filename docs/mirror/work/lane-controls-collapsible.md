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
  - `HandleGridCellSizeChanged(Size, Size)`: Release IL calls `callvirt CollapsibleAreaViewModel::get_StrictMinimumSize`; `StrictMinimumSize` in the hand-written override compiles to `callvirt WhiteboardViewModel::get_StrictMinimumSize`. Not resolved (the base is `protected virtual`, no redeclaration in `MathboardViewModel`). Kept as the base pass-through, as on main.

## Remaining (not yet started)
- `CollapsibleAreaControl` (61 stubs), then the two ViewModel placeholders listed in batch 4.
