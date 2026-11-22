# Lane checkpoint: Quire.Controls.Text (agy)

- **Owner:** agy
- **Branch:** `feature/controls-text-agy`
- **Assembly:** `Quire.Controls.Text` (`Ptc.Controls.Text.dll`)

## Batch 1 (Ready to land)
- Completed all remaining members (0 `.g.cs` stubs left) for:
  - `Quire.Controls.Text.HyperlinkDialog`
  - `Quire.Controls.Text.Paragraph`
  - `Quire.Controls.Text.RichTextTitle`
  - `Spirit.FindReplaceRichTextBox`
- **Release IL:** PASS (`607 checked`; report `.mirror-cache/release-il-b224b9e1a214/release-il.json`)
- **Remaining:** `RichTextWithEmbeddedItem` (45 `.g.cs` stubs), `RichTextBase` (50 `.g.cs` stubs).

## Batch 11 — RichTextWithEmbeddedItem accessors

- Owner: agy (this worktree, `feature/controls-text-agy`, based on main `88eb55a8b`).
- Members: `TextStyleChangedHandler` get/set (with backing field `_textStyleChangedHandler`, moved to the hand-written partial), `DeserializeMathFormat` set, `DeserializeLabeledIdFormat` set.
- Shapes: the `StyleChanged += OnStyleChanged` method group matches Mathcad's `ldvirtftn RichTextBase::OnStyleChanged` (a `((RichTextBase)this).OnStyleChanged` cast fails with CS1540 protected access).
- Blocked: `IsTabNavigatable` (`WhiteboardItems.Any(lambda)`) stays a stub. Its lambda is emitted as `<get_IsTabNavigatable>b__105_0` while Mathcad has `b__149_0`; the closure ordinal counts earlier lambda-bearing members in the class, so the placeholder must reproduce those members' closures first.
- Release IL (project-wide): PASS — 611 checked, 0 failures; 120 generated stubs remain in this project.
- Remaining: `RichTextBase` (≈164 stubs before this batch), `RichTextWithEmbeddedItem` (≈130 stubs, including the closure-ordinal blocked members above).

## Attempt — RichTextBase SelectionStart / SelectionEnd (not landed)

- Shape `TextPointer start = Selection.Start; return GetInsertionPointAtPointer(ref start);` fails: Mathcad pushes `this` as the receiver of `GetInsertionPointAtPointer` before `Selection.Start` is loaded into the local (`ldarg.0; ldarg.0; call get_Selection; ...; stloc.0; ldloca.s 0`), while Roslyn emits the receiver after the local initializer. Same unresolved receiver-retention shape as batch 5 (`CompareBaselineOffset`) and batch 7 (`ToggleScriptUndoCommand`).
- Reverted; both stubs stay in `Generated/Mirror/Controls.Text.RichTextBase.g.cs`.

## Batch 12 — RichTextWithEmbeddedItem.InsertBaselineOffsetTag

