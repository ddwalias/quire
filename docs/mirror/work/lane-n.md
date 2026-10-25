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
