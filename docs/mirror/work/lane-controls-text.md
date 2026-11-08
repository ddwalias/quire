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

## Batch 6 — HyperlinkToScreenProvider

- Members: `.ctor`, `GetRectAroundTextPosition`, `ComputeExtend`, `TranslatePoint`, `PointToScreen`, `ActualHeight`, `ActualWidth`, `IsVisible`; readonly fields `_owner`, `_clickInTextRect`.
- Shapes: local declaration order matters (`position` before `oppositeDirection`, Mathcad local slots 11/12). The `lineBoundary` ternary must test `== Forward` first (Mathcad jumps to the Forward arm); the `!= Forward` form fails with "calls differ". The direction ternary `CompareTo(...) != 0 ? clickedPosition.LogicalDirection : oppositeDirection` matches.
- Release IL (project-wide, no `--type`): PASS — 99 checked, 99 exact, 0 failures, 616 unfinished.
- Remaining mismatches in this batch: none.

## Batch 7 — ToggleScriptUndoCommand

- Members: `Name`, `Do` (`using` on `CommandScope`), `RecreatedRange`, `RecreateSelection`, `Undo`, `Redo`; readonly fields (declaration order kept). Hand-written in `Mirrored/Controls.Text.ToggleScriptUndoCommand.cs` with `DocumentTextRange` alias (Quire has its own `TextRange`).
- Remaining stubs (kept in the `.g.cs`): constructor and `CreateToggleScriptUndoCommand`.
  - Constructor shape tried: sequential field assignments with one `TextPointer` local reused for start/end and `_insertionPoint = textRegion.Selection.IsNotEmpty() ? -1 : textRegion.SelectionStart`. The ternary matched; the `TextPointer` local did not. Mathcad keeps `this` on the stack across the `stloc` of each `GetInsertionPointAtPointer(ref ...)` argument, which this shape does not reproduce.
  - `CreateToggleScriptUndoCommand` uses a `dup`/`brtrue` null check followed by `FailAssertion` with no local; not yet tried.
- Release IL (project-wide, no `--type`): PASS — 106 checked, 105 exact, 0 failures, 610 unfinished.

## Batch 8 — RichText serialization and deserialization strategies

