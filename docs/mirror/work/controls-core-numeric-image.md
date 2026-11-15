# Controls Core, Numeric and Image

- Owner: sub-lane "Controls Core, Numeric and Image" (task_31558eb4d965); branch `mirror/controls-core-numeric-image-20261114`.
- Types: `WorksheetPageLayout`, `Serialization.NumberFormatSerializer`, `NumberFormat`, `ImageControl`, `NumericUpDown`, `IORegions.IOItemsAliasManager`, `Serialization.McdxDeserializer`, `CommonDragger`.

## Batch 1 — IOItemsAliasManager

- Landed members: ctor, `GetItem`, `Add`, `Remove`, `IsExist`, `SetInputAliases`, `SetOutputAliases`, `SetIOAliasMap`, `GetIOAliasMap`, `ClearIOAliasMap`, `GetAliases`, `SetAliases`, `FindSingleCaseInsensitiveAliasMatch`, `HandleInAliassesPropertyChanged`, `HandleOutAliassesPropertyChanged`, fields/events.
- Parked (stubs): `DistinctIds` (closure ordinal `b__10_*` needs one fewer member ahead of it than the generated stub leaves; the generated partial compiles first), `GetSExpressionsList` (Release IL returns the new empty list through a temporary `IList<string>` local; direct return and declared local both compile to `newobj; ret`).
- Report: full `--project Quire.Controls.Core` gate PASS, 2477 checked, 0 failures.
- Stub count left in this type: 2.

## Batch 2 — NumericUpDown

- Landed members: `NumberValue` (get/set), `IsInputValid` (get/set), `DefaultValidationMessage`, `ValidationMessage` (get/set), ctor, `OnInitialized`, `ValueTextBox_TextChanged`, `ValueTextBox_KeyUp`, `IncrementValue`, `DecrementValue`, `OnNumberChanged`, `ShowValue`, `IsTextBoxEmpty`, `InitializeComponent`, `IComponentConnector.Connect`, fields. `Minimum`/`Maximum` now carry Mathcad's `int.MinValue`/`int.MaxValue` initializers.
- Report: `--type` gate PASS (first form); full `--project Quire.Controls.Core` gate PASS, 2496 checked, 0 failures.
- Note: the `.xaml` resource is not in the repo for this control; the gate compares IL only.
- Stub count left in this type: 0.

## Batch 3 — CommonDragger and Serialization.McdxDeserializer

- Landed members:
  - `CommonDragger`: all 11 fields (`_elementsToDrag`, `_mode`, `_container`, `_currentPoint`, `_started`, `_preserveDimensions`, `onDragStart`, `onDragEnd`, `onDragProgress`, `bringIntoViewDispatcherTimer`, `_pt`).
  - `Serialization.McdxDeserializer`: all 16 stubs (`_deserializationStrategy`, `_regionCollectionSerializer`, `_worksheetNodeName`, ctor, `Deserialize(Stream)`, `RepairXmlDocument`, `RepairNumberFormatPrecision`, `RepairPictureElements`, `RepairPictureElementIfNeed`, `ThrowIncorrectSerializationFormatException`, `Deserialize(XmlDocument)`, `UnpackExternalObject`, `UnpackProtectedPart`, `UnpackFlowDocument`, `ThrowFirstRegionDeserializationExceptionIfExists`, `Dispose`).
- Report: `--type` gate PASS; full `--project Quire.Controls.Core` gate PASS, 3100 checked, 0 failures.
- Stub count left in these types: 0 in CommonDragger, 0 in Serialization.McdxDeserializer.

## Batch 4 — ImageControl

- Landed members: all 51 members: ctor, .cctor, `_typeOfThis`, `_dictionary`, `_originalImageUri`, `SourceProperty`, `ChangedEvent`, `BrowseForImageCommand`, `_boundTo`, `OriginalStreamHash` (get/set), `Source` (get/set), `OriginalImageUri` (get/set), `BitmapImage`, `BoundTo` (get/set), `VerticalAlignmentOffset`, `IsEmpty`, `PreserveAspectRatio`, `SubElementToResizeData`, `ResizeState`, `PropertyChanged`, `OnGotFocusEvent`, `RequestRemove`, `RequestResizeToFitOrClearRestriction`, `HandleSourcePropertyChanged`, `FireRequestResizeToFit`, `FireChanged`, `CreateImageControl`, `GetBitmapSourceFromUri`, `GetImageExtension(Uri)`, `GetImageExtension()`, `ShouldSerializeSource`, `SourceIsBitmapImage`, `OnMouseLeftButtonDown`, `OnGotFocus`, `BrowseForImage(object, ExecutedRoutedEventArgs)`, `BrowseForImage()`, `Activate`, `NavigateIntoItem`, `SimulateMouseClick`, `Clone`, `SetRegionPersistentData`, `GetImageRelationshipId`, `GetRegionPersistentData`, `GetHelpContext`, and compiler-generated closures `<.ctor>b__14_0`, `<>c__DisplayClass22_0`, `<Activate>b__50_0`.
- Report: full `--project Quire.Controls.Core` gate PASS, 3174 checked, 0 failures.
- Stub count left in this type: 0.
