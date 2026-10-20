# Lane C work checkpoint

## Scope and ownership

Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. This assignment removes both assemblies from Lane A's future scope. No other lane's active batch or files are part of this work.

Ownership is tracked against the active `mirror/c-calc-*` branches and worktrees. This checkpoint covers their combined Lane C Tier A ownership; reconcile every candidate with current `origin/main` and the live worktree list before starting another batch. `mirror/gen-calc-core` is already contained in `main`; the existing generated mirrors and evidence are the starting point, not duplicated work.

## Active handoff

`origin/main` is `7bf0cb4fca386c2ad0911be36e314ff4dbf151fd`. `ListResultNode<T>` landed at `53253550` after its Phase Release Differential passed 7/7. The accumulated Core MemberGen/initializer candidate `53b8f5be59355bb97bc747d906d36b6fde0d4365` landed at `25c7bb8e`; its final Phase Release Differential passed 156/156. CalculatableChildHider children landed at `e64f3aca`; direct Release comparisons passed 8/8 and its Phase Release comparison passed 1/1. DefaultUnitDescriptorTable landed at `e2932053` after the standard gate passed: focused tests 401/401 in normal and Release Differential, 1,655 exact / 61 structure-only IL matches, lint 0, focused ratchet passed; Phase Release Differential 2/2; all phase test projects passed; phase audit 6,730 exact / 180 structure-only, lint 0, phase ratchet passed. Its `.cctor` remains blocked on the absent XAML resource. ComplexSimpleResultNode `Equals(object)` landed at `7bf0cb4f`; its affected Release Mathcad filter passed 8/8, focused validation passed 401/401 in normal and Release Differential with 1,655 exact / 61 structure-only IL matches, and phase validation passed all projects with Release Differential 8/8, 6,730 exact / 181 structure-only IL matches, lint 0 and ratchet passed. No protected override or acceptance-list change was used.

`mirror/c-calc-child-hider-events` landed at `5a2675634d3d4df14731733a4cd6539b0e0b170f`. Its child-enumeration follow-up landed at `e64f3aca`; candidate commit `7d242fb1` implements `IsHiddenFromCalculation` (`0x060002cb`) and `get_CalculatableChildren` (`0x060002cc`) using Core artifact SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`. Its combined Release Mathcad filter passed 8/8; its Phase Release comparison passed 1/1. Cases cover null/ordinary and visible/hidden children, direct calculatables, non-calculatables, and ordinary-container flattening. Remaining `Initialize`, `ChildAdded`, `ChildRemoved`, `ChildMoved`, and `HiddenStateChanged` depend on out-of-scope `Ptc.EnumerableExtensions.Each`; property-change subscription members need a direct fixture for an object implementing both original interfaces.

`mirror/c-calc-default-unit-descriptor-table` was rebased onto `bd52510c` and merged at `e2932053`. Direct Release Mathcad comparisons for `InitializeUnitSystems` (`0x06000009`) and explicit `IComponentConnector.Connect` (`0x06000012`) pass 2/2. An initial ratchet caught an inventory key change from omitting the generated `global::` qualifier; the source was corrected to preserve the slot spelling. Final focused report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-k_oqie7r\run\summary.json` (401/401 in normal and Release Differential, 1,655 exact / 61 structure-only IL matches, lint 0, ratchet passed). Final Phase report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-z4j2u_e7\run\summary.json` (Calculation 8, Controls 16, Core 7, Differential Release 2/2, Engine 97, EquationEditor 88, Harness 97, Mirror 32, Mpl 188; audit 6,730 exact / 180 structure-only; lint 0; ratchet passed). The normal `lane_merge.py` wrapper validated on `bd52510c` and pushed the result to `e2932053`; no protected override or acceptance-list change.

`mirror/c-calc-complex-simple-result-node` was based on `e2932053` and landed at `7bf0cb4f`. Current-main Tier A inventory had only `Equals(object)` (`0x060001a8`) outstanding on `ComplexSimpleResultNode`; `ToString()` was already implemented and explicit `IResultNode.ValueType` is outside Tier A. The implementation follows inspected Mathcad IL: accept only the same type, compare real then imaginary components using `double.Equals`. Its Release Mathcad class filter passed 8/8. Focused report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-wsplb399\run\summary.json` (401/401 in normal and Release Differential, 1,655 exact / 61 structure-only IL matches, lint 0, ratchet passed). Phase report: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-fwvmna76\run\summary.json` (Calculation 8, Controls 16, Core 7, Differential Release 8/8, Engine 97, EquationEditor 88, Harness 97, Mirror 32, Mpl 188; audit 6,730 exact / 181 structure-only; lint 0; ratchet passed). The normal wrapper validated on `e2932053` and pushed `7bf0cb4f`.

Current batch `mirror/c-calc-unit-descriptor-core` is based on `7bf0cb4f` at `359ce614`. Ownership scan of active Lane C branch diffs found no overlap on `UnitDescriptor`. Its 10 independent methods are Tier A: `SetUnitSystem`, the eight base/derived/exclude unit-system setters, and `ToString`; the affected Release Mathcad filter passes 30/30 for the class. The implementation and generated stub-pair deletions are committed; evidence is updated. `Key` and `RL` property pairs remain generated stubs because their getters call `Ptc.EnumerableExtensions.IsNullOrEmpty<char>`, which is a Tier A Quire.Core member outside Lane C and remains stubbed. The `SymbolResultNode` getter depends on `RL` for its true branch. No out-of-scope helper or Tier B member was implemented. Run the normal gate next. Reference artifact: Core SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`; IL captured through `python tools\reference\reference.py Ptc.Calculation.Core.UnitDescriptor --assembly Ptc.Calculation.Core --il`.

After this gate, refresh current Lane C ownership and inventory before selecting another member group. Known blockers include `UnitDescriptor.Key`/`RL` (`Ptc.EnumerableExtensions.IsNullOrEmpty`), `CalculatableAggregateExtensions.GetAggregateRl` (`Ptc.EnumerableExtensions.Each`), trace converter validation (trace type/error dependencies), vector lazy conversion (active matrix/list work), and the ResultConverter Units shape mismatch. No Lane A/B files, shared validation baselines, or maintainer acceptance lists were changed.
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
