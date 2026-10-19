# Lane C work checkpoint

## Scope and ownership

Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. Lane A and Lane B files are out of scope. Use a separate worktree per candidate; never switch or share the shared clone. Current `origin/main` is `572eb622e62b95aa248ff559a2f49129c3d2dc30`.

## Active handoff

No independent unowned Calculation/Core cctor remains. `DefaultUnitDescriptorTable::.cctor()` is Tier B and excluded; `EngineErrorMessages::.cctor()` is part of the 35-member batch; `ResultManager::.cctor()` plus its constructor and `Clear` are on `mirror/c-calc-core-cctors`; `CalculationController::.cctor()` is on `mirror/c-calc-controller-cctor`. Other cctors have dedicated C candidate branches listed below.

Normal gates against the current base have finished without merging:

- `mirror/c-calc-simple-result-factory-cctor` (the requested 35-member Core batch plus `SimpleResultNodeFactory::.cctor()`): generated-stub-pair path check passed; Harness 97/97, Mpl 188/188, focused Differential 393/393, Release Differential 393/393, MethodAudit, integrity lint 0, ratchet and mutation passed. Phase suites passed Calculation 8/8, Controls 16/16 and Core 7/7, then the Differential runner created `tests/Quire.Differential.Tests/bin/Debug/net10.0-windows/s1000001`; phase validation stopped before ratchet. Exact reports: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-wt8n2bxf\run\summary.json` and `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-8im9hwn2\run\summary.json`.
- `mirror/c-calc-request-show-error-event-args`: direct Release Mathcad 3/3 and exact Release MethodAudit. Focused checks, Release Differential 393/393, MethodAudit (1547 exact / 77 structure-only), integrity lint 0, ratchet and mutation passed. Phase suites passed Calculation 8/8, Controls 16/16 and Core 7/7; Differential exited `4294967295` and wrote the same `s1000001` path. Its log also shows pre-existing failures in `PtcActionItem`, `PtcXmlSchemasManager`, and `PtcControlsIOUnitMonomialBuilder`; the latter reaches Tier B `DefaultUnitDescriptorTable`. Exact reports: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-6zyhp2_l\run\summary.json` and `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-y8c2m39g\run\summary.json`.
- `mirror/c-calc-trace-result-analyzer-methods`: five direct Release Mathcad comparisons pass 5/5 and Release MethodAudit is exact. Focused checks, Release Differential 393/393, MethodAudit, integrity and ratchet passed. Phase validation stopped before tests because it could not find the built `Quire.Mpl.Tests` assembly (empty reported path). Exact reports: `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-rew1japf\run\summary.json` and `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-phase-dw8kpeps\run\summary.json`.

These Phase failures are outside the three batch diffs. Do not edit baselines, shared validation tooling, `s1000001`, acceptance lists, or other lanes' files. Retry only after the Phase runner/build blocker changes; continue independent C work now.

The `mirror/c-calc-string-list-meaningful` normal gate rebased to candidate `3d94a578` on main `572eb622` and stopped before focused tests because `Quire.Focused.Differential.Tests.csproj` build failed. `build.log` at `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-fzu2t8os\run\build.log` ends after dependency builds and has no build error; the summary is `C:\Users\DuyDang\AppData\Local\Temp\quire-gate-results-fzu2t8os\run\summary.json`. It did not merge. Retry when the concurrent B lane build clears. The batch implements only Tier A `StringListResultNode.IsMeaningfulValue(string)` (`0x0600025f`). ILSpy evidence is `return value != null`; original IL is `ldarg.1; ldnull; cgt.un; ret`. Its direct Release Mathcad test passed 1/1 with `QUIRE_REQUIRE_MATHCAD=1`; Release MethodAudit inspected four declarations with 0 errors and found Quire token `0x06000315` exactly matched to Mathcad token `0x0600025f`. Audit artifact: `C:\Users\DuyDang\AppData\Local\Temp\quire-calc-string-list-meaningful-release-audit\inventory.json`. The full existing class filter also exposes two pre-existing Tier B constructor failures: `ctor_0600025d` leaves `_meaningfulValuesCount` 0 instead of -1; `ctor_0600025e` returns normally for null where Mathcad throws `ArgumentNullException`. They remain out of Lane C Tier A scope.

Current branch: `mirror/c-calc-engine-result-ctors`, worktree `C:\Projects\Quire-wt\c-calc-engine-result-ctors`; implementation and evidence are pushed. It implements independent `EngineResult` constructors at Mathcad tokens `0x06000051`, `0x06000052`, `0x06000054`, and `0x06000056`. All 14 members in its existing differential test class pass in Release with Mathcad. Release MethodAudit inspected 19 declarations with 0 errors; all four new constructor bodies are exact (Quire tokens `0x0600005a`–`0x0600005d`). Audit artifact: `C:\Users\DuyDang\AppData\Local\Temp\quire-calc-engine-result-ctors-release-audit\engine-result-14.json`. First default-parallel build attempt exited `MSB4166` (child node 16); retry with `-m:1` passed the full class 14/14. Checkpoint, evidence, implementation and tests are complete; gate after the StringList retry and current B lane gate. Remaining EngineResult members require separate dependency review: `.ctor(EngineResult, EngineError)` depends on EngineErrorCollection; the remaining warnings constructor depends on error collection state and calls out-of-scope `DebugTools`; `IsEmpty` depends on error count / null-or-empty helpers; ResultXml accessors depend on unfinished transform methods.

