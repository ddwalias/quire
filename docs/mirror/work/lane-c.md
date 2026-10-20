# Lane C work checkpoint

## Scope and ownership

Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. This assignment removes both assemblies from Lane A's future scope. No other lane's active batch or files are part of this work.

Ownership is tracked against the active `mirror/c-calc-*` branches and worktrees. This checkpoint covers their combined Lane C Tier A ownership; reconcile every candidate with current `origin/main` and the live worktree list before starting another batch. `mirror/gen-calc-core` is already contained in `main`; the existing generated mirrors and evidence are the starting point, not duplicated work.

## Active handoff

`origin/main` was fetched at `71932051b18b841853a472ea07e11242a9feb3f2`; this batch is based on that current tip and has no rebase conflicts or local committed Lane C work to replay. `mirror/c-calc-engineerror-relations` implements four previously unowned EngineError Tier A members: `get_Message`, `IsEquivalentTo`, `FindNextError`, and `IsAncestorOf` (tokens `0x06000032`, `0x0600003a`–`0x0600003c`). Mathcad IL was generated with `python tools\reference\reference.py Ptc.Calculation.Core.EngineError --assembly Ptc.Calculation.Core --il`; artifact SHA-256 is `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`. The targeted Mathcad Release differential command passed 4/4 before and after converting the Quire side of the test harness to reflection. Direct cases are summarized in `Ptc.Calculation.Core.EngineError.md`; verification is case-limited. EngineError now has 19/22 mapped Tier A members marked Verified (the prior 15 plus these four). Remaining members are blocked `Clone()` (calls the non-Tier-A `set_Message`) and the `MessagesForXamlSerialization` property pair (setter calls out-of-scope `Ptc.EnumerableExtensions.IsNotEmpty`). The first normal gate attempt at `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-uskcikah\run\summary.json` stopped in the focused Release build with five CS0234/CS0246 errors because that project links this test source but has no Calculation reference. The test no longer uses compile-time Calculation types; `dotnet build tests\Quire.Focused.Differential.Tests\Quire.Focused.Differential.Tests.csproj -c Release --no-restore` now passes (0 errors). Retry the normal runtime gate from this branch worktree; keep the candidate fixed during validation.

The Lane C checkpoint refresh was based on `b6bb9b8e4de20c4880bff27648a9014dc50937ad`; its documentation gate validated on `f326fcabe0631f8dc043a5e0ab8652a5be02f7ed` and merged as `8579b7685c24bbbcbbf3111e26d9a1fd60a6bda4`. `mirror/c-calc-vectors-converter-base` landed its implementation at `d2083827` and checkpoint at `a943914b`. `IsOfExactPlainType` is Verified for exact zero/NaN flags, absent requested bits, and overlapping flags. Its Release Mathcad filter passes 8/8 including the existing constructor comparison; gate details and reports are recorded under Validation below. `GetVectorsValueType` remains Blocked because Mathcad's enum results `Numeric=1`, `Mixed=3`, and `NaNs=4` conflict with Quire's current `ResultValueType` names and categories.

The 35-member Core MemberGen batch remains on `mirror/c-calc-simple-result-factory-cctor` at `42a3d72b`. Its normal gate's Phase Release differential fails only at `StringListResultNode` constructors `ctor_0600025d` and `ctor_0600025e`, which depend on the separately owned `ListResultNode<T>` constructors in `mirror/c-calc-listresultnode`. The generated stub-pair deletion check passed under the current policy; the remaining failures are the constructor dependency, not generated-path acceptance. Retry after the base-constructor branch lands.

