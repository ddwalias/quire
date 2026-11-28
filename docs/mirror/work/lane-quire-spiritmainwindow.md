# Quire SpiritMainWindow lane checkpoint

- Owner: Orca worker `task_06cb9306969f`; branch `feature/quire-spiritmainwindow-20261127`.
- Batch 1: moved 11 field-like events into the hand-written partial; implemented their 22 accessors, 31 property/accessor methods, `InitializeComponent`, and `_CreateDelegate`. Generated twins were removed in the same change.
- Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 1,438 checked, 0 failures; report `.mirror-cache/release-il-552f7c850460/release-il.json`.
- Remaining: `IComponentConnector.Connect` stays a generated stub because connection ID 12 requires the unmapped `DevComponents.WpfDock.DockWindowGroup` field `_mainDockWindowGroup`; `LastCustomMarginValueSetting.set` stays a placeholder because the current idiomatic nullable comparison lowers differently. Closure/iterator members remain for a later batch and must use whole-type layout in Mathcad IL order.
- Batch 2: added 16 exact methods (`OpenStartPage`, command handlers, template helpers, and Prime activation handlers) in `SpiritMainWindow.Batch2.cs`; `HandleShowPropertyGrid` remains a generated stub because Roslyn emits `call Window.Show` for `new DebugPropertyWindow().Show()`, while Mathcad IL requires `callvirt`.
- Batch 2 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 1,454 checked, 0 failures; report `.mirror-cache/release-il-0b14a24be1a5/release-il.json`. Combined transition count: 62 newly exact methods across batches 1 and 2.
- Batch 3: added six exact event/window helpers (`HandleDocumentZoomIn`, `DocumentPropertyChanged`, `HelpManagerPropertyChanged`, `OnEngNotebookConnectionStatusChanged`, `HandleException`, `ContextMenuOpeningEventHandler`). `IsVisibleChangedEventHandler` remains a generated stub; its Mathcad `brfalse.s` target is one byte shorter than current Roslyn's target for the decompiled foreach/return form.
- Batch 3 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 1,460 checked, 0 failures; report `.mirror-cache/release-il-11c753140ddb/release-il.json`. Combined transition count: 68 newly exact methods across batches 1–3.
- Batch 4: added nine exact members for open-document notifications, settings, license handlers, read-only checks, and the input/output designation window. `GetXamlObject` remains a generated stub because its `XmlReader` using-local lowering differs from Mathcad by one local slot.
- Batch 4 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 1,648 checked, 0 failures; report `.mirror-cache/release-il-5cf9b808666b/release-il.json`. The current branch has 31 newly exact methods beyond the landed Batch 1 (77 cumulative across batches 1–4).
- Batch 5: implemented 18 exact members for template initialization, document notifications, help and dialog commands, window procedure hooks, and settings. `GetXamlObject` remains a generated stub because its `XmlReader` using-local lowering differs by one local slot; `AddHandlersForWorksheetInputOutputWindowEvents` remains a generated stub because Quire's `DockingManager` has no mapped `AfterDocked` event.
- Batch 5 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 1,742 checked, 0 failures; report `.mirror-cache/release-il-98ddd8e85f41/release-il.json`. This brings the cumulative exact implementation count to 95 across batches 1–5. Closure and iterator members still require a whole-type layout in Mathcad IL order.
- Batch 6: added 12 exact members for message dispatch, snapshot cursor handling, document load notifications, tab naming, warning commands, tooltip setup, find availability, and the automation peer.
- Batch 6 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 1,831 checked, 0 failures; report `.mirror-cache/release-il-a8e6ec12ffe0/release-il.json`. Cumulative exact implementation count: 107 across batches 1–6.
- Parked for mapping/layout: `SetTabHeaderBindingForNewDocument` requires `LayoutAnchorable.DataContext`; `SaveWindowState` requires the unmapped `RibbonControl.QuickAccessToolbarLayout`; `OnClosing` uses unmapped window fields `CommandBuffer` and `CalculationController`; `UnhookFromDocument` synthesizes `<0>__OnCWDChanged` where Mathcad expects `<6>__OnCWDChanged`; `AskCancelPrintInProgressDialog` still differs at nullable Boolean comparison lowering. Closure/iterator members remain for the whole-type Mathcad-order layout; anchor deltas are constructor 83, `AnyDocumentRequestToUpdateInputsEnabledCheck` 85, `GetOpenFilesList` 94, `OnFunctionalityServiceLicenseTypeChanged` 98, `BindCommands`/`AreAliasesValidForUpdateInputs` 104, `HandleLaunchConverter`/`HandleCloseTabs`/`GetFilterForCloseTabsEnum` 106, `HandleShowSaveAsError`/`ShouldPromptForFileNameOnSave`/`ExtractCaseInsensitiveDuplicates` 109, and `OnDictionaryUpdated` 121.
- Batch 7: added 17 exact members for window visibility, opening files/templates, worksheet scrolling, print and import commands, find navigation, Registry lookup, and Excel insertion. `Find` remains a generated stub after comparison-polarity variants still emitted the opposite branch; `HandleOpenTemplateDocument` remains a generated stub because the duplicate null/empty checks lower to a different branch shape.
- Batch 7 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 1,903 checked, 0 failures; report `.mirror-cache/release-il-a2f39720d7ed/release-il.json`. Cumulative exact implementation count: 124 across batches 1–7.
- Batch 8: added nine exact members for XAML object loading, active-document callback, tab/document lookup, template selection, find navigation, and unit/margin dialogs. `HandleShowPropertyGrid`, `LastCustomMarginValueSetting.set`, and `GetStringParameter` remain stubs after IL shape mismatches or missing `IButtonCommandParameter` mapping.
- Batch 8 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 1,987 checked, 0 failures; report `.mirror-cache/release-il-54d172d6c4e9/release-il.json`. Cumulative exact implementation count: 133 across batches 1–8.
- Batch 9: added ten exact save wrappers for API PDF export, save-as/copy, regular/non-Prime/worksheet-state save options, and instance save-filter lookup. `HandleSaveDocumentWithExtension` and `OnCWDChanged` remain generated stubs after one-branch/local-shape and call/dup IL mismatches.
- Batch 9 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 2,037 checked, 0 failures; report `.mirror-cache/release-il-1c37199e8475/release-il.json`. Cumulative exact implementation count: 143 across batches 1–9.
- Batch 10: added five exact file-open and save-filter methods; `HandleSaveDocumentWithExtension` now matches with an early return.
- Batch 10 Release IL: PASS, 2,060 checked, 0 failures; report `.mirror-cache/release-il-ce5d48a7b6be/release-il.json`. Cumulative exact implementation count: 148 across batches 1–10.
- Batch 11: implemented `SaveDocumentWithFilenameAndExtension`, `SaveDocument`, and `AskForSaveDefaultFileName`.
- Batch 11 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 2,063 checked, 0 failures; report `.mirror-cache/release-il-6ec5564449fd/release-il.json`. Cumulative exact implementation count: 151 across batches 1–11.
- Batch 12: implemented `LaunchOpenFileDialogAndGetFileNameForTemplates`, `CloneWorksheetAndOpenInDocument`, and `HandleSaveFileFailure`.
- Batch 12 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 2,066 checked, 0 failures; report `.mirror-cache/release-il-3e046847eafc/release-il.json`. Cumulative exact implementation count: 154 across batches 1–12.
- Batch 13: implemented six template/document methods: `HandleNewDocument`, `HandleNewDocumentFromExisting`, `HandleClonningAndOpenningWorksheetInNewDocument`, `HandleOpenTemplateDocument`, `OpenFileInDocument`, and the `NewDocument(DocumentReadonlyOptions)` overload.
- Batch 13 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 2,098 checked, 0 failures; report `.mirror-cache/release-il-07cd4e8c83bc/release-il.json`. Cumulative exact implementation count: 160 across batches 1–13.
- Parked because `MainDockWindowGroup` has no mapped Quire member: `NewDocument(bool, DocumentReadonlyOptions, bool)`, `ReplaceActiveTabWithDocument`, `ActiveTabChanged`, and `GetIndexOfTabForDocument`. The `NewDocument(DocumentReadonlyOptions)` overload remains a stub because its decompiled branch compiles to a target one byte shorter than Mathcad's.
- Batch 14: implemented `CanReplaceAndFindNext`, `HandleReplaceAndFindNext`, `Find`, and `HandleGoToPage`.
- Batch 14 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 2,119 checked, 0 failures; report `.mirror-cache/release-il-e7b8306b2c65/release-il.json`. Cumulative exact implementation count: 164 across batches 1–14.
- Batch 10: added five exact members for save-extension handling, static save/open filters, opening files, and file-open failure handling. `OnCWDChanged` remains a generated stub because Roslyn emits `callvirt` with a local where Mathcad emits `call` with `dup`.
- Batch 10 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 2,060 checked, 0 failures; report `.mirror-cache/release-il-ce5d48a7b6be/release-il.json`. Cumulative exact implementation count: 148 across batches 1–10.
- Parked: AskCancelPrintInProgressDialog still lowers Nullable<bool> comparison differently (Release IL: Quire stloc.1 where Mathcad emits ldc.i4.0/cgt.un); OnIsKeyboardFocusWithinChanged needs the unmapped Fluent RibbonControl. HandleDockWindowClosedEvent and OnDocumentTabIconChanged also need unmapped DockingManager.Detach and LayoutAnchorable.Image respectively.
- Batch 17: implemented `ClassifyWriteFailure` and `WndProc`; the classifier shares the false nullable path with the switch default to match Mathcad's branch layout.
- Batch 17 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 2,123 checked, 0 failures; report `.mirror-cache/release-il-4d1c971fef66/release-il.json`. Cumulative exact implementation count: 168 across batches 1–17.
- Batch 18: implemented `AskForSaveFileName` and `OnCWDChanged`; the null-conditional `?? true` form preserves Mathcad's `dup` branch shape for the static `Addons` field.
- Batch 18 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 2,125 checked, 0 failures; report `.mirror-cache/release-il-3e4f8ecb1ccb/release-il.json`. Cumulative exact implementation count: 170 across batches 1–18.
- Parked after Batch 18 compile probes: `HandleMoveTab`, `CanExecuteMoveTabCommand`, `CloseDocument`, and `RemoveTabAndDocument` require unmapped `MainDockWindowGroup`/`DockWindow` members; `OpenWorksheetInputOutputWindow` requires unmapped `InputOutputDesignationWindow` and `DockingManager` members. `EnsureLicenseManagerInstance` requires the unmapped `PtcLicensePlpf.LicenseImpl` type. The remaining closure and iterator members need the whole-type Mathcad-order layout before implementation.
- Batch 16: implemented `UnhandledDispatcherExceptionHandler` and `UnhandledExceptionHandler`; the latter uses an early return for the `_handlingException` guard to match Mathcad branch polarity.
- Batch 16 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 2,121 checked, 0 failures; report `.mirror-cache/release-il-5516d0949860/release-il.json`. Cumulative exact implementation count: 166 across batches 1–16.


