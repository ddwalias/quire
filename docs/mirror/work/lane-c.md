# Lane C work checkpoint

## Scope and ownership

Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. This assignment removes both assemblies from Lane A's future scope. No other lane's active batch or files are part of this work.

The initial ownership audit found no Calculation-named active branch or worktree. `mirror/gen-calc-core` is already contained in `main`; the existing generated mirrors and evidence are the starting point, not duplicated work.

## Active handoff

**Next:** The 35-member Core MemberGen batch is committed on `mirror/c-calc-simple-result-factory-cctor`; `accept.py --project Quire.Calculation.Core --include-tier-a --no-mutate` accepted Tier A members across 21 Core types and generated comparisons passed 36/36. Its standard gate stops before validation because 21 changed `Generated/Mirror` files are maintainer-owned. Do not use `--allow-protected` or edit protected acceptance lists; keep the batch intact pending the maintainer-owned path decision. Once that path is available, continue with independent static initializers: ten Core cctors are implemented and checked, leaving three Core cctors to inspect. In `Ptc.Calculation.dll`, `EngineBuilder` and `MatrixValueProxyWrapper` cctors are complete; `CalculationController` depends on a generated Tier A `EngineBuilder` constructor. `DefaultUnitDescriptorTable` and `EngineErrorMessages` also depend on generated constructor/component stubs. `ResultManager::.cctor()` depends on a constructor whose object state currently differs from Mathcad (`_oldResultKey`/`_resultKey` are 0 instead of -1 and `_results` is null). `EngineError.Message` is a field outside Tier A. Do not hand-write Tier B members.

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

`mirror/c-calc-core-cctors` is rebased onto current `main` at `84a4d567`. Its latest gate run (against the previous `b0c7227` base) passed focused validation: Harness 97/97, Mpl 188/188, Focused Differential 392/392, integrity and ratchet passed. The phase profile is blocked by 464/3,558 existing `Quire.Differential.Tests` failures, including `WorksheetPageLayout` and `WpfUtils.CmToDips` mirror stubs, plus generated `s1000001`/`s2000001` files. Calculation, Controls, and Core phase tests passed 8/8, 16/16, and 7/7. Failure report: `C:\qt\quire-gate-phase-85jml5vu\run`. The focused-host compile issue was fixed by renaming the cctor test file/class to `PtcCalculationCoreSingletonCctorsMechanicalDifferentialTests`; the targeted cctor run passed 3/3. The aggregate MemberGen gate on current `main` stops before validation at the maintainer-owned generated paths. Do not edit Lane A/B files or test baselines.
