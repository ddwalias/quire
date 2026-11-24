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

## Batch 29 — RichTextWithEmbeddedItem.BindHyperlinkCommands

- Member: `private void BindHyperlinkCommands()`, four `CommandBindings.Add(new CommandBinding(...))` calls using method groups only (no lambda, no ordinal). Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs` (placed before the font-color pair); stub removed from the `.g.cs`.
- Placement keeps the earlier ordinals: `<>c__DisplayClass96_0`, `97_0`, `b__149_0`, `d__114` and `d__284` re-measured unchanged.
- Release IL (project-wide): PASS — 658 checked, 0 failures.
- Regeneration: `MirrorGen emit` gives the same stub removal; the only difference is the pre-existing `LogDocumentContents` placement drift.

## Batch 30 — RichTextWithEmbeddedItem.GetCombinedDocumentXaml (static)

- Member: `private static string GetCombinedDocumentXaml(string xaml, IList<string> itemXamls)`, with a `Sum` lambda `<GetCombinedDocumentXaml>b__103_0` in `<>c`. Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`; stub removed from the `.g.cs`.
- Placement: the lambda's ordinal needed 103. Measured moves: before `DeserializeMathFormat` (after `TextStyleChangedHandler`) gives 103; `CopyHyperlink` was moved before the font-color pair so the `96_0`/`97_0` display classes stay correct after the stub removal.
- Re-measured unchanged: `<>c__DisplayClass96_0`, `97_0`, `b__149_0`, `d__114`, `d__284`.
- Release IL (project-wide): PASS — 662 checked, 0 failures.
- Regeneration: `MirrorGen emit` gives the same stub removal; the only difference is the pre-existing `LogDocumentContents` placement drift.

## Batch 31 — RichTextWithEmbeddedItem.RepairWrapperHosts

- Member: `private void RepairWrapperHosts()`, a `for` over `Document.ContentStart` with the `Grid { Parent: InlineUIContainer parent } grid && GetWrapper(grid) is UIElement value` pattern, then `using (new CommandScope(new CompoundCommand()))` with a `foreach`. No lambda, so no closure ordinal. Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs` (placed before `BindHyperlinkCommands`, ahead of every ordinal-bearing member); stub removed from the `.g.cs`.
- Re-measured unchanged: `b__103_0`, `<>c__DisplayClass96_0`, `97_0`, `b__149_0`, `d__114`, `d__284`.
- Release IL (project-wide): PASS — 663 checked, 0 failures.
- Regeneration: `MirrorGen emit` gives the same stub removal; the only difference is the pre-existing `LogDocumentContents` placement drift.

## Parked — RichTextWithEmbeddedItem.ReproduceRemovedHyperlinkColors

- Member: `private void ReproduceRemovedHyperlinkColors(List<Inline>, Hyperlink)`, no lambda. Stays a `[MirrorStub]` in the `.g.cs`; no hand-written partial.
- Measured first mismatch (Release IL gate, 664 checked, 1 failure): `#25` Quire `brfalse.s` vs Mathcad `brtrue.s`; also `Enumerable.First` in Quire where Mathcad calls `Colors.get_Black` first.
- Mathcad materializes the `dictionary == null || dictionary.Count != 0` condition as a bool on the stack (`ldc.i4.1` / `cgt.un`, then `brtrue`), with no bool local. Two ternary forms (original and reversed `||`) did not reproduce that shape.
- Next step: find a source form that Roslyn lowers to this branch layout, or leave it parked.

## Parked — three RichTextWithEmbeddedItem members (no candidate this round)

- `GetEmbeddedObjectCaretOffset(object)`: no lambda. Mathcad evaluates `this` before the `TextPointer` temp for `GetInsertionPointAtPointer(ref ...)`. The `ref` argument must be a local, so the first difference is at #25 (`ldarg.0` vs Quire `ldloc.0`). No idiomatic C# form found that puts `this` first.
- `ValidateURL(Uri)` (static): no lambda. First difference at #9 (Quire `br.s` target 41 vs Mathcad 40); Quire has 59 entries vs Mathcad 56. Mathcad keeps `result` as `Nullable` with `ldloca` plus the constructor call. Not resolved.
- `OpenHyperlinkUrl(Uri)`: Mathcad IL keeps a literal `ldc.i4.0; brfalse.s` around the `ValidateURL` block (source decompiles as `if (false)`). Roslyn Release removes it, so the branch and call counts cannot match. Parked.

## Batch 32 — RichTextWithEmbeddedItem.HandleLinkOpsOnPreviewKeyDown