- `RichTextSerializationStrategy`: all members (`.ctor`, `SerializeRegionsEpilog`, `GetRegions`, `ConvertToRegionPersistantData`). Shapes: the region list is declared `IList<IRegionType>` (Mathcad's `ICollection<T>` calls); the return is `regions.Count <= 0 ? null : regions` (`ble` to the null arm).
- `RichTextDeserializationStrategy`: `.ctor`, `Regions`, `RegionFactory`, both `AddRegion` overloads, `DeleteRegion`, `IsRegionNotAffectingCalculation`. Shapes: the nested `if (eq != null) { if (type == 2) return true; return type == 0; } return true;` form and the `if (eq == null) return true;` form both failed ("Release IL differs"); the single conditional `eq == null || type == Evaluation ? true : type == Expression` matched.
- Added `ProjectReference` to `Quire.Controls.EquationControl` in the hand-written `Quire.Controls.Text.csproj` (the assembly map lists it as a dependency; no cycle). The generated `Mirror.props` is unchanged.
- Release IL (project-wide, no `--type`): PASS — 116 checked, 116 exact, 0 failures, 599 unfinished.

## Batch 9 — HyperlinkDialogViewModel

- Members: all (`.ctor`, `UserCanModifyText`, `ContextText`, `ContextLinkAddress`, `UserInputAddress`, `UserInputText`, `PerformedLinkEditOperation`, `UserInputUri`, `IsOkButtonEnabled`, `TryCreateUriFromAddress`, `GetState`, `UpdatePerformedLinkOperation`, `ValidateDialogInputsState`). Hand-written in `Mirrored/Controls.Text.HyperlinkDialogViewModel.cs`.
- Shapes that mattered:
  - `TryCreateUriFromAddress` returns `true` on error (Mathcad's flag). Used `resultUri = cond ? null : Try(addr, out resultUri) ? resultUri : new Uri(addr, UriKind.RelativeOrAbsolute)`: the `UriKind` value `0` is `RelativeOrAbsolute`, not `Absolute` (which is `1`). Using `Absolute` failed with "Release IL differs".
  - Setter with a shared `return`: `if (_userInputText == value || DebugTools.DevAssert(...).Failed) return;` matches the single shared `ret`; `if (eq) return;` followed by a separate assert-return does not.
  - `GetState`: early `return default` placed first failed; `if (!HasErrors) { return new ...; } return default;` matched.
  - `UpdatePerformedLinkOperation`: one assignment from a nested ternary (a single `set` call); two assignments in if/else failed.
- Release IL (project-wide): PASS — 134 checked, 134 exact, 0 failures, 581 unfinished.

## Batch 10 — HyperlinkContextValues

- Members: all (`.ctor`, `ConvertToLinkRange`, `ContextText`, `ContextLinkAddress`, `ContextLink`, `UserCanModifyText`). Hand-written in `Mirrored/Controls.Text.HyperlinkContextValues.cs`; the WPF `TextRange` is aliased as `DocumentTextRange` (Quire has its own `TextRange`).
- Release IL: PASS on the first shape — 142 checked, 142 exact, 0 failures, 573 unfinished.
- Note for maintainers: a gate run failed once with "disk was full" while writing `inventory.json` (the C: drive had 195 MB free). It passed after removing my own stale temp build trees and the `gate-il-*` directories whose `gate.json` names `mirror/controls-text-20261108`. Other lanes' gate directories were left alone (about 170 remain in the main checkout's `.mirror-cache`, roughly 48 GB).

## Batch 11 — constants and application settings

- `RichTextBaseConstants` (const only), `ProductInfo.Constants` (consts + `.ctor`), `Properties.Settings` (`defaultInstance`, `Default`, `DictionaryPath`, `UseWindowsDictionary`, `.ctor`). Hand-written in `Mirrored/`.
- Shapes: `Settings` getter for the `bool?` setting `((bool?)this["UseWindowsDictionary"]).GetValueOrDefault()` matched on the first try; the string getter `(string)this[...] ?? null` matched too (the `dup`/`brtrue` in the original came from the `?? null`).
- Attribute values copied from the original metadata (`DefaultSettingValue` "True" for `UseWindowsDictionary`), though the gate does not compare attributes.
- Release IL: PASS — 148 checked, 148 exact, 0 failures, 568 unfinished.

## Batch 12 — DictionaryHelper

- Members: all (`MaxDictionaryWordLength`, `DictionaryWordStopFlags`, `WpfSpellerCustomDictionaryUri`, `WindowsSpellerCustomDictionaryPath`, `IsWindowsSpellerCustomDictionaryValid`, the static constructor, `AddWordToDictionary`, `RemoveFromDictionary`, `UpdateCustomDictionary`, `SynchronizeWindowsSpellerCustomDictionary`). Hand-written in `Mirrored/Controls.Text.DictionaryHelper.cs`.
- Shapes: the static constructor is an explicit body (not field initializers), with the `DictionaryPath` default check under `DebugTools.IsDebug`. `UpdateCustomDictionary` uses lambdas over the two parameters; the `RemoveAll`/`Find` predicates compare with `StringComparison.Ordinal`. Every method matched on the first try.
- Release IL: PASS — 159 checked, 159 exact, 0 failures, 561 unfinished.

## Batch 13 — EmbededItemsCommandWithKeepSelection

- Members: all (`.ctor`, `HasCommandAction`, `HasSelection`, `Name`, `Do`, `Undo`, `Redo`, `MakeSelection`, `Execute`). Hand-written in `Mirrored/Controls.Text.EmbededItemsCommandWithKeepSelection.cs`.
- Shapes: `Do` with `using (CommandScope ...)` inside the `if`, `result` assigned in both branches, matched first try. Constructor: `if (HasCommandAction) { ...; return; } DebugTools.DevAssert(HasSelection);` matched.
- Release IL: PASS — 169 checked, 169 exact, 0 failures, 551 unfinished.

## Batch 14 — ItemToImagesRichTextWithEmbededItemCloneHelper (three of four members)

- Exact: `CreatePopup`, `Clone`, `Insert` (`Insert` keeps the `InlineUIContainer` in a local; a shape without the local was not tried because Mathcad creates the container before the `InsertBaselineOffsetTag` call and the local matched first time).
- Stub kept: the constructor. Mathcad stores `_resourceDict` (a `ResourceDictionary` whose `Source` is `pack://application:,,,/<self>;component/TextDictionary.xaml`) and `_engineeringDocument` **before** calling `Object::.ctor`. C# always calls the base constructor first, so no C# shape matches. The hand-written partial declares both fields without assigning them, and the generated `{ }` constructor stub is left unchanged, so `_engineeringDocument` stays null until the constructor is resolved. Clone/Insert/CreatePopup depend on it; this is a known gap.
- Release IL: PASS — 172 checked, 172 exact, 0 failures, 548 unfinished.

## Batch 15 — CustomSplitRichTextWithEmbeddedItem (simple members)

- Exact: internal `.ctor`, `HandleTextChanged` and `OnRenderSizeChanged` overrides (base call, then `_splitData = null`), `FireSplitItemIfNeeded` (`SplitItemIfNeeded?.Invoke(this)`), and `FlowDocumentSplitData.LineData` (`TextPtr`, `LineRect`, `.ctor`).
- Still stubbed: `GetValidSplitOffset`, `GetInvalidPointer`, `ValidateLayout`, the `FlowDocumentSplitData` members (`.ctor`, `FirstLineRect`, `LastLineRect`, `GetLineRect` x2, `EnsureLines`), the `_splitData` field and the `SplitItemIfNeeded` event declaration (add/remove are exact).
- Release IL: PASS — 179 checked, 179 exact, 0 failures, 541 unfinished.

## Batch 16 — FlowDocumentSplitData (constructor, line rects)

- Exact: `FlowDocumentSplitData(FlowDocument)` ctor, `FirstLineRect`, `LastLineRect`, `GetLineRect(int)`, and static `GetLineRect(TextPointer, TextPointer)`.
- Shape: `GetLineRect(int)` re-reads `line.LineRect` into the same nullable local before `Value` (`lineRect = line.LineRect; return lineRect.Value;`). `return line.LineRect.Value;` allocated a third local and differed.
- Still stubbed: `EnsureLines`, `GetValidSplitOffset`, `GetInvalidPointer`, `ValidateLayout`, and the `_splitData`/event declarations that are not methods.
- Release IL: PASS — 184 checked, 184 exact, 0 failures, 536 unfinished.

## Batch 17 — FlowDocumentSplitData.EnsureLines

- Exact on the first shape: `_lines = new List<LineData> { new LineData { ... } }` for the first line, a nested `while (next != null && ptr.CompareTo(next) == 0)` loop, and a final `DevAssert(last.TextPtr.CompareTo(_lastLinePtr) == 0).Passed` guard before setting `LastLineRect`.
- Release IL: PASS — 185 checked, 185 exact, 0 failures, 535 unfinished.

## Batch 18 — CustomSplit layout validation

- Exact: `ValidateLayout` (first shape) and `GetInvalidPointer` (protected). `GetInvalidPointer`'s last branch must be `if (!Selection.End.HasValidLayout) {...} else { result = null; }`; the ternary `cond ? null : Selection.End` shares one store and differed. The caret branch keeps the ternary (it matched).
- Release IL: PASS — 187 checked, 187 exact, 0 failures, 533 unfinished.

## Batch 19 — CustomSplit: GetValidSplitOffset and remaining declarations

- Exact: `GetValidSplitOffset` (binary search over lines with the closure `<>c__DisplayClass5_0`). Shapes that mattered:
  - The method is declared **first** in the hand-written partial. The closure ordinal (`DisplayClass5_0`) is the count of earlier members in the merged partial; with it after the constructor and overrides the ordinal was `8` and failed.
  - Selector lambda: `if (!(rect.Height <= pageHeight)) return originalOffset; return rect.Top;` matched. `rect.Height > pageHeight ? originalOffset : rect.Top` and the block `if (rect.Height > pageHeight) return orig` both laid out the branches in the wrong order (`bgt`/`ble.un` vs Release `ble`).
  - `int mid = lineNumberHint == 0 ? (count - 1) / 2 : lineNumberHint;` matched; `lineNumberHint != 0 ? lineNumberHint : (count - 1) / 2` placed the branches in the opposite order.
- Declarations moved from `Generated` to the hand-written partial (no IL, so no gate effect except the ordinal, which stayed correct): `_splitData`, `SplitItemIfNeeded` event, the four line/rect fields and `_lines` of `FlowDocumentSplitData`. The class now has no `[MirrorStub]` markers.
- Release IL: PASS — 190 checked, 190 exact, 0 failures, 532 unfinished.

## Batch 20 — RichTextTitle (constants, static command, simple overrides)

- Exact: `ToggleOrientation` static command (field initializer, `PlotsCategoryToggleAxisTitleOrientation`), `InnerSpacing`/`XYPlotControl`/`PolarPlotControl` constants, `IsDirty`, `_isSanitizing`, `_textStyleChangedHandler`, `InitialTitleText` and `ToggleOrientationRequest` auto-properties, `IsEditingAllowed` (`FunctionalityService` key 765), `SupportsLists`/`SupportsIndentation` (false), the parameterless ctor, `HandleTextChanged`, `OnStyleChanged` (calls `SetStyleFromTextStyles`), `ApplyTextStyleCanExecute`, `OnGotFocus`, `OnLostFocus` (selects ContentStart to ContentStart), `OnIsInPrintingPropertyChanged` (empty).
- Mismatch found and fixed: `OnLostFocus` selects from `ContentStart` to `ContentStart` in Release IL, not to `ContentEnd`.
- Still stubbed: `RichTextTitle(string)` ctor (uses `<>O` delegate caches), `TextStyleChangedHandler` property, `CanEnableSpellCheck`, `PrePasting`/`PostPasting`, `ToggleSubSuperscriptCommandCanExecute`, `SetRegionPersistentData`, the sanitize and measure helpers, `SetText`/`GetText`/`ResetText`/`ApplyInitialTextFormat`, and `RichTextTitle` ctor helpers.
- Release IL: PASS — 205 checked, 205 exact, 0 failures, 518 unfinished.

## Batch 21 — RichTextTitle text helpers and paste hooks

- Exact: `SetParagraphMarginToZero`, `SetText` (`TextAlignment.Center`), `ResetText`, `ApplyInitialTextFormat`, `ChangeTextRegionAccordingToRules` (`TextAlignment.Center`), `SetRegionPersistentData` (override), `PrePasting`, `PostPasting` (try/finally with `MathFormatForPaste.Instance?.Dispose()`).
- Findings: `TextAlignment.Right` is `1` and `Center` is `2` in WPF; Mathcad uses `2`. `string.Empty` loads a field; Mathcad loads `""`.
- Shape not reproduced: `GetText` (`new TextRange(...).Text.Replace(...)`). Mathcad calls `callvirt get_Text` on a freshly constructed `TextRange`; Roslyn emits `call` for a property read on a `new` receiver. A local would add a local slot Mathcad does not have. Left as a stub.
- Shape not reproduced: `SetInitialTitleText` needs `ApplyTextFormatValues` to run between `new TextRange(...)` and its `.Text` read, which requires a temporary. Left as a stub.
- Release IL: PASS — 213 checked, 213 exact, 0 failures, 510 unfinished.

## Batch 22 — RichTextTitle measure helpers (content, paragraph, run)

- Exact: `MeasureContentWidth` (foreach over `Document.Blocks`, `if (paragraph != null)` with no `break`; zero case tested first with `if (!(width > 0.0)) return 0.0; return width;`), `MeasureParagraphWidth`, `MeasureRunWidth`.
- Shape note: `if (width > 0.0) return width; return 0.0;` lays out the zero return first; Release IL branches on `bgt` to the width return.
- Still stubbed in RichTextTitle: `MeasureInlineWidth` (68 IL instructions), `SetInitialTitleText`, `GetText`, `TextStyleChangedHandler` (property setter), `CanEnableSpellCheck`, `ToggleSubSuperscriptCommandCanExecute`, `SanitizeUnsupportedFormatting`, `CollectUnsupportedBlocks`/`Inlines`, `RemoveHyperlinkPreservingText`, `ConvertListToParagraphs`, and the `RichTextTitle(string)` ctor.
- Release IL: PASS — 216 checked, 216 exact, 0 failures, 507 unfinished.

## Batch 23 — RichTextBase small members (21)

- Exact: `GetHelpContext`, `HighlightAll` (`throw new NotImplementedException()` is the Release IL body, not a MirrorStub), `CopyImageToTempBitmap`, `GetTextInRange`, `IsSelectedChangedHandler`, `ClearInsertionPointsCache`, `ClearIsInSerialization`, `ClearRevertCoerceAfterSerializationProcess`, `ClearTextFormatCache`, `DisableSpellCheck`, `OnIsInPrintingPropertyChanged`, `OnIsInSavingCLRPropertyChanged`, `RemoveBaselineOffsetFromImage`, `SimulateMouseClick`, `CacheScriptStateInRange`, `FireGotFocusEvent`, `GetIsInSerialization`, `GetPointerToInsertionPoint`, `HandleIsOnVisiblePageChanged`, `HandleIsVisibleChanged`, `OnLoaded`.
- Hand-written: new `Mirrored/Controls.Text.RichTextBase.cs`. Also the `DocumentTextRange` alias is needed here (Quire.TextRange).
- Release IL: PASS — 237 checked, 237 exact, 0 failures, 486 unfinished.

## Batch 24 — RichTextBase serialization flags and focus (8)

- Exact: `SetIsInSerialization`, `SetRevertCoerceAfterSerializationProcess`, `OnIsInCopyPropertyChanged`, `WrapUndoAtSerialization` (the `return` after `StartCommandScope()` is the shape that matched; `if/else` did not), `ShouldSerializeProperty` (`SpellCheck.IsEnabledProperty`), `OnGotFocus`, `CacheTextFormat`, `SetSelection`.
- Release IL: PASS — 245 checked, 245 exact, 0 failures, 478 unfinished.

## Batch 25 — RichTextBase input, paste and copy hooks (7)

- Exact: `OnDrop` (`IsOnDrop` set around `base.OnDrop`), `ExecuteToggleScriptUndoCommand`, `PastingHandler`, `AttemptActivatingSpellCheck`, `OnPreviewTextInput`, `Copy` (try/finally with `ClearIsInCopy`), `FirePropertyChanged` (local handler copy).
- Release IL: PASS — 252 checked, 252 exact, 0 failures, 471 unfinished.

## Batch 26 — RichTextBase region data, property-changed guards, paste and caret handlers (6)

- Exact: `DoGetRegionPersistentDataInternal` (object initializer for `textType`: `Document`, `S11NProvider`), `OnProhibitedPropertyChanged` (`DevAssert(false)`), `UpdateCurrentProofingLanguage`, `OnInheritedOnlyPropertyChanged` (`ReadLocalValue(e.Property) == UnsetValue`), `OnPreviewEnter` (`object.Equals(sender, this)`), `CanExecutePasteIntoTextRegion`.
- Release IL: PASS — 258 checked, 258 exact, 0 failures, 465 unfinished.

## Batch 27 — RichTextBase helpers (6)

- Exact: `LoadXamlContent` (try/catch with `DebugTools.LogError(ex.ToString())`), `UpdateCurrentFormats` (`GetIsSelected(this) ? (TextRange)TextRange : Selection`; the negated form `!GetIsSelected ? Selection : TextRange` laid out the arms the other way and differed), `GetSpellingErrorInPosition`, `ClearSplitFontSizeDataQueue`, `SetCaretOffsetAtContentEnd`, `SetCaretPositionAndClearSuggestedX`.
- Not done: `GetTextOnlyFromRuns` (static; the Release IL calls `GetTextBoundary` with no receiver, but the generated stub declares `GetTextBoundary` as an instance method; to be checked against the original metadata).
- Release IL: PASS — 264 checked, 264 exact, 0 failures, 459 unfinished.
