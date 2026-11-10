# Lane Quire-main-rest — main Quire project (MathcadPrime.dll), folder group `rest`

- Owner: dispatched worker task_0b897f298aad (Orca session ab9881fc). Branch `mirror/quire-main-rest-20261110` from `main` (4b4fc5b61).
- Assembly: `Quire` target project ↔ `MathcadPrime.dll` (assembly-map layer 20). All 286 stub files in scope carry `MirrorOf("Spirit.* , MathcadPrime")`; no row in WORK.md owns this assembly.
- Type-map check: every in-scope type is `mathcad_assembly = MathcadPrime.dll` (the `AutomationServiceEngNote*` types included; they are not Ptc.EngNotebook* rows, so they are not lane C/G members).
- Scope: `src/Quire/Generated/Mirror/*.g.cs` excluding `Controls.*` (other worker). 3317 `[MirrorStub]` markers in 286 files.

## Step 1 — classification

Status legend (file level; the stub markers in each file share the status):

- **U** unblocked: pure code, no XAML/COM/resource dependency visible in the stub signatures. Batch candidates.
- **P** partly blocked: XAML-compiled type. `InitializeComponent`, `Connect` (IComponentConnector), `_contentLoaded` and XAML-named element fields are blocked (generated BAML loading and event wiring). Other members in the type are U-like and are taken per batch.
- **B** blocked: COM class factory / server registration / P/Invoke (native shape), or settings/product literal decisions (Mathcad `Settings.settings` values, product string constants).
- **D** deferred: Automation surface (`Spirit.Automation*`, `AutomationSpirit*`, `CMathcadPrime*`): COM event wiring, closure-heavy, depends on the Quire.Automation/Interop surfaces. Taken per member after the U batches.

Totals: U 127 files / 1021 markers; P 78 / 1336; B 16 / 126; D 65 / 834; all 286 files / 3317 markers.

