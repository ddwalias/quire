# Lane C work checkpoint

## Scope and ownership

Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. This assignment removes both assemblies from Lane A's future scope. No other lane's active batch or files are part of this work.

The initial ownership audit found no Calculation-named active branch or worktree. `mirror/gen-calc-core` is already contained in `main`; the existing generated mirrors and evidence are the starting point, not duplicated work.

## Active handoff

**Next:** The current `mirror/c-calc-request-show-error-event-args` batch implements the one unowned Tier A constructor in `RequestShowErrorEventArgs`; its direct Release Mathcad test passed 1/1 and MethodAudit matched the constructor exactly. The existing Tier B `Error` getter/setter comparisons fail when rendering the dependent `EngineError` type because Mathcad has `<NodeHandle>` while Quire has `<Message>`; this batch does not modify those Tier B members. `TraceResultAnalyzer` is unowned with eight Tier A methods. Its `GetLabels` method calls `Ptc.DebugTools.DevAssert`, whose Quire.Core implementation is a mirror stub outside Lane C ownership; inspect and proceed with the other independent methods while leaving that dependency recorded. The remaining cctor list is reconciled: `DefaultUnitDescriptorTable` is Tier B and excluded; `EngineErrorMessages` is already touched by the gated MemberGen batch; `CalculationController` is owned by its separate worktree; `ResultManager::.cctor()` is implemented and Release-verified. Do not hand-write Tier B members.

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

### RequestShowErrorEventArgs constructor (`mirror/c-calc-request-show-error-event-args`)

Implemented the Tier A constructor to call the base `EventArgs` constructor and store its `EngineError` argument through `Error`. The custom direct comparison supplies null and an uninitialized non-null error and renders the input by type plus whether the exact object reference was stored; this isolates the constructor behavior from the Tier B `EngineError` field-name mismatch.

- Release Mathcad command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationCoreRequestShowErrorEventArgsConstructorTests'` — passed 1, failed 0, skipped 0 (two direct cases in the test).
- Release MethodAudit: 987 Core declarations, 0 inspection errors; Quire token `0x06000093` is an exact IL match for original constructor token `0x060000c5`.
- The first full RequestShowErrorEventArgs class run failed 3/3 because the existing getter/setter comparisons serialize different dependent `EngineError` fields (`<NodeHandle>` on Mathcad, `<Message>` on Quire), and the initial mechanical constructor comparison did the same for its input. The constructor comparison now uses a dedicated type/reference renderer. Getter/setter implementation and evidence are unchanged.
- TraceResultAnalyzer's eight Tier A methods are unowned. Its `GetLabels` path calls out-of-scope `Quire.Core.DebugTools.DevAssert`; this blocks full-class verification without a Lane C-owned fix. Other methods can be split into independent cases and do not need that helper for valid inputs.

`origin/main` is `f863245e`. The MemberGen branch gate passed path checks, Harness 97/97, Mpl 188/188, and focused Differential 392/392, then stopped before MethodAudit/ratchet because it could not find `Quire.Focused.Differential.Tests.dll`; the focused project emits `Quire.Differential.Tests.dll`. Report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-dmijh3dp\run\summary.json`. The ResultManager branch gate independently passed the same path checks and suites, then stopped on the same Release lookup; report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-56x7wibm\run\summary.json`. Both failures occur in `tools/validation/validate.py:788` before MethodAudit and ratchet. The broad Release sweep of changed Core classes passed 145/151; its six failures are `GetUnitPowerDenominator_06000237` and `GetUnitPowerNumerator_06000238` (call `XmcdToIResultNodeTransform.GetIntegerFromAttribute`), `Visit_0600017b` and `Visit_0600017c` (call `ResultNodeToXmcdTransform.WriteDisplayMatrixXml` and `WriteMatrixXml`), and both `StringListResultNode` constructors (Quire `_meaningfulValuesCount` 0; Mathcad -1). On the ResultManager branch, affected Release Mathcad tests passed 6/6 and MethodAudit found the constructor, `Clear`, and `.cctor` exact (991 Core declarations, 0 inspection errors). Its broader class filter has one failure, `CreateOldResultRefId_0600015f`, which reaches Tier B `AddOldResult`; that evidence row is `Blocked`. The cctor audit excludes `DefaultUnitDescriptorTable` (Tier B), `EngineErrorMessages` (same type already changed on the MemberGen branch), and `CalculationController` (separate owner); ResultManager is complete. Next unowned candidate: `TraceResultAnalyzer`, 8 Tier A methods, not changed in any active C worktree; dependency, evidence, and direct-test review is in progress. No validation code, baseline, protected list, or other lane files were changed. Do not edit Lane A/B files or test baselines.
