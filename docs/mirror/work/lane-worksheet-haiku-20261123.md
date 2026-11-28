# Lane Quire.Controls.Worksheet (haiku-worksheet-20261123)

- Owner: Claude worker (task_9a74b07c793c)
- Branch: `feature/haiku-worksheet-20261123` (worktree `C:/Users/DuyDang/orca/workspaces/Quire/haiku-worksheet-20261123`), from `main` be8d3e6b6

## Batch 1 — CombinedInvalidConverter (Convert)

- Members: `CombinedInvalidConverter.Convert` (stub removed from `.g.cs`, implemented in `Mirrored/`). `ConvertBack` was already hand-written.
- Shape: `values[i] is bool f && f` pattern tests, combined with bool `&`. Release IL: PASS (gate, `--type Quire.Controls.Worksheet.CombinedInvalidConverter`).
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: see the batch receipt.

## Batch 2 — WorksheetControlExtensions.SeparateOverlappedItems

- Member: `SeparateOverlappedItems(this WorksheetControl, Dimension)` (stub removed from `.g.cs`, implemented in `Mirrored/`). Shape: `(IInputElement)(ActiveItem as IMathboardControl) ?? (IsContentActive ? control : ...)`; the `if`/`else` form differed at the `dup` of the null test. Release IL: PASS (1264 checked, 0 failures).

## Parked — Accept overrides (`visitor as IVisitorFuncOfRegionPersistentData ?? throw ...`)

- `WorksheetPageBreakPersistentData`, `TemplatedTextBoxPersistentData`, `SpecTableRegionPersistentData` (and the parked ones in the earlier checkpoint): Mathcad emits `isinst; dup; brtrue.s` with **no** `pop` on the null path; Roslyn emits `pop` for `?? throw`, and the `is`-pattern and switch-expression forms use a local. First difference at #3 (Quire 14 entries, Mathcad 13). Not reshaped further; stubs kept. Shared across all Accept overrides in the assembly.

## Batch 3 — CollapsibleAreaHeightConverter (Convert, ConvertBack, constructor) + ProjectReference

- `Quire.Controls.Worksheet.csproj` gains a direct `ProjectReference` to `Quire.Controls.CollapsibleArea` (hand-written csproj, not `Generated/Mirror.props`). Evaluated closure of CollapsibleArea (Mirror, Calculation.Core, Controls.Core, Controls.Whiteboard, Core) does not reach Worksheet: acyclic.
- Members: `CollapsibleAreaHeightConverter` constructor, `Convert` and `ConvertBack` (stubs removed from `.g.cs`, implemented in `Mirrored/`). Shape: `worksheetPageLayout.GridCellSize.Height` read directly; a named `Size` local differed (Mathcad stores the rvalue struct in a temp).
- Release IL: PASS (1266 checked, 0 failures).

## Batch 4 — ControlFactory.CreateCollapsibleAreaControl; RegionCollectionWithProperty text-format handlers

- `ControlFactory.CreateCollapsibleAreaControl`: `new CollapsibleAreaControl(gridCellSize) { RegionFactory = RegionFactory.Instance }`, now possible with the CollapsibleArea reference.
- `RegionCollectionWithProperty`: the static `HandleTextFormatChanged(DependencyObject, DependencyPropertyChangedEventArgs)` and the instance `HandleTextFormatChanged(TextFormat, TextFormat)`. Both have no closures.
- Release IL: PASS (1269 checked, 0 failures).
- Left for later: the static constructor and `DependencyProperty` fields of RegionCollectionWithProperty (they reference `TextFormat.TextFormatProperty.AddOwner` and the `PlotCommonProperties`/`AttachedProperties` owners), the constructor, and `OnTextFormattingChanged` (`using (new CommandScope)` with pattern loops, not attempted).

## Batch 5 — RegionCollectionWithProperty initialization, constructor, OnTextFormattingChanged

- Static `DependencyProperty` fields and `_typeOfThis` declared in `Mirrored/` (stubs removed from `.g.cs`), with the static constructor (`AddOwner` on `TextFormat`, `AttachedProperties` and `PlotCommonProperties` owners).
- Constructor: `_regions` from `DebugTools.AssertNotNull(...).Passed` ternary, `RangeChanged` hook, initial `HandleCollectionChanged`. The instance field `_currentProofingLanguage = XmlLanguage.Empty` initializer is required by Mathcad's constructor IL (first difference was `XmlLanguage.get_Empty`).
- `OnTextFormattingChanged`: `EnvironmentScope` guard, `using (new CommandScope(...))`, pattern `foreach` with `continue`. Exact as written.
- Release IL: PASS (1272 checked, 0 failures).
- Left: `GetTextFormat`, `GetCalcIsDisabled` and the other `Get*ForRegions` helpers, `SetTextFormatPropertyValueWithCommand`, `UpdateShowNumbers`/`UpdateShowTicks` and the `HandleCollectionChanged` body (not attempted in this batch).

## Batch 6 — RegionCollectionWithProperty GetTextFormat, GetPlotBackgroundType, GetPlotsCommonState

