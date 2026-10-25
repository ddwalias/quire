# Lane N — Quire.Controls.Core

- Owner: Lane N; branch `mirror/n-controls-core-20261025-01`; worktree `C:/Projects/Quire-wt/lane-n-controls-core`.

## Batch 1 — Converters

- Scope:
  - `XmlBooleanConverter.ConvertTo`
  - `FindOptionToBooleanConverter` (`.cctor`, `Convert`, `ConvertBack`, `_allSet`, `_targetValue`)
  - `FindAndReplaceControlRemoveFormatConverter` (`_variableRegex`, `.ctor`, `ConvertBack`)
  - `WarningTypesToBooleanConverter` (`_targetValue`, `Convert`, `ConvertBack`)
  - `IdLabelToStringConverter` (`Convert(object, bool?)`, `Convert(object, Type, object, CultureInfo)`, `ConvertBack(string, bool?)`, `ConvertBack(object, Type, object, CultureInfo)`)
  - `DictionaryKeyToValueConverter` (`Instance`, `.cctor`, `Convert`)
  - `SystemIconConverter` (`Instance`, `.cctor`, `Convert`)
  - `XmlLanguageConverterWithDefaultValue` (`.ctor`, `ConvertTo`)
  - `MessageListToMessagesDecoratorConverter` (`Instance`, `.cctor`, `Convert`)
  - `GetCellValueAtColumnConverter` (`Convert`)
  - `UnitSystemsConverter` (`.ctor`, `ConvertTo`, `ConvertFrom`)
- Report: `.mirror-cache/release-il-ef561dffb403/release-il.json` — PASS, 852 checked, 852 exact, 0 failures, 3,000 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 2 — Leaf types, event args, and helpers

- Scope:
  - `FileDialogFilterPair` (`Extension`)
  - `DragEventArgs` (`.ctor`)
  - `ResizeEventMouseArgs` (`DragMode`, `SourceElement`, `StartPoint`, `OriginalEventSource`, `Automation`, `.ctor`)
  - `SubElementToResizeData` (`.ctor`)
  - `ResizableElementAttachedProperties` (`_typeOfThis`, `IsResizingProperty`, `.cctor`, `GetIsResizing`, `SetIsResizing`)
  - `BorderGripDescriptor` (`DockSideProperty`, `CanResizeProperty`, `PreserveDimensionsProperty`, `.cctor`, `DockSide`, `CanResize`, `PreserveDimensions`)
  - `WhiteboardItemSizeChangedEventArgs` (`PreviousSize`, `NewSize`, `.ctor(SizeChangedEventArgs)`, `.ctor(Size, Size)`)
  - `BackgroundTypeColors` (`Paper`, `Gray`, `.cctor`)
  - `Urls` (`DefaultMathcadPrimeWebServerHelpPathDir`, `DefaultEngNbWebServerHelpPathDir`, `DefaultUrlParam`, `.cctor`)
  - `FindReplaceStatusExtensions` (`IsEmpty(FindReplaceStatus?)`, `IsEmpty(FindReplaceStatus)`, `IsAtDocumentBoundary`)
  - `IdLabelsExtensions` (`EmptyLabelRl`, `.cctor`, `IsNullOrNone`, `GetRl`)
- Report: `.mirror-cache/release-il-b333b8e0eba9/release-il.json` — PASS, 879 checked, 879 exact, 0 failures, 2,978 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 3 — AttachedBehavior core, interfaces, and decorators

- Scope:
  - `DocumentDynamicResourceKeys` (`IsDocumentSectionClickableKey`)
  - `ImageControlConstants` (`ImageHelpContext`)
  - `FindRestrictorExtension` (`FindOnce`)
  - `Dialogs.FullWidthDecorator` (`MeasureOverride`)
  - `AttachedBehavior.IAttachedBehavior` (`Attach`, `Detach`)
  - `AttachedBehavior.IAttachedBehavior_1` (`OriginalValue`)
  - `AttachedBehavior.IActivator` (`IsActive`, `Activated`)
  - `AttachedBehavior.OneTimePropertyOwnerExtensionMethods` (`OneTimeSetExt`)
  - `AttachedBehavior.PropertyValueBase` (`_typeOfThis`, `PropertyValueProperty`, `.cctor`)
  - `AttachedBehavior.RoutedEventBinding` (`.ctor`)
  - `AttachedBehavior.RoutedEventHandlersBinder` (`Attach`, `Detach`)
  - `AttachedBehavior.PropertyBindersCollection` (`Attach`, `Detach`)
  - `AttachedBehavior.PropertyValueKeeper` (`_propertyPath`, `PropertyPath`, `SetBinding`)
