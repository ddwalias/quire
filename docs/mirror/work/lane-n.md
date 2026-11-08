# Lane N — Quire.Controls.Core

- Owner: Lane N (Orca task `task_a0cac29ff7b0`); branch `mirror/n-controls-core-20261107-b`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/lane-n-controls-core-20261107`.

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

## Batch 49 — OleContainerMessageKeys

- Scope:
  - `OleContainerMessageKeys` (all constants)
- Report: `.mirror-cache/release-il-d466b82179ce/release-il.json` — PASS, 1,317 checked, 1,317 exact, 0 failures, 2,607 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 50 — PlotCommonProperties

- Scope:
  - `PlotCommonProperties` (`.cctor`, `CreateShowTicksMetadata()`, `CreateShowTicksMetadata(PropertyChangedCallback)`, `CreateShowNumbersMetadata()`, `CreateShowNumbersMetadata(PropertyChangedCallback)`, `PropertyNames`)
- Report: `.mirror-cache/release-il-ddc4f62d3da5/release-il.json` — PASS, 1,322 checked, 1,322 exact, 0 failures, 2,602 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 51 — PlotCommonConstants

- Scope:
  - `PlotCommonConstants` (`.cctor`, `GetPlotLineThickness(PlotLineTicknessType)`, all fields/constants)
- Report: `.mirror-cache/release-il-799990f28004/release-il.json` — PASS, 1,324 checked, 1,324 exact, 0 failures, 2,600 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 52 — PageLayoutConstants

- Scope:
  - `PageLayoutConstants` (`.cctor`, `GetPageSizeUnitsByPageSize`, all constants)
- Report: `.mirror-cache/release-il-3f452f66933a/release-il.json` — PASS, 1,326 checked, 1,326 exact, 0 failures, 2,598 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 53 — RtfConstants

- Scope:
  - `RtfConstants` (all string constants)
- Report: `.mirror-cache/release-il-c037587d5ab0/release-il.json` — PASS, 1,326 checked, 1,326 exact, 0 failures, 2,598 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 54 — RegionBorderConstants

- Scope:
  - `RegionBorderConstants` (`.cctor`, all fields/constants)
- Report: `.mirror-cache/release-il-28205641c23c/release-il.json` — PASS, 1,327 checked, 1,327 exact, 0 failures, 2,597 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 55 — WhiteboardDependencyProperties

- Scope:
  - `WhiteboardDependencyProperties` (`.cctor`, `HandleIsSelectedPropertyChanged`, `GetIsSelected`, `SetIsSelected`, `HandleIsActivePropertyChanged`, `GetIsActive`, `SetIsActive`, `GetUserResizable`, `SetUserResizable`, `GetShowResizeIndicator`, `SetShowResizeIndicator`, `GetIsRegionBorderVisible`, `SetIsRegionBorderVisible`, `ToggleRegionBorderVisibility`, properties)
- Report: `.mirror-cache/release-il-b97dd206f7e1/release-il.json` — PASS, 1,341 checked, 1,341 exact, 0 failures, 2,583 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 56 — MC14Conversion.SolveBlockXmlElements and ImeHandler

- Scope:
  - `MC14Conversion.SolveBlockXmlElements` (`.cctor`, `ContainNode`, `ContainNodes`, `ContainsSolverFunctionWithFilter`, `ContainsMCDXMinMaxSolverFunction`, `SolverFunctions`)
  - `ImeHandler` (`SetIsInputMethodSuspended`)
- Report: `.mirror-cache/release-il-ad9d2e5d7e9b/release-il.json` — PASS, 1,347 checked, 1,347 exact, 0 failures, 2,587 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 57 — Protection.ProtectDialog properties and DisplayState

- Scope:
  - `ProtectDialog` (`DisplayState`, `AreaBorderState`, `DoPasswordsMatch`, `ProtectProperties.DisplayState`)
- Report: `.mirror-cache/release-il-d48b1a3c68a2/release-il.json` — PASS, 1,352 checked, 1,352 exact, 0 failures, 2,582 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 58 — Protection.UnprotectDialog DoPasswordsMatch

- Scope:
  - `UnprotectDialog` (`DoPasswordsMatch`)
- Report: `.mirror-cache/release-il-10ae5b32d464/release-il.json` — PASS, 1,353 checked, 1,353 exact, 0 failures, 2,581 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 59 — Excel.ExcelReader helper methods

- Scope:
  - `ExcelReader` (`.cctor`, `GetRow`, `GetColumn`, `GetNextColumn`, `CompareColumn`, `MinColumn`, `CreateColumns`, `addressRegex`)
- Report: `.mirror-cache/release-il-3bdeb2433632/release-il.json` — PASS, 1,360 checked, 1,360 exact, 0 failures, 2,575 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 60 — FontConstants

- Scope:
  - `FontConstants` (`.cctor`, `GetDefaultFontForMath`, `GetDefaultIdLabelFormat`, constants/fields)
- Report: `.mirror-cache/release-il-1879929a045e/release-il.json` — PASS, 1,363 checked, 1,363 exact, 0 failures, 2,573 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 61 — WorksheetPropertyNames

- Scope:
  - `WorksheetPropertyNames` (`.cctor`, `Contains`, constants/fields)
- Report: `.mirror-cache/release-il-301aef15d99e/release-il.json` — PASS, 1,365 checked, 1,365 exact, 0 failures, 2,572 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 62 — Dialogs.MessageDialogBase<T>

- Scope:
  - `MessageDialogBase<T>` (`.ctor`, `SetMessageImageAndButtons`, `SetDialogResultAndClose`, `OnButtonClick`, `AddButton`, `OnClosed`, `SetButtonValue`, `GetButtonValue`, `_buttonsDict`)
- Report: `.mirror-cache/release-il-faab846a4d70/release-il.json` — PASS, 1,373 checked, 1,373 exact, 0 failures, 2,564 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 63 — Settings.UnitSystemCustomized

- Scope:
  - `UnitSystemCustomized` (`.ctor`, `CreateChildren`, fields `_engineeringDocument`, `_localName`, `_nameSpaceUri`, lambda methods `<CreateChildren>b__4_0`, `<CreateChildren>b__4_1`)
- Report: `.mirror-cache/release-il-0d6f0910fbb7/release-il.json` — PASS, 1,377 checked, 1,377 exact, 0 failures, 2,562 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 64 — Settings.TextRenderingSerializer

- Scope:
  - `TextRenderingSerializer` (`.ctor`, `.cctor`, `CreateChildren`, `GetProofingLanguageSerializableAttribute`, `LocalName`, `Ns`, `_textStyles`, `TextStyles`, `WorksheetData`, `XmlProofingLanguageConverter`)
- Report: `.mirror-cache/release-il-0e8bcbc7ba7c/release-il.json` — PASS, 1,381 checked, 1,381 exact, 0 failures, 2,559 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 65 — FrameworkElementXamlHelper

- Scope:
  - `FrameworkElementXamlHelper` (`GetPrintView`, `ClearBitmapEffects`, `.ctor`, closure `b__0_0`)
- Report: `.mirror-cache/release-il-9cfc4b5f7e8b/release-il.json` — PASS, 1,386 checked, 1,386 exact, 0 failures, 2,557 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 66 — Settings.McdxPresentationSettings

- Scope:
  - `McdxPresentationSettings` (`.ctor`, `GetSchema`, `ReadXml`, `WriteXml`, `_textRendering`)
- Report: `.mirror-cache/release-il-26a6dc117a68/release-il.json` — PASS, 1,390 checked, 1,390 exact, 0 failures, 2,553 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 67 — Settings.TextStyleCollectionSerializable

- Scope:
  - `TextStyleCollectionSerializable` (`.ctor`, `ToTextFormats`, `get_Item`, `set_Item`, `Add`, `CreateChildren`, `LocalName`, `_styles`, compiler-generated closure `<>c`, `b__3_0`, `b__3_1`)
- Report: `.mirror-cache/release-il-7f118b0865b3/release-il.json` — PASS, 1,400 checked, 1,400 exact, 0 failures, 2,547 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 68 — Settings.TextFormatSerializer

- Scope:
  - `TextFormatSerializer` (`.ctor`, `ReadXml`, `GetThisElement`, `CreateChildren`, `get_TextDecorationSerializer`, `set_TextDecorationSerializer`, `get_StyleKey`, `set_StyleKey`, `get_TextFormat`, `set_TextFormat`, lambda methods `<GetThisElement>b__15_0`, `<GetThisElement>b__15_1`)
- Report: `.mirror-cache/release-il-27adc4f998ee/release-il.json` — PASS, 1,408 checked, 1,408 exact, 0 failures, 2,541 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 69 — Whiteboard.WhiteboardItemSearchRestrictor

- Scope:
  - `WhiteboardItemSearchRestrictor` (`.ctor`, `TryCreateFindRestrictor`, `CreateFindRestrictor`, `get_CanSearch`, `get_SearchableRegion`, `_whiteboardItem`, `_searchableRegion`, `_itemManager`)
- Report: `.mirror-cache/release-il-0b437db38abe/release-il.json` — PASS, 1,413 checked, 1,413 exact, 0 failures, 2,536 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 70 — Serialization.SerializableXmlPackagePart

- Scope:
  - `SerializableXmlPackagePart` (`.ctor(1)`, `.ctor(2)`, `Write`, `Read`, `Validate`, `OnWriteValidationFailedDevAssert`, `OnWriteValidationFailedHandler`, `_xmlSerializable`, `OnWriteValidationFailed`, closures `<>c__DisplayClass5_0`, `<>c__DisplayClass8_0`)
- Report: `.mirror-cache/release-il-762762adda4f/release-il.json` — PASS, 1,423 checked, 1,423 exact, 0 failures, 2,530 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 71 — Protection.UnprotectDialog

- Scope:
  - `UnprotectDialog` (`ShowUnProtectDialog`, `.ctor`, `InitializeComponent`, `_CreateDelegate`, `Connect`, `_doesPasswordsMatch`, `_hasPassword`, `_PasswordBox`, `_contentLoaded`)
- Report: `.mirror-cache/release-il-5e3dcf20e205/release-il.json` — PASS, 1,427 checked, 1,427 exact, 0 failures, 2,526 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 72 — Controls.Core.Crosshair

- Scope:
  - `Crosshair` (`.ctor`, `InitializeComponent`, `Connect`, `_contentLoaded`)
- Report: `.mirror-cache/release-il-dc00752552d8/release-il.json` — PASS, 1,430 checked, 1,430 exact, 0 failures, 2,523 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 81 — Controls.Core.FileManager

- Scope:
  - `FileManager` (`EnsureStreamRelease`, `IsPackage(string, string)`, `IsPackage(string, string, bool)`, `IsWorksheet`, `WaitFor`, `IsPrimeExtension`, `.ctor`)
  - `FileManager.Extension` (`.ctor`, constants `Xmcd`, `Mcdx`, `Mctx`, `Rtf`, `Xps`, `Dita`, `Pdf`, `All`)
- Report: `.mirror-cache/release-il-b3f1f86b9de7/release-il.json` — PASS, 1,505 checked, 1,505 exact, 0 failures, 2,455 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 82 — Controls.Core.ViewModelBaseWithDataErrors

- Scope:
  - `ViewModelBaseWithDataErrors` (`_errorsDictionary`, `_errors`, `_hasErrors`, `get_HasErrors`, `set_HasErrors`, `ErrorsChanged`, `.ctor`, `GetErrors`, `SetError`, `RemoveAllErrorsForProperty`)
- Report: `.mirror-cache/release-il-42c984cbe118/release-il.json` — PASS, 1,511 checked, 1,511 exact, 0 failures, 2,449 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 83 — Controls.Core.VisualAdornerBase, Settings.XmlLanguageConverterWithDefaultValue, and Serialization interfaces

- Scope:
  - `VisualAdornerBase` (`_typeOfThis`, `AdornerAddedEvent`, `_removed`, `_renderTransformGroup`, `AdornedElementAdornerLayer`, `ParentAdornerLayer`, `AdornedFrameworkElement`, `AddToAdornerLayer(AdornerDecorator, Adorner)`, `.cctor`, `CoerceRenderTransformPropertyValue`, `.ctor`, `UpdateRenderTransformGroup`, `FireAdornerAdded`, `Remove`, `Add`, `AddToAdornerLayer()`, `AdornedElementLoadedHandler`, `OnLayoutUpdated`)
  - `Settings.XmlLanguageConverterWithDefaultValue` (`.ctor`, `ConvertTo`)
  - `Serialization.IXDocumentRepairer` (`RepairXDocument`)
  - `Serialization.IXmlAndXElementSerializable` (`WriteXml(XDocument)`)
- Report: `.mirror-cache/release-il-30727802bf1d/release-il.json` — PASS, 1,513 checked, 1,513 exact, 0 failures.
- Remaining mismatches: none in claimed batch.

## Batch 84 — Interface members (63 small interfaces)

- Claimed: every stubbed member of the 63 interfaces with at most two stubs (interface properties, methods and events become abstract declarations in hand-written partials, `Controls.Core` namespaces unchanged; generated stubs removed in the same change).
- Report: `.mirror-cache/release-il-67212f244b09/release-il.json` — PASS, 1,622 checked, 0 failures, 2,332 unfinished (was 2,443).
- Remaining mismatches: none in claimed batch.
- Note: `MirrorGen emit` in this worktree rewrote 742 generated files across 36 projects (stubs added in projects outside this lane); those were reverted and not committed. It did not drop the interface stubs, so they were removed by hand as in the batch 83 pattern.
- Note: untracked `Generated/Mirror/` folders for `Quire.AutomationPrivate`, `Quire.Interop.Mathcad`, `Quire.Interop.MathcadOld` and `Quire.Interop.VSInterfacesLib` were created by that emit run and are left uncommitted; remove them when the lane finishes.

## Batch 85 — Interface members (36 interfaces with at most nine stubs)

- Claimed: every stubbed member of 36 interfaces (`ISpecialCopyVisitor`, `IPackagePartReferenceManager`, `IWhiteboardItem`, `IIOItem`, `IIORegionContainer`, `IFontManager`, `ICollapsibleAreaControl`, and 29 others), declared abstract in new hand-written partials; generated stubs removed in the same change.
- Report: `.mirror-cache/release-il-49675c41d839/release-il.json` — PASS, 1,694 checked, 0 failures, 2,149 unfinished (was 2,332).
- Remaining mismatches: none in claimed batch.

## Batch 86 — Interface members (10 interfaces with 8–19 stubs)

- Claimed: `IAlignmentGrid`, `IUIElementFacade`, `IOleContainer`, `IMathFormat`, `IRichTextBase`, `IRichTextWithEmbeddedItem`, `ITextStylesManager`, `IWhiteboardItemManager`, `IFrameworkElementFacade`, `IWorksheetProperties` (abstract declarations in hand-written partials; generated stubs removed).
- Report: `.mirror-cache/release-il-5256ffb7bfb0/release-il.json` — PASS, 1,660 checked, 0 failures, 2,000 unfinished (was 2,149).
- Remaining mismatches: none in claimed batch.

## Batch 87 — Interface members (6 large interfaces)

- Claimed: `IIORegionManager`, `IContainerItemManager`, `IWorksheetPersistentData`, `IWhiteboardControl`, `IEquationControl` (except `Changed`), `IEngineeringDocument` (abstract declarations in hand-written partials; generated stubs removed).
- Report: `.mirror-cache/release-il-e16d7ce73eee/release-il.json` — PASS, 1,910 checked, 0 failures, 1,601 unfinished (was 2,000).
- Remaining mismatches: `IEquationControl.Changed` stays a stub. Declared as `RoutedEventHandler` in Mathcad, but Quire's `RegionControl.Changed` is `EventHandler?` (hand-written), so the abstract declaration breaks `Quire.Controls.EquationControl` (CS0738, another lane's project, found by the merge gate, not by the local check).
- Tooling note: `release_il.py --project Quire.Controls.Core` built only `Quire.Controls.Core`; the merge gate built 12 dependents (`Quire`, `Quire.Controls*`, `Quire.EquationEditor`). Interface changes need the dependents built locally before landing.

## Batch 88 — Small classes (extension getter, ImeHandler)

- Claimed: `HasUIElementArrangeInfoProviderExtension.ArrangeInfoProvider`, `ImeHandler.HandleImeFocus`, `ImeHandler.PositioningIme` (hand-written bodies; generated stubs removed).
- Report: `.mirror-cache/release-il-41edbbc225a7/release-il.json` — PASS, 1,913 checked, 0 failures, 1,598 unfinished, 3 more exact.
- Remaining mismatches (not in this batch's claim): `FindReplaceQuery.IsFullySubscript` stays a stub. The Release IL calls `List<T>.get_Count` with `call`; the C# form emits `callvirt`, and no idiomatic C# shape changes that.
- Batch 84–87 are committed on this branch and not yet landed (merge gate blocked by the main-checkout untracked folder; maintainer hold).

## Batch 89 — Settings and serialization repairers

- Claimed: `Serialization.Integration10Repairer` (`RepairXDocument`, `UpdateNameSpace`), `Settings.Calculation10Repairer` (`RepairXDocument`, `UpdateNodeValue`, consts `_sourceNamespaceURI`, `_targetNamespaceURI`).
- Report: `.mirror-cache/release-il-833cbf816992/release-il.json` — PASS, 1,917 checked, 0 failures, 1,594 unfinished, 1,518 exact.
- Remaining mismatches: none in claimed batch. Guard shape `if (Failed == DevAssert(...)) return;` is kept because Release IL branches on the inverted test.

## Batch 90 — Serialization.Result10Repairer

- Claimed: `Result10Repairer` (`RepairXDocument`, `UpdateNameSpace`, `UpdateResultNode`, const `TargetNamespaceUri`).
- Report: `.mirror-cache/release-il-69780d0feb6c/release-il.json` — PASS, 1,521 checked, 1,521 exact, 0 failures, 1,591 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 91 — Settings.Presentation10Repairer

- Claimed: `Presentation10Repairer` (`RepairXDocument`, `UpdateNodeValue`, `UpdateNameSpace(IEnumerable, ICollection, string, string)`, `UpdateNameSpace(XDocument)`, `CreateNewParent`, `UpdateNodeStruct`, consts `_sourceNamespaceURI`, `_targetNamespaceURI`).
- Report: `.mirror-cache/release-il-a86c4fd4baba/release-il.json` — PASS, 1,530 checked, 1,530 exact, 0 failures, 1,585 unfinished.
- Remaining mismatches: none in claimed batch.

## Batch 92 — CachedWorksheetPropertiesSource, BusyClock, BytesComparer, SolveBlockXmlElements, ListOfRegionPersistentDataExtensions

- Claimed:
  - `CachedWorksheetPropertiesSource` (`.ctor(IWorksheetProperties)`, `_typeOfThis`)
  - `BusyClock` (`BusyClockAngleProperty`, `BusyClockAngle`, `DefiningGeometry`)
  - `Serialization.SerializationHelper.BytesComparer` (`Equals(byte[], byte[])`, `GetHashCode(byte[])`)
  - `MC14Conversion.SolveBlockXmlElements` (`GetEnumerator`)
  - `Serialization.ListOfRegionPersistentDataExtensions` (`AllDeserializationExceptions(IList<IRegionPersistentData>)`, `HasDeserializationErrors(IList<IRegionPersistentData>)`)
- Report: `.mirror-cache/release-il-45a965110f26/release-il.json` — PASS, 1,560 checked, 1,560 exact, 0 failures, 1,576 unfinished (was 1,585).
- Remaining mismatches: none in claimed batch.

## Batch 93 — TableLayoutManager.RowMetric, ControlsWindow, FindUtilities, TextDecorationsToAttributesSerializer

- Claimed:
  - `TableLayoutManager.RowMetric` (`.ctor(double, double)`, `RowHeight`, `RowBaseline`, `RowBottom`)
  - `ControlsWindow` (`DisplayHelpCommand`, `.ctor()`, `HelpCommandHandler`, `DialogLoaded()`, `OnOkButtonClick()`)
  - `FindUtilities` (`CaseSensitiveStringComparison`, `CaseInsensitiveStringComparison`, `GetIndexInText`, `IsWholeWord`, `CreateSearchFunctionForGivenString`, `CreateCaseInsensitiveSearchFunctionForGivenString`, `.ctor()`, plus closures)
  - `Settings.TextDecorationsToAttributesSerializer` (`LocalName`, `Underline`, `Overline`, `Strikethrough`, `Baseline`, `_existingDecorations`, `HasUnderline`, `HasOverLine`, `HasStrikethrough`, `HasBaseline`, `.ctor`, `ReadElementContents`, `WriteElementContents`, `CreateChildren`) and nested `TextDecorationsExistence` (`_collection`, `Exists`, `Collection`, `.ctor`, `IsSameDecoration`)
- Report: `.mirror-cache/release-il-e645f53c0397/release-il.json` — PASS, 1,598 checked, 1,598 exact, 0 failures, 1,551 unfinished (was 1,576).
- Remaining mismatches: none in claimed batch.

## Batch 94 — RtfHelper leaf members and TableLayoutManager

- Claimed:
  - `RtfHelper` leaf members (`.cctor`, `ConvertPixelsToTwips(double)`, `NormalizeHorizontalPosition(double)`, consts `DefaultMarginInTwips`, `TwipsPerPoint`, `PointsPerInch`, `TwipsPerInch`, `WpfPointsPerInch`, `LeftMarginTag`, `RightMarginTag`, `PageWidthTag`)
  - `TableLayoutManager` (all 33 remaining members: fields, constructors, point converters, `SearchOnColumn`, `SearchOnRow`, `CellIndexFromPoint`, `Measure` overloads, `CalculateRowAndColumnMetrics`, metric getters, `RowMetric`, and closure `<Measure>b__55_0`)
- Report: `.mirror-cache/release-il-424c105e02cb/release-il.json` — PASS, 1,632 checked, 1,632 exact, 0 failures, 1,517 unfinished (was 1,551).
- Remaining mismatches: none in claimed batch.

## Batch 95 — Settings.ResultsSerializer, SnappableItem helpers, and CustomScriptServiceFunctions

- Claimed:
  - `Settings.ResultsSerializer` (`LocalName`, `.ctor`, `CreateChildren`, `WriteElementContents`, `ReadElementContents`, `ReadXml(XElement)`)
  - `SnappableItemBaseHelper` (`_renderTarget`, `_pageDescendantItem`, `_isOnVisiblePageChangedHandler`, `_targetLoadedHandler`, `_targetUnloadedHandler`, `_layoutUpdatedHandler`, `IsPageDescendant`, `RenderTarget`, `ItemElement`, `.ctor`, `Dispose`, `RegisterEvents`, `UnregisterEvents`, `RecalculateRenderTransform`, `FindIsPageDescendant`, and closure `<.ctor>b__12_0`)
  - `SnappableZombieItemHelper` (`_zombie`, `_onLayoutUpdated`, `Create<T>`, `.ctor`, `Dispose`, `OnLayoutUpdated`, nested interface `IZombieElement`, generic wrapper `ZombieElement<T>`, and closure `<.ctor>b__5_0`)
  - `Whiteboard.SnapToPixelWhiteboardHelper` (`_item`, `Create<T>`, `.ctor`, `Dispose`, `TransformFromPoint`, `HasPointThatSnapsToPixel`, nested interface `IContainerElement`, generic wrapper `ContainerElement<T>`, and closures `<get_TransformFromPoint>b__6_0`, `<get_HasPointThatSnapsToPixel>b__8_0`)
  - `Controls.Text.CustomScriptServiceFunctions` (`RunTypeToScriptState`, `ConvertScriptLevelInternal`, `CopyLocalFormattingValues<T>`, `SplitRun(TextPointer)`, `SplitRun<T>(TextPointer)`)
- Report: `.mirror-cache/release-il-6e8d0d9060fc/release-il.json` — PASS, 1,676 checked, 1,672 exact, 0 failures, 1,484 unfinished (was 1,517).
- Remaining mismatches: none in claimed batch.

## Batch 96 — HasUIElementArrangeInfoProviderExtension, MessageDialogBase, PropertyAggregateExtension, and VisualTableContents

- Claimed:
  - `HasUIElementArrangeInfoProviderExtension` (`_facade`)
  - `Dialogs.MessageDialogBase<T>` (`Message`, `Image`, `Buttons`)
  - `PropertyAggregateExtension` (`SetPropertyValueWithCommandUndoable`, `SetPropertyValueWithCommand` (2 overloads), `ValidateReferenceType<T>`, `ValidateNullableType<T>`, `IsValidateValueType<T>`, `ErrorMessageTypeMissmatch`, `GetCommonPropertyValueForList`, `GetCommonFormat<T>`, plus closures `<>c`, `<>c__8`1`, `<>c__DisplayClass0_0`, `<>c__DisplayClass7_0`)
  - `VisualTableContents` (`Rows`, `CellCount`, `.ctor(TableLayoutManager)`, `OnVisualChildrenChanged`, `MeasureOverride`, `GetChildrenInRowMajorOrder`, `ArrangeOverride`, and nested iterator `<GetChildrenInRowMajorOrder>d__29`)
