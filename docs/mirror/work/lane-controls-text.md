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

## Batch 28 — RichTextBase command and split-data helpers (9)

- Exact: `ApplyTextStyleCanExecute` (base form, `IsActive || IsSelected`), `ClearTextFormatCanExecute` (`if (!IsActive) { if (IsSelected) ...; return false; } return true;`), `CanAddToDictionary` (`word.Length <= 128`), `DequeueSplitFontSizeData`, `EnqueueSplitFontSizeData`, `GetRegionPersistentData` (try/finally around `IsInSaving`), `InitializePasteCommand`, `CanSetCurrentProofingLanguage`, `GetScriptType`.
- Release IL: PASS — 273 checked, 273 exact, 0 failures, 450 unfinished.

## Batch 29 — RichTextBase caret, style, clipboard and selection helpers (5)

- Exact: `CaretShouldLeaveTextRegionInDirection`, `OnStyleChanged` (`if (!IsVisible) { canvas...; if (canvas == null || canvas.IsVisible) return; }`), `ExtractXamlFromClipboard`, `RestoreSelection`, `CloneContent`.
- Release IL: PASS — 278 checked, 278 exact, 0 failures, 445 unfinished.

## Batch 30 — RichTextBase context menu and paste event (2)

- Exact: `OnContextMenuOpening` (`DevAssert(!_contextMenuOpening)`, then a try/finally on the flag) and `RaiseEvent(IDataObject)` (`DataObjectPastingEventArgs` with `RoutedEvent = DataObject.PastingEvent`).
- Release IL: PASS — 280 checked, 280 exact, 0 failures, 443 unfinished.

## Status at batch 30 (checkpoint)

- Landed on `main` through batch 30. Project-wide Release IL: 280 checked, 280 exact, 0 failures, 443 unfinished.
- Stub markers left in `Generated/Mirror`: `RichTextBase` (~300), `RichTextWithEmbeddedItem` (~400), `Paragraph` (38, 3 of them XAML), `FindReplaceRichTextBox` (38), `RichTextTitle` (25), `HyperlinkDialog` (17, XAML), `TextRegion` (13, XAML), `HyperlinkDialogErrorsResourceDictionary` (7, XAML), `ToggleScriptUndoCommand` (3), `WhiteboardItemInTextWrapper` (2), `ItemToImagesRichTextWithEmbededItemCloneHelper` (1, constructor), `ContextMenuModifyHyperlinkCommandParameter` (1, event declaration).
- Blocked shapes: XAML `InitializeComponent`/`Connect` (maintainer decision); `ItemToImages` constructor (base call after field stores); `WhiteboardItemInTextWrapper.CompareBaselineOffset`; `ToggleScriptUndoCommand` constructor and `CreateToggleScriptUndoCommand`; `RichTextTitle.GetText`/`SetInitialTitleText` (`callvirt` on a `new` receiver, temporary between construction and use); `RichTextBase.GetTextOnlyFromRuns` (static `GetTextBoundary` vs instance stub signature).
- Not yet started: most of `RichTextBase` (about 80 member names, including lambdas and iterators) and `RichTextWithEmbeddedItem`, `Paragraph` and `FindReplaceRichTextBox` members.

## Batch 31 — RichTextBase command scope, subscript and find/replace helpers (4)

- Exact: `StartCommandScope` (`TryGetValue` with early return), `ToggleSubSuperscriptCommandCanExecute` (`if (!Selection.IsEmpty) return Length > 0; return true;`), `ShouldLeaveTextRegionOnCaretMove` (`!= Up && != Down` guard, then `GetLineStartPosition(..., out lines)`), `CanReplace`.
- Not matched: `GetPointerToAddDictionary`. Release IL pushes `this` before `CaretPosition` is stored in the caret local, so the call uses `ref caret` with a duplicated `ldarg.0`; no C# shape reproduced that. Left as a stub.
- Release IL: PASS — 284 checked, 284 exact, 0 failures, 439 unfinished.

## Batch 32 — RichTextBase spell-check timer, dictionary refresh, paste and RTF helpers (4)