## Closure ordinal follow-up tables

Reference: `Spirit.SpiritMainWindow.members.tsv` from MathcadPrime 12.0.1.0; each range is exclusive of the previous calibrated anchor and inclusive of the failing parent. Quire entries below are the corresponding declarations in `SpiritMainWindow.Ordered.cs`.

### `HandleLaunchConverter` (232 vs 233)

| Mathcad token | Kind | Mathcad member | Quire member |
|---|---|---|---|
| `0x060004f2` | method | `UpdateInputsEngNote` | `UpdateInputsEngNote` |
| `0x060004f3` | method | `UpdateInputs` | `UpdateInputs` |
| `0x060004f4` | method | `SetContextDataEngNote` | `SetContextDataEngNote` |
| `0x060004f5` | method | `HandleOpenFuncitonBrowser` | `HandleOpenFuncitonBrowser` |
| `0x06000597` | local function | `<HandleOpenFuncitonBrowser>g__SetDockControlSizeBeforeDocking|230_0` | exact local function with mapped argument type |
| `0x060004f6` | method | `SetForegroundWindow` | `SetForegroundWindow` |
| `0x060004f7` | method | `HandleLaunchConverter` | `HandleLaunchConverter` |

The only missing row was the local function (+1); the hand-written parent now emits it with the mapped `BeforeDockedRoutedEventArgs` type and an exact local function body, while the closure parent remains a placeholder.

