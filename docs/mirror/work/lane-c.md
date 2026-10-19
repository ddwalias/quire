# Lane C work checkpoint

## Scope and ownership

Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. This assignment removes both assemblies from Lane A's future scope. No other lane's active batch or files are part of this work.

The initial ownership audit found no Calculation-named active branch or worktree. `mirror/gen-calc-core` is already contained in `main`; the existing generated mirrors and evidence are the starting point, not duplicated work.

## Active handoff

**Current:** `c-calc-simple-result-factory-cctor` passed focused validation, including Release Differential 392/392, but its phase gate is blocked by unrelated Differential tests recorded below. Current fetched `origin/main` is `57c3bc67`. The independent `EngineErrorCollection(IEnumerable<EngineError>, EngineError)` Tier A constructor is being completed at Mathcad token `0x06000050`; its Release direct comparisons pass. Its batch started at `f863245e` and will be rebased by `lane_merge.py`. Continue with independent Lane C work even if phase validation remains blocked.

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

### TraceDataPoints constructor and emptiness state (`mirror/c-calc-trace-data-points`)

Implemented the Tier A two-argument constructor and `IsEmpty` getter from the ILSpy output. The constructor throws parameterless `ArgumentNullException` when either vector reference is null and otherwise stores the references in RangePoints/ResultPoints order. `IsEmpty` returns true only when both references are null. Existing no-argument constructor and property accessors remain unchanged.

- Baseline Release Mathcad run before implementation: the existing test class passed 5/5.
- Release Mathcad command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationCoreTrace2DResultsTraceDataPointsMechanicalDifferentialTests'` — passed 7, failed 0, skipped 0. Direct additions cover three constructor cases and four `IsEmpty` states.
- Release MethodAudit: 7 declarations; Quire constructor `0x06000343` exactly matches Mathcad `0x060002b0`, and Quire getter `0x06000344` exactly matches Mathcad `0x060002b6`. Both `structure_matches=true`, `exact_match=true`; the targeted inventory reports 9 unavailable ancillary assemblies outside this type.
- Original IL was extracted with `python tools\reference\reference.py Ptc.Calculation.Core.Trace2DResults.TraceDataPoints --assembly Ptc.Calculation.Core --il`. The evidence table now covers all seven original declarations.

### TraceErrorCodes overlap (superseded candidate)

The formatter was already implemented and documented in the active `mirror/c-calc-simple-result-factory-cctor` branch at checkpoint commit `c37e8b3b`. This branch (`mirror/c-calc-trace-error-codes`, code commit `b2d0b87f`) duplicates that member and adds direct comparisons for 18 cases across three cultures; local Release tests passed 1/1 and Release MethodAudit was exact. A normal gate was invoked for `b2d0b87f`; during validation, documentation commit `bd208971` changed the candidate tip, so the report does not validate the current branch. The gate stopped before audit/ratchet on the unrelated Release assembly-name lookup (report `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-f1m33fk0\run\summary.json`). This superseded branch will not be applied or queued. The active owner remains `c-calc-simple-result-factory-cctor`.

### TraceErrorCodes point-limit message (`mirror/c-calc-trace-error-codes`)

Implemented Tier A `CreateTooManyPointsInResultMessage(int)` using Mathcad's current UI culture, exact format string and `ErrorTooManyPointsInResult` key.

- Release Mathcad command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests'` — passed 1, failed 0, skipped 0; the test performs 18 direct comparisons across three cultures.
- Release MethodAudit: Quire token `0x06000345` exactly matches Mathcad token `0x0600027f`; `structure_matches=true`, `exact_match=true`. The targeted inventory reports 9 missing ancillary assemblies unrelated to this type.
- Original IL was extracted with `python tools\reference\reference.py Ptc.Calculation.Core.Trace2DResults.TraceErrorCodes --assembly Ptc.Calculation.Core --il`.



