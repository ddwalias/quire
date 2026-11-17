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
