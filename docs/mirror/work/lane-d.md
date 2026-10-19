# Lane D — Ptc.PersistentData* in Ptc.Core

## Active handoff

- **Batch:** `mirror/d-persistentdata-oleutils`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`.
- **Scope:** `Quire.PersistentData.OleUtils`: `CanCreateExcelContainer`, `GetExcelSheetGuid`, `IsMicrosoftEquationGuid`, `IsOleAppSupportZoom`, `IsSupportedExcelGuid`, `IsVersionInstalled`, `ProgIDFromCLSID`, `get_IsExcel8Installed`, plus the unmapped Tier A `.cctor()` required to initialize their GUID dependencies. The initializer’s Tier B GUID fields and chart conversion map are initialized as part of that required dependency.
- **Evidence:** [OleUtils contract](../evidence/Ptc.PersistentData.OleUtils.md). Original `Ptc.Core` is version 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`; exact member tokens and signatures are recorded there. The artifact-bound ILSpy reference is in ignored `.mirror-cache/Ptc.Core/d60141dbcf757ae6/`.
- **Validation:** `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcPersistentDataOleUtilsDifferentialTests'` — passed, 1 passed, 0 failed, 0 skipped. The test directly compares all nine Tier A members and initialized GUID/dictionary state. ProgID and `ole32.dll` calls compare against Mathcad on the same machine; results depend on local COM registration and do not assert an application is installed.
- **Gate:** not yet run. Commit and push this branch, then run `python C:\Projects\Quire\tools\validation\lane_merge.py mirror/d-persistentdata-oleutils --jobs 14`. Keep the candidate fixed while the gate runs.
- **Remaining:** after merge, fetch `origin/main`, inspect branches and worktrees again, and select the next eligible unowned Tier A class in the lane 4 area. Do not infer ownership from the inventory alone.
- **Blockers:** none known; runtime gate is pending.