The c-calc-simple-result-factory-cctor normal gate on commit 545d375ea5aefcb79352545f942340b7501a663e passed generated-path checks, Harness (97/97), Mpl (188/188), focused Differential (392/392), and the Release Differential rerun (392/392). Phase validation then failed: Quire.Calculation.Tests passed 8/8, Quire.Controls.Tests 16/16, Quire.Core.Tests 7/7, while Quire.Differential.Tests had 3144 passed and 461 failed. Report: C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-g2e68owu\run\summary.json; gate report: C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-be5g8ens\run\summary.json. Failures span unrelated stub-backed IO/package, serialization and controls comparisons, Lane C Trace2D VectorsLegacyConverter renderer mismatch, and concurrent file access to phase output paths s1000001 and s2000001; the validator also detected those files changed under tests/Quire.Differential.Tests/bin/Debug/net10.0-windows. The focused Release gate had no shape, integrity, or ratchet regressions. No Lane C fix can safely waive these failures; the candidate was not merged. Recheck relevant type ownership before addressing any specific C failure, and continue independent batches.
### EngineErrorCollection sequence-plus-error constructor (mirror/c-calc-engine-error-collection)

Implemented Tier A constructor token 0x06000050: initialize the inherited list from errors, then call List<EngineError>.Add(error). The original IL was inspected through python tools\reference\reference.py Ptc.Calculation.Core.EngineErrorCollection --assembly Ptc.Calculation.Core --il; the two-argument body is List<EngineError>(IEnumerable<EngineError>), List<EngineError>.Add, ret. The implementation uses base.Add so Release IL preserves the original non-virtual call.

- Direct Release Mathcad command: $env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationCoreEngineErrorCollectionMechanicalDifferentialTests' — passed 2/2, skipped 0. Added direct cases compare empty and multiple input sequences, null elements/null appended error, and null sequence exception behavior. The existing no-argument constructor comparison remains in the class.
- Release MethodAudit targeted this type: 2 declarations; both the existing no-argument constructor and new constructor report structure_matches=true, exact_match=true. The tool reports 9 unavailable ancillary assembly inspections outside this type.
- Evidence record: docs/mirror/evidence/Ptc.Calculation.Core.EngineErrorCollection.md. Candidate started at f863245e; origin/main was fetched at 57c3bc67 before candidate completion. Normal gate and push are pending; no protected-path override is used.
## Validation and blockers

The `mirror/c-calc-trace-result-analyzer-methods` gate passed generated-path checks, Harness 97/97, Mpl 188/188, and focused Differential 392/392. It then stopped before MethodAudit/ratchet because the Release pass looked for `Quire.Focused.Differential.Tests.dll` instead of the project's `Quire.Differential.Tests.dll`. Exact report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-d9gezyo1\run\summary.json`; missing path: `C:\Users\DuyDang\AppData\Local\Temp\quire-audit-release-hsgvwc3w\Quire.Focused.Differential.Tests.dll`.

The `mirror/c-calc-result-node-utils` gate passed generated-path checks, Harness 97/97, Mpl 188/188, and focused Differential 392/392, then stopped before MethodAudit/ratchet on the same Release assembly-name lookup. Report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-5f4gvh46\run\summary.json`; missing path: `C:\Users\DuyDang\AppData\Local\Temp\quire-audit-release-lj84oav5\Quire.Focused.Differential.Tests.dll`.

The `mirror/c-calc-engine-error-placeholder` gate passed generated-path checks, Harness 97/97, Mpl 188/188, and focused Differential 392/392, then stopped before MethodAudit/ratchet on the same Release assembly-name lookup. Report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-600gvm8k\run\summary.json`; missing path: `C:\Users\DuyDang\AppData\Local\Temp\quire-audit-release-yh4a3xo_\Quire.Focused.Differential.Tests.dll`.

The `mirror/c-calc-vectors-legacy-readhelpers` gate passed generated-path checks, Harness 97/97, Mpl 188/188, and focused Differential 392/392, then stopped before MethodAudit/ratchet at `tools/validation/validate.py:788` because it requested `Quire.Focused.Differential.Tests.dll` at `C:\Users\DuyDang\AppData\Local\Temp\quire-audit-release-d5ibxye6`; the focused project emits `Quire.Differential.Tests.dll`. Exact report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-sh5xxq9d\run\summary.json`.

The `mirror/c-calc-trace-data-points` gate passed generated-path checks, Harness 97/97, Mpl 188/188, and focused Differential 392/392, then stopped before MethodAudit/ratchet at `tools/validation/validate.py:788` because it requested `Quire.Focused.Differential.Tests.dll` from `C:\Users\DuyDang\AppData\Local\Temp\quire-audit-release-ki5_9oen`; the focused project emits `Quire.Differential.Tests.dll`. Exact report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-zcqverd5\run\summary.json`.

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