`mirror/c-calc-core-cctors` has the ResultManager constructor, `Clear`, and `.cctor` implementation plus direct Release evidence, but its earlier gate is blocked by Tier B `ResultManager.AddOldResult`. Remaining initializer ownership is explicit: `CalcResultsProcessingPriorities` and `CalculationOptionDefaults` in `c-calc-core-constants`; `RedefinitionWarningsDictionary` in `c-calc-core-redefinition-warnings`; `TraceTypeDefinitions` in `c-calc-core-trace-types`; `VectorsListJSonSerializer<T>` in `c-calc-core-vector-serializer`; `XmcdToIResultNodeTransform` in `c-calc-core-xmcd-transform`; `SimpleResultNodeFactory` and `EngineErrorMessages` in `c-calc-simple-result-factory-cctor`; `CalculationController` in `c-calc-controller-cctor`; `EngineBuilder` in `c-calc-enginebuilder-cctor`; and `MatrixValueProxyWrapper` in `c-calc-matvalue-proxy-cctor`. `CalculationController`, Engine error/result constructors, `ListResultNode<T>`, `MatrixValueDataAccessorWrapper`, `SimpleCalculatable`, and `TraceResultAnalyzer` also remain assigned to active C worktrees. Do not overlap those owners. `ValueToIResultNodeTransform` remains owned by its worktree; its gate previously reported 2,205 Phase ratchet regressions. `MatrixValueDataAccessorWrapper` previously hit the unrelated Quire.Engine shape failure; its owner has current-main Release evidence for the mirror-test baseline.

The previous ownership/dependency sweep found no dependency-ready batch, but a subsequent live-worktree and exact-Tier-A recheck found `EngineError` relations unowned and independent; see Active handoff above. The remaining unmapped Core cctors are owned by active C worktrees; `DefaultUnitDescriptorTable::.cctor()` is Tier B and excluded. Other unowned candidates currently blocked include `CalculatableAggregateExtensions.GetAggregateRl` (`Ptc.EnumerableExtensions.Each`), `Trace2DResultConverterHelperBase` validation (`TraceTypeDefinitions` and `TraceErrorCodes`), vector lazy conversion (active matrix/list work), and the result converter draft (`IUnitedValueResultNode.Units` shape mismatch). Recheck ownership and dependencies after this batch lands.

The separate `mirror/c-calc-resultnode-xmcd-scalars` draft duplicates the scalar writers/visits in the 35-member branch and is not an independent batch. No Lane A/B files or shared baselines were changed.
## Completed batches

### Core singleton constructors (`mirror/c-calc-core-cctors`)

Implemented and directly compared the static constructors for `CalculatingStatusChangedEventArgs`, `ErrorChangedEventArgs`, and `SyncStatusChangedEventArgs`. Their original IL is a `newobj`/`stsfld`/`ret` initializer; all three Release IL bodies match exactly. Evidence is in the three corresponding `Ptc.Calculation.Core.*.md` records.

- Differential command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj --filter 'FullyQualifiedName~PtcCalculationCoreSingletonCctorsMechanicalDifferentialTests|FullyQualifiedName~PtcCalculationCoreCalculatingStatusChangedEventArgsMechanicalDifferentialTests|FullyQualifiedName~PtcCalculationCoreErrorChangedEventArgsMechanicalDifferentialTests|FullyQualifiedName~PtcCalculationCoreSyncStatusChangedEventArgsMechanicalDifferentialTests' --no-restore --verbosity quiet` — passed 9, failed 0, skipped 0.
- Release audit: `Quire.Calculation.Core` Release build; MethodAudit inspected 990 declarations with 0 inspection errors. Exact constructors: 3/3 after (0/3 mapped before; all three were unmapped in the base Tier A inventory).
- A broader diagnostic run also selected existing `ResultManager` and `SimpleResultNodeFactory` differential classes. Six existing tests failed because their Tier B/other-type state is incomplete. The factory cctor is independently verified; its factory-method comparisons still expose unmirrored result-node state. `ResultManager::.cctor()` remains blocked on its instance-constructor state.
- An earlier focused run exposed a test renderer namespace mismatch; the final renderer compares null, simple runtime type name, and inherited `PropertyName` where available.
- The `EngineError.Message` inventory entry is a field, while the tier definition excludes fields; it is not part of this Tier A scope.

### Core scalar constants (`mirror/c-calc-core-constants`)

Implemented direct static constructors for `CalcResultsProcessingPriorities` and `CalculationOptionDefaults` from the original IL. Their three initialized fields are compared by `PtcCalculationCoreStaticConstantsCctorsMechanicalDifferentialTests`.

- Differential command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj --filter 'FullyQualifiedName~PtcCalculationCoreStaticConstantsCctorsMechanicalDifferentialTests' --verbosity minimal` — passed 2, failed 0, skipped 0.
- Release MethodAudit: 992 declarations, 0 inspection errors; all five implemented Core cctors are exact (3 event-args + 2 constants). The two constants were absent from Quire's method inventory before this batch.
- Evidence: `Ptc.Calculation.Core.CalcResultsProcessingPriorities.md` and `Ptc.Calculation.Core.CalculationOptionDefaults.md`.

