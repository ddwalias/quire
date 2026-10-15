# Lane B checkpoint

Updated: 2026-10-16

## Current main and merged work

Checkpoint base: Linux main `6bfaea5f1646e45f81330beebaaa625d3ac220ac` (current main observed before these queued MatVal branches). Earlier checkpoint bases below are historical and must not be treated as current integration state. Lane B history is retained compactly here; older Windows worktree results are historical and are not the active handoff.

| Batch | Main status |
|---|---|
| Function / FunctionUtilities, function matrix bridge and follow-up | Merged after passing runtime gates; Function evidence refresh recorded at `26e402e`. |
| Engine interfaces | Merged; focused gate passed. A Windows phase report had a pre-existing NumVal issue, corrected below. |
| MatVal collection/growth and views/export | Merged after passing runtime gates. Evidence records nine view/export structure differences. |
| MatDim/MatVal dimension/index | Implementations are present on main; stronger direct tests and evidence are pending on the MatVal feature branch. |
| DimVal.ApplyMatVal | The older `mirror/b-dimval-apply` branch remains pending; its implementation is separate from the new sensitivity-only batch. |

The earlier Windows report about missing `Mcd.NumVal.Compare` is not a current blocker: the current manifest and original member inventory have no such member. See [NumVal evidence](../evidence/Mcd.NumVal.md).

## MatDim/MatVal pending batch

Feature branch: `mirror/b-matval-gate-feature`, candidate `0028f2c`, rebased onto `b107792` (original reviewed candidate `02ecce1` was based on `c011f35`). Test/evidence/acceptance content is unchanged by the clean rebase. The scoped run on that older base passed Harness 36/36, Mpl 159/159 and Differential 75/75, killed 298/299 mutants, and had lint 0/ratchet pass. Independent review passed. Its one equivalent `MatVal.get_Shared` threshold proposal was approved by the user and added to this branch only; it is not accepted on main. The three former MatDim proposals were withdrawn and have direct killing cases.

The subsequent full runtime gate record is `/tmp/quire-matval-final-runtime-gate/gate.json`. It is **not a pass**: validator exit 241, no sealed summary or final ratchet verdict. Mutation was interrupted at 1,180/2,112. Do not treat the candidate as gate-cleared or merge it. The type-wide audit still reports 134 missing members, 120 structure mismatches and 19 verified-unsupported members; zero uncovered lines/branches does not mean the type is complete. Details and the exact scoped run remain in the branch checkpoint and evidence registers.

## DimVal pending batch

The stronger DimVal candidate is `mirror/a-dimval-sensitivity` at `b4867dc`, rebased onto `b107792`. Four targeted DimVal facts passed, and the full diff review found the 19 exact DynCall/VecCall actual-count proposals sound as equivalent. User approval for those proposals is still pending; they must not be added to main's `accepted-mutants.txt` before approval. The earlier `mirror/b-dimval-apply` implementation is not imported by this test-only batch. Exact proposal keys and the scoped test command are in [Lane A](lane-a.md#dimval-differential-sensitivity-follow-up), with matching branch records in the published backup.

## Queued Lane B handoffs

Agent A owns integration. The following changes are separate queued tips based on the current main above; none is assumed merged. They touch the shared `Mcd.MatVal.md` evidence and `lane-b.md` checkpoint independently, so preserve their distinct sections when resolving integration. The coordination note is [lane-b shared-file coordination](../../../../quire-handoffs/lane-b/coordination.md).

| Batch | Branch / tip | Status | Scope |
|---|---|---|---|
| MatVal pointer helpers | `mirror/b-matval-pointer-helpers` / `3cf3136cbacf3ab44e99e3b3cf4e767e2797efdc` | Frozen, reviewed, pushed; [ready handoff](../../../../quire-handoffs/lane-b/matval-pointer-helpers.md) | Four pointer helpers; 34 direct comparisons. |
| MatVal pointer properties | `mirror/b-matval-pointer-properties` / `e68e5ac688665078f32941d48ac83691a764be3e` | Merged to main at `59df412`; [handoff](../../../../quire-handoffs/lane-b/matval-pointer-properties.md) | Five pointer/uniformity properties; 40 direct comparisons. |
| MatVal dimension properties | `mirror/b-matval-dimension-properties` / frozen tip in [ready handoff](../../../../quire-handoffs/lane-b/matval-dimension-properties.md) | Independent review passed, pushed, ready | `IsDimless`, `IsUniformDimOf`, `PhysicalDimension`, `UniformDimension`; 31 direct comparisons. |

