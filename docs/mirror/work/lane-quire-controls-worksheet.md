# Lane Quire.Controls.Worksheet

- Owner: task_98d833b58e17
- Branch: feature/controls-worksheet-stubs-agy-20261116
- Worktree: C:/Users/DuyDang/orca/workspaces/Quire/lane-quire-controls-worksheet

## Batch 1 — Constants, ContourPlotRegionPersistentData, Plot3DRegionPersistentData, DraftModeGutterWidthConverter, FileNameConverter

- Members:
  - `Quire.ProductInfo.Constants`: `ConverterProductName`, `ConverterExe`, `.ctor`
  - `Quire.Controls.Worksheet.ContourPlotRegionPersistentData`: `.ctor`, `DeserializeDimensions`
  - `Quire.Controls.Worksheet.Plot3DRegionPersistentData`: `.ctor`, `DeserializeDimensions`
  - `Quire.Controls.Worksheet.DraftModeGutterWidthConverter`: `Convert`, `ConvertBack`, `.ctor`
  - `Quire.Controls.Worksheet.FileNameConverter`: `Instance`, `.ctor`, `Convert`, `ConvertBack`, `.cctor`
- Release IL: PASS; 193 checked; 0 failures.
- Landed commit: `ee29bbbbf2186c23d0355dbd9b77cddb382ef5a9`
- Remaining mismatches: none in this batch (3046 stubs remaining in Quire.Controls.Worksheet).

## Batch 2 — NonClippingStackPanel, WorksheetPageLayoutExtensions, SavedDateConverter, FileNameControl, FilePathControl

- Members:
  - `Quire.Controls.Worksheet.NonClippingStackPanel`: `.ctor`, `GetLayoutClip`
  - `Quire.Controls.Worksheet.WorksheetPageLayoutExtensions`: `SetGridSize`
  - `Quire.Controls.Worksheet.SavedDateConverter`: `Convert`, `ConvertBack`, `.ctor`
  - `Quire.Controls.Worksheet.FileNameControl`: `GetRegionPersistentData`, `.ctor`
  - `Quire.Controls.Worksheet.FilePathControl`: `GetRegionPersistentData`, `.ctor`
- Release IL: PASS; 199 checked; 0 failures.
- Landed commit: `35b689c41f75bf1c98fbd4b96a9b0547bf1c0236`
- Remaining mismatches: none in this batch (3040 stubs remaining in Quire.Controls.Worksheet).

## Batch 3 — NonPrintableSpaceIndicatorPositionConverter, HeaderFooterRoleExtensions, FirstPageHeaderFooterConditionConverter, WorksheetItemPresenter

- Members:
  - `Quire.Controls.Worksheet.NonPrintableSpaceIndicatorPositionConverter`: `Convert`, `ConvertBack`, `.ctor`
  - `Quire.Controls.Worksheet.HeaderFooterRoleExtensions`: `IsHeaderRole`, `IsFooterRole`
  - `Quire.Controls.Worksheet.FirstPageHeaderFooterConditionConverter`: `Convert`, `ConvertBack`, `.ctor`
  - `Quire.Controls.Worksheet.WorksheetItemPresenter`: `Clip` (get, set), `HandleItemLocationChanged`, `.ctor`
- Release IL: PASS; 208 checked; 0 failures.
- Landed commit: `340d0f97ce93ee490ee2f6b0c49c3185376b84ef`
- Remaining mismatches: none in this batch (3031 stubs remaining in Quire.Controls.Worksheet).

## Batch 4 — HeaderFooterItemsControl, WorksheetPageExtensions, WorksheetRegionsInfo, PageBodyViewModel

- Members:
  - `Quire.Controls.Worksheet.HeaderFooterItemsControl`: `.ctor`
  - `Quire.Controls.Worksheet.WorksheetPageExtensions`: `GetViewModel`, `GetPageBody`
  - `Quire.Controls.Worksheet.WorksheetRegionsInfo`: `_isScriptableControlExists`, `get_IsScriptableControlExists`, `set_IsScriptableControlExists`
  - `Quire.Controls.Worksheet.PageBodyViewModel`: `.ctor`, `GetChildItemFromLocation`
- Release IL: PASS; 214 checked; 0 failures.
- Landed commit: `43a42ae89cfd313d11b33215893a9d94944ca324`
- Parked members:
  - `Quire.Controls.Worksheet.CollapsibleAreaHeightConverter`: blocked by missing project reference to `Quire.Controls.CollapsibleArea` in `Quire.Controls.Worksheet.csproj`.
