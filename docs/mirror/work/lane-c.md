# Lane C work checkpoint

## Scope and ownership

Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. This assignment removes both assemblies from Lane A's future scope. No other lane's active batch or files are part of this work.

The initial ownership audit found no Calculation-named active branch or worktree. `mirror/gen-calc-core` is already contained in `main`; the existing generated mirrors and evidence are the starting point, not duplicated work.

## Active handoff

**Current:** Current fetched `origin/main` is `10deda91`. The owned `c-calc-engine-error-placeholder` candidate remains independent: its Release Mathcad class passed 1/1 on the refreshed worktree and Release MethodAudit confirms exact IL at Quire token `0x0600004a` against Mathcad `0x06000040`. The old focused Release assembly-name blocker was fixed on main at a2b91391; rebase and run its normal gate now. Other candidates currently blocked by phase/test-host issues are documented below. Check live worktrees and remaining Tier A scope again after this gate.

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


### Current-main gate status after Release lookup fix

Commit a2b91391 fixed the Release focused assembly-name lookup. The earlier EngineErrorExtensions report that stopped at that path is historical; this branch has now passed the affected Release Mathcad class (1/1) and exact Release MethodAudit for IsPlaceholderError. New normal gate pending.

The c-calc-trace-data-points candidate ac0694d2 on gate base 750f984b passed focused suites (Harness 97/97, Mpl 188/188, Differential 393/393 Debug and Release), Release audit/coverage, integrity lint 0 and ratchet. Phase validation failed: Calculation 8/8, Controls 16/16, Core 7/7, Differential 3092 passed and 468 failed of 3560. All seven TraceDataPoints tests passed in phase; validator flagged the shared Debug output writes s1000001 and s2000001. Reports: C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-2tu1y7ab\run\summary.json and C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-ojxy9n1u\run\summary.json.

The corrected c-calc-vectors-legacy-readhelpers gate on ce26a431 against e9a9bec4 passed focused Differential 393/393 but its Release test host crashed after 320 passes before its test class ran. Its amended Release Mathcad class then passed 3/3. Reports: C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-lydvszwd\run\summary.json and its Release log/TRX. Retry after concurrent gate/test-host load eases.
## Validation and blockers

The `mirror/c-calc-trace-result-analyzer-methods` gate passed generated-path checks, Harness 97/97, Mpl 188/188, and focused Differential 392/392. It then stopped before MethodAudit/ratchet because the Release pass looked for `Quire.Focused.Differential.Tests.dll` instead of the project's `Quire.Differential.Tests.dll`. Exact report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-d9gezyo1\run\summary.json`; missing path: `C:\Users\DuyDang\AppData\Local\Temp\quire-audit-release-hsgvwc3w\Quire.Focused.Differential.Tests.dll`.

The `mirror/c-calc-result-node-utils` gate passed generated-path checks, Harness 97/97, Mpl 188/188, and focused Differential 392/392, then stopped before MethodAudit/ratchet on the same Release assembly-name lookup. Report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-5f4gvh46\run\summary.json`; missing path: `C:\Users\DuyDang\AppData\Local\Temp\quire-audit-release-lj84oav5\Quire.Focused.Differential.Tests.dll`.

The mirror/c-calc-engine-error-placeholder gate originally stopped at the Release assembly-name lookup, fixed by main a2b91391; that report is historical and the branch is being gated again.

### EngineErrorExtensions placeholder classifier (`mirror/c-calc-engine-error-placeholder`)

Implemented Tier A `IsPlaceholderError(EngineError)` with the original null check and `ErrorCode == "bad%_placeholder"` comparison. Direct cases use uninitialized fixture objects with only the private code field set and normalize the renderer to that code, avoiding unrelated EngineError fields. The Tier B `ErrorCode` getter is unchanged and remains independently Release-verified.

- Release Mathcad command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationCoreEngineErrorExtensionsMechanicalDifferentialTests'` — passed 1, failed 0, skipped 0 (three direct cases).
- Release MethodAudit: 987 Core declarations, 0 inspection errors; Quire token `0x0600004a` exactly matches original token `0x06000040`.

### ResultNodeUtils private plottable-value parser (`mirror/c-calc-result-node-utils`)

Implemented Tier A `TryParsePlottableValue(IResultNode, out double)` using the original interface tests and shared return path. The direct comparison covers real, complex, imaginary, nested united-real, and unknown node inputs; it checks both the bool result and the mutated `out` value. `ParsePlottableValue` remains a generated stub because its call to `Quire.DebugTools.DevAssert` is outside Lane C ownership.

- Release Mathcad command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationCoreResultNodeUtilsMechanicalDifferentialTests'` — passed 1, failed 0, skipped 0 (five direct cases in the test).
- Release MethodAudit: 987 Core declarations, 0 inspection errors; Quire token `0x060000d0` exactly matches original token `0x060001ff`.

### TraceResultAnalyzer unblocked methods (`mirror/c-calc-trace-result-analyzer-methods`)

