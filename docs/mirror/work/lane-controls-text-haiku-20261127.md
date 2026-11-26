# Quire.Controls.Text checkpoint (haiku, 2026-11-27)

- Owner: dispatched worker `task_243faeb802be`. Branch `mirror/controls-text-haiku-20261127`, from main `3ad58204c`. Not landed: the batch is below the 20-member landing minimum.
- Survey (generated `MirrorStub` lines, `src/*/Generated`): Quire 3509, Quire.Efi 6037, Quire.EquationEditor 5327, Quire.Mcd 3167, Quire.Engine.Native 3090, Quire.Mpl.Modules 1693, Quire.Controls.EquationControl 1761, Quire.Controls.Worksheet 1441, Quire.Controls.Plots 1206, Quire.Engine 315, Quire.Controls.ExcelComponent 186, Quire.Controls.Core 57, Quire.Calculation 25, Quire.Controls.Text 14 generated plus 37 hand-written placeholders, Quire.Controls 7, Quire.Componentsmgd 4, Quire.Msclr 3, Quire.Controls.SolveBlock 2. Quire.Media3D, Quire.Engine.Misc, Quire.Core and Quire.Utils have no remaining stubs or placeholders.
- Claimed: Quire.Controls.Text. Last Text commit was 2026-11-26 (`feature/haiku-controls-text-land`, `controls-text-land` worktree clean since 11-26). No dirty worktree found for Text lanes, so no active worker was detected.

## Members exact in Release IL (this checkpoint)

- `ToggleScriptUndoCommand` private constructor: moved from `.g.cs` to `Mirrored/Controls.Text.ToggleScriptUndoCommand.cs`; the stub is removed from the generated file.
- `ContextMenuModifyHyperlinkCommandParameter.PropertyChanged`: moved to the hand-written partial; the stub is removed.
- `RichTextWithEmbeddedItem.GetEmbeddedObjectCaretOffset`: implemented.
- `RichTextBase.OnPreviewBackspace`: implemented.

Release IL: `python tools/validation/release_il.py --base main --project Quire.Controls.Text` — PASS, 777 checked, 0 failures.

## Parked (kept as placeholders)

- `HandleToggleSubscriptCommand`, `HandleToggleSuperscriptCommand`: a static local function passed as a delegate makes Roslyn cache it in `RichTextBase.<>O`; Mathcad does not. The new cached fields shift `SetFormattedContent`'s `<>O` index (`<2>__RemoveBaselineOffsetFromImage`), so the whole group is parked until the cached-delegate shape can be avoided.
- `HandleToggleSubSuperScriptCommand`: first branch difference at IL #71 (Quire `brfalse 50`, Mathcad `brfalse 56`) around the `CurrentInsertionScriptState` ternary.
- `GetTextFormatForWholeDocument`: Mathcad allocates the display class for the `styleKey` lambda at method entry; Roslyn allocates it inside the `if` block.
- `LogDocumentContents`: `[Conditional("DEBUG")]`; Release IL keeps `maxstack 6` and the locals from the body, while the C# empty body emits `maxstack 8`.
- Earlier parked items remain listed in `lane-controls-text-richtext-ordinal.md` (`GetPotentialLinkEndPosition`, `GetWhiteboardItemsInRange`, `IsInvalidOperation`, `ToggleRangeToState`, `CalculateBaseline`).

## Remaining

- Hand-written placeholders still open: about 35 in `RichTextBase.cs` and `RichTextWithEmbeddedItem.cs` (see `grep -rn MirrorStub.NotImplemented src/Quire.Controls.Text/Mirrored`), and 14 generated stub lines in `RichTextBase.g.cs` (`SyncTextChangedCommand` fields, constructor, `Undo`, `Redo`) which need a nested-class closure layout.
- `RichTextBase.SyncTextChangedCommand`: nested closures at ordinals 9 and 10, plus `<>c`/display-class ordinals. Requires the Mathcad order layout before any member can land.