- Exact: `OnPasteCanExecute`, `StartOrStopSpellCheckActivationTimer` (`if (start) { ... } else { ... }`, with the handled test inside the true branch and no `return`), `RefreshDictionary`, `SerializeRangeContentToRtfString` (static, nested `using`).
- Not matched: `PostPasting`. Its `Each` lambda compiles to `<PostPasting>b__N_0`, and N is the method's ordinal in the merged type (Mathcad 159). With `PostPasting` in the hand-written partial the merged ordinal is 221, because the remaining generated members come first and cannot be reduced without implementing them. Left as a stub; blocked on lambda ordinal.
- Release IL: PASS — 288 checked, 288 exact, 0 failures, 435 unfinished.

## Batch 33 — RichTextBase spell-check toggle and print-state handler (2)

- Exact: `ApplySpellCheckValue` (`if (!value) Clear(); else { if (exists && !valid) Add; }` inside try/catch), `HandleIsInPrintingChanged` (early return on `NewValue`; `while` loop with `base.Undo()` inside a `using (CommandScope)`).
- Not matched: `SelectTo`. Release IL evaluates `Selection.IsEmpty` with a `brtrue` to the caret arm placed last and stores the anchor once through a shared `stloc.0`. Neither a ternary (arm order) nor if/else (per-branch stores) reproduced that. Left as a stub.
- Release IL: PASS — 290 checked, 290 exact, 0 failures, 433 unfinished.

## Batch 34 — RichTextWithEmbeddedItem small members (first group, 17)

- New hand-written partial `Mirrored/Controls.Text.RichTextWithEmbeddedItem.cs`. Exact: `Accept`, `Clone` (virtual), `EndInit`, `OnStyleChanged`, `OnToggleHideFromCalculationContext`, `CanToggleHideFromCalculationContext`, `ClearSavedState`, `ClearState`, `SaveState`, `RemoveItem`, `GetLineStartPosition`, `GetFirstLine`, `GetLastLine`, `UNITTEST_ExecuteAddItemToListCommand`, `ExecuteDecreaseFontSize`, `ExecuteIncreaseFontSize`, `CanOpenOrEditOrCopySingleHyperlink` (both overloads; the bool one is `GetContextLinks()?.Count == 1`).
- Not matched: `LogDocumentContents` (empty body in Release IL, but the method header differs: Mathcad uses a fat header with `.maxstack 6` and a `[Conditional("DEBUG")]` attribute; neither the attribute nor the plain form matched). Left as a stub.
- Release IL: PASS — 307 checked, 307 exact, 0 failures, 416 unfinished.

## Batch 35 — RichTextWithEmbeddedItem command, hyperlink and state-change handlers (10)

- Exact: `UNITTEST_ExecuteRemoveItemFromListCommand`, `GetHyperlinksInRange` (`EnumerateHyperlinksInRange(range, allowEmpty).ToList()`), `RemoveHyperlinks`, `AddItem`, `CanExecuteMathFormatCommands`, `GetHyperlinkFromTextPointer`, `HandleChildItemRequestRemove`, `OnIsInCopyPropertyChanged`, `OnIsInPrintingPropertyChanged`, `OnIsInSavingCLRPropertyChanged` (each base call then `UpdateHyperlinkCanHaveToolTip()`).
- Note: `TextRange` inside `Quire.Controls.Text` resolves to `Quire.TextRange` now that main has it; WPF signatures need the `DocumentTextRange` alias.
- Skipped (lambda-ordinal risk): `AddItems`, `RemoveItems`, `MoveItems`, `EnumerateEmbeddedObjectsInRange`.
- Release IL: PASS — 317 checked, 317 exact, 0 failures, 406 unfinished.

## Batch 36 — RichTextWithEmbeddedItem calculatable events, caret and hyperlink helpers (13)

- Exact: `GetNextLine`, `FireCalculatableAdded/Removed/Moved`, `GetItem`, `ProhibitMouseClickOnItem`, `CanDeleteActiveOrSelectedItems`, `GetHyperlinksFromWholeDocument`, `GetHyperlinkAtCaretPosition`, `CanBookmarkTag`, `OnDragOver`, `SetSelection` (override), `OpenHyperlink`.
- Release IL: PASS — 330 checked, 330 exact, 0 failures, 393 unfinished.