- Remaining mismatches: none in this batch (3025 stubs remaining in Quire.Controls.Worksheet).

## Batch 5 — ItemLocationWorksheetStrategyHelper, PrintGridSettings, MouseUtilities, SwitchToDraftModeButtonAdorner, ZombieAdorner, DragZombieAdorner, WorksheetItemPositionComparer, SavedDateControl, FilePathConverter, HeaderFooterSelectorConverter

- Members:
  - `Quire.Controls.Worksheet.ItemLocationWorksheetStrategyHelper`: `GetItemContentClipping`
  - `Quire.Controls.Worksheet.PrintGridSettings`: `MajorLinesThickness`, `MinorLinesThickness`, `MajorLinesColor`, `MinorLinesColor`, `FrameColor`
  - `Quire.Controls.Worksheet.MouseUtilities`: `Win32Point.X`, `Win32Point.Y`, `GetCursorPos`, `GetMousePosition`, `.ctor`
  - `Quire.Controls.Worksheet.SwitchToDraftModeButtonAdorner`: `_rect`, `.ctor`, `MeasureOverride`, `ArrangeOverride`
  - `Quire.Controls.Worksheet.ZombieAdorner`: `Visual`, `_bounds`, `.ctor`, `OnRender`
  - `Quire.Controls.Worksheet.DragZombieAdorner`: `_childBoundaries`, `_visual`, `.ctor`, `OnRender`
  - `Quire.Controls.Worksheet.WorksheetItemPositionComparer`: `ItemManager`, `Compare`, `.ctor`
  - `Quire.Controls.Worksheet.SavedDateControl`: `GetRegionPersistentData`, `SetRegionPersistentData`, `.ctor`
  - `Quire.Controls.Worksheet.FilePathConverter`: `Convert`, `ConvertBack`, `.ctor`
  - `Quire.Controls.Worksheet.HeaderFooterSelectorConverter`: `Convert`, `ConvertBack`, `.ctor`
- Release IL: PASS; 233 checked; 0 failures.
- Landed commit: `bd19c06e68b8587b04dd58ffce07b1006bd0fc1f`
- Parked members:
  - `Quire.Controls.Worksheet.CollapsibleAreaHeightConverter`: blocked by missing project reference to `Quire.Controls.CollapsibleArea` in `Quire.Controls.Worksheet.csproj`.
- Remaining mismatches: none in this batch (3007 stubs remaining in Quire.Controls.Worksheet).

## Batch 6 — RtfItemImage, RegionCollectionWithPropertyCreator, ItemLocationInWhiteboardStrategyOnePage, HeaderFooterSearchRestrictor, and Worksheet Interfaces (IHeaderFooter, IPageNumberControl, IWorksheetControlDebugger, IWorksheetPageSection, IWorksheetItemCopyVisitor, ITemplatedTextBox, IPageSnapshot, IWorksheetPageBody, IWorksheetSection, IWorksheetPasteHandler)

- Members:
  - `Quire.Controls.Worksheet.RtfItemImage`: `Image`, `.ctor`
  - `Quire.Controls.Worksheet.IHeaderFooter`: `PageNumber`
  - `Quire.Controls.Worksheet.IPageNumberControl`: `TemplateKey`
  - `Quire.Controls.Worksheet.IWorksheetControlDebugger`: `OnModeChanged`
  - `Quire.Controls.Worksheet.IWorksheetPageSection`: `IsAssociatedWithPage`
  - `Quire.Controls.Worksheet.IWorksheetItemCopyVisitor`: `GenericImageSettings`, `VisitAll`
  - `Quire.Controls.Worksheet.ITemplatedTextBox`: `TemplateFormat`, `IsUnregisterWithCommand`, `Text`
  - `Quire.Controls.Worksheet.Printing.IPageSnapshot`: `PageBody`, `Header`, `Footer`
  - `Quire.Controls.Worksheet.IWorksheetPageBody`: `WaterMark`, `Zombies`, `Clip`, `GetItemHeight`
  - `Quire.Controls.Worksheet.IWorksheetSection`: `HandleDirectionalMove`, `UpdateItemLocationsAfterGridSizeChanged`, `HandleDelete`, `MoveItemsBy`
  - `Quire.Controls.Worksheet.IWorksheetPasteHandler`: `WorksheetMessages`, `ClipboardContainsRecognizedFormat`, `ClipboardContainsSingleEmbeddableItemForText`, `ProcessClipboardContents`, `ProcessSingleEmbeddableItemForTextInClipboard`
  - `Quire.Controls.Core.RegionCollectionWithPropertyCreator`: `IsConnected`, `CreateRegionCollection`, `.ctor`
  - `Quire.Controls.Worksheet.ItemLocationInWhiteboardStrategyOnePage`: `GetPageIndex`, `GetItemClipping`, `.ctor`
  - `Quire.Controls.Worksheet.HeaderFooterSearchRestrictor`: `_searchableRegion`, `CanSearch`, `SearchableRegion`, `CreateFindRestrictor`, `.ctor`
