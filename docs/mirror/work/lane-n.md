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

## Batch 17 — SelectionDraggerRubberBand

- Scope:
  - `SelectionDraggerRubberBand` (`_bounds`, `_pen`, `Bounds`, `Remove`, `OnRender`, `.ctor(UIElement)`, `.ctor(UIElement, Brush)`)
- Report: `.mirror-cache/release-il-665b3128c99d/release-il.json` — PASS, 1,105 checked, 1,105 exact, 0 failures, 2,783 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 18 — IOTag

- Scope:
  - `IOTag` (`IOTagProperty`, `IOTagSet`, `IOTagUnSet`, `HandleIOTagPropertyChanged`, `SetIOTag`, `ClearIOTag`, `GetIOTag`, `IsIOTagged`, `IsRegionOfType`, `HasInputTag`, `HasOutputTag`, `.cctor`)
- Report: `.mirror-cache/release-il-32e24bf55e48/release-il.json` — PASS, 1,111 checked, 1,111 exact, 0 failures, 2,778 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 19 — Serialization elements (Integration, Result, SpiritMetadata, MathFormat)

- Scope:
  - `IntegrationElementsSerializer` (`ShouldValidateOnWrite`, `ShouldValidateOnRead`, `GetSchema`, `RepairXDocument`, `.ctor(IIORegionContainer, ISerializationHelper)`)
  - `ResultElementsSerializer` (`ResultsElemName`, `ShouldValidateOnWrite`, `ShouldValidateOnRead`, `GetSchema`, `RepairXDocument`, `.ctor`)
  - `SpiritMetadataSerializer` (`schemaNamespaceURI`, `CreateSchemas`, `FillInMetadataModel`, `FillInXmlMetadata`, `.ctor`)
  - `MathFormatSerializer` (`converter`, `_ignoreUnsetProperty`, `MathFormat`, `CreateChildren`, `.ctor(string, string, MathFormat, bool)`)
- Report: `.mirror-cache/release-il-97646de9b89b/release-il.json` — PASS, 1,123 checked, 1,123 exact, 0 failures, 2,770 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 20 — RegionImageSettings, ProtectCommand

- Scope:
  - `RegionImageSettings` (`ApplyZoom`, `ApplyTransparency`, `ShrinkToFrame`, `DefaultForRtfCopy`, `DefaultForImageCopy`, `.cctor`)
  - `ProtectCommand` (`_toggleDisplayState`, `_undoCommand`, `Name`, `Do`, `Undo`, `Redo`, `.ctor(IProtectableModel, ProtectionDialogState, Action, Action<string>)`)
- Report: `.mirror-cache/release-il-047348fc6230/release-il.json` — PASS, 1,127 checked, 1,127 exact, 0 failures, 2,767 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 21 — MetadataSerializer

- Scope:
  - `MetadataSerializer` (`Serialize`, `Deserialize`, `CreateSchemas`, `FillInMetadataModel`, `FillInXmlMetadata`, `.ctor`)
- Report: `.mirror-cache/release-il-52bcaf3b0a3d/release-il.json` — PASS, 1,129 checked, 1,129 exact, 0 failures, 2,765 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 22 — DependencyObjectSerializer, CalculationSettings

- Scope:
  - `DependencyObjectSerializer` (`property`, `dependencyObject`, `localName`, `GetSchema`, `ReadXml`, `WriteXml`, `.ctor(Dictionary, DependencyObject, string)`)
  - `CalculationSettings` (`_nameSpaceUri`, `LocalName`, `ShouldValidateOnWrite`, `ShouldValidateOnRead`, `GetSchema`, `OnSchemaValidation`, `RepairXDocument`, `.ctor(IEngineeringDocument, string)`, `.ctor(CalculationVariables, CalculationOptions, string)`)
- Report: `.mirror-cache/release-il-4f9d5d530a6e/release-il.json` — PASS, 1,134 checked, 1,134 exact, 0 failures, 2,760 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 23 — SharedPropertyNames, RowColumnHelper, SubscriptRun, SuperscriptRun

