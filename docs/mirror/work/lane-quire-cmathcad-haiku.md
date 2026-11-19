# Lane Quire root, CMathcadPrime automation types (haiku)

- Owner: Orca task `task_25ffc201f6c6` (dispatched worker).
- Branch: `mirror/quire-cmathcad-haiku-20261119`, from main `c8c869918`.
- Scope: `src/Quire/Generated/Mirror/AutomationServiceEngNote.CMathcadPrimeApplication.g.cs` (116 stubs at start) and `AutomationServiceEngNote.CMathcadPrimeWorksheet.g.cs` (201 stubs at start). Hand-written partials: `src/Quire/Mirrored/AutomationServiceEngNote.CMathcadPrime{Application,Worksheet}.cs`.

## Landed (Release IL, `release_il.py --type`; gate run 5 report `release-il-af57646d720e`)

- CMathcadPrimeApplication: 42 hand-written members exact, including fields, both constructors, `IsEventsInitialized`/`IsEventsInitialized2`, `ActiveWorksheet`, `Worksheets`, `GetVersion`, `CreateWorksheetReadonlyOptions`, `Activate`/`DoActivate`, `DoGetVisible`/`DoSetVisible`, `DoGetActiveWorksheet`, the `On*` document handlers, `FireOn*` helpers, `GetOpenedDocument` (loop bound `> -1`), `DoCloseAll`, `InitializeEvents`/`InitializeEvents2`, `SubscribeEvent`, `UnsubscribeEvent`, `ShowPopupError`, `ReleaseComEvents`.
- CMathcadPrimeWorksheet: 29 hand-written members exact (constructor, `_isUiInteractiveModeDisabled`, `Name`, `FullName`, `IsReadOnly`, `DoIsReadOnly`, `DoIsDocumentOpen`, `IsOpen`, `GetAssociatedEngineeringDocument`, `DoSetModified`, `DoSetTitle`, `GetTitle`, the `DoSet*` setters, `DoSynchronize`, `DoPauseCalculation`, `DoResumeCalculation`, `DoActivate`, `ActivateWorksheet`) plus the nested `WorksheetData` partial (constructor, `DocumentFullName`, `DocumentName`, `UpdateWorksheetData`, `SetIsComClientControlled`, `UnsetIsComClientControlled`, `IsComClientControlled`), which are exact.

## Parked (stubs kept)

- Worksheet `IsAssociatedEngineeringDocument`: flat `fp == null && ...` / `else if` form emits 55 IL entries against Mathcad's 52 (Mathcad re-tests the null first-clause in the second clause). Not resolved in one attempt.
- Worksheet getters `DoGetModified`, `DoGetTitle`, `DoGetWorksheetTabIcon`, `DoGetWorksheetTabName`, `DoGetClosingPrompt`, `DoGetWorksheetDisplayedFilePath`, `DoGetWorksheetWorkingDirectory`: `doc != null ? doc.X : null` emits `brtrue` at the first branch; Mathcad emits `brfalse` on the local. One ternary attempt; no matching form found yet.
- Application closures (`Visible` setter, `SubscribesApplicationEvents`, `UnsubscribesApplicationEvents`, `OnPingClient`, `Quit`, `DoTryToCloseMathcadPrime`, `Open`/`DoOpen`/`OpenEx`, `CloseAll`, `UpdateInputs`, `SetContextData`): display-class/`<>c` compile-order ordinals; stay stubs. `Visible` property stays a stub as a whole because its getter and setter are one member.

## Remaining

- CMathcadPrimeApplication: 24 stub attributes left (the closures above and the `Visible` property).
- CMathcadPrimeWorksheet: 146 stub attributes left, not attempted (`DoClose`, `DoSave*`, `Save*`, `SetRealValue`/matrix/string/S-expression getters and setters, `DoGetWorksheetReadOnlyOptionValue` (switch), `Close`/`DoClose` (switch and loop), value-getter families, `Modified`/`WorksheetTabIcon`/etc. property wrappers, `SetTitle` and `Synchronize`/`PauseCalculation`/`ResumeCalculation` public wrappers (closure/delegate shapes not checked)).
