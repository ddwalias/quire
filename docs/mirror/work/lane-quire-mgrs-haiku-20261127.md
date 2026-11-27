# Lane Quire managers (haiku-quire-mgrs-20261127)

- Owner: dispatched worker task_f004b9e9e7a5. Branch `mirror/quire-mgrs-haiku-20261127` from main `b61df4896`.
- Landed (27 members, Release IL PASS for the changed members; project failures unchanged at 129):
  - AddonsCollection: constructor, LoadAddonsAndAddToCollection, StartOnIdleTimeProcessing, OnIdleTimeProcessing, UpdateRibbonMenuItems, DisableInRibbonAllAddonsMenuItems, SetAddonsMenuItemsStatus, ForcePtcPlacesEnableIfNeeds, CallAddonsToGetResolvedFileNameForOpen, CallAddonsOnOpenDialogOpening (both overloads), CallAddonsOnOpenDialogClosing, CallAddonsOnSaveDialogOpening, GetInitialDirectoryOnOpen/OnSave, CallAddonsToGetDefaultDirectory, CallAddonsToOpenFromWindchill, GetDisplayNameForDocName, CallAddonsToGetDisplayName.
  - Addon: constructor (`_hModule` field initializer), AddOnId, LoadAddon, CallAddonInitialize, CallAddonToGetResolvedFileNameForOpenAlloc, CallAddonToGetDefaultDirectory, CallAddonUpdateMenuItem, CallAddonToCheckIsAddOnActive.
- Parked (literal or name mismatch, one line each):
  - AddonsProperties registry and resource members: Mathcad literals `Software\PTC\Mathcad Prime\...`, `PTC Places`, and the `MathcadPrime;component` pack URI differ from Quire's renamed constants; needs a decision on literal renaming (D21).
  - Addon `CallAddonToOpenFromWindchill`, `CallAddonToEnablePtcPlaces`, `CallAddonSaveToPtcServer`: exported names `getPTCPlacesLocation`, `enablePTCPlaces`, `saveToPTCServerForPrime` differ from Quire constants.
  - AddonsCollection `GetDisplayTitle`: calls `SpiritMainWindow.get_OpenDocuments`; Quire exposes `OpenDocumentsList` (window lane).
  - AddonsCollection `CallAddonsWhenDocumentOpened`/`Closing`: branch layout (previously parked).
- Not started: AddonsProperties members other than the non-literal ones, TabManager, MathcadPrimeComServerManager, Behaviors.BaseUIFunctionalityAttachedBehavior.

## Batch 2 (branch mirror/quire-mgrs-haiku-20261127b)

- Landed members (20 method and accessor rows): MathcadPrimeComServerManager Instance, PrimeApp, PrimeWindow, IsInitialized, ListOfControlledWorksheets, AddWorksheet, RemoveWorksheet, GetDialogManager, GetMessageBoxManager, DisableMessageBoxManager, Succeeded, plus auto-property accessors for CallsCounter, RuntimeKey and RuntimeKeyHex; AddonsProperties GetAddonsProperties, GetAddonPath, CheckIfAddonsShouldBeLoaded; AddonsCollection CopyViewable, CallAddonsToGeneratePdfFile; TabManager IsIndexInCollectionRange.
- Gate: none of the changed members fails; project failures are the pre-existing SpiritMainWindow stubs (18).
- Parked (unchanged): the AddonsProperties registry literals and resource URI, the Addon renamed-export members, GetDisplayTitle, CallAddonsWhenDocument*.

## TabManager blockers (coordinator question, 2026-11-27)

- `DebugTools.DevAssert`: present in Quire.Core (`DebugTools.cs:62`, `(bool, string, params object[])`, returns `AssertionResult`); not a missing member. The IL-level difference, if any, is in the call shape of the members that use it, not the signature.
- `WpfUtils.ExecuteOnLayoutUpdated`: present in Quire.Core (`Wpf/WpfUtils.cs:229`, `(UIElement, Action)`); not a missing member.
- DockWindow (DevComponents.WpfDock): stays parked (D6).

## Batch 3 (branch mirror/quire-mgrs-haiku-20261127c, not landed)

- Gate: Release IL passes for the batch members (1897 checked; 44 failures in SpiritMainWindow and Automation types from other lanes after main moved).
- Members (11): AddonsCollection CallAddonsWhenDocumentOpened, CallAddonsWhenDocumentClosing; ComServerManager SetComServerReady, EnableMessageBoxManager, Initialize; BaseUI GetUIFunctionalityBehaviorKey, SetUIFunctionalityBehaviorKey, UpdateUIElementImageForButtonDropDown; TabManager OnApplyTemplate, ItemsCollection, TabItemContainerAtIndex.
- Parked in batch 3: BaseUI `GetImage` (Mathcad's `brtrue` targets an inline `return image` block; two layouts tried, both differ at #4); BaseUI `.ctor` (Interactivity vs Xaml.Behaviors base type); TabManager `get_ViewItems`, `set_SelectedViewIndex`, `GetTabIndexForWindow`, `GetDataTupleForWindow` (DockWindow, D6); ComServerManager `InvokeDelegate` (retry loop with try/catch around Dispatcher.Invoke; not attempted), `ClosePrimeWindow` (cached `Ptc.Delegate0` closure).
- Remaining short of 20: 9 members, which need the parked shapes above or DockWindow.