## Batch 37 — RichTextWithEmbeddedItem tooltip, paste, activation and command-state helpers (8)

- Exact: `UpdateHyperlinkCanHaveToolTip` (`!(IsInPrinting || IsInCopy) && !IsInSaving`), `CanRemoveHyperlinks`, `IsItemRemovedFromDocument`, `RestoreSavedState`, `PasteItem`, `ActivateActiveItem`, `CanRemoveBookmarkTag`, `CanSetNumberFormat`.
- Left as stubs: `CanModifyHyperlink` (both overloads, the bool one is not yet read) and `HasHyperlinkInMouseContext` (ternary layout not checked yet). Removing a stub removes every overload with that name, so these were kept together.
- Release IL: PASS — 338 checked, 338 exact, 0 failures, 385 unfinished.

## Batch 38 — RichTextWithEmbeddedItem clone, math format, offset and hyperlink-removal helpers (7)

- Exact: `CloneRichText`, `ExecuteClearMathFormat`, `ExecuteCommandKeepingSelection`, `GetEmbeddedItemSymbolOffset`, `RemoveHyperlinkUndoableCommand` (both overloads; the `Hyperlink` overload uses `link != null ? ... : null`, the form Mathcad emits with `brtrue`), `ContainsSimpleText`.
- Release IL: PASS — 345 checked, 345 exact, 0 failures, 378 unfinished.

## Batch 39 — RichTextWithEmbeddedItem key handling, word range and context-menu handling (3)

- Exact: `OnPreviewKeyDown` (override; early `Handled` return before the base call), `GetWordRange` (returns `System.Windows.Documents.TextRange`, so the alias is required), `SetItemContextMenuHandling`.
- Release IL: PASS — 348 checked, 348 exact, 0 failures, 375 unfinished.

## Batch 40 — RichTextWithEmbeddedItem focus, commit and context-menu overrides (3)

- Exact: `OnContextMenuOpening` (override; returns early when not focused), `CommitItem`, `HandleChildItemGotFocus`.
- Release IL: PASS — 351 checked, 351 exact, 0 failures, 372 unfinished.

## Batch 41 — RichTextWithEmbeddedItem calculation errors, caret hyperlink, bookmark removal and item list helpers (7)