### `GetFilterForCloseTabsEnum` (236 vs 237) and `HandleShowSaveAsError` (257 vs 258)

| Mathcad token | Kind | Mathcad member | Quire member |
|---|---|---|---|
| `0x060004f8` | method | `CanLaunchConverter` | `CanLaunchConverter` |
| `0x060004f9` | method | `HandleCloseEngineeringDocument` | `HandleCloseEngineeringDocument` |
| `0x060004fa` | method | `HandleCloseTabs` | `HandleCloseTabs` |
| `0x060004fb` | method | `GetFilterForCloseTabsEnum` | `GetFilterForCloseTabsEnum` |

| Mathcad token | Kind | Mathcad member | Quire member |
|---|---|---|---|
| `0x060004fc` | method | `HandleMoveTab` | `HandleMoveTab` |
| `0x060004fd` | property accessor | `get_MainDockWindowGroup` | `get_MainDockWindowGroup` |
| `0x060004fe` | method | `CanExecuteMoveTabCommand` | `CanExecuteMoveTabCommand` |
| `0x060004ff` | method | `HandleCloseEngineeringDocument` | `HandleCloseEngineeringDocument` |
| `0x06000500` | method | `LaunchImportExcel` | `LaunchImportExcel` |
| `0x06000501` | property accessor | `get_WorksheetInputOutputWindow` | `get_WorksheetInputOutputWindow` |
| `0x06000502` | property accessor | `set_WorksheetInputOutputWindow` | `set_WorksheetInputOutputWindow` |
| `0x06000503` | method | `HandleOpenInputOutputDesignationWindow` | `HandleOpenInputOutputDesignationWindow` |
| `0x06000504` | method | `OpenWorksheetInputOutputWindow` | `OpenWorksheetInputOutputWindow` |
| `0x06000505` | method | `HandleDockWindowClosedEvent` | `HandleDockWindowClosedEvent` |
| `0x06000506` | method | `AddHandlersForWorksheetInputOutputWindowEvents` | `AddHandlersForWorksheetInputOutputWindowEvents` |
| `0x06000507` | method | `SetBindingForWorksheetInputOutputWindowProperties` | `SetBindingForWorksheetInputOutputWindowProperties` |
| `0x06000508` | method | `InsertReadExcel` | `InsertReadExcel` |
| `0x06000509` | method | `DisplayHelp` | `DisplayHelp` |
| `0x0600050a` | method | `ToolTipOpened` | `ToolTipOpened` |
| `0x0600050b` | method | `HandleTooltipIsVisibleChanged` | `HandleTooltipIsVisibleChanged` |
| `0x0600050c` | method | `HandleShowPropertyGrid` | `HandleShowPropertyGrid` |
| `0x0600050d` | method | `HandleShowSaveAsError` | `HandleShowSaveAsError` |