StringList's focused-build blocker is unrelated to its verified member and is recorded above. Continue with the prepared EngineResult constructor batch, then take the next unowned Tier A batch only after refreshing `origin/main`, checking all C worktree owners, and reviewing the type's evidence, callers, and affected tests. `ComplexSimpleResultNode` is currently owned by the 35-member branch, so do not edit it until that batch lands.

## Current Lane C candidate owners

| Branch/worktree | Owned batch or status |
|---|---|
| `mirror/c-calc-simple-result-factory-cctor` | 35-member Core MemberGen batch and `SimpleResultNodeFactory::.cctor()`; latest normal gate stopped in Phase Differential at `s1000001` |
| `mirror/c-calc-core-cctors` | Event-args singleton constructors and `ResultManager` `.ctor`, `Clear`, `.cctor`; prior gate stopped at the old Release assembly lookup, retry after current gates |
| `mirror/c-calc-core-constants` | `CalcResultsProcessingPriorities` and `CalculationOptionDefaults` initializers |
| `mirror/c-calc-core-redefinition-warnings` | `RedefinitionWarningsDictionary::.cctor()` |
| `mirror/c-calc-core-trace-types` | `TraceTypeDefinitions::.cctor()` |
| `mirror/c-calc-core-vector-serializer` | `VectorsListJSonSerializer<T>::.cctor()` and serializer behavior |
| `mirror/c-calc-core-xmcd-transform` | `XmcdToIResultNodeTransform::.cctor()` |
| `mirror/c-calc-controller-cctor` | `CalculationController::.cctor()` |
| `mirror/c-calc-enginebuilder-cctor` | `EngineBuilder::.cctor()` |
| `mirror/c-calc-matvalue-proxy-cctor` | `MatrixValueProxyWrapper::.cctor()` |
| `mirror/c-calc-engine-error-collection` | `EngineErrorCollection(IEnumerable<EngineError>, EngineError)` |
| `mirror/c-calc-engine-error-placeholder` | `EngineErrorExtensions.IsPlaceholderError` |
| `mirror/c-calc-request-show-error-event-args` | `RequestShowErrorEventArgs` ctor, `Error` getter/setter; Phase blocked at `s1000001` |
| `mirror/c-calc-result-node-utils` | `ResultNodeUtils.TryParsePlottableValue`; direct Release tests pass, gate retry pending |
| `mirror/c-calc-trace-data-points` | `TraceDataPoints` constructor and `IsEmpty` |
| `mirror/c-calc-trace-error-codes` | trace error code members |
| `mirror/c-calc-trace-result-analyzer-methods` | five unblocked `TraceResultAnalyzer` methods; three blocked helper-dependent methods; Phase Mpl assembly lookup failed |
| `mirror/c-calc-vectors-legacy-readhelpers` | vector legacy read helpers; direct Release tests pass, previous test-host crash needs gate retry |
| `mirror/c-calc-string-list-meaningful` | `StringListResultNode.IsMeaningfulValue`, direct Release and exact IL pass; latest gate stopped during focused test build |
| `mirror/c-calc-engine-result-ctors` | Current batch: three independent `EngineResult` constructors, Release direct class 13/13 and exact IL pass; normal gate next after the concurrent B gate exits |

## Completed direct evidence and remaining gaps

The 35-member Core MemberGen group is 36/36 direct comparisons, all 36 accepted tokens exact in Release MethodAudit, with 12 implemented cctors exact. The SimpleResultNodeFactory cctor comparison passes 1/1 and is exact; factory-method comparisons still depend on incomplete result-node state. RequestShowErrorEventArgs direct Release comparisons pass 3/3 after identity-safe rendering for Tier B `EngineError`. `ResultManager` affected Release comparisons pass 6/6 and its constructor, `Clear`, and `.cctor` are exact; the full class has a Tier B `AddOldResult` dependency. The five implemented TraceResultAnalyzer methods pass direct Release 5/5 and exact Release IL. `StringListResultNode.IsMeaningfulValue` passes direct Release 1/1 and exact IL. The four selected EngineResult constructors pass direct Release comparisons; all 14 EngineResult direct tests now pass with a type-local numeric enum renderer.

The previous Release sweep of changed Core classes passed 145/151. Remaining failures were two calls into `XmcdToIResultNodeTransform.GetIntegerFromAttribute`, two calls into `ResultNodeToXmcdTransform.WriteDisplayMatrixXml` / `WriteMatrixXml`, and the two Tier B `StringListResultNode` constructors. Keep fixes within Lane C Tier A; do not implement Tier B bodies or change another lane's files.

## Validation and blockers

The old `f863245e` checkpoint and claim that generated stub-pair deletions needed maintainer approval were stale. The updated gate accepts whole generated stub/member-pair deletions across projects; the requested 35-member batch passed its generated-path check without an override. Current fetched main at the latest recorded point is `572eb622`.

No `--allow-protected`, protected-path override, baseline edit, acceptance-list edit, validation-tool edit, or Lane A/B file change was made. StringList gate report records a focused test-project build failure while concurrent validation builds were running; no candidate edits were made during its gate. Keep Release output/audit evidence current and record new gate summaries before the next candidate is queued.
