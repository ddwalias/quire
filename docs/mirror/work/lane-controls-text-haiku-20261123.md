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