- Release IL: PASS; 278 checked; 0 failures.
- Landed commit: `e5ccefc7a886aff48fffb96aad2f93a3b966bd59`
- Parked members:
  - `Quire.Controls.Worksheet.CollapsibleAreaHeightConverter`: blocked by missing project reference to `Quire.Controls.CollapsibleArea` in `Quire.Controls.Worksheet.csproj`.
- Remaining mismatches: none in this batch (2993 stubs remaining in Quire.Controls.Worksheet).

## Batch 7 — IHeaderFooterZombie, IHeaderFooterZombieViewModel, IItemLocationStrategy, PageNumberConverter, SizeChangeNotificationDebugData, WorksheetItemsControl, IHeaderFooterViewModel, IWhiteboardItemZombie, IHeaderFooterControl, IWorksheetItemSplitter, ErrorWarningMessageKeys, Printing.PageSnapshot, IWorksheetVisiblePages, IVisitorFuncOfRegionPersistentData, IWorksheetPageViewModel, IWorksheetPage

- Members:
  - `Quire.Controls.Worksheet.IHeaderFooterZombie`: `ResetVisualBrush`, `RestoreVisualBrush`, `Show`, `Collapse`, `SubscribeToZombieIsInitialized`, `UnsubscribeFromZombieIsInitialized`
  - `Quire.Controls.Worksheet.IHeaderFooterZombieViewModel`: `ZombieBackground`, `ZombieItems`, `SnapsToPixelItem`, `ItemToZombieTransform`, `zombieIsInitialized`, `ResetVisualBrush`, `RestoreVisualBrush`
  - `Quire.Controls.Worksheet.IItemLocationStrategy`: `GetPageBodySize`, `GetItemRectLocationInWhiteboard`, `GetPageIndex`, `PrePrintUpdateWorksheetLayout`, `PostPrintUpdateWorksheetLayout`, `GetItemClipping`
  - `Quire.Controls.Worksheet.PageNumberConverter`: `_fallbackValue`, `FallbackValue`, `Convert`, `ConvertBack`, `.ctor`
  - `Quire.Controls.Worksheet.SizeChangeNotificationDebugData`: `AddedItemsCount`, `RemovedItemsCount`, `DeadItemsCount`, `NotifyAttemptsCount`, `UnexpectedNotifyAttemptsCount`, `AbortedOperations`, `SkippedOperations`
  - `Quire.Controls.Worksheet.WorksheetItemsControl`: `GetContainerForItemOverride`, `IsItemItsOwnContainerOverride`, `PrepareContainerForItemOverride`, `ClearContainerForItemOverride`, `.ctor`
  - `Quire.Controls.Worksheet.IHeaderFooterViewModel`: `CommonItems`, `UniqueItems`, `Margin`, `PageNumberControlMoved`, `NavigateOutOfHeaderFooter`, `PageNumberControlActivated`, `UpdateItemLocationsAfterGridSizeChanged`, `WorksheetHandleDelete`
  - `Quire.Controls.Worksheet.IWhiteboardItemZombie`: `Item`, `ItemSize`, `ItemBounds`, `ItemVerticalAlignmentOffset`, `UpdateZombieRect`, `ResetVisualBrush`, `Hide`, `Show`
  - `Quire.Controls.Worksheet.IHeaderFooterControl`: `CommonContents`, `TextFormat`, `Margin`, `BodySize`, `IsHeader`, `IsFooter`, `Role`, `PageNumberControlActivated`, `NavigateOutOfHeaderFooter`, `PageNumberControlMoved`
  - `Quire.Controls.Worksheet.IWorksheetItemSplitter`: `GetItemParts`, `Split`, `IsItemSplit`, `RemoveZombies`, `ReplaceZombieWithRealItem`, `GetCollectiveSplitOffsetDiff`, `GetItemZombieOnPage`, `HideZombies`, `ShowZombies`, `GetMatchingPart`, `TranslateToRepresentMatchingPartLocation`
  - `Quire.Controls.Worksheet.ErrorWarningMessageKeys`: `ErrorWarningMessageResourceDictionaryName`, `CantPrint`, `CantPaste`, `ClipboardTypePrimeObject`, `ClipboardTypeSExpression`, `ClipboardTypeXMLSpreadsheet`, `ClipboardTypeText`, `ClipboardTypeImage`, `ClipboardTypeGeneral`, `NetworkDriveDisconnected`, `ContainsScriptableObjects`
  - `Quire.Controls.Worksheet.Printing.PageSnapshot`: `Header`, `Footer`, `Visual`, `.ctor()`, `.ctor(Size?)`, `Dispose`
  - `Quire.Controls.Worksheet.IWorksheetVisiblePages`: `VisiblePages`, `FirstVisiblePageIndex`, `LastVisiblePageIndex`, `IsVisiblePagesLoaded`, `IsPageIndexVisible`, `IsAnyPageIndexVisible`, `UpdateVisiblePagesOnScrollChanged`, `UpdateVisiblePagesOnPageAddedIfNeeded`, `UpdateVisiblePagesOnPageRemovedIfNeeded`, `RemoveAllPages`, `SetVisiblePages`
  - `Quire.Controls.Worksheet.IVisitorFuncOfRegionPersistentData`: 13 `Visit` overloads
  - `Quire.Controls.Worksheet.IWorksheetPageViewModel`: `PageBody`, `HeaderZombie`, `FooterZombie`, `FirstPageHeaderZombie`, `FirstPageFooterZombie`, `Header`, `Footer`, `PageNumber`, `HasPageBreak`, `ShowNonPrintableSpaceIndicator`, `NonPrintableSpaceButtonPosition`, `NonPrintableSpaceIndicatorHeight`, `GetNavigateOutLocation`, `AddItemAtLocation`, `RemoveItem`, `RemoveItems`, `UpdateNonPrintableSpaceIndicator`
  - `Quire.Controls.Worksheet.IWorksheetPage`: `WhiteboardItems`, `NonOwnedWhiteboardItems`, `InsertionPoint`, `ActiveItem`, `PageBody`, `Header`, `Footer`, `HeaderZombie`, `FooterZombie`, `FirstPageHeaderZombie`, `FirstPageFooterZombie`, `PageNumber`, `HasPageBreak`, `RequestScrollTo`, `PageBodyActivated`, `AddItemAtLocation`, `RemoveItem`, `RemoveItems`, `GetNavigateOutLocation`, `Activate`, `UpdateNonPrintableSpaceIndicator`
