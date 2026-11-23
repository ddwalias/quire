# Lane checkpoint: Quire.Controls.Text (haiku, 20261123)

- **Owner:** haiku (this worktree, `feature/haiku-controls-text-high`, based on main `fc4a8bfd2`)
- **Assembly:** `Quire.Controls.Text` (`Ptc.Controls.Text.dll`)

## Batch 24 — RichTextWithEmbeddedItem.EnumerateEmbeddedObjectsInRange

- Member: `public override IEnumerable<object> EnumerateEmbeddedObjectsInRange(DocumentTextRange range)`. Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`; stub removed from `Generated/Mirror/Controls.Text.RichTextWithEmbeddedItem.g.cs`.
- Mathcad ordinal: the state machine is `<EnumerateEmbeddedObjectsInRange>d__114` (the task text said d__217; the cached reference says d__114).
- Compile-order device: the iterator follows six counted members (`DispatchCommandToItems`, `ModifyHyperlink`, `CopyHyperlink` with `_copyLinkText`, `InsertBaselineOffsetTag`, `OnToggleHideFromCalculationContext`, `Accept`), which makes the Roslyn name d__114. Measured: top of partial d__99, before `OnToggleHideFromCalculationContext` d__112, before `ClearSavedState` d__114.
- Shape: the `for (TextPointer curPos = ...; ...; curPos = ...)` form emits the Mathcad clear `<curPos> = null` before the final return. The `while` form with a method-level local does not.
- Release IL (project-wide): PASS — 639 checked, 0 failures.
- Regeneration: `MirrorGen emit` gives the same stub removal; the only difference in the regenerated file is the pre-existing `LogDocumentContents` placement drift already on main.
- Remaining in this file: 104 generated stubs not implemented; the iterator's layout is the count device above, so the partial's member order is not Mathcad token order.

## Batch 25 — RichTextWithEmbeddedItem.Lines (get)

- Member: `public IEnumerable<TextPointer> Lines` (getter iterator, `for` over `GetFirstLine`/`GetNextLine`). Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`; stub removed from the `.g.cs`.
- Mathcad ordinal: `<get_Lines>d__284`. Measured here: placed before `AddEmbeddedItem(IWhiteboardItemInText, TextPointer)` gives d__284. Removing the get-only stub from the `.g.cs` shifted earlier counts by two, so the `EnumerateEmbeddedObjectsInRange` iterator was moved after `ClearSavedState`/`ClearState` to keep `d__114`.
- Release IL (project-wide): PASS — 648 checked, 0 failures; both `get_Lines` and `d__114` are in the report.
- Regeneration: `MirrorGen emit` gives the same stub removal; the only difference is the pre-existing `LogDocumentContents` placement drift.

## Batch 26 — RichTextWithEmbeddedItem.IsTabNavigatable (get)

- Member: `public bool IsTabNavigatable => WhiteboardItems.Any((IWhiteboardItem item) => ContainerItemManager.GetIsTabNavigatable(item));`. Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`; stub removed from the `.g.cs`.
- Mathcad ordinal: lambda `<get_IsTabNavigatable>b__149_0`. Measured placement: before `SetSelection` (after 35 counted members from `Clone`) gives b__149_0.
- Stub removal shifted earlier counts by two; `EnumerateEmbeddedObjectsInRange` was moved after `Clone` and `OnStyleChanged` to keep `d__114`. `get_Lines` stays at `d__284`.
- Release IL (project-wide): PASS — 650 checked, 0 failures; `get_IsTabNavigatable`, `d__114` and `d__284` are in the report.
- Regeneration: `MirrorGen emit` gives the same stub removal; the only difference is the pre-existing `LogDocumentContents` placement drift.

## Batch 27 — RichTextWithEmbeddedItem.IHasChildArrangeInfoProvider.ChildArrangeInfoProvider (get)

- Member: explicit `IUIElementArrangeInfoProvider IHasChildArrangeInfoProvider.ChildArrangeInfoProvider => ContainerItemManager;`. Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs` (placed before `DispatchCommandToItems`); stub removed from the `.g.cs`.
- Reference IL: `ldarg.0; call get_ContainerItemManager; ret`, no closure and no ordinal, so placement before the ordinal-bearing members keeps `d__114`, `b__149_0` and `d__284` unchanged (re-measured).
- Release IL (project-wide): PASS — 651 checked, 0 failures.
- Regeneration: `MirrorGen emit` gives the same stub removal; the only difference is the pre-existing `LogDocumentContents` placement drift.
- Parked for later: `CanModifyHyperlink()` (lambda `<CanModifyHyperlink>b__20_0` needs an ordinal of 20, below the generated-stub prefix) and `RemoveInlineRecursively` (lambda, recursive) need the g.cs prefix reordered first.

## Batch 28 — RichTextWithEmbeddedItem.ExecuteChangeFontColor and ExecuteChangeHighlightColor

- Members: `private void ExecuteChangeFontColor(object, ExecutedRoutedEventArgs)` and `ExecuteChangeHighlightColor(...)`, each a single `ExecuteCommandKeepingSelection` call with a lambda that captures `this` and `e`. Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs` (placed before `CopyHyperlink`); stubs removed from the `.g.cs`.
- Mathcad closures: `<>c__DisplayClass96_0` (`<ExecuteChangeFontColor>b__0`) and `<>c__DisplayClass97_0` (`<ExecuteChangeHighlightColor>b__0`). Measured: at the top of the counted members the display classes were 91_0 and 92_0; after `ChildArrangeInfoProvider`, `DispatchCommandToItems`, `ModifyHyperlink` and `_copyLinkText` the names are 96_0 and 97_0.
- The iterator `d__114`, `b__149_0` and `d__284` are unchanged (re-measured).
- Release IL (project-wide): PASS — 657 checked, 0 failures.
- Regeneration: `MirrorGen emit` gives the same stub removal; the only difference is the pre-existing `LogDocumentContents` placement drift.
- Not attempted: `CanModifyHyperlink()` (lambda ordinal 20). The 33 generated fields are compiled before every hand-written member, so ordinal 20 needs the field and method prefix reordered; that is a wider change than one batch.
