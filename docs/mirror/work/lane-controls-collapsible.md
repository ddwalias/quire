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

## Remaining (not yet started)
- `CollapsibleAreaModel` (23 stubs), `CollapsibleAreaViewModel` (67 stubs, incl. nested `CollapsibleAreaResizeData`), `CollapsibleAreaControl` (61 stubs).