### Core redefinition warning owner types (`mirror/c-calc-core-redefinition-warnings`)

Implemented `RedefinitionWarningsDictionary::.cctor()` with the one-element `KeysOwnerTypes` array.

- Differential command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj --filter 'FullyQualifiedName~PtcCalculationCoreRedefinitionWarningsDictionaryCctorMechanicalDifferentialTests' --verbosity minimal` — passed 1, failed 0, skipped 0.
- Release MethodAudit: 993 declarations, 0 inspection errors; all six implemented Core cctors are exact.
- Evidence: `Ptc.Calculation.Core.RedefinitionWarningsDictionary.md`.

### Core trace-type range table (`mirror/c-calc-core-trace-types`)

Implemented `TraceTypeDefinitions::.cctor()` with all eight trace keys and their min/max column ranges.

- Differential command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj --filter 'FullyQualifiedName~PtcCalculationCoreTrace2DResultsTraceTypeDefinitionsCctorMechanicalDifferentialTests' --verbosity minimal` — passed 1, failed 0, skipped 0.
- Release MethodAudit: 994 declarations, 0 inspection errors; all seven implemented Core cctors are exact.
- Evidence: `Ptc.Calculation.Core.Trace2DResults.TraceTypeDefinitions.md`.

### Core vector-list serializer (`mirror/c-calc-core-vector-serializer`)

Implemented `VectorsListJSonSerializer<T>::.cctor()` to create a `DataContractJsonSerializer` for `T[]`. Reworked the existing `Write` comparison to close the generic type over `double` and compare bytes for empty, one-item, and mixed-value arrays; this fixes its prior open-generic harness failure.

- Differential command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj --filter 'FullyQualifiedName~PtcCalculationCoreTrace2DResultsVectorsListJSonSerializer1MechanicalDifferentialTests' --no-restore --verbosity minimal` — passed 2, failed 0, skipped 0.
- Release MethodAudit: 995 declarations, 0 inspection errors; all eight implemented Core cctors are exact.
- Evidence file: Ptc.Calculation.Core.Trace2DResults.VectorsListJSonSerializer`1.md.

### Core XSLT result unescaping (`mirror/c-calc-core-xmcd-transform`)

Implemented `XmcdToIResultNodeTransform::.cctor()` to load the `DataVectors` XSLT. Directly compared its output for plain text and nested vector nodes.

- Differential command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj --filter 'FullyQualifiedName~PtcCalculationCoreXmcdToIResultNodeTransformCctorMechanicalDifferentialTests' --no-restore --verbosity quiet` — passed 1, failed 0, skipped 0.
- Release MethodAudit: 996 declarations, 0 inspection errors; all nine implemented Core cctors are exact.
- Broader existing `XmcdToIResultNodeTransformMechanicalDifferentialTests` run failed its two wrapper tests because the Tier A `GetIntegerFromAttribute` helper is still a protected generated stub.
- Evidence: `Ptc.Calculation.Core.XmcdToIResultNodeTransform.md`.

### Calculation engine request strings (`mirror/c-calc-enginebuilder-cctor`)

Implemented `EngineBuilder::.cctor()` to initialize its six static request and unit-system format strings.

- Differential command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj --filter 'FullyQualifiedName~PtcCalculationEngineBuilderCctorMechanicalDifferentialTests' --verbosity minimal` — passed 1, failed 0, skipped 0.
- Release MethodAudit: `Quire.Calculation` inspected 258 declarations with 0 inspection errors; the cctor is exact.
- Evidence: `Ptc.Calculation.EngineBuilder.md`.