- Report: `.mirror-cache/release-il-88cba98920c7/release-il.json` — PASS, 1,708 checked, 1,708 exact, 0 failures, 1,469 unfinished (was 1,484).
- Remaining mismatches: none in claimed batch.

## Batch 97 — INotifyMeasureOverride, IIORegion, ISelectionDragger, SerializationHelperUtils, AnnotationTypeList, and NodeHandleMapper

- Claimed:
  - `INotifyMeasureOverride` (`MeasureOverideCompleted`)
  - `IORegions.IIORegion` (`IORegionChanged`)
  - `ISelectionDragger` (`DragDone`)
  - `Serialization.SerializationHelperUtils` (`IsConvertedFromMathcad14`, `SetConvertedFromMathcad14`, `SaveToPartUsingTemporaryFile`, `TryDeleteTemporaryFile`, `CopyObfuscatedStream`, `FinalizeSeekableStream`, `InitializeSeekableFileStream`, `Obfuscate`, `Mask`)
  - `Serialization.AnnotationTypeList` (`.ctor()`, `.ctor(AnnotationTypeList, bool)`, `ToStrings()`, `this[int]`, `AddOnce`, `Contains(string)`, `Add`, `Clear`, `Contains(MC14ConversionAnnotationType)`, `CopyTo`, `Remove`, `Count`, `IsReadOnly`, `Freeze`, `IsFrozen`, `GetEnumerator()`, `IEnumerable.GetEnumerator()`, and closure `<ToStrings>b__3_0`)
  - `NodeHandleMapper` (`_nodeHandleList`, `HandlePrefix`, `.ctor()`, `Clear()`, `GetReplacedRl(ICalculatable)`, `GetNodesAndHandles`, `GetReplacedRl(ICalculatable, string)`, `Map`, `GetMappedNodeNandle`, `GetMappedEquation`, `GetIndexFromHandle`)
