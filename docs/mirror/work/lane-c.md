# Lane C work checkpoint

## Scope and ownership

Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. This assignment removes both assemblies from Lane A's future scope. No other lane's active batch or files are part of this work.

Ownership is tracked against the active `mirror/c-calc-*` branches and worktrees. This checkpoint covers their combined Lane C Tier A ownership; reconcile every candidate with current `origin/main` and the live worktree list before starting another batch. `mirror/gen-calc-core` is already contained in `main`; the existing generated mirrors and evidence are the starting point, not duplicated work.

## Active handoff

`mirror/c-calc-calculatable-wrapper` merged on current `main` at `d6f0bf4624a25f7d98e3d5dbc27d2f961b7cb2cc`. Its affected Release Mathcad filter passed 28/28 and Release MethodAudit matched 28/28 exactly and structurally with 0 inspection errors. The Phase Release changed-method filter passed 28/28, and all phase test projects passed. Phase report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-olisn7kr\run\summary.json`; focused gate report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-lnzt9egs\run\summary.json`. `SetIdentifierLabels` is verified for generated non-self-referential keys; its remap branch constructs Tier B `ResolvedId` and remains unverified. No Tier B members were hand-written.

`CalculatableAggregateExtensions.GetAggregateRl` is another unowned Tier A member, but its original body calls `Ptc.EnumerableExtensions.Each`, whose Quire counterpart remains a generated stub in out-of-scope `Ptc.Core`; direct cases are blocked on that dependency. No Ptc.Core code was changed.

The committed 35-member Core MemberGen batch remains on `mirror/c-calc-simple-result-factory-cctor` at `42a3d72b`, rebased from `f5cdc826` for its latest gate. Focused and Release Differential passed 396/396, integrity reported 0 lint, and the focused ratchet passed. Phase Release failed only on `StringListResultNode` constructors `ctor_0600025d` and `ctor_0600025e`: expected `_meaningfulValuesCount=-1` but got 0, and expected `ArgumentNullException` for null `source` but got a result with count 0. Both cases reach the base `ListResultNode<T>` constructors on the separately owned, still-active `mirror/c-calc-listresultnode` branch. Reports: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-b3alhkdu\run\summary.json`, `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-jot1cuuk\run\summary.json`, and Release log `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-jot1cuuk\run\Quire.Differential.Tests.release.log`. No phase ratchet ran after the Release differential failure. Retry after the owned base-constructor dependency lands. Whole generated stub-pair policy checks passed; no override, acceptance-list edit, baseline edit, or other-lane change was used.

`ValueToIResultNodeTransform` passes its targeted Release Mathcad 7/7 and normal focused checks, but its phase run is blocked by 2,205 ratchet regressions after filtering to its selected Calculation methods; report `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-8t6el5cn\run\ratchet.json`.

`ThreadExtensions` merged on current `main` at `3dae0a1f02bd7cf52171118dc86c6bd6910a9f38`. Both Tier A methods pass 2/2 direct Release Mathcad comparisons and Release MethodAudit exact/structure match 2/2 with 0 inspection errors. Its failure handlers call Tier B `Ptc.Core.DebugTools.FailAssertion(Exception)`; only successful capture and synchronization paths are claimed. The first gate found two decompiler-local-name lints (`ex2`, `ex3`); the renamed retry passed focused and Release Differential 395/395, integrity (0 lint), the phase Release filter 2/2, all phase test projects, and the phase ratchet. Results: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-9dyoi6yh\run\summary.json` and `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-a3ew40mh\run\summary.json`. Evidence is in `Ptc.Calculation.ThreadExtensions.md`.

`mirror/c-calc-core-cctors` remains blocked on Tier B `ResultManager.AddOldResult`. `mirror/c-calc-listresultnode` owns `ListResultNode<T>` constructors, `ExtendList`, and `MeaningfulValuesCount`; `mirror/c-calc-matrix-data-accessor` owns selected accessor methods; `mirror/c-calc-simplecalculatable-lifecycle` owns `SimpleCalculatable`; and `mirror/c-calc-trace-result-analyzer-methods` owns TraceResultAnalyzer. `CalculationController`, `MatrixValueProxyWrapper`, Engine error/result constructors, vector helpers, and remaining Core constants are still assigned to live C worktrees. Do not overlap these owners.
The scalar `ResultNodeToXmcdTransform` writers and visits are already included in the rebased 35-member branch above. The separate uncommitted `mirror/c-calc-resultnode-xmcd-scalars` draft duplicates those methods; it is not an independent batch and remains unmerged. The uncommitted `mirror/c-calc-result-converter` draft is blocked by the `IUnitedValueResultNode.Units` shape mismatch (`IReadOnlyList<UnitPower>` in Quire versus writable `IList<Unit>` in Mathcad); its other paths are unclaimed. `ListResultNode<T>` owns the two failing base constructors; nonempty value counting also reaches Tier C `Quire.DoubleExtension.IsValid` in out-of-scope `Ptc.Core`.

