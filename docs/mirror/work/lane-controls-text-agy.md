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