- Report: `.mirror-cache/release-il-0dd5a792c0c2/release-il.json` — PASS, 1,745 checked, 1,739 exact, 0 failures, 1,435 unfinished (was 1,469).
- Remaining mismatches: none in claimed batch.

## Batch 98 — FindReplaceQuery, DeserializationStrategy, and RestrictedAreaAlignmentGrid

- Claimed:
  - `FindReplaceQuery` (`PostReplaceAndFindNext`, `IsFullySubscript`, `.ctor`)
  - `Serialization.DeserializationStrategy` (`Regions`, `RegionFactory`, `AddRegion(UIElement, Point)`, `DeleteRegion(UIElement)`, `IsRegionNotAffectingCalculation(IRegionType)`, `SetRegions(IEnumerable<IRegionType>, IDeS11NProvider)`, `AddRegion(IRegionPersistentData, bool)`, `AddRegion(IRegionPersistentData)`, `CleanUpLastRegion(IRegionPersistentData)`, and closure `<>c__DisplayClass16_0`)
  - `RestrictedAreaAlignmentGrid` (`AreasToSkip`, `.ctor()`, `.ctor(Size?)`, `GetNearestIntersectionInDirection`, `GetNextIntersection`, `GetNearestIntersectionPointDisregardingBounds`, `GetNearestInsertionPointIgnoringRestrictedAreas`, `IsWithinRestrictedArea`, `Dispose`, and closures `<>c__DisplayClass10_0`, `<>c__DisplayClass11_0`, `<>c__DisplayClass12_0`, `<>c__DisplayClass16_0`)