- Release IL: PASS (1275 checked, 0 failures).
- `GetTextFormat`: the decompiled `region as X` / `is X` shape matched; the pattern-variable form (`is IRichTextBase richText`) differed at the first type test.
- Parked, lambda-bearing helpers (reverted to stubs): `UpdateShowNumbers`, `UpdateShowTicks`, `UpdateShowAxisExpressions`, `GetNumberFormatsForRegions`, `GetProofingLanguageForRegions`, `GetMatrixFormatsForRegions`, `GetCalcIsDisabled`, `GetOleObjectAutoResizeState`. Shared blocker: the Quire lambda names are `<UpdateShowNumbers>b__97_0` and `<GetCalcIsDisabled>b__...` with method ordinals about 30 higher than Mathcad's (`<UpdateShowNumbers>b__67_0`, `<GetCalcIsDisabled>b__77_0`), so the Mathcad closure names (`<>c` members and `<>c__DisplayClass105_0`) have no counterpart. The Generated stubs for members Mathcad places earlier are compiled before the Mirrored partial. A placeholder layout in Mathcad order (AGENTS: unmarked placeholder stubs) is needed before these can land.

## Probe — UpdateShowNumbers closure ordinal (parked; no source change)

- Mathcad: `<UpdateShowNumbers>b__67_0` (members.tsv `+<>c` row 77; `UpdateShowNumbers` is method row 40, token 0x06000031).
- Quire, current main: `<UpdateShowNumbers>b__97_0` (the stub sits in `Generated/`, which compiles before `Mirrored/`).
- To reach 67, exactly 66 methods must precede it in compile order. That needs a compile-first hand-written file declaring the Mathcad members before it as placeholders, in Mathcad order, and removing their generated stubs. This is a broad reorder of the class, so the group is parked per the task rule.
- Still open, all lambda-bearing: UpdateShowNumbers/Ticks/AxisExpressions, GetNumberFormatsForRegions, GetProofingLanguageForRegions, GetMatrixFormatsForRegions, GetCalcIsDisabled, GetOleObjectAutoResizeState, GetPlotCameraType, SetTextFormatPropertyValueWithCommand, HandleCollectionChanged.

## Batch 7 — DirectRenderingAlignmentGridBorder static DependencyProperty initializer

- `TypeOfThis`, the ten static DependencyProperty fields and the static constructor moved to `Mirrored/` (stubs removed from `.g.cs`). No lambdas; callbacks are method groups. `GridCellSizeProperty` uses `AlignmentGrid.GridCellSizeProperty.AddOwner`.
- Release IL: PASS (1276 checked, 0 failures).
- Left as stubs: `BuildTileDrawingGeometry` (contains a `UpTo` lambda; closure-ordinal blocker as for RegionCollectionWithProperty), and the instance pen fields.

## Batch 8 — HeaderFooterZombie IsAssociatedWithPage and static PageNumber property

- `_typeOfThis` and `PageNumberProperty` moved to `Mirrored/` as field initializers (stubs removed). `IsAssociatedWithPage` implemented (no closures). Release IL: PASS (1278 checked, 0 failures).
- Left as XAML-generated stubs: `InitializeComponent`, `IComponentConnector.Connect`, `_contentLoaded`.
- Survey, closure-free types checked: `RegionFactory` (large serialization chain, not attempted), `RegionPersistentData` (Accept parked), `WhiteboardItemZombie` (constructor and `HandleZombieLoaded` use lambdas).

## Batch 9 — RegionFactory.CreateFailedRegionPersistentData, CreateTemplatedTextBoxTypedPersistentData

- Both members implemented in `Mirrored/` (stubs removed). The failed-region image URI keeps Mathcad's `Ptc.Controls.Core` resource path, because the IL compares the string constant (the mirrored `FlowDocumentFamilyConverter` does the same). The enum dispatch is a `switch` statement; the switch expression differed at the first local (`stloc.1` vs Mathcad `stloc.0`).
- Release IL: PASS (1280 checked, 0 failures).
- Left as stubs: `ConvertToIRegionType`, `ConvertFromIRegionType`, `CreatePlotTypedPersistentData` and the rest of `RegionFactory` (not attempted in this batch).

## Batch 10 — RegionFactory.CreatePlotTypedPersistentData

- Implemented in `Mirrored/` as the `is` chain (no closures; stub removed). Release IL: PASS (1281 checked, 0 failures).
- Left as stubs: `ConvertToIRegionType`, `ConvertFromIRegionType` (not attempted in this batch).

## Parked — RegionFactory.ConvertToIRegionType (code reverted; stub kept)

- Release IL first difference after the decompiled-shape edits: `first difference at #2 (Quire has 137 entries, Mathcad 139): Quire brfalse 126, Mathcad brfalse 128`. Type operations match after these edits: `castclass IPersistentDataProvider` before `GetRegionPersistentData`, `isinst IHasPreSerialize` twice, no `DependencyObject` castclass before `GetBookmarkTag` or `GetIsRegionBorderVisible`; `string.Format` with `((object)control).GetType()` for the exception.
- Edits that were needed: `conversionAnnotations != null ? conversionAnnotations.ToStrings() : null` (the `?.` form emitted `call` where Mathcad emits `callvirt`); `((IPersistentDataProvider)control).GetRegionPersistentData(...)` (pattern variable differed); `(regionPersistentData as IHasPreSerialize).PreSerialize()`.
- Unresolved: Mathcad's body is 2 instructions longer before the throw block; not located. Next step: diff the full sequence with `MethodAudit --diff` for `RegionFactory::ConvertToIRegionType`.
- `ConvertFromIRegionType`: not attempted (larger chain of type tests).

## Batch 11 — RegionFactory.ConvertFromIRegionType

