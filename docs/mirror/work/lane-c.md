# Lane C work checkpoint

## Scope and ownership

Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. This assignment removes both assemblies from Lane A's future scope. No other lane's active batch or files are part of this work.

Ownership is tracked against the active `mirror/c-calc-*` branches and worktrees. This checkpoint covers their combined Lane C Tier A ownership; reconcile every candidate with current `origin/main` and the live worktree list before starting another batch. `mirror/gen-calc-core` is already contained in `main`; the existing generated mirrors and evidence are the starting point, not duplicated work.

## Active handoff

**Next:** The existing 35-member MemberGen branch was based at `009f83df`; its previous gate stopped at Phase Release Differential (145/151). Four of the six Lane C failures are addressed here; renamed direct Release filters pass 22/22, including newly implemented Tier A `WriteRealXml` and `WriteImaginaryXml` that complete the populated matrix writer path. The two `StringListResultNode` constructor failures are addressed by `ListResultNode<T>` on `mirror/c-calc-listresultnode`; its targeted and phase Release comparisons pass 7/7. Its latest normal gate against base `5e4b3b95` did not merge: summary `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-ma51uzbs\run\summary.json`, phase Release TRX `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-ma51uzbs\run\release-results\Quire.Differential.Tests.trx`, and mirror-shape TRX `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-ma51uzbs\run\Quire.Mirror.Tests.trx`. The Lane C focused Release differential passed 7/7. The sole phase blocker is `Quire.Mirror.Tests.MirrorShapeTests.Project_keeps_its_mirrored_shape(project: "Quire.Engine")`, which reports 14 changed/disappeared Engine members: `IBuiltin.GetBuiltins`, `GetVariable`, `SetVariable`, `UnsetVariable`; `IEngine.RecalculateAll`, `IsUpToDate`; `IEnginePriv.Initialize`; `IEngineUnstable.GetNodeType`; `IFactory.Initialize`, `VerifyIntegrity`; `IFactoryFactory.AppendPath`; and `IMCEngine.AddNotify`, `RemoveNotify`, `StartNotify`. This is outside Lane C ownership; no Engine files or baselines were changed. Do not retry until the Engine shape blocker is resolved upstream. Continue with the independent Lane C batches below.