- Report: `.mirror-cache/release-il-21ed139c7207/release-il.json` — PASS, 1,767 checked, 1,767 exact, 0 failures, 1,419 unfinished (was 1,435).
- Remaining mismatches: none in claimed batch.

## Batch 99 — AttachedBehavior subsystem (PropertiesBinder, FocusActivator, and AttachedBehavior)

- Claimed:
  - `AttachedBehavior.PropertiesBinder` (`_typeOfThis`, `SourceValueConnectorProperty`, `TargetValueConnectorProperty`, `OnValueChanged`, `_pathToSource`, `_pathToTarget`, `SourceValueConnector`, `TargetValueConnector`, `PathToSource`, `PathToTarget`, `BlockUndoWhenBindingToTarget`, `.ctor()`, `Attach`, `CreateTargetBinding`, `Detach`, `.cctor()`, closures `<.cctor>b__26_0`, `<OnValueChanged>b__0`)
  - `AttachedBehavior.FocusActivator` (`_element`, `_activator`, `Attach`, `Detach`, `OnDataContextChanged`, `ElementGotFocus`, `ElementLostFocus`, `HookupToElementEvents`, `HookupToActivatorEvents`, `UnhookFromElementEvents`, `UnhookFromActivatorEvents`, `HandleMoveFocus`, `ActivateElement`, `.ctor()`)
  - `AttachedBehavior.AttachedBehavior` (`_typeOfThis`, `CanBeActivatedProperty`, `ParentPropertyValueKeepersProperty`, `ChildPropertyValueFindersProperty`, `PropertyBindersProperty`, `RoutedEventBindingsProperty`, `MessagesDecoratorProperty`, `GetCanBeActivated`, `SetCanBeActivated`, `GetParentPropertyValueKeepers`, `SetParentPropertyValueKeepers`, `GetChildPropertyValueFinders`, `SetChildPropertyValueFinders`, `GetPropertyBinders`, `SetPropertyBinders`, `SetRoutedEventBindings`, `GetRoutedEventBindings`, `GetMessagesDecorator`, `SetMessagesDecorator`, `CoerceAttachedBehavior<T1, T2>`, `CoerceBooleanToAttachedBehavior<T>`, `CoerceItemToCollection<T1, T2>`, `OnAttachedBehaviorValueChanged`, `.cctor()`)