| Stub file | Mathcad type | Markers | Status | Reason |
|---|---|---:|---|---|
| `AboutWindow.g.cs` | `Spirit.AboutWindow` | 8 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `Addon.g.cs` | `Spirit.Addon` | 36 | U |  |
| `AddonsCollection.g.cs` | `Spirit.AddonsCollection` | 42 | U |  |
| `AddonsControls.g.cs` | `Spirit.AddonsControls` | 20 | U |  |
| `AddonsLoaderHelper.g.cs` | `Spirit.AddonsLoaderHelper` | 3 | B | COM/native: class-factory/registration or P/Invoke shape; needs native COM decision |
| `AddonsProperties.g.cs` | `Spirit.AddonsProperties` | 26 | U |  |
| `AliasTextBox.g.cs` | `Spirit.AliasTextBox` | 3 | U |  |
| `App.g.cs` | `Spirit.App` | 15 | U |  |
| `AppCommands.g.cs` | `Spirit.AppCommands` | 40 | U |  |
| `ApplicationMenuControl.g.cs` | `Spirit.ApplicationMenuControl` | 29 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `Automation.SpiritMainWindowAutomationPeer.g.cs` | `Spirit.Automation.SpiritMainWindowAutomationPeer` | 1 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationGetValueResult.g.cs` | `Spirit.AutomationGetValueResult` | 9 | U |  |
| `AutomationService.AutomationSpiritAliasesList.g.cs` | `Spirit.AutomationService.AutomationSpiritAliasesList` | 5 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationGetValueResult.g.cs` | `Spirit.AutomationServiceEngNote.AutomationGetValueResult` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritInputMatrixResult.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritInputMatrixResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritInputResult.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritInputResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritInputs.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritInputs` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritInputsOutputsConflicts.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritInputsOutputsConflicts` | 6 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritInputsOutputsStates.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritInputsOutputsStates` | 14 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritMatrix.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritMatrix` | 10 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritMatrixResult.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritMatrixResult` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritOutputMatrixResult.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritOutputMatrixResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritOutputMatrixResultAs.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritOutputMatrixResultAs` | 3 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritOutputResult.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritOutputResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritOutputResultAs.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritOutputResultAs` | 6 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritOutputs.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritOutputs` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritResult.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritResult` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritSetValueResults.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritSetValueResults` | 5 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.AutomationSpiritValuesSetter.g.cs` | `Spirit.AutomationServiceEngNote.AutomationSpiritValuesSetter` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.CMathcadPrimeApplication.g.cs` | `Spirit.AutomationServiceEngNote.CMathcadPrimeApplication` | 64 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.CMathcadPrimeWorksheet.g.cs` | `Spirit.AutomationServiceEngNote.CMathcadPrimeWorksheet` | 103 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.CMathcadPrimeWorksheetReadonlyOptions.g.cs` | `Spirit.AutomationServiceEngNote.CMathcadPrimeWorksheetReadonlyOptions` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.CMathcadPrimeWorksheets.g.cs` | `Spirit.AutomationServiceEngNote.CMathcadPrimeWorksheets` | 6 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.IConvertableToDocumentReadonlyOptions.g.cs` | `Spirit.AutomationServiceEngNote.IConvertableToDocumentReadonlyOptions` | 1 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNote.ScalarStateExtensions.g.cs` | `Spirit.AutomationServiceEngNote.ScalarStateExtensions` | 2 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationGetValueResult.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationGetValueResult` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritInputMatrixResult.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritInputMatrixResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritInputResult.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritInputResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritInputs.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritInputs` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritInputsOutputsConflicts.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritInputsOutputsConflicts` | 6 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritInputsOutputsStates.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritInputsOutputsStates` | 14 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritMatrix.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritMatrix` | 10 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritMatrixResult.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritMatrixResult` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritOutputMatrixResult.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritOutputMatrixResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritOutputMatrixResultAs.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritOutputMatrixResultAs` | 3 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritOutputResult.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritOutputResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritOutputResultAs.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritOutputResultAs` | 6 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritOutputs.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritOutputs` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritResult.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritResult` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritSetValueResults.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritSetValueResults` | 5 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.AutomationSpiritValuesSetter.g.cs` | `Spirit.AutomationServiceEngNotePlus.AutomationSpiritValuesSetter` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.CMathcadPrimeApplication.g.cs` | `Spirit.AutomationServiceEngNotePlus.CMathcadPrimeApplication` | 63 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.CMathcadPrimeWorksheet.g.cs` | `Spirit.AutomationServiceEngNotePlus.CMathcadPrimeWorksheet` | 103 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.CMathcadPrimeWorksheetReadonlyOptions.g.cs` | `Spirit.AutomationServiceEngNotePlus.CMathcadPrimeWorksheetReadonlyOptions` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.CMathcadPrimeWorksheets.g.cs` | `Spirit.AutomationServiceEngNotePlus.CMathcadPrimeWorksheets` | 6 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.IConvertableToDocumentReadonlyOptions.g.cs` | `Spirit.AutomationServiceEngNotePlus.IConvertableToDocumentReadonlyOptions` | 1 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationServiceEngNotePlus.ScalarStateExtensions.g.cs` | `Spirit.AutomationServiceEngNotePlus.ScalarStateExtensions` | 2 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritInputMatrixResult.g.cs` | `Spirit.AutomationSpiritInputMatrixResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritInputResult.g.cs` | `Spirit.AutomationSpiritInputResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritInputs.g.cs` | `Spirit.AutomationSpiritInputs` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritInputsOutputsConflicts.g.cs` | `Spirit.AutomationSpiritInputsOutputsConflicts` | 6 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritInputsOutputsStates.g.cs` | `Spirit.AutomationSpiritInputsOutputsStates` | 14 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritMatrix.g.cs` | `Spirit.AutomationSpiritMatrix` | 10 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritMatrixResult.g.cs` | `Spirit.AutomationSpiritMatrixResult` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritOutputMatrixResult.g.cs` | `Spirit.AutomationSpiritOutputMatrixResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritOutputMatrixResultAs.g.cs` | `Spirit.AutomationSpiritOutputMatrixResultAs` | 3 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritOutputResult.g.cs` | `Spirit.AutomationSpiritOutputResult` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritOutputResultAs.g.cs` | `Spirit.AutomationSpiritOutputResultAs` | 6 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritOutputs.g.cs` | `Spirit.AutomationSpiritOutputs` | 4 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritResult.g.cs` | `Spirit.AutomationSpiritResult` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritSetValueResults.g.cs` | `Spirit.AutomationSpiritSetValueResults` | 5 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `AutomationSpiritValuesSetter.g.cs` | `Spirit.AutomationSpiritValuesSetter` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `Behaviors.AllFunctionsBrowserTreeViewUIFunctionalityAttachedBehavior.g.cs` | `Spirit.Behaviors.AllFunctionsBrowserTreeViewUIFunctionalityAttachedBehavior` | 15 | U |  |
| `Behaviors.BaseUIFunctionalityAttachedBehavior.g.cs` | `Spirit.Behaviors.BaseUIFunctionalityAttachedBehavior` | 26 | U |  |
| `Behaviors.UIFunctionalityWithCommandAttachedBehavior.g.cs` | `Spirit.Behaviors.UIFunctionalityWithCommandAttachedBehavior` | 8 | U |  |
| `Behaviors.UIFunctionalityWithoutCommandAttachedBehavior.g.cs` | `Spirit.Behaviors.UIFunctionalityWithoutCommandAttachedBehavior` | 8 | U |  |
| `BooleanToMultiThreadingValueConverter.g.cs` | `Spirit.BooleanToMultiThreadingValueConverter` | 4 | U |  |
| `ButtonDropDownSelection.g.cs` | `Spirit.ButtonDropDownSelection` | 3 | U |  |
| `CMathcadPrimeApplication.g.cs` | `Spirit.CMathcadPrimeApplication` | 57 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `CMathcadPrimeWorksheet.g.cs` | `Spirit.CMathcadPrimeWorksheet` | 98 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `CMathcadPrimeWorksheetReadonlyOptions.g.cs` | `Spirit.CMathcadPrimeWorksheetReadonlyOptions` | 8 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `CMathcadPrimeWorksheets.g.cs` | `Spirit.CMathcadPrimeWorksheets` | 6 | D | Automation surface (COM events, closures, Quire.Automation/Interop types): per-member, after code batches |
| `CalculationControlRibbonTab.g.cs` | `Spirit.CalculationControlRibbonTab` | 4 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `CalculationErrorTracingRibbonBar.g.cs` | `Spirit.CalculationErrorTracingRibbonBar` | 9 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `CalculationOptionsRibbonBar.g.cs` | `Spirit.CalculationOptionsRibbonBar` | 12 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `CalculationStatusConverter.g.cs` | `Spirit.CalculationStatusConverter` | 3 | U |  |
| `CalculationWarningRibbonBar.g.cs` | `Spirit.CalculationWarningRibbonBar` | 9 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `CaretInsertionTypeToBooleanConverter.g.cs` | `Spirit.CaretInsertionTypeToBooleanConverter` | 2 | U |  |
| `ChangeTextStyleCommand.g.cs` | `Spirit.ChangeTextStyleCommand` | 9 | U |  |
| `ColumnViewModel.g.cs` | `Spirit.ColumnViewModel` | 13 | U |  |
| `ComInteropIds.g.cs` | `Spirit.ComInteropIds` | 4 | U |  |
| `ComNativeExternalApi.g.cs` | `Spirit.ComNativeExternalApi` | 4 | B | COM/native: class-factory/registration or P/Invoke shape; needs native COM decision |
| `ComplexNumberStyleToStringConverter.g.cs` | `Spirit.ComplexNumberStyleToStringConverter` | 3 | U |  |
| `CompositSymbolDescriptorCollection.g.cs` | `Spirit.CompositSymbolDescriptorCollection` | 3 | U |  |
| `CultureHelper.g.cs` | `Spirit.CultureHelper` | 1 | B | Settings/resource/product literal: depends on Mathcad settings or product constants decision |
| `CustomMarginsDialog.g.cs` | `Spirit.CustomMarginsDialog` | 15 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `CustomMarginsDialogResourceDictionary.g.cs` | `Spirit.CustomMarginsDialogResourceDictionary` | 5 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `CustomMarginsViewModel.g.cs` | `Spirit.CustomMarginsViewModel` | 30 | U |  |
| `DataMatrixDataAndMatricesRibbonBar.g.cs` | `Spirit.DataMatrixDataAndMatricesRibbonBar` | 11 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `DataMatrixFileAccessRibbonBar.g.cs` | `Spirit.DataMatrixFileAccessRibbonBar` | 11 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `DataMatrixRowsAndColumnsRibbonBar.g.cs` | `Spirit.DataMatrixRowsAndColumnsRibbonBar` | 7 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `DebugPropertyWindow.g.cs` | `Spirit.DebugPropertyWindow` | 4 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `DeleteTextStyleCommand.g.cs` | `Spirit.DeleteTextStyleCommand` | 9 | U |  |
| `DialogManagerResourceDictionary.g.cs` | `Spirit.DialogManagerResourceDictionary` | 5 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `DictionaryEntryWrapper.g.cs` | `Spirit.DictionaryEntryWrapper` | 4 | U |  |
| `DimensionEditDialogViewModel.g.cs` | `Spirit.DimensionEditDialogViewModel` | 15 | U |  |
| `DimensionsEditDialog.g.cs` | `Spirit.DimensionsEditDialog` | 10 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `DipsToDisplayUnitsConverter.g.cs` | `Spirit.DipsToDisplayUnitsConverter` | 5 | U |  |
| `DisplayUnitsUtils.g.cs` | `Spirit.DisplayUnitsUtils` | 2 | U |  |
| `DocNameToTabHeaderConverter.g.cs` | `Spirit.DocNameToTabHeaderConverter` | 3 | U |  |
| `DocumentInsertRibbonBar.g.cs` | `Spirit.DocumentInsertRibbonBar` | 13 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `DocumentRegionsRibbonBar.g.cs` | `Spirit.DocumentRegionsRibbonBar` | 9 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `DocumentViewRibbonBar.g.cs` | `Spirit.DocumentViewRibbonBar` | 7 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `DropDownSelector.g.cs` | `Spirit.DropDownSelector` | 26 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `DropDownSelectorItem.g.cs` | `Spirit.DropDownSelectorItem` | 7 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `EmptyDropDownSelectorItem.g.cs` | `Spirit.EmptyDropDownSelectorItem` | 4 | U |  |
| `EnumMatchToBooleanConverterWithNull.g.cs` | `Spirit.EnumMatchToBooleanConverterWithNull` | 2 | U |  |
| `ExperimentalCalculationRibbonBar.g.cs` | `Spirit.ExperimentalCalculationRibbonBar` | 6 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `ExtendedCommands.g.cs` | `Spirit.ExtendedCommands` | 6 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `FindControl.g.cs` | `Spirit.FindControl` | 20 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `FindUserProperties.g.cs` | `Spirit.FindUserProperties` | 19 | U |  |
| `FlowDocumentFamilyConverter.g.cs` | `Spirit.FlowDocumentFamilyConverter` | 3 | U |  |
| `FontDataObject.g.cs` | `Spirit.FontDataObject` | 4 | U |  |
| `FontFamilyComboBox.g.cs` | `Spirit.FontFamilyComboBox` | 11 | U |  |
| `FontFormatRibbonBar.g.cs` | `Spirit.FontFormatRibbonBar` | 12 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `FontSizeComboBox.g.cs` | `Spirit.FontSizeComboBox` | 13 | U |  |
| `FontSizeToStringConverter.g.cs` | `Spirit.FontSizeToStringConverter` | 2 | U |  |
| `FontSizeValidationRule.g.cs` | `Spirit.FontSizeValidationRule` | 9 | U |  |
| `FontStyleToBooleanConverter.g.cs` | `Spirit.FontStyleToBooleanConverter` | 2 | U |  |
| `FontVariantsToBooleanConverter.g.cs` | `Spirit.FontVariantsToBooleanConverter` | 2 | U |  |
| `FontWeightToBooleanConverter.g.cs` | `Spirit.FontWeightToBooleanConverter` | 2 | U |  |
| `FormatPlotValueVisiblityConverter.g.cs` | `Spirit.FormatPlotValueVisiblityConverter` | 3 | U |  |
| `FormatPlotValuesColumnConverter.g.cs` | `Spirit.FormatPlotValuesColumnConverter` | 3 | U |  |
| `FormatPlotValuesRowConverter.g.cs` | `Spirit.FormatPlotValuesRowConverter` | 3 | U |  |
| `Freemium.FreemiumAboutBox.g.cs` | `Spirit.Freemium.FreemiumAboutBox` | 29 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `Freemium.HideIfNullConverter.g.cs` | `Spirit.Freemium.HideIfNullConverter` | 3 | U |  |
| `Freemium.VCardDisplay.g.cs` | `Spirit.Freemium.VCardDisplay` | 11 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `FunctionBrowser.g.cs` | `Spirit.FunctionBrowser` | 36 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `FunctionBrowserPopupWindow.g.cs` | `Spirit.FunctionBrowserPopupWindow` | 4 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `FunctionBrowserWindow.g.cs` | `Spirit.FunctionBrowserWindow` | 7 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `FunctionLabel.g.cs` | `Spirit.FunctionLabel` | 19 | U |  |
| `GettingStartedAskCommunityRibbonBar.g.cs` | `Spirit.GettingStartedAskCommunityRibbonBar` | 4 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `GettingStartedHelpRibbonBar.g.cs` | `Spirit.GettingStartedHelpRibbonBar` | 4 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `GettingStartedLearningRibbonBar.g.cs` | `Spirit.GettingStartedLearningRibbonBar` | 4 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `GettingStartedTechnicalSupportRibbonBar.g.cs` | `Spirit.GettingStartedTechnicalSupportRibbonBar` | 5 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `HeadersAndFootersRibbonBar.g.cs` | `Spirit.HeadersAndFootersRibbonBar` | 12 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `IClassFactory.g.cs` | `Spirit.IClassFactory` | 2 | B | COM/native: class-factory/registration or P/Invoke shape; needs native COM decision |
| `IClassFactory2.g.cs` | `Spirit.IClassFactory2` | 5 | B | COM/native: class-factory/registration or P/Invoke shape; needs native COM decision |
| `IConvertableToDocumentReadonlyOptions.g.cs` | `Spirit.IConvertableToDocumentReadonlyOptions` | 1 | U |  |
| `IDropDownSelectorItem.g.cs` | `Spirit.IDropDownSelectorItem` | 3 | U |  |
| `IOWindowConstants.g.cs` | `Spirit.IOWindowConstants` | 5 | B | Settings/resource/product literal: depends on Mathcad settings or product constants decision |
| `ITabManagerItem.g.cs` | `Spirit.ITabManagerItem` | 1 | U |  |
| `IdLabelToItemIndexInFormatsListConverter.g.cs` | `Spirit.IdLabelToItemIndexInFormatsListConverter` | 4 | U |  |
| `IdLabelsEnumToLocalizedStringConverter.g.cs` | `Spirit.IdLabelsEnumToLocalizedStringConverter` | 3 | U |  |
| `ImportExcel.g.cs` | `Spirit.ImportExcel` | 38 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `IncrementDecimalPlaces.g.cs` | `Spirit.IncrementDecimalPlaces` | 3 | U |  |
| `IndentRule.g.cs` | `Spirit.IndentRule` | 2 | U |  |
| `InputControlsRibbonBar.g.cs` | `Spirit.InputControlsRibbonBar` | 17 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `InputOutputDesignationWindow.g.cs` | `Spirit.InputOutputDesignationWindow` | 42 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `IntToStringConverter.g.cs` | `Spirit.IntToStringConverter` | 3 | U |  |
| `IntegrationCommands.g.cs` | `Spirit.IntegrationCommands` | 18 | U |  |
| `IsCalcNowEnabledConverter.g.cs` | `Spirit.IsCalcNowEnabledConverter` | 3 | U |  |
| `LICINFO.g.cs` | `Spirit.LICINFO` | 3 | B | COM/native: class-factory/registration or P/Invoke shape; needs native COM decision |
| `LabelDropDown.g.cs` | `Spirit.LabelDropDown` | 14 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `License.LicenseError.g.cs` | `Spirit.License.LicenseError` | 2 | U |  |
| `License.LicenseMessageDictionary.g.cs` | `Spirit.License.LicenseMessageDictionary` | 1 | U |  |
| `License.LicenseOptionsPage.g.cs` | `Spirit.License.LicenseOptionsPage` | 47 | U |  |
| `License.ViewModel.LicenseErrorViewModel.g.cs` | `Spirit.License.ViewModel.LicenseErrorViewModel` | 5 | U |  |
| `License.ViewModel.LicenseOptionsPageViewModel.g.cs` | `Spirit.License.ViewModel.LicenseOptionsPageViewModel` | 3 | U |  |
| `LinksRibbonBar.g.cs` | `Spirit.LinksRibbonBar` | 9 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `LocBamlHelper.g.cs` | `Spirit.LocBamlHelper` | 5 | U |  |
| `LocalizedFontNameConverter.g.cs` | `Spirit.LocalizedFontNameConverter` | 12 | U |  |
| `MCADRibbonBar.g.cs` | `Spirit.MCADRibbonBar` | 9 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MarginConverter.g.cs` | `Spirit.MarginConverter` | 4 | U |  |
| `MarginSelectorItemGrid.g.cs` | `Spirit.MarginSelectorItemGrid` | 13 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MathClipboardRibbonBar.g.cs` | `Spirit.MathClipboardRibbonBar` | 8 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MathFontSizeValidationRule.g.cs` | `Spirit.MathFontSizeValidationRule` | 2 | U |  |
| `MathFormatRibbonBar.g.cs` | `Spirit.MathFormatRibbonBar` | 9 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MathFormatWithDebugTab.g.cs` | `Spirit.MathFormatWithDebugTab` | 7 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MathFunctionsRibbonBar.g.cs` | `Spirit.MathFunctionsRibbonBar` | 31 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MathGroupingRibbonBar.g.cs` | `Spirit.MathGroupingRibbonBar` | 12 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MathOperatorsAndSymbolsRibbonBar.g.cs` | `Spirit.MathOperatorsAndSymbolsRibbonBar` | 14 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MathRegionsRibbonBar.g.cs` | `Spirit.MathRegionsRibbonBar` | 16 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MathStyleRibbonBar.g.cs` | `Spirit.MathStyleRibbonBar` | 6 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MathSymbolMenuItemConverter.g.cs` | `Spirit.MathSymbolMenuItemConverter` | 5 | U |  |
| `MathUnitsSettingsRibbonBar.g.cs` | `Spirit.MathUnitsSettingsRibbonBar` | 7 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MathcadPrimeClassFactory.g.cs` | `Spirit.MathcadPrimeClassFactory` | 3 | B | COM/native: class-factory/registration or P/Invoke shape; needs native COM decision |
| `MathcadPrimeClassFactoryObsolete.g.cs` | `Spirit.MathcadPrimeClassFactoryObsolete` | 3 | B | COM/native: class-factory/registration or P/Invoke shape; needs native COM decision |
| `MathcadPrimeComServerManager.g.cs` | `Spirit.MathcadPrimeComServerManager` | 34 | B | COM/native: class-factory/registration or P/Invoke shape; needs native COM decision |
| `MathcadPrimeEngNoteClassFactory.g.cs` | `Spirit.MathcadPrimeEngNoteClassFactory` | 6 | B | COM/native: class-factory/registration or P/Invoke shape; needs native COM decision |
| `MathcadPrimeEngNotePlusClassFactory.g.cs` | `Spirit.MathcadPrimeEngNotePlusClassFactory` | 6 | B | COM/native: class-factory/registration or P/Invoke shape; needs native COM decision |
| `MathcadWorksheetsRibbonBar.g.cs` | `Spirit.MathcadWorksheetsRibbonBar` | 8 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MatricesAndTablesFormatRibbonBar.g.cs` | `Spirit.MatricesAndTablesFormatRibbonBar` | 7 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MessageBoxes.DialogManager.g.cs` | `Spirit.MessageBoxes.DialogManager` | 5 | U |  |
| `MessageBoxes.DialogWithOptions.g.cs` | `Spirit.MessageBoxes.DialogWithOptions` | 9 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MessageBoxes.ErrorWarningTabData.g.cs` | `Spirit.MessageBoxes.ErrorWarningTabData` | 4 | U |  |
| `MessageBoxes.MessageBoxManager.g.cs` | `Spirit.MessageBoxes.MessageBoxManager` | 21 | U |  |
| `MessageBoxes.MessageDialogWithOptions.g.cs` | `Spirit.MessageBoxes.MessageDialogWithOptions` | 15 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MessageBoxes.MessageDialogWithProgressBar.g.cs` | `Spirit.MessageBoxes.MessageDialogWithProgressBar` | 18 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MessageBoxes.SaveAndSyncErrorWarningDialog.g.cs` | `Spirit.MessageBoxes.SaveAndSyncErrorWarningDialog` | 13 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MruFiles.IMruFilePersistanceManager.g.cs` | `Spirit.MruFiles.IMruFilePersistanceManager` | 6 | U |  |
| `MruFiles.MruAccessKeyConverter.g.cs` | `Spirit.MruFiles.MruAccessKeyConverter` | 4 | U |  |
| `MruFiles.MruFileExtensionComparer.g.cs` | `Spirit.MruFiles.MruFileExtensionComparer` | 4 | U |  |
| `MruFiles.MruFileManager.g.cs` | `Spirit.MruFiles.MruFileManager` | 33 | U |  |
| `MruFiles.MruUserConfigPersistance.g.cs` | `Spirit.MruFiles.MruUserConfigPersistance` | 9 | U |  |
| `MultiLineBinaryLinearNodeDebugFormat.g.cs` | `Spirit.MultiLineBinaryLinearNodeDebugFormat` | 4 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `MultiThreadingValueToBooleanConverter.g.cs` | `Spirit.MultiThreadingValueToBooleanConverter` | 4 | U |  |
| `NumberFormatRibbonBar.g.cs` | `Spirit.NumberFormatRibbonBar` | 24 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `NumberOfPointsComboBox.g.cs` | `Spirit.NumberOfPointsComboBox` | 19 | U |  |
| `NumberStyleEnablesExponentialThresholdConverter.g.cs` | `Spirit.NumberStyleEnablesExponentialThresholdConverter` | 3 | U |  |
| `NumberStyleEnablesRadixStyleMultiConverter.g.cs` | `Spirit.NumberStyleEnablesRadixStyleMultiConverter` | 3 | U |  |
| `NumberStyleToStringConverter.g.cs` | `Spirit.NumberStyleToStringConverter` | 3 | U |  |
| `ObjectToBoolConverter.g.cs` | `Spirit.ObjectToBoolConverter` | 6 | U |  |
| `ObjectsRibbonBar.g.cs` | `Spirit.ObjectsRibbonBar` | 9 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `OpenEngineeringDocumentParameter.g.cs` | `Spirit.OpenEngineeringDocumentParameter` | 3 | U |  |
| `OptOutQualityAgentHandler.g.cs` | `Spirit.OptOutQualityAgentHandler` | 10 | U |  |
| `OptionsWindow.g.cs` | `Spirit.OptionsWindow` | 33 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `PageBackgroundColorMatchToBooleanConverter.g.cs` | `Spirit.PageBackgroundColorMatchToBooleanConverter` | 4 | U |  |
| `PageMarginSelectorTypeValueConverter.g.cs` | `Spirit.PageMarginSelectorTypeValueConverter` | 4 | U |  |
| `PageNumberIsCheckedConverter.g.cs` | `Spirit.PageNumberIsCheckedConverter` | 3 | U |  |
| `PageRibbonBar.g.cs` | `Spirit.PageRibbonBar` | 10 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `PlotAxesRibbonBar.g.cs` | `Spirit.PlotAxesRibbonBar` | 14 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `PlotCreationRibbonBar.g.cs` | `Spirit.PlotCreationRibbonBar` | 22 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `PlotLabelsRibbonBar.g.cs` | `Spirit.PlotLabelsRibbonBar` | 10 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `PlotTraceStylesRibbonBar.g.cs` | `Spirit.PlotTraceStylesRibbonBar` | 13 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `PrimeCommandLineArguments.g.cs` | `Spirit.PrimeCommandLineArguments` | 7 | B | COM/native: class-factory/registration or P/Invoke shape; needs native COM decision |
| `PrintInProgressDialog.g.cs` | `Spirit.PrintInProgressDialog` | 12 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `ProductInfo.Constants.g.cs` | `ProductInfo.Constants` | 3 | B | Settings/resource/product literal: depends on Mathcad settings or product constants decision |
| `ProofingLanguageDataItem.g.cs` | `Spirit.ProofingLanguageDataItem` | 11 | U |  |
| `ProofingLanguageDropDownSelector.g.cs` | `Spirit.ProofingLanguageDropDownSelector` | 11 | U |  |
| `Properties.Settings.g.cs` | `Spirit.Properties.Settings` | 39 | B | Settings/resource/product literal: depends on Mathcad settings or product constants decision |
| `PropertyNames.g.cs` | `Spirit.PropertyNames` | 2 | B | Settings/resource/product literal: depends on Mathcad settings or product constants decision |
| `PythonDirectoryPathConverter.g.cs` | `Spirit.PythonDirectoryPathConverter` | 3 | U |  |
| `QatControl.g.cs` | `Spirit.QatControl` | 9 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `RadixStyleToStringConverter.g.cs` | `Spirit.RadixStyleToStringConverter` | 3 | U |  |
| `RadixStyleToStringMultiConverter.g.cs` | `Spirit.RadixStyleToStringMultiConverter` | 3 | U |  |
| `ReadonlyOptionConverter.g.cs` | `Spirit.ReadonlyOptionConverter` | 6 | U |  |
| `RegionBorderRibbonBar.g.cs` | `Spirit.RegionBorderRibbonBar` | 6 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `ResourceProviderService.g.cs` | `Spirit.ResourceProviderService` | 3 | U |  |
| `RibbonResources.g.cs` | `Spirit.RibbonResources` | 5 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `RichTextContextMenuItemsDictionary.g.cs` | `Spirit.RichTextContextMenuItemsDictionary` | 5 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `SavedDateIsCheckedConverter.g.cs` | `Spirit.SavedDateIsCheckedConverter` | 4 | U |  |
| `ScalarStateExtensions.g.cs` | `Spirit.ScalarStateExtensions` | 2 | U |  |
| `SizeToVisibilityMultiConverter.g.cs` | `Spirit.SizeToVisibilityMultiConverter` | 3 | U |  |
| `SpellingRibbonBar.g.cs` | `Spirit.SpellingRibbonBar` | 8 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `SpiritMainWindow.g.cs` | `Spirit.SpiritMainWindow` | 323 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `SpiritOptions.g.cs` | `Spirit.SpiritOptions` | 7 | U |  |
| `SplitCamelCase.g.cs` | `Spirit.SplitCamelCase` | 3 | U |  |
| `StatusBarControls.GotoPageValidationRule.g.cs` | `Spirit.StatusBarControls.GotoPageValidationRule` | 5 | U |  |
| `StatusBarControls.GotoPageValidatorHelper.g.cs` | `Spirit.StatusBarControls.GotoPageValidatorHelper` | 7 | U |  |
| `StatusBarControls.StatusRibbonBar.g.cs` | `Spirit.StatusBarControls.StatusRibbonBar` | 29 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `StickyDropDownSelector.g.cs` | `Spirit.StickyDropDownSelector` | 7 | U |  |
| `StringToTextIndent.g.cs` | `Spirit.StringToTextIndent` | 3 | U |  |
| `StyleFontFamilyConverter.g.cs` | `Spirit.StyleFontFamilyConverter` | 3 | U |  |
| `StyleFontFamilyValidationRule.g.cs` | `Spirit.StyleFontFamilyValidationRule` | 2 | U |  |
| `StyleFontSizeToStringConverter.g.cs` | `Spirit.StyleFontSizeToStringConverter` | 2 | U |  |
| `StyleFontSizeValidationRule.g.cs` | `Spirit.StyleFontSizeValidationRule` | 3 | U |  |
| `StylePropertiesLocalized.g.cs` | `Spirit.StylePropertiesLocalized` | 3 | U |  |
| `StylesAndLabelsFormatRibbonBar.g.cs` | `Spirit.StylesAndLabelsFormatRibbonBar` | 11 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `SubscriptStateToCheckedConverter.g.cs` | `Spirit.SubscriptStateToCheckedConverter` | 4 | U |  |
| `SystemUnitEditDialog.g.cs` | `Spirit.SystemUnitEditDialog` | 16 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `SystemUnitEditDialogViewModel.g.cs` | `Spirit.SystemUnitEditDialogViewModel` | 37 | U |  |
| `TabManager.g.cs` | `Spirit.TabManager` | 30 | U |  |
| `TabManagerItem.g.cs` | `Spirit.TabManagerItem` | 4 | U |  |
| `TabManagerItemClickedEventArgs.g.cs` | `Spirit.TabManagerItemClickedEventArgs` | 3 | U |  |
| `TextAndRegionFormatRibbonBar.g.cs` | `Spirit.TextAndRegionFormatRibbonBar` | 13 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `TextDecorationCollectionToBooleanConverter.g.cs` | `Spirit.TextDecorationCollectionToBooleanConverter` | 3 | U |  |
| `TextStyleComboBox.g.cs` | `Spirit.TextStyleComboBox` | 11 | U |  |
| `TextStyleEditDialog.g.cs` | `Spirit.TextStyleEditDialog` | 36 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `TextStyleEditDialogViewModel.g.cs` | `Spirit.TextStyleEditDialogViewModel` | 38 | U |  |
| `TextStyleInheritedFontWeight.g.cs` | `Spirit.TextStyleInheritedFontWeight` | 3 | U |  |
| `TextStyleNameToHeight.g.cs` | `Spirit.TextStyleNameToHeight` | 5 | U |  |
| `TextStyleNameToVisibility.g.cs` | `Spirit.TextStyleNameToVisibility` | 3 | U |  |
| `TextStyleProperties.g.cs` | `Spirit.TextStyleProperties` | 3 | U |  |
| `TextStylePropertyToVisibility.g.cs` | `Spirit.TextStylePropertyToVisibility` | 3 | U |  |
| `TextStylesRibbonBar.g.cs` | `Spirit.TextStylesRibbonBar` | 16 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `TwoParametersObject.g.cs` | `Spirit.TwoParametersObject` | 7 | U |  |
| `UniqueTextStyleNameHelper.g.cs` | `Spirit.UniqueTextStyleNameHelper` | 6 | U |  |
| `UniqueTextStyleNameRule.g.cs` | `Spirit.UniqueTextStyleNameRule` | 5 | U |  |
| `UnitSystemsToStringConverter.g.cs` | `Spirit.UnitSystemsToStringConverter` | 3 | U |  |
| `WindowTabTupleData.g.cs` | `Spirit.WindowTabTupleData` | 7 | U |  |
| `WindowTitleConverter.g.cs` | `Spirit.WindowTitleConverter` | 3 | U |  |
| `WorksheetPageLayoutToRowCountConverter.g.cs` | `Spirit.WorksheetPageLayoutToRowCountConverter` | 3 | U |  |
| `XmlLanguageToCultureInfoConverter.g.cs` | `Spirit.XmlLanguageToCultureInfoConverter` | 4 | U |  |
| `Zoom.g.cs` | `Spirit.Zoom` | 25 | P | XAML: InitializeComponent/Connect/_contentLoaded and XAML-named element fields blocked (BAML/event wiring); other members classified per batch |
| `ZoomControlWidthToVisibilityMultiConverter.g.cs` | `Spirit.ZoomControlWidthToVisibilityMultiConverter` | 3 | U |  |

## Blocked, grouped by reason

**COM / native / P-Invoke:** `AddonsLoaderHelper`, `ComNativeExternalApi`, `IClassFactory`, `IClassFactory2`, `LICINFO`, `MathcadPrimeClassFactory`, `MathcadPrimeClassFactoryObsolete`, `MathcadPrimeComServerManager`, `MathcadPrimeEngNoteClassFactory`, `MathcadPrimeEngNotePlusClassFactory`, `PrimeCommandLineArguments`

**Settings / product literal:** `CultureHelper`, `IOWindowConstants`, `ProductInfo.Constants`, `Properties.Settings`, `PropertyNames`

**XAML (partly blocked, InitializeComponent/Connect):** 78 files, listed in the table above.

**Automation (deferred per member):** 65 files, listed in the table above.


## Batches

- **Batch 1 (converters, U):** `BooleanToMultiThreadingValueConverter` (Convert x2, ConvertBack), `CalculationStatusConverter`, `CaretInsertionTypeToBooleanConverter`, `ComplexNumberStyleToStringConverter`, `FontSizeToStringConverter`, `FontStyleToBooleanConverter`, `FontVariantsToBooleanConverter`, `FontWeightToBooleanConverter`, `IntToStringConverter`, `IncrementDecimalPlaces`, `IsCalcNowEnabledConverter`, `SplitCamelCase`, `StringToTextIndent`, `NumberStyleToStringConverter`, `RadixStyleToStringConverter`, `NumberStyleEnablesExponentialThresholdConverter`, `TextStyleNameToVisibility`, `FormatPlotValueVisiblityConverter`, `FormatPlotValuesColumnConverter`, `FormatPlotValuesRowConverter`, `ZoomControlWidthToVisibilityMultiConverter`, `Freemium.HideIfNullConverter` (Convert and ConvertBack of each). Default constructors stay as generated.
  - Gate: PASS, 270 checked. Report: `.mirror-cache/release-il-2aea9c31781d/release-il.json`.
  - Shapes: the bool-then-`ToString` `ConvertBack` family keeps the bool on the stack (Roslyn does this in Release); the `switch` statement with `case` returns matches Mathcad's brtrue-to-case layout. `RadixStyleToStringConverter.ConvertBack` is a switch expression with a named string subject (slot order string, object) and a `(object)` first arm. `ZoomControlWidthToVisibilityMultiConverter.Convert` stores the nullable width in a named `double?` local.
  - Mismatches found on the way and fixed before landing: CaretInsertion (local instead of switch shape), Radix (slot order), Zoom (inline nullable compare).
- **Batch 2 (converters, U):** `EnumMatchToBooleanConverterWithNull.ConvertBack`, `MultiThreadingValueToBooleanConverter` (Convert, static Convert(object), ConvertBack), `TextStyleNameToHeight` (both const fields, Convert, ConvertBack), `UnitSystemsToStringConverter` (Convert, ConvertBack), `PageBackgroundColorMatchToBooleanConverter` (`Instance` initializer, Convert, ConvertBack).
  - Gate: PASS, 281 checked. Report: `.mirror-cache/release-il-df3c675855e3/release-il.json`.
  - Shape: `(bool)value ? Settings : 1` matches Mathcad's `brtrue` layout; `(!(bool)value) ? 1 : Settings` does not (Roslyn emits the branches in the other order).
- **Batch 3 (small classes, U):** `TwoParametersObject` (ParameterOne, ParameterTwo, ctor, both As-string methods), `DictionaryEntryWrapper` (Key, Value, ctor, ToDictionaryEntry), `TabManagerItemClickedEventArgs` (both properties, ctor), `OpenEngineeringDocumentParameter.FilePath`, `MessageBoxes.ErrorWarningTabData` (three properties, ctor), `WindowTabTupleData` (Wnd, Order, PropertyChanged event, ctor, OnPropertyChanged).
  - Gate: PASS, 310 checked. Report: `.mirror-cache/release-il-fe748ebc2197/release-il.json`.
  - Shape: `Order` setter is `if (_order == value) return;` then store and notify (the `!=` block form branches the other way).
  - Blocked, stub kept: `WindowTabTupleData.Header`. Mathcad's `Wnd` is `DockWindow` and its `Header` getter is a DockWindow member; the type-map maps `Wnd` to `AvalonDock.Layout.LayoutAnchorable`, which has no `Header` (`Title`/`Content` differ). Needs a type-map decision.
- **Batch 4 (constants and converters, U):** `IOWindowConstants` (three consts, `AliasPath`, `IdNamePath` initializers), `PropertyNames` (const, `CurrentDirectionPath`), `TextDecorationCollectionToBooleanConverter` (Convert, ConvertBack), `FlowDocumentFamilyConverter` (Convert, ConvertBack).
  - Gate: PASS, 329 checked. Report: `.mirror-cache/release-il-0143022a3562/release-il.json`.
  - Shape: `FlowDocumentFamilyConverter.Convert` joins the pack-URI path with an explicit `string.Concat`; a literal `+` literal is folded by the compiler, and Mathcad's IL keeps the call. Keeps the Mathcad literal `Ptc.Controls.Core;component/...` (the holding assembly is Quire, so it is not `<self>`).
- **Batch 5 (small converters and statics, U):** `StyleFontSizeToStringConverter.ConvertBack`, `StyleFontSizeValidationRule` (const, MaxFontSize), `DisplayUnitsUtils` (`CmToMmMult` initializer, DipsToDisplayUnits), `EmptyDropDownSelectorItem` (Value, Header with initializer, IsChecked; default ctor), `StylePropertiesLocalized` (Convert, ConvertBack).
  - Gate: PASS, 362 checked. Report: `.mirror-cache/release-il-c8ec93ef8f0b/release-il.json`.
  - Shape: `DipsToDisplayUnits` assigns a local in if/else and returns it (Mathcad's `bne.un` else branch); the early-return form does not match.
- **Batch 6 (validation rules, text box, multi converters, U):** `IndentRule.Validate`, `FontSizeValidationRule` (three consts, MinFontSize, MaxFontSize, Precision, ValidateFontSizeStringInput, Validate), `FontDataObject` (FontFamily, LocalizedName, ctor, ToString), `AliasTextBox` (PreMouseDown event, OnMouseDown), `SizeToVisibilityMultiConverter` (Convert, ConvertBack), `RadixStyleToStringMultiConverter` (Convert, ConvertBack).
  - Gate: PASS, 377 checked. Report: `.mirror-cache/release-il-51ccbbed6674/release-il.json`.
- **Batch 7 (text style commands, options, rule properties, U):** `ChangeTextStyleCommand` and `DeleteTextStyleCommand` (Name, ctor, Do, Undo, Redo, Execute), `SpiritOptions` (`_instance` initializer, const, Instance, EnableGettingStartedTab, PropertyChanged event, ctor), `UniqueTextStyleNameRule` (StyleNames, DependencyPropertyHelper). `UniqueTextStyleNameRule.Validate` stays a generated stub.
  - Gate: PASS, 432 checked. Report: `.mirror-cache/release-il-e03ddd5f2758/release-il.json`.
  - Blocked, stub kept: `UniqueTextStyleNameRule.Validate`. Mathcad calls `String.IndexOfAny` with `call`; Roslyn emits `callvirt` for the same receiver (the first attempt failed on that call alone).
- **Batch 8 (converters, U):** `WorksheetPageLayoutToRowCountConverter` (Convert, ConvertBack), `NumberStyleEnablesRadixStyleMultiConverter` (Convert, ConvertBack), `ReadonlyOptionConverter` (SaveAndSync const, Instance, Convert, ConvertBack).
  - Gate: PASS, 439 checked. Report: `.mirror-cache/release-il-d9283ec20269/release-il.json`.
- **Batch 9 (id-label and dips converters, U):** `IdLabelsEnumToLocalizedStringConverter` (Convert, ConvertBack, ctor), `IdLabelToItemIndexInFormatsListConverter` (Convert, ConvertBack, FindItemIndexToSelect, ctor), `DipsToDisplayUnitsConverter` (Instance, `_numOfDigitsForRound`, Convert, ConvertBack; private ctor stays generated).
  - Gate: PASS, 474 checked. Report: `.mirror-cache/release-il-8fcc658b3077/release-il.json`.
  - Shape: the lambda in `Convert` is named `<Convert>b__0_0` only when no earlier member is in the generated file; a generated constructor ahead of `Convert` gave `b__1_0`. Both constructors moved into the hand-written file after the methods (Mathcad order: Convert, ConvertBack, FindItemIndexToSelect, ctor).
- **Batch 10 (dimension edit view model, U):** `DimensionEditDialogViewModel` (DimensionDataSource, UnitDataSource with its lambda, Action, IsDimensionEnabled, DimensionSelectedItem, UnitSelectedItem, IsCustomDataChanged, ctor, HandlePostDimensionChanged). Fields stay generated.
  - Gate: PASS, 487 checked. Report: `.mirror-cache/release-il-4dc5c6e1905d/release-il.json`.
  - Shape: the lambda in `UnitDataSource` is `<get_UnitDataSource>b__10_0` in Mathcad. The ordinal counts the constructor, which comes first in Mathcad's source, so the constructor moved to the top of the hand-written members.
- **Batch 11 (ColumnViewModel, U; branch `mirror/quire-main-rest-pure-20261111`):** `ColumnViewModel` (CollectionProperty with its static initializer, Collection, ColumnDataName, CurrentDirection, PropertyChanged event, HandleCollectionSortOrderCleared, OnCollectionChanged, NotifySortOrderChanged, SortByNextOrder, GetNextSortDirection, FirePropertyChanged, ctor). `_isCollectionInitialized` stays generated.
  - Gate: PASS, 499 checked. Report: `.mirror-cache/release-il-20cc601339f8/release-il.json`.
  - Shapes: `OnCollectionChanged` tests each assertion in its own `if` (a combined `||` branches differently). `CurrentDirection` tests the name with `!=` and returns on the else path, with an empty then-block (the `==` form calls `op_Equality`). `GetNextSortDirection` falls to one trailing `return null` after the switch (the default section then reuses the subject's slot as Mathcad does).
- **Batch 12 (CustomMarginsViewModel, U; branch `mirror/quire-main-rest-pure-20261111`):** `CustomMarginsViewModel` (`_numOfDigitsForRound` initializer, the four `*MarginText` and four `*Margin` properties, PageSizeUnits, NewPageMargin, ctor, IsValidMargin, FixPageMargins, ValidateInput, RemoveAllErrors, SetMarginsInDisplayUnits, DipsToDisplayUnitsMargins). Other fields stay generated.
  - Gate: PASS, 524 checked. Report: `.mirror-cache/release-il-0a9180bd5399/release-il.json`.
  - Shapes: setters are guard returns (`if (old == value || !IsValidMargin(...)) return;`, `if (Round(a) == Round(b)) return;`), because Release IL branches on the guard with `brtrue`. `FixPageMargins` is a guard return on `DevAssert(...).Failed`. `NewPageMargin` assigns one local on both branches (a ternary of two `new Thickness` emits `newobj` where Mathcad uses `ldloca` + `call .ctor`).

- **Batch 13 (SystemUnitEditDialogViewModel, U; branch `mirror/quire-main-rest-pure-20261111`):** `SystemUnitEditDialogViewModel` except `HandleDialogAction`: static `RbCustomCommand`, the three dictionaries, IsFunctionalityEnabled, DiscardMessage, CustomBaseUnitSystem, DerivedListBox, IsCustom/IsCustomExpress/IsInsert/IsRemove/IsChange, the selected-item/index and data-source properties, CurrentUnitSystem, ctor, IsCustomEqualsBase, Commit, HandleRbCustomClicked, BuildDerivedInsertDataSource, BuildDefaultViewLists, BuildCustomViewLists, CustomBaseChange, ShowDimEditDialog, IsUserDiscardChanges, HandlePostCustomBaseOnSelectionChanged. Fields stay generated.
  - Gate: PASS, 567 checked. Report: `.mirror-cache/release-il-03373819a20e/release-il.json`.
  - Shapes: members are in Mathcad's metadata order (the lambda ordinal `<BuildDerivedInsertDataSource>b__20_N` changed to `b__61_N` when the members were in a different order). `Commit` is a pattern `if (... is WorksheetControl worksheetControl)` around the body (the early-return form differs).
  - Blocked, stub kept: `HandleDialogAction`. Its nullable `switch` temp takes local slot 1 (Mathcad uses slot 3, after the case-block locals), and the `!ShowDimEditDialog || (...)` guard lays out its branches differently (Mathcad: `bne.un` to the method end). No source form tried so far matched both.
- **Batch 14 (TextStyleEditDialogViewModel, U; branch `mirror/quire-main-rest-pure-20261111`):** `TextStyleEditDialogViewModel` except two members kept as unmarked placeholders: `FontFamily` (its setter needs `FontConstants`, not in Quire) and `UpdateMarkerStyle` (its `MarkerStyle` and `LastBlock` pattern temps take different local slots than Mathcad's). Fields and consts stay generated; `_okIsEnabled` moved with its initializer.
  - Gate: PASS, 618 checked. Report: `.mirror-cache/release-il-69b38af43a33/release-il.json`.
  - Shapes: `(double)` casts where Mathcad emits `conv.r8` on `Nullable<double>.Value`; `textStyleData?.BaseOnKey != null` (Roslyn emits `call` for the property as Mathcad does; `!= null` after `&&` emits `callvirt`); a two-case enum `switch` written as `if`/`else if` (Mathcad lays out the first case body before the second test); `ResetValue` tests `is TextDecorationCollection textDecorations`. Unmarked placeholders keep the constructor's lambda ordinal (`<.ctor>b__20_0`) because a generated `UpdateMarkerStyle` stub counted before the constructor.
- **Batch 15 (LocalizedFontNameConverter, U; branch `mirror/quire-main-rest-pure-20261111`):** `Instance`, `FontNames`, the two field initializers (`_collectedFontsDict`, `_missingFontsDict`), `ConvertBack`, `IsUniMathFontExist` and `DoesFontFamilyExist` (with their local functions). Blocked, stubs kept: the private constructor and `Convert` (they need `FontConstants` and `XmlLanguageHelper`, neither in Quire).
  - Gate: PASS, 626 checked. Report: `.mirror-cache/release-il-10dcac6b483c/release-il.json`.
- **Batch 16 (MRU helpers, U; branch `mirror/quire-main-rest-pure-20261111`):** `MruFileExtensionComparer` (Equals, GetHashCode, IsMctxExtension, ctor) and `MruAccessKeyConverter` (Convert, ConvertBack, GetDisplayedName, ctor). Both use Mathcad's `string.Format("_{0}. ", n) + name` form for the access key; the interpolated string emits `DefaultInterpolatedStringHandler`.
  - Gate: PASS, 632 checked. Report: `.mirror-cache/release-il-5416afeb2492/release-il.json`.
- **Batch 17 (ProofingLanguageDataItem, U; branch `mirror/quire-main-rest-pure-20261111`):** constructors, ToString, Culture, Value, Header, IsChecked, PropertyChanged event. Fields stay generated. Blocked, stub kept: `GetCultureName` (reads `Settings.Default.ShowNativeProofingLanguage`, a settings value).
  - Gate: PASS, 640 checked. Report: `.mirror-cache/release-il-1a79f4cec4c2/release-il.json`.
- **Batch 18 (DialogManager, U; branch `mirror/quire-main-rest-pure-20261111`):** both `ShowDialog` overloads, `IsConnected`, `IsDialogOpen`, ctor. Gate: PASS, 646 checked. Report: `.mirror-cache/release-il-460a79782701/release-il.json`.
  - Blocked (not started): `MruUserConfigPersistance` (reads `Settings`, a settings class).
- **Batch 19 (MessageBoxManager, U; branch `mirror/quire-main-rest-pure-20261111`):** the four private `Show` overloads, all `ShowWarning/Question/Error/Info` overloads, `ShowQuestionWithOptions`, `ShowOkDialog`, `IsDisabled`, the four delegate properties, `IsConnected`, ctor. Gate: PASS, 672 checked. Report: `.mirror-cache/release-il-7ea8d6de9d01/release-il.json`.

## Remaining (after batch 10)

- Markers left in `rest` (non-Controls): 3152 in 276 files after batch 10 (3317 at classification). Batches 1–10 removed 165 markers.
- **Blocked, not attempted:** COM/native class factories and registration (`IClassFactory*`, `ComNativeExternalApi`, `MathcadPrime*ClassFactory*`, `MathcadPrimeComServerManager`, `LICINFO`, `AddonsLoaderHelper` P/Invoke); settings and product literals (`Properties.Settings`, `ProductInfo.Constants`, `CultureHelper`).
- **Blocked, type/surface decisions:** `WindowTabTupleData.Header` (type-map `DockWindow` → `AvalonDock.Layout.LayoutAnchorable` has no `Header`); `FontConstants` is not in Quire (`StyleFontFamilyConverter`, `MathFontSizeValidationRule`, `TextStyleProperties`, `MathSymbolMenuItemConverter`); `IPageNumberControl`, `ISavedDateControl`, `XmlLanguageHelper` are not in Quire (`PageNumberIsCheckedConverter`, `SavedDateIsCheckedConverter`, `XmlLanguageToCultureInfoConverter`); `TextStylePropertyInfoExt.IsInherited` is not in Quire.Core (`TextStyleInheritedFontWeight`).
- **Mismatch kept as stub:** `UniqueTextStyleNameRule.Validate` (Mathcad uses `call` for `String.IndexOfAny`; Roslyn emits `callvirt`).
- **XAML (78 files, 1336 markers):** `InitializeComponent`, `Connect`, `_contentLoaded` and XAML-named fields stay blocked. Code members in those types are not yet attempted.
- **Automation (65 files, 834 markers):** not attempted; per-member work after the code batches.
- **Pure code still to do (about 850 markers in about 110 files):** view models (`TextStyleEditDialogViewModel`, `SystemUnitEditDialogViewModel`, `CustomMarginsViewModel`, `ColumnViewModel`), `MruFiles`, `Behaviors`, `MessageBoxes`, `LocalizedFontNameConverter`, `ProofingLanguage*`, `AppCommands`, `IntegrationCommands`, and the WPF control classes.