- Implemented in `Mirrored/` (stub removed from `.g.cs`). The shape is the `if`/`else if` `is` chain in Mathcad's order, the `ProtectedItemType` unwrap, and the `ISupportCustomInitialization`, `ISolveBlockControl` and `PlotControlBase` post-checks. The `istextblock` ternary picks `Paragraph` or `TextRegion`.
- Build fix: `Quire.Controls.Include` (`IncludeWorksheetControl`) is reached through the generated `Mirror.props` closure, so only a `using` was needed. The added `System.Windows.Documents` using made `Paragraph` ambiguous, so it was removed and the plain name resolves to `Quire.Controls.Text.Paragraph`.
- Release IL: PASS (1282 checked, 1282 exact, 0 failures).
- Left as stubs: `ConvertToIRegionType` (parked, batch 10 note).

## Batch 12 — RegionPersistentData.AllDeserializationExceptions (getter)

- Implemented in `Mirrored/` (stub removed from `.g.cs`). The initial list is `DeserializationException != null ? new List { x } : new List()`. The `== null` form and an `if`/`else` both differed: the `== null` ternary places the non-null branch as fall-through, and the `if`/`else` stores to the local twice (one `stloc` too many).
- The `foreach` over `Children` (`IList`) with `AddRange` is the closure-free bottom-tested loop.
- Release IL: PASS (1283 checked, 0 failures).
- Still parked: `PageNumberControl.IsValidTemplateKey` (lambda, `<>c__DisplayClass17_0` ordinal blocker) and the closure-bearing members listed in batch 6 and batch 8 notes.

## Batch 13 — WorksheetPageManager closure-free delegates and page helpers

- Implemented in `Mirrored/` (stubs removed from `.g.cs`): `AppendNewPage`, `GetTrailingEmptyPageCount` (bottom-tested `for` with `break`), `GetItemPage`, `GetWorksheetItemParts`, `IsItemSplit`, `HideZombies`, `ShowZombies`, `UpdateVisiblePagesOnPageRemovedIfNeeded`, `RemoveItemFromPage` (one-item array), `CreatePage`.
- Release IL: PASS (1293 checked, 0 failures) on the first attempt.
- Still stubs in this type: `IsTrailingEmptyPage` and `GetPage(double)` (both closure-bearing per the IL scan, or pending), `RemoveItemsFromPage` (closure), and the rest of the list in the batch 13 survey.

## Batch 14 — WorksheetPageManager GoToPage, ScrollToPageIfNeeded, RemoveNonOwnedItemAndZombies, RemoveEmptyPagesAfterLocation(Point)

- Implemented in `Mirrored/` (stubs removed from `.g.cs`). `RemoveNonOwnedItemAndZombies` stays `private`, as the stub declares it.
- Release IL: PASS (1297 checked, 0 failures).
- Still stubs: `RemoveEmptyPagesAfterLocation(int)` (loop with `Enumerable.Last`, bottom-tested with an entry guard), `GetPage(double)` (DevAssert with `Array.Empty`), `IsTrailingEmptyPage` (closure).

## Batch 15 — WorksheetPageManager RemoveEmptyPagesAfterLocation(int), GetPage(double)

- Implemented in `Mirrored/` (stubs removed from `.g.cs`). `RemoveEmptyPagesAfterLocation(int)` is an entry guard followed by a bottom-tested `do`/`while (Pages.Count > 1)` with two early `break`s. `GetPage` uses the public `DebugTools.DevAssert(pageIndex >= 0).Failed` form.
- Release IL: PASS (1299 checked, 0 failures).
- Still stubs in this type: `IsTrailingEmptyPage` (closure), `RemoveItemsFromPage` (closure), `HandleItemMoved`, `SplitItemIfNeeded`, the `UpdateVisiblePages*` set, `Dispose`, `ActivateDescendant*`, `GetPagesContaining` and others from the batch 13 survey.

## Batch 16 — WorksheetPageManager RemoveItemFromAppropriatePage (both overloads), GetPageIndex (double and item), AddNonOwnedWhiteboardItem, RemoveNonOwnedWhiteboardItem, PageBodyActivatedHandler

- Implemented in `Mirrored/` (stubs removed from `.g.cs`). The crosshair branch in `RemoveItemFromAppropriatePage` reads `_worksheetPageLayout` directly, as the IL reads the field. `PageBodyActivatedHandler` invokes the event without a null check, as the IL does.
- Release IL: PASS (1306 checked, 0 failures).
- Still stubs in this type: `GetPageIndex(double, bool)` (uses `DoubleExtension.IsLessThan` with a NaN epsilon; not attempted), `IsTrailingEmptyPage` and `RemoveItemsFromPage` (closures), and the `UpdateVisiblePages*`, `Print*`, `Split*`, `Activate*` members.

## Batch 17 — WorksheetPageManager GetPageIndex(double, bool), UpdateVisiblePagesOnPageAddedIfNeeded

- Implemented in `Mirrored/` (stubs removed). `GetPageIndex(double, bool)` declares `gridHeight` from `PageLayout.GridCellSize.Height` directly, so Roslyn's temp holds the `Size`, as Mathcad's does. The `IsLessThan` call uses the extension's default NaN epsilon, which is the constant Mathcad passes.
- Release IL: PASS (1308 checked, 0 failures).
- Still stubs in this type: `IsTrailingEmptyPage`, `RemoveItemsFromPage`, `AppendNewPageIfNeeded`, `RemoveAllPages`, `Dispose` (closures), `UpdateVisiblePagesOnScrollChanged`, `PrePrint/PostPrintUpdateVisiblePages`, `GetPagesContaining`, the `Activate*` and `Split*` families.

## Batch 18 — WorksheetPageManager Activate and Split helpers