- Release IL: PASS; 414 checked; 0 failures.
- Landed commit: `b3ed44ba2d57057223e57caca4cbfb33e4f892a1`
- Parked members:
  - `Quire.Controls.Worksheet.CollapsibleAreaHeightConverter`: blocked by missing project reference to `Quire.Controls.CollapsibleArea` in `Quire.Controls.Worksheet.csproj`.
- Remaining mismatches: none in this batch (2841 stubs remaining in Quire.Controls.Worksheet).

## Batch 8 — HeaderAndFooterResourcesKeys, ItemLocationInWhiteboardStrategyBase, IControlFactory, IWorksheetContent, IWorksheetItemManager, IWorksheetModel, IWorksheetPageManager, IWorksheetVisualHelper, IWorksheetControl, IWorksheetViewModel

- Members:
  - Quire.Controls.Worksheet.HeaderAndFooterResourcesKeys: PageNo, Page_PageNo, p_PageNo, PageNo_of_PagesTotal, Page_PageNo_of_PagesTotal, _PageNo_, OpenBracketPageNoCloseBracket, PageNo_OpenBracketPagesTotalCloseBracket, HFPageNumberNone, mm_dd_yyyy, mm_dd_yy, dd_mm_yyyy, dd_mm_yy, yyyy_mm_dd, System_Short, System_Long, HFLastSavedDateNone
  - Quire.Controls.Worksheet.ItemLocationInWhiteboardStrategyBase: _pageBodySize, WhiteboardItemManager, WorksheetPageManager, GetPageBodySize, .ctor, GetItemRectLocationInWhiteboard, GetPageIndex, GetItemClipping, PrePrintUpdateWorksheetLayout, PostPrintUpdateWorksheetLayout
  - Quire.Controls.Worksheet.IControlFactory: 28 contract factory methods
  - Quire.Controls.Worksheet.IWorksheetContent: 23 contract members (properties, events, methods)
  - Quire.Controls.Worksheet.IWorksheetItemManager: 19 contract methods
  - Quire.Controls.Worksheet.IWorksheetModel: 33 contract members (properties, events, methods)
  - Quire.Controls.Worksheet.IWorksheetPageManager: 42 contract members (properties, events, methods)
  - Quire.Controls.Worksheet.IWorksheetVisualHelper: 34 contract members (properties, methods)
  - Quire.Controls.Worksheet.IWorksheetControl: 78 contract members (properties, events, methods)
  - Quire.Controls.Worksheet.IWorksheetViewModel: 162 contract members (properties, events, methods)