### Calculation matrix proxy options (`mirror/c-calc-matvalue-proxy-cctor`)

Implemented `MatrixValueProxyWrapper::.cctor()` to construct its `PlainDataConversionOptions` and set `ImaginaryThreshold` to NaN.

- Differential command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj --filter 'FullyQualifiedName~PtcCalculationMatrixValueProxyWrapperCctorMechanicalDifferentialTests' --verbosity minimal` — passed 1, failed 0, skipped 0.
- Release MethodAudit: `Quire.Calculation` inspected 259 declarations with 0 inspection errors; `EngineBuilder` and `MatrixValueProxyWrapper` cctors are both exact.
- Evidence: `Ptc.Calculation.MatrixValueProxyWrapper.md`.

### Core default result-node factory (`mirror/c-calc-simple-result-factory-cctor`)

Implemented `SimpleResultNodeFactory::.cctor()` to initialize its `Default` field.

- Differential command: `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj --filter 'FullyQualifiedName~PtcCalculationCoreSimpleResultNodeFactoryCctorMechanicalDifferentialTests' --verbosity minimal` — passed 1, failed 0, skipped 0.
- Release MethodAudit: 997 declarations, 0 inspection errors; all ten implemented Core cctors are exact.
- Existing factory member tests still fail for incomplete result-node state, as recorded above. The cctor comparison isolates and verifies the static `Default` field initialization.
- Evidence: `Ptc.Calculation.Core.SimpleResultNodeFactory.md`.

## Validation and blockers

The current `mirror/c-calc-vectors-converter-base` gate passed on candidate `a943914b2cda31a5753014cadd085f579736eeba`, based on `2bb1f527f7001632e1147235f7908162a17e84d1`, and merged to `main` at `b6bb9b8e4de20c4880bff27648a9014dc50937ad`. The affected Release Mathcad filter passed 8/8. Focused validation passed 396/396 in normal and Release Differential; MethodAudit reported 1,583 exact and 73 structure-only matches, integrity lint 0, and ratchet passed. The Phase Release filter passed 8/8; all phase projects passed (Calculation 8, Controls 16, Core 7, Differential 8, Engine 97, EquationEditor 88, Harness 97, Mirror 32, Mpl 188). Final reports: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-k_vwxjyz\run\summary.json` and `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-cf_4cbim\run\summary.json`. The protected generated stub-pair deletion used the normal gate policy, with no override or acceptance-list edit.
The failed Core gate used revision `4b70b1c2` against base `009f83df`; it did not merge. It passed generated-path validation, focused Release comparisons, focused coverage, MethodAudit, integrity and ratchet, then stopped during Phase Release Differential with the six failures recorded above. Reports: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-65lik2xd\run\summary.json`, `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-7kidmo3i\run\summary.json`, and `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-7kidmo3i\run\Quire.Differential.Tests.release.log`. The phase summary contains historical Debug counts and is not a request to inspect or rerun Debug locally. The Core transform Release filters pass 22/22, including numeric XML writers and the populated matrix path; ListResultNode Release filters pass 7/7. Nonempty double-list counting reaches Tier C `Quire.DoubleExtension.IsValid`; the base getter's empty-list lazy/cache path is directly compared. The Calculation accessor proxy/forwarder filters pass 2/2 in Release; four enum conversion methods remain blocked on out-of-scope `Ptc.Core.EnumHelper.TryParse`. That checkpoint snapshot was `5a0f77542a02be793e3c8d42b403e4e90f60ee79`; current progress and main status are recorded in Active handoff above. No validation tool, acceptance list, baseline, or other lane file was changed. See [PLAN.md](../PLAN.md), [validation.md](../validation.md), and [validation flow](validation-flow.md).