- Implemented in `Mirrored/` (stubs removed): `CheckIfSplitIsAllowedAndRequireFutureSplitIfNeeded` (static), `ActivateItemPart`, `ActivateMatchingPart`, `ReplaceWithRealItemAndActivateIfNeeded`, and the two-argument `ActivateDescendantParentIfNeeded`.
- Parked: the four-argument `ActivateDescendantParentIfNeeded` (stub kept). Mathcad evaluates `this` and `activeItem`, then stores the `IWhiteboardItemContainer` into a temp inside the argument list before `dup`/`brtrue` for the `?? topArrangeInfoProvider`. Roslyn stores the temp as a statement before `ldarg.0`, so the first difference is at #24 (`ldloc.1` vs `ldarg.0`). Only an in-argument assignment could match, and that still emits `dup` in a different order.
- Also parked: `GetCustomSplitItemRenderSize` (Mathcad shares one `First()` receiver across two member reads with `dup`, which Roslyn does not emit from source).
- Release IL: PASS (1313 checked, 0 failures).

## Batch 19 — WorksheetPageManager GetPagesContaining, UpdateVisiblePagesOnScrollChanged

- Implemented in `Mirrored/` (stubs removed). `GetPagesContaining` uses the `if (DevAssert(...).Failed)` split on `GetItemSplitCheckIsRequired`, the `IsNotEmpty(out IList<T>)` overload, and `new PageIndexRange(int)` for the default-null end. `UpdateVisiblePagesOnScrollChanged` keeps the pre-print range comparison and the `Math.Min`/`Math.Max` union.
- Release IL: PASS (1315 checked, 0 failures) on the first attempt.
- Still stubs: the four-argument `ActivateDescendantParentIfNeeded` (argument-list temp), `GetCustomSplitItemRenderSize` (shared `dup` receiver), closure members, and the `Split`/`Print` families not yet attempted.

## Batch 20 — WorksheetPageManager PrePrintUpdateVisiblePages, PostPrintUpdateVisiblePages

- Implemented in `Mirrored/` (stubs removed). `PostPrintUpdateVisiblePages` reads `_pageRangeOnPrePrint.Value` in the `AssertNotNull(...).Passed` branch and falls back to the full page range; it then clears the field with `= null`.
- Release IL: PASS (1317 checked, 0 failures) on the first attempt.
- Probe left for later: `ItemSplitIsNeeded` (out-parameter variant). Mathcad keeps `topLeft.Y` on the stack with `dup` for both `CompareLocation(top, 0)` and `top + height`, the same receiver-sharing shape as `GetCustomSplitItemRenderSize`; not yet measured.

## Batch 21 — WorksheetPageManager SetInsertionPointOnAppropriatePage, HandleItemSizeChanged

- Implemented in `Mirrored/` (stubs removed). `SetInsertionPointOnAppropriatePage` keeps a named `Point` copy and the `double` offset temp, which matches the IL's local order. `HandleItemSizeChanged` reads the location as an argument of `GetPage`: a separate `Point` statement put `ldarg.0` after `get_ItemManager` and failed at #23.
- Release IL: PASS (1319 checked, 0 failures).
- Measured probe, `ItemSplitIsNeeded` (out-parameter variant): natural form fails at #17 (call order) and #49 (locals, 71 vs 72 entries). Mathcad computes `topLeft.Y + height` with `dup` sharing `topLeft.Y` before the zero comparison. That is the receiver-sharing shape, so it stays parked (stub kept); the probe edit was reverted and not landed.

## Batch 22 — WorksheetPageManager GetClippedItemPartRect

- Implemented in `Mirrored/` (stub removed). The `itemPart`/`itemParts` shape matches the IL: the empty-parts branch only takes the real item for index 0, and the out-of-bounds check is an `AssertNotNull` with the two-argument message. `clipBounds = ItemManager.GetItemClipGeometry(itemPart)?.Bounds` matched the `dup`/`brtrue` null-conditional lowering. The stub's third argument (`worksheetItemManager`) is not read by the IL, so the body uses the static `ItemManager`, as the IL does.
- Release IL: PASS (1320 checked, 0 failures) on the first attempt.

## Batch 23 — WorksheetPageManager ActivateSplitItemPartContainingDescendant, ActivateMatchingPartIfNeeded

- Implemented in `Mirrored/` (stubs removed). Both match the IL with the natural guards: the `IsEmpty` branch with the whiteboard-control check, `parentControl != topItemToActivate` before the ancestor transform, and the `||` range test in `ActivateMatchingPartIfNeeded`.
- Release IL: PASS (1322 checked, 0 failures).
- Probe, parked: `GetSplitVerticalAdjustment` (foreach with try/finally). Mathcad's loop-exit is a single conditional branch straight to the `leave` that follows the try (`bgt.un` into `leave.s`). With `if (clipBottom > lim) break;` Roslyn emits a conditional skip plus an unconditional `leave`, one instruction longer (first difference at #11, 69 vs 68 entries; the `if (<=) { add }` form is semantically wrong and differs at the branch target). Stub kept; the source change was reverted and not landed.

## Batch 24 — WorksheetPageManager SplitItemIfNeeded