- Member: `InsertBaselineOffsetTag` (static, `internal`). Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`; stub removed from the `.g.cs`.
- Shape: `GetPropertyValue` and `ApplyPropertyValue` on a `new DocumentTextRange(...)` receiver emit `call`; Mathcad uses `callvirt`. Routing each receiver through a named local produces `callvirt`. The ternary is `fontWeight != FontWeights.Bold ? FontWeights.Bold : FontWeights.DemiBold`.
- Tried and reverted: `RichTextBase.ToggleRangeToState`. Mathcad stores the second `GetPositionAtTextOffset` result in a local after pushing the first argument, which the natural `new TextRange(A, B)` shape does not reproduce (first difference at #6).
- Release IL (project-wide): PASS — 612 checked, 0 failures; 119 generated stubs remain.

## Batch 13 — RichTextWithEmbeddedItem.CopyHyperlink

- Member: `CopyHyperlink` (`private void`, `ExecutedRoutedEventArgs`). Field `_copyLinkText` moved from the `.g.cs` to the hand-written partial.
- Shape: Mathcad wraps the body in one positive guard, `if (hyperlink != null && DebugTools.AssertNotNull(hyperlink.NavigateUri, null).Passed) { try { ... } finally { _copyLinkText = null; } }`. A separate early-return guard and a helper method both fail (the helper adds a call Mathcad does not make).
- Release IL (project-wide): PASS — 613 checked, 0 failures; 118 generated stubs remain.
- Not attempted in this batch: `EditLinkUndoableCommand` (795 bytes, a `TextFormat`/`CommandScope`/`DeclareChangeBlock` try/finally tree) and `ModifyHyperlink(ModifyHyperlinkCommandParameter)` (396 bytes, with `DevAssert` and a `switch` over `LinkEditOperation`). Both are larger than one independent batch and remain stubs.

## Batch 14 — RichTextWithEmbeddedItem.ModifyHyperlink(ModifyHyperlinkCommandParameter)

- Member: `internal void ModifyHyperlink(ModifyHyperlinkCommandParameter param)`. Hand-written in `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`; stub removed from the `.g.cs`.
- Shapes that matched: `!param.ShowDialog || ShowHyperlinkDialog(...)` as one guard around `GetState` and the `DevAssert`; `DevAssert(state.PerformedLinkEditOperation != None, ...)`; switch sections in source order Delete, Insert/Edit, default, None (Mathcad's code layout); `state.PerformedLinkEditOperation` re-read for the Edit comparison and the final cleanup condition (Mathcad calls the getter each time); `DevAssert(parentInlines == null, null).Failed` before `Remove` (Mathcad's literal condition).
- Calls `EditLinkUndoableCommand` (still a stub), which only matters at runtime.
- Release IL (project-wide): PASS — 614 checked, 0 failures.

## Attempt — RichTextWithEmbeddedItem.EditLinkUndoableCommand (not landed)

- Wrote the full method from its Release IL (CommandScope, DeclareChangeBlock, foreach/try-finally over `Selection.EnumerateInlinesInRange`, the `IsNotEmpty` / `UserChangedText` / try-catch branches, the `BlockUIContainer` path, the `CommandBuffer` undo loop). The project compiles with it; the gate's first difference is at instruction 58 and, after a diagnostic temporary, at instruction 81. Reverted to the stub; nothing from this attempt is committed.
- Blocker 1 (instruction 58): Mathcad calls `blockContainer.get_Child()`, then `set_Child(null)`, then `new InlineUIContainer(child)`. Roslyn emits the constructor after the `Child` read's use, so the set cannot precede it without a temporary. Mathcad's local list has no `UIElement` slot, so a temporary changes the locals. This is the receiver-retention/argument-order family already recorded for SelectionStart/End.
- Blocker 2 (instruction 81): Mathcad calls `TextFormat.ApplyTextFormat(range)` with `call` on the `textFormat` local; Roslyn emits `callvirt` for a plain public instance method on a reference-type local. Not yet resolved.
- Remaining unverified: after 81 the gate does not report further differences for this method, so the rest of the body is unchecked.

## Batch 15 — RichTextBase.SetRegionPersistentData

- Member: `public virtual IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider, object)`. Hand-written in `Mirrored/Controls.Text.RichTextBase.cs`; stub removed from `Generated/Mirror/Controls.Text.RichTextBase.g.cs`.
- Shape: `textType text = (textType)data` then `text.DeS11NProvider = ...` matched first time (the local is not in Mathcad's list but the gate passed, so no extra slot was emitted).
- Release IL (project-wide): PASS — 615 checked, 0 failures.

## Probe — EditLinkUndoableCommand blockers (follow-up)

- Blocker 1 (get_Child / set_Child(null) before the InlineUIContainer constructor, no UIElement local): not reproduced by any source form tried. A `??`/ternary or a tuple wrapper introduces branches or temporaries Mathcad does not have; a private helper would be a new member without a Mathcad counterpart. Still blocked.
- Blocker 2 (`call` on the `TextFormat` local for ApplyTextFormat): not probed with a gate run; Roslyn emits `callvirt` for a plain public instance method on a reference-type local. Still blocked.

## Probe — SubscriptsMatch (not landed)

- Nested `if (!IsNotEmpty) { if (IsFullySubscript) return true; }` followed by `if (SequenceEqual) return true; return false;` fails at instruction 2: Mathcad branches the first test straight to the compute block, and returns both `true` cases through one shared `ldc.i4.1; ret`. Reverted to the stub.

## Probe — RichTextBase.ClearSelectedRegionFormatProperties (not landed)

- Body matched except the closure identity: `CommandBuffer.Execute(ToggleScriptUndoCommand.CreateToggleScriptUndoCommand(this, TextRange, ScriptState.None, false), null)`, `Document.ApplyTextFormatValues(textFormat)`, `using (DeclareChangeBlock()) Document.Blocks.Each(block => block.ClearTextFormatPropsRecursively((TextElement x) => true, TextFormat.ClearFormatPropsList))`. The gate's first difference is the `ldftn` of the outer lambda.
- Expected (Mathcad): `<>c::<ClearSelectedRegionFormatProperties>b__113_0` and nested `b__113_1`. Emitted with the method at the top of the hand-written partial: `b__117_0` / `b__117_1`. Emitted with the method at the end of the class: `b__297_0` / `b__297_1`.
- The ordinal therefore follows the method's position among the class's members in compile order (generated `.g.cs` members first), not the lambda count alone. Hypothesis: the top position counts four members that Mathcad's order does not, located in the generated partial (not verified). Placeholder stubs would only add to the count, and the generated file cannot be reordered by hand.
- Parked: no safe placement found that yields 113 in this worktree. Reverted to the stub.

## Trace — RichTextBase closure ordinals (evidence)

- Mathcad (`members.tsv`) closure ordinal vs. containing method's row among the 217 RichTextBase method rows (token order): (2, 0), (81, 31), (93, 39), (95, 41), (113, 58), (122, 65), (159, 98), (165, 103), (169, 106), (207, 135), (261, 177), (297, 203). The offset grows from 2 to 94, so the ordinal is not a method row index plus a constant.
- Quire: the closure-bearing methods on `main` have no RichTextBase closures apart from the probe, so there is no in-class anchor to regress against. The probe method sits at generated declaration position 66 (of 105 generated member lines) and the emitted ordinal is 117 with the method at the top of the hand-written partial, 297 at the end.
- Where the evidence runs out: the reference IL lists fields first and the decompiled `.cs` regroups members by kind, so Mathcad's source interleaving of fields, properties, events and accessors is not recoverable from the cache. Until that interleaving is known, the declaration(s) responsible for the 4-step gap at the top of the partial cannot be named.

## Landed — RichTextBase.ClearSelectedRegionFormatProperties (closure ordinal 113)

- Measured (disposable probes): a method in a folder that compiles first gets `b__0_*`; with the method at the top of the hand-written partial the ordinal is `b__117_*`, and at the end of the class `b__297_*`. The ordinal is the count of counted members (fields, accessors, nested types, the auto-property backing fields and the implicit `.cctor` included) in compile order, with all generated members first. Generated members precede `113` by 117.
- Arrangement that gives exactly 113: remove the `ClearSelectedRegionFormatProperties` stub and the four throw-only stubs that follow it in Mathcad order (`IsFullySubscript`, `BuildSubscriptPositions`, `GetPointerToAddDictionary`, `PostPasting`) from the generated file, and declare the method first in the hand-written partial with those four as unmarked placeholders after it. Four members leave the counted prefix, so the count is 117 - 4 = 113.
- Gate (project-wide, LF tree): PASS — 620 checked, 620 exact, 0 failures, 115 unfinished (placeholders are unchanged throws, so they do not block), 46 missing.
- Caveat: this relies on the generated stubs being replaced by hand-written unmarked declarations, which regeneration handles by omitting them. Regeneration was not run to confirm the exact output.

## Verification and next step

- MirrorGen parity (disposable worktree, all 47 production projects built in Release, `emit`): regenerated `RichTextBase.g.cs` is byte-identical to the landed generated file, so the five omitted stubs match regeneration. `emit` also changes `RichTextWithEmbeddedItem.g.cs` (LogDocumentContents placement) and `CustomSplitRichTextWithEmbeddedItem.g.cs` (blank line); these drifts are present on `main` and predate this change.
- Next closure candidate `SetStyleFromTextStyles` (display class ordinal 105): its metadata row (50) precedes `ClearSelectedRegionFormatProperties` (row 58), but MirrorGen emits the stub after it, so reaching 105 would need reordering several generated declarations. Not attempted; blocked on the MirrorGen ordering difference.

## Probe — SetStyleFromTextStyles layout (parked; not landed)

- Moving `SetStyleFromTextStyles` and the seven stubs that follow it in the generated file (`HandleApplyTextStyle`, `DoApplyTextStyle_internal`, `HandleClearTextFormat`, `ClearFormatPropertiesInRange`, `HandleToggleSubscriptCommand`, `HandleToggleSuperscriptCommand`, `HandleToggleSubSuperScriptCommand`) to the head of the hand-written partial, ahead of `ClearSelectedRegionFormatProperties`, gives the lambda display class `<>c__DisplayClass105_0`, the Mathcad ordinal. The layout is confirmed; the body is not.
- Body shape mismatches (first difference): `runIndex = 0` is stored before `EnumerateRunsInRange` runs in Quire, but after it in Mathcad; `"Normal".Equals(...)` is `callvirt` in Quire and `call` in Mathcad; the `styleKey == null ? ... : ...` condition compiles to a different branch layout (6 vs 8 conditional branches).
- Toggle group (`HandleToggleSubscriptCommand` 118, `HandleToggleSuperscriptCommand` 119): the toggles would have to sit after `ClearSelectedRegionFormatProperties` and its four placeholders. They are counted members of the generated prefix, so removing them lowers the `SetStyleFromTextStyles` count below 105, and restoring 105 with 113 for `ClearSelected` needs extra counted members (padding). Blocked by that count arithmetic.

## Batch 16 — RichTextBase.ApplyTextFormatPropertyValueToControl

- Member: `public void ApplyTextFormatPropertyValueToControl(DependencyProperty, object, Block block = null)`. Hand-written in `Mirrored/Controls.Text.RichTextBase.cs`; stub removed from the `.g.cs`.
- Shapes that matched on the first try: `(block as List)?.ListItems.FirstListItem.Blocks.FirstBlock as Paragraph` (the null-conditional chain), and `paragraph?.Inlines.FirstInline != null ? paragraph.Inlines.FirstInline.GetValue(property) : value` as the `SetTextFormat` value argument. The `WPF Paragraph` is qualified because `Quire.Controls.Text.Paragraph` shadows it in this file.
- Release IL (project-wide): PASS — 621 checked, 0 failures.

## Batch 17 — RichTextBase.OnCorrectSpellingError

- Member: `private void OnCorrectSpellingError(object, ExecutedRoutedEventArgs)`. Hand-written in `Mirrored/Controls.Text.RichTextBase.cs`; stub removed from the `.g.cs`.
- Shape: Mathcad returns early through a `ret` placed right after the negated three-part guard, and then runs the body (`if (!(... && ... && ...)) return;`). The block form `if (... && ...) { body }` branches to the end and fails at instruction 8. The WPF `TextRange` is written as `DocumentTextRange`; `out string partialWord` is the out local, which matches Mathcad's local order.
- Release IL (project-wide): PASS — 622 checked, 0 failures.

## Parked — VerticalAlignmentOffset and CalculateBaseline (evidence)

- `VerticalAlignmentOffset` getter: Mathcad pushes the `GeneralTransform` receiver (`IsMeasureValid`-guarded `AssertNotNull(FindCommonVisualAncestor)` ternary, `Transform.Identity` or `TransformToAncestor`) before reading `Padding.Top` into a local and computing `Baseline + top`. A C# expression cannot store a local between a receiver and its arguments without a `dup`, so this is the receiver-retention shape. Parked.
- `CalculateBaseline`: the logic is expressible, and the full build differs at exactly two instructions (`get_Text` on `new DocumentTextRange(...)`): Mathcad emits `callvirt`, Roslyn emits `call`. The same mismatch blocked `InsertBaselineOffsetTag` until a named local was added, which Mathcad does not have here. Reproducible probe: `new DocumentTextRange(a, b).Text` in Quire compiles to `call`; the Mathcad IL for the same source uses `callvirt`. Parked as a shared compiler-policy difference.
- Next unattempted groups (not started): `ReplaceTextInHyperlink` (lifted `int?` arithmetic with copies between `Nullable<int>` locals), `AddBlockedSelectDirectionForNextElement` (nullable comparisons), `OnPreviewBackspace` (`Nullable<int>` and `ICustomFontSizeOwner`), `HandleReplace`, `VerifyInsertionPositions` (interpolated string handler), `SearchWholeWordRuns` (`ValueTuple` lists with `Reverse`).

## Batch 18 — RichTextBase.AddBlockedSelectDirectionForNextElement

- Member: `private void AddBlockedSelectDirectionForNextElement(LogicalDirection)`. Hand-written in `Mirrored/Controls.Text.RichTextBase.cs`; stub removed from the `.g.cs`.
- Shapes that mattered: `LogicalDirection.Backward` is `0` (the Mathcad branch on `brtrue` falls through for Backward; `Forward` is `1`); `TextPointerContext.None` (not `Text`) is the null-position context; the direction ternary is written `direction == LogicalDirection.Backward ? 22 : 41` (the `!=` form emits the branch arms the other way round).
- Release IL (project-wide): PASS — 623 checked, 0 failures.

## Parked — ContextMenuOpened (evidence)

- The first half (the `foreach` over `Enum.GetValues(typeof(TextMenuItemCommands))` with `continue` guards and the `Header` copy) and the second loop structure match in shape; the static `_contextMenuOpening` guard and the early returns match.
- Local slots: `Type[] keyTypes` declared at the method's top level takes slot `[2]` (Roslyn allocates top-level locals first). Mathcad keeps it in slot `[8]`, so it has to be declared in a nested block after the first loop; the nested block fixes the slot.
- Remaining mismatch (instruction #92): Mathcad stores `handled = false` (`ldc.i4.0; stloc.s 10`) between `ldloc keyTypes; ldloc i; ldelem.ref` and the `Enum.GetValues` call. A C# `bool handled = false;` at the top of the loop body always stores before the element load. No source form found that places the store after the element load without a `dup`. Parked (shape blocker, evidence above).

## Batch 19 — RichTextBase.HandleReplace

- Member: `protected virtual void HandleReplace(object, ExecutedRoutedEventArgs)`. Hand-written in `Mirrored/Controls.Text.RichTextBase.cs`; stub removed from the `.g.cs`.
- Shape: Mathcad materializes `ValueToFind != null && ValueToFind.Equals(Selection.Text)` into a value before the `IsInvalidOperation` test. The direct `if (A && B && C) return;` short-circuits to the end and differs at instruction 56. Naming the first two terms as a `bool` local matches the materialized form.
- Release IL (project-wide): PASS — 624 checked, 0 failures.

## Batch 20 — RichTextBase.ReplaceTextInHyperlink

- Member: `private void ReplaceTextInHyperlink(string, Hyperlink, bool, ScriptState, List<Pair<int,int>>, int, out bool)`. Hand-written in `Mirrored/Controls.Text.RichTextBase.cs`; stub removed from the `.g.cs`.
- Shapes that mattered: Mathcad keeps only `originalLength` and `hyperlinkLength` as named locals. The later lengths are inline expressions evaluated inside the `out` assignment (`shouldRestoreStyle = (hyperlink != null ? (int?)hyperlink.GetText().Length : null) == hyperlinkLength - originalLength + text.Length && !(originalLength == hyperlinkLength);`). Named `int?` locals for `delta`, `newEnd` or a scoped block shift the temporary slots and differ at the first ternary's null temp.
- Release IL (project-wide): PASS — 625 checked, 0 failures.

## Batch 21 — RichTextBase.VerifyInsertionPositions

- Member: `private void VerifyInsertionPositions()`. Hand-written in `Mirrored/Controls.Text.RichTextBase.cs`; stub removed from the `.g.cs`.
- Shapes that mattered: `_cachedInsertionPoints.Concat(Document.ContentEnd.Yield())` for the foreach source; `previous` and `current` locals (copies of `last` and `insertionPoint` taken at the top of each iteration, used later in the inner loop) match Mathcad's slots `[3]` and `[4]`; the assertion message is an interpolated string (`DefaultInterpolatedStringHandler`), which matches as written.
- Release IL (project-wide): PASS — 626 checked, 0 failures.