Every method row in these two ranges is present in the same order in the partial; the +1 remains inherited from the `HandleOpenFuncitonBrowser` local-function row until the closure parents are re-enabled.

### `OnDictionaryUpdated` (378 vs 382)

The preceding calibrated closure parent is `ShouldPromptForFileNameOnSave` (`b__268_0`). The full MethodDef range is listed member by member below; direct methods and accessors are all present by name and order in the partial, so they do not explain the additional +4.

| Mathcad token | Kind | Mathcad member | Quire member |
|---|---|---|---|
| `0x06000518` | method | `SaveDocumentAsPdf_ForApiOnly` | `SaveDocumentAsPdf_ForApiOnly` |
| `0x06000519` | method | `SaveAndPushEngNote` | `SaveAndPushEngNote` |
| `0x0600051a` | method | `SaveDocument` | `SaveDocument` |
| `0x0600051b` | method | `HandleSaveDocumentWithInputsOutputsStates` | `HandleSaveDocumentWithInputsOutputsStates` |
| `0x0600051c` | method | `HandleSaveDocumentWithoutInputsOutputsStates` | `HandleSaveDocumentWithoutInputsOutputsStates` |
| `0x0600051d` | method | `SaveDocumentWithoutInputsOutputsStates` | `SaveDocumentWithoutInputsOutputsStates` |
| `0x0600051e` | method | `AskForSaveDefaultFileName` | `AskForSaveDefaultFileName` |
| `0x0600051f` | method | `GetFileSaveOpenFilters` | `GetFileSaveOpenFilters` |
| `0x06000520` | method | `GetFileSaveOpenFilters` | `GetFileSaveOpenFilters` |
| `0x06000521` | method | `GetFilterIdxForExtensionOrDefault` | `GetFilterIdxForExtensionOrDefault` |
| `0x06000522` | method | `AskForSaveFileName` | `AskForSaveFileName` |
| `0x06000523` | method | `ReviseFileExtension` | `ReviseFileExtension` |
| `0x06000524` | method | `ExtractCaseInsensitiveDuplicates` | `ExtractCaseInsensitiveDuplicates` |
| `0x06000525` | method | `GenerateInputOutputStatesOnSave` | `GenerateInputOutputStatesOnSave` |
| `0x06000526` | method | `TrySaveFile` | `TrySaveFile` |
| `0x06000527` | method | `OnCWDChanged` | `OnCWDChanged` |
| `0x06000528` | method | `OnZoomChanged` | `OnZoomChanged` |
| `0x06000529` | method | `OnDocumentModified` | `OnDocumentModified` |
| `0x0600052a` | method | `IsReadonly` | `IsReadonly` |
| `0x0600052b` | method | `HandleOpenDocument` | `HandleOpenDocument` |
| `0x0600052c` | method | `HandleNewDocumentFromExisting` | `HandleNewDocumentFromExisting` |
| `0x0600052d` | method | `HandleClonningAndOpenningWorksheetInNewDocument` | `HandleClonningAndOpenningWorksheetInNewDocument` |
| `0x0600052e` | method | `GetTemplateFileName` | `GetTemplateFileName` |
| `0x0600052f` | method | `HandleOpenTemplateDocument` | `HandleOpenTemplateDocument` |
| `0x06000530` | method | `IsDirectoryOneOfDefaultTemplateLocations` | `IsDirectoryOneOfDefaultTemplateLocations` |
| `0x06000531` | method | `LaunchOpenFileDialogAndGetFileNameForTemplates` | `LaunchOpenFileDialogAndGetFileNameForTemplates` |
| `0x06000532` | method | `SetAndDisplayWarning` | `SetAndDisplayWarning` |
| `0x06000533` | method | `CloneWorksheetAndOpenInDocument` | `CloneWorksheetAndOpenInDocument` |
| `0x06000534` | method | `LaunchOpenFileDialogAndGetFileName` | `LaunchOpenFileDialogAndGetFileName` |
| `0x06000535` | method | `OnDrop` | `OnDrop` |
| `0x06000536` | method | `OpenFile` | `OpenFile` |
| `0x06000537` | method | `ScrollToTagInDoc` | `ScrollToTagInDoc` |
| `0x06000538` | method | `TryOpenFileInDocument` | `TryOpenFileInDocument` |
| `0x06000539` | method | `HandleOpenFileFailure` | `HandleOpenFileFailure` |
| `0x0600053a` | method | `HandleSaveFileFailure` | `HandleSaveFileFailure` |
| `0x0600053b` | method | `DisplayCriticalWarningWithLogFile` | `DisplayCriticalWarningWithLogFile` |
| `0x0600053c` | method | `OpenFileInDocument` | `OpenFileInDocument` |
| `0x0600053d` | method | `ResetUntitledIndex` | `ResetUntitledIndex` |
| `0x0600053e` | property accessor | `get_UseDocumentTabs` | `get_UseDocumentTabs` |
| `0x0600053f` | property accessor | `set_UseDocumentTabs` | `set_UseDocumentTabs` |
| `0x06000540` | method | `UpdateCurrentShowWarningsFilter` | `UpdateCurrentShowWarningsFilter` |
| `0x06000541` | property accessor | `get_CurrentShowWarningsFilter` | `get_CurrentShowWarningsFilter` |
| `0x06000542` | property accessor | `set_CurrentShowWarningsFilter` | `set_CurrentShowWarningsFilter` |
| `0x06000543` | property accessor | `get_IsShowWarningsEnabled` | `get_IsShowWarningsEnabled` |
| `0x06000544` | property accessor | `set_IsShowWarningsEnabled` | `set_IsShowWarningsEnabled` |
| `0x06000545` | property accessor | `get_DisplayWarningsFilter` | `get_DisplayWarningsFilter` |
| `0x06000546` | property accessor | `set_DisplayWarningsFilter` | `set_DisplayWarningsFilter` |
| `0x06000547` | method | `NewDocument` | `NewDocument` |
| `0x06000548` | method | `CanGetDefaultBuiltInTemplateIfPossible` | `CanGetDefaultBuiltInTemplateIfPossible` |
| `0x06000549` | method | `CanGetDefaultBuildInTemplateFromDefaultLocationIfPossible` | `CanGetDefaultBuildInTemplateFromDefaultLocationIfPossible` |
| `0x0600054a` | method | `DoesTemplateFileExists` | `DoesTemplateFileExists` |
| `0x0600054b` | method | `CanUseTemplateForNewDocument` | `CanUseTemplateForNewDocument` |
| `0x0600054c` | method | `ChooseDefaultTemplateIfAnyForNewDocument` | `ChooseDefaultTemplateIfAnyForNewDocument` |
| `0x0600054d` | method | `NewDocument` | `NewDocument` |
| `0x0600054e` | method | `ReplaceActiveTabWithDocument` | `ReplaceActiveTabWithDocument` |
| `0x0600054f` | method | `ActiveTabChanged` | `ActiveTabChanged` |
| `0x06000550` | method | `GetIndexOfTabForDocument` | `GetIndexOfTabForDocument` |
| `0x06000551` | method | `GetTabForDocument` | `GetTabForDocument` |
| `0x06000552` | method | `GetDocumentForTab` | `GetDocumentForTab` |
| `0x06000553` | method | `GetApplicationFullWindowTitle` | `GetApplicationFullWindowTitle` |
| `0x06000554` | method | `CreateNewTabWithDocument` | `CreateNewTabWithDocument` |
| `0x06000555` | method | `LaunchCustomMarginsDialog` | `LaunchCustomMarginsDialog` |
| `0x06000556` | method | `LaunchSystemUnitDialog` | `LaunchSystemUnitDialog` |
| `0x06000557` | method | `HandleWorksheetControlDebuggerModeChange` | `HandleWorksheetControlDebuggerModeChange` |
| `0x06000558` | method | `SetTabHeaderBindingForNewDocument` | `SetTabHeaderBindingForNewDocument` |
| `0x06000559` | method | `GetUntitledHeader` | `GetUntitledHeader` |
| `0x0600055a` | method | `InitializeDocument` | `InitializeDocument` |
| `0x0600055b` | method | `OnDocumentLoaded` | `OnDocumentLoaded` |
| `0x0600055c` | method | `HandleDocumentPropertyChanged` | `HandleDocumentPropertyChanged` |
| `0x0600055d` | method | `HandleDocumentInputsOutputsSelected` | `HandleDocumentInputsOutputsSelected` |
| `0x0600055e` | method | `OnDocumentTabIconChanged` | `OnDocumentTabIconChanged` |
| `0x0600055f` | method | `UnhookFromDocument` | `UnhookFromDocument` |
| `0x06000560` | method | `OnDocumentFilePathChanged` | `OnDocumentFilePathChanged` |
| `0x06000561` | method | `ExitApplication` | `ExitApplication` |
| `0x06000562` | method | `DisplayAboutDialog` | `DisplayAboutDialog` |
| `0x06000563` | method | `DisplayOptionsDialog` | `DisplayOptionsDialog` |
| `0x06000564` | method | `UpdateTemplateLocationsIfNeeded` | `UpdateTemplateLocationsIfNeeded` |
| `0x06000565` | method | `DisplayHyperlink` | `DisplayHyperlink` |
| `0x06000566` | method | `OpenUrl` | `OpenUrl` |
| `0x06000567` | method | `ShowFailedToOpenUrlMessage` | `ShowFailedToOpenUrlMessage` |
| `0x06000568` | method | `DisplayFreemiumAboutBox` | `DisplayFreemiumAboutBox` |
| `0x06000569` | method | `AskCancelPrintInProgressDialog` | `AskCancelPrintInProgressDialog` |
| `0x0600056a` | method | `HandleToggleMultithreadingCalculationOption` | `HandleToggleMultithreadingCalculationOption` |
| `0x0600056b` | method | `UpdateWarningsDisplayFilterHandler` | `UpdateWarningsDisplayFilterHandler` |
| `0x0600056c` | method | `ToggleShowWarningsHandler` | `ToggleShowWarningsHandler` |
| `0x0600056d` | method | `OnCreateAutomationPeer` | `OnCreateAutomationPeer` |
| `0x0600056e` | method | `OnClosing` | `OnClosing` |
| `0x0600056f` | method | `SaveWindowState` | `SaveWindowState` |
| `0x06000570` | method | `AskToSaveDocument` | `AskToSaveDocument` |
| `0x06000571` | property accessor | `get_FindProperties` | `get_FindProperties` |
| `0x06000572` | method | `HandleFindNext` | `HandleFindNext` |
| `0x06000573` | method | `HandleFindNext` | `HandleFindNext` |
| `0x06000574` | method | `HandleFindPrev` | `HandleFindPrev` |
| `0x06000575` | method | `FindIsNotAllowedInFreemiumMode` | `FindIsNotAllowedInFreemiumMode` |
| `0x06000576` | method | `CanFind` | `CanFind` |
| `0x06000577` | method | `HandleReplaceAndFindNext` | `HandleReplaceAndFindNext` |
| `0x06000578` | method | `ReplaceIsNotAllowedInFreemiumMode` | `ReplaceIsNotAllowedInFreemiumMode` |
| `0x06000579` | method | `CanReplaceAndFindNext` | `CanReplaceAndFindNext` |
| `0x0600057a` | method | `Find` | `Find` |
| `0x0600057b` | method | `HandleGoToFind` | `HandleGoToFind` |
| `0x0600057c` | method | `HandleGoToPage` | `HandleGoToPage` |
| `0x0600057d` | method | `OnDictionaryUpdated` | `OnDictionaryUpdated` |

