# Lane Quire.Controls.Worksheet (haiku takeover)

- Owner: Claude worker (task_a4b72e175505), takeover of the stopped Antigravity lane (`docs/mirror/work/lane-quire-controls-worksheet.md`, which is historical only).
- Branch: `mirror/worksheet-haiku-20261117`
- Worktree: `C:/Users/DuyDang/orca/workspaces/Quire/claude-worksheet-haiku`

## Batch 1 — CombinedInvalidConverter

- Members: `CombinedInvalidConverter.ConvertBack`, `.ctor` (implicit default constructor).
- Parked: `CombinedInvalidConverter.Convert`. Idiomatic `values[0] is bool a && a && values[1] is bool b && b` gives 3 conditional branches; Mathcad has 2 and materializes the `is bool` tests with `ldc.i4.1; br; ldc.i4.0` followed by non-short-circuit `and`. Stub left in place.
- Release IL: `python tools/validation/release_il.py --base main --project Quire.Controls.Worksheet`: PASS; 436 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated` after batch 1: 2203 (2152 after batch 3) (`[global::Quire.Mirror.MirrorStub]` markers).

## Batch 2 — WorksheetViewModel (delegating and unit-test members)

- Members: `VisiblePages`, `Pages`, `TrailingEmptyPageCount`, `IsAutoCalcChecked`, `IsCalculating`, `UNITTEST_Evaluate`, `UNITTEST_DirectionalMoveHandler`, `UNITTEST_CanMoveInDirection`, `UNITTEST_CanPaste`, `UNITTEST_Paste`, `HandleSelectAll`, `ToggleItemSelection`, `SelectContiguous`, `SelectItem`, `SelectItems`, `ClearSelection`, `PageDown`, `PageUp`.
- Release IL: PASS; 454 checked; 0 failures.

## Batch 3 — WorksheetViewModel (auto-properties and one-call members)

- Members: auto-properties `HasScriptableControls`, `CurrentErrorChain`, `Header`, `Footer`, `FirstPageHeader`, `FirstPageFooter`, `IsSavedWorksheet`, `WorksheetContent`, `DisplayNumberFormat`, `ResultMatrixFormat`, `CurrentDragMode`; methods `CollapseIfPresent`, `ShowIfPresent`, `ResetVisualBrushIfPresent`, `OnStyleCollectionChanged`, `IsValidInsertionPoint`, `CanCreateOleObjectControl`, `CanCreateChartComponent`, `CreatePageBreakControl`, `HandleToggleAutoCalculation`, `CanReCalculateAll`, `CanRemoveSpace`, `CanCopySelection`, `CanCutSelection`, `GetItemRect`, `ItemsOverlapped`, `IsPageWideSizablePredicate`, `ImportData`, `GetDefaultLabeledIdFormat`, `GetInputRegions`, `GetOutputRegions`, `HandleSetCurrentProofingLanguage`, `OnRequestRestoreInsertionPoint`.
- Release IL: PASS; 498 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 2152.

## Batch 4 — WorksheetViewModel (backed properties)

- Members: 30 properties backed by the existing field stubs, including `IsContentActive`, `IsHeaderActive`, `IsFooterActive`, `InsertionPoint`, `ActiveItem`, `ActiveDescendant`, `SelectedDescendants`, `PageCount`, `StartingPageNumber`, `TotalPagesOffset`, `WorksheetHeight`, `HeaderTop`, `FooterTop`, `WorksheetPageLayoutDisplayHeight`, `NonprintableSpaceButtonHeight`, the four `*TextFormat` properties, `WidthWithNonprintableSpace`, `HasSelection`, `IsInLimbo`, the two `HideFromCalculation*` properties, `WorksheetDragHandler`, `PreRecalculateAll`, `ContentMathFormat`, `EnableEditCurrentPageNumber`, `HeaderFooterZombiesReady`, `IsRegionBorderShown`.
- Setter shape: `if (field == value) return;` guard clauses. The `if (field != value) { ... }` form differs from Mathcad (`beq.s` against `bne.un.s`), so the guard form is used.
- Release IL: PASS; 552 checked; 0 failures.

## Parked (blocked or differing shape)

- `CombinedInvalidConverter.Convert`: idiomatic `is bool && bool` gives 3 conditional branches; Mathcad has 2 (`ldc.i4.1; br; ldc.i4.0; and` shape). Stub kept.
- RtfTree/RtfItem members (`RtfItemModel` ctor, `Rtf`, `AddItemToDocumentInternal`; `SaveAsRtfWorksheetItemsVisitor.VisitAll`; `MultiWorksheetItemCopyVisitor`; `MultiWorksheetItemRtfCopyVisitorBase`; `RtfItemWrapperHelper.PrepareRtf` users): `NRtfTree` is not referenced by Quire and its licence is pending D7. Blocked.
- `Accept` overrides on persistent-data types (`ContainerRegionPersistentData`, `ImageRegionPersistentData`, and the other `*PersistentData` types): Roslyn emits a stack `pop` before `throw` in `as ... ?? throw`, which Mathcad's legacy compiler omits. Not reshaped to match a compiler artifact; stubs kept.

- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 2122.

## Batch 5 — WorksheetViewModel (one-call members and flag helpers)

- Members (30): `FireStartingPageNumberChanged`, `UpdateTotalPagesOffsetOnPageChange`, `UpdateWorksheetHeight`, `SetCopyWhiteboardItemsDelegate`, `GetPageSnapshot`, `GetPagesContaining`, `GetWorksheetItemParts`, `IsCalculableHiddenFromCalculationContext`, `GetWorksheetSectionItemLocation`, `SetWorksheetSectionItemLocation`, `GetItemIgnoreForHorizontalMovementAndWidth`, `CanInsertSpace`, `GetItemWorksheetPageIndex`, `ToggleInputRegions`, `ToggleOutputRegions`, `CanDeleteSelectedItems`, `CanDeleteActiveOrSelectedItems`, `CanDeleteActiveItem`, `CanToggleHideFromCalculationContext`, `CanBookmarkTag`, `ErrorCanTraceBackward`, `ErrorCanTraceForward`, `UpdateHasSelectionProperty`, `UpdateIsInLimboProperty`, `CanAddToDictionary`, `AddWordToDictionary`, `RemoveFromDictionary` (guard clause, matching Mathcad `brtrue.s`), `FireDictionaryUpdated`, `HandleSelectedDescendantsChanged`, `FireChanged`.
- Release IL: PASS; 582 checked; 0 failures.
- Not taken, as they need nested command classes or closures that are not yet mirrored: `DeleteActiveItem`, `HandleRemoveAnnotations`, `OnZombieInitializationComplete` (lambda), `CreateFileNameControl`/`CreateFilePathControl`, `CanCreateExcelComponent`, `GoToLastError`.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 2092.

## Batch 6 — WorksheetVisiblePages (one type; nested WorksheetVisiblePagesRange)

- Members (32 stubs removed): `VisiblePages`, `FirstVisiblePageIndex`, `LastVisiblePageIndex`, `IsVisiblePagesLoaded`, the constructor and the `_pages`, `_visiblePagesRange`, `_visiblePages` fields, `IsPageIndexVisible`, `IsAnyPageIndexVisible`, `SetVisiblePages`, `UpdateVisiblePagesOnScrollChanged`, `UpdateVisiblePagesOnPageAddedIfNeeded`, `UpdateVisiblePagesOnPageRemovedIfNeeded`, `RemoveAllPages`, `Dispose`, `UpdateVisiblePages`, `AddToEnd`, `AddToBeginning`, `RemoveFromBeginning`, `RemoveFromEnd`, `PassFocusToAncestorIfNeeded`, `PassFocusToPagesAncestor`; nested `WorksheetVisiblePagesRange`: constants, `First`, `Last`, constructor, `Update`, `IsIndexInVisibleRange`, `IsAnyIndexOfRangeInVisibleRange`, `Reset`.
- Shape changes: `UpdateVisiblePagesOnPageAddedIfNeeded` uses the positive condition (`IsVisiblePagesLoaded && !IsPageIndexVisible(...)`), since the early-return form differs; `PassFocusTo*` drop the `(DependencyObject)(object)` casts, which Mathcad does not emit.
- Parked:
  - `PageWhiteboardItemsCollectionChanged` (static, `foreach` over `IList`): the first difference is one instruction longer in Quire than Mathcad, at the `newItems != null` test's branch target. Not diagnosed further; stub kept.
  - `UpdatePageItemsVisibility`: its body is built from lambdas (`Each(...)` over `ToList()`), which need compiler-generated closure classes matching Mathcad's ordinals. Stub kept.
- Release IL: PASS; 612 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 2060.

## Batch 7 — WhiteboardItemZombie (one type)

- Members (20 stubs removed): `ItemManager`, `Item`, `ItemSize`, `ItemBounds`, `ItemVerticalAlignmentOffset`, `VerticalAlignmentOffset`, `IsEmpty` (throws `NotImplementedException`, as in the original), `SnappingHelper`, `ItemToZombieTransform`, `SnapsToPixelItem`, `HandleZombieVisibilityChanged`, `ResetVisualBrush`, `Hide`, `Show`, `UpdateZombieRect`, `Activate`, `NavigateIntoItem`, `SimulateMouseClick`, `Dispose`, `GetHelpContext`.
- Shape change: `UpdateZombieRect` reads `ItemBounds` directly instead of through a local `Rect itemBounds`/`Rectangle` local; the named locals gave different local slots from Mathcad's (AdornerLayer first, then the Rect temporaries).
- Left as stubs (with the fields and the constructor, which are kept for the same reason): the constructor and `HandleZombieLoaded`, because both use lambdas (`Loaded += ...`, `ExecuteOnLayoutUpdated(..., () => ...)`) that need Mathcad's closure ordinals. The fields `_zombieRect`, `_zombieAdorner`, `_itemManager`, `_item`, `_itemElement`, `_snappableHelper`, `_isDisposed` stay as stub declarations.
- Release IL: PASS; 636 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 2040.

## Batch 8 — WorksheetPageViewModel (one type)

- Members (19 stubs removed): `PageBody`, `HeaderZombie`, `FooterZombie`, `FirstPageHeaderZombie`, `FirstPageFooterZombie`, `Header`, `Footer`, `PageNumber`, `HasPageBreak`, `ShowNonPrintableSpaceIndicator`, `NonPrintableSpaceButtonPosition`, `NonPrintableSpaceIndicatorHeight`, `OnRequestBringIntoView`, `GetNavigateOutLocation`, `OnSectionRequestScrollTo`, `OnDispose`, `AddItemAtLocation`, `RemoveItem`, `RemoveItems`.
- Shape choices, each checked by the gate: `HasPageBreak` and `ShowNonPrintableSpaceIndicator` keep the `if (field != value) { ... }` form (Mathcad uses `beq.s` here, unlike `IsContentActive` in batch 4); `OnRequestBringIntoView` uses `as` plus an early return (Mathcad: `isinst`, `brtrue.s`, `ret`); `AddItemAtLocation` uses a trailing early return instead of `else if` (Mathcad's second half is `op_Equality; brfalse.s; ret`).
- Left as stubs: the constructor, `UpdateNonPrintableSpaceIndicator` (lambdas), the fields `_pageNumber`, `_header`, `_footer`, `_hasPageBreak`, `_showNonPrintableSpaceIndicator`, `_nonPrintableSpaceButtonPosition`, `_nonPrintableSpaceIndicatorHeight` (stub declarations kept, as in earlier batches).
- Release IL: PASS; 667 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 2021.

## Batch 9 — WorksheetControlExtensions (one static class)

- Members (28 stubs removed): `GetViewModel`, `SetItemWorksheetLocation`, `SetItemWorksheetGridLocation`, `MoveItemGridLocation`, `MoveItemsByGridLocation`, both `GetWorksheetSectionItemLocation` and both `GetItemWorksheetGridLocation` overloads, `GetItemWorksheetPageIndex`, `SetLayoutMode`, `SetInsertionPoint`, `DiscreteSelectRegion`, `ContinuousSelectRegion`, `SelectRegion`, `ActivateRegion`, `GetInsertionPointInGridCoordinates`, `SetInsertionPointInGridCoordinates`, `ActivateHeader`, `ActivateFooter`, `ActivateContent`, `InsertPageBreak`, `GetWorksheetItemParts`, the four `WorksheetLocationToGridLocation`/`GridLocationToWorksheetLocation` overloads, and `GetLinesPerPage`.
- Parked: `SeparateOverlappedItems`. It needs `WhiteboardCommands` (`Quire.Controls.Whiteboard`), which `Quire.Controls.Worksheet.csproj` does not reference. Confirmed by a build: CS0103 `WhiteboardCommands` does not exist. Stub kept.
- Release IL: PASS; 695 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 1993 (includes the parked `SeparateOverlappedItems`).

## Batch 10 — ControlFactory (one type)

- Members (31 stubs removed): `IsConnected`, the `DataFormatRichText` constant, the constructor, and 27 factory methods (every `Create*` except `CreateCollapsibleAreaControl`), plus the private `InitializeTextRegion`.
- Shape choices, each checked by the gate: `textFormat?.Apply...` became `if (textFormat != null) { ... }` (Mathcad uses `callvirt` after an explicit null test; Roslyn's `?.` form emits `call`); `CreateIncludeControl` assigns `RegionFactory` through `IRegionFactoryOwner` (Mathcad calls the interface setter, and the object initializer binds to the class member instead).
- Parked: `CreateCollapsibleAreaControl`. A build confirmed CS0246 `CollapsibleAreaControl`: `Quire.Controls.CollapsibleArea` is not referenced by `Quire.Controls.Worksheet.csproj`. Same blocker as `CollapsibleAreaHeightConverter`. Stub kept.
- Release IL: PASS; 725 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 1962 (includes the parked `SeparateOverlappedItems` and `CreateCollapsibleAreaControl`).

## Batch 11 — WorksheetItemSplitter (one type)

- Members (12 stubs removed): `ItemManager`, `Pages`, `PageLayout` (private accessors), `IsItemSplit`, `AddItemToPage`, `RemoveItemFromContainingPage`, `GetMatchingPart`, `TranslateToRepresentMatchingPartLocation`, `AddZombieToPartsMap`, `AddSizeChangedHandlerToItemMap`, `RemoveSizeChangedHandlerFromItemMap`, `UnhookAndDispose`.
- Parked (stubs kept, each tried once or twice):
  - `GetZombiesForItem`: Mathcad returns through a local (`stloc.0` before `ret`) that neither the early-return form nor the `??` form reproduces.
  - `ReplaceZombieWithRealItem`: the `&&` form and the early-return form both differ from Mathcad's short `brtrue.s` shape at the first `AssertNotNull(...).Failed` test.
  - `GetCollectiveSplitOffsetDiff`: the first difference is a constant operand (`r8 0` against Mathcad's `r8 2`); not diagnosed further.
  - Members with lambdas (`GetItemParts`, `Split`, `RemoveZombies`, `GetItemZombieOnPage`, `HideZombies`, `ShowZombies`, `DisposeEventHandlers`, `Dispose`): need Mathcad's closure ordinals. Also the constructor and the field stubs.
- Release IL: PASS; 737 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 1950.

## Batch 12 — PageNumberControl (one type)

- Members (8 stubs removed): `ClearTextForXamlSerialization`, `TemplateFormat` (override), `TemplateKey`, `HandleTagChanged`, `MeasureOverride`, `SetRegionPersistentData`, `GetPageNumberFormat`, `GetTemplateKeyFromGivenFormat`.
- Shape choices, each checked by the gate: `HandleTagChanged` returns early when the value is null or the assertion fails (Mathcad's branch order differs from the `&&` form); `GetTemplateKeyFromGivenFormat` calls `AppendFormat` on a named `StringBuilder` (Mathcad uses `callvirt`; the `new` receiver form emits `call`).
- Dropped: the `[DependsOn]` and `[UsedImplicitly]` attributes on `ClearTextForXamlSerialization`. They come from JetBrains.Annotations, which Quire does not reference. Attributes are not part of the IL compare.
- Left as stubs (lambdas, closure ordinals): `GetRegionPersistentData`, `IsValidTemplateKey`; plus the static constructor and the field stub `_typeOfThis`.
- Release IL: PASS; 747 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 1942.

## Batch 13 — PrintTreeXpsDecorator (one type)

- Members (4 stubs removed): `ItemManager` (private static accessor), `CanEnrichPrintingTree`, `EnrichPrintingTree`, `GetItemRectForPrint`.
- Left as stubs (lambdas, closure ordinals): `EnrichPagePart` (`EachWithIndex` lambda), `RecursiveSetItemsName` (lambdas), `ProcessAddorners` (`SelectMany` lambda). The static field `_whiteboardItemMgr` stays a stub declaration.
- Release IL: PASS; 752 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 1938.

## Batch 14 — WorksheetDragResizer (one type)

- Members (13 stubs removed): `StartPointInElementCoordinates`, `HandleResizeEnd` and `HandleResizeInProgress` (switches on `DragMode`), `HookupMouseEvents`, `UnhookMouseEvents`, `MouseUp`, `ResizeEnd`, `ResizeBy`, `TransformToElement`, both `MouseMove` overloads, `OffsetTo`, `HandleMouseUp`.
- Left as stubs: the constructor (it calls the base constructor and initializes the `DragDone`/`DragProgress` event fields, with `delegate { }` initializers) and the readonly field stubs `_mode`, `_container`, `_startPoint`.
- Release IL: PASS; 765 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 1925.

## Batch 15 — WorksheetDragger (one type)

- Members (22 stubs removed): the `ViewModel` accessor, `MouseDown`, the four `Get/Set{DragSource,DropTarget}Handler` accessors, `DragSourceHandlerChanged` and `DropTargetHandlerChanged` (`is not` guard, Mathcad's `brtrue.s` shape), `FireOnNoDrag`, `DragSourceMouseLeftButtonUp`, `PrepareItemForDragging`, `GrowWorksheetIfAppropriate`, `TopWindowDragEnter`, `TopWindowDragOver`, `TopWindowDragLeave`, `DoDrag` (`is not` guard), `HookupDragSourceEvents`, `OnDragSourceOnPreviewMouseLeftButtonDown` (`as` without a local), `UnhookDragSourceEvents`, `HookupDropTargetEvents`, `UnhookDropTargetEvents`, `ConvertPageCoordinatesToWorksheetCoordinates`.
- Parked (stubs kept):
  - `TopWindowQueryContinueDrag`: first difference at the `ViewModel != null && ...` branch (`brfalse.s` target differs); not diagnosed further.
  - Members with lambdas (`DragSourceMouseLeftButtonDownAuxiliary`, `DragSourceMouseMove`, `PrepareItemZombiesForDragging`, `EndDrag`, `CleanupDrag`, `UpdatePositionsDuringDrag`, `DropTargetDrop`, `DropTargetDragEnter`, `DropTargetDragOver`, `DropTargetDragLeave`).
  - The static constructor, the `OnDragDropEnd`/`OnDragDropMove` event fields, the dependency property fields, and the private fields.
- Release IL: PASS; 788 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 1903.

## Batch 16 — XmlSpreadsheetPasteHandler (one type)

- Members (11 stubs removed): `ControlCreator`, `AddQuotationMarks`, both `FormatNumber` overloads, `CreateNumberFormatFromExcelFormat`, `FixExcelNumberFormat`, `GetExcelNumberFormat`, `IsNumberFormat`, `IsFloatFormat`, `IsPrecentNumberFormat`, `GetDecimalPlacesAndShowTralingZeros`.
- Shape choices, each checked by the gate: string interpolation became `string.Format` (Mathcad calls `String.Format`); `new X(...).Method()` became a named local receiver (Mathcad uses `callvirt`); the regex and the `SelectSingleNode` result each use a named local; `GetDecimalPlacesAndShowTralingZeros` uses a `switch` statement, since the switch expression differed.
- Parked: `ProcessXmlSpreadsheet`. The first difference is at instruction 84 of the candidate (the dictionary initializer or the `Vector` construction); not diagnosed further. Stub kept.
- Release IL: PASS; 799 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 1892 (includes the parked `ProcessXmlSpreadsheet`).

## Batch 17 — DirectRenderingAlignmentGridBorder (one type)

- Members (27 stubs removed, including the constructor): the ten dependency-property accessors, `MinorLinePen` and `MajorLinePen` (guard-clause setters, since the `if (!Equals) { ... }` form differs from Mathcad), `ActualMajorLinePen`, the `DrawingBrush` auto-property, the five change handlers, `CreateMinorPen`, `CreateMajorPen`, `InvalidateGrid`, `CreatePen`, `DirectDcDrawing`, `MeasureOverride`, `OnRender`.
- Receiver casts kept as in the original: `((DependencyObject)this).GetValue/SetValue` and `((Freezable)brush).Freeze()`, so the member references name the same base type as Mathcad.
- Left as stubs: `BuildTileDrawingGeometry` (a `UpTo` lambda), the static constructor, the static `DependencyProperty` fields and `TypeOfThis`.
- Release IL: PASS; 839 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 1865.

## Batch 18 — WorksheetItemManager (partial: attached properties and item location, clip and page members)

- Members (20 stubs removed): the attached-property accessors for `WorksheetItemSizeChangedHandler`, `SizeChangedEventArgsList`, `AddMoveRemoveTimestamp` and `SplitCheckIsRequired`; `ItemHasPendingSizeChangedEvents`; both `GetItemWorksheetLocation` and both `SetItemWorksheetLocation` overloads; `SetItemWorksheetLocationForObject`; `SetContainingPage`, `GetContainingPage`, `SetItemClipGeometry`; `GetItemRect` (override).
- Shape choices, each checked by the gate: the `if (!AssertNotNull(...).Failed) { ... }` setters use the guard form (Mathcad branches with `brfalse.s`); `SetContainingPage` uses an early return; `obj is DependencyObject ? obj : null` became `as`.
- Parked: `GetItemClipGeometry`. Mathcad's second branch sits one instruction later than Quire's at the `itemPresenter != null` test; not diagnosed further. Stub kept.
- Left as stubs: the static field declarations and the `_typeOfThis` static field, the private fields, and the members that use lambdas (`SortItemsByLocation`, the size-changed dispatch, and the others with closures). Also the static constructor, if present.
- Release IL: PASS; 859 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 1845 (includes the parked `GetItemClipGeometry`).

## Batch 19 — WorksheetVisualHelper (partial: selection flags, ancestor lookups, translation and bring-into-view)

- Members (21 stubs removed): `ShouldToggleSelect`, `ShouldContiguousSelect` (the `ModifierKeys` flags are read as `int`), the three explicit `IWorksheetVisualHelper` forwarders with their static helpers (`FindWhiteboardItemAncestor`, `FindWhiteboardItemAncestorIncludingSelf`, `FindTopWhiteboardItemAncestorIncludingSelf`), `FindContainerAncestor`, `FindWorksheetPageAncestor`, `FindUIElementAncestorOrSelf`, `FindWorksheetControlAncestor`, `FindOneOfPageRelatedAncestorsIncludingSelf`, `TranslatePoint`, `IsSignificantDistance`, `BringIntoView` (item and item with target rect), `SuppressBringIntoViewAndExecute`, `GetScrollViewerRect`, `GetEngineeringDocumentZoomValue`, `GetVisualLocation`.
- Shape choices: `child is DependencyObject ? child : null` became `child as DependencyObject` (Mathcad has no branch there).
- Parked: `TransformBounds`. The first difference is at instruction 24 (Quire has 30 entries, Mathcad fewer); not diagnosed further. Stub kept.
- Left as stubs: `FindPageSectionContainingPoint`, `ScrollIntoView`, `AdjustScrollToOffset`, `GetDefaultLabeledIdFormat`, `BringIntoView(section, target)`, the remaining interface members, `BringItemsBoundingBoxIntoView` (lambdas), and the static `ItemManager` property and the constructor. These were not attempted in this batch.
- Release IL: PASS; 880 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 1824 (includes the parked `TransformBounds`).