## MatVal pointer property batch — active checkpoint

Base: `6bfaea5f1646e45f81330beebaaa625d3ac220ac`; worktree `/root/src/quire-matval-pointer-properties`.
Original identity remains mplrun Prime 12.0.1.0 SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; reference source and IL were regenerated/read through `python3 tools/reference/reference.py Mcd.MatVal --il`.
Exact direct member tokens are recorded in `docs/mirror/evidence/Mcd.MatVal.md`; five test facts compare 40 cases (HasPtrs 9, RepHasPtrs 4, IsUniformScalar 8, IsUniformScalarPtrs 9, IsUniform 10), including return/exception, storage/table/pointer identity and mutation states. The state observer retains the prior Hashtable alias to check that `RepHasPtrs` empties it before nulling the field. This is case-limited evidence, not whole-type equivalence.

Commands and receipts:

```text
dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj --filter 'FullyQualifiedName~MatValAccessTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 8 passed, 0 failed, 0 skipped.

QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj --filter 'FullyQualifiedName~MatValPointerPropertiesDifferentialTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 5 passed, 0 failed, 0 skipped; 40 direct MethodComparison cases.
```

No implementation source changed, no scoped mutation run was requested or used, and no acceptance exception was proposed. Independent whole-diff review passed after adding a prior-table alias observation for the `RepHasPtrs` clear. Windows/native and full merge-gate checks remain unperformed and are not claimed; the focused Linux differential run was strict and loaded the pinned Mathcad artifact. The branch is a single reviewed commit, pushed as backup, and frozen at the exact tip in the atomic handoff linked above. Next action: start the dimension-query batch on a new branch from freshly inspected current main.

The next independent batch is the four dimension queries `get_IsDimless`, `IsUniformDimOf`, `get_PhysicalDimension`, and `get_UniformDimension`; isolate `get_MatrixDimension` because it crosses into MatDim factory behavior. Use the evidence analysis and exact member contracts before changing files.

## MatVal dimension-property batch — active checkpoint

Worktree: `/root/src/quire-matval-dimension-properties`. Original identity: mplrun Prime 12.0.1.0 SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. The hash-keyed reference was regenerated/read with `python3 tools/reference/reference.py Mcd.MatVal --il`; exact tokens/signatures are in `docs/mirror/evidence/Mcd.MatVal.md`. Original source confirms `IsUniformDimOf` tests live pointed values against current `m_dim`, assigns supplied `d` when live count equals `Rows*Cols` (including zero area), and otherwise falls back to `Dim.Eq_b`. `PhysicalDimension` virtually delegates to `UniformDimension`; `UniformDimension` may canonicalize via `IsUniform`. Existing broad pointer query scenarios logged these results but did not directly attribute them with MethodComparison.

Four direct-comparison facts cover 31 finite cases (IsDimless 7, IsUniformDimOf 10, PhysicalDimension 6, UniformDimension 8). The cases compare exact original tokens, return/exception outcomes, matrix storage/dimension state, table and pointer identities, returned dimension identity, requested-dimension argument identity, canonicalization, stale/live pointers, shared padded storage, mismatched dimensions, null dimension assignment, and zero-area full coverage. State snapshots retain the prior Hashtable alias so the comparisons also detect in-place table clearing during canonicalization.

```text
dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj --filter 'FullyQualifiedName~MatValDimensionTests|FullyQualifiedName~MatValAccessTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 14 passed, 0 failed, 0 skipped.

QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj --filter 'FullyQualifiedName~MatValDimensionPropertiesDifferentialTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 4 passed, 0 failed, 0 skipped; 31 direct MethodComparison cases.

git diff --check
=> passed.
```

No production source changed; no scoped mutation run was requested or used and no acceptance exception was proposed. These getters depend on the separately queued pointer-property branch for direct comparisons of `IsUniformScalar`/`IsUniform` and their callees. This dependency is coordinated in the handoff table; it is not claimed integrated. `get_MatrixDimension` remains a separate cross-lane MatDim factory batch. Windows phase closure and the full runtime merge gate remain unperformed; no phase completion is claimed from focused tests.

Independent whole-diff review passed after adding full-coverage null-dimension assignment and an argument-to-`m_dim` reference check. This batch is a single reviewed commit, pushed as backup, and frozen at the exact tip recorded in its atomic handoff linked above. Next action: start the next independent Lane B batch from freshly inspected current main.

