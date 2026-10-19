# Lane C work checkpoint

## Scope and ownership

Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. This assignment removes both assemblies from Lane A's future scope. No other lane's active batch or files are part of this work.

The initial ownership audit found no Calculation-named active branch or worktree. `mirror/gen-calc-core` is already contained in `main`; the existing generated mirrors and evidence are the starting point, not duplicated work.

## Active handoff

**Next:** The 35-member Core MemberGen batch on `mirror/c-calc-simple-result-factory-cctor` now passes generated-path validation, but its gate stops before MethodAudit/ratchet because the focused Release pass looks for `Quire.Focused.Differential.Tests.dll` instead of the project's `Quire.Differential.Tests.dll`; exact report and six broad-sweep comparison gaps are in Validation and blockers. The remaining cctor list is reconciled as follows: `DefaultUnitDescriptorTable` is Tier B and excluded; `EngineErrorMessages` is already touched by the gated MemberGen batch; `CalculationController` is owned by its separate worktree; `ResultManager::.cctor()` is implemented and Release-verified by this branch. The next unowned Tier A work should be selected after gating this branch, using current statuses, dependencies, direct test evidence, and ownership. Do not hand-write Tier B members.

The mapped Tier A methods already present in `tier-a.txt` and their evidence are not presumed complete without verifying status, direct comparison coverage, and exact Release IL for each type selected. Check dependencies and existing tests before each batch.

## Completed batches

### Core singleton constructors (`mirror/c-calc-core-cctors`)

Implemented and directly compared the static constructors for `CalculatingStatusChangedEventArgs`, `ErrorChangedEventArgs`, and `SyncStatusChangedEventArgs`. Their original IL is a `newobj`/`stsfld`/`ret` initializer; all three Release IL bodies match exactly. Evidence is in the three corresponding `Ptc.Calculation.Core.*.md` records.

- Differential command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj --filter 'FullyQualifiedName~PtcCalculationCoreSingletonCctorsMechanicalDifferentialTests|FullyQualifiedName~PtcCalculationCoreCalculatingStatusChangedEventArgsMechanicalDifferentialTests|FullyQualifiedName~PtcCalculationCoreErrorChangedEventArgsMechanicalDifferentialTests|FullyQualifiedName~PtcCalculationCoreSyncStatusChangedEventArgsMechanicalDifferentialTests' --no-restore --verbosity quiet` — passed 9, failed 0, skipped 0.
- Release audit: `Quire.Calculation.Core` Release build; MethodAudit inspected 990 declarations with 0 inspection errors. Exact constructors: 3/3 after (0/3 mapped before; all three were unmapped in the base Tier A inventory).
- A broader diagnostic run also selected existing `ResultManager` and `SimpleResultNodeFactory` differential classes. Six existing tests failed because their Tier B/other-type state is incomplete; those types are recorded above as dependencies to resolve before their cctors can be verified.
- An earlier focused run exposed a test renderer namespace mismatch; the final renderer compares null, simple runtime type name, and inherited `PropertyName` where available.
- The `EngineError.Message` inventory entry is a field, while the tier definition excludes fields; it is not part of this Tier A scope.

### ResultManager initialization (`mirror/c-calc-core-cctors`)

Implemented Tier A `.ctor()` and `Clear()` from the helper-generated original IL and added the singleton `.cctor()` field initialization. The constructor delegates to `Clear`; `Clear` sets both counters to -1 and creates an empty result dictionary; the `.cctor()` constructs and stores the singleton. This resolves the prior default-field state gap without implementing its Tier B result mutation methods.

- Release Mathcad comparison: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationCoreSingletonCctorsMechanicalDifferentialTests|FullyQualifiedName~PtcCalculationCoreResultManagerMechanicalDifferentialTests.ctor_0600014e|FullyQualifiedName~PtcCalculationCoreResultManagerMechanicalDifferentialTests.Clear_0600014f|FullyQualifiedName~PtcCalculationCoreResultManagerCctorMechanicalDifferentialTests' --no-build --verbosity quiet` — passed 6, failed 0, skipped 0.
- Release MethodAudit: 991 Core declarations, 0 inspection errors; constructor token `0x0600009e`, `Clear` token `0x0600009f`, and `.cctor` token `0x060000a0` are exact. Original constructor and `Clear` tokens are `0x0600014e` and `0x0600014f`. Artifact-bound IL and cases are in `Ptc.Calculation.Core.ResultManager.md`.
- The full existing ResultManager class filter has one pre-existing failure, `CreateOldResultRefId_0600015f`, because it calls the Tier B `AddOldResult` mirror stub. It is outside this batch and remains unimplemented.

## Validation and blockers

`origin/main` is `f863245e`. The MemberGen branch gate passed path checks, Harness 97/97, Mpl 188/188, and focused Differential 392/392, then stopped before MethodAudit/ratchet because it could not find the focused Release differential assembly at `C:\Users\DuyDang\AppData\Local\Temp\quire-audit-release-x7hltukg\Quire.Focused.Differential.Tests.dll`; the project output is named `Quire.Differential.Tests.dll`. Report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-dmijh3dp\run\summary.json`. The broad Release sweep of changed Core classes passed 145/151; the six failures are existing `GetIntegerFromAttribute`, `WriteMatrixXml`, `WriteDisplayMatrixXml`, and `StringListResultNode` constructor state gaps, recorded with exact tests in the MemberGen branch checkpoint. On this branch, the Release ResultManager targeted run passed 6/6, and MethodAudit found all three new method bodies exact (991 Core declarations, 0 inspection errors). Its full class filter has the unrelated Tier B `CreateOldResultRefId` failure described above. The normal lane gate for this ResultManager batch is next. No validation code, baseline, protected list, or other lane files were changed. Do not edit Lane A/B files or test baselines.
