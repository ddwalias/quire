# Lane controls-core — classification of Quire.Controls.Core stubs

- Owner: dispatched worker, Orca task `task_113b76a9ec27`; branch `feature/controls-core-claude` (worktree `C:/Users/DuyDang/orca/workspaces/Quire/controls-core-claude`), based on `main`.
- Scope: every `[MirrorStub]` member in `src/Quire.Controls.Core/Generated/Mirror/*.g.cs` (1,108 members in 39 files). Step 1 classification only.
- Reference IL: `tools/reference/reference.py <Ptc.Type> --assembly Ptc.Controls.Core --il` (`--assembly` takes the name without `.dll`; with the suffix the tool looks for `….dll.dll`). Cached under the ignored `.mirror-cache/Ptc.Controls.Core/`.
- Method: each stub is matched by name to its Mathcad member in the reference IL (`get_`/`set_` for properties, `.ctor`, operator names, explicit-interface names, generic arity removed). The member body is scanned for: closure/lambda markers (`<>c`, `<>9__`, `DisplayClass`, `b__`/`g__`), XAML (`InitializeComponent`, `LoadComponent`, BAML), and native/COM (P/Invoke, `Marshal`, `InteropServices`, `calli`). Quire-side declarations are checked for `global::Quire.*` type references; all resolve to existing source types, so no member is blocked on a missing type.
- Status: `unblocked` = no closure, XAML or native marker and not in a touched group (IL shape not yet tested; the gate decides). `blocked` = a reason above, or a checkpoint record. `deferred` = in a type group that lane N already claimed or landed members in (`docs/mirror/work/lane-n.md`); this lane is limited to untouched groups, so these stay for the maintainer.
- Checkpoint records skipped: lane-n batches 87, 101, 109, 112, 117, 118, 119 (see the reason column).
- Type-map ownership: `docs/mirror/type-map.csv` maps `Ptc.Controls.Core.dll` types; all 39 stubbed Mathcad types have a row in that map (checked).
- Static constructors with lambdas (`CalculationOptions`, `CustomScriptRunBase`): their static members are ordinal-sensitive; they are marked unblocked and the gate decides.

**Totals:** 1108 stubs — unblocked 901, blocked 90, deferred (touched groups) 117.

## Summary by type

| Generated file (Controls.Core.*.g.cs) | Stubs | Unblocked | Blocked | Deferred | Blocked reasons |
|---|---:|---:|---:|---:|---|
| `Controls.CalculationOptions` | 22 | 12 | 10 | 0 | closure/lambda 10 |
| `Controls.CalculationVariables` | 30 | 30 | 0 | 0 | - |
| `AlignmentGrid` | 24 | 24 | 0 | 0 | - |
| `AttachedProperties` | 6 | 0 | 0 | 6 | - |
| `CommonDragger` | 29 | 29 | 0 | 0 | - |
| `DummyWorksheetPropertiesSource` | 23 | 23 | 0 | 0 | - |
| `Excel.ExcelReader` | 2 | 0 | 0 | 2 | - |
| `ExtendedFont` | 38 | 38 | 0 | 0 | - |
| `FrameworkElementFacade` | 22 | 21 | 1 | 0 | closure/lambda 1 |
| `IEquationControl` | 1 | 0 | 0 | 1 | - |
| `IORegions.IOItemsAliasManager` | 18 | 16 | 2 | 0 | closure/lambda 2 |
| `ImageControl` | 30 | 27 | 3 | 0 | closure/lambda 3 |
| `MC14Conversion.Mathcad14TextStyle` | 37 | 24 | 13 | 0 | closure/lambda 13 |
| `MathFormat` | 36 | 28 | 8 | 0 | closure/lambda 8 |
| `MathFormatAttachedProperties` | 27 | 27 | 0 | 0 | - |
| `MatrixFormat` | 27 | 24 | 3 | 0 | closure/lambda 3 |
| `NodeMessageAdorner` | 18 | 17 | 1 | 0 | closure/lambda 1 |
| `NumberFormat` | 61 | 61 | 0 | 0 | - |
| `NumericUpDown` | 23 | 21 | 2 | 0 | XAML 2 |
| `Ole.IPublicOleContainerModel_1` | 10 | 10 | 0 | 0 | - |
| `Ole.OleContainerModel_1` | 98 | 90 | 8 | 0 | closure/lambda 7, COM/native 1 |
| `Ole.OleContainerViewModel_3` | 87 | 83 | 4 | 0 | closure/lambda 4 |
| `Ole.OleContainer_4` | 6 | 0 | 0 | 6 | - |
| `Protection.ProtectDialog` | 15 | 0 | 0 | 15 | - |
| `RegionBorder` | 19 | 0 | 0 | 19 | - |
| `RtfDocConstructor` | 12 | 11 | 1 | 0 | closure/lambda 1 |
| `RtfHelper` | 1 | 0 | 0 | 1 | - |
| `Serialization.McdxDeserializer` | 16 | 16 | 0 | 0 | - |
| `Serialization.McdxSerializer` | 17 | 11 | 6 | 0 | closure/lambda 6 |
| `Serialization.NumberFormatSerializer` | 37 | 37 | 0 | 0 | - |
| `Serialization.ResultSerializer` | 43 | 41 | 2 | 0 | closure/lambda 2 |
| `Serialization.SerializationHelper` | 25 | 24 | 1 | 0 | closure/lambda 1 |
| `TextStylesCollectionManager` | 25 | 17 | 8 | 0 | closure/lambda 8 |
| `UIElementFacade` | 16 | 16 | 0 | 0 | - |
| `VisualDragger` | 36 | 36 | 0 | 0 | - |
| `Whiteboard.ContainerItemManager` | 61 | 54 | 7 | 0 | closure/lambda 7 |
| `WorksheetPageLayout` | 67 | 0 | 0 | 67 | - |
| `WorksheetPropertiesManager` | 22 | 22 | 0 | 0 | - |
| `Controls.Text.CustomScriptRunBase` | 21 | 11 | 10 | 0 | closure/lambda 10 |

## Blocked members, grouped by reason

### closure/lambda ordinal (needs Mathcad member order; see batch notes) — 87

| File | Member | Reason |
|---|---|---|
| `Controls.CalculationOptions` | `CalculationOptions.TypeOfThis` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.CalculationOptions` | `CalculationOptions.StrictComparisionsProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.CalculationOptions` | `CalculationOptions.StringsIndexedFromOriginProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.CalculationOptions` | `CalculationOptions.StrictSingularityCheckingProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.CalculationOptions` | `CalculationOptions.UnitsAndConstantsInSymbolicsProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.CalculationOptions` | `CalculationOptions.IsAutoCalcOnProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.CalculationOptions` | `CalculationOptions.NumberOfWorkerThreadsProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.CalculationOptions` | `CalculationOptions.SymbolicResultMaximumSizeProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.CalculationOptions` | `CalculationOptions.BinOctHexModeProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.CalculationOptions` | `CalculationOptions._restrictedMode` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `FrameworkElementFacade` | `FrameworkElementFacade.ExecuteOnUnload` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `IORegions.IOItemsAliasManager` | `IOItemsAliasManager.DistinctIds` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `IORegions.IOItemsAliasManager` | `IOItemsAliasManager.FindSingleCaseInsensitiveAliasMatch` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `ImageControl` | `ImageControl..ctor` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `ImageControl` | `ImageControl.GetBitmapSourceFromUri` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `ImageControl` | `ImageControl.Activate` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.FontFamily` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.FontSize` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.FontWeight` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.FontStyle` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.TextDecorations` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.Color` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.Variants` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.Margin` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.TextIndent` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.TextAlign` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.MarkerStyle` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.ReadInlineAttr` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MC14Conversion.Mathcad14TextStyle` | `Mathcad14TextStyle.ReadBlockAttr` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MathFormat` | `MathFormat.BindToPropertyChanged` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MathFormat` | `MathFormat.GetPropertyValue` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MathFormat` | `MathFormat.SetPropertyValue` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MathFormat` | `MathFormat.IncreaseFontSize` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MathFormat` | `MathFormat.DecreaseFontSize` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MathFormat` | `MathFormat.ClearFormat` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MathFormat` | `MathFormat.DoAction` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MathFormat` | `MathFormat.IsPropertySet` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MatrixFormat` | `MatrixFormat.op_Equality` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MatrixFormat` | `MatrixFormat.Clone` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `MatrixFormat` | `MatrixFormat.Clone` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `NodeMessageAdorner` | `NodeMessageAdorner.Setup` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Ole.OleContainerModel_1` | `OleContainerModel..ctor` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Ole.OleContainerModel_1` | `OleContainerModel.Run` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Ole.OleContainerModel_1` | `OleContainerModel.UpdateLink` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Ole.OleContainerModel_1` | `OleContainerModel.Activate` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Ole.OleContainerModel_1` | `OleContainerModel.DoActivate` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Ole.OleContainerModel_1` | `OleContainerModel.Save` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Ole.OleContainerModel_1` | `OleContainerModel.ClearMessage` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Ole.OleContainerViewModel_3` | `OleContainerViewModel..ctor` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Ole.OleContainerViewModel_3` | `OleContainerViewModel.AttemptUpdatingSizeOnViewChanged` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Ole.OleContainerViewModel_3` | `OleContainerViewModel.ExecutedToggleOleAutoResize` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Ole.OleContainerViewModel_3` | `OleContainerViewModel.ExecutedOleResetSize` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `RtfDocConstructor` | `RtfDocConstructor..ctor` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Serialization.McdxSerializer` | `McdxSerializer.Serialize` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Serialization.McdxSerializer` | `McdxSerializer.PackImage` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Serialization.McdxSerializer` | `McdxSerializer.PackImage` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Serialization.McdxSerializer` | `McdxSerializer.PackIncludedWorksheet` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Serialization.McdxSerializer` | `McdxSerializer.PackProtectedPart` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Serialization.McdxSerializer` | `McdxSerializer.BuildDocumentXml` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Serialization.ResultSerializer` | `ResultSerializer.WriteEngineWarnings` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Serialization.ResultSerializer` | `ResultSerializer.ReadEngineWarnings` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Serialization.SerializationHelper` | `SerializationHelper.InitializeForWrite` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `TextStylesCollectionManager` | `TextStylesCollectionManager.ToTextStyleDictionary` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `TextStylesCollectionManager` | `TextStylesCollectionManager.LoadWorkSheetBasedOnStyles` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `TextStylesCollectionManager` | `TextStylesCollectionManager.LoadWorkSheetEmbeddedStyles` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `TextStylesCollectionManager` | `TextStylesCollectionManager.UpdateStyleToLimbo` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `TextStylesCollectionManager` | `TextStylesCollectionManager.AddStyle` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `TextStylesCollectionManager` | `TextStylesCollectionManager.GetTextStyleDataByName` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `TextStylesCollectionManager` | `TextStylesCollectionManager.GetTextStyleDataByKey` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `TextStylesCollectionManager` | `TextStylesCollectionManager.GetTextStyleDataByStyle` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Whiteboard.ContainerItemManager` | `ContainerItemManager.HookupSizeChanged` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Whiteboard.ContainerItemManager` | `ContainerItemManager.HookupSizeChanged` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Whiteboard.ContainerItemManager` | `ContainerItemManager.HookupSizeChanged` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Whiteboard.ContainerItemManager` | `ContainerItemManager.HookupMouseEnter` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Whiteboard.ContainerItemManager` | `ContainerItemManager.HookupLeftMouseButtonDown` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Whiteboard.ContainerItemManager` | `ContainerItemManager.HookupDragEnter` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Whiteboard.ContainerItemManager` | `ContainerItemManager.SetIsSelectedProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.Text.CustomScriptRunBase` | `CustomScriptRunBase.TypeOfThis` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.Text.CustomScriptRunBase` | `CustomScriptRunBase.StyleKeyProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.Text.CustomScriptRunBase` | `CustomScriptRunBase.WpfRecreateScriptsProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.Text.CustomScriptRunBase` | `CustomScriptRunBase.IsInSerializationProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.Text.CustomScriptRunBase` | `CustomScriptRunBase._deferInit` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.Text.CustomScriptRunBase` | `CustomScriptRunBase.FontSizeForReportPropertyKey` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.Text.CustomScriptRunBase` | `CustomScriptRunBase.FontSizeForReportProperty` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.Text.CustomScriptRunBase` | `CustomScriptRunBase.ToggleRangeText` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.Text.CustomScriptRunBase` | `CustomScriptRunBase.RecoverSubSuperscriptRunsInRange` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |
| `Controls.Text.CustomScriptRunBase` | `CustomScriptRunBase.RecoverCustomScriptRunsInRange` | closure/lambda ordinal (Mathcad display class or `<>c` lambda) |

### XAML / InitializeComponent — 2

| File | Member | Reason |
|---|---|---|
| `NumericUpDown` | `NumericUpDown..ctor` | XAML: InitializeComponent / XAML-generated ctor |
| `NumericUpDown` | `NumericUpDown.InitializeComponent` | XAML: InitializeComponent / XAML-generated ctor |

### COM / native — 1

| File | Member | Reason |
|---|---|---|
| `Ole.OleContainerModel_1` | `OleContainerModel.SetVisibleSize` | COM/native |

### checkpoint-recorded (lane N) — IL shape / type conflict / library — 0

None.

## Deferred (touched groups — lane N already landed or claimed members here)

These are excluded from this lane by the `untouched groups only` scope. Each row keeps its own sub-reason.