- Report: `.mirror-cache/release-il-29aa36331ad4/release-il.json` — PASS, 1,804 checked, 1,804 exact, 0 failures, 1,389 unfinished (was 1,419).
- Remaining mismatches: none in claimed batch.

## Batch 100 — WhiteboardItemsCollection, FontManager, and RealNumberFormatter

- Claimed:
  - `WhiteboardItemsCollection` (`_parentIsOnVisiblePage`, `ParentIsOnVisiblePage`, `_parentSupportsInputsOutputs`, `ParentSupportsInputsOutputs`, `_containedInCollapsedWhiteboard`, `ContainedInCollapsedWhiteboard`, `.ctor()`, `.ctor(IEnumerable<IWhiteboardItem>)`, `OnRangeChanged`, `GetObjectData`, `.ctor(SerializationInfo, StreamingContext)`, `Dispose`, closures `<set_ParentIsOnVisiblePage>b__0`, `<set_ParentSupportsInputsOutputs>b__0`, `<set_ContainedInCollapsedWhiteboard>b__0`, `<Dispose>b__17_0`)
  - `FontManager` (`MaxCacheSize`, `_emptyTextDecorations`, `_underlineTextDecorations`, `_fontCache`, `_fontCacheMru`, `GetFont(string, double, bool, bool, bool, Color?)`, `GetFont(IMathFormat)`, `GetScaledFont`, `GetScaledFontSize`, `GetFont(FontKey)`, `AddFontToCache`, `IsConnected`, `.ctor()`, closure `<AddFontToCache>b__11_0`)
  - `RealNumberFormatter` (`_helper`, `DisplayNumberFormat`, `.ctor(NumberFormat, IFormatterHelper)`, `FormatReal`, `AddNegIfNeeded`, `FormatRealToGeneral`, `FormatRealToDecimal`, `FormatRealToExponential`, `GetBaseVal`, `CreateExponentialENotationFormat`, `ToStringAndBack`, `FormatStringAndIsZeroRepresentation`, `CreateExponentialMultiplyByBaseToAPowerFormat`, `FormatExponent`, `FormatRealToPercent`, `GetFormatStringForDouble`, closures `<FormatRealToDecimal>b__9_0`, `<CreateExponentialENotationFormat>b__0`, `<CreateExponentialMultiplyByBaseToAPowerFormat>b__15_0`, `<FormatRealToPercent>b__17_0`)
- Report: `.mirror-cache/release-il-a945a6001108/release-il.json` — PASS, 1,855 checked, 1,855 exact, 0 failures, 1,357 unfinished (was 1,389).
- Remaining mismatches: none in claimed batch.

## Batch 101 — BookmarkTagDialog, ExcelReader, and MathFormatDobjController