- Report: `.mirror-cache/release-il-c6893ba03532/release-il.json` — PASS, 899 checked, 891 exact, 0 failures, 2,959 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 4 — Border draggers, shapes, grips, and parameters

- Scope:
  - `BorderDragger` (`_typeOfThis`, `.cctor`, `.ctor`)
  - `DefaultPageElementXamlHelper` (`GetPrintView`, `.ctor`)
  - `SetPropertyValueCommandParameter` (`TargetObjectProperty`, `PropertyNameProperty`, `DependencyPropValProperty`, `TargetObject`, `PropertyName`, `DependencyPropVal`, `UserData`, `.ctor(IPropertyAggregate, string, object, string)`, `.ctor()`, `CoercePropertyName`, `CreateInstanceCore`, `.cctor`)
  - `BorderShape` (`_rect`, `RightEdgeVisibilityProperty`, `CornerRadiusProperty`, `DefiningGeometry`, `RightEdgeVisibility`, `CornerRadius`, `ArrangeOverride`, `MeasureOverride`, `GetPen`, `OnRender`, `GetBorderGeometry`, `.cctor`, `.ctor`)
  - `ResizeGrip` (`DockSideProperty`, `DockSide`, `DefiningGeometry`, `DockSideChanged(DependencyObject, DependencyPropertyChangedEventArgs)`, `DockSideChanged(DependencyPropertyChangedEventArgs)`, `MeasureOverride`, `OnRender`, `GetBoundingRect`, `GetLocation`, `.cctor`, `.ctor`)
  - `CornerResizeGrip` (`DefiningGeometry`, `DockSideChanged`, `GetBoundingRect`, `GetLocation`, `.ctor`)
- Report: `.mirror-cache/release-il-fbca75c3daf4/release-il.json` — PASS, 924 checked, 924 exact, 0 failures, 2,930 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 5 — AttachedBehavior property value subsystem, IO region extensions, and Excel book

- Scope:
  - `IORegions.IORegionExtensions` (`IsSuitableForTagging`)
  - `IORegions.InputRegionExtensions` (`IsSuitableForInputTagging`)
  - `IORegions.OutputRegionExtensions` (`IsSuitableForOutputTagging`)
  - `Excel.ExcelBook` (`Sheets`)
  - `AttachedBehavior.PropertyValueCollection` (`Attach`, `Detach`, `OnLoaded(object, RoutedEventArgs)`, `OnUnloaded(object, RoutedEventArgs)`, `OnLoaded(FrameworkElement)`, `OnUnloaded(FrameworkElement)`, `AddValuesToResources`, `RemoveValuesFromResources`, `ForEachDictionary`, `.ctor`)
  - `AttachedBehavior.PropertyValueFinder` (`_propertyName`, `_propertyProviderKey`, `_propertyOwnerType`, `PropertyName`, `PropertyProviderKey`, `PropertyOwnerType`, `InitializeValue`, `.ctor`)
  - `AttachedBehavior.ParentPropertyValueKeepers` (`OnLoaded`, `OnUnloaded`, `.ctor`)
  - `AttachedBehavior.ChildPropertyValueFinders` (`OnLoaded`, `InitializeValues`, `.ctor`)
- Report: `.mirror-cache/release-il-75c20ea7cb36/release-il.json` — PASS, 951 checked, 951 exact, 0 failures, 2,909 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 6 — Protection commands, settings XML elements, and leaf helpers