- Exact: `ContainsError`, `ShowError` (foreach over `CalculatableChildren` with `as ICalculationErrorOwner`), `GetHyperlinkAtCaretPositionIncludingEdges` (`??` chain; the second adjacent-hyperlink direction is `dir == Forward ? Backward : Forward`, the only form that gives Mathcad's `beq`), `HandleRemoveBookmarkTag`, `RemoveEmbeddedItem`, `RemoveItemFromList`, `GetContainer`.
- Release IL: PASS — 358 checked, 358 exact, 0 failures, 365 unfinished.

## Batch 42 — RichTextWithEmbeddedItem context links, drop check, number/matrix format and warning dialogs (5 names, 6 members)

- Exact: `GetContextLinks` (`?.Yield().ToList()` on the caret hyperlink), `CanDrop` (`System.Windows.DragEventArgs`, the `Quire.Controls.Core` type is ambiguous), `OnSetMatrixFormat`, `OnSetNumberFormat` (`Command == ClearResultFormat ? CurrentNumberFormatClear : CurrentNumberFormat`), `ShowWarningDialog` (both overloads).
- Release IL: PASS — 364 checked, 364 exact, 0 failures, 359 unfinished.

## Batch 43 — RichTextWithEmbeddedItem descendant item, URL resolution and exactly-one hyperlink (3)

- Exact: `GetDescendantItem` (early return of the local `item`), `ResolveUrl` (`out Uri` from `UrlUtils.TryCreateAbsoluteUri`), `CheckAndGetExactlyOneHyperlink` (`Failed ? null : FirstOrDefault()`; the `if (Failed) return null;` form gives the wrong branch sense).
- Not matched: `GetEmbeddedObjectCaretOffset`. Release IL pushes `this` before the pointer local is stored and loads it by `ldloca`, so `GetInsertionPointAtPointer(ref pointer)` with a named local does not reproduce it. Left as a stub (same shape as `GetPointerToAddDictionary`).
- Release IL: PASS — 367 checked, 367 exact, 0 failures, 356 unfinished.

## Batch 44 — RichTextWithEmbeddedItem question dialog and mouse hyperlink context (2)

- Exact: `ShowQuestionWarningDialog` (`ShowQuestion(..., YesNo) == Yes`), `HasHyperlinkInMouseContext` (`element != null ? element.GetTextElementParentOrSelf<Hyperlink>() : GetHyperlinkFromTextPointer(GetPositionFromPoint(Mouse.GetPosition(this), false))`; the other arm order puts the calls in the wrong IL order).
- Not matched, left as a stub: `InsertBaselineOffsetTag`. Mathcad calls `GetPropertyValue` and `ApplyPropertyValue` with `callvirt` on a `new TextRange(...)` receiver; Roslyn emits `call` for a `new` receiver. Same blocker as `RichTextTitle.GetText`.
- Release IL: PASS — 369 checked, 369 exact, 0 failures, 354 unfinished.

## Batch 45 — RichTextWithEmbeddedItem add-embedded-item overload and clipboard embedded-item check (2)

- Exact: `AddEmbeddedItem(IWhiteboardItemInText, bool)` (the other two overloads stay stubs; the stub line for this overload is removed by its signature only, since `rmstub` removes every overload with the name), `DataObjectContainsAnyEmbeddedItems` (static, optional `filter`; the Mathcad `Nullable` is a pattern temporary, so the source is `is RichTextEmbeddedItemsInfo info`, not a named `info` local).
- Release IL: PASS — 371 checked, 371 exact, 0 failures, 352 unfinished.

## Batch 46 — RichTextWithEmbeddedItem embedded-item find and hyperlink modify command state (2)

- Exact: `FindInEmbeddedItem` (static; `restrictor.FindOnce(...)` through the `IFindRestrictor` extension), `CanModifyHyperlink(object, CanExecuteRoutedEventArgs)` (command handler only; the parameterless `CanModifyHyperlink()` overload is still a stub, so its stub line is kept and only the handler's stub line is removed).
- Not attempted: `DeleteActiveOrSelectedItems`. Mathcad calls `RichTextBase::Activate` with `callvirt`, but this class overrides `Activate`; the binding does not match a plain `Activate(true)` call. Needs a look before implementation.
- Release IL: PASS — 373 checked, 373 exact, 0 failures, 350 unfinished.

## Batch 47 — RichTextWithEmbeddedItem modify-hyperlink command and region persistent data (2)

- Exact: `ModifyHyperlink(object, ExecutedRoutedEventArgs)` (command handler; `e.Parameter as ModifyHyperlinkCommandParameter ?? new ...(true, null, null)`; only the handler's stub line is removed, the parameter overload stays a stub), `SetRegionPersistentData` (override; base call, then the `textType` regions deserialized through `_deserializationStrategy` and the caret moved to `Document.ContentStart`).
- Not attempted: `CanModifyHyperlink()` (bool overload). Its IL contains a `<>c` lambda cached in a static field (`<CanModifyHyperlink>b__20_0`), so the closure ordinal must match first.
- Release IL: PASS — 375 checked, 375 exact, 0 failures, 348 unfinished.

## Batch 48 — RichTextWithEmbeddedItem deactivate, bookmark tag command and hyperlink display (3)

- Exact: `DeactivateActiveItem` (protected virtual; `info == null || !info.IsCommitted` before `Commit`), `HandleBookmarkTag` (command handler; `Failed` early return, then `Passed && ShowBookmarkTagsDialog(...)`), `DisplayHyperlink` (optional `showAboutBox = true`; `SetLinkVisited`, then the functionality check with `OpenHyperlinkUrl` or the freemium about-box command).
- Not attempted: `ValidateURL` (its `FileInfo`/`DirectoryInfo` expression leaves the `FileInfo` on the stack across the `DirectoryInfo` store, which I have not reproduced), `GetWhiteboardItemsInRange` and `ExecuteRemoveItemFromListCommand` (closure and anonymous-type LINQ, ordinal first).
- Release IL: PASS — 378 checked, 378 exact, 0 failures, 345 unfinished.

## Batch 49 — RichTextWithEmbeddedItem item-list change dispatch, offset map and whole-range XAML copy (3)

- Exact: `HandleItemsListChanged` (`switch` on the action with Add, Remove, Move cases, each storing its `Cast` in its own local, and `default` calling `FailAssertion`), `UpdateItemOffsetMap` (foreach over `WhiteboardItems`, remove or store the offset), `CopyWholeRangeXamlToDataObject` (the `out RichTextEmbeddedItemsInfo info` must be an inline out variable: a separate declaration puts the `info` local in the wrong slot).
- Release IL: PASS — 381 checked, 381 exact, 0 failures, 342 unfinished.

## Batch 50 — RichTextWithEmbeddedItem preview-enter, placeholder replacement and wrapper set (3)

- Exact: `OnPreviewEnter` (`castclass` to `IWhiteboardItemInText`, `Activate(true)` on the base binding matches), `ReplacePlaceholderWithEmbeddedItem` (`var paragraph` avoids the `Paragraph` name clash with `Quire.Controls.Text.Paragraph`), `SetItem` (`container.Child?.GetType().FullName` inside the failure message).
- Release IL: PASS — 384 checked, 384 exact, 0 failures, 339 unfinished.
- Note: the earlier question about `Activate(true)` binding (batch 46) is answered by this batch: calls on `this` bind to the base method in IL, so `DeleteActiveOrSelectedItems` should follow the same shape.

## Batch 51 — RichTextWithEmbeddedItem delete command and embedded wrapper lookup (2)

- Exact: `DeleteActiveOrSelectedItems` (the `Activate(true)` question from batch 46 is settled: it binds to the base method, see batch 50), `GetWrapper` (static; `foreach` over `Grid.Children` with the duplicate check and `break`, the `break` lands on the `return result`).
- Not attempted: `OpenHyperlinkUrl` (depends on `ValidateURL`, which is not implemented), `RangeContainsOnlyInlineElements` (the loop condition mixes `&` and `&&`, and the source form is unclear).
- Release IL: PASS — 386 checked, 386 exact, 0 failures, 337 unfinished.

## Batch 52 — RichTextWithEmbeddedItem hyperlink navigation and key-down data (2)

- Exact: `NavigateToHyperLink` (try/catch around the mouse position, `DevAssert(...).Failed` fallback to `ElementStart`, `IsActive` branch with `Focus()`), `CollectKeyDataForPostChangeHyperlinkInsertion` (`Key.Tab`, `Key.Return`, `Key.Space`; the nullable is `Passed ? new PreviewKeyDownData(...) : (PreviewKeyDownData?)null`). The key numbers in IL are 3, 6, 18, which are Tab, Return, Space.
- Not attempted: `UpdateHideFromCalculationButton` (its `Any` predicate is a `<>c` cached lambda, so closure ordinal first).
- Release IL: PASS — 388 checked, 388 exact, 0 failures, 335 unfinished.

## Batch 53 — RichTextBase copy marking and whole-text RTF save (2)

- Exact: `OnPreviewCopy` (`!e.OriginalSource.Equals(this) || !IsActive` early return, then `SetIsInCopy`), `SaveWholeTextToRtf` (`try`/`finally` around `StartCommandScope`, `TextRange.Save(..., Rtf, true, false)`, `EndCommandScope`).
- Not matched, left as stubs: `HandleToggleSubscriptCommand` and `HandleToggleSuperscriptCommand`. Mathcad's `<HandleToggleSubscriptCommand>g__IsSubscriptElement|118_0` local function has ordinal 118, while the same local function in this merged type compiles to ordinal 290. Roslyn also caches the static method-group delegate in a `<>O` field, which Mathcad's IL does not do. Blocked on ordinal and caching; same as the closure-ordinal blockers.
- Release IL: PASS — 390 checked, 390 exact, 0 failures, 333 unfinished.

## Batch 54 — RichTextBase selection match, text boundary and paste handler (3)

- Exact: `SelectMatch` (written as `LogicalDirection.Forward == direction`; the constant must be the left operand to match Mathcad's `ldc.i4.1; ldarg.3`), `GetTextBoundary(direction, out partialText, stopFlags = FindWord)` (instance overload; the `Forward` arm is caret to end, the other arm is start to caret; the static 4-argument overload stays a stub), `OnPasteExecuted` (try/catch that logs `ex.ToString()`).
- Not attempted: `CreateTextChangedCommand`. Mathcad stores `_undoRunsTextStyleKey = null` after `newobj` while the command is still on the stack, and no C# statement shape produces that.
- Release IL: PASS — 393 checked, 393 exact, 0 failures, 330 unfinished.

## Batch 55 — RichTextBase navigate-into-item caret placement (1)

- Exact: `NavigateIntoItem(Direction)` (`Activate(true)`, then `switch` with `Left`/`Up` placing the caret at `ContentEnd` and `Right`/`Down` at `ContentStart`, then `return null`).
- Not matched, left as a stub: `SubscriptsMatch`. Mathcad returns through one shared `ldc.i4.1; ret` reached by two `brtrue` branches (from `IsFullySubscript` and from `SequenceEqual`), and no plain `if`/`return` form reproduces that single return block. Tried the `if (!IsNotEmpty && IsFullySubscript) return true;` form; the first branch differed.
- Release IL: PASS — 394 checked, 394 exact, 0 failures, 329 unfinished.

## Batch 56 — RichTextBase word range at caret and forward/whole-word search (3)

- Exact: `GetWordRangeAtCaretPosition` (`out` word text, boundary checks through the instance `GetTextBoundary`, `partialStart + partialEnd`), `SearchForward` (`while` over `rangeText` with `searchStart = start + 1`; the `break` on no index is the shape that matches), `TryFindWholeWord` (`GetInsertionPointAtPointer(ref start) - originalRangeStartPoint + index.Value`).
- Not attempted: `GetRangeIfMatch` (Mathcad's `new TextRange(p1, p2)` with `p1` duplicated on the stack, which no plain local form reproduces).
- Release IL: PASS — 397 checked, 397 exact, 0 failures, 326 unfinished.

## Batch 57 — RichTextBase backward search and subscript text replacement (2)

- Exact: `SearchBackward` (`while (length > 0)` with `break` on no index, then `length = index.Value`; the same shape as `SearchForward`), `ReplaceText` (`preserveScriptState` branch with `ToggleRangeText(..., double.NaN)` and `return`; otherwise `Selection.Text = string.Empty`, an unused `new Run(text, Selection.Start)`, and a `foreach` over the subscript pairs that calls `ToggleRangeToState(..., Second + 1, ScriptState.Subscript)`).
- Release IL: PASS — 399 checked, 399 exact, 0 failures, 324 unfinished.

## Batch 58 — RichTextBase same-type run ranges (1)

- Exact: `GetSameTypeRunRanges` (`for` from index 1 comparing run `GetType()` values; `ValueTuple` pairs added with `result.Add((runStart, runs[i - 1].ContentEnd))`).
- Not matched, left as stubs:
  - `ToggleRangeToState`. Mathcad keeps the second `TextPointer` in a local (`stloc.0; ldloc.0`) between its two `GetPositionAtTextOffset` calls and the `TextRange` constructor. A plain expression leaves out that local, so the body differs.
  - `OnCorrectSpellingError`. Mathcad tests `IsNullOrEmpty(word)` with `brfalse` to the body and places a `ret` between the checks. The `&&` form emits `brtrue` to the end, so the layout differs.
  - `SetRegionPersistentData` (base). Mathcad casts `data` to `textType`, duplicates it for the `DeS11NProvider` setter, and then reads `Document`. No plain statement form produces that `dup`.
- Release IL: PASS — 400 checked, 400 exact, 0 failures, 323 unfinished.

## Batch 59 — RichTextBase copy handler, find in text, and spelling-error recovery (3)

- Exact: `CopyingHandler` (protected virtual; RTF serialization of the selection, then `ClearIsInCopy`), `FindOncePrivate` (public virtual; `FindInText` gate, then the caret goes to the start for `Forward` and the end otherwise), `SpellingErrorCorrected` (tuple deconstruction of `_spellingErrorRange.Value`; the end pointer uses `startPointer?.GetPositionAtOffset(end)`, which emits `call` as Mathcad does; the failure uses `FailAssertion((string)null)` because the `null` literal is ambiguous).
- Not attempted: `EndCommandScope` (the queue and active-scope tail is long; left for a later batch), `GetSpellingErrorAtPosition` (try/catch with `ref` and `dup`).
- Release IL: PASS — 403 checked, 403 exact, 0 failures, 320 unfinished.

## Batch 60 — RichTextBase end of command scope (1)

- Exact: `EndCommandScope` (protected virtual). The ref-count check with `FailAssertion((string)null)`, the queue and active-scope bookkeeping, `if (ActiveScopes.Count != 0 || _isInCopyUndesiredCommandScope == null) return;` (one shared return, which is the only layout that matches), the `SetRevert…` foreach, and then `try { while (CanUndo) Undo(); } finally { foreach … ClearRevert…; SerializedObjectsToReleaseFromScope.Clear(); }`. The final `_isInCopyUndesiredCommandScope` dispose and null store follow the try.
- Release IL: PASS — 404 checked, 404 exact, 0 failures, 319 unfinished.
- Not matched, tried and reverted: `CalculateBaseline` (`new TextRange(...).Text` uses `callvirt` on a `new` receiver, which Roslyn emits as `call`); `ExecuteAddItemToListCommand` (closure display class ordinal is 188 here against 117 in Mathcad, and moving the method to the top of the partial only moved it from 287 to 188).

## Batch 61 — text-offset positions and inline-only range check (2)

- Exact: `RichTextBase.GetPositionAtTextOffset` (static; the `offset` parameter is reduced in place, and the result is `current ?? from.DocumentEnd`), `RichTextWithEmbeddedItem.RangeContainsOnlyInlineElements` (the loop condition is `(start != null && start.CompareTo(end) < 0) & result`; without the parentheses C# binds `&` tighter and the IL differs).
- Not attempted: `ReplaceTextInHyperlink` (nullable arithmetic with many temporaries that do not map to a plain source form).
- Release IL: PASS — 406 checked, 406 exact, 0 failures, 317 unfinished.

## Batch 62 — RichTextBase find within a range (1)

- Exact: `FindInRange` (protected, non-virtual as the stub declares it; `subscriptPositions?.Count == 1` gives the `call` on `get_Count` that Mathcad emits, where `!= null && Count` gave `callvirt`; the comparison is `options.HasFlag(…)` through boxed `Enum.HasFlag`; `isWholeSubscript` is a plain `bool` and `IsNullOrEmpty(...) | isWholeSubscript` is non-short-circuit).
- Note: `FindOncePrivate` (batch 59) calls `FindInRange` with `callvirt`, so it matched only once `FindInRange` was non-virtual; the stub's modifier is the one to keep.
- Release IL: PASS — 407 checked, 407 exact, 0 failures, 316 unfinished.

## Batch 63 — RichTextBase spelling error at a text position (1)

- Exact: `GetSpellingErrorAtPosition(ref TextPointer, out SpellingError)` (`spellingError = GetSpellingError(position = textPointer) ?? GetSpellingError(position = textPointer.GetReversedTextPointer())` inside a try; `catch { DevAssert(false); }`; the return is `if (spellingError != null) return position; return null;`).
- Not matched, tried and reverted: `SearchWholeWordRuns`. Mathcad pushes `this` before it stores `originalRange.Start` in the local used by the `ref` argument, and a plain `TextPointer` local followed by `GetInsertionPointAtPointer(ref …)` does not reproduce that. The `break` form and the separate `int startOffset;` declaration both fixed the rest of the method; only this one ordering remains.
- Release IL: PASS — 408 checked, 408 exact, 0 failures, 315 unfinished.

## Batch 64 — RichTextBase text-changed handler (1)

- Exact: `HandleTextChanged` (protected virtual; the `_cachedBaseline` reset and insertion-point cache clear, a `CommandScope` around the content-changed event and the create-and-reactivate call, then `UpdateCurrentRuns` with the create test and `args.Changes?.FirstOrDefault()?.RemovedLength > 0` (a lifted comparison, which gives the temporary Mathcad keeps), `CurrentTextFormat = SelectionTextFormat`, and the change-level guarded script-state and spelling-error updates).
- Note: the new-run argument and the removed-length comparison must both stay inline in the call; a local for either changes the IL order.
- Release IL: PASS — 409 checked, 409 exact, 0 failures, 314 unfinished.

## Batch 65 — RichTextBase selection-changed handler (1)

- Exact: `HandleSelectionChanged` (protected virtual; paste completion in `try`/`finally`, the run, script-state and text-format updates, the blocked-direction setup, and the revive-after-backspace block that applies `BaselineAlignment.Baseline` and the font size from the insertion state).
- Note: `BaselineAlignment` values in this WPF build are Baseline=3, Top=0, Center=1, Bottom=2, TextTop=4, TextBottom=5; the IL constant 3 is `Baseline`, not `TextTop`.
- Not matched, tried and left as the stub: `HandleReplace`. Mathcad evaluates `query.ValueToFind != null && query.ValueToFind.Equals(Selection.Text)` as a value (`ldc.i4.0` and `br` inside the `&&`), and the plain `if` form emits branches only.
- Release IL: PASS — 410 checked, 410 exact, 0 failures, 313 unfinished.

## Batch 66 — RichTextBase selection command gating (1)

- Exact: `ShouldNoOpOnSelection` (command-enable gate for the directional select commands; each `EditingCommands.Select*` compares by reference and sets `CanExecute` from the blocked-select direction flag, then the unexpected-command assertion).
- Not attempted in this batch: `OnPreviewBackspace` (reverted; Mathcad branches the first clause with `beq` to the body, which a plain `if` does not produce; the `?.` offset and the font-size ternary arm order did match once adjusted).
- Release IL: PASS — 411 checked, 411 exact, 0 failures, 312 unfinished.

## Batch 67 — RichTextBase split font-size data before a property apply (1)

- Exact: `CreateSplitFontSizeDataBeforeApplyPropOnRange` (returns early on an empty range; for the start and end runs it enqueues the font size when the inline's font size is set locally, `BaseValueSource.Local`).
- Note: the value-source check must be written inline (`DependencyPropertyHelper.GetValueSource(...).BaseValueSource == BaseValueSource.Local`). A named `ValueSource` local takes a different slot from the compiler temporary Mathcad uses.
- Release IL: PASS — 412 checked, 412 exact, 0 failures, 311 unfinished.
- Tried and reverted: `IsFullySubscript` (the cached lambda is `<IsFullySubscript>b__293_0` here against `b__297_0` in Mathcad, so the closure ordinal is short by four).

## Batch 68 — RichTextBase formatting-change handler and subscript caching on type-over (2)

- Exact: `HandleTextFormattingChanged` (protected/private stub kept the same; `Selection` range, widened to the caret word for a font-size change on an empty selection, split font-size data, `TextFormat.ApplyPropertyValueFormat`, then `Focus()` when active), `CacheSubscriptStateOnTypeOver` (written as one nested `if (!Selection.IsEmpty)` block; an early `return` on empty selection gives a different branch layout).
- Release IL: PASS — 414 checked, 414 exact, 0 failures, 309 unfinished.

## Batch 69 — RichTextBase pre-paste state (1)

- Exact: `PrePasting` (protected virtual; `PasteStartPositionOffset` from the caret or selection start, the split font-size queue, the single-owner check that enqueues the font size when the selection is inside one custom-size inline, then `CurrentInsertionScriptState = null`).
- Release IL: PASS — 415 checked, 415 exact, 0 failures, 308 unfinished.
