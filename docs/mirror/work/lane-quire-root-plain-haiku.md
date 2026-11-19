# Lane Quire root, plain files (MathcadPrime.dll) — Quire

- Owner: Orca task `task_87291e5b13e6`.
- Branch: `mirror/quire-root-plain-haiku-20261119`, from main `74e9462c4`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/quire-root-plain-haiku`.
- Set: generated stubs in `src/Quire/Generated/Mirror` from `AboutWindow` to `DropDownSelector`, non-Automation, whose file has no `InitializeComponent` (19 files). Parked by a prior lane and kept as stubs: CompositSymbolDescriptorCollection, CMathcadPrimeWorksheetReadonlyOptions, ButtonDropDownSelection, DimensionEditDialogViewModel.

## Batch 1 — CMathcadPrimeWorksheets (`ef1e25385`)

- Members: field `_isUiInteractiveModeDisabled`, `.ctor`, `Count`, `DoGetCount`, `Item`, `DoGetOpenItemByIndex`, and the `<Item>b__0` closure (through the `try` return shape).
- Release IL: PASS (`--type Quire.CMathcadPrimeWorksheets`), 799 checked, 0 failures.

## Batch 2 — UIFunctionalityWithoutCommandAttachedBehavior (`dd012ed6f`)

- Members: field `_uiElementBehaviorKey`, `FunctionalityKey` get/set, `OnFunctionalityServicePropertyChanged`, `.ctor`.
- Parked: `ButtonDropDownOnPreviewOpen` — `AssociatedObject` resolves to `Microsoft.Xaml.Behaviors.Behavior`1` in Quire, but Mathcad calls `System.Windows.Interactivity.Behavior`1`; the base class is in `Behaviors.BaseUIFunctionalityAttachedBehavior.g.cs` (not this lane's file).
- Parked: `SetAndUpdateElementBehavior`, `OnLoaded` — `DevComponents.WpfRibbon.ButtonDropDown` (`isinst`) is not referenced by Quire; the Fluent `DropDownButton` has no `Role`, `IsCheckable`, `OptionGroup`, `Image`.
- Parked: `FunctionalityKeyProperty` and `.cctor` (WPF DependencyProperty static).
- Release IL: the three changed members are not in the failure list (805 checked); the remaining failures are stubs.

## Batch 3 — UIFunctionalityWithCommandAttachedBehavior (`b346d5a42`)

- Members: `AssociatedObjectAsCommandSource` (auto-property), `CommandTarget` get/set, `ShouldReplaceToFreemiumIcon`, `OnCommandTargetChanged`, `.ctor`.
- Parked: `OnLoaded` (closure `<OnLoaded>b__0`, and the `Behavior`1` base mismatch above), `UpdateElementBehavior` (`DevComponents.WpfRibbon.ButtonDropDown` isinst), `CommandTargetProperty` and `.cctor`.
- Release IL: the five changed members are not in the failure list (810 checked).

## Batch 4 — ScrollViewerGrid (`880152f40`)

- Members: `_scrollViewer`, `_clippingRectangle`, `_scrollViewerToolTipHeader` fields; `EngineeringDocumentInstance` (auto-property, private setter); `ScrollViewerToolTipHeader` (`??=` resource lookup); `Worksheet`; `.ctor`.
- Parked: `SetTooltipAndMinScrollbarThumbSize`, `UpdateEngineeringDocument`, `OnStartingPageNumberChanged`, `StopUpdates`, `OnScrollChanged`, `UpdatePageNumbersTooltip`, `OnPagesCollectionChanged` (not started in this batch).
- Release IL: the five changed members are not in the failure list (820 checked).

## Batch 5 — AddonsProperties+AddonsData (`967b93525`)

- Members: `AddonsData` fields `_path`, `_id`, `.ctor`, `Path`, `ID`. Release IL: the three members are not in the failure list (825 checked).
- Parked: the registry members (`OpenAddonsRegKey`, `CreateAddonsRegKey`, `OpenRegKeyByBaseKey`, `GetPtcPlacesConnectionStatus`, `SetPtcPlacesConnectionStatus`, `CheckIfAddonsShouldBeLoaded`, `GetAddonPath`). Mathcad's literal is `Software\PTC\Mathcad Prime\Mathcad Prime 1.0\Addons`, but the Quire constant `AddonRegKey` is `Software\Quire\Quire\Quire 1.0\Addons`. Matching needs a maintainer decision on the key string; not changed here.
- Parked: `ReadOpenFromWcCmdText` (ResourceDictionary; not attempted), `GetAddonsProperties` (List and loop; not attempted), `.cctor` and static fields (static-constructor members).

## Batch 6 — FlexingStackPanel (`b396bcbe7`)

- Members: `Orientation` get/set (DependencyProperty), `SetSizeType`, `GetSizeType`, `.ctor`. Release IL: not in the failure list (827 checked).
- Parked (triaged, not attempted): `MeasureOverride`, `ArrangeOverride` (stub base calls), `Measure`, `GetMinSize`, `SubtractSize`, `GetStrechLenght`, `Max<T>` (lambda closures `<MeasureOverride>b__7_*` and `<GetStrechLenght>b__13_*` need compile-order ordinals; `Max<T>` maps to `System.Linq.Enumerable.Max`).

## Remaining (not done; set incomplete)