- Scope:
  - `Protection.ProtectWarningsDialogsKeys` (`ActiveOleWarningMessageKey`)
  - `Protection.ProtectionDialogsCommon` (`MaxPasswordLength`)
  - `Protection.RoutedProtectionCommands` (`ToggleProtectState`, `.cctor`)
  - `Protection.UnProtectCommand` (`Name`, `Do`, `Undo`, `Redo`, `.ctor`)
  - `NumberFormatConstants` (`Increase`, `Decrease`)
  - `Settings.BuiltInVariables` (`.ctor(object, string)`)
  - `Settings.XmlSerializableEndElement` (`GetSchema`, `ReadXml`, `WriteXml`, `.ctor`)
  - `Settings.XmlSerializableStartElement` (`prefix`, `localName`, `ns`, `.ctor(string, string, string)`, `.ctor(string)`, `GetSchema`, `ReadXml`, `WriteXml`)
  - `MathFormatValues` (`SetPropertiesFrom`)
  - `NestedMatrixFormats` (`ItemMap`)
  - `NonClippingTextBlock` (`GetLayoutClip`)
  - `ToolTipContentExtractor` (`Convert`, `ConvertBack`, `.ctor`)
- Report: `.mirror-cache/release-il-62ad6c00227b/release-il.json` — PASS, 962 checked, 962 exact, 0 failures, 2,899 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 7 — Warnings display, whiteboard extensions, and serialization helpers

- Scope:
  - `WarningsTypesToShowExtension` (`ShouldDisplayAllWarnings`, `IsAnyWarningActive`)
  - `Whiteboard.WhiteboardControlExtensions` (`GetInsertionPointRect`)
  - `Serialization.MC14ConversionLossException` (`Messages`, `.ctor(string, Exception)`, `.ctor(string[])`)
  - `Serialization.XmlToXElementSerializableAdapter` (`ReadXml(XElement)`, `WriteXmlToXContainer(XContainer)`)
  - `PropertiesChangeInfo<TSource>` (`.ctor`, `IsValid`)
  - `Settings.UnitSerializer` (`localName`, `.ctor`)
  - `Serialization.DeserializationStrategyWithMathFormat` (`DeserializeRegionProlog`)
  - `Serialization.MetadataModel` (`_version = 1`, `.ctor()`)
- Report: `.mirror-cache/release-il-e6041691254a/release-il.json` — PASS, 974 checked, 974 exact, 0 failures, 2,887 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 8 — Adorners, ToolTips, and Resizers

- Scope:
  - `AnnotatedAdornerDefinitions` (`MarginLine`, `ArrowBase`, `ArrowHeadTop`, `ArrowHeadBottom`, `AdornerBrush`, `AdornerPen`, `.cctor`)
  - `VisibilityAwareAdorner` (`.ctor`)
  - `AnnotationArrowAdorner` (`.ctor`, `OnRender`, `DrawAnnotationArrow`)
  - `ToolTipImage` (`.ctor`, `SnappingHelper`)
  - `VisualAdorner` (`.ctor`, `Hookup`, `AddVisualChild`, `Remove`)
  - `IWhiteboardItemExtension` (`GetRegionsOfType(IEnumerable)`, `GetRegionsOfType(IWhiteboardItem)`, `GetCanSelect`, `IsSupportingRegionBorder`)
  - `MarginLineAdorner` (`_typeOfThis`, `IsDisplayed`, `.cctor`, `OnVisibilityChanged`, `Create`, `.ctor`, `AddToAdornerLayer`, `GetDesiredTransform`, `OnRender`)
  - `SnappableItemHelper` (`_onLayoutUpdated`, `Create`, `.ctor`, `Dispose`, `RecalculateOffset`)
  - `ItemResizerBase` (`ElementToResize`, `OriginalWidth`, `OriginalHeight`, `WidthIsAuto`, `HeightIsAuto`, `_preserveAspectRatio`, `.ctor`, `ResizeElementBasedOnOffset`)
- Report: `.mirror-cache/release-il-3f333569131a/release-il.json` — PASS, 1,011 checked, 1,011 exact, 0 failures, 2,864 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 9 — DocumentAttachedProperties