The direct MethodDef list has no missing named methods in this interval. The missing +4 is the compiler-generated nested type for `ShowFailedToOpenUrlMessage`, whose parent was a placeholder in Quire:

| Kind | Mathcad member in `<>c__DisplayClass358_0` | Quire before fix | Weight |
|---|---|---|---:|
| field | `<>4__this : SpiritMainWindow` | absent | +1 |
| field | `url : string` | absent | +1 |
| method | `.ctor()` | absent | +1 |
| method | `<ShowFailedToOpenUrlMessage>b__0(Window, string)` | absent | +1 |

The candidate implementation emitted the four members above, but its nested type was named `<>c__DisplayClass354_0`; Mathcad names it `<>c__DisplayClass358_0`. The Release gate therefore reports `ShowFailedToOpenUrlMessage` as non-exact (18 IL entries on each side, first difference at instruction 0), with the generated constructor and lambda method lacking Mathcad counterparts. The +4 member weight is accounted for by this closure shape, but the four-slot generated-type ordinal gap is not yet accounted for. The other direct methods/accessors in this MethodDef interval are present in the same order; the unresolved comparison is the nested generated type ordinal.

### `BindCommands` cached delegate field and lambdas

| Mathcad order | Member | Source of cached field or lambda | Quire finding |
|---:|---|---|---|
| 0 | `<>O::<0>__SpiritOptionPropertyChanged` | constructor method-group conversion | constructor is still a generated placeholder |
| 1 | `<>O::<1>__OnRibbonControlBeforeAddToQat` | constructor method-group conversion | constructor is still a generated placeholder |
| 2 | `<>O::<2>__HandleShowPropertyGrid` | `BindCommands` method-group conversion | prior implemented candidate emitted cached field `<0>`; current method is a placeholder |
| 0–4 | `<BindCommands>b__220_0` through `b__220_4` | five instance lambdas in prior candidate's `BindCommands` source order | matched Mathcad order; these do not create the static `<>O` cache fields |