| File | Member | Status detail |
|---|---|---|
| `AttachedProperties` | `AttachedProperties.SetRegisterCommandBindings` | touched group: lane N batches 118-119 (AttachedProperties); checkpoint blocked: IL shape: callvirt vs call on DependencyObject.SetValue (batch 119) |
| `AttachedProperties` | `AttachedProperties.GetRegisterCommandBindings` | touched group: lane N batches 118-119 (AttachedProperties); checkpoint blocked: IL shape: callvirt vs call on DependencyObject.GetValue (batch 119) |
| `AttachedProperties` | `AttachedProperties.GetPrintView` | touched group: lane N batches 118-119 (AttachedProperties); checkpoint blocked: IL shape: cast sequence differs (batch 119) |
| `AttachedProperties` | `AttachedProperties.OnCanActivateChanged` | touched group: lane N batches 118-119 (AttachedProperties); checkpoint blocked: closure: lambda, display class 32 (batch 118) |
| `AttachedProperties` | `AttachedProperties.SetIsBringToView` | touched group: lane N batches 118-119 (AttachedProperties); checkpoint blocked: IL shape: callvirt vs call on DependencyObject.SetValue (batch 119) |
| `AttachedProperties` | `AttachedProperties.GetIsBringToView` | touched group: lane N batches 118-119 (AttachedProperties); checkpoint blocked: IL shape: callvirt vs call on DependencyObject.GetValue (batch 119) |
| `Excel.ExcelReader` | `ExcelReader.GetSharedString` | touched group: lane N batches 59, 101 (ExcelReader); checkpoint blocked: library: OpenXml 3.x dropped the public package API (batch 101) |
| `Excel.ExcelReader` | `ExcelReader.LoadExcelBook` | touched group: lane N batches 59, 101 (ExcelReader); checkpoint blocked: library: OpenXml 3.x dropped the public package API (batch 101) |
| `IEquationControl` | `IEquationControl.Changed` | touched group: lane N batch 87 (IEquationControl); checkpoint blocked: type conflict: RoutedEventHandler vs Quire EventHandler (batch 87) |
| `Ole.OleContainer_4` | `OleContainer._snappableHelper` | touched group: lane N batches 110-113, 119 (OleContainer) |
| `Ole.OleContainer_4` | `OleContainer._prePrintingZoom` | touched group: lane N batches 110-113, 119 (OleContainer) |
| `Ole.OleContainer_4` | `OleContainer.StorageFileNameForXamlSerialization` | touched group: lane N batches 110-113, 119 (OleContainer); checkpoint blocked: closure (batch 112) |
| `Ole.OleContainer_4` | `OleContainer.StartOleCloseOperation` | touched group: lane N batches 110-113, 119 (OleContainer); checkpoint blocked: closure: anonymous delegates (batch 112) |
| `Ole.OleContainer_4` | `OleContainer.TrySavingBitmapForPrint` | touched group: lane N batches 110-113, 119 (OleContainer); checkpoint blocked: closure: lambda (batch 112) |
| `Ole.OleContainer_4` | `OleContainer.GetOlePersistentData` | touched group: lane N batches 110-113, 119 (OleContainer); checkpoint blocked: closure: lambda (batch 112) |
| `Protection.ProtectDialog` | `ProtectDialog._displayState` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog._areaBorderState` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog._primaryPasswordBox` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog._reenterPasswordBox` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog.showTimeStampCheckBox` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog.showAreaBorderCheckBox` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog.lockDisplayStateCheckBox` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog.displayStateComboBox` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog._contentLoaded` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog.ShowProtectionDialog` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog..ctor` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog.OnControlDisplayStateChanged` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog.OnChangeBorderVisibilityState` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog.InitializeComponent` | touched group: lane N batch 57 (ProtectDialog) |
| `Protection.ProtectDialog` | `ProtectDialog.Connect` | touched group: lane N batch 57 (ProtectDialog) |
| `RegionBorder` | `RegionBorder._borderGrid` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._border` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._draggers` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._busyClock` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._marchingAnts` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._background` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._foreground` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._header` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._regionBorderBrushChanged` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._rightEdgeVisibilityChanged` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._bottomEdgeVisibilityChanged` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._cornerRadiusChanged` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder._areDraggersShown` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder.UpdateBackgroundAndForeground` | touched group: lane N batches 106-109 (RegionBorder); checkpoint blocked: IL shape: then-block two bytes short (batch 109) |
| `RegionBorder` | `RegionBorder.SetBorderLook` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder.UpdateBorderVisibilityState` | touched group: lane N batches 106-109 (RegionBorder); checkpoint blocked: IL shape: brfalse vs brtrue for ternary (batch 109) |
| `RegionBorder` | `RegionBorder.ShowDraggers` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder.HideDraggers` | touched group: lane N batches 106-109 (RegionBorder) |
| `RegionBorder` | `RegionBorder.UpdateDraggersLook` | touched group: lane N batches 106-109 (RegionBorder) |
| `RtfHelper` | `RtfHelper.FixHyperlinksRtfSerialization` | touched group: lane N batch 94 (RtfHelper) |
| `WorksheetPageLayout` | `WorksheetPageLayout._pageOrientation` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._pageMargin` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._headerMargin` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._footerMargin` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._marginType` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._gridCellSize` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._headerFooterGridCellSize` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._pageSize` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._pageSizeUnits` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._pageWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._pageHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._pageBodyWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._pageBodyHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._majorGridLines` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._displayHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._gridSize` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._pageBodyContentWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._headerWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._headerHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._headerBodyWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._headerBodyHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._footerWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._footerHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._footerBodyWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout._footerBodyHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.DraftModeGutterWidthInDips` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.BuiltInPageSizes` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.MarginEpsilon` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.UpdatePropertyAndDelayNotification` | touched group: lane N batches 114-117 (WorksheetPageLayout); checkpoint blocked: closure: local function ordinal 122 (batch 117) |
| `WorksheetPageLayout` | `WorksheetPageLayout.SetPageSize` | touched group: lane N batches 114-117 (WorksheetPageLayout); checkpoint blocked: closure: display class 124 (batch 117) |
| `WorksheetPageLayout` | `WorksheetPageLayout.SetPageOrientation` | touched group: lane N batches 114-117 (WorksheetPageLayout); checkpoint blocked: closure: display class 125 (batch 117) |
| `WorksheetPageLayout` | `WorksheetPageLayout.SetupDefaultPageSizeInformation` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.SetupMarginMap` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.ScaledMargin` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.ScaledSize` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.ContentAreaSize` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.ContentArea` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.GetMarginTypeOfMarginByPageSize` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.GetMaxPageMarginsRestriction` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.GetMinPageMarginsRestriction` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `WorksheetPageLayout.CoercePageMargin` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.DisplayHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.HeaderWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.HeaderHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.HeaderBodyWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.HeaderBodyHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.HeaderMargin` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.FooterWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.FooterHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.FooterBodyWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.FooterBodyHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.FooterMargin` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.PageBodyWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.PageBodyHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.PageBodyContentWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.PageWidth` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.PageHeight` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.PageBackgroundColor` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.PageOrientation` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.GridSize` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.PageSize` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.PageSizeUnits` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.MarginType` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.HeaderFooterGridCellSize` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.MajorGridLines` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.PageMargin` | touched group: lane N batches 114-117 (WorksheetPageLayout) |
| `WorksheetPageLayout` | `PropertyNames.GridCellSize` | touched group: lane N batches 114-117 (WorksheetPageLayout) |

## Unblocked members (untouched groups), by file

### `Controls.CalculationOptions` — 12

| Type | Member | Declaration |
|---|---|---|
| `CalculationOptions` | `StrictComparisions` | `public bool StrictComparisions` |
| `CalculationOptions` | `StringsIndexedFromOrigin` | `public bool StringsIndexedFromOrigin` |
| `CalculationOptions` | `StrictSingularityChecking` | `public bool StrictSingularityChecking` |
| `CalculationOptions` | `UnitsAndConstantsInSymbolics` | `public bool UnitsAndConstantsInSymbolics` |
| `CalculationOptions` | `IsAutoCalcOn` | `public bool IsAutoCalcOn` |
| `CalculationOptions` | `NumberOfWorkerThreads` | `public int NumberOfWorkerThreads` |
| `CalculationOptions` | `SymbolicResultMaximumSize` | `public int SymbolicResultMaximumSize` |
| `CalculationOptions` | `BinOctHexMode` | `public bool BinOctHexMode` |
| `CalculationOptions` | `SymbolicEngineType` | `public global::Quire.PersistentDataObjects.SymbolicEngineType SymbolicEngineType` |
| `CalculationOptions` | `RestrictedMode` | `public bool RestrictedMode` |
| `CalculationOptions` | `HandleOptionChanged` | `private static void HandleOptionChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs` |
| `CalculationOptions` | `HandleBinOctHexModeOptionChanged` | `private static void HandleBinOctHexModeOptionChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyCha` |

### `Controls.CalculationVariables` — 30

| Type | Member | Declaration |
|---|---|---|
| `CalculationVariables` | `TolVarName` | `public const string TolVarName = "mc_TOL";` |
| `CalculationVariables` | `CTolVarName` | `public const string CTolVarName = "mc_CTOL";` |
| `CalculationVariables` | `IndexOriginVarName` | `public const string IndexOriginVarName = "mc_ORIGIN";` |
| `CalculationVariables` | `PRNColWidthVarName` | `public const string PRNColWidthVarName = "mc_PRNCOLWIDTH";` |
| `CalculationVariables` | `PRNPrecisionVarName` | `public const string PRNPrecisionVarName = "mc_PRNPRECISION";` |
| `CalculationVariables` | `CWDVarName` | `public const string CWDVarName = "mc_CWD";` |
| `CalculationVariables` | `RandSeedVarName` | `public const string RandSeedVarName = "rand_seed";` |
| `CalculationVariables` | `TolProperty` | `public static readonly global::System.Windows.DependencyProperty TolProperty;` |
| `CalculationVariables` | `CTolProperty` | `public static readonly global::System.Windows.DependencyProperty CTolProperty;` |
| `CalculationVariables` | `IndexOriginProperty` | `public static readonly global::System.Windows.DependencyProperty IndexOriginProperty;` |
| `CalculationVariables` | `PRNColWidthProperty` | `public static readonly global::System.Windows.DependencyProperty PRNColWidthProperty;` |
| `CalculationVariables` | `PRNPrecisionProperty` | `public static readonly global::System.Windows.DependencyProperty PRNPrecisionProperty;` |
| `CalculationVariables` | `CurrentWorkingDirectoryProperty` | `public static readonly global::System.Windows.DependencyProperty CurrentWorkingDirectoryProperty;` |
| `CalculationVariables` | `RandSeedProperty` | `public static readonly global::System.Windows.DependencyProperty RandSeedProperty;` |
| `CalculationVariables` | `Tol` | `public double Tol` |
| `CalculationVariables` | `CTol` | `public double CTol` |
| `CalculationVariables` | `IndexOrigin` | `public int IndexOrigin` |
| `CalculationVariables` | `PRNColWidth` | `public int PRNColWidth` |
| `CalculationVariables` | `PRNPrecision` | `public int PRNPrecision` |
| `CalculationVariables` | `CurrentWorkingDirectory` | `public string CurrentWorkingDirectory` |
| `CalculationVariables` | `RandSeed` | `public double RandSeed` |
| `CalculationVariables` | `.ctor` | `public CalculationVariables()` |
| `CalculationVariables` | `HandleTolChanged` | `private static void HandleTolChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)` |
| `CalculationVariables` | `HandleCTolChanged` | `private static void HandleCTolChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e` |
| `CalculationVariables` | `HandleIndexOriginChanged` | `private static void HandleIndexOriginChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEven` |
| `CalculationVariables` | `HandlePRNColWidthChanged` | `private static void HandlePRNColWidthChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEven` |
| `CalculationVariables` | `HandlePRNPrecisionChanged` | `private static void HandlePRNPrecisionChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEve` |
| `CalculationVariables` | `HandleCurrentWorkingDirectoryChanged` | `private static void HandleCurrentWorkingDirectoryChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropert` |
| `CalculationVariables` | `HandleRandSeedChanged` | `private static void HandleRandSeedChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventAr` |
| `CalculationVariables` | `EscapeDirectory` | `public static string EscapeDirectory(string dir)` |

### `AlignmentGrid` — 24

| Type | Member | Declaration |
|---|---|---|
| `AlignmentGrid` | `TypeOfThis` | `private static readonly global::System.Type TypeOfThis;` |
| `AlignmentGrid` | `DefaultGridCellSize` | `public static readonly global::System.Windows.Size DefaultGridCellSize;` |
| `AlignmentGrid` | `GridCellSizeProperty` | `public static readonly global::System.Windows.DependencyProperty GridCellSizeProperty;` |
| `AlignmentGrid` | `GridAreaProperty` | `public static readonly global::System.Windows.DependencyProperty GridAreaProperty;` |
| `AlignmentGrid` | `ZeroThreshold` | `private const double ZeroThreshold = 1E-06d;` |
| `AlignmentGrid` | `Default` | `public static global::Quire.Controls.Core.IAlignmentGrid Default;` |
| `AlignmentGrid` | `GridArea` | `public global::System.Windows.Rect GridArea` |
| `AlignmentGrid` | `GridCellSize` | `public global::System.Windows.Size GridCellSize` |
| `AlignmentGrid` | `HighestValidY` | `protected double HighestValidY` |
| `AlignmentGrid` | `HighestValidX` | `protected double HighestValidX` |
| `AlignmentGrid` | `.ctor` | `public AlignmentGrid(global::System.Windows.Rect rect)` |
| `AlignmentGrid` | `.ctor` | `public AlignmentGrid(global::System.Nullable<global::System.Windows.Size> gridSize)` |
| `AlignmentGrid` | `GetHorizontalLines` | `public global::Quire.Pair<global::System.Windows.Point, global::System.Windows.Point>[] GetHorizontalLines()` |
| `AlignmentGrid` | `GetVerticalLines` | `public global::Quire.Pair<global::System.Windows.Point, global::System.Windows.Point>[] GetVerticalLines()` |
| `AlignmentGrid` | `ConvertDipsToGridCoordinates` | `public global::System.Windows.Point ConvertDipsToGridCoordinates(global::System.Windows.Point dipCoordinates)` |
| `AlignmentGrid` | `ConvertGridCoordinatesToDips` | `public global::System.Windows.Point ConvertGridCoordinatesToDips(global::System.Windows.Point value)` |
| `AlignmentGrid` | `GetNextIntersection` | `public virtual global::System.Windows.Point GetNextIntersection(global::System.Windows.Point startPoint, global::Quire.Direction direction)` |
| `AlignmentGrid` | `FixRoundingError` | `protected global::System.Windows.Point FixRoundingError(global::System.Windows.Point pt)` |
| `AlignmentGrid` | `GetNearestIntersectionInDirection` | `public virtual global::System.Windows.Point GetNearestIntersectionInDirection(global::System.Windows.Point startPoint, global::Quire.Directi` |
| `AlignmentGrid` | `IsModZero` | `private static bool IsModZero(double dividend, double divisor)` |
| `AlignmentGrid` | `GetNearestIntersectionPoint` | `public global::System.Windows.Point GetNearestIntersectionPoint(global::System.Windows.Point startPoint)` |
| `AlignmentGrid` | `GetNearestIntersectionPointDisregardingBounds` | `public virtual global::System.Windows.Point GetNearestIntersectionPointDisregardingBounds(global::System.Windows.Point startPoint, global::Q` |
| `AlignmentGrid` | `Clip` | `private global::System.Windows.Point Clip(global::System.Windows.Point point, params global::Quire.Direction[] directionsToIgnore)` |
| `AlignmentGrid` | `GetCandidateIntersectionPoint` | `private global::System.Windows.Point GetCandidateIntersectionPoint(global::System.Windows.Point startPoint, double cellWidth, double cellHei` |

### `CommonDragger` — 29

| Type | Member | Declaration |
|---|---|---|
| `CommonDragger` | `_elementsToDrag` | `protected readonly global::System.Windows.UIElement[] _elementsToDrag;` |
| `CommonDragger` | `_mode` | `protected readonly global::Quire.Controls.Core.DragMode _mode;` |
| `CommonDragger` | `_originalWidthCache` | `protected readonly global::System.Collections.Generic.Dictionary<global::System.Windows.UIElement, double> _originalWidthCache;` |
| `CommonDragger` | `_originalHeightCache` | `protected readonly global::System.Collections.Generic.Dictionary<global::System.Windows.UIElement, double> _originalHeightCache;` |
| `CommonDragger` | `_container` | `protected readonly global::System.Windows.FrameworkElement _container;` |
| `CommonDragger` | `_currentPoint` | `private global::System.Windows.Point _currentPoint;` |
| `CommonDragger` | `_started` | `private bool _started;` |
| `CommonDragger` | `_preserveDimensions` | `protected bool _preserveDimensions;` |
| `CommonDragger` | `onDragStart` | `public global::Quire.Controls.Core.CommonDragger.DragStartHandler onDragStart;` |
| `CommonDragger` | `onDragEnd` | `public global::Quire.Controls.Core.CommonDragger.DragEndHandler onDragEnd;` |
| `CommonDragger` | `onDragProgress` | `public global::Quire.Controls.Core.CommonDragger.DragProgressHandler onDragProgress;` |
| `CommonDragger` | `bringIntoViewDispatcherTimer` | `private global::System.Windows.Threading.DispatcherTimer bringIntoViewDispatcherTimer;` |
| `CommonDragger` | `_pt` | `private global::System.Windows.Point _pt;` |
| `CommonDragger` | `.ctor` | `public CommonDragger(global::System.Windows.FrameworkElement container, global::System.Windows.Point startPosition, bool bAutoStart, global:` |
| `CommonDragger` | `StartCaptureEvents` | `public void StartCaptureEvents(global::System.Windows.Point startPoint)` |
| `CommonDragger` | `EndCaptureEvents` | `public void EndCaptureEvents()` |
| `CommonDragger` | `MouseUp` | `private void MouseUp(object sender, global::System.Windows.Input.MouseButtonEventArgs e)` |
| `CommonDragger` | `MouseMove` | `private void MouseMove(object sender, global::System.Windows.Input.MouseEventArgs e)` |
| `CommonDragger` | `NotifyDragStart` | `private void NotifyDragStart()` |
| `CommonDragger` | `NotifyDragEnd` | `private void NotifyDragEnd(global::System.Windows.Point pt)` |
| `CommonDragger` | `NotifyDragProgress` | `private void NotifyDragProgress(global::System.Windows.Point pt)` |
| `CommonDragger` | `BringIntoViewCallback` | `private void BringIntoViewCallback(object sender, global::System.EventArgs e)` |
| `CommonDragger` | `CacheOriginalWidths` | `protected void CacheOriginalWidths()` |
| `CommonDragger` | `CacheOriginalHeights` | `protected void CacheOriginalHeights()` |
| `CommonDragger` | `MouseMove_ResizeRightMode` | `protected void MouseMove_ResizeRightMode(global::System.Windows.Vector offset, global::System.Windows.UIElement element)` |
| `CommonDragger` | `MouseMove_ResizeBottomMode` | `protected void MouseMove_ResizeBottomMode(global::System.Windows.Vector offset, global::System.Windows.UIElement element)` |
| `CommonDragger` | `ResizeLeft` | `protected double ResizeLeft(global::System.Windows.Vector offset, global::System.Windows.FrameworkElement fe)` |
| `CommonDragger` | `MouseMove_ResizeLeftMode` | `protected void MouseMove_ResizeLeftMode(global::System.Windows.Vector offset, global::System.Windows.UIElement element)` |
| `CommonDragger` | `SetMouseCursor` | `protected static void SetMouseCursor(global::Quire.Controls.Core.DragMode mode)` |

### `DummyWorksheetPropertiesSource` — 23

| Type | Member | Declaration |
|---|---|---|
| `DummyWorksheetPropertiesSource` | `_instance` | `private static global::Quire.Controls.Core.DummyWorksheetPropertiesSource _instance;` |
| `DummyWorksheetPropertiesSource` | `_worksheetPageLayoutMode` | `private static readonly global::Quire.Controls.Core.PageLayoutMode _worksheetPageLayoutMode;` |
| `DummyWorksheetPropertiesSource` | `_pageMargin` | `private static readonly global::System.Windows.Thickness _pageMargin;` |
| `DummyWorksheetPropertiesSource` | `_pageSizeValue` | `private static readonly global::System.Windows.Size _pageSizeValue;` |
| `DummyWorksheetPropertiesSource` | `_gridCellSize` | `private static readonly global::System.Windows.Size _gridCellSize;` |
| `DummyWorksheetPropertiesSource` | `_docCalculationOptions` | `private static readonly global::Quire.Controls.CalculationOptions _docCalculationOptions;` |
| `DummyWorksheetPropertiesSource` | `_docCalculationVariables` | `private static readonly global::Quire.Controls.CalculationVariables _docCalculationVariables;` |
| `DummyWorksheetPropertiesSource` | `WorksheetPageLayoutMode` | `public global::Quire.Controls.Core.PageLayoutMode WorksheetPageLayoutMode` |
| `DummyWorksheetPropertiesSource` | `PageMargin` | `public global::System.Windows.Thickness PageMargin` |
| `DummyWorksheetPropertiesSource` | `PageSizeValue` | `public global::System.Windows.Size PageSizeValue` |
| `DummyWorksheetPropertiesSource` | `GridCellSize` | `public global::System.Windows.Size GridCellSize` |
| `DummyWorksheetPropertiesSource` | `FilePath` | `public string FilePath` |
| `DummyWorksheetPropertiesSource` | `CacheWorksheet` | `public bool CacheWorksheet` |
| `DummyWorksheetPropertiesSource` | `DocCalculationOptions` | `public global::Quire.Controls.CalculationOptions DocCalculationOptions` |
| `DummyWorksheetPropertiesSource` | `DocCalculationVariables` | `public global::Quire.Controls.CalculationVariables DocCalculationVariables` |
| `DummyWorksheetPropertiesSource` | `WorksheetName` | `public string WorksheetName` |
| `DummyWorksheetPropertiesSource` | `DisplayFilePath` | `public string DisplayFilePath` |
| `DummyWorksheetPropertiesSource` | `SpellCheckEnabled` | `public bool SpellCheckEnabled` |
| `DummyWorksheetPropertiesSource` | `DefaultUnitSystem` | `public global::Quire.Calculation.Core.UnitSystems DefaultUnitSystem` |
| `DummyWorksheetPropertiesSource` | `CustomBaseUnitSystem` | `public global::Quire.Calculation.Core.UnitSystems CustomBaseUnitSystem` |
| `DummyWorksheetPropertiesSource` | `SimplifyUnitSystem` | `public bool SimplifyUnitSystem` |
| `DummyWorksheetPropertiesSource` | `HelpServerPath` | `public string HelpServerPath` |
| `DummyWorksheetPropertiesSource` | `GetInstance` | `public static global::Quire.Controls.Core.DummyWorksheetPropertiesSource GetInstance()` |

### `ExtendedFont` — 38

| Type | Member | Declaration |
|---|---|---|
| `ExtendedFont` | `_fontTypeface` | `private global::System.Windows.Media.Typeface _fontTypeface;` |
| `ExtendedFont` | `_M_Run` | `private global::System.Windows.Media.GlyphRun _M_Run;` |
| `ExtendedFont` | `_n_Run` | `private global::System.Windows.Media.GlyphRun _n_Run;` |
| `ExtendedFont` | `_j_Run` | `private global::System.Windows.Media.GlyphRun _j_Run;` |
| `ExtendedFont` | `_glyphTypeface` | `private global::System.Windows.Media.GlyphTypeface _glyphTypeface;` |
| `ExtendedFont` | `_baselineAdjustment` | `private double _baselineAdjustment;` |
| `ExtendedFont` | `FontFamilyProperty` | `public static readonly global::System.Windows.DependencyProperty FontFamilyProperty;` |
| `ExtendedFont` | `FontSizeProperty` | `public static readonly global::System.Windows.DependencyProperty FontSizeProperty;` |
| `ExtendedFont` | `FontWeightProperty` | `public static readonly global::System.Windows.DependencyProperty FontWeightProperty;` |
| `ExtendedFont` | `FontStyleProperty` | `public static readonly global::System.Windows.DependencyProperty FontStyleProperty;` |
| `ExtendedFont` | `TextDecorationsProperty` | `public static readonly global::System.Windows.DependencyProperty TextDecorationsProperty;` |
| `ExtendedFont` | `ForegroundProperty` | `public static readonly global::System.Windows.DependencyProperty ForegroundProperty;` |
| `ExtendedFont` | `FontStretchProperty` | `public static readonly global::System.Windows.DependencyProperty FontStretchProperty;` |
| `ExtendedFont` | `GlyphRunCache` | `private static readonly global::System.Collections.Generic.IDictionary<string, global::System.Windows.Media.GlyphRun> GlyphRunCache;` |
| `ExtendedFont` | `FontFamily` | `public global::System.Windows.Media.FontFamily FontFamily` |
| `ExtendedFont` | `FontSize` | `public double FontSize` |
| `ExtendedFont` | `FontWeight` | `public global::System.Windows.FontWeight FontWeight` |
| `ExtendedFont` | `FontStyle` | `public global::System.Windows.FontStyle FontStyle` |
| `ExtendedFont` | `TextDecorations` | `public global::System.Windows.TextDecorationCollection TextDecorations` |
| `ExtendedFont` | `Foreground` | `public global::System.Windows.Media.Brush Foreground` |
| `ExtendedFont` | `FontStretch` | `public global::System.Windows.FontStretch FontStretch` |
| `ExtendedFont` | `FontTypeface` | `public global::System.Windows.Media.Typeface FontTypeface` |
| `ExtendedFont` | `n_Run` | `public global::System.Windows.Media.GlyphRun n_Run` |
| `ExtendedFont` | `j_Run` | `public global::System.Windows.Media.GlyphRun j_Run` |
| `ExtendedFont` | `GlyphTypeface` | `public global::System.Windows.Media.GlyphTypeface GlyphTypeface` |
| `ExtendedFont` | `BaselineAdjustment` | `public double BaselineAdjustment` |
| `ExtendedFont` | `CoerceFontFamily` | `private static object CoerceFontFamily(global::System.Windows.DependencyObject d, object basevalue)` |
| `ExtendedFont` | `SafeComputeInkBoundingBox` | `public static global::System.Windows.Rect SafeComputeInkBoundingBox(global::System.Windows.Media.GlyphRun glyphRun)` |
| `ExtendedFont` | `.ctor` | `public ExtendedFont(global::System.Windows.Media.FontFamily fontFamily, double fontSize, global::System.Windows.FontStyle fontStyle, global:` |
| `ExtendedFont` | `.ctor` | `public ExtendedFont(global::System.Windows.Media.Typeface typeface, double fontSize) : this()` |
| `ExtendedFont` | `GenerateGlyphRun` | `public static global::System.Windows.Media.GlyphRun GenerateGlyphRun(string text, global::System.Uri fontUri, double fontSize)` |
| `ExtendedFont` | `CreateGlyphRun` | `private static global::System.Windows.Media.GlyphRun CreateGlyphRun(string text, global::System.Uri fontUri, double fontSize)` |
| `ExtendedFont` | `GenerateGlyphRun` | `private global::System.Windows.Media.GlyphRun GenerateGlyphRun(string text)` |
| `ExtendedFont` | `CalculateTypeface` | `private void CalculateTypeface()` |
| `ExtendedFont` | `HandleExtendedFontPropertyChanged` | `private static void HandleExtendedFontPropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyCh` |
| `ExtendedFont` | `Clone` | `public global::Quire.Controls.Core.ExtendedFont Clone()` |
| `ExtendedFont` | `Clone` | `object global::System.ICloneable.Clone()` |
| `ExtendedFont` | `CopyTo` | `public void CopyTo(global::Quire.Controls.Core.ExtendedFont target)` |

### `FrameworkElementFacade` — 21

| Type | Member | Declaration |
|---|---|---|
| `FrameworkElementFacade` | `IsItemLoaded` | `public bool IsItemLoaded(object obj)` |
| `FrameworkElementFacade` | `SetSize` | `public void SetSize(object obj, global::System.Windows.Size size, global::Quire.Controls.Core.DragMode dragMode)` |
| `FrameworkElementFacade` | `GetResizableElement` | `private global::System.Windows.FrameworkElement GetResizableElement(object obj, global::Quire.Controls.Core.DragMode dragMode = (global::Qui` |
| `FrameworkElementFacade` | `GetSize` | `public global::System.Windows.Size GetSize(object obj)` |
| `FrameworkElementFacade` | `SetHeight` | `public void SetHeight(object obj, double height)` |
| `FrameworkElementFacade` | `SetWidth` | `public void SetWidth(object obj, double width)` |
| `FrameworkElementFacade` | `SetMaxSize` | `public void SetMaxSize(object obj, global::System.Windows.Size maxSize)` |
| `FrameworkElementFacade` | `ClearMaxSize` | `public void ClearMaxSize(object obj)` |
| `FrameworkElementFacade` | `SetItemName` | `public void SetItemName(object item, string name)` |
| `FrameworkElementFacade` | `GetItemName` | `public string GetItemName(object item)` |
| `FrameworkElementFacade` | `ExecuteOnLoad` | `public void ExecuteOnLoad(object obj, global::System.Action action)` |
| `FrameworkElementFacade` | `ExecuteInBackground` | `public void ExecuteInBackground(global::System.Action action, bool asynch = false)` |
| `FrameworkElementFacade` | `GetFrameworkElement` | `protected static global::System.Windows.FrameworkElement GetFrameworkElement(object obj)` |
| `FrameworkElementFacade` | `GetParent` | `public object GetParent(object control)` |
| `FrameworkElementFacade` | `SetHandleChangesEnabled` | `public void SetHandleChangesEnabled(object item, bool value)` |
| `FrameworkElementFacade` | `ClearHandleChangesEnabled` | `public void ClearHandleChangesEnabled(object item)` |
| `FrameworkElementFacade` | `GetResizableElement` | `public static global::System.Windows.FrameworkElement GetResizableElement(global::System.Windows.FrameworkElement elem, global::Quire.Contro` |
| `FrameworkElementFacade` | `SetOverrideLabelColor` | `public void SetOverrideLabelColor(object item, bool overideLabelColor)` |
| `FrameworkElementFacade` | `SetLabeledIdFormatProperty` | `public void SetLabeledIdFormatProperty(object item, object labelIdFormatDictionary)` |
| `FrameworkElementFacade` | `GetOverrideLabelColor` | `public bool GetOverrideLabelColor(object equationVisualElement)` |
| `FrameworkElementFacade` | `GetLabeledIdFormatProperty` | `public global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IdLabels, global::Quire.Controls.Core.ExtendedFont> GetLab` |

### `IORegions.IOItemsAliasManager` — 16

| Type | Member | Declaration |
|---|---|---|
| `IOItemsAliasManager` | `_typeOfThis` | `private static readonly global::System.Type _typeOfThis;` |
| `IOItemsAliasManager` | `EvaluationDefaultAlias` | `public const string EvaluationDefaultAlias = "out";` |
| `IOItemsAliasManager` | `InAliasMapProperty` | `public static global::System.Windows.DependencyProperty InAliasMapProperty;` |
| `IOItemsAliasManager` | `OutAliasMapProperty` | `public static global::System.Windows.DependencyProperty OutAliasMapProperty;` |
| `IOItemsAliasManager` | `.ctor` | `public IOItemsAliasManager()` |
| `IOItemsAliasManager` | `GetSExpressionsList` | `public static global::System.Collections.Generic.IList<string> GetSExpressionsList(global::System.Windows.DependencyObject region, global::Q` |
| `IOItemsAliasManager` | `HandleInAliassesPropertyChanged` | `private static void HandleInAliassesPropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChan` |
| `IOItemsAliasManager` | `HandleOutAliassesPropertyChanged` | `private static void HandleOutAliassesPropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyCha` |
| `IOItemsAliasManager` | `SetIOAliasMap` | `public static void SetIOAliasMap(global::System.Windows.DependencyObject element, global::System.Collections.Generic.Dictionary<string, stri` |
| `IOItemsAliasManager` | `GetIOAliasMap` | `public static global::System.Collections.Generic.Dictionary<string, string> GetIOAliasMap(global::System.Windows.DependencyObject element, b` |
| `IOItemsAliasManager` | `ClearIOAliasMap` | `public static void ClearIOAliasMap(global::System.Windows.DependencyObject element, bool isIn)` |
| `IOItemsAliasManager` | `GetAliases` | `public static global::System.Collections.Generic.List<string> GetAliases(global::System.Windows.DependencyObject element, bool isInp)` |
| `IOItemsAliasManager` | `SetAliases` | `private static void SetAliases(global::System.Windows.DependencyObject element, global::System.Collections.Generic.IList<string> aliases, bo` |
| `IOItemsAliasManager` | `Add` | `public bool Add(string alias, global::Quire.Controls.Core.IORegions.IIOItem item)` |
| `IOItemsAliasManager` | `Remove` | `public bool Remove(string alias)` |
| `IOItemsAliasManager` | `GetItem` | `public global::Quire.Controls.Core.IORegions.IIOItem GetItem(string alias, bool compareAliasCaseInsensitively)` |

### `ImageControl` — 27

| Type | Member | Declaration |
|---|---|---|
| `ImageControl` | `_typeOfThis` | `private static readonly global::System.Type _typeOfThis;` |
| `ImageControl` | `_dictionary` | `private static global::Quire.Controls.Core.ImageControlDictionary _dictionary;` |
| `ImageControl` | `SourceProperty` | `public static readonly global::System.Windows.DependencyProperty SourceProperty;` |
| `ImageControl` | `ChangedEvent` | `public static readonly global::System.Windows.RoutedEvent ChangedEvent;` |
| `ImageControl` | `BrowseForImageCommand` | `public static readonly global::System.Windows.Input.RoutedCommand BrowseForImageCommand;` |
| `ImageControl` | `_boundTo` | `private global::Quire.Controls.Core.ImageControl _boundTo;` |
| `ImageControl` | `Source` | `public global::System.Windows.Media.ImageSource Source` |
| `ImageControl` | `OriginalImageUri` | `public global::System.Uri OriginalImageUri` |
| `ImageControl` | `BitmapImage` | `private global::System.Windows.Media.Imaging.BitmapImage BitmapImage` |
| `ImageControl` | `BoundTo` | `public global::Quire.Controls.Core.ImageControl BoundTo` |
| `ImageControl` | `ResizeState` | `public global::Quire.Controls.Core.ResizeToFitState ResizeState` |
| `ImageControl` | `HandleSourcePropertyChanged` | `private static void HandleSourcePropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedE` |
| `ImageControl` | `FireRequestResizeToFit` | `private void FireRequestResizeToFit()` |
| `ImageControl` | `FireChanged` | `private void FireChanged()` |
| `ImageControl` | `CreateImageControl` | `public static global::Quire.Controls.Core.ImageControl CreateImageControl(bool isNewImageControl = true)` |
| `ImageControl` | `GetImageExtension` | `public static string GetImageExtension(global::System.Uri imageUri)` |
| `ImageControl` | `ShouldSerializeSource` | `public bool ShouldSerializeSource()` |
| `ImageControl` | `SourceIsBitmapImage` | `private bool SourceIsBitmapImage()` |
| `ImageControl` | `OnMouseLeftButtonDown` | `protected override void OnMouseLeftButtonDown(global::System.Windows.Input.MouseButtonEventArgs e)` |
| `ImageControl` | `OnGotFocus` | `protected override void OnGotFocus(global::System.Windows.RoutedEventArgs e)` |
| `ImageControl` | `BrowseForImage` | `public void BrowseForImage(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args)` |
| `ImageControl` | `BrowseForImage` | `public void BrowseForImage()` |
| `ImageControl` | `NavigateIntoItem` | `public global::System.Nullable<global::System.Windows.Vector> NavigateIntoItem(global::Quire.Direction direction)` |
| `ImageControl` | `Clone` | `public object Clone()` |
| `ImageControl` | `SetRegionPersistentData` | `public global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::Quire.Ser` |
| `ImageControl` | `GetImageRelationshipId` | `private string GetImageRelationshipId(global::Quire.Serialization.IS11NProvider s11NProvider)` |
| `ImageControl` | `GetRegionPersistentData` | `public object GetRegionPersistentData(global::Quire.Serialization.IS11NProvider s11NProvider)` |

### `MC14Conversion.Mathcad14TextStyle` — 24

| Type | Member | Declaration |
|---|---|---|
| `Mathcad14TextStyle` | `_namespace` | `private static readonly global::System.Xml.Linq.XNamespace _namespace;` |
| `Mathcad14TextStyle` | `_baseStyle` | `private global::Quire.Controls.Core.MC14Conversion.Mathcad14TextStyle _baseStyle;` |
| `Mathcad14TextStyle` | `_fontFamily` | `private global::System.Windows.Media.FontFamily _fontFamily;` |
| `Mathcad14TextStyle` | `_fontSize` | `private global::System.Nullable<double> _fontSize;` |
| `Mathcad14TextStyle` | `_fontWeight` | `private global::System.Nullable<global::System.Windows.FontWeight> _fontWeight;` |
| `Mathcad14TextStyle` | `_fontStyle` | `private global::System.Nullable<global::System.Windows.FontStyle> _fontStyle;` |
| `Mathcad14TextStyle` | `_textDecorations` | `private global::System.Windows.TextDecorationCollection _textDecorations;` |
| `Mathcad14TextStyle` | `_color` | `private global::System.Nullable<global::System.Windows.Media.Color> _color;` |
| `Mathcad14TextStyle` | `_variants` | `private global::System.Nullable<global::System.Windows.FontVariants> _variants;` |
| `Mathcad14TextStyle` | `_margin` | `private global::System.Nullable<global::System.Windows.Thickness> _margin;` |
| `Mathcad14TextStyle` | `_textIndent` | `private global::System.Nullable<double> _textIndent;` |
| `Mathcad14TextStyle` | `_textAlign` | `private global::System.Nullable<global::System.Windows.TextAlignment> _textAlign;` |
| `Mathcad14TextStyle` | `_markerStyle` | `private global::System.Nullable<global::System.Windows.TextMarkerStyle> _markerStyle;` |
| `Mathcad14TextStyle` | `BaseStyle` | `private global::Quire.Controls.Core.MC14Conversion.Mathcad14TextStyle BaseStyle` |
| `Mathcad14TextStyle` | `ReadLocalOrInheritedValue` | `protected T ReadLocalOrInheritedValue<T>(ref T thisValue, global::Quire.DelegateFunction1<T, global::Quire.Controls.Core.MC14Conversion.Math` |
| `Mathcad14TextStyle` | `MC14TextStyleNameConverter` | `public static string MC14TextStyleNameConverter(string MC14TextStyleName)` |
| `Mathcad14TextStyle` | `ResetVariantsToDeafault` | `public void ResetVariantsToDeafault()` |
| `Mathcad14TextStyle` | `SetInlineTextClassProperty` | `public void SetInlineTextClassProperty(string elementName, global::System.Xml.XmlAttributeCollection attributes)` |
| `Mathcad14TextStyle` | `.ctor` | `public Mathcad14TextStyle(global::Quire.Controls.Core.MC14Conversion.Mathcad14TextStyle baseStyle) : this(default(global::System.Collections` |
| `Mathcad14TextStyle` | `SetLocalValueFromXElementValue` | `private void SetLocalValueFromXElementValue(global::System.Xml.Linq.XElement xElement, string attributeName, global::Quire.Delegate1<string>` |
| `Mathcad14TextStyle` | `ApplyInlineAttr` | `public void ApplyInlineAttr(global::System.Windows.Documents.Run inline)` |
| `Mathcad14TextStyle` | `ApplyBlockAttr` | `public void ApplyBlockAttr(global::System.Windows.Documents.Paragraph paragraph)` |
| `Mathcad14TextStyle` | `ReadXml` | `public void ReadXml(global::System.Xml.Linq.XElement xElement)` |
| `Mathcad14TextStyle` | `LoadTextStylesFromXml` | `public static void LoadTextStylesFromXml(global::System.Xml.XmlNode stylesNode, global::System.Collections.Generic.Dictionary<string, global` |

### `MathFormat` — 28

| Type | Member | Declaration |
|---|---|---|
| `MathFormat` | `ChangeFontColorCommand` | `public static readonly global::System.Windows.Input.RoutedUICommand ChangeFontColorCommand;` |
| `MathFormat` | `ChangeHighlightColorCommand` | `public static readonly global::System.Windows.Input.RoutedUICommand ChangeHighlightColorCommand;` |
| `MathFormat` | `IncreaseFontSizeCommand` | `public static readonly global::System.Windows.Input.RoutedUICommand IncreaseFontSizeCommand;` |
| `MathFormat` | `DecreaseFontSizeCommand` | `public static readonly global::System.Windows.Input.RoutedUICommand DecreaseFontSizeCommand;` |
| `MathFormat` | `ClearMathFormatCommand` | `public static readonly global::System.Windows.Input.RoutedUICommand ClearMathFormatCommand;` |
| `MathFormat` | `FontFamilyName` | `public string FontFamilyName` |
| `MathFormat` | `FontSize` | `public global::System.Nullable<double> FontSize` |
| `MathFormat` | `IsBold` | `public global::System.Nullable<bool> IsBold` |
| `MathFormat` | `IsItalic` | `public global::System.Nullable<bool> IsItalic` |
| `MathFormat` | `IsUnderline` | `public global::System.Nullable<bool> IsUnderline` |
| `MathFormat` | `FontColor` | `public global::System.Nullable<global::System.Windows.Media.Color> FontColor` |
| `MathFormat` | `HighlightColor` | `public global::System.Nullable<global::System.Windows.Media.Color> HighlightColor` |
| `MathFormat` | `AreAnyPropertiesSet` | `public bool AreAnyPropertiesSet` |
| `MathFormat` | `.ctor` | `public MathFormat(global::System.Windows.DependencyObject owner)` |
| `MathFormat` | `ChangeFontColor` | `public bool ChangeFontColor(global::System.Windows.Media.Color fontColor)` |
| `MathFormat` | `ChangeHighlightColor` | `public bool ChangeHighlightColor(global::System.Nullable<global::System.Windows.Media.Color> highlightColor)` |
| `MathFormat` | `IsEquivalentTo` | `public bool IsEquivalentTo(global::Quire.Controls.Core.IMathFormat mathFormat)` |
| `MathFormat` | `ReadLocalValue` | `private object ReadLocalValue(global::System.Windows.DependencyProperty dependencyProperty)` |
| `MathFormat` | `GetValue` | `private object GetValue(global::System.Windows.DependencyProperty dependencyProperty)` |
| `MathFormat` | `ClearAllValues` | `private void ClearAllValues()` |
| `MathFormat` | `RestoreInitialValues` | `public void RestoreInitialValues()` |
| `MathFormat` | `CopyTo` | `public void CopyTo(global::Quire.Controls.Core.IMathFormat target, bool copyInheritedProperties = false)` |
| `MathFormat` | `ToStaticMathFormat` | `public global::Quire.Controls.Core.StaticMathFormat ToStaticMathFormat()` |
| `MathFormat` | `FromStaticMathFormat` | `public void FromStaticMathFormat(global::Quire.Controls.Core.StaticMathFormat mathFormat)` |
| `MathFormat` | `HandleOwnerPropertyChanged` | `private void HandleOwnerPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)` |
| `MathFormat` | `SetReadonlyProperty` | `public void SetReadonlyProperty(global::System.Windows.DependencyProperty dependencyProperty, object val)` |
| `MathFormat` | `GetDataObject` | `public global::Quire.PersistentDataObjects.mathFormattingType GetDataObject(bool copyInheritedProperties = false)` |
| `MathFormat` | `SetDataObject` | `public void SetDataObject(global::Quire.PersistentDataObjects.mathFormattingType formattingOverride)` |

### `MathFormatAttachedProperties` — 27

| Type | Member | Declaration |
|---|---|---|
| `MathFormatAttachedProperties` | `FontSizeProperty` | `public static readonly global::System.Windows.DependencyProperty FontSizeProperty;` |
| `MathFormatAttachedProperties` | `BoldProperty` | `public static readonly global::System.Windows.DependencyProperty BoldProperty;` |
| `MathFormatAttachedProperties` | `ItalicProperty` | `public static readonly global::System.Windows.DependencyProperty ItalicProperty;` |
| `MathFormatAttachedProperties` | `UnderlineProperty` | `public static readonly global::System.Windows.DependencyProperty UnderlineProperty;` |
| `MathFormatAttachedProperties` | `FontColorProperty` | `public static readonly global::System.Windows.DependencyProperty FontColorProperty;` |
| `MathFormatAttachedProperties` | `HighlightColorProperty` | `public static readonly global::System.Windows.DependencyProperty HighlightColorProperty;` |
| `MathFormatAttachedProperties` | `GetFontFamilyName` | `public static string GetFontFamilyName(global::System.Windows.DependencyObject depObj)` |
| `MathFormatAttachedProperties` | `GetFontSize` | `public static double GetFontSize(global::System.Windows.DependencyObject depObj)` |
| `MathFormatAttachedProperties` | `SetFontSize` | `public static void SetFontSize(global::System.Windows.DependencyObject depObj, double fontSize)` |
| `MathFormatAttachedProperties` | `GetBold` | `public static bool GetBold(global::System.Windows.DependencyObject depObj)` |
| `MathFormatAttachedProperties` | `SetBold` | `public static void SetBold(global::System.Windows.DependencyObject depObj, bool bold)` |
| `MathFormatAttachedProperties` | `GetItalic` | `public static bool GetItalic(global::System.Windows.DependencyObject depObj)` |
| `MathFormatAttachedProperties` | `SetItalic` | `public static void SetItalic(global::System.Windows.DependencyObject depObj, bool italic)` |
| `MathFormatAttachedProperties` | `GetUnderline` | `public static bool GetUnderline(global::System.Windows.DependencyObject depObj)` |
| `MathFormatAttachedProperties` | `SetUnderline` | `public static void SetUnderline(global::System.Windows.DependencyObject depObj, bool underline)` |
| `MathFormatAttachedProperties` | `GetFontColor` | `public static global::System.Windows.Media.Color GetFontColor(global::System.Windows.DependencyObject depObj)` |
| `MathFormatAttachedProperties` | `SetFontColor` | `public static void SetFontColor(global::System.Windows.DependencyObject depObj, global::System.Windows.Media.Color fontColor)` |
| `MathFormatAttachedProperties` | `GetHighlightColor` | `public static global::System.Windows.Media.Color GetHighlightColor(global::System.Windows.DependencyObject depObj)` |
| `MathFormatAttachedProperties` | `SetHighlightColor` | `public static void SetHighlightColor(global::System.Windows.DependencyObject depObj, global::System.Windows.Media.Color highlightColor)` |
| `MathFormatAttachedProperties` | `CopyAllMathFormatProperties` | `public static void CopyAllMathFormatProperties(global::System.Windows.DependencyObject source, global::System.Windows.DependencyObject targe` |
| `MathFormatAttachedProperties` | `GetOverrideLabelColor` | `public static bool GetOverrideLabelColor(global::System.Windows.DependencyObject obj)` |
| `MathFormatAttachedProperties` | `SetOverrideLabelColor` | `public static void SetOverrideLabelColor(global::System.Windows.DependencyObject obj, bool val)` |
| `MathFormatAttachedProperties` | `GetHandleChangesEnabled` | `public static bool GetHandleChangesEnabled(global::System.Windows.DependencyObject obj)` |
| `MathFormatAttachedProperties` | `SetHandleChangesEnabled` | `public static void SetHandleChangesEnabled(global::System.Windows.DependencyObject obj, bool value)` |
| `MathFormatAttachedProperties` | `HandleMathFormatChanged` | `private static void HandleMathFormatChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEvent` |
| `MathFormatAttachedProperties` | `CoerceFontFamily` | `private static object CoerceFontFamily(global::System.Windows.DependencyObject d, object basevalue)` |
| `MathFormatAttachedProperties` | `CoerceFontSize` | `private static object CoerceFontSize(global::System.Windows.DependencyObject d, object basevalue)` |

### `MatrixFormat` — 24

| Type | Member | Declaration |
|---|---|---|
| `MatrixFormat` | `_size` | `private global::Quire.TableSize _size;` |
| `MatrixFormat` | `_offset` | `private global::Quire.TableCoordinate _offset;` |
| `MatrixFormat` | `_origin` | `private int _origin;` |
| `MatrixFormat` | `_showIndices` | `private bool _showIndices;` |
| `MatrixFormat` | `_collapseNestedMatrices` | `private bool _collapseNestedMatrices;` |
| `MatrixFormat` | `_nestedMatrixFormats` | `private global::Quire.Controls.Core.NestedMatrixFormats _nestedMatrixFormats;` |
| `MatrixFormat` | `SetMatrixFormatFieldCommand` | `public static readonly global::System.Windows.Input.RoutedCommand SetMatrixFormatFieldCommand;` |
| `MatrixFormat` | `Size` | `public global::Quire.TableSize Size` |
| `MatrixFormat` | `Offset` | `public global::Quire.TableCoordinate Offset` |
| `MatrixFormat` | `Origin` | `public int Origin` |
| `MatrixFormat` | `ShowIndices` | `public bool ShowIndices` |
| `MatrixFormat` | `CollapseNestedMatrices` | `public bool CollapseNestedMatrices` |
| `MatrixFormat` | `NestedMatrixFormats` | `public global::Quire.Controls.Core.NestedMatrixFormats NestedMatrixFormats` |
| `MatrixFormat` | `HandleMatrixFormatChanged` | `private void HandleMatrixFormatChanged(string propertyName, object newValue)` |
| `MatrixFormat` | `.ctor` | `public MatrixFormat()` |
| `MatrixFormat` | `HandleNestedMatrixChanged` | `private void HandleNestedMatrixChanged(object sender, global::Quire.Matrices.SparseMatrix<global::Quire.Controls.Core.MatrixFormat>.SparseMa` |
| `MatrixFormat` | `GetValueByName` | `public object GetValueByName(string propertyName)` |
| `MatrixFormat` | `SetValueByName` | `public void SetValueByName(string propertyName, object value)` |
| `MatrixFormat` | `op_Inequality` | `public static bool operator !=(global::Quire.Controls.Core.MatrixFormat format, global::Quire.Controls.Core.MatrixFormat other)` |
| `MatrixFormat` | `GetFormatAdjustedForActualMatrixSize` | `public static global::Quire.Controls.Core.MatrixFormat GetFormatAdjustedForActualMatrixSize(global::Quire.TableSize actualSize, global::Quir` |
| `MatrixFormat` | `AddFormatChangedHandlerToNestedMatrixFormat` | `public void AddFormatChangedHandlerToNestedMatrixFormat(global::Quire.Controls.Core.ActionByPropertyNameAndValue action)` |
| `MatrixFormat` | `RemoveFormatChangedHandlerFromNestedMatrixFormat` | `public void RemoveFormatChangedHandlerFromNestedMatrixFormat(global::Quire.Controls.Core.ActionByPropertyNameAndValue action)` |
| `MatrixFormat` | `GetCommonMatrixFormat` | `public static global::Quire.Controls.Core.MatrixFormat GetCommonMatrixFormat(global::System.Collections.Generic.IEnumerable<global::Quire.Co` |
| `MatrixFormat` | `FirePropertyChanged` | `protected void FirePropertyChanged(string propertyName)` |

### `NodeMessageAdorner` — 17

| Type | Member | Declaration |
|---|---|---|
| `NodeMessageAdorner` | `_messageTextBox` | `private readonly global::System.Windows.Controls.TextBox _messageTextBox;` |
| `NodeMessageAdorner` | `_messageBorder` | `private readonly global::System.Windows.Controls.Border _messageBorder;` |
| `NodeMessageAdorner` | `RelativeLocation` | `public global::System.Windows.Point RelativeLocation` |
| `NodeMessageAdorner` | `Size` | `public global::System.Windows.Size Size` |
| `NodeMessageAdorner` | `AdornedElementDesiredSize` | `private global::System.Windows.Size AdornedElementDesiredSize` |
| `NodeMessageAdorner` | `NodeMessageAdorner_SizeChanged` | `private void NodeMessageAdorner_SizeChanged(object sender, global::System.Windows.SizeChangedEventArgs e)` |
| `NodeMessageAdorner` | `.ctor` | `public NodeMessageAdorner(global::System.Windows.FrameworkElement adornedElement, string error, global::Quire.Controls.Core.NodeMessageAdorn` |
| `NodeMessageAdorner` | `HandleAdornedElementWasMeasured` | `private void HandleAdornedElementWasMeasured(object sender, global::System.Windows.RoutedEventArgs routedEventArgs)` |
| `NodeMessageAdorner` | `MeasureOverride` | `protected override global::System.Windows.Size MeasureOverride(global::System.Windows.Size constraint)` |
| `NodeMessageAdorner` | `ArrangeOverride` | `protected override global::System.Windows.Size ArrangeOverride(global::System.Windows.Size finalSize)` |
| `NodeMessageAdorner` | `CalculateDesiredMessageBorderWidth` | `private double CalculateDesiredMessageBorderWidth()` |
| `NodeMessageAdorner` | `Remove` | `public override void Remove()` |
| `NodeMessageAdorner` | `GetAllNodeMessageAdorners` | `private global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.NodeMessageAdorner> GetAllNodeMessageAdorners()` |
| `NodeMessageAdorner` | `InvalidateArrangeForAllNodeMessageAdornersExcludingSelf` | `private void InvalidateArrangeForAllNodeMessageAdornersExcludingSelf()` |
| `NodeMessageAdorner` | `GetOffset` | `private double GetOffset(global::Quire.Controls.Core.NodeMessageAdorner current)` |
| `NodeMessageAdorner` | `CalculateOffset` | `private static double CalculateOffset(global::System.Collections.Generic.IList<global::Quire.Controls.Core.NodeMessageAdorner> adorners, int` |
| `NodeMessageAdorner` | `GetAdornedElement` | `public static global::System.Windows.FrameworkElement GetAdornedElement(global::System.Windows.FrameworkElement control)` |

### `NumberFormat` — 61

| Type | Member | Declaration |
|---|---|---|
| `NumberFormat` | `_decimalPlaces` | `private global::System.Nullable<int> _decimalPlaces;` |
| `NumberFormat` | `_showTrailingDecimalZeros` | `private bool _showTrailingDecimalZeros;` |
| `NumberFormat` | `_numberStyle` | `private global::System.Nullable<global::Quire.Controls.Core.NumberFormat.NumberStyles> _numberStyle;` |
| `NumberFormat` | `_exponentStyle` | `private global::System.Nullable<global::Quire.Controls.Core.NumberFormat.ExponentStyles> _exponentStyle;` |
| `NumberFormat` | `_complexStyle` | `private global::System.Nullable<global::Quire.Controls.Core.NumberFormat.ComplexStyles> _complexStyle;` |
| `NumberFormat` | `_zeroThreshold` | `private global::System.Nullable<int> _zeroThreshold;` |
| `NumberFormat` | `_exponentialThreshold` | `private global::System.Nullable<int> _exponentialThreshold;` |
| `NumberFormat` | `_complexThreshold` | `private global::System.Nullable<int> _complexThreshold;` |
| `NumberFormat` | `_radixStyle` | `private global::System.Nullable<global::Quire.Controls.Core.NumberFormat.RadixStyles> _radixStyle;` |
| `NumberFormat` | `DefaultComplexThresholdValue` | `public const int DefaultComplexThresholdValue = 10;` |
| `NumberFormat` | `MinComplexThresholdAllowed` | `public const int MinComplexThresholdAllowed = 0;` |
| `NumberFormat` | `MaxComplexThresholdAllowed` | `public const int MaxComplexThresholdAllowed = 307;` |
| `NumberFormat` | `DefaultZeroThresholdValue` | `public const int DefaultZeroThresholdValue = 15;` |
| `NumberFormat` | `MinZeroThresholdAllowed` | `public const int MinZeroThresholdAllowed = 0;` |
| `NumberFormat` | `MaxZeroThresholdAllowed` | `public const int MaxZeroThresholdAllowed = 307;` |
| `NumberFormat` | `DefaultExponentialThresholdValue` | `public const int DefaultExponentialThresholdValue = 3;` |
| `NumberFormat` | `MinExponentialThresholdAllowed` | `public const int MinExponentialThresholdAllowed = 0;` |
| `NumberFormat` | `MaxExponentialThresholdAllowed` | `public const int MaxExponentialThresholdAllowed = 15;` |
| `NumberFormat` | `MinDecimalPlacesAllowed` | `public const int MinDecimalPlacesAllowed = 0;` |
| `NumberFormat` | `MaxDecimalPlacesAllowed` | `public const int MaxDecimalPlacesAllowed = 15;` |
| `NumberFormat` | `DefaultDecimalPlacesValue` | `public const int DefaultDecimalPlacesValue = 3;` |
| `NumberFormat` | `RoundTripDecimalPlaces` | `public const int RoundTripDecimalPlaces = -1;` |
| `NumberFormat` | `DecimalBase` | `public const int DecimalBase = 10;` |
| `NumberFormat` | `BinaryBase` | `public const int BinaryBase = 2;` |
| `NumberFormat` | `OctalBase` | `public const int OctalBase = 8;` |
| `NumberFormat` | `HexadecimalBase` | `public const int HexadecimalBase = 16;` |
| `NumberFormat` | `SetNumberFormatFieldCommand` | `public static readonly global::System.Windows.Input.RoutedCommand SetNumberFormatFieldCommand;` |
| `NumberFormat` | `SetNumberFormatExponentialThresholdCommand` | `public static global::System.Windows.Input.RoutedCommand SetNumberFormatExponentialThresholdCommand;` |
| `NumberFormat` | `ExponentialThresholdControl` | `public static global::System.Windows.Controls.Control ExponentialThresholdControl;` |
| `NumberFormat` | `SetNumberFormatZeroThresholdCommand` | `public static global::System.Windows.Input.RoutedCommand SetNumberFormatZeroThresholdCommand;` |
| `NumberFormat` | `ZeroThresholdControl` | `public static global::System.Windows.Controls.Control ZeroThresholdControl;` |
| `NumberFormat` | `SetNumberFormatComplexThresholdCommand` | `public static global::System.Windows.Input.RoutedCommand SetNumberFormatComplexThresholdCommand;` |
| `NumberFormat` | `ComplexThresholdControl` | `public static global::System.Windows.Controls.Control ComplexThresholdControl;` |
| `NumberFormat` | `DecimalPlaces` | `public global::System.Nullable<int> DecimalPlaces` |
| `NumberFormat` | `ShowTrailingDecimalZeros` | `public bool ShowTrailingDecimalZeros` |
| `NumberFormat` | `NumberStyle` | `public global::System.Nullable<global::Quire.Controls.Core.NumberFormat.NumberStyles> NumberStyle` |
| `NumberFormat` | `ExponentStyle` | `public global::System.Nullable<global::Quire.Controls.Core.NumberFormat.ExponentStyles> ExponentStyle` |
| `NumberFormat` | `ComplexStyle` | `public global::System.Nullable<global::Quire.Controls.Core.NumberFormat.ComplexStyles> ComplexStyle` |
| `NumberFormat` | `ZeroThreshold` | `public global::System.Nullable<int> ZeroThreshold` |
| `NumberFormat` | `ExponentialThreshold` | `public global::System.Nullable<int> ExponentialThreshold` |
| `NumberFormat` | `ComplexThreshold` | `public global::System.Nullable<int> ComplexThreshold` |
| `NumberFormat` | `RadixStyle` | `public global::System.Nullable<global::Quire.Controls.Core.NumberFormat.RadixStyles> RadixStyle` |
| `NumberFormat` | `ComplexStyleIsCartesian` | `public bool ComplexStyleIsCartesian` |
| `NumberFormat` | `ComplexStyleIsPolar` | `public bool ComplexStyleIsPolar` |
| `NumberFormat` | `IsValidDecimalPlaces` | `private static bool IsValidDecimalPlaces(global::System.Nullable<int> value)` |
| `NumberFormat` | `CoerceDecimalPlaces` | `public static int CoerceDecimalPlaces(int value, int increment)` |
| `NumberFormat` | `ShouldBeDisplayedAsScientific` | `public bool ShouldBeDisplayedAsScientific(double value)` |
| `NumberFormat` | `IsNumberInRange` | `private static bool IsNumberInRange(global::System.Nullable<int> num, int minimum, int maximum)` |
| `NumberFormat` | `HandleNumberFormatPropertyChanged` | `private void HandleNumberFormatPropertyChanged(string propertyName, object newValue)` |
| `NumberFormat` | `ShouldBeDisplayedAsZero` | `public bool ShouldBeDisplayedAsZero(double value)` |
| `NumberFormat` | `ApplyComplexThreshold` | `public void ApplyComplexThreshold(ref double rValue, ref double iValue)` |
| `NumberFormat` | `.ctor` | `public NumberFormat()` |
| `NumberFormat` | `Clone` | `public global::Quire.Controls.Core.NumberFormat Clone()` |
| `NumberFormat` | `op_Equality` | `public static bool operator ==(global::Quire.Controls.Core.NumberFormat format, global::Quire.Controls.Core.NumberFormat other)` |
| `NumberFormat` | `op_Inequality` | `public static bool operator !=(global::Quire.Controls.Core.NumberFormat format, global::Quire.Controls.Core.NumberFormat other)` |
| `NumberFormat` | `ToString` | `public override string ToString()` |
| `NumberFormat` | `Clone` | `object global::System.ICloneable.Clone()` |
| `NumberFormat` | `GetValueByName` | `public object GetValueByName(string propertyName)` |
| `NumberFormat` | `SetValueByName` | `public void SetValueByName(string propertyName, object value)` |
| `NumberFormat` | `GetCommonNumberFormat` | `public static global::Quire.Controls.Core.NumberFormat GetCommonNumberFormat(global::System.Collections.Generic.IEnumerable<global::Quire.Co` |
| `NumberFormat` | `FirePropertyChanged` | `protected void FirePropertyChanged(string propertyName)` |

### `NumericUpDown` — 21

| Type | Member | Declaration |
|---|---|---|
| `NumericUpDown` | `NumberValueProperty` | `protected static readonly global::System.Windows.DependencyProperty NumberValueProperty;` |
| `NumericUpDown` | `IsInputValidProperty` | `private static readonly global::System.Windows.DependencyProperty IsInputValidProperty;` |
| `NumericUpDown` | `ValidationMessageProperty` | `private static readonly global::System.Windows.DependencyProperty ValidationMessageProperty;` |
| `NumericUpDown` | `ValueTextBox` | `internal global::System.Windows.Controls.TextBox ValueTextBox;` |
| `NumericUpDown` | `WaterMarkTextBlock` | `internal global::System.Windows.Controls.TextBlock WaterMarkTextBlock;` |
| `NumericUpDown` | `UpButton` | `internal global::System.Windows.Controls.Button UpButton;` |
| `NumericUpDown` | `DownButton` | `internal global::System.Windows.Controls.Button DownButton;` |
| `NumericUpDown` | `_contentLoaded` | `private bool _contentLoaded;` |
| `NumericUpDown` | `NumberValue` | `public virtual int NumberValue` |
| `NumericUpDown` | `IsInputValid` | `public bool IsInputValid` |
| `NumericUpDown` | `DefaultValidationMessage` | `public string DefaultValidationMessage` |
| `NumericUpDown` | `ValidationMessage` | `public string ValidationMessage` |
| `NumericUpDown` | `OnInitialized` | `protected override void OnInitialized(global::System.EventArgs e)` |
| `NumericUpDown` | `ValueTextBox_TextChanged` | `private void ValueTextBox_TextChanged(object sender, global::System.Windows.Controls.TextChangedEventArgs e)` |
| `NumericUpDown` | `ValueTextBox_KeyUp` | `private void ValueTextBox_KeyUp(object sender, global::System.Windows.Input.KeyEventArgs e)` |
| `NumericUpDown` | `IncrementValue` | `private void IncrementValue()` |
| `NumericUpDown` | `DecrementValue` | `private void DecrementValue()` |
| `NumericUpDown` | `OnNumberChanged` | `protected static void OnNumberChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e` |
| `NumericUpDown` | `ShowValue` | `private void ShowValue()` |
| `NumericUpDown` | `IsTextBoxEmpty` | `public bool IsTextBoxEmpty()` |
| `NumericUpDown` | `Connect` | `void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)` |

### `Ole.IPublicOleContainerModel_1` — 10

| Type | Member | Declaration |
|---|---|---|
| `IPublicOleContainerModel` | `IsStorageEmpty` | `bool IsStorageEmpty` |
| `IPublicOleContainerModel` | `ProgId` | `string ProgId` |
| `IPublicOleContainerModel` | `ClassId` | `global::System.Guid ClassId` |
| `IPublicOleContainerModel` | `Save` | `string Save(global::System.Action onServerBusy)` |
| `IPublicOleContainerModel` | `Load` | `bool Load(string fileName)` |
| `IPublicOleContainerModel` | `CleanStorage` | `void CleanStorage(bool deleteFile)` |
| `IPublicOleContainerModel` | `SaveBitmap` | `string SaveBitmap(global::System.Func<global::System.Windows.Media.Imaging.BitmapSource> getImageSource)` |
| `IPublicOleContainerModel` | `LoadBitmap` | `global::System.Windows.Media.Imaging.BitmapSource LoadBitmap(string filename)` |
| `IPublicOleContainerModel` | `SetRegionPersistentData` | `global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::Quire.Serializat` |
| `IPublicOleContainerModel` | `GetRegionPersistentData` | `TPersistentData GetRegionPersistentData(global::Quire.Serialization.IS11NProvider s11NProvider, string uniqueId, global::System.Action onSer` |

### `Ole.OleContainerModel_1` — 90

| Type | Member | Declaration |
|---|---|---|
| `OleContainerModel` | `_oleSiteManager` | `private readonly global::Quire.OLE.IOleSiteManager _oleSiteManager;` |
| `OleContainerModel` | `_temporaryStorageHelper` | `private readonly global::Quire.Controls.Core.TemporaryStorageHelper _temporaryStorageHelper;` |
| `OleContainerModel` | `Dispatcher` | `protected readonly global::System.Windows.Threading.Dispatcher Dispatcher;` |
| `OleContainerModel` | `_lock` | `private readonly object _lock;` |
| `OleContainerModel` | `_worksheetName` | `private string _worksheetName;` |
| `OleContainerModel` | `_displayFilePath` | `private string _displayFilePath;` |
| `OleContainerModel` | `_sizeData` | `private readonly global::Quire.Controls.Core.Ole.OleSizeDataWithNotification _sizeData;` |
| `OleContainerModel` | `_messageList` | `private global::Quire.Controls.Core.AttachedBehavior.MessageList _messageList;` |
| `OleContainerModel` | `_guid` | `private global::System.Guid _guid;` |
| `OleContainerModel` | `SiteManager` | `protected global::Quire.OLE.IOleSiteManager SiteManager` |
| `OleContainerModel` | `WorksheetName` | `public string WorksheetName` |
| `OleContainerModel` | `DisplayFilePath` | `public string DisplayFilePath` |
| `OleContainerModel` | `SizeData` | `protected global::Quire.Controls.Core.Ole.OleSizeDataWithNotification SizeData` |
| `OleContainerModel` | `AutoResize` | `public bool AutoResize` |
| `OleContainerModel` | `OleViewportSize` | `public global::System.Windows.Size OleViewportSize` |
| `OleContainerModel` | `AutomaticSize` | `public global::System.Windows.Size AutomaticSize` |
| `OleContainerModel` | `SizeDataForXamlSerialization` | `public global::Quire.PersistentData.IOleSizeData SizeDataForXamlSerialization` |
| `OleContainerModel` | `HasFakeOleViewportSize` | `public bool HasFakeOleViewportSize` |
| `OleContainerModel` | `ProgId` | `public virtual string ProgId` |
| `OleContainerModel` | `ExcludeElementsNames` | `protected virtual string[] ExcludeElementsNames` |
| `OleContainerModel` | `IsDisconnected` | `protected virtual bool IsDisconnected` |
| `OleContainerModel` | `IsOleSiteManagerRunning` | `public virtual bool IsOleSiteManagerRunning` |
| `OleContainerModel` | `IsOutOfPlaceActive` | `public bool IsOutOfPlaceActive` |
| `OleContainerModel` | `IsOleAppSupportZoom` | `public bool IsOleAppSupportZoom` |
| `OleContainerModel` | `SiteCreated` | `public bool SiteCreated` |
| `OleContainerModel` | `AsyncUpdateVisibleSize` | `public virtual bool AsyncUpdateVisibleSize` |
| `OleContainerModel` | `Width` | `public double Width` |
| `OleContainerModel` | `Height` | `public double Height` |
| `OleContainerModel` | `ToggleActivateStateFunctionalityKey` | `protected abstract global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey ToggleActivateStateFunctionalityKey` |
| `OleContainerModel` | `StorageFileName` | `private string StorageFileName` |
| `OleContainerModel` | `GuidsConversionDict` | `public virtual global::System.Collections.Generic.Dictionary<global::System.Guid, global::System.Guid> GuidsConversionDict` |
| `OleContainerModel` | `IsStorageEmpty` | `public bool IsStorageEmpty` |
| `OleContainerModel` | `ClassId` | `public global::System.Guid ClassId` |
| `OleContainerModel` | `MessageList` | `public global::Quire.Controls.Core.AttachedBehavior.MessageList MessageList` |
| `OleContainerModel` | `Unlocked` | `private event global::System.Action Unlocked;` |
| `OleContainerModel` | `OleSavedAsync` | `protected internal event global::System.Action OleSavedAsync;` |
| `OleContainerModel` | `OleViewChangedAsync` | `protected internal event global::System.Action OleViewChangedAsync;` |
| `OleContainerModel` | `OleShownAsync` | `protected internal event global::System.Action OleShownAsync;` |
| `OleContainerModel` | `OleHiddenAsync` | `protected internal event global::System.Action OleHiddenAsync;` |
| `OleContainerModel` | `OleClosedAsync` | `protected internal event global::System.Action OleClosedAsync;` |
| `OleContainerModel` | `OleEditingStartedAsync` | `protected internal event global::System.Action OleEditingStartedAsync;` |
| `OleContainerModel` | `OleEditingEndedAsync` | `protected internal event global::System.Action OleEditingEndedAsync;` |
| `OleContainerModel` | `OleSavedSync` | `protected internal event global::System.Action OleSavedSync;` |
| `OleContainerModel` | `OleViewChangedSync` | `protected internal event global::System.Action OleViewChangedSync;` |
| `OleContainerModel` | `OleShownSync` | `protected internal event global::System.Action OleShownSync;` |
| `OleContainerModel` | `OleHiddenSync` | `protected internal event global::System.Action OleHiddenSync;` |
| `OleContainerModel` | `OleDataChangedSync` | `protected internal event global::System.Action OleDataChangedSync;` |
| `OleContainerModel` | `OleClosedSync` | `protected internal event global::System.Action OleClosedSync;` |
| `OleContainerModel` | `OleEditingStartedSync` | `protected internal event global::System.Action OleEditingStartedSync;` |
| `OleContainerModel` | `OleEditingEndedSync` | `protected internal event global::System.Action OleEditingEndedSync;` |
| `OleContainerModel` | `PropertyChanged` | `public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;` |
| `OleContainerModel` | `OnOleClosedSync` | `protected virtual void OnOleClosedSync()` |
| `OleContainerModel` | `CopyFrom` | `public void CopyFrom(global::Quire.PersistentData.IOleSizeData data, bool notify)` |
| `OleContainerModel` | `AttemptGetVisibleSize` | `public bool AttemptGetVisibleSize(bool forceRun)` |
| `OleContainerModel` | `OnStartRunning` | `protected virtual void OnStartRunning()` |
| `OleContainerModel` | `AssertDispatcherAccess` | `protected void AssertDispatcherAccess()` |
| `OleContainerModel` | `GetOleObject` | `protected object GetOleObject(bool forceRun, bool ignoreRunningState = false)` |
| `OleContainerModel` | `SetExtent` | `private bool SetExtent(global::System.Drawing.Size size, bool forceRun)` |
| `OleContainerModel` | `GetExtent` | `private global::System.Drawing.Size GetExtent(bool forceRun)` |
| `OleContainerModel` | `SetHostNamesIfRunning` | `private void SetHostNamesIfRunning(string appName, string docName)` |
| `OleContainerModel` | `CreateHBitmap` | `internal nint CreateHBitmap(global::System.Windows.Int32Rect rect, nint winHandle)` |
| `OleContainerModel` | `CreateOleFromProgId` | `public bool CreateOleFromProgId(string progId, string oleContainerPath)` |
| `OleContainerModel` | `CreateOleFromFile` | `public bool CreateOleFromFile(string filePath, bool link, string oleContainerPath)` |
| `OleContainerModel` | `CreateOleWithDialog` | `public bool CreateOleWithDialog(nint winHandler, string dialogTitle, string oleContainerPath, out bool isNew, out bool isLink, out bool isCa` |
| `OleContainerModel` | `GetVisibleSize` | `public global::System.Windows.Size GetVisibleSize(bool forceRun)` |
| `OleContainerModel` | `UpdateOleSizeIfNeeded` | `public void UpdateOleSizeIfNeeded()` |
| `OleContainerModel` | `UpdateOleMessageIfNeeded` | `private void UpdateOleMessageIfNeeded(global::Quire.OLE.OleResult oleResult, global::System.Collections.Generic.Dictionary<global::Quire.OLE` |
| `OleContainerModel` | `Dispose` | `public virtual void Dispose()` |
| `OleContainerModel` | `CheckServerStatus` | `protected virtual global::Quire.OLE.OleResult CheckServerStatus(bool forceRun)` |
| `OleContainerModel` | `CheckServerAvailable` | `public bool CheckServerAvailable(bool forceRun, global::System.Collections.Generic.Dictionary<global::Quire.OLE.OleResult, string> messageKe` |
| `OleContainerModel` | `Deactivate` | `public void Deactivate(bool mustTakeLock = true)` |
| `OleContainerModel` | `BeforeActivate` | `protected virtual bool BeforeActivate()` |
| `OleContainerModel` | `AfterActivate` | `protected virtual void AfterActivate()` |
| `OleContainerModel` | `DoDeactivate` | `protected virtual void DoDeactivate()` |
| `OleContainerModel` | `AbortDeactivate` | `public void AbortDeactivate()` |
| `OleContainerModel` | `SaveBitmap` | `public string SaveBitmap(global::System.Func<global::System.Windows.Media.Imaging.BitmapSource> getImageSource)` |
| `OleContainerModel` | `Load` | `public virtual bool Load(string storageFileName)` |
| `OleContainerModel` | `LoadBitmap` | `public global::System.Windows.Media.Imaging.BitmapSource LoadBitmap(string filename)` |
| `OleContainerModel` | `CleanStorage` | `public void CleanStorage(bool deleteFile)` |
| `OleContainerModel` | `SetRegionPersistentData` | `public abstract global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::` |
| `OleContainerModel` | `GetRegionPersistentData` | `public abstract TPersistentData GetRegionPersistentData(global::Quire.Serialization.IS11NProvider s11NProvider, string uniqueId, global::Sys` |
| `OleContainerModel` | `Pack` | `protected string Pack(global::Quire.Serialization.IS11NProvider s11NProvider, string uniqueId, global::Quire.Serialization.ExternalObjectTyp` |
| `OleContainerModel` | `Unpack` | `protected void Unpack(global::Quire.Serialization.IDeS11NProvider d11NProvider, string itemidref)` |
| `OleContainerModel` | `Lock` | `public bool Lock(bool wait, bool checkOleActiveState)` |
| `OleContainerModel` | `Unlock` | `public void Unlock()` |
| `OleContainerModel` | `FireComponentUnlocked` | `private void FireComponentUnlocked()` |
| `OleContainerModel` | `TryDoingActionWithLock` | `protected bool TryDoingActionWithLock(global::System.Action action, bool checkOleActiveState, bool mustTakeLock = true)` |
| `OleContainerModel` | `SetMessage` | `public void SetMessage(global::Quire.Controls.Core.AttachedBehavior.MessageCategory category, global::Quire.Controls.Core.NodeMessageAdorner` |
| `OleContainerModel` | `ClearMessages` | `public void ClearMessages()` |
| `OleContainerModel` | `OnPropertyChanged` | `private void OnPropertyChanged(string propName)` |

### `Ole.OleContainerViewModel_3` — 83

| Type | Member | Declaration |
|---|---|---|
| `OleContainerViewModel` | `Model` | `protected readonly TModel Model;` |
| `OleContainerViewModel` | `_forceGrayVisibility` | `private bool _forceGrayVisibility;` |
| `OleContainerViewModel` | `_maxHeight` | `private double _maxHeight;` |
| `OleContainerViewModel` | `_maxWidth` | `private double _maxWidth;` |
| `OleContainerViewModel` | `PublicModel` | `public TIPublicModel PublicModel` |
| `OleContainerViewModel` | `IsOleSiteRunning` | `public bool IsOleSiteRunning` |
| `OleContainerViewModel` | `MustTakeLockOnUserDeactivation` | `protected virtual bool MustTakeLockOnUserDeactivation` |
| `OleContainerViewModel` | `IsOutOfPlaceActive` | `public bool IsOutOfPlaceActive` |
| `OleContainerViewModel` | `IsCreated` | `public bool IsCreated` |
| `OleContainerViewModel` | `MessageList` | `public global::Quire.Controls.Core.AttachedBehavior.MessageList MessageList` |
| `OleContainerViewModel` | `AutoResize` | `public bool AutoResize` |
| `OleContainerViewModel` | `OleViewportSize` | `public global::System.Windows.Size OleViewportSize` |
| `OleContainerViewModel` | `AutomaticSize` | `public global::System.Windows.Size AutomaticSize` |
| `OleContainerViewModel` | `SiteCreated` | `public bool SiteCreated` |
| `OleContainerViewModel` | `WorksheetName` | `public string WorksheetName` |
| `OleContainerViewModel` | `DisplayFilePath` | `private string DisplayFilePath` |
| `OleContainerViewModel` | `GrayVisibility` | `public global::System.Windows.Visibility GrayVisibility` |
| `OleContainerViewModel` | `SizeDataForXamlSerialization` | `public global::Quire.PersistentData.IOleSizeData SizeDataForXamlSerialization` |
| `OleContainerViewModel` | `Height` | `public double Height` |
| `OleContainerViewModel` | `Width` | `public double Width` |
| `OleContainerViewModel` | `DefaultMinSize` | `public global::System.Windows.Size DefaultMinSize` |
| `OleContainerViewModel` | `MinSizeValue` | `internal global::System.Windows.Size MinSizeValue` |
| `OleContainerViewModel` | `IsStrictMinSize` | `protected virtual bool IsStrictMinSize` |
| `OleContainerViewModel` | `IsOleAppSupportZoom` | `public bool IsOleAppSupportZoom` |
| `OleContainerViewModel` | `MinSize` | `public virtual global::System.Windows.Size MinSize` |
| `OleContainerViewModel` | `MinHeight` | `public double MinHeight` |
| `OleContainerViewModel` | `MinWidth` | `public double MinWidth` |
| `OleContainerViewModel` | `MaxHeight` | `public double MaxHeight` |
| `OleContainerViewModel` | `MaxWidth` | `public double MaxWidth` |
| `OleContainerViewModel` | `CanToggleAutoResize` | `public bool CanToggleAutoResize` |
| `OleContainerViewModel` | `CanResetSize` | `public bool CanResetSize` |
| `OleContainerViewModel` | `CanUpdateLink` | `public bool CanUpdateLink` |
| `OleContainerViewModel` | `HasFakeOleViewportSize` | `protected bool HasFakeOleViewportSize` |
| `OleContainerViewModel` | `ViewChangedSync` | `public event global::System.Action ViewChangedSync;` |
| `OleContainerViewModel` | `ViewChangedAsync` | `public event global::System.Action ViewChangedAsync;` |
| `OleContainerViewModel` | `SavedSync` | `public event global::System.Action SavedSync;` |
| `OleContainerViewModel` | `SavedAsync` | `public event global::System.Action SavedAsync;` |
| `OleContainerViewModel` | `RequestResizeToFit` | `public event global::System.Action RequestResizeToFit;` |
| `OleContainerViewModel` | `BeforeActiveStateToggled` | `public event global::System.Action BeforeActiveStateToggled;` |
| `OleContainerViewModel` | `HandleModelPropertyChanged` | `private void HandleModelPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs args)` |
| `OleContainerViewModel` | `UpdateOleSizeIfNeeded` | `public void UpdateOleSizeIfNeeded()` |
| `OleContainerViewModel` | `UpdateContainerSizeIfNeeded` | `private void UpdateContainerSizeIfNeeded()` |
| `OleContainerViewModel` | `OnDispose` | `protected override void OnDispose()` |
| `OleContainerViewModel` | `ToggleOutOfPlaceActiveState` | `protected void ToggleOutOfPlaceActiveState()` |
| `OleContainerViewModel` | `AbortDeactivate` | `public void AbortDeactivate()` |
| `OleContainerViewModel` | `CreateHBitmap` | `internal nint CreateHBitmap(global::System.Windows.Int32Rect rect, nint winHandle)` |
| `OleContainerViewModel` | `ClearMessage` | `public void ClearMessage(global::Quire.Controls.Core.AttachedBehavior.MessageCategory category)` |
| `OleContainerViewModel` | `SetMessage` | `public void SetMessage(global::Quire.Controls.Core.AttachedBehavior.MessageCategory category, global::Quire.Controls.Core.NodeMessageAdorner` |
| `OleContainerViewModel` | `OnOleViewChangedAsync` | `protected virtual void OnOleViewChangedAsync()` |
| `OleContainerViewModel` | `OnOleSavedAsync` | `protected virtual void OnOleSavedAsync()` |
| `OleContainerViewModel` | `OnOleEditingStartedAsync` | `protected virtual void OnOleEditingStartedAsync()` |
| `OleContainerViewModel` | `OnOleEditingEndedAsync` | `protected virtual void OnOleEditingEndedAsync()` |
| `OleContainerViewModel` | `OnOleViewChangedSync` | `protected virtual void OnOleViewChangedSync()` |
| `OleContainerViewModel` | `OnOleSavedSync` | `protected virtual void OnOleSavedSync()` |
| `OleContainerViewModel` | `OleForceClose` | `internal void OleForceClose()` |
| `OleContainerViewModel` | `SetRegionPersistentData` | `public virtual global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::Q` |
| `OleContainerViewModel` | `GetRegionPersistentData` | `public virtual TPersistentData GetRegionPersistentData(global::Quire.Serialization.IS11NProvider s11NProvider, string uniqueId, global::Syst` |
| `OleContainerViewModel` | `ForceGrayVisibility` | `internal void ForceGrayVisibility()` |
| `OleContainerViewModel` | `FireGrayVisibilityChanged` | `private void FireGrayVisibilityChanged()` |
| `OleContainerViewModel` | `CreateOleWithDialog` | `public virtual bool CreateOleWithDialog(nint winHandler, string dialogTitle, string oleContainerPath, out bool isNew, out bool isLink, out b` |
| `OleContainerViewModel` | `CreateOleFromProgId` | `public virtual bool CreateOleFromProgId(string progId, bool activate, string oleContainerPath)` |
| `OleContainerViewModel` | `CreateOleFromFile` | `public virtual bool CreateOleFromFile(string filePath, bool link, string oleContainerPath)` |
| `OleContainerViewModel` | `CreateWindowlessOle` | `public bool CreateWindowlessOle()` |
| `OleContainerViewModel` | `PostCreationWindowless` | `protected virtual void PostCreationWindowless()` |
| `OleContainerViewModel` | `PostCreation` | `private void PostCreation(bool activate, bool isLink, bool isNew)` |
| `OleContainerViewModel` | `PostCreation` | `protected virtual void PostCreation(bool isNew)` |
| `OleContainerViewModel` | `UpdateLink` | `public void UpdateLink()` |
| `OleContainerViewModel` | `EstablishInitialSize` | `public abstract global::System.Windows.Size EstablishInitialSize();` |
| `OleContainerViewModel` | `OnWorksheetPropertiesChanged` | `public virtual void OnWorksheetPropertiesChanged(string[] propertyNames, global::Quire.Controls.Core.IWorksheetProperties oldProperties, glo` |
| `OleContainerViewModel` | `ApplySizeRestrictions` | `protected global::System.Windows.Size ApplySizeRestrictions(global::System.Windows.Size size)` |
| `OleContainerViewModel` | `CoerceSize` | `protected global::System.Windows.Size CoerceSize(global::System.Windows.Size size, double diagonal)` |
| `OleContainerViewModel` | `CoerceAndUpdateSize` | `private void CoerceAndUpdateSize(global::System.Windows.Size size, double diagonal)` |
| `OleContainerViewModel` | `CalculateDiagonalLength` | `protected double CalculateDiagonalLength(global::System.Windows.Size size)` |
| `OleContainerViewModel` | `CalculateSize` | `protected global::System.Windows.Size CalculateSize(global::System.Windows.Size size, double diagonal)` |
| `PropertyNames` | `AutoResize` | `public const string AutoResize = "AutoResize";` |
| `PropertyNames` | `AutomaticSize` | `public const string AutomaticSize = "AutomaticSize";` |
| `PropertyNames` | `GrayVisibility` | `public const string GrayVisibility = "GrayVisibility";` |
| `PropertyNames` | `Height` | `public const string Height = "Height";` |
| `PropertyNames` | `Width` | `public const string Width = "Width";` |
| `PropertyNames` | `MinHeight` | `public const string MinHeight = "MinHeight";` |
| `PropertyNames` | `MinWidth` | `public const string MinWidth = "MinWidth";` |
| `PropertyNames` | `MaxHeight` | `public const string MaxHeight = "MaxHeight";` |
| `PropertyNames` | `MaxWidth` | `public const string MaxWidth = "MaxWidth";` |

### `RtfDocConstructor` — 11

| Type | Member | Declaration |
|---|---|---|
| `RtfDocConstructor` | `DefaultFont` | `private const string DefaultFont = "Tahoma";` |
| `RtfDocConstructor` | `ImageOffsetForDefaultFont` | `internal const int ImageOffsetForDefaultFont = -27;` |
| `RtfDocConstructor` | `_orderedRtfItems` | `private readonly global::System.Collections.Generic.List<global::Quire.Controls.Core.IRtfItemBase> _orderedRtfItems;` |
| `RtfDocConstructor` | `_isSavingWholeDocument` | `private readonly bool _isSavingWholeDocument;` |
| `RtfDocConstructor` | `_startingPageIndex` | `private readonly int _startingPageIndex;` |
| `RtfDocConstructor` | `_leftTrim` | `private readonly double _leftTrim;` |
| `RtfDocConstructor` | `_contentWidth` | `private readonly double _contentWidth;` |
| `RtfDocConstructor` | `_pageSize` | `private readonly global::System.Windows.Size _pageSize;` |
| `RtfDocConstructor` | `_pageBodySize` | `private readonly global::System.Windows.Size _pageBodySize;` |
| `RtfDocConstructor` | `ImprovedColorMatchingDictionary` | `public static global::System.Collections.Generic.Dictionary<int, global::System.Drawing.Color> ImprovedColorMatchingDictionary;` |
| `RtfDocConstructor` | `ComputeIsOnePageContent` | `private void ComputeIsOnePageContent()` |

### `Serialization.McdxDeserializer` — 16

| Type | Member | Declaration |
|---|---|---|
| `McdxDeserializer` | `_deserializationStrategy` | `private readonly global::Quire.Controls.Core.Serialization.IDeserializationStrategy _deserializationStrategy;` |
| `McdxDeserializer` | `_regionCollectionSerializer` | `private readonly global::Quire.PersistentData.IRegionCollectionSerializer _regionCollectionSerializer;` |
| `McdxDeserializer` | `_worksheetNodeName` | `private readonly string _worksheetNodeName;` |
| `McdxDeserializer` | `Deserialize` | `public void Deserialize(global::System.IO.Stream stream)` |
| `McdxDeserializer` | `RepairXmlDocument` | `private void RepairXmlDocument(global::System.Xml.XmlDocument xmlContentDocument)` |
| `McdxDeserializer` | `RepairNumberFormatPrecision` | `private void RepairNumberFormatPrecision(global::System.Xml.XmlDocument xmlContentDocument)` |
| `McdxDeserializer` | `RepairPictureElements` | `private void RepairPictureElements(global::System.Xml.XmlDocument xmlContentDocument)` |
| `McdxDeserializer` | `RepairPictureElementIfNeed` | `private static void RepairPictureElementIfNeed(global::System.Xml.XmlNode region)` |
| `McdxDeserializer` | `ThrowIncorrectSerializationFormatException` | `protected void ThrowIncorrectSerializationFormatException(global::System.Exception ex)` |
| `McdxDeserializer` | `.ctor` | `public McdxDeserializer(string filePath, global::System.IO.Packaging.PackagePart part, global::Quire.Controls.Core.Serialization.IDeserializ` |
| `McdxDeserializer` | `Deserialize` | `public void Deserialize(global::System.Xml.XmlDocument xmlContentDocument)` |
| `McdxDeserializer` | `UnpackExternalObject` | `public string UnpackExternalObject(string itemIdRef)` |
| `McdxDeserializer` | `UnpackProtectedPart` | `public global::System.IO.Stream UnpackProtectedPart(string itemIdRef)` |
| `McdxDeserializer` | `UnpackFlowDocument` | `public void UnpackFlowDocument(ref global::System.Windows.Documents.FlowDocument flowDocument, string itemIdRef)` |
| `McdxDeserializer` | `ThrowFirstRegionDeserializationExceptionIfExists` | `public void ThrowFirstRegionDeserializationExceptionIfExists()` |
| `McdxDeserializer` | `Dispose` | `public void Dispose()` |

### `Serialization.McdxSerializer` — 11

| Type | Member | Declaration |
|---|---|---|
| `McdxSerializer` | `_onWriteValidationFailed` | `private readonly global::System.Action<global::System.Xml.Schema.XmlSchemaException> _onWriteValidationFailed;` |
| `McdxSerializer` | `_serializationStrategy` | `private readonly global::Quire.Controls.Core.Serialization.ISerializationStrategy _serializationStrategy;` |
| `McdxSerializer` | `_regionCollectionSerializer` | `private readonly global::Quire.PersistentData.IRegionCollectionSerializer _regionCollectionSerializer;` |
| `McdxSerializer` | `_worksheetNodeName` | `private readonly string _worksheetNodeName;` |
| `McdxSerializer` | `IsConverterApplication` | `public bool IsConverterApplication` |
| `McdxSerializer` | `.ctor` | `public McdxSerializer(global::System.IO.Packaging.PackagePart part, global::Quire.Controls.Core.Serialization.ISerializationStrategy seriali` |
| `McdxSerializer` | `OnWriteValidationFailedHandler` | `private void OnWriteValidationFailedHandler(object sender, global::System.Xml.Schema.ValidationEventArgs args)` |
| `McdxSerializer` | `PackFile` | `public static string PackFile(global::System.IO.Packaging.PackagePart sourcePackagePart, string sourceFileName, string pathInPackage, string` |
| `McdxSerializer` | `PackExternalObject` | `public string PackExternalObject(string sourceFileName, string objectUniqueId, global::Quire.Serialization.ExternalObjectType objectType)` |
| `McdxSerializer` | `PackFlowDocument` | `public string PackFlowDocument(global::System.Windows.Documents.FlowDocument flowDocument, string regionId)` |
| `McdxSerializer` | `InitializeDocument` | `private static void InitializeDocument(global::System.Xml.XmlDocument xmlContentDocument)` |

### `Serialization.NumberFormatSerializer` — 37

| Type | Member | Declaration |
|---|---|---|
| `NumberFormatSerializer` | `_numberFormat` | `private readonly global::Quire.Controls.Core.NumberFormat _numberFormat;` |
| `NumberFormatSerializer` | `_nameSpace` | `private readonly string _nameSpace;` |
| `NumberFormatSerializer` | `roundTripDecimalPlaces` | `public const string roundTripDecimalPlaces = "round-trip-decimal-places";` |
| `NumberFormatSerializer` | `NumberStyle` | `private global::Quire.Controls.Core.NumberFormat.NumberStyles NumberStyle` |
| `NumberFormatSerializer` | `ShowTrailingZeros` | `private string ShowTrailingZeros` |
| `NumberFormatSerializer` | `Precision` | `private string Precision` |
| `NumberFormatSerializer` | `ZeroThreshold` | `private string ZeroThreshold` |
| `NumberFormatSerializer` | `ExponentialThreshold` | `private string ExponentialThreshold` |
| `NumberFormatSerializer` | `ComplexThreshold` | `private string ComplexThreshold` |
| `NumberFormatSerializer` | `UseENotation` | `private string UseENotation` |
| `NumberFormatSerializer` | `ExponentStyleIsENotation` | `private bool ExponentStyleIsENotation` |
| `NumberFormatSerializer` | `ImaginaryValue` | `private string ImaginaryValue` |
| `NumberFormatSerializer` | `RadixStyle` | `private string RadixStyle` |
| `NumberFormatSerializer` | `ResultFormatImaginaryValue` | `private global::Quire.PersistentDataObjects.resultFormatGeneralTypeImaginaryvalue ResultFormatImaginaryValue` |
| `NumberFormatSerializer` | `.ctor` | `public NumberFormatSerializer(global::Quire.Controls.Core.NumberFormat numberFormat, string nameSpace)` |
| `NumberFormatSerializer` | `ReadXml` | `public void ReadXml(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadGeneralXml` | `private void ReadGeneralXml(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadScientificXml` | `private void ReadScientificXml(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadEngineeringXml` | `private void ReadEngineeringXml(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadPercentXml` | `private void ReadPercentXml(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadShowTrailingZeros` | `private void ReadShowTrailingZeros(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadPrecision` | `private void ReadPrecision(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadZeroThreshold` | `private void ReadZeroThreshold(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadExponentialThreshold` | `private void ReadExponentialThreshold(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadComplexThreshold` | `private void ReadComplexThreshold(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadFractionalXml` | `private void ReadFractionalXml(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadDecimalXml` | `private void ReadDecimalXml(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadExpXml` | `private void ReadExpXml(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `ReadRadix` | `private void ReadRadix(global::System.Xml.XmlReader reader)` |
| `NumberFormatSerializer` | `WriteXml` | `public void WriteXml(global::System.Xml.XmlWriter writer)` |
| `NumberFormatSerializer` | `WriteGeneral` | `private void WriteGeneral(global::System.Xml.XmlWriter writer)` |
| `NumberFormatSerializer` | `WritePercent` | `private void WritePercent(global::System.Xml.XmlWriter writer)` |
| `NumberFormatSerializer` | `WriteDecimal` | `private void WriteDecimal(global::System.Xml.XmlWriter writer)` |
| `NumberFormatSerializer` | `WriteScientific` | `private void WriteScientific(global::System.Xml.XmlWriter writer)` |
| `NumberFormatSerializer` | `WriteEngineering` | `private void WriteEngineering(global::System.Xml.XmlWriter writer)` |
| `NumberFormatSerializer` | `GetDataObject` | `public global::Quire.PersistentDataObjects.resultFormatType GetDataObject(global::Quire.Calculation.Core.IResultManager resultManager)` |
| `NumberFormatSerializer` | `SetDataObject` | `public void SetDataObject(global::Quire.PersistentDataObjects.resultFormatType resultFormat, global::Quire.Calculation.Core.IResultManager r` |

### `Serialization.ResultSerializer` — 41

| Type | Member | Declaration |
|---|---|---|
| `ResultSerializer` | `ErrorId` | `private const string ErrorId = "error-id";` |
| `ResultSerializer` | `NodeHandleAttr` | `private const string NodeHandleAttr = "error-node-handle";` |
| `ResultSerializer` | `Argument` | `private const string Argument = "errorArgument";` |
| `ResultSerializer` | `ResourceStringElement` | `private const string ResourceStringElement = "resource-string";` |
| `ResultSerializer` | `OriginalEngineErrorIdAttr` | `private const string OriginalEngineErrorIdAttr = "previous-error-id";` |
| `ResultSerializer` | `ErrorCodeElem` | `private const string ErrorCodeElem = "errorCode";` |
| `ResultSerializer` | `ResultData` | `private const string ResultData = "resultData";` |
| `ResultSerializer` | `Result` | `public const string Result = "result";` |
| `ResultSerializer` | `Trace2dResult` | `public const string Trace2dResult = "Trace2dResult";` |
| `ResultSerializer` | `SymbolicResult` | `private const string SymbolicResult = "symResult";` |
| `ResultSerializer` | `CalculationStatusAttr` | `private const string CalculationStatusAttr = "calculation-status";` |
| `ResultSerializer` | `ResultIdAttr` | `private const string ResultIdAttr = "result-id";` |
| `ResultSerializer` | `ResultTagAttr` | `private const string ResultTagAttr = "tag";` |
| `ResultSerializer` | `EngineErrorElem` | `private const string EngineErrorElem = "engineError";` |
| `ResultSerializer` | `EngineErrorsElem` | `private const string EngineErrorsElem = "engineErrors";` |
| `ResultSerializer` | `WarningsElem` | `private const string WarningsElem = "Warnings";` |
| `ResultSerializer` | `WarningElem` | `private const string WarningElem = "Warning";` |
| `ResultSerializer` | `NodeId` | `private const string NodeId = "nodeId";` |
| `ResultSerializer` | `OldType` | `private const string OldType = "oldType";` |
| `ResultSerializer` | `OldLocation` | `private const string OldLocation = "oldLocation";` |
| `ResultSerializer` | `_xmcdToResultNodeTransform` | `private static readonly global::Quire.Calculation.Core.XmcdToIResultNodeTransform _xmcdToResultNodeTransform;` |
| `ResultSerializer` | `_nextKey` | `private int _nextKey;` |
| `ResultSerializer` | `AddError` | `private int AddError(global::Quire.Calculation.Core.EngineError error)` |
| `ResultSerializer` | `FindError` | `private global::Quire.Calculation.Core.EngineError FindError(int id)` |
| `ResultSerializer` | `FindError` | `private int FindError(global::Quire.Calculation.Core.EngineError error)` |
| `ResultSerializer` | `WriteXml` | `public void WriteXml(global::System.Xml.Linq.XElement xElement)` |
| `ResultSerializer` | `WriteXml` | `public void WriteXml(global::System.Xml.Linq.XElement xElement, global::Quire.Calculation.Core.IResultManager resultManager)` |
| `ResultSerializer` | `AddResultToElement` | `private static void AddResultToElement(global::Quire.Calculation.Core.IResultNode node, global::System.Xml.XmlWriter writer)` |
| `ResultSerializer` | `WriteEngineErrors` | `private void WriteEngineErrors(global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.EngineError> errors, global::Sy` |
| `ResultSerializer` | `WriteWarning` | `private void WriteWarning(global::Quire.Calculation.Core.EngineRedefInfo warningRedefInfo, global::System.Xml.XmlWriter writer)` |
| `ResultSerializer` | `WriteEngineError` | `private void WriteEngineError(global::Quire.Calculation.Core.EngineError error, global::System.Xml.XmlWriter writer)` |
| `ResultSerializer` | `WriteElement` | `private static void WriteElement(string name, string valueString, global::System.Xml.XmlWriter writer)` |
| `ResultSerializer` | `RemoveNsFromChildren` | `private static void RemoveNsFromChildren(global::System.Xml.Linq.XElement element)` |
| `ResultSerializer` | `ReadXml` | `public void ReadXml(global::System.Xml.Linq.XElement xElement)` |
| `ResultSerializer` | `ReadXml` | `public void ReadXml(global::System.Xml.Linq.XElement xElement, global::Quire.Calculation.Core.IResultManager resultManager)` |
| `ResultSerializer` | `ReadWarning` | `private global::Quire.Calculation.Core.EngineRedefInfo ReadWarning(global::System.Xml.Linq.XElement warningXElement, int resultId)` |
| `ResultSerializer` | `ReadEngineErrors` | `private global::Quire.Calculation.Core.EngineErrorCollection ReadEngineErrors(global::System.Xml.Linq.XContainer element, int resultId)` |
| `ResultSerializer` | `ReadEngineError` | `private global::Quire.Calculation.Core.EngineError ReadEngineError(global::System.Xml.Linq.XElement element, int resultId)` |
| `ResultSerializer` | `ReadElement` | `private static string ReadElement(global::System.Xml.Linq.XContainer errorElement, string name)` |
| `ResultSerializer` | `ReadXAttribute` | `private static string ReadXAttribute(global::System.Xml.Linq.XElement errorElement, string attributeName)` |
| `ResultSerializer` | `.ctor` | `public ResultSerializer()` |

### `Serialization.SerializationHelper` — 24

| Type | Member | Declaration |
|---|---|---|
| `SerializationHelper` | `ConvertedFromProperty` | `public static readonly global::System.Windows.DependencyProperty ConvertedFromProperty;` |
| `SerializationHelper` | `_serializedSchemaVersions` | `private global::System.Collections.Generic.Dictionary<global::Quire.Xml.SchemasManager.SchemaNameEnum, global::System.Version> _serializedSc` |
| `SerializationHelper` | `RegionIdProperty` | `public static readonly global::System.Windows.DependencyProperty RegionIdProperty;` |
| `SerializationHelper` | `_regionId` | `private long _regionId;` |
| `SerializationHelper` | `_imagesHashTable` | `private readonly global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<byte[], global::Quire.P` |
| `SerializationHelper` | `_byteArraysComparer` | `private static readonly global::Quire.Controls.Core.Serialization.SerializationHelper.BytesComparer _byteArraysComparer;` |
| `SerializationHelper` | `_cachedWorksheetsTable` | `private readonly global::System.Collections.Generic.Dictionary<global::Quire.Pair<string, global::System.DateTime>, global::Quire.Pair<strin` |
| `SerializationHelper` | `SerializedSchemaVersions` | `public global::System.Collections.Generic.IDictionary<global::Quire.Xml.SchemasManager.SchemaNameEnum, global::System.Version> SerializedSch` |
| `SerializationHelper` | `ErrorsOccurredWhenLoadingRegions` | `public bool ErrorsOccurredWhenLoadingRegions` |
| `SerializationHelper` | `GetRegionId` | `public long GetRegionId(global::System.Windows.UIElement region)` |
| `SerializationHelper` | `AssignNextRegionId` | `public void AssignNextRegionId(global::System.Windows.UIElement region)` |
| `SerializationHelper` | `GetNextRegionId` | `public long GetNextRegionId()` |
| `SerializationHelper` | `SetRegionId` | `public void SetRegionId(global::System.Windows.UIElement region, long regionId)` |
| `SerializationHelper` | `GetRegionById` | `public global::System.Windows.UIElement GetRegionById(global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> regio` |
| `SerializationHelper` | `CleanSerializerTempData` | `public void CleanSerializerTempData(global::System.Collections.Generic.IList<global::System.Windows.UIElement> regions)` |
| `SerializationHelper` | `Reset` | `public void Reset()` |
| `SerializationHelper` | `AddImagePath` | `public void AddImagePath(string imageType, byte[] imageHash, global::Quire.Pair<string, global::System.Uri> imagePath)` |
| `SerializationHelper` | `GetImagePath` | `public global::Quire.Pair<string, global::System.Uri> GetImagePath(string imageType, byte[] imageHash)` |
| `SerializationHelper` | `GetCachedWorksheetPath` | `public global::Quire.Pair<string, global::System.Uri> GetCachedWorksheetPath(global::Quire.Pair<string, global::System.DateTime> fullFilePat` |
| `SerializationHelper` | `AddCachedPartPath` | `public void AddCachedPartPath(global::Quire.Pair<string, global::System.DateTime> fullFilePathAndTimeStamp, global::Quire.Pair<string, globa` |
| `SerializationHelper` | `OptionalPartHasErrors` | `private static bool OptionalPartHasErrors(global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> elemen` |
| `SerializationHelper` | `CompulsoryPartHasErrors` | `private static bool CompulsoryPartHasErrors(global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> elem` |
| `SerializationHelper` | `CleanupRegions` | `private void CleanupRegions()` |
| `SerializationHelper` | `.ctor` | `public SerializationHelper()` |

### `TextStylesCollectionManager` — 17

| Type | Member | Declaration |
|---|---|---|
| `TextStylesCollectionManager` | `_stylesCollection` | `private global::System.Collections.ObjectModel.ObservableCollection<global::Quire.TextStyleData> _stylesCollection;` |
| `TextStylesCollectionManager` | `StyleResourceKey` | `private const string StyleResourceKey = "Style.";` |
| `TextStylesCollectionManager` | `StylesCollection` | `public global::System.Collections.ObjectModel.ObservableCollection<global::Quire.TextStyleData> StylesCollection` |
| `TextStylesCollectionManager` | `.ctor` | `public TextStylesCollectionManager()` |
| `TextStylesCollectionManager` | `StyleCreateRequest` | `public string StyleCreateRequest(global::Quire.Core.TextFormat arg)` |
| `TextStylesCollectionManager` | `FirePropertyChanged` | `private void FirePropertyChanged(string propertyName)` |
| `TextStylesCollectionManager` | `CreateAndAddStyle` | `public string CreateAndAddStyle(string styleName, string styleKey, global::System.Windows.Style style, string basedOnKey)` |
| `TextStylesCollectionManager` | `RenameStyleNameToCounter` | `private string RenameStyleNameToCounter(string styleName)` |
| `TextStylesCollectionManager` | `LoadWorkSheetStyles` | `public void LoadWorkSheetStyles(global::System.Collections.Generic.IReadOnlyDictionary<string, global::Quire.ITextStyleObject> textStyles)` |
| `TextStylesCollectionManager` | `LoadWorkSheetStylesFromFile` | `protected void LoadWorkSheetStylesFromFile(global::System.Collections.Generic.SortedList<string, global::Quire.TextStyleData> stylesCollecti` |
| `TextStylesCollectionManager` | `LoadWorkSheetLegacyStyles` | `protected void LoadWorkSheetLegacyStyles(global::System.Collections.Generic.IList<global::Quire.TextStyleData> stylesCollection, global::Sys` |
| `TextStylesCollectionManager` | `IsUniqueTextStyleDataName` | `public bool IsUniqueTextStyleDataName(string textStyleName)` |
| `TextStylesCollectionManager` | `IsUniqueStyleObject` | `public bool IsUniqueStyleObject(global::System.Windows.Style textStyle)` |
| `TextStylesCollectionManager` | `RemoveStyle` | `public void RemoveStyle(global::Quire.TextStyleData textStyleData)` |
| `TextStylesCollectionManager` | `UpdateTextStyleData` | `public void UpdateTextStyleData(string name, string key, global::System.Windows.Style style, string basedOnKey)` |
| `TextStylesCollectionManager` | `UpdateTextStyleDataBasedOn` | `private void UpdateTextStyleDataBasedOn(global::Quire.TextStyleData updatedTextStyleData, global::System.Collections.Generic.ICollection<glo` |
| `TextStylesCollectionManager` | `KeyIsBasedOnKey` | `private void KeyIsBasedOnKey(string styleKey, global::Quire.TextStyleData textStyleData, global::System.Collections.Generic.ICollection<glob` |

### `UIElementFacade` — 16

| Type | Member | Declaration |
|---|---|---|
| `UIElementFacade` | `SetFocus` | `public bool SetFocus(object obj)` |
| `UIElementFacade` | `Measure` | `public void Measure(object obj, global::System.Windows.Size availableSize)` |
| `UIElementFacade` | `Arrange` | `public void Arrange(object obj, global::System.Windows.Rect finalRect)` |
| `UIElementFacade` | `GetRenderSize` | `public global::System.Windows.Size GetRenderSize(object obj)` |
| `UIElementFacade` | `GetDesiredSize` | `public global::System.Windows.Size GetDesiredSize(object obj)` |
| `UIElementFacade` | `GetDescendantBounds` | `public virtual global::System.Windows.Rect GetDescendantBounds(object obj)` |
| `UIElementFacade` | `IsItemMeasureValid` | `public bool IsItemMeasureValid(object obj)` |
| `UIElementFacade` | `IsItemArrangeValid` | `public bool IsItemArrangeValid(object obj)` |
| `UIElementFacade` | `IsRectEmpty` | `private static bool IsRectEmpty(global::System.Windows.Rect rect)` |
| `UIElementFacade` | `SetRenderTransform` | `public void SetRenderTransform(object obj, global::System.Windows.Media.Transform transform)` |
| `UIElementFacade` | `TranslatePoint` | `public global::System.Windows.Point TranslatePoint(object source, object target, global::System.Windows.Point sourcePoint)` |
| `UIElementFacade` | `TransformBoundsToAncestor` | `public global::System.Windows.Rect TransformBoundsToAncestor(object descendant, object ancestor, global::System.Windows.Rect bounds)` |
| `UIElementFacade` | `BlockUntilLoaded` | `public void BlockUntilLoaded()` |
| `UIElementFacade` | `BlockUntilInput` | `public void BlockUntilInput()` |
| `UIElementFacade` | `GetUIElement` | `protected static global::System.Windows.UIElement GetUIElement(object obj)` |
| `UIElementFacade` | `ExecuteOnLayoutUpdated` | `public void ExecuteOnLayoutUpdated(object control, global::System.Action action)` |

### `VisualDragger` — 36

| Type | Member | Declaration |
|---|---|---|
| `VisualDragger` | `DraggedElementProperty` | `public static readonly global::System.Windows.DependencyProperty DraggedElementProperty;` |
| `VisualDragger` | `IsDraggingPropertyKey` | `private static readonly global::System.Windows.DependencyPropertyKey IsDraggingPropertyKey;` |
| `VisualDragger` | `IsDraggingProperty` | `public static readonly global::System.Windows.DependencyProperty IsDraggingProperty;` |
| `VisualDragger` | `DirectionProperty` | `public static readonly global::System.Windows.DependencyProperty DirectionProperty;` |
| `VisualDragger` | `ModeProperty` | `public static readonly global::System.Windows.DependencyProperty ModeProperty;` |
| `VisualDragger` | `_referenceElement` | `private global::System.Windows.IInputElement _referenceElement;` |
| `VisualDragger` | `DragEndEvent` | `public static readonly global::System.Windows.RoutedEvent DragEndEvent;` |
| `VisualDragger` | `DragStartEvent` | `public static readonly global::System.Windows.RoutedEvent DragStartEvent;` |
| `VisualDragger` | `DragEvent` | `public static readonly global::System.Windows.RoutedEvent DragEvent;` |
| `VisualDragger` | `_originalMargin` | `private global::System.Windows.Thickness _originalMargin;` |
| `VisualDragger` | `_originalSize` | `private global::System.Windows.Size _originalSize;` |
| `VisualDragger` | `DraggedElement` | `public global::System.Windows.FrameworkElement DraggedElement` |
| `VisualDragger` | `IsDragging` | `public bool IsDragging` |
| `VisualDragger` | `Direction` | `public global::Quire.Controls.Core.DraggingDirection Direction` |
| `VisualDragger` | `Mode` | `public global::Quire.Controls.Core.DraggingMode Mode` |
| `VisualDragger` | `ReferenceElement` | `public global::System.Windows.IInputElement ReferenceElement` |
| `VisualDragger` | `DragEnd` | `public event global::System.Windows.RoutedEventHandler DragEnd;` |
| `VisualDragger` | `DragStart` | `public event global::System.Windows.Input.MouseEventHandler DragStart;` |
| `VisualDragger` | `Drag` | `public event global::Quire.Controls.Core.VisualDragger.DragHandler Drag;` |
| `VisualDragger` | `.ctor` | `public VisualDragger()` |
| `VisualDragger` | `DirectionPropertyChanged` | `private static void DirectionPropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEven` |
| `VisualDragger` | `StartDrag` | `private void StartDrag(global::System.Windows.Input.MouseEventArgs e)` |
| `VisualDragger` | `StopDrag` | `private void StopDrag()` |
| `VisualDragger` | `CancelDrag` | `private void CancelDrag()` |
| `VisualDragger` | `DragInternal` | `private void DragInternal(global::System.Windows.Vector vector)` |
| `VisualDragger` | `CustomDrag` | `private void CustomDrag(global::System.Windows.Vector vector)` |
| `VisualDragger` | `CustomDragHorizontal` | `private void CustomDragHorizontal(global::System.Windows.Vector vector)` |
| `VisualDragger` | `CustomDragVertical` | `private void CustomDragVertical(global::System.Windows.Vector vector)` |
| `VisualDragger` | `DragInDirection` | `private void DragInDirection(global::System.Windows.Vector vector)` |
| `VisualDragger` | `DragMoveMargin` | `private void DragMoveMargin(global::System.Windows.Vector vector)` |
| `VisualDragger` | `SetMargin` | `private void SetMargin(global::System.Windows.Thickness newMargin)` |
| `VisualDragger` | `OnMouseLeftButtonDown` | `protected override void OnMouseLeftButtonDown(global::System.Windows.Input.MouseButtonEventArgs e)` |
| `VisualDragger` | `OnMouseLeftButtonUp` | `protected override void OnMouseLeftButtonUp(global::System.Windows.Input.MouseButtonEventArgs e)` |
| `VisualDragger` | `OnMouseMove` | `protected override void OnMouseMove(global::System.Windows.Input.MouseEventArgs e)` |
| `VisualDragger` | `OnLostMouseCapture` | `protected override void OnLostMouseCapture(global::System.Windows.Input.MouseEventArgs e)` |
| `VisualDragger` | `OnKeyDown` | `protected override void OnKeyDown(global::System.Windows.Input.KeyEventArgs e)` |

### `Whiteboard.ContainerItemManager` — 54

| Type | Member | Declaration |
|---|---|---|
| `ContainerItemManager` | `_typeOfThis` | `private static readonly global::System.Type _typeOfThis;` |
| `ContainerItemManager` | `MouseEnterHandlerProperty` | `public static global::System.Windows.DependencyProperty MouseEnterHandlerProperty;` |
| `ContainerItemManager` | `DragEnterHandlerProperty` | `public static global::System.Windows.DependencyProperty DragEnterHandlerProperty;` |
| `ContainerItemManager` | `WhiteboardItemSizeChangedHandlerProperty` | `public static global::System.Windows.DependencyProperty WhiteboardItemSizeChangedHandlerProperty;` |
| `ContainerItemManager` | `GetWhiteboardItemSizeChangedHandler` | `public static global::Quire.Controls.Core.Whiteboard.WhiteboardItemSizeChangedEventHandler GetWhiteboardItemSizeChangedHandler(global::Syste` |
| `ContainerItemManager` | `SetWhiteboardItemSizeChangedHandler` | `public static void SetWhiteboardItemSizeChangedHandler(global::System.Windows.DependencyObject depObj, global::Quire.Controls.Core.Whiteboar` |
| `ContainerItemManager` | `ClearWhiteboardItemSizeChangedHandler` | `public static void ClearWhiteboardItemSizeChangedHandler(global::System.Windows.DependencyObject depObj)` |
| `ContainerItemManager` | `SelectItem` | `public void SelectItem(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `DeselectItem` | `public void DeselectItem(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `IsItemSelected` | `public bool IsItemSelected(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `ActivateItem` | `public void ActivateItem(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `DeactivateItem` | `public void DeactivateItem(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `IsItemActivated` | `public bool IsItemActivated(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `SetIsTabNavigatable` | `public void SetIsTabNavigatable(global::Quire.Controls.Core.IWhiteboardItem item, bool isTabStop)` |
| `ContainerItemManager` | `GetIsTabNavigatable` | `public bool GetIsTabNavigatable(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `IsTabNavigatable` | `public bool IsTabNavigatable(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `TabInto` | `public void TabInto(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Input.FocusNavigationDirection dir)` |
| `ContainerItemManager` | `BringIntoView` | `public void BringIntoView(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Rect areaOfInterest)` |
| `ContainerItemManager` | `SetIsBringToView` | `public void SetIsBringToView(global::Quire.Controls.Core.IWhiteboardItem item, bool value)` |
| `ContainerItemManager` | `GetIsBringToView` | `public bool GetIsBringToView(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `IsSuppressBringIntoView` | `public bool IsSuppressBringIntoView(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `SuppressBringIntoViewIfNeededAndExecute` | `public void SuppressBringIntoViewIfNeededAndExecute(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Action action, bool sup` |
| `ContainerItemManager` | `UnhookSizeChanged` | `public void UnhookSizeChanged(global::Quire.Controls.Core.IWhiteboardItem item, global::Quire.Controls.Core.Whiteboard.WhiteboardItemSizeCha` |
| `ContainerItemManager` | `OnItemSizeChanged` | `protected virtual void OnItemSizeChanged(object sender, global::System.Windows.SizeChangedEventArgs args)` |
| `ContainerItemManager` | `GetItemSizeChangedHandler` | `protected virtual global::Quire.Controls.Core.Whiteboard.WhiteboardItemSizeChangedEventHandler GetItemSizeChangedHandler(global::Quire.Contr` |
| `ContainerItemManager` | `SetItemSizeChangedHandler` | `protected virtual void SetItemSizeChangedHandler(global::Quire.Controls.Core.IWhiteboardItem item, global::Quire.Controls.Core.Whiteboard.Wh` |
| `ContainerItemManager` | `ClearItemSizeChangedHandler` | `protected virtual void ClearItemSizeChangedHandler(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `UnhookMouseEnter` | `public void UnhookMouseEnter(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `UnhookDragEnter` | `public void UnhookDragEnter(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `ProhibitSizeChangedEventHandler` | `public void ProhibitSizeChangedEventHandler(global::Quire.Controls.Core.IWhiteboardItem item, bool prohibit)` |
| `ContainerItemManager` | `IsSizeChangedEventHandlerProhibited` | `public bool IsSizeChangedEventHandlerProhibited(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `GetItemHandlesContextMenus` | `public bool GetItemHandlesContextMenus(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `SetItemHandlesContextMenus` | `public void SetItemHandlesContextMenus(global::Quire.Controls.Core.IWhiteboardItem item, bool handlesContextMenus)` |
| `ContainerItemManager` | `ClearItemHandlesContextMenus` | `public void ClearItemHandlesContextMenus(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `GetCanActivate` | `public bool GetCanActivate(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `SetCanActivate` | `public void SetCanActivate(global::Quire.Controls.Core.IWhiteboardItem item, bool value)` |
| `ContainerItemManager` | `GetItemIsInCopy` | `public bool GetItemIsInCopy(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `SetItemIsInCopy` | `public void SetItemIsInCopy(global::Quire.Controls.Core.IWhiteboardItem item, bool value)` |
| `ContainerItemManager` | `ClearItemIsInCopy` | `public void ClearItemIsInCopy(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `SetActiveItemProperty` | `private void SetActiveItemProperty(global::Quire.Controls.Core.IWhiteboardItem item, bool isActive)` |
| `ContainerItemManager` | `SetItemSupportsInputsOutputs` | `public void SetItemSupportsInputsOutputs(global::Quire.Controls.Core.IWhiteboardItem item, bool visible)` |
| `ContainerItemManager` | `GetItemSupportsInputsOutputs` | `public bool GetItemSupportsInputsOutputs(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `ClearItemSupportsInputsOutputs` | `public void ClearItemSupportsInputsOutputs(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `SetItemIsOnVisiblePage` | `public void SetItemIsOnVisiblePage(global::Quire.Controls.Core.IWhiteboardItem item, bool visible)` |
| `ContainerItemManager` | `GetItemIsOnVisiblePage` | `public bool GetItemIsOnVisiblePage(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `ClearItemIsOnVisiblePage` | `public void ClearItemIsOnVisiblePage(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `SetElementIsOnVisiblePage` | `public void SetElementIsOnVisiblePage(global::Quire.Core.IMaybeHasPointThatSnapsToPixel item, bool isPageVisible)` |
| `ContainerItemManager` | `GetElementIsOnVisiblePage` | `public bool GetElementIsOnVisiblePage(global::Quire.Core.IMaybeHasPointThatSnapsToPixel item)` |
| `ContainerItemManager` | `AddItemIsOnVisiblePageChangedHandler` | `public void AddItemIsOnVisiblePageChangedHandler(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.DependencyProperty` |
| `ContainerItemManager` | `RemoveItemIsOnVisiblePageChangedHandler` | `public void RemoveItemIsOnVisiblePageChangedHandler(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.DependencyPrope` |
| `ContainerItemManager` | `SetContainedInCollapsedWhiteboard` | `public void SetContainedInCollapsedWhiteboard(global::Quire.Controls.Core.IWhiteboardItem item, bool isContainedInCollapsedWhiteboard)` |
| `ContainerItemManager` | `GetContainedInCollapsedWhiteboard` | `public bool GetContainedInCollapsedWhiteboard(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `ClearContainedInCollapsedWhiteboard` | `public void ClearContainedInCollapsedWhiteboard(global::Quire.Controls.Core.IWhiteboardItem item)` |
| `ContainerItemManager` | `.ctor` | `public ContainerItemManager()` |

### `WorksheetPropertiesManager` — 22

| Type | Member | Declaration |
|---|---|---|
| `WorksheetPropertiesManager` | `_typeOfThis` | `private static readonly global::System.Type _typeOfThis;` |
| `WorksheetPropertiesManager` | `GetWorksheetPropertiesSource` | `private static global::Quire.Controls.Core.IWorksheetPropertiesSource GetWorksheetPropertiesSource(global::System.Windows.DependencyObject d` |
| `WorksheetPropertiesManager` | `GetWorksheetPropertiesChangeInfo` | `private static global::Quire.Controls.Core.WorksheetPropertiesManager.WorksheetPropertiesChangeInfo GetWorksheetPropertiesChangeInfo(global:` |
| `WorksheetPropertiesManager` | `SetOrRemoveWorksheetPropertiesSource` | `public static void SetOrRemoveWorksheetPropertiesSource<TSource>(TSource propertiesSourceElement, bool doSet) where TSource : global::System` |
| `WorksheetPropertiesManager` | `SetOrRemoveWorksheetPropertiesSource` | `public static void SetOrRemoveWorksheetPropertiesSource(global::System.Windows.FrameworkElement worksheetElement, global::Quire.Controls.Cor` |
| `WorksheetPropertiesManager` | `RegisterMockWorksheetPropertiesSourcePropertyForType` | `public static void RegisterMockWorksheetPropertiesSourcePropertyForType(global::System.Type type, global::Quire.Controls.Core.IWorksheetProp` |
| `WorksheetPropertiesManager` | `GetWorksheetProperties` | `public static global::Quire.Controls.Core.IWorksheetProperties GetWorksheetProperties<TTargetVisual>(TTargetVisual visual) where TTargetVisu` |
| `WorksheetPropertiesManager` | `HasWorksheetProperties` | `public static bool HasWorksheetProperties<TTargetVisual>(TTargetVisual visual) where TTargetVisual : global::System.Windows.Media.Visual` |
| `WorksheetPropertiesManager` | `RegisterToWorksheetPropertyNotifications` | `public static void RegisterToWorksheetPropertyNotifications(global::System.Type type)` |
| `WorksheetPropertiesManager` | `GetWorksheetFrameworkElement` | `public static global::System.Windows.FrameworkElement GetWorksheetFrameworkElement<TTargetVisual>(TTargetVisual visual) where TTargetVisual ` |
| `WorksheetPropertiesManager` | `GetWorksheetContentControl` | `public static global::System.Windows.Controls.ContentControl GetWorksheetContentControl<TTargetVisual>(TTargetVisual visual) where TTargetVi` |
| `WorksheetPropertiesManager` | `OnWorksheetPropertyChanged` | `private static void OnWorksheetPropertyChanged(global::Quire.Controls.Core.WorksheetPropertiesManager.WorksheetPropertiesChangeInfo oldInfo,` |
| `WorksheetPropertiesManager` | `GetWorksheetPropertyChangeInfo` | `private static global::Quire.Controls.Core.WorksheetPropertiesManager.WorksheetPropertiesChangeInfo GetWorksheetPropertyChangeInfo<TTargetVi` |
| `WorksheetPropertiesManager` | `OnWorksheetPropertyChangeInfoChanged` | `private static void OnWorksheetPropertyChangeInfoChanged(global::System.Windows.DependencyObject dObj, global::System.Windows.DependencyProp` |
| `WorksheetPropertiesManager` | `UpdateTarget` | `private static void UpdateTarget(global::Quire.Controls.Core.IWorksheetPropertiesTarget propertyTarget, string[] propertyNames, global::Quir` |
| `WorksheetPropertiesChangeInfo` | `_cachedWorksheetProperties` | `private global::Quire.Controls.Core.IWorksheetPropertiesSource _cachedWorksheetProperties;` |
| `WorksheetPropertiesChangeInfo` | `CachedWorksheetProperties` | `public global::Quire.Controls.Core.IWorksheetPropertiesSource CachedWorksheetProperties` |
| `WorksheetPropertiesChangeInfo` | `.ctor` | `public WorksheetPropertiesChangeInfo(string propertyName, global::Quire.Controls.Core.IWorksheetPropertiesSource propertySource, global::Sys` |
| `WorksheetPropertiesChangeInfo` | `.ctor` | `public WorksheetPropertiesChangeInfo(string[] propertyNames, global::Quire.Controls.Core.IWorksheetPropertiesSource propertySource, global::` |
| `WorksheetPropertiesChangeInfo` | `RegisterToWorksheetPropertyChanges` | `public void RegisterToWorksheetPropertyChanges()` |
| `WorksheetPropertiesChangeInfo` | `UnregisterFromWorksheetPropertyChanges` | `public void UnregisterFromWorksheetPropertyChanges()` |
| `WorksheetPropertiesChangeInfo` | `OnWorksheetPropertyChanged` | `private void OnWorksheetPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs eArgs)` |

### `Controls.Text.CustomScriptRunBase` — 11

| Type | Member | Declaration |
|---|---|---|
| `CustomScriptRunBase` | `WpfRecreateScripts` | `public bool WpfRecreateScripts` |
| `CustomScriptRunBase` | `IsInSerialization` | `public bool IsInSerialization` |
| `CustomScriptRunBase` | `RevertCoerceAfterSerializationProcess` | `public bool RevertCoerceAfterSerializationProcess` |
| `CustomScriptRunBase` | `FontVariantsForCopy` | `protected abstract global::System.Windows.FontVariants FontVariantsForCopy` |
| `CustomScriptRunBase` | `FontSizeForReport` | `public double FontSizeForReport` |
| `CustomScriptRunBase` | `SetRevertCoerceAfterSerializationProcess` | `public void SetRevertCoerceAfterSerializationProcess()` |
| `CustomScriptRunBase` | `HandleAnySerializationChanged` | `private void HandleAnySerializationChanged(bool inSerialization)` |
| `CustomScriptRunBase` | `CoerceVariantsProperty` | `private static object CoerceVariantsProperty(global::System.Windows.DependencyObject depObj, object value)` |
| `CustomScriptRunBase` | `CoerceCustomScriptFontSizeCallback` | `private static object CoerceCustomScriptFontSizeCallback(global::System.Windows.DependencyObject depObj, object value)` |
| `CustomScriptRunBase` | `ScriptBaseRun_DataContextChanged` | `private static void ScriptBaseRun_DataContextChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs e)` |
| `CustomScriptRunBase` | `SetSpanPropToRunIfNeeded` | `private static void SetSpanPropToRunIfNeeded(global::System.Collections.Generic.List<global::System.Windows.Documents.Run> runsToConvert)` |