- Scope:
  - `DocumentAttachedProperties` (`ZoomProperty`, `LocationProperty`, `SnapToGridProperty`, `DefaultUnitSystemProperty`, `CustomBaseUnitSystemProperty`, `PreRecalculateAllProperty`, `defaultVerticalPageGutter`, `VerticalPageGutterProperty`, `DocumentIsActiveProperty`, `CacheWorksheetProperty`, `ShowInputOutputDesignationProperty`, `CoerceZoom`, `SetZoom`, `GetZoom`, `GetLocation`, `SetLocation`, `GetSnapToGrid`, `SetSnapToGrid`, `GetDocCalculationOptions`, `GetDocCalculationVariables`, `GetPreRecalculateAll`, `SetPreRecalculateAll`, `GetVerticalPageGutter`, `SetVerticalPageGutter`, `GetDocumentIsActive`, `SetDocumentIsActive`, `GetCacheWorksheet`, `SetCacheWorksheet`, `GetShowInputOutputDesignation`, `SetShowInputOutputDesignation`, `GetInputOutputRegionManager`, `GetBookmarksManager`, `GetLabel`, `CoerceFilePath`, `GetFilePath`, `GetIsInPrinting`, `SetIsInPrinting`, `GetIsInCopyNonSerializable`, `SetIsInCopy`, `GetIsInPasting`, `SetIsInPasting`, `GetCopyAsImageSettings`, `SetCopyAsImageSettings`, `.cctor`)
- Report: `.mirror-cache/release-il-93ff90339b20/release-il.json` — PASS, 1,044 checked, 1,044 exact, 0 failures, 2,832 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 10 — RegionCollection, Settings, Ole, and Helpers

- Scope:
  - `RegionCollection` (`GetObjectData`, `.ctor(SerializationInfo, StreamingContext)`)
  - `ColorPickerHelper` (`FindClosestMatchingColorInPalette`, `ColorDifference`)
  - `DocumentReadonlyOptions` (`RequestToUpdateInputsEnabled`, `FileLocationHistoryEnabled`, `GenerateInputsOutputsStatesOnSave`, `CaseInsensitiveAliasComparisonEnabled`, `Default`, `.ctor(bool, bool, bool, bool)`)
  - `OleConstants` (`OleEventPriority`, `DefaultProgram`, `ChartContainerModel`)
  - `OleSizeConstants` (`MinLongestEdge`, `MinShortestEdge`, `IgnoreDiffSizePercentageRatio`)
  - `UnprotectDialogKeys` (`UnprotectDialogTitle`, `UnprotectButtonTitle`, `UnprotectQuestion`)
  - `ProductInfo.Constants` (`ConverterProductName`, `ConverterExe`, `.ctor`)
  - `CalculationBehavior` (`LocalName`, `BoolConverter`, `.ctor(object, string)`, `.cctor`)
  - `SerializableAsElementContents` (`Serializable`, `.ctor(XElementSerializable)`, `ReadXml(XElement)`, `WriteXml(XElement)`)
  - `OleContainerViewModelBase` (`_typeOfThis`, `ToggleActiveState`, `.ctor`, `.cctor`)
  - `StaticMathFormat` (`FontFamilyName`, `FontSize`, `IsBold`, `IsItalic`, `IsUnderline`, `FontColor`, `HighlightColor`, `.ctor()`, `.ctor(IMathFormat)`, `.ctor(...)`)
- Report: `.mirror-cache/release-il-6101506a230f/release-il.json` — PASS, 1,059 checked, 1,059 exact, 0 failures, 2,819 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 11 — BookmarkTags (Tag & TagViewModel)

- Scope:
  - `BookmarkTag` (`BookmarkTagProperty`, `SetBookmarkTag`, `ClearBookmarkTag`, `GetBookmarkTag`, `HasBookmarkTag(DependencyObject, string)`, `HasBookmarkTag(DependencyObject)`, `.cctor`)
  - `BookmarkTagViewModel` (`_tag`, `Tag`, `.ctor(string)`)