- Release IL: PASS; 773 checked; 0 failures.
- Landed commit: aec0241fc842e80effa67ed0b2631f1e2400513b
- Parked members:
  - Quire.Controls.Worksheet.CollapsibleAreaHeightConverter: blocked by missing project reference to Quire.Controls.CollapsibleArea in Quire.Controls.Worksheet.csproj.
- Remaining mismatches: none in this batch (2395 stubs remaining in Quire.Controls.Worksheet).

### Batch 9 (Current)
- Owner: task_f8ef750565be
- Branch: feature/controls-worksheet-stubs-agy-20261116
- Members implemented:
  - Quire.Controls.Worksheet.WorksheetItemsCanvas: 8 members (PageManagerProperty, WorksheetPageLayoutModeProperty, PageManager, WorksheetPageLayoutMode, ItemManager, ArrangeOverride, .ctor, .cctor)
  - Quire.Controls.Worksheet.TemplatedTextBoxPersistentData: .ctor
  - Quire.Controls.Worksheet.WorksheetPageBreakPersistentData: .ctor
  - Quire.Controls.Worksheet.ItemLocationInWorksheetStrategy: 11 members (fields _pageBodySize, _worksheetItemManager, _worksheetPageManager, .ctor, GetItemRectLocationInWhiteboard, GetItemClipping, GetItemClippingInternal, GetPageIndex, PrePrintUpdateWorksheetLayout, PostPrintUpdateWorksheetLayout, GetPageBodySize)
  - Quire.Controls.Worksheet.AreaItemLocationInWorksheetStrategy: 9 members (fields _worksheetItemManager, _areaRectLocation, .ctor, GetItemRectLocationInWhiteboard, GetPageIndex, GetItemClipping, CountItemParts, GetItemClippingInternal, CalculateVirtualClipping)
  - Quire.Controls.Worksheet.WorksheetPageBody: 13 members (WaterMarkProperty, LabeledIdFormatProperty, _canvas, WaterMark, Zombies, IWorksheetPageBody.Clip, PropertyChanged, static .cctor, HandleItemsCollectionChanged, SetupInputGestures, Dispose, HandleLabelIdChanged, SnapPointToPixel)
- Release IL: PASS; 304 checked; 0 failures.
- Landed commit: 6d9190fe0fe26e8a695ab2283e3b4b712bf5fa4c
- Parked members:
  - TemplatedTextBoxPersistentData.Accept, WorksheetPageBreakPersistentData.Accept: modern Roslyn emits stack-balancing pop prior to throw in null-coalescing expressions producing brtrue.s 5 instead of legacy Mathcad brtrue.s 4.
  - WorksheetPageBody..ctor: legacy csc assignment expression emitted stloc.0 ldloc.0 whereas modern Roslyn optimizes to dup stloc.0.
- Remaining mismatches: none in this batch (2352 stubs remaining in Quire.Controls.Worksheet).

