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