- Scope:
  - `SharedPropertyNames` (`CurrentMatrixFormat`, `CurrentNumberFormat`, `CurrentTextFormat`, `CurrentMathFormat`, `HideFromCalculationButtonEnabled`, `HideFromCalculationButtonChecked`)
  - `RowColumnHelper` (`_columnCount`, `ColumnCount`, `RowIndexFromCellIndex`, `ColIndexFromCellIndex`, `IndexFromRowColumn`, `GetRowIndices`, `GetColumnsIndices`, `GetMinMaxRowColIndices`, `.ctor(int)`)
  - `SubscriptRun` (`TypeOfThis`, `DefaultAlignment`, `FontVariantsForCopy`, `CoerceBaselineAlignmentCallback`, `InitBaseline`, `.ctor()`, `.ctor(string)`, `.ctor(string, TextPointer)`, `.cctor`)
  - `SuperscriptRun` (`TypeOfThis`, `DefaultAlignment`, `FontVariantsForCopy`, `CoerceBaselineAlignmentCallback`, `InitBaseline`, `.ctor()`, `.ctor(string)`, `.ctor(string, TextPointer)`, `.cctor`)
- Report: `.mirror-cache/release-il-ac3c4a8af580/release-il.json` — PASS, 1,150 checked, 1,150 exact, 0 failures, 2,746 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 24 — TemporaryStorageHelper

- Scope:
  - `TemporaryStorageHelper` (`StorageFileName`, `SaveIfFileNameNullOrEmpty(Func)`, `SaveBitmap`, `LoadBitmap`, `CleanStorage`, `SaveIfFileNameNullOrEmpty(Action)`, `.ctor`)
- Report: `.mirror-cache/release-il-22416b405483/release-il.json` — PASS, 1,157 checked, 1,157 exact, 0 failures, 2,741 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 25 — MatrixFormatPropertyNames, OleSizeDataForXamlSerialization

- Scope:
  - `MatrixFormatPropertyNames` (`Size`, `Offset`, `Origin`, `ShowIndices`, `CollapseNestedMatrices`, `NestedMatrixFormats`)
  - `OleSizeDataForXamlSerialization` (`AutoResize`, `OleViewportSize`, `HasFakeOleViewportSize`, `CopyFrom`, `.ctor()`, `.ctor(IOleSizeData)`)
- Report: `.mirror-cache/release-il-5b6f5183dbef/release-il.json` — PASS, 1,165 checked, 1,165 exact, 0 failures, 2,733 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 26 — PropertyChangedNotificationHandler

- Scope:
  - `PropertyChangedNotificationHandler` (`_propertyChangedNotificationHandlers`, `AddPropertyChangedNotificationHandler`, `AttachPropertyChangedHandler`, `DetachPropertyChangedHandler`, `PropertyChanged`, `UpdateProperty`, `UpdateAllProperties`, `.ctor`)
- Report: `.mirror-cache/release-il-8c8df934a2cb/release-il.json` — PASS, 1,171 checked, 1,171 exact, 0 failures, 2,727 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 27 — MathRendering

- Scope:
  - `MathRendering` (`LocalName`, `_document`, `_namespace`, `_resultsNS`, `CreateChildren`, `.ctor(IEngineeringDocument, XName, string)`)
- Report: `.mirror-cache/release-il-bc488513c97b/release-il.json` — PASS, 1,173 checked, 1,173 exact, 0 failures, 2,725 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 28 — ToggleRegionsDesignationCommand

- Scope:
  - `ToggleRegionsDesignationCommand` (`_commandsToUndo`, `_commands`, `_name`, `Name`, `Execute`, `Do`, `Undo`, `Redo`, `.ctor(List<UndoableCommand>, string)`)
- Report: `.mirror-cache/release-il-18c90a708eba/release-il.json` — PASS, 1,182 checked, 1,182 exact, 0 failures, 2,721 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 29 — IORegionCommands, ProtectionCommonBase

- Scope:
  - `IORegionCommands` (`_typeOfThis`, `ToggleInputRegion`, `ToggleOutputRegion`, `UpdateInputs`, `ToggleAnInputRegion`, `ToggleAnOutputRegion`, `.ctor`, `.cctor`)
  - `ProtectionCommonBase` (`_model`, `_protectionData`, `_showWarningDialog`, `Unprotect`, `Protect`, `.ctor(IProtectableModel, Action<string>)`)
- Report: `.mirror-cache/release-il-418b50720921/release-il.json` — PASS, 1,186 checked, 1,186 exact, 0 failures, 2,718 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 30 — MessageDialogsKeys, ControlSettings