### Batch 10 (Current)
- Owner: task_517ae8bb098d
- Branch: feature/controls-worksheet-stubs-agy-20261116
- Members implemented:
  - Quire.Controls.Worksheet.DragAdorner: 8 members (_child, _borderOffset, VisualChildrenCount, .ctor, GetVisualChild, MeasureOverride, ArrangeOverride, GetDesiredTransform)
  - Quire.Controls.Worksheet.NameBuilderHelper: 16 members (PosPrefix, SizePrefix, DoublePairDelimiter, DoubleEmpty, DoubleNegativeSign, RegionNameSeparatorChar, _regionNameSeparator, _numberFormatForRegionName, DoubleToString, PositionToString, RectToString, SizeToString, DoublePairToString, StripDimensionsFromName, Combine, .ctor, .cctor)
  - Quire.Controls.Worksheet.ChartComponentPersistentData: 3 members (_chartComponent, .ctor, DeserializeEpilog)
  - Quire.Controls.Worksheet.ExcelComponentPersistentData: 3 members (_excelComponent, .ctor, DeserializeEpilog)
  - Quire.Controls.Worksheet.SpecTableRegionPersistentData: 2 members (.ctor, Deserialize)
  - Quire.Controls.Worksheet.ContainerRegionPersistentData: 2 members (.ctor, Deserialize)
  - Quire.Controls.Worksheet.ChartContainerRegionPersistentData: 2 members (.ctor, Deserialize)
  - Quire.Controls.Worksheet.ExcelContainerRegionPersistentData: 2 members (.ctor, Deserialize)
  - Quire.Controls.Worksheet.OleObjectPersistentData: 2 members (.ctor, Deserialize)
  - Quire.Controls.Worksheet.ImageRegionPersistentData: 3 members (.ctor, Deserialize, DeserializeDimensions)
  - Quire.Controls.Worksheet.PlotRegionPersistentData: 3 members (.ctor, Deserialize, DeserializeEpilog)
  - Quire.Controls.Worksheet.RichTextBaseRegionPersistentData: 3 members (_text, .ctor, Deserialize)
  - Quire.Controls.Worksheet.MathRegionPersistentData: 4 members (_equationControl, .ctor, DeserializeEpilog, Deserialize)
- Release IL: PASS; 346 checked; 0 failures.
- Landed commit: 983b210662abb5dfde16a08f7371250507e46157
- Parked members:
  - Accept on persistent data types (SpecTableRegionPersistentData, ContainerRegionPersistentData, ChartContainerRegionPersistentData, ExcelContainerRegionPersistentData, OleObjectPersistentData, ImageRegionPersistentData, PlotRegionPersistentData, RichTextBaseRegionPersistentData, MathRegionPersistentData): modern Roslyn null-coalescing throw stack pop opcode divergence.
- Remaining mismatches: none in this batch (2299 stubs remaining in Quire.Controls.Worksheet).

### Batch 11 (Current)
- Owner: task_9fc594398ea6
- Branch: feature/controls-worksheet-stubs-agy-20261116
- Members implemented:
  - Quire.Controls.Worksheet.IncludeRegionPersistentData: 5 members (_include, .ctor, HandleGetPackagePartByRefId, Deserialize, DeserializeEpilog)
  - Quire.Controls.Worksheet.HeaderFooterOptionsViewModel: 14 members (_worksheetViewModel, _isUsingDefaultTotalPagesValue, IsFirstPageHeaderFooterDifferent, InvalidTotalPagesMessage, StartingPageNumber, TotalPagesNumber, .ctor, HandlePageNumberChanged, UpdateTotalPagesValidationMessage, hasInvalidTotalPageAsMessage, UpdateTotalPagesByStartingPage, UpdateWatermark, CalculateDefaultTotalPages, UpdateTotalPagesValue)
  - Quire.Controls.Worksheet.PasteDeS11NProvider: 15 members (Instance, _helper, _xmlDocument, TextStyleChangedHandler, PackagePart, Helper, NameSpaceManager, XmlContentDocument, UseOverrides, FilePath, SkipRegionsNotAffectingCalculation, UnpackExternalObject, UnpackFlowDocument, UnpackProtectedPart, .ctor)
  - Quire.Controls.Worksheet.InitializedRegionFactory: 15 members (Instance, 13 Visit methods, .ctor)
  - Quire.Controls.Worksheet.WorksheetPageBody: 1 member (.ctor(Size?, bool))
- Release IL: PASS; 395 checked; 0 failures.
- Landed commit: (in-progress)
- Parked members:
  - Accept on IncludeRegionPersistentData: modern Roslyn null-coalescing throw stack pop opcode divergence (brtrue.s 5 vs legacy Mathcad brtrue.s 4).
  - CombinedInvalidConverter: legacy csc boolean evaluation sequence (ldc.i4.1; br; ldc.i4.0; and) differs from modern Roslyn boolean pattern matching.
- Remaining mismatches: none in this batch (2249 stubs remaining in Quire.Controls.Worksheet).