Implemented private `AddOneDataSegment(points, xCoordinate, yData, ensureTrailingNaN)`, `StringVectorToIndices`, `ToDoubleDataList`, `BuildPointsForBoxes`, and `BuildTuplesOfPoints`. All are Tier A methods from the helper-generated IL; the direct tests exercise NaN separators, offsets, meaningful strings, mixed string/double vectors, null rows, and x offsets. `GetLabels`, public `AddOneDataSegment`, and `BuildPointsForWaterfall` remain `[MirrorStub]` methods because their paths depend on out-of-scope `Quire.DebugTools.DevAssert` or `Quire.Wpf.PointExtension.IsNaN` stubs. The valid-input `ToDoubleDataList` case covers its double/string branches; the unsupported-vector assertion path remains blocked by `DebugTools.FailAssertion`.

- Release Mathcad command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationCoreTraceResultAnalyzerMechanicalDifferentialTests'` — passed 5, failed 0, skipped 0.
- Release MethodAudit: 990 Core declarations, 0 inspection errors; Quire tokens `0x06000349`–`0x0600034d` are exact for original tokens `0x060002b9`–`0x060002bb`, `0x060002bd`, and `0x060002be` respectively.
- The first Release test run passed 4/5; `ToDoubleDataList_060002bb` failed only because the proxy input renderer emitted differing generated proxy type names. The renderer now normalizes `IDoubleListResultNode` inputs; the rerun passed 5/5.

### RequestShowErrorEventArgs constructor (`mirror/c-calc-request-show-error-event-args`)

Implemented the Tier A constructor to call the base `EventArgs` constructor and store its `EngineError` argument through `Error`. The custom direct comparison supplies null and an uninitialized non-null error and renders the input by type plus whether the exact object reference was stored; this isolates the constructor behavior from the Tier B `EngineError` field-name mismatch.

- Release Mathcad command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationCoreRequestShowErrorEventArgsConstructorTests'` — passed 1, failed 0, skipped 0 (two direct cases in the test).
- Release MethodAudit: 987 Core declarations, 0 inspection errors; Quire token `0x06000093` is an exact IL match for original constructor token `0x060000c5`.
- The first normal gate stopped at path validation because it treats new test files as maintainer-owned (`tests/Quire.Differential.Tests/PtcCalculationCoreRequestShowErrorEventArgsConstructorTests.cs`). The same test class now lives in the existing `PtcCalculationCoreRequestShowErrorEventArgsMechanicalDifferentialTests.cs`; no override was used.
- The corrected normal gate passed path checks, Harness (97/97), Mpl (188/188), and focused Differential (392/392), then stopped before MethodAudit/ratchet at `tools/validation/validate.py:788` because it requested `Quire.Focused.Differential.Tests.dll` instead of the focused project's `Quire.Differential.Tests.dll`. Exact report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-7_ijv4uu\run\summary.json`.
- The first full RequestShowErrorEventArgs class run failed 3/3 because the existing getter/setter comparisons serialize different dependent `EngineError` fields (`<NodeHandle>` on Mathcad, `<Message>` on Quire), and the initial mechanical constructor comparison did the same for its input. The constructor comparison now uses a dedicated type/reference renderer. Getter/setter implementation and evidence are unchanged.
- TraceResultAnalyzer's eight Tier A methods are unowned. Its `GetLabels` path calls out-of-scope `Quire.Core.DebugTools.DevAssert`; this blocks full-class verification without a Lane C-owned fix. Other methods can be split into independent cases and do not need that helper for valid inputs.

`origin/main` is `f863245e`. The MemberGen, ResultManager, RequestShowErrorEventArgs, TraceResultAnalyzer, and ResultNodeUtils branch gates all passed generated-path checks, Harness 97/97, Mpl 188/188, and focused Differential 392/392, then stopped before MethodAudit/ratchet because they could not find `Quire.Focused.Differential.Tests.dll`; the focused project emits `Quire.Differential.Tests.dll`. Reports: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-dmijh3dp\run\summary.json`, `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-56x7wibm\run\summary.json`, `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-7_ijv4uu\run\summary.json`, `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-d9gezyo1\run\summary.json`, and `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-5f4gvh46\run\summary.json`. All five failures occur in `tools/validation/validate.py:788` before MethodAudit and ratchet. The broad Release sweep of changed Core classes passed 145/151; its six failures are `GetUnitPowerDenominator_06000237` and `GetUnitPowerNumerator_06000238` (call `XmcdToIResultNodeTransform.GetIntegerFromAttribute`), `Visit_0600017b` and `Visit_0600017c` (call `ResultNodeToXmcdTransform.WriteDisplayMatrixXml` and `WriteMatrixXml`), and both `StringListResultNode` constructors (Quire `_meaningfulValuesCount` 0; Mathcad -1). On the ResultManager branch, affected Release Mathcad tests passed 6/6 and MethodAudit found the constructor, `Clear`, and `.cctor` exact (991 Core declarations, 0 inspection errors). Its broader class filter has one failure, `CreateOldResultRefId_0600015f`, which reaches Tier B `AddOldResult`; that evidence row is `Blocked`. The RequestShowErrorEventArgs gate and Release evidence are above. The cctor audit excludes `DefaultUnitDescriptorTable` (Tier B), `EngineErrorMessages` (same type already changed on the MemberGen branch), and `CalculationController` (separate owner); ResultManager is complete. Five TraceResultAnalyzer methods are exact and directly compared; three remain blocked on Quire.Core helper stubs. ResultNodeUtils Release tests passed five cases and its private helper is exact; its public wrapper remains blocked on DebugTools. No validation code, baseline, protected list, or other lane files were changed. Do not edit Lane A/B files or test baselines.
