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

## Remaining (not yet started)

- `CollapsibleAreaSerializationStrategy`, `CollapsibleAreaDeserializationStrategy` (3 + 3 stubs).
- `ICollapsibleAreaModel`, `ICollapsibleAreaViewModel` (interface declarations).
- `CollapsibleAreaModel` (23 stubs), `CollapsibleAreaViewModel` (67 stubs, incl. nested `CollapsibleAreaResizeData`), `CollapsibleAreaControl` (61 stubs).