Not attempted: App (13 stubs), AppCommands (static constructor only; its cctor is hand-written already, and the static field stubs remain), AddonsControls (19), Addon (28 incl. native delegates), AddonsCollection (38), CMathcadPrimeApplication (53), CMathcadPrimeWorksheet (100), AllFunctionsBrowserTreeViewUIFunctionalityAttachedBehavior (13), BaseUIFunctionalityAttachedBehavior (about 20; `.ctor` blocked: base-class type `Microsoft.Xaml.Behaviors.Behavior`1` vs Mathcad `System.Windows.Interactivity.Behavior`1), ScrollViewerGrid (7 stubs left), AddonsProperties (registry members parked above).

Project-level blocker for Behaviors: `Behaviors.BaseUIFunctionalityAttachedBehavior.g.cs` derives from `Microsoft.Xaml.Behaviors.Behavior<T>`; Mathcad's `System.Windows.Interactivity.Behavior<T>` appears in every behavior's `AssociatedObject` call and base constructor call. Owned by the base-class file, not this lane.


Not started: Controls.FlexingStackPanel, Behaviors.BaseUIFunctionalityAttachedBehavior, Behaviors.AllFunctionsBrowserTreeViewUIFunctionalityAttachedBehavior, App, AppCommands, AddonsProperties, AddonsControls, Addon, AddonsCollection, CMathcadPrimeApplication, CMathcadPrimeWorksheet.

## Batch 7 — App (`73b064e07`), AddonsControls (`987443771`)

- App: `OpenFileName` (static property over `Application.Current.Properties`), `RemoveDuplicatedEnvironmentVariables`, `LoadAppConfigurations`, `.ctor`. Gate PASS for these members (971 checked).
- Parked in App: `PrimeStartup`, `ConnectToExistingAppOrInitializeNewWithMutex`, `ShowWindowAndInitializeComServer`, `SetupServiceProviders`, `SetupCultureInfo`, `ShowSplashScreen*` (closures and out parameter; not attempted this pass).
- AddonsControls: constructor, `BindCommands`, all handler/can-execute methods. Gate PASS (983 checked, 0 failures).

## Batch 8 — Addon (`fee6f8450`), ScrollViewerGrid StopUpdates (`567ea2630`)

- Addon: `UnloadAddon`, `GetAddressOfFunction`, `CallAddonCleanup`, `CallAddonOnIdle`, `CallAddonWhenDocumentOpened/Closing/Saving/Saved`, `CallAddonOnCommand`. Gate: not in the failure list (1004 checked).
- Parked in Addon: `.ctor` (Mathcad calls the `AddOnId` setter; a Roslyn auto-property assignment emits `stfld`), `AddOnId` getter/setter (the private setter; not attempted), `CallAddonToOpenFromWindchill`, `CallAddonToEnablePtcPlaces`, `CallAddonSaveToPtcServer` (Mathcad's export names `getPTCPlacesLocation`, `enablePTCPlaces`, `saveToPTCServerForPrime` differ from the Quire constants `getQuirePlacesLocation`, `enableQuirePlaces`, `saveToQuireServerForPrime`; the exported names must match), `CallAddonInitialize`, `CallAddonToGetResolvedFileNameForOpenAlloc`, `CallAddonToGetDefaultDirectory`, `CallAddonUpdateMenuItem`, `CallAddonToCheckIsAddOnActive` (not attempted; uint/bool conversions), `LoadAddon` (not attempted).
- ScrollViewerGrid `StopUpdates` and `OnStartingPageNumberChanged`: not in the failure list (1008 checked). Still stubs: `SetTooltipAndMinScrollbarThumbSize` (DevComponents.WpfRibbon.SuperToolTip, not referenced by Quire), `UpdateEngineeringDocument`, `OnScrollChanged`, `UpdatePageNumbersTooltip`, `OnPagesCollectionChanged` (not attempted).

## Remaining (set still incomplete)

Not attempted: AddonsCollection (38 stubs), AllFunctionsBrowserTreeViewUIFunctionalityAttachedBehavior, BaseUIFunctionalityAttachedBehavior (its `.ctor` is blocked by the `Microsoft.Xaml.Behaviors.Behavior<T>` vs `System.Windows.Interactivity.Behavior<T>` mismatch), CMathcadPrimeApplication, CMathcadPrimeWorksheet (both reassigned to this worker), AppCommands (cctor only), and the App closures listed above, plus the Addon and ScrollViewerGrid items above.

## Batch 9 — AddonsCollection (`731394cd4`, `fa468cab5`)

- Landed members: `CallAddonsAtQuit`, `CallAddonsToEnablePtcPlaces` (the flag reset sits in the `else`, which is the layout Mathcad emits), `CallAddonsOnCommand`, `CallAddonsUpdateMenuItem`, `IsAddonsCollectionNotEmpty`, `CallAddonsWhenDocumentSaving`, `CallAddonsWhenDocumentSaved`. Gate: none of them in the failure list (1051 checked).
- Parked (stubs kept, branch layout differs): `CallAddonsWhenDocumentOpened` (the Mathcad false paths jump to a different target than Roslyn's `&&` form) and `CallAddonsWhenDocumentClosing` (the `Remove` block placement differs).
- Still stubs: the constructor, `LoadAddonsAndAddToCollection`, idle-timer members, ribbon members, dialog and file-name members, `CallAddonsToOpenFromWindchill`, `CallAddonsSaveToPtcServer`, `GetActiveAddOnId`, and the rest of the AddonsCollection list (not attempted).
- Gate note: one attempt failed on an MSBuild OutOfMemoryException in an unrelated project; the rerun passed the same build.