- Scope:
  - `MessageDialogsKeys` (`SaveButtonKey`, `CancelButtonKey`, `YesButtonKey`, `NoButtonKey`, `DoNotSaveButtonKey`, `SaveWithoutSyncButtonKey`, `SaveAndSyncButtonKey`, `OkButtonKey`, `.ctor`)
  - `ControlSettings` (`Instance`, `HandleFocusForIme`, `EnableFontCaching`, `MaxNumberOf2dPoints`, `MaxNumberOf3dPoints`, `MinPointsForDensity`, `MinPointsForForceDensity`, `DensityPointsPrintDPI`, `.ctor`, `.cctor`)
- Report: `.mirror-cache/release-il-07c4254a6803/release-il.json` — PASS, 1,196 checked, 1,196 exact, 0 failures, 2,709 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 31 — Urls (constants)

- Scope:
  - `Urls` (`MathcadExpressUpgrade`, `MathcadCommunity`, `MathcadEngineeringResources`, `MathcadSupportCenter`, `MathcadWebsiteAddress`, `MathcadLearningConnector`, `MathcadNewUsers`, `MathcadStartPageAddress`)
- Report: `.mirror-cache/release-il-d4d28c2c189c/release-il.json` — PASS, 1,196 checked, 1,196 exact, 0 failures, 2,709 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 32 — SingleRichTextCopyVisitor

- Scope:
  - `SingleRichTextCopyVisitor` (`Visit(IRichTextWithEmbeddedItem)`, `Visit(ICollapsibleAreaControl)`)
- Report: `.mirror-cache/release-il-d17565e84274/release-il.json` — PASS, 1,198 checked, 1,198 exact, 0 failures, 2,707 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 33 — PresentationSettings

- Scope:
  - `PresentationSettings` (`pageModelName`, `textStyleName`, `document`, `localName`, `_namespaceName`, `_resultsNS`, `pageModel`, `mathRendering`, `PageModelName`, `MathRenderingName`, `ShouldValidateOnWrite`, `ShouldValidateOnRead`, `GetXName`, `ReadXml(XmlReader)`, `WriteXml(XmlWriter)`, `CreateXElement`, `OnSchemaValidation`, `ReadXml(XElement)`, `WriteXml(XElement)`, `WriteXml(XDocument)`, `RepairXDocument`, `.ctor(IEngineeringDocument, string, string)`, `.cctor`)
- Report: `.mirror-cache/release-il-36fa8860970a/release-il.json` — PASS, 1,206 checked, 1,206 exact, 0 failures, 2,700 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 34 — CustomInsertedMenuItemsHelper

- Scope:
  - `CustomInsertedMenuItemsHelper` (`ContextMenuOpened`, `TryInsertMenuItemByKey`, `GetMenuItemWithCommand`, `IndexOfCommandMenuItem`)
- Report: `.mirror-cache/release-il-15b2dd9f990e/release-il.json` — PASS, 1,212 checked, 1,212 exact, 0 failures, 2,696 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 35 — NumericUpDownWatermark

- Scope:
  - `NumericUpDownWatermark` (`WatermarkValueProperty`, `NumberValue`, `WatermarkValue`, `OnLoaded`, `WatermarkVisibilityToggle`, `.ctor`, `.cctor`)
- Report: `.mirror-cache/release-il-328de43f6fcd/release-il.json` — PASS, 1,219 checked, 1,219 exact, 0 failures, 2,690 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 36 — PrimeExceptionLogger

- Scope:
  - `PrimeExceptionLogger` (`OpenFilesListGetter`, `GetExceptionData`, `GetOpenFilesData`, `GetUndoStackData`, `GetLicenseTypeData`, `GetApplicationVersionData`, `GetMiscInfo`)
- Report: `.mirror-cache/release-il-efe0c13161e6/release-il.json` — PASS, 1,229 checked, 1,229 exact, 0 failures, 2,684 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 37 — Properties.Settings

- Scope:
  - `Properties.Settings` (`HandleFocusForIme`, `EnableFontCaching`, `MaxNumberOf2dPoints`, `MaxNumberOf3dPoints`, `MinPointsForDensity`, `MinPointsForForceDensity`, `DensityPointsPrintDPI`, `.cctor`)
- Report: `.mirror-cache/release-il-a71c3647f7a9/release-il.json` — PASS, 1,244 checked, 1,244 exact, 0 failures, 2,670 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 38 — Properties.Resources

- Scope:
  - `Properties.Resources` (`resourceMan`, `ResourceManager`)
- Report: `.mirror-cache/release-il-f42615804a36/release-il.json` — PASS, 1,245 checked, 1,245 exact, 0 failures, 2,669 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 39 — Property (Serialization)

