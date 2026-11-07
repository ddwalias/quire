# Lane CtlTxt — Quire.Controls.Text

- Owner: Orca task `task_2c7b98c21fbc` (maintainer-assigned; no WORK.md lane row).
- Branch: `mirror/controls-text-20261108`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/controls-text-20261108`.
- Baseline (main `c312205e`): 36 checked/exact, 677 unfinished methods.

## Batch 1 — TextRegionModel

- Members: `.cctor`, `HandleDocumentChanged`, `Text` get/set, `Document` get/set, `.ctor` (fields `TextProperty`, `DocumentProperty`, `TextChangedEvent` moved to the hand-written partial).
- Hand-written: `src/Quire.Controls.Text/Mirrored/Controls.Text.TextRegionModel.cs`; stubs removed from `Generated/Mirror/Controls.Text.TextRegionModel.g.cs`.
- Release IL: PASS — 7 of 7 selected methods exact on first shape; 42 exact total, 0 failures, 672 unfinished.
- Remaining mismatches in this batch: none.

## Batch 2 — hyperlink helpers, converters, commands

- Members: `HyperlinkExtensions.HasSimpleTextContent`; `HyperlinkProperties` (`.cctor`, `Get/SetLinkVisited`); `UriToAddressConverter` (`Instance`, `.cctor`, `Convert`, `ConvertBack`, `.ctor`); `XmlLanguageHelper` (`.cctor`, `ConvertXmlLanguageToCultureInfo`, `ConvertCultureInfoToXmlLanguage`); `HyperlinkToolTipConverter` (`Instance`, `.cctor`, `Convert`, `ConvertBack`, `.ctor`); `HyperlinkCommands` (`.cctor`, explicit static ctor); `IWhiteboardItemInTextWrapper.WrappedItem` (declaration).
- Constant-only `HyperlinkInputValidationErrorKeys` moved to a hand-written partial; `--type` on it fails with "selected type not found" (no IL members), so it is not passed to the gate.
- Hand-written: `src/Quire.Controls.Text/Mirrored/Controls.Text.<Type>.cs`; stubs removed from the matching `Generated/Mirror/*.g.cs`.
- Release IL: PASS — 58 checked, 0 failures (`--type` for the seven types above). Shapes tried: field initializers for static fields (no explicit static ctor, beforefieldinit); explicit `static` ctor where Mathcad has no beforefieldinit (`HyperlinkCommands`), `XmlLanguageHelper` try/catch as `CultureInfo culture; try { ... } catch (Exception ex) { ... }` — matched first time.
- Remaining mismatches in this batch: none.

## Batch 3 — modify-hyperlink parameters, clone helpers

- Members: `ModifyHyperlinkCommandParameter` (`.ctor`, `ShowDialog`, `UserInputText`, `UserInputAddress` getters as `{ get; }`, `ResultHyperlink`, virtual `ResultIsSingleLinkEditOperation`); `ContextMenuModifyHyperlinkCommandParameter` (`.ctor`, `GenericLabel`, `SingleLinkEditLabel`, `CurrentLabel`, override `ResultIsSingleLinkEditOperation` setter); `IRichTextWithEmbededItemCloneHelper` (declarations); `DefaultRichTextWithEmbededItemCloneHelper` (`.ctor`, `CreatePopup`, `Clone`, `Insert`).
- Override setter shapes: `if (value != base.X) { base.X = value; PropertyChanged?.Invoke(...); }` → "Release IL differs" (Roslyn emits `beq` to the end); guard clause `if (value == base.X) { return; }` → exact.
- Hand-written: `Mirrored/Controls.Text.{ModifyHyperlinkCommandParameter,ContextMenuModifyHyperlinkCommandParameter,IRichTextWithEmbededItemCloneHelper,DefaultRichTextWithEmbededItemCloneHelper}.cs`. The override + event stubs of ContextMenu that were exact in main are kept in the `.g.cs` (`PropertyChanged` event stays generated); the override setter moved to hand-written.
- Release IL: PASS — 77 checked, 0 failures; 74 exact total, 640 unfinished.
- Remaining mismatches in this batch: none.

## Batch 4 — HyperlinkEditState

- Members: `HyperlinkEditState` (struct `.ctor`, `PerformedLinkEditOperation`, `UserInputText`, `UserChangedText`, `UserInputUri` getters as `{ get; }`). Backing fields follow declaration order (Performed, UserInputText, UserChangedText, UserInputUri); the ctor assigns them in Mathcad's order (Performed, UserChangedText, UserInputText, UserInputUri), which the first shape matched.
- Release IL: PASS — 79 checked, 0 failures; 79 exact total, 635 unfinished.
- Remaining mismatches in this batch: none.
- Known blocker (not in batch): XAML-generated `InitializeComponent` bodies (TextRegion, HyperlinkDialog, HyperlinkDialogErrorsResourceDictionary) load `/Ptc.Controls.Text;component/<name>.xaml`; the Quire project has no matching XAML resources, so these bodies cannot match without a string that points at the wrong assembly. Needs maintainer decision.