- Report: `.mirror-cache/release-il-222f5f71d7d6/release-il.json` — PASS, 1,066 checked, 1,066 exact, 0 failures, 2,813 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 12 — LayoutMetrics, PrimeContextMenu, MenuItem, and Separator

- Scope:
  - `LayoutMetrics` (`OverallSize`, `Baseline`, `ItegralTextWidth`, `.ctor(Size, double)`)
  - `PrimeContextMenu` (`PrimeContextMenuStyleProperty`, `PrimeContextMenuStyle`, `HandlePrimeContextMenuStyleChanged`, `.ctor`, `.cctor`)
  - `PrimeContextMenuItem` (`PrimeContextMenuItemStyleProperty`, `PrimeContextMenuItemStyle`, `HandlePrimeContextMenuItemStyleChanged`, `.ctor`, `.cctor`)
  - `PrimeContextMenuSeparator` (`PrimeContextMenuSeparatorStyleProperty`, `PrimeContextMenuSeparatorStyle`, `HandlePrimeContextMenuSeparatorStyleChanged`, `.ctor`, `.cctor`)
- Report: `.mirror-cache/release-il-458dc5421a42/release-il.json` — PASS, 1,079 checked, 1,079 exact, 0 failures, 2,803 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 13 — MatrixFormatCommonConstants, MaxMatrixWidth, OleContainerMessageKeysDict, MathFormatForPaste, CopyWidgetBase

- Scope:
  - `MatrixFormatCommonConstants` (`DefaultHeight`, `DefaultWidth`, `DefaultTopRow`, `DefaultTopCol`)
  - `MaxMatrixWidth` (`MaxIntegral`, `MaxFractional`, `MaxVisualNode`, `.ctor(double, double, double)`)
  - `OleContainerMessageKeysDict` (`UpdateMessageKeys`, `ActivateMessageKeys`, `ResizeMessageKeys`, `ValidateMessageKeysDict`, `.cctor`)
  - `MathFormatForPaste` (`_instance`, `Instance`, `DocumentMathFormat`, `DocumentLabeledIdFormat`, `Generate(IMathFormatDeserializationProvider)`, `Generate(IMathFormat, IDictionary)`, `SetDefault`, `Dispose`, `.ctor`)
  - `CopyWidgetBase` (`ParentWidth`, `IncludeWidth`, `ChildBorder`, `Dispose`, `.ctor`)
- Report: `.mirror-cache/release-il-b3aea6d8fdde/release-il.json` — PASS, 1,087 checked, 1,087 exact, 0 failures, 2,796 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 14 — ProtectDialogKeys, CustomScriptsSettings, RelayCommand

- Scope:
  - `ProtectDialogKeys` (`ProtectDialogTitle`, `ProtectButtonTitle`, `LockDisplayStateLabel`, `ShowAreaBorderTitle`)
  - `CustomScriptsSettings` (`FontRatio`, `SubscriptAlignment`, `SuperscriptAlignment`, `.cctor`)
  - `RelayCommand` (`_execute`, `_canExecute`, `CanExecuteChanged`, `CanExecute`, `Execute`, `.ctor(Action)`, `.ctor(Action, Predicate)`)
- Report: `.mirror-cache/release-il-7ffb0a347c8d/release-il.json` — PASS, 1,092 checked, 1,092 exact, 0 failures, 2,792 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 15 — UnitSystemSerializer

- Scope:
  - `UnitSystemSerializer` (`localName`, `_unitDescriptor`, `CreateChildren`, `.ctor(UnitDescriptor, string)`)
- Report: `.mirror-cache/release-il-38e3c73970d9/release-il.json` — PASS, 1,094 checked, 1,094 exact, 0 failures, 2,790 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 16 — WhiteboardItemContainerExtensions

- Scope:
  - `WhiteboardItemContainerExtensions` (`_itemManager`, `ItemManager`, `HasRegionsWithConverterAnnotations`, `RegionsWithConverterAnnotations`)
- Report: `.mirror-cache/release-il-e5725093ee4c/release-il.json` — PASS, 1,101 checked, 1,101 exact, 0 failures, 2,787 unfinished.
- Remaining mismatches: none in claimed batch.