- Member: `private bool HandleLinkOpsOnPreviewKeyDown(Key key)`, no lambda, no closure. Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`; stub removed from the `.g.cs`.
- Key values checked against WPF: Back = 2, Return = 6, Delete = 32 (Space = 18 was the wrong name). Mathcad's switch sections are laid out Return, Back, Delete, so the `switch` lists Return first, with a one-line comment.
- Release IL (project-wide): PASS — 664 checked, 0 failures.
- Regeneration: `MirrorGen emit` removes the same stub. The `.g.cs` also shows the pre-existing `LogDocumentContents` placement drift, which is not part of this change. Emit also wrote unrelated whitespace and Generated-directory drift; reverted.

## Probe — CanModifyHyperlink ordinal (blocked, not landed)

- Hand-written implementation gates with `calls differ at #8`: Quire `<CanModifyHyperlink>b__93_0`, Mathcad `<CanModifyHyperlink>b__20_0`. The rest of the body matched up to that point.
- Cause: `Generated/` compiles before hand-written partials, and the generated class has at least 75 declaration lines (about 25 fields first) before any hand-written member. A partial member therefore cannot reach ordinal 20. The fix needs a MirrorGen change (Mathcad field and member order), not a hand-written reorder.
- Probe patch kept outside the repo, reversed after measurement.

## Batch 33 — RichTextWithEmbeddedItem.FindOncePrivate

- Member: `public override bool FindOncePrivate(string, bool, LogicalDirection, FindOptions, List<Pair<int,int>>)`, no lambda, no closure (foreach over `Cast` and `Reverse`). Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`; stub removed from the `.g.cs`.
- Measured mismatch: the first try (ternary assigning `textPointer`) failed on a block one byte short. Release IL stores `textPointer` in each arm of `if (direction == Forward)`, so the source is an if/else with an assignment in each arm, with a one-line comment.
- Release IL (project-wide): PASS — 665 checked, 0 failures.
- Regeneration: `MirrorGen emit` gives the same stub removal plus the pre-existing `LogDocumentContents` placement drift; emit-written drift outside `Generated/` was reverted. The solution build's 14 errors are in `tests/Quire.Mirror.Tests` (pre-existing, unrelated).

## Batch 34 — RichTextWithEmbeddedItem.CloneContent(RichTextWithEmbeddedItem, IEngineeringDocument, bool export)

- Member: `protected void CloneContent(...)`, no lambda, no closure. Uses `using (new CommandScope(new CompoundCommand()))`, a `foreach` over `WhiteboardItems.Reverse()`, and a `using` declaration for the popup. Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`; stub removed from the `.g.cs`. The `export` parameter is unused, as in Mathcad.
- Release IL (project-wide): PASS — 666 checked, 0 failures (first try).
- Regeneration: `MirrorGen emit` gives the same stub removal plus the pre-existing `LogDocumentContents` placement drift. Emit output elsewhere was reverted. The solution build's 14 errors are in `tests/Quire.Mirror.Tests` (pre-existing).

## Batch 35 — RichTextWithEmbeddedItem.InlayXamlWithEmbeddedItems

- Member: `private string InlayXamlWithEmbeddedItems(DocumentTextRange, string, out RichTextEmbeddedItemsInfo, List<IWhiteboardItem>)`, no lambda. Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`; stub removed from the `.g.cs`.
- Two shapes differ from the obvious source: (1) Release IL tests `Text` first with `bne.un` and falls through, so the dispatch is an if/else-if on a `TextPointerContext` local, not a `switch` (a `switch` gave `beq` tests with the body placed later); (2) `textElement` is declared first, so its local number matches Mathcad's. `adjacentElement` keeps no local (Roslyn `dup`s it), as in Mathcad.
- Release IL (project-wide): PASS — 667 checked, 0 failures.
- Regeneration: `MirrorGen emit` gives the same stub removal plus the pre-existing `LogDocumentContents` placement drift. Emit output elsewhere was reverted. The solution build's 14 errors are in `tests/Quire.Mirror.Tests` (pre-existing).

## Batch 36 — RichTextWithEmbeddedItem.EditLinkUndoableCommand

- Member: `private Hyperlink EditLinkUndoableCommand(HyperlinkEditState, DocumentTextRange, bool skipOneInsertionPosition = false)`, no lambda. Uses `using` scopes, a `try`/`catch`, and a pattern variable. Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`; stub removed from the `.g.cs`.
- Names are qualified where the partial's imports clash: `Quire.Core.TextFormat`, `Quire.Wpf.HyperlinkExtensions` (not the local `Controls.Text.HyperlinkExtensions`), `System.Windows.Documents.Paragraph` (not the local `Controls.Text.Paragraph`).
- Release IL (project-wide): PASS — 668 checked, 0 failures (first try).
- Regeneration: `MirrorGen emit` gives the same stub removal plus the pre-existing `LogDocumentContents` placement drift. Emit output was reverted. The solution build's 14 errors are in `tests/Quire.Mirror.Tests` (pre-existing).