- Scope:
  - `Property` (`component`, `_propertyName`, `PropertyValue`, `StringPropertyValue`, `.ctor(object, string, TypeConverter)`, `.ctor(object, string)`, `AttemptToGetDefaultValue`, `ResetValue`)
- Report: `.mirror-cache/release-il-e3be1fe26cdd/release-il.json` — PASS, 1,253 checked, 1,253 exact, 0 failures, 2,661 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 40 — MessagesDecorator

- Scope:
  - `MessagesDecorator` (`_elem`, `_adornerList`, `_messageList`, `AdornerList`, `Attach`, `Detach`, `HandleMessageListChanged`, `CreateMessages`, `ClearMessages`, `.ctor`)
- Report: `.mirror-cache/release-il-1ee7eac46ae6/release-il.json` — PASS, 1,259 checked, 1,259 exact, 0 failures, 2,655 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 41 — ViewModelBase

- Scope:
  - `ViewModelBase` (`OnPropertyChanged`, `ModelPropertyChangedInner`, `Dispose`, `CreateCommandBinding(ICommand, Action<object>, Predicate<object>)`, `CreateCommandBinding(ICommand, Action<RoutedCommand, object>, Predicate<object>)`, `CreatePreviewCommandBinding`, `GetNewValue`, `.ctor`)
- Report: `.mirror-cache/release-il-66f04ad181d9/release-il.json` — PASS, 1,277 checked, 1,277 exact, 0 failures, 2,647 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 42 — PasswordDialog

- Scope:
  - `PasswordDialog` (`ShowError`, `FirePropertyChanged`, `OnCancelButtonClick`, `OnAcceptPasswordButtonClick`, `Properties.ShowError`, `Properties.AreaBorder`)
- Report: `.mirror-cache/release-il-b7cb64f262ca/release-il.json` — PASS, 1,282 checked, 1,282 exact, 0 failures, 2,642 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 43 — SelectionDragger

- Scope:
  - `SelectionDragger` (`.ctor`, `MouseMove(object, MouseEventArgs)`, `MouseUp(object, MouseButtonEventArgs)`, `MouseMove(Point)`, `MouseUp(Point)`)
- Report: `.mirror-cache/release-il-44d650c384d6/release-il.json` — PASS, 1,290 checked, 1,290 exact, 0 failures, 2,634 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 44 — WorksheetPersistentData and WorksheetSectionPersistentData

- Scope:
  - `WorksheetPersistentData` (`.ctor`)
  - `WorksheetSectionPersistentData` (`.ctor`)
- Report: `.mirror-cache/release-il-986490236260/release-il.json` — PASS, 1,292 checked, 1,292 exact, 0 failures, 2,632 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 45 — PageSizeInformation

- Scope:
  - `PageSizeInformation` (fields + `.ctor`)
- Report: `.mirror-cache/release-il-68f2f1cf9c3e/release-il.json` — PASS, 1,293 checked, 1,293 exact, 0 failures, 2,631 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 46 — ResultFormat

- Scope:
  - `ResultFormat` (`.cctor`, `.ctor(string)`, `.ctor(NumberFormat, MatrixFormat, string)`, `NumberFormat`, `MatrixFormat`, `ReadXml`, `WriteXml`)
- Report: `.mirror-cache/release-il-db937cc3bf96/release-il.json` — PASS, 1,302 checked, 1,302 exact, 0 failures, 2,622 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 47 — MathStyles

- Scope:
  - `MathStyles` (`.ctor`, `ReadXml(XmlReader)`, `WriteXml(XmlWriter)`, `ReadXml(XElement)`, `WriteXml(XElement)`)
- Report: `.mirror-cache/release-il-573267255930/release-il.json` — PASS, 1,307 checked, 1,307 exact, 0 failures, 2,617 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 48 — MathStyle

- Scope:
  - `MathStyle` (`.ctor()`, `.ctor(ExtendedFont, XNamespace)`, `.ctor(IdLabels, ExtendedFont)`, `.ctor(ExtendedFont)`, `.ctor(XNamespace)`, `.ctor(IdLabels, ExtendedFont, XNamespace)`, `Font`, `CreateChildren`, `ReadXml`)
- Report: `.mirror-cache/release-il-feed5924d75d/release-il.json` — PASS, 1,317 checked, 1,317 exact, 0 failures, 2,607 unfinished.
- Remaining mismatches: none in claimed batch.