- Implemented in `Mirrored/` (stub removed). The `forceSplitEvenIfNoVisiblePages | isAnyPageVisible` uses the non-short-circuit `|` that the IL shows. The locals are declared in Mathcad order (`pageIndices`, `isAnyPageVisible`, `offsetDiff`) so the slot numbering matches. `ItemSplitIsNeeded` stays a stub; it is only a call target here.
- Release IL: PASS (1323 checked, 0 failures) on the first attempt.
- Parked: `ActivateWorksheetItemPartForcingSplitIfNeeded`. It calls `ExecuteOrScheduleSplitAttempt` (closure stub) and the four-argument `ActivateDescendantParentIfNeeded` (argument-list temp, parked), and its `DevAssert` message is a conditional expression (`intoViewItem == null ? null : intoViewItem.GetType().Name`) that was not measured.

## Batch 25 probe — IsTrailingEmptyPage (not landed)

- Measured the closure path with real members only: `page.WhiteboardItems` and `page.NonOwnedWhiteboardItems` `IsNullOrEmpty` guards, then `Pagebreaks.Any(pageBreak => pageBreak.NextPage == pageIndex)`. Release IL: FAIL (1326 checked, 3 failures).
- Only failing shape: the display class is `WorksheetPageManager+<>c__DisplayClass100_0` in the candidate but `<>c__DisplayClass52_0` in Mathcad, so the lambda is `<IsTrailingEmptyPage>b__0` in the candidate with a different class. The class number is the compile-order ordinal across the whole class, so it cannot be fixed without the broad reorder or placeholder-stub ordinal placement already parked.
- Reverted both files with `git checkout --`; no member landed in this batch. Closure members stay parked until the ordinal approach is resolved.

## Batch 26 — WorksheetPageManager type-scoped layout; IsTrailingEmptyPage

- Gap measured: the failing closure was `<>c__DisplayClass100_0` (candidate) vs `52_0` (Mathcad), a 48-position gap. The ordinal is the count of members before the method in compile order, with fields, properties and events added (rule from the EquationControl batch 27 checkpoint). The candidate had the 9 generated-stub methods that Mathcad token-orders after `IsTrailingEmptyPage` (rows 38–80) compile ahead of the partial, plus hand-written members appended after it. Measured at the Mathcad position (row 33): 33 rows + 8 fields + 9 properties + 2 events = 52.
- Layout: the hand-written partial now holds all members in Mathcad token order (`members.tsv` rows 0–80, event accessors excluded). Generated stubs at rows 38–75 moved in as unmarked placeholders (`=> throw global::Quire.Mirror.MirrorStub.NotImplemented();`). The ctor(splitter) (row 80) and the `SplitAction` nested delegate moved to the tail, because each compiled ahead of `IsTrailingEmptyPage` and added one position. `Generated/` keeps the 8 fields, 2 events and the stubs at rows 12–27 (all before row 33).
- `IsTrailingEmptyPage` (row 33): `if (!WhiteboardItems.IsNullOrEmpty()) return false; if (!NonOwnedWhiteboardItems.IsNullOrEmpty()) return false; if (Pagebreaks.Any(pb => pb.NextPage == pageIndex)) return false; return true;` The explicit `if` branch shape matches the Mathcad `brfalse` after `Any`; `return !Any(...)` did not.
- Release IL gate (project): PASS, 1326 checked, 1326 exact, 0 failures; the 273 "no Quire member pairs" entries and 1239 unfinished generated stubs are skipped as before. All earlier exact members (batches 3–24) stay exact after the reorder.
- Not verified here: `dotnet run -- emit` (MirrorGen regeneration) needs Release builds of twelve projects that this worktree does not have, so byte-identity of `Generated/` against regeneration was not checked. The hand-written placeholders are unmarked, so the generator omits their stubs by the same rule as the partial's other members.
- Remaining parked: the 4-argument `ActivateDescendantParentIfNeeded`, `GetSplitVerticalAdjustment`, `ItemSplitIsNeeded` (3-arg), `CalculateItemPageIndices` and the other closure members are placeholders in Mathcad order; they are now at their Mathcad ordinals, so their closure-name probes can be retried.

## Batch 27 — WorksheetPageManager AppendNewPageIfNeeded (closure ordinal)

- Implemented `AppendNewPageIfNeeded` (`requiredPageCount = pageIndex + 1; if (requiredPageCount > Pages.Count) (requiredPageCount - Pages.Count).Times(i => AppendNewPage());`). The lambda captures only `this`, so Roslyn names it `<AppendNewPageIfNeeded>b__N_0`.
- Ordinal measured: Mathcad `b__86_0`, candidate `b__85_0` with the delegate at the tail. Every ordinal-named lambda in Mathcad fits `token row + 19`, plus 1 for rows after the nested `SplitAction` delegate: Dispose (26 → 45), CalculateItemPageIndices (41 → 60), AppendNewPageIfNeeded (66 → 86), HandlePagebreakAdded (70 → 90), HandlePagesCollectionChanged (72 → 92), RemoveAllItemsFromPage (73 → 93), constructor (80 → 100).
- Moved the `SplitAction` nested delegate from the tail to just before `ActivateItemPart` (row 42, after row 41). Result: the closure name matches Mathcad. Release IL gate: PASS, 1328 checked, 1328 exact, 0 failures, including the previously exact `IsTrailingEmptyPage`.

## Batch 28 — WorksheetPageManager RemoveItemsFromPage, HandlePagebreakAdded, HandlePagebreakRemoved