Keep this checkpoint authoritative for Lane B; WORK is an index. Do not repeat old run logs here.

## MatVal pointer-helper mutation remediation — active checkpoint

The old candidate `mirror/b-matval-pointer-helpers` at `3cf3136cbacf3ab44e99e3b3cf4e767e2797efdc` remains frozen and unmerged; its integration gate was blocked by nine survivors (25/34 killed), detailed in the [old handoff](../../../../quire-handoffs/lane-b/matval-pointer-helpers.md). This remediation is a separate branch `mirror/b-matval-pointer-helper-remediation` in `/root/src/quire-matval-pointer-helper-remediation`, freshly based on clean main `92e763bf581a737c8c7ce3aac0fa5f3c4175f0c1`. Only this worktree is mutable. The shared evidence was inspected against the exact original before adding the four helper contracts; no production source changes are in scope because the implementations are already present on current main.

Original artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. The hash-keyed reference was regenerated/read with `python3 tools/reference/reference.py Mcd.MatVal --il`; exact tokens are `Canonicalize()` 0x0600043b, `TidyPtrs(int,int,Dim,bool)` 0x06000442, `IsUniformPtrs()` 0x06000458, and `IsUniformPtrsNoChange()` 0x06000459. Their original contracts are recorded in `docs/mirror/evidence/Mcd.MatVal.md`; `CanonicalizePtrs()` remains a separate direct-comparison gap.

The direct suite now has 46 comparisons across four facts (Canonicalize 10, TidyPtrs 15, IsUniformPtrs 15, IsUniformPtrsNoChange 6). Its state observer compares matrix/backing/table identity, pointer-object identity, and each NumVal pointer dimension before and after. The final case makes the matrix dimension incompatible with the common dimension of its full live-pointer coverage, directly checking the original full-coverage adoption path and killing the remaining count mutation.

```text
dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj --filter 'FullyQualifiedName~MatValAccessTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 8 passed, 0 failed, 0 skipped.

QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj --filter 'FullyQualifiedName~MatValPointerHelpersDifferentialTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 4 passed, 0 failed, 0 skipped; 46 direct MethodComparison cases.

python3 tools/validation/validate.py --method-type Quire.Mcd.MatVal --mutate --jobs 4
=> /tmp/quire-validation-gpjyf7i3/summary.json; Harness 36/36, Mpl 160/160, Differential 92/92; method coverage compared 45, declaration-only 0, excluded 0, missing 143, structure mismatch 109, uncovered 0, verified-unsupported 32; integrity lint 0/complete; ratchet pass; mutation 230/230 killed, 0 survivors, 0 accepted; focused gate would pass with the listed inventory gaps.
```

No exception or unreachable proposal is made. This scoped Linux run does not establish whole-type equivalence or phase completion. Windows/native and full merge-gate checks remain unperformed. The batch was squashed in one commit, passed independent whole-diff review, was pushed as backup, and is frozen at the exact tip recorded in the [handoff](../../../../quire-handoffs/lane-b/matval-pointer-helper-remediation.md). Agent A owns integration. Do not amend this queued tip concurrently.

## MatVal CanonicalizePtrs — queued handoff

This is the next independent Lane B batch on branch `mirror/b-matval-canonicalize-ptrs` in `/root/src/quire-matval-canonicalize-ptrs`, freshly based on clean main `92e763bf581a737c8c7ce3aac0fa5f3c4175f0c1`. The old pointer-helper and classifier branches remain separate queued tips; no integration is assumed. This batch directly compares the private `CanonicalizePtrs()` method, token `0x06000441`; source and exact original IL have been refreshed with `python3 tools/reference/reference.py Mcd.MatVal --il`.

Original identity is Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Evidence contract is recorded in `docs/mirror/evidence/Mcd.MatVal.md`. The 18 direct cases cover dimension normalization/order, logical nonpointer versus pointer-only dimensions, stale/live pointers, matching/incompatible/zero dimensions, non-NumVal pointers, full/partial coverage, both reinsertion paths, shared padded storage, and original exceptions with post-state. The observer compares return/exception, matrix dimension reference, flags/shape/offset/stride, backing/table references and prior aliases, pointer identities, and NumVal values/dimension references.

Validation commands and results:

```text
dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj --filter 'FullyQualifiedName~MatValAccessTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 8 passed, 0 failed, 0 skipped.

QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj --filter 'FullyQualifiedName~MatValCanonicalizePtrsDifferentialTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 1 fact passed, 0 failed, 0 skipped; 18 direct MethodComparison cases.

python3 tools/validation/validate.py --method-type Quire.Mcd.MatVal --mutate --jobs 4
=> /tmp/quire-validation-dc0wh8p3/summary.json: Harness 36/36, Mpl 160/160, Differential 89/89; method inventory compared 41, declaration-only 0, excluded 0, missing 147, structure mismatch 109, uncovered 0, verified-unsupported 32; integrity lint 0/complete; ratchet pass; mutation 179/179 killed, zero survivors, zero accepted/proposed. Focused local checks pass; the scoped gate would pass with the listed inventory gaps.
```

No acceptance or unreachable proposal is made. This batch is a single reviewed commit, pushed as backup, and frozen at the exact tip recorded in its [handoff](../../../../quire-handoffs/lane-b/matval-canonicalize-ptrs.md).

## MatVal ReinsertPtrs / ReinsertPtrs1 — active

This adjacent two-member private-helper batch is on branch `mirror/b-matval-reinsert-ptrs` in `/root/src/quire-matval-reinsert-ptrs`, freshly based on clean main `92e763bf581a737c8c7ce3aac0fa5f3c4175f0c1`. Neither frozen pointer-helper nor CanonicalizePtrs work is assumed integrated. The exact original Prime 12.0.1.0 reference was refreshed with `python3 tools/reference/reference.py Mcd.MatVal --il`; exact members are `ReinsertPtrs()` token `0x06000443` and `ReinsertPtrs1()` token `0x06000444`.

Artifact SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Evidence contracts are in `docs/mirror/evidence/Mcd.MatVal.md`. Direct comparisons cover reinsertion/materialization versus retained pointers, stale entries, the `mcd_zero` mutation unique to `ReinsertPtrs1`, Shared padded/offset packing and index remapping, backing/table/pointer identities, and original exception state. `MatValReinsertPtrsDifferentialTests` contains 23 direct cases (11 for `ReinsertPtrs`, 12 for `ReinsertPtrs1`), each checking exact tokens and a state snapshot of matrix dimension, backing and geometry, old/current array and table aliases, and pointer/NumVal identity, value and dimension.

Validation commands and results:

```text
dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj --filter 'FullyQualifiedName~MatValAccessTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 8 passed, 0 failed, 0 skipped.

QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj --filter 'FullyQualifiedName~MatValReinsertPtrsDifferentialTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 2 facts passed, 0 failed/skipped; 23 direct MethodComparison cases.

QUIRE_REQUIRE_MATHCAD=1 python3 tools/validation/validate.py --method-type Quire.Mcd.MatVal --mutate --jobs 4
=> Harness 36/36, Mpl 160/160, Differential 90/90; inventory compared 41, missing 147, structure mismatches 109, uncovered 0, verified-unsupported 32; integrity 0/complete and ratchet pass; 179/179 mutants killed, 0 survivors/proposals. LOCAL CHECKS pass; scoped gate would pass with existing gaps. Receipt: /tmp/quire-validation-84au0rip/summary.json.

git diff --check
=> passed.
```

No production source changed, and no acceptance or unreachable proposal is made. Windows/native phase closure and the full runtime merge gate remain unperformed; the scoped run is not a phase-completion claim. This branch is one squashed commit pushed for backup. Its exact tip and independent review verdict will be recorded in the atomic handoff.

## MatVal contains(object)

Branch `mirror/b-matval-contains`, worktree `/root/src/quire-matval-contains`, was freshly based on clean main `d1f744900cc2cc6c7dc43056a9f0eaf4d030ee69`. The pointer-slot-writer handoff at `def1c0b3420c2d762661e745eb866d3ff5e06a34` remains separate and was not assumed integrated. Original Prime 12.0.1.0 artifact identity was refreshed with `python3 tools/reference/reference.py Mcd.MatVal --il`: `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Exact public instance signature `Mcd.MatVal.contains(object) -> bool`, token `0x0600051a`, is recorded in `docs/mirror/evidence/Mcd.MatVal.md`. Original source/IL enumerates table values, matches by reference identity, and recursively visits only MatVal-valued entries. It ignores keys, PtrNan markers, dimensions, and value equality, and does not mutate state.

The direct suite in `tests/Quire.Differential.Tests/MatValContainsDifferentialTests.cs` compares 15 finite cases in one fact. It covers null/empty tables, null query/value, same and equal-but-distinct NumVal/StrVal/object/string references, arbitrary keys, stale values, unrelated live pointer slots, nested present/absent values through multiple levels, and unchanged retained table/backing/dimension/entry identities and contents. Recursive cycles and concurrent table mutation are excluded because the original has no cycle guard and its enumeration can be invalidated.

```text
dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj --filter 'FullyQualifiedName~MatValAccessTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 8 passed, 0 failed, 0 skipped.

QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj --filter 'FullyQualifiedName~MatValContainsDifferentialTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 1 passed, 0 failed, 0 skipped; 15 direct MethodComparison cases against token 0x0600051a.

git diff --check
=> passed.
```

No production source changed, so no scoped mutation run was required. No acceptance or unreachable proposal is made. Windows/native and full merge-gate checks remain unperformed; these focused Linux checks do not establish whole-type equivalence or phase completion. Independent whole-diff review, squash, backup push and frozen handoff are pending. Remaining Lane B work follows the current queue in `WORK.md`; this branch must remain isolated from the queued pointer-slot-writer tip.

## DimVal.ApplyMatVal — active checkpoint

Current main was first inspected at `71c16c5dc2fba8a836f398a78c24fd41159012de`; after Agent A integrated the unrelated SCons batch, this active branch was rebased onto current main `92e763bf581a737c8c7ce3aac0fa5f3c4175f0c1`. Worktree `/root/src/quire-dimval-apply-current`, branch `mirror/b-dimval-apply-current`. The old `origin/mirror/b-dimval-apply` tip `73c53d539871700ea0cc41536a7a60ad4eacc936` remains separate and is not the base for this batch because it is not descended from current main and its full diff includes unrelated intervening changes.

Scope is only public static `DimVal.ApplyMatVal(DimVal fv, DimVal mv)` (`0x060006b0`). Fresh same-artifact IL/C# and member index were generated in this worktree with `python3 tools/reference/reference.py Mcd.DimVal --il`: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Original flow is `mv.asMatrix(2u)`, then `MatVal.PackObjects()`, then virtual `fv.Apply(object[])`. Relevant caller/dependency behavior: base `DimVal.asMatrix(uint)` delegates to the error conversion at argument 2; `MatVal.asMatrix` returns itself; `PackObjects` loops columns outer/rows inner through `get_2` and can expose pointer objects or mutate stale-marker storage. No production caller was found. Contracts are in `docs/mirror/evidence/Mcd.DimVal.md`.

The implementation is a separate partial member matching that observed order; the generated `[MirrorStub]` pair was removed per PLAN. Five `MethodComparison` cases in `DimValApplyMatValDifferentialTests` cover a padded non-symmetric 2×3 matrix and column-major packed order, live pointer identity, retained stale table entry, missing pointer marker conversion/mutation to NaN, base `Apply` exception, and matrix-conversion exception before callback invocation. The comparisons observe callback calls, ordered arguments, returned array identity, pointer aliases, and matrix storage/table state. Do not import the old branch's unrelated broader DimVal test changes. Do not edit or accept `accepted-mutants.txt`, `unreachable-lines.txt`, or any of the 19 proposed DynCall/VecCall arity exceptions; those remain unapproved and outside this member batch.

Commands and receipts:

```text
dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj --filter 'FullyQualifiedName~FunctionTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 8 passed, 0 failed, 0 skipped.

QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj --filter 'FullyQualifiedName~DimValApplyMatValDifferentialTests' -m:1 -nr:false -p:UseSharedCompilation=false
=> 3 passed, 0 failed, 0 skipped; 5 direct MethodComparison cases.

git diff --check
=> passed.

python3 tools/validation/validate.py --method-type Quire.Mcd.DimVal --jobs 4
=> Harness 36/36, Mpl 160/160, Differential 91/91; lint 0, integrity complete, ratchet ok. Scoped method coverage: compared 102, declaration-only 0, missing 1, structure mismatch 0, uncovered 21, verified-unsupported 22. LOCAL CHECKS: pass; GATE: would pass with gaps; mutation deferred to gate. Receipt: `/tmp/quire-validation-w_v25vbn/summary.json`.
```

An initial filtered type-scoped diagnostic selected only these three facts, so it left existing `DimVal` comparisons untested and produced 125 ratchet regressions; the unfiltered receipt above is the complete scoped run. Strict comparisons loaded the pinned Mathcad artifact. No scoped mutation was run; the full runtime merge gate owns mutation. Windows/native and full runtime merge-gate checks remain unperformed, and this member result does not claim phase completion. No acceptance proposal was made. Next: obtain independent whole-diff review, squash/push, and publish a ready handoff.