The next independent Core helper batch is `mirror/c-calc-vectors-converter-base`, rebased onto current `origin/main` `d6f0bf46` at `5f8ebe9b`. It directly compares the private Tier A `IsOfExactPlainType` helper over 7 cases; its Release filter passes 8/8 including the existing constructor comparison. The sibling `GetVectorsValueType` remains blocked: its original IL returns numeric values 1 (`Numeric`), 3 (`Mixed`), and 4 (`NaNs`), but Quire's local `ResultValueType` gives those numbers conflicting names and lacks those categories. The implementation keeps that public method stubbed; raw casts would hide this semantic mismatch. The normal lane gate is next.

The independent `SimpleCalculatable` Core batch is committed on `mirror/c-calc-simplecalculatable-lifecycle` at `3f31e388` and passes 28/28 targeted Release comparisons after aligning its lifecycle class with the required test-file shape. Its IL-derived constructor uses numeric calculating-status value 2, and `SetSynchronizationStatus` invokes its callback only for numeric value 3; direct comparisons normalize enum values. `IsErrorOnScalingUnits` calls out-of-scope `Ptc.DebugTools.FailAssertion` and remains blocked. Gate this branch after the ListResultNode and accumulated Core gates so it rebases to current main. All remaining Calculation/Core cctors are owned by active C branches; `DefaultUnitDescriptorTable::.cctor()` is Tier B and excluded. `ResultManager` has implementation and direct Release evidence in `mirror/c-calc-core-cctors` and needs a normal gate against current main. No Lane A or B files were changed.

The independent Calculation batch `MatrixValueDataAccessorWrapper` is committed on `mirror/c-calc-matrix-data-accessor` at `5950f6bc`, based on `origin/main` `5a0f7754`. The Release command `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationMatrixValueDataAccessorWrapper_MechanicalDifferentialTests'` passes 2/2. It mirrors the constructor/accessor property, value-forwarding methods and first non-plain indices with direct interface-proxy comparisons. `GetEntireMatrixDataTypes`, both row/column type conversions, and the private conversion helper remain Blocked on `Ptc.Core.EnumHelper.TryParse`, outside Lane C's assignment; no Ptc.Core code was changed. Its previous gate hit the old Quire.Engine shape failure; current-main Release phase evidence passes `Quire.Mirror.Tests` 32/32. Its active branch has not been retried; do not overlap its owner.

The mapped Tier A methods already present in `tier-a.txt` and their evidence are not presumed complete without verifying status, direct comparison coverage, and exact Release IL for each type selected. Check dependencies and existing tests before each batch.

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

The failed Core gate used revision `4b70b1c2` against base `009f83df`; it did not merge. It passed generated-path validation, focused Release comparisons, focused coverage, MethodAudit, integrity and ratchet, then stopped during Phase Release Differential with the six failures recorded above. Reports: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-65lik2xd\run\summary.json`, `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-7kidmo3i\run\summary.json`, and `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-7kidmo3i\run\Quire.Differential.Tests.release.log`. The phase summary contains historical Debug counts and is not a request to inspect or rerun Debug locally. The Core transform Release filters pass 22/22, including numeric XML writers and the populated matrix path; ListResultNode Release filters pass 7/7. Nonempty double-list counting reaches Tier C `Quire.DoubleExtension.IsValid`; the base getter's empty-list lazy/cache path is directly compared. The Calculation accessor proxy/forwarder filters pass 2/2 in Release; four enum conversion methods remain blocked on out-of-scope `Ptc.Core.EnumHelper.TryParse`. That checkpoint snapshot was `5a0f77542a02be793e3c8d42b403e4e90f60ee79`; current progress and main status are recorded in Active handoff above. No validation tool, acceptance list, baseline, or other lane file was changed. See [PLAN.md](../PLAN.md), [validation.md](../validation.md), and [validation flow](validation-flow.md).