- `RemoveItemsFromPage`: foreach with `++stamp` on the item timestamp, `RemoveNonOwnedItemAndZombies`, `page.RemoveItems(items)`, then `if (items.Any()) ItemManager.ExecuteOnUnload(items.First(), () => page.UpdateNonPrintableSpaceIndicator(RightEdgeOfPage))`. Its display class `95_0` matches Mathcad (row 75: 75+19+1).
- `HandlePagebreakAdded`: `if (NextPage <= 0) NextPage = GetPageIndex(ItemManager.GetItemWorksheetLocation(pagebreak).Y); if (NextPage >= Pages.Count) Pages.Count.UpTo(NextPage, i => AppendNewPage());`. Lambda ordinal `b__90_0` matches.
- `HandlePagebreakRemoved`: empty body, Mathcad IL is `ret`.
- Release IL gate: PASS, 1334 checked, 1334 exact, 0 failures.
- Parked: `RemoveAllItemsFromPage`. Mathcad keeps one list on the stack and passes it to both `OfType` and `Where` via `dup` (no local for it), a receiver-sharing shape that the Roslyn source cannot reproduce without an extra local.

## Batch 29 — WorksheetPageManager collection-changed handlers

- `HandlePagebreaksCollectionChanged`: switch on `e.Action` (Add → `NewItems.OfType<IWorksheetPageBreak>().Each(HandlePagebreakAdded)`, Remove → `OldItems…Each(HandlePagebreakRemoved)`, default → `DebugTools.FailAssertion("Unsupported action: {0}", e.Action)`).
- `HandlePagesCollectionChanged`: the same switch over `IWorksheetPage` (Add subscribes `page.PageBodyActivated += PageBodyActivatedHandler`, Remove unsubscribes), then `Pages.EachWithIndex((index, page) => page.PageNumber = index + 1)`. Lambda ordinals `b__92_1`/`b__92_2` (instance) and `b__92_0` (static in `<>c`) match Mathcad.
- Release IL gate: PASS, 1341 checked, 1341 exact, 0 failures.

## Batch 30 — WorksheetPageManager Dispose

- `Dispose` implemented in the partial at token row 26: `if (_disposeStatus == Disposed) return; _disposeStatus = Disposing; try { Pages.Each(page => page.Dispose()); _worksheetVisiblePages.Dispose(); Pages.RemoveAll(); _worksheetItemSplitter.Dispose(); } finally { _disposeStatus = Disposed; }`. Lambda `<Dispose>b__45_0` (static, in `<>c`) matches Mathcad.
- The generated stub for `HandleItemMoved` (row 27) compiled ahead of `Dispose` and made the ordinal 46. It moved into the partial as an unmarked placeholder after `Dispose`, which restores the Mathcad count.
- Release IL gate: PASS, 1343 checked, 1343 exact, 0 failures.

## Batch 31 — WorksheetPageManager RemoveAllPages

- `RemoveAllPages` (token row 25): `DebugTools.LogData("Worksheet.RemoveAllPages: Enter"); _worksheetVisiblePages.RemoveAllPages(); Pages.Each(RemoveAllItemsFromPage); Pages.RemoveAll();`. Closure-free (method group). The generated stub moved to the partial. It calls the parked `RemoveAllItemsFromPage` placeholder, so a non-empty page set still reaches that runtime stub.
- Release IL gate: PASS, 1344 checked, 1344 exact, 0 failures.
- Next candidates measured read-only: `SplitItemIfNeededAndRemoveEmptyPages` (display class `33_0` with a `SplitAction` conditional and an `ExecuteOnLayoutUpdated` lambda), `AddItemToAppropriatePage` (display class `31_0`, captured struct `itemWorksheetLocation`), `HandleItemMoved` (display class `46_0`, captured struct).

## Batch 32 — WorksheetPageManager VisiblePagesRangeToString

- `VisiblePagesRangeToString(int startingPageNumber = 1)`: `Regex.Replace(string.Join(",", VisiblePages.Select(page => page.PageNumber + startingPageNumber - 1)), ",([0-9]+,)+", "-")`. One captured parameter, so no captured-field ordering issue. Display class `82_0` matches Mathcad.
- The nested `SplitAction` delegate moved from after `ActivateItemPart` (row 41) to just before `AppendNewPageIfNeeded` (row 66). Its position now sits between rows 63 and 66, which is where Mathcad's display-class and lambda numbering place it. Batch 27's `AppendNewPageIfNeeded` lambda (`b__86_0`) still matches.
- Release IL gate: PASS, 1347 checked, 1347 exact, 0 failures.
- `SplitItemIfNeededAndRemoveEmptyPages` remains parked and reverted (display class 37 against 33, and captured-field store order stamp, item, location, this).

## Batch 33 — WorksheetPageManager HandleItemMoved

- `HandleItemMoved(item, oldLocation)`: page is `GetContainingPage(item) ?? GetPage(oldLocation.Y)`; captured `location` and `stamp` locals; `AppendNewPageIfNeeded`; the stamp update as `ItemManager.SetItemAddMoveRemoveTimestamp(item, ++stamp)` (captured-field increment as an argument matches the Mathcad temp-local shape, where the statement-level `long newStamp` form did not); same-page branch removes zombies, moves the item and registers `ExecuteOnLoad(item, () => SplitItemIfNeededAndRemoveEmptyPages(item, location, stamp))`; other branch removes from page and re-adds.
- Runtime dependency: the `ExecuteOnLoad` closure calls the parked `SplitItemIfNeededAndRemoveEmptyPages`, which is still a stub at runtime. The IL of `HandleItemMoved` itself does not depend on it.
- Release IL gate: PASS, 1350 checked, 1350 exact, 0 failures.

## Batches 39-43 — WorksheetViewModel closure-free members