The separate Calculation batch `MatrixValueDataAccessorWrapper` is committed on `mirror/c-calc-matrix-data-accessor`; its affected Release Mathcad filter passes 2/2. The first normal gate, rebased onto `2c4d5d33`, passed focused and Release Differential 393/393 but stopped at integrity for a forbidden literal `Assembly.GetType` test lookup. Replacing that lookup with a Calculation-assembly type lookup passed integrity (lint 0, complete) and ratchet; its correction is commit `b6f4f1f9` on the remote branch. The retry gate rebased onto `b692fe8a` and passed local checks, focused and Release Differential 393/393, mutation selection, and the focused Phase Release differential 2/2. Phase validation failed only in `Quire.Mirror.Tests`. Summary reports: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-lcmy03q7\run\summary.json` and `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-cj014_34\run\summary.json`. Reproduced the failure using Release Quire with `dotnet test tests\Quire.Mirror.Tests\Quire.Mirror.Tests.csproj -c Release --filter 'FullyQualifiedName~MirrorShapeTests.Project_keeps_its_mirrored_shape'`: 29 passed, 1 failed. The failing case is `MirrorShapeTests.Project_keeps_its_mirrored_shape(project: "Quire.Engine")`; it reports 14 changed/disappeared Lane B Engine members: `IBuiltin.GetBuiltins`, `GetVariable`, `SetVariable`, `UnsetVariable`; `IEngine.RecalculateAll`, `IsUpToDate`; `IEnginePriv.Initialize`; `IEngineUnstable.GetNodeType`; `IFactory.Initialize`, `VerifyIntegrity`; `IFactoryFactory.AppendPath`; and `IMCEngine.AddNotify`, `RemoveNotify`, `StartNotify`. This is outside Lane C ownership; no Engine files or baselines were changed. The batch remains unmerged until that blocker is resolved. Latest fetched `origin/main` is `2082723a`. Four flag conversion members remain blocked on Tier C/out-of-lane `Ptc.Core.EnumHelper.TryParse`; details are in its evidence record.

The branch contains IL-based implementations and direct tests for `GetIntegerFromAttribute`, `WriteMatrixXml`, `WriteRealXml`, `WriteImaginaryXml`, and `WriteDisplayMatrixXml`; their renamed Release filters passed 22/22. The new 1x1 matrix proxy case traverses the writer cell loop and directly compares `ml:real` output. `WriteDisplayMatrixXml`'s normal size branches remain blocked by Quire.Core `Ptc.TableSize` stubs outside Lane C; direct cases compare null-display and null-viewport behavior. `ListResultNode<T>` owns its constructors, `ExtendList`, and lazy `MeaningfulValuesCount`; renamed lifecycle/accessor Release filters passed 7/7. Nonempty counting reaches Tier C `Quire.DoubleExtension.IsValid` in `Ptc.Core.dll`, so that case remains blocked outside Lane C. Its gate retry uses the same base and includes both test-shape fixes.

The independent `SimpleCalculatable` Core batch is committed on `mirror/c-calc-simplecalculatable-lifecycle` at `3f31e388` and passes 28/28 targeted Release comparisons after aligning its lifecycle class with the required test-file shape. Its IL-derived constructor uses numeric calculating-status value 2, and `SetSynchronizationStatus` invokes its callback only for numeric value 3; direct comparisons normalize enum values. `IsErrorOnScalingUnits` calls out-of-scope `Ptc.DebugTools.FailAssertion` and remains blocked. Gate this branch after the ListResultNode and accumulated Core gates so it rebases to current main. All remaining Calculation/Core cctors are owned by active C branches; `DefaultUnitDescriptorTable::.cctor()` is Tier B and excluded. `ResultManager` has implementation and direct Release evidence in `mirror/c-calc-core-cctors` and needs a normal gate against current main. No Lane A or B files were changed.

The independent Calculation batch `MatrixValueDataAccessorWrapper` is committed on `mirror/c-calc-matrix-data-accessor` at `5950f6bc`, based on `origin/main` `5a0f7754`. The Release command `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationMatrixValueDataAccessorWrapper_MechanicalDifferentialTests'` passes 2/2. It mirrors the constructor/accessor property, value-forwarding methods and first non-plain indices with direct interface-proxy comparisons. `GetEntireMatrixDataTypes`, both row/column type conversions, and the private conversion helper remain Blocked on `Ptc.Core.EnumHelper.TryParse`, outside Lane C's assignment; no Ptc.Core code was changed. Gate it after the current ListResultNode/C branches settle their checkpoint on `main`.

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

The failed Core gate used revision `4b70b1c2` against base `009f83df`; it did not merge. It passed generated-path validation, focused Release comparisons, focused coverage, MethodAudit, integrity and ratchet, then stopped during Phase Release Differential with the six failures recorded above. Reports: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-65lik2xd\run\summary.json`, `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-7kidmo3i\run\summary.json`, and `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-7kidmo3i\run\Quire.Differential.Tests.release.log`. The phase summary contains historical Debug counts and is not a request to inspect or rerun Debug locally. The Core transform Release filters pass 22/22, including numeric XML writers and the populated matrix path; ListResultNode Release filters pass 7/7. Nonempty double-list counting reaches Tier C `Quire.DoubleExtension.IsValid`; the base getter's empty-list lazy/cache path is directly compared. The Calculation accessor proxy/forwarder filters pass 2/2 in Release; four enum conversion methods remain blocked on out-of-scope `Ptc.Core.EnumHelper.TryParse`. Latest fetched `origin/main` is `5a0f77542a02be793e3c8d42b403e4e90f60ee79`; the ListResultNode retry is still in Phase validation and its wrapper will handle further main movement. No validation tool, acceptance list, baseline, or other lane file was changed. See [PLAN.md](../PLAN.md), [validation.md](../validation.md), and [validation flow](validation-flow.md).
