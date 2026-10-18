# Lane C work checkpoint

## Scope and ownership

Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. This assignment removes both assemblies from Lane A's future scope. No other lane's active batch or files are part of this work.

The initial ownership audit found no Calculation-named active branch or worktree. `mirror/gen-calc-core` is already contained in `main`; the existing generated mirrors and evidence are the starting point, not duplicated work.

## Active handoff

**Next:** Continue with independent Core static constructors, starting with the scalar constants in `CalcResultsProcessingPriorities` and `CalculationOptionDefaults`. Inspect the source and exact original IL before each change. `ResultManager::.cctor()` depends on a constructor whose object state currently differs from Mathcad (`_oldResultKey`/`_resultKey` are 0 instead of -1 and `_results` is null); use the member-generator workflow for that prerequisite. `SimpleResultNodeFactory` and its singleton cctor are deferred because existing factory method comparisons expose unmirrored result-node state. The inventory also lists 3 `Ptc.Calculation` `.cctor()` methods and `Ptc.Calculation.Core.EngineError::Message`; the latter is a field, not a method, and is outside Tier A. Do not hand-write Tier B members or edit protected acceptance lists.

The mapped Tier A methods already present in `tier-a.txt` and their evidence are not presumed complete without verifying status, direct comparison coverage, and exact Release IL for each type selected. Check dependencies and existing tests before each batch.

## Completed batches

### Core singleton constructors (`mirror/c-calc-core-cctors`)

Implemented and directly compared the static constructors for `CalculatingStatusChangedEventArgs`, `ErrorChangedEventArgs`, and `SyncStatusChangedEventArgs`. Their original IL is a `newobj`/`stsfld`/`ret` initializer; all three Release IL bodies match exactly. Evidence is in the three corresponding `Ptc.Calculation.Core.*.md` records.

- Differential command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj --filter 'FullyQualifiedName~PtcCalculationCoreSingletonCctorsMechanicalDifferentialTests|FullyQualifiedName~PtcCalculationCoreCalculatingStatusChangedEventArgsMechanicalDifferentialTests|FullyQualifiedName~PtcCalculationCoreErrorChangedEventArgsMechanicalDifferentialTests|FullyQualifiedName~PtcCalculationCoreSyncStatusChangedEventArgsMechanicalDifferentialTests' --no-restore --verbosity quiet` — passed 9, failed 0, skipped 0.
- Release audit: `Quire.Calculation.Core` Release build; MethodAudit inspected 990 declarations with 0 inspection errors. Exact constructors: 3/3 after (0/3 mapped before; all three were unmapped in the base Tier A inventory).
- A broader diagnostic run also selected existing `ResultManager` and `SimpleResultNodeFactory` differential classes. Six existing tests failed because their Tier B/other-type state is incomplete; those types are recorded above as dependencies to resolve before their cctors can be verified.
- An earlier focused run exposed a test renderer namespace mismatch; the final renderer compares null, simple runtime type name, and inherited `PropertyName` where available.
- The `EngineError.Message` inventory entry is a field, while the tier definition excludes fields; it is not part of this Tier A scope.

## Validation and blockers

The branch rebased onto `main` at `89a8910e`. The first gate reached validation after the protected-path restore fix, then stopped because the focused test host compiled the new Calculation test class without a Calculation project reference. Renamed that file/class to `PtcCalculationCoreSingletonCctorsMechanicalDifferentialTests` so it follows the focused host's existing `*MechanicalDifferentialTests.cs` exclusion. The targeted full differential project then passed all 3 cctor tests; rerun the documented lane gate. Gate output: `C:\qt\quire-gate-results-6pzo1pge\run`.