- Batch 39: CopySelection, CreateFileNameControl, CreateFilePathControl, GetPageIndex, UpdateWorksheetGridSize (landed b8594bf70).
- Batch 40: CreateSolveBlockControl, CreateCollapsibleAreaControl, ActivateContent(Point) (landed f0fa28c2e). Needed `using Quire.Controls.Mathboard;`.
- Batch 41: ApplyFirstPageHeaderFooterToInactive, ApplyRegularHeaderFooterToInactive, UpdateHeaderFooterTops, GetCurrentTextFormat (landed 3c3684b72). `UpdateHeaderFooterTops` must assign `HeaderTop` as a separate statement; the chained `HeaderTop = ...` pushes the setter receiver first, which Mathcad does not.
- Batch 42: createFirstPageZombiesIfNeeded, UpdateActiveSectionForDifferentFirstPage (landed 9723265cd).
- Batch 43: UpdateNonprintableSpaceIndicator, RemoveGridRestrictedAreaForItem, OverlapsRightMargin, UpdateSizeLimit.
- Blocker, `SetStyleValues`: Mathcad emits `conv.r8` on `style.FontSize.Value` (the original source value is float); Quire's `ITextStyle.FontSize` is `double`, so the callsite differs. Fix needed in `src/Quire.Core/Generated/Mirror/ITextStyle.g.cs` (the member type must match Mathcad) before this method can be exact. Parked, stub kept.
- Parked Execute-shape members (new-object `WorksheetCommand.Execute` call vs callvirt): HandleDelete, HandleBackspace, DeleteActiveItem, HandleRemoveAnnotations, InsertPageBreak(Point), HandleAddToDictionary; and MoveItemsBy.

## Batches 48-69 — WorksheetViewModel wrap-up (haiku-worksheet-20261123)

- Owner: Claude worker (haiku-worksheet-20261123). Last landed: `25f8c4453` (batch 69), on `origin/main`.
- Landed in this stretch (each Release IL gate PASS): batch 48 `DisplayMessageId`; 51 `CanToggleIORegions<T>`; 61 `CreateIncludeControl(string)`; 62 `CanCreateInputControl` (static); 63 `SetStyleValues` (explicit `(double)` cast on `FontSize.Value`); 64 `CreateExcelComponentControl`, `CreateChartComponentControl` (try/catch `ApplicationException`); 65 `CommitActive` (static), `SetPagebreakVisualOffset`; 66 `InitializeXYZPlotControl`, `HandleBookmarkTag`, `HandleRemoveBookmarkTag`; 67 `HandleItemGotFocus` (`activeDescendant != item` branch order); 68 `CreateOleObjectControl` (the other three create members came from `mirror/worksheet-vm-plain-b2`); 69 `ActivateHeader(Point)`, `ActivateFooter(Point)`, `ScrollToActiveArea()`.
- Remaining stubs in `Generated/Mirror/Controls.Worksheet.WorksheetViewModel.g.cs`: 158 lines. By classifier: 50 with no IL body (properties and accessors, other), 72 closure-bearing, 11 execute-shape, 25 closure-free (listed below).
- Closure-free, still stubbed: `UNITTEST_CanCreate*` (4, wrappers over the closure-based `CanCreate*`), `UpdateFirstPageHeaderFooter`, `GetCalculatableFromVisibleWhiteBoardItem`, `CreateSpecTableControl` (x3 overloads), `FindOnce`, `CanTogglePageLayout`, `CanCreateInputControlOfType`, `CanCreateParagraphControl`, `CanPaste`, `HandleProtectedStateChanged`, `HandleCreateStyleFrom`, `MoveByPage`, `SelectItemsInBounds`, `HandleHeaderFooterNavigation`, `HandleHeaderFooterPageUp`, `HandleHeaderFooterPageDown`, `UpdateCalculationButtonStatus`, `ResizeItemToFit`, `RequestNavigateHandler`, `HandlePreviewMouseUp`, `HandleMouseUp`, `HandlePreviewMouseDown`.
- Parked, with reasons:
  - `CanPaste`: Mathcad allocates the `bool` return local (slot 0) before the `using` resource (slot 1). With `return` inside the `using`, Roslyn allocates the resource first. The `result`-declared-before-`using` form changes branch shape. Not resolved.
  - `InitializePlotControl`: signature mismatch. The Mathcad header is non-generic `IPlotControl`, while the body calls the generic `AddEquations` without a cast.
  - `HandleCaretLeavesTextArea`: needs the exact enum names for the switch.
  - Execute-shape (about 11-16 members, e.g. `HandleDelete`, `HandleBackspace`, `DeleteActiveItem`, `InsertPageBreak(Point)`, `HandleAddToDictionary`): `new WorksheetCommand` `.Execute` call versus `callvirt`. Not resolved.
  - `DeleteItemIfNeeded`: Mathcad inline-return guard shape.
  - `UNITTEST_CanCreate*`: depend on the closure-based private `CanCreate*`, so they wait for the compile-order layout.
- Blocked on compile-order layout (closure ordinals): the 72 closure-bearing members need a whole-type hand-written partial in Mathcad IL order before they can match. Handed to the Luna worker.
- Release IL gate for batch 69: PASS, 2193 checked, 0 failures. Batch 68 gate: PASS, 2061 checked, 0 failures.

## Luna WorksheetViewModel layout and closure batch

