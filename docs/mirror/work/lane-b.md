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