- Claimed:
  - `BookmarkTags.BookmarkTagDialog` (`InsertTagTextBox`, `OkButton`, `_contentLoaded`, `ShowBookmarkTagsDialog`, `.ctor(BookmarkTagViewModel)`, `BookmarkTagDialogLoaded`, `OnOkButtonClick`, `InitializeComponent`, `_CreateDelegate`, `IComponentConnector.Connect`, closure `<.ctor>b__4_0`)
  - `Excel.ExcelReader` (`_nsManager`, `_doc`, `_sharedStringDoc`, `CreateExcelCel`, `CreateExcelRow`, `CompareColumn`, `MinColumn`, `CreateColumns`, `CreateExcelSheet`, `GetNextColumn`, `.ctor()`; note: `GetSharedString` and `LoadExcelBook` remain stubs because DocumentFormat.OpenXml 3.x dropped the public `OpenXmlPackage.Package` property present in Mathcad's 2.17.1.0 dependency)
  - `MathFormatDobjController` (`FontFamilyName`, `FontSize`, `IsBold`, `IsItalic`, `IsUnderline`, `FontColor`, `HighlightColor`, `.ctor(DependencyObject)`, `SetPropertiesFrom`, `IsPropertySet`)
- Report: `.mirror-cache/release-il-9a030a55dd1b/release-il.json` — PASS, 1,883 checked, 1,883 exact, 0 failures, 1,330 unfinished (was 1,357).
- Remaining mismatches: none in claimed batch.

## Batch 102 — OleContainerBase, OleSizeDataWithNotification, and XElementSerializable

- Claimed:
  - `Ole.OleContainerBase` (`_typeOfThis`, `_bitmap`, `Bitmap`, `AcquireBitmapSource`, `CreateBitmap`, `GetRenderRectInPixels`, `get_IsOleCreated`, `OnZoomChanged`, `.ctor()`, `.cctor()`, closure `<.cctor>b__1_0`)
  - `Ole.OleSizeDataWithNotification` (`_autoResize`, `_oleViewportSize`, `HasFakeOleViewportSize`, `AutoResize`, `OleViewportSize`, `AutomaticSize`, `PropertyChanged`, `CopyFrom`, `.ctor(bool)`, `OnPropertyChanged`, `UpdateAndNotify`, `PropertyNames.AutoResize`, `PropertyNames.AutomaticSize`, and closures `<CopyFrom>b__0` in `<>c__DisplayClass6_0`, `<set_OleViewportSize>b__0` in `<>c__DisplayClass9_0`, `<set_AutoResize>b__0` in `<>c__DisplayClass16_0`)
  - `Serialization.XElementSerializable` (`_name`, `Children`, `Name`, `Namespace`, `HasChildren`, `.ctor(XName, params object[])`, `CreateChildren`, `GetThisElement(XElement)`, `GetThisElement()`, `ReadXml(XElement)`, `WriteXml(XElement)`, `WriteXml(XDocument)`, `WriteElementContents`, `ReadElementContents`, `GetSchema`, `ReadXml(XmlReader)`, `WriteXml(XmlWriter)`, `GetXmlAttributeName`, `GetXmlAttributeDefaultValue`)
- Report: `.mirror-cache/release-il-018bbc164611/release-il.json` — PASS, 1,922 checked, 1,922 exact, 0 failures, 1,301 unfinished (was 1,330).
- Remaining mismatches: none in claimed batch.

## Batch 103 — MC14ConversionAnnotation and MC14MathConversionValidation

- Claimed:
  - `Serialization.MC14ConversionAnnotation` (`_typeOfThis`, `ConversionAnnotationsProperty`, `TopLevelAnnotationAdornersPropertyKey`, `AnnotatedTooltipResourceKey`, `_resourceDictionary`, `GetConversionAnnotations`, `SetConversionAnnotations`, `ClearConversionAnnotations`, `HasConversionAnnotations`, `AddConversionAnnotation`, `SetOldConversionAnnotation`, `SetConversionFailedAnnotation`, `GetTopLevelAnnotationAdorners`, `SetTopLevelAnnotationAdorners`, `ClearTopLevelAnnotationAdorners`, `HandleChangedByConversionChanged`, `SetAnnotations`, `RemoveAnnotations`, `ElementIsLoadedSetAnnotations`, `ElementTrySetAnnotations`, `.cctor()`, `GetMC14ConversionAnnotationByType`, `GetConversionAnnotationDescriptor(MC14ConversionAnnotationType)`, `GetConversionAnnotationDescriptor(string)`, `GetCategoryDesription`, and closures `<RemoveAnnotations>b__17_0`, `<RemoveAnnotations>b__17_1`, `<ElementTrySetAnnotations>b__19_1`, `<ElementTrySetAnnotations>b__19_2`, `<ElementTrySetAnnotations>b__0` in `<>c__DisplayClass19_0`, `<ElementTrySetAnnotations>b__3` in `<>c__DisplayClass19_1`, `<ElementTrySetAnnotations>b__4` in `<>c__DisplayClass19_2`)
  - `PersistentDataObjects.MC14MathConversionValidation` (`PercentString`, `WhitespaceString`, `ElementOfString`, `_numberSetStrings`, `GradientString`, `GradientNumChildren`, `_types`, `MathValidate`, `ContainsSymbolicEvaluationOfSolver`, `ContainsUnsupportedNames`, `ContainsOldMc15Functions`, `ContainsWhitespaces`, `ContainsExponentialNotation`, `ContainsSymbolicEvaluationAsEquals`, `ContainsBooleanEqualityAsEquals`, `ContainsLocalDefinitionAsEquals`, `ContainsPercentOperatorInResult`, `ContainsOperatorWhichDisplayWasChanged`, `IsNotAutoSelectedOrNotDefaultAdvancedOptionsAlgorithm`, `ContainsMixedNumber`, `ContainsNameSpace`, `ContainsFontStyleAttribute`, `IsValidGradientExpression`, `IsValidElementOfExpression`, `ContainsInOrTreeFixity`, `ContainsOtherwiseOperator`, `AnnotateSymbolicIfNeeded`, `ContainsStyleAsRightShaft`, `ContainsHideLHS`, `ContainsHideKeyword`, `IsXNameInXDocument`, `ContainsAttribute(string, string, XContainer, Func)`, `ContainsAttribute(string, string, XContainer)`, `ContainsAttributeWithValue`, `ContainsAttributeWithValueNotEqualTo`, `GetMc14Math30XName`, `GetRealAndImaginaryElements`, `.cctor()`, and closures `<ContainsSymbolicEvaluationOfSolver>b__8_0`, `<ContainsUnsupportedNames>b__9_0`, `<ContainsWhitespaces>b__11_0`, `<ContainsPercentOperatorInResult>b__16_0`, `<IsValidElementOfExpression>b__0` in `<>c__DisplayClass23_0`, `<ContainsInOrTreeFixity>b__24_0`, `<ContainsAttribute>b__0` in `<>c__DisplayClass31_0`, `<ContainsAttribute>b__32_0`, `<ContainsAttributeWithValue>b__0` in `<>c__DisplayClass33_0`, `<ContainsAttributeWithValueNotEqualTo>b__0` in `<>c__DisplayClass34_0`)
- Report: `.mirror-cache/release-il-62e6a1c039e3/release-il.json` — PASS, 1,995 checked, 1,995 exact, 0 failures, 1,258 unfinished (was 1,301).
- Remaining mismatches: none in claimed batch.
- Still open: 1,258 unfinished members. Largest groups: `RegionBorder` (87), `WorksheetPageLayout` (95), `NumberFormat`/`MathFormat` (35/37), `AttachedProperties` (49), `Whiteboard.ContainerItemManager` (56), the `Ole` generics (`OleContainer_4`, `OleContainerModel_1`, `OleContainerViewModel_3`), and `IPublicOleContainerModel_1`.

## Batch 104 — ObservableUIElementCollection

- Owner: Lane N (Orca task `task_ed3239f64a4d`); branch `mirror/n-controls-core-20261108`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/controls-core-resume-20261108`.
- Claimed: every member of `Ptc.Controls.Core.ObservableUIElementCollection` (indexer, eight `UIElementCollection` overrides, `OnChanged`, `OnPropertiesChanged`, `OnPropertyChanged(string)`, `OnPropertyChanged(PropertyChangedEventArgs)`, `OnCollectionChanged`, `OnRangeChanging`, `OnRangeChanged`, explicit `INotifyCollectionRangeChanged.Count`, `IsEmpty`); stubs removed from the `.g.cs`, which keeps the class header.
- Report: `.mirror-cache/release-il-618908c0d5a5/release-il.json` — PASS, 2,013 checked, 2,013 exact, 0 failures, 1,240 unfinished (was 1,258).
- Remaining mismatches: none in claimed batch. `Equals` needs the flag-and-`as` shape: the idiomatic `is not X other` pattern gave a type-operation mismatch at the first `isinst`.
- Batch 103 (commit `6a4641ee`, cherry-picked here) landed on main; its Release IL gate passed twice (`release-il-a6e068ad616d`, `gate-il-823ed257df8c`, `gate-il-b7e5ee45b166`).

## Batch 105 — MatrixFormatSerializer

- Claimed: all members of `Ptc.Controls.Core.Serialization.MatrixFormatSerializer` (constants, fields, constructor, `ReadXml`, `ReadMatrixFormatAttrs`, `ReadNestedMatrixFormats`, `ReadNestedMatrixFormat`, `WriteXml`, `WriteMatrixFormatAttrs`, `WriteNestedMatrixFormats`, `WriteNestedMatrixFormat`); stubs removed from the `.g.cs`, which keeps the class header.
- Report: `.mirror-cache/release-il-878e26a23804/release-il.json` — PASS, 2,022 checked, 2,022 exact, 0 failures, 1,231 unfinished (was 1,240).
- Remaining mismatches: none in claimed batch. Two shapes are kept as Release IL requires: `ReadXml`/`WriteXml` return early on a null format, and compare `"matrix"` with the literal on the left. Writing `!(x == null)` or `!=` on `MatrixFormat` failed the gate with a different overload call, and an `if (x != null)` block shape also failed.

## Batch 106 — RegionBorder static members

- Claimed: `RegionBorder` constants (`Part*`), `_typeOfThis`, the static dependency property and routed event fields, the static constructor, and the static change handlers (`OnRegionBorderBackgroundChanged`, `OnRegionBorderBrushChanged`, `OnDraggerBackgroundChanged`, the eleven dragger visibility handlers, `OnDraggerVisibilityChanged`, `OnShowBusyChanged`, `OnActiveSelectedChanged`, `OnForceBorderChanged`, `OnRightEdgeVisibilityChanged`, `OnBottomEdgeVisibilityChanged`, `OnCornerRadiusChanged`). Stubs removed from the `.g.cs`, which keeps its instance members.
- Report: `.mirror-cache/release-il-7ccb2ab19086/release-il.json` — PASS, 2,041 checked, 2,041 exact, 0 failures, 1,213 unfinished.
- Remaining mismatches: none in claimed batch. Remaining RegionBorder work: instance fields and properties, `ResizeElement`, `GetRegionBorder`, the drag and busy/active logic, and the `RequestMoveAndResize` event.

## Batch 107 — RegionBorder dependency property wrappers

- Claimed: the 21 `GetValue`/`SetValue` property wrappers, `IsBorderShapeVisible`, `IsBorderHeaderVisible`, `BorderShapeColor`, the private `AreDraggersShown` property, and the `RequestMoveAndResize` event. Stubs removed from the `.g.cs`.
- Report: `.mirror-cache/release-il-c96fa06614b5/release-il.json` — PASS, 2,090 checked, 2,090 exact, 0 failures.
- Remaining mismatches: none in claimed batch. Still open in RegionBorder: `ResizeElement`, `GetRegionBorder`, the drag, busy and active logic, and the `OnApplyTemplate` and `GetBoundingRect` members.

## Batch 108 — RegionBorder template, drag and resize members

- Claimed: `OnApplyTemplate`, `GetBoundingRect`, `OnMouseLeftButtonDown`, `GetDragMode` (both overloads), `RaiseMoveAndResizeEvent`, `ResizeElement`, `GetGrip`, `GetRegionBorder`, `FindBorderDraggerByName`, plus `_busyProgressBar` and `HideBusyStateElements` (moved ahead of the lambda). Stubs removed from the `.g.cs`.
- Report: `.mirror-cache/release-il-4d12fcc3be6c/release-il.json` — PASS, 2,102 checked, 2,102 exact, 0 failures.
- Shapes Release IL requires: `FindBorderDraggerByName`'s lambda display class is numbered by the count of hand-written members declared before it (Mathcad: 95), so the method sits after the nine visibility properties and the busy members; `OnMouseLeftButtonDown` declares `originalControlElement` before `dragMode`; `GetDragMode` keeps the switched name in a local declared before the switch; `GetGrip` is a switch statement that returns from each arm.

## Batch 109 — RegionBorder visual-state helpers (part 1)

- Claimed: `UpdateBusyState`, `UpdateActiveState`, `SetBackgroundSelectedLook`, `SetBackgroundActiveLook`, `SetBackgroundDefaultLook`, `ShowBusyStateElements`, `UpdateZIndex`, `GetDesiredDraggerVisibility`. Stubs removed from the `.g.cs`.
- Report: `.mirror-cache/release-il-3618536adb5a/release-il.json` — PASS, 2,110 checked, 2,110 exact, 0 failures.
- Shapes Release IL requires: `ShowBusyStateElements` returns early on a null grid; `GetDesiredDraggerVisibility` is a switch statement that returns from each arm.
- Remaining mismatches (kept as generated stubs, not landed): `UpdateBackgroundAndForeground` — branch structure matches but a then-block is two bytes shorter than Mathcad's; the early-return shape and the `Color ==` operand order did not close it. `UpdateBorderVisibilityState` — Mathcad emits `brfalse` for `ShowBusy ? visibility : Visibility.Visible`, Roslyn emits `brtrue` with the constant on the fall-through.

## Batch 110 — OleContainer static members

- Claimed: `OleContainer<TViewModel,TModel,TIPublicModel,TPersistentData>` `_typeOfThis`, `OleForceClosePriority`, `ContentChangedEvent`, `IsInPrintingProperty`, `IsInCopyProperty`, the static constructor, and the two static change handlers. New hand-written partial `src/Quire.Controls.Core/Ole/OleContainer.cs`; stubs removed from the `.g.cs`, which keeps its class header and instance members.
- Report: `.mirror-cache/release-il-3ef9c3974b8d/release-il.json` — PASS, 2,113 checked, 2,113 exact, 0 failures.
- Remaining mismatches: none in claimed batch.

## Batch 111 — OleContainer property members

- Claimed: `ViewModel`, explicit `IOleContainer.ViewModel`, `PublicModel`, `OleForceCloseOperation`, `MinSizeForXamlSerialization`, `IsOleCreated`, `IsZoomAware`, `IsInPrinting`, `IsInCopy`, `SizeDataForXamlSerialization`, `CanPrint`, `ImageFileNameForPrintXamlSerialization`, `IsOleOpenForXamlSerializationInPrinting`, `MessageListForPrint`, `SiteCreated`, `IsOutOfPlaceActive`, `IsStorageEmpty`, `SnappingHelper`, `Zoom`, `IsPrintingZoomSet`, `HasEffectivePrintingZoom`. Stubs removed from the `.g.cs`.
- Report: `.mirror-cache/release-il-d64976e1ef4f/release-il.json` — PASS, 2,141 checked, 2,141 exact, 0 failures.
- Shapes Release IL requires: `MessageListForPrint` setter tests `value != null` around the loop.
- Not in this batch: `StorageFileNameForXamlSerialization` (a lambda closure), the constructor, and the instance methods.

## Batch 112 — OleContainer lifecycle, persistence and printing-zoom members

- Claimed: `OnZoomChanged`, `OnIsInPrintingPropertyChanged`, `OnIsInCopyPropertyChanged`, `ClearStorageOnPropertyChanged`, `HandleBeforeActiveStateToggled`, `ShouldSerializeProperty`, `GetDimensionsConverter`, `OnViewModelPropertyChanged`, `OnViewChangedSync`, `OnViewChangedOrSavedAsync`, `AbortOleCloseOperation`, `OleForceClose`, `OnCreationError` (both overloads), `CreateWindowlessOle`, `ThrowExceptionWithMessage`, `CleanStorage`, `SetOlePersistentData`, `OnWorksheetPropertiesChanged`, `Dispose`, `SetPrintingZoom`, `ApplyPrintingZoom`. Stubs removed from the `.g.cs`.
- Report: `.mirror-cache/release-il-acd1922e0dc5/release-il.json` — PASS, 2,163 checked, 2,163 exact, 0 failures.
- Remaining mismatches: none in claimed batch. Still open in OleContainer: `StartOleCloseOperation` (anonymous delegates), `StorageFileNameForXamlSerialization`, `GetOlePersistentData`, `TrySavingBitmapForPrint` (lambdas), the rendering and bitmap members, `GetRenderRectInPixels`, `CreateHBitmap`, `CreateBitmapSource`, `OnRender`, `AcquireBitmapSource`, and the constructor.

## Batch 113 — OleContainer constructor, rendering and bitmap members

- Claimed: the constructor, `OnRender`, `CreateBitmap`, the three `GetRenderRectInPixels` overloads, `CreateHBitmap`, `CreateBitmapSource`. Stubs removed from the `.g.cs`.
- Report: `.mirror-cache/release-il-65a78fbc0c74/release-il.json` — PASS, 2,171 checked, 2,171 exact, 0 failures.
- Shapes Release IL requires: `OnRender` declares `scale` before the render rectangle and builds `Rect` in an if/else with the `DevAssert` test in the then-position; the `Rect` is assigned from `new` in each branch.
- Remaining mismatch (kept as generated stub, not landed): `AcquireBitmapSource` — Mathcad reads `RenderSize` inside the `Int32Rect` arguments; the assignment-expression form is still two bytes off in the then-block.
- Still open in OleContainer: `AcquireBitmapSource`, `StorageFileNameForXamlSerialization`, `StartOleCloseOperation`, `GetOlePersistentData`, `TrySavingBitmapForPrint` (lambdas and anonymous delegates).

## Batch 114 — WorksheetPageLayout static metric and US size members

- Claimed: `MetricGridCellSize`, `MetricFineGridCellSize`, `MetricHeaderFooterGridCellSize`, `USGridCellSize`, `USFineGridCellSize`, `USHeaderFooterGridCellSize`, `StandardMetricMargin`, `StandardUSMargin`, `LegacyMargin`, `USHeaderMargin`, `MetricHeaderMargin`, `MetricInitialInsertionPoint`, `USInitialInsertionPoint`. Stubs removed from the `.g.cs`.
- Report: `.mirror-cache/release-il-4bd492ba2a6b/release-il.json` — PASS, 2,184 checked, 2,184 exact, 0 failures.
- Shapes Release IL requires: the initial insertion points build `Point` with `0.0` pushed before the grid-cell `Size` is read.
- Remaining in WorksheetPageLayout: the instance properties, constructor (`WorksheetPageLayout() { }` stub), `Clone`, the setters and `PageSize` machinery, and the two closures (`UpdatePropertyAndDelayNotification` callers and the event initializers).

## Batch 115 — WorksheetPageLayout simple property setters

- Claimed: `PageBodyContentWidth`, `PageBodyWidth`, `PageBackgroundColor`, `MajorGridLines`, `HeaderBodyWidth`, `HeaderBodyHeight`, `FooterBodyWidth`, `FooterBodyHeight`, `PageSizeUnits`, `InitialInsertionPoint`, `GridSize`. Property stubs removed from the `.g.cs`; the nested `PropertyNames` constants stay.
- Report: `.mirror-cache/release-il-8bc9c092eb0e/release-il.json` — PASS, 2,205 checked, 2,205 exact, 0 failures.
- Shapes Release IL requires: the compare-and-notify setters are `if (field == value) return;` followed by the assignment and `OnPropertyChanged`, not a `!=` block.
- Still open in WorksheetPageLayout: the cascading setters (`PageOrientation`, `PageHeight`, `PageWidth`, `PageMargin`, `HeaderMargin`, `FooterMargin`, `HeaderWidth`, `HeaderHeight`, `FooterWidth`, `FooterHeight`, `DisplayHeight`, `MarginType`, `PageSize`, `GridCellSize`, `HeaderFooterGridCellSize`), `LinesPerPage`, the constructor, `Clone`, the page-size and margin methods, and the closures.

## Batch 116 — WorksheetPageLayout constructor, events and background-colour field

- Claimed: the constructor, `_pageBackgroundColor` initializer (`BackgroundTypeColors.Paper`), and the four event declarations with `= delegate { }` initializers. Stubs removed from the `.g.cs`.
- Report: `.mirror-cache/release-il-97819d0a4b83/release-il.json` — PASS, 2,210 checked, 2,210 exact, 0 failures.
- Ordinal calibration (WorksheetPageLayout, checked against the gate): a closure in a member takes the count of hand-written members declared before it, where a method counts 1, a field counts 1, a getter-only property counts 2, a get/set property counts 3, and a field-like event counts 3. Mathcad's constructor closure is numbered 29; the hand-written order was arranged to reach 29 (field, four events, `PageBodyContentWidth`-free prefix, then the constructor).
- Still open: the cascading WorksheetPageLayout setters and methods, AttachedProperties, OleContainerModel_1 and OleContainerViewModel_3.
