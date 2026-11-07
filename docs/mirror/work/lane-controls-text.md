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

## Batch 5 — WhiteboardItemInTextWrapper

- Members: `.cctor` (MarginProperty override), `.ctor` x2, `WrappedItemForXamlSerialization` get/set, `WrappedItem` get/set, `OnRequestBringIntoView`, `HandleWrappedItemSizeChanged`, `MeasureOverride`, `SetBaselineOffset`, `ShouldSerializeContent` (now `=> false`), field `_wrappedItem`; class attribute `[ContentProperty("WrappedItem")]` (Mathcad's class-level attribute) on the hand-written partial.
- Shapes: getter `if (!GetIsInCopyNonSerializable(this)) return null; return _wrappedItem;` → exact (the ternary and positive `if` gave `brfalse`, Mathcad uses `brtrue`).
- Remaining mismatch: `CompareBaselineOffset` stays a marked stub in the `.g.cs`. Shape tried: `if (!IsMeasureValid) return; DebugTools.DevAssert(TextBlock.GetBaselineOffset(this) == -WrappedItem.VerticalAlignmentOffset);`. Mathcad's IL stores `-VerticalAlignmentOffset` in a `float64` local between the `GetBaselineOffset` call and the `ceq`, which this shape does not produce. Not yet resolved.
- Release IL (project-wide, no `--type`): PASS — 92 checked, 91 exact, 0 failures, 624 unfinished. Note: `--type` also checks the remaining stub and fails on the stub's `NotImplemented` caller-path arguments, so the project-wide run is the acceptance run.