- Landed `b82102e94` (Mathcad IL-order hand-written layout) and `df8843934` (closure batch); `SetupCommandBindings` now emits its 151 handler lambdas at the calibrated ordinal. The lane merge Release IL gate passed 2,396 checked with zero failures, preserving all previously exact members.
- `Initialize`'s width callback was shaped with an explicit `this` receiver to match its lambda IL. `SetupDefaultPageLayout` remains a placeholder after the simple grid-cell callbacks were aligned; the nested size-change callback retains a compiler delegate-cache mismatch.
- Parked diff for `SetupDefaultPageLayout`: in `WorksheetViewModel+<>c__DisplayClass532_1::<SetupDefaultPageLayout>b__5`, Quire loads `<>9__6` (cached `Action<WhiteboardItemSizeChangedEventHandler>`) at #10 where Mathcad loads the `area` field; Quire has one conditional branch and Mathcad zero; first instruction difference is #22 (Quire 68 entries, Mathcad 58). An explicit `new Action<T>(delegate { ... })` still produced the cache; the handler was reverted to its unmarked placeholder.

## Luna WorksheetViewModel remaining members — batch 1

- Implemented 27 remaining utility, selection, navigation, input, and event handlers in the hand-written partial; final Release IL results follow after validation.

## Luna WorksheetViewModel remaining members — batch 2

- `CreateTextRegionFromEquationControl` is parked. Its Release IL differs at #1: Quire uses `castclass System.Windows.DependencyObject`, while Mathcad uses `isinst Ptc.Controls.EquationControl`; Quire has 10 conditional branches and 114 entries versus Mathcad's 9 branches and 109 entries, with the first instruction difference at #7 (`brtrue.s 3` versus `GetIsRegionBorderVisible`).
- `CanTogglePageLayout` is parked. Its Release IL differs at #2: Quire branches `brfalse.s 6` while Mathcad branches `brfalse.s 8`; Quire has 83 entries versus Mathcad's 85.
- Release IL gate: PASS, 2,426 checked, 2,426 exact, zero failures; 20 WorksheetViewModel members changed from stubs to exact, with all previously exact members preserved. `HandleProtectedStateChanged` retains one documented goto because Release IL branches collapsed areas to the method's shared exit; lint passes with the required label comment.
- Remaining WorksheetViewModel members are still stubs or shape-blocked; the two exact diffs above plus `SetupDefaultPageLayout`'s cached delegate mismatch are parked for later batches. The gate also reports 467 generated stubs not implemented, 105 Mathcad members without Quire counterparts, and 7 unchanged members not yet matching.

## Luna WorksheetViewModel remaining members — batch 3

- Implemented 26 WorksheetViewModel accessors and service properties (including `ItemManager`, `ControlCreator`, and `MouseFacadeService`) in the hand-written IL-order partial. Release IL gate: PASS, 2,459 checked, 2,459 exact, zero failures; all previously exact members remained exact. Lint passes.
- Parked members remain `CreateTextRegionFromEquationControl` and `CanTogglePageLayout` with the Release IL diffs above, plus `SetupDefaultPageLayout`'s nested cached-delegate mismatch recorded in the layout batch. The gate reports 434 generated stubs not implemented, 105 Mathcad members without Quire counterparts, and 7 unchanged members not yet matching.

## Luna WorksheetViewModel remaining members — batch 4 candidate

- Candidate work stays in `Mirrored/Controls.Worksheet.WorksheetViewModel.cs`; the previous Release IL run `.mirror-cache/release-il-f2d4a3588301/release-il.json` found 19 exact transitions and one remaining mismatch in `HandleEnter`. The body was reshaped from that report before landing validation. Lint and `git diff --check` pass.
- Parked/reverted after Release IL comparison: `CanDeleteVerticalSpace` (closure type `<>c__DisplayClass209_0` vs `<>c__DisplayClass205_0`, 104 entries both); `GetLastVisibleCalculatable` (`b__296_0` vs `b__294_0`, 84 entries both); `AddItemAtLocation` (lambda ordinals/cache slots differ, 121 vs 118 entries); `DirectionalMoveHandler(Direction)` (first difference #5: `ldarg.0` vs `ldloca.s 0`, 54 entries both); `FindOnce` (first difference #1: `get_ActiveItem` vs `get_IsFirstPageHeaderFooterDifferent`, 193 vs 210 entries); `CanTogglePageLayout` (first difference #2: `brfalse.s 6` vs `brfalse.s 8`, 81 vs 85 entries); `DeleteItemIfNeeded` (first difference #1: `brfalse.s 24` vs `brfalse.s 6`, 29 vs 31 entries); `UpdateNonprintableSpaceIndicatorWhenAreaHeightChanged` (first difference #15: `brtrue.s 14` vs `brtrue.s 13`, 44 vs 43 entries); `HandleCreateStyleFrom` (`InlineCollection.get_Count` callvirt vs call, 114 vs 118 entries); `MoveByPage` (first difference #2: `beq.s 4` vs `beq.s 5`, 184 vs 185 entries; an earlier shape also loaded `r8 0` vs `r8 NaN`); `CanSeparateOverlappedItems` (first difference #18: `brtrue.s 5` vs `brtrue.s 6`, 59 vs 58 entries); `SeparateOverlappedItems` (first difference #8: `brtrue.s 5` vs `brtrue.s 6`, 67 vs 66 entries); `HandleHeaderFooterNavigation` (first difference #6: `get_InsertionPoint` vs `get_ActiveItem`, 196 vs 199 entries); `UpdateCalculationButtonStatus` (first difference #22: `stloc.s 5` vs `stloc.s 4`, 101 entries both); `GetSearchableRegions` (lambda ordinal `b__618_0` vs `b__604_0`, cached method-group slot `<>O::<0>` vs `<>O::<3>`, 71 entries both).
