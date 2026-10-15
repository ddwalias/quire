# Lane B checkpoint

Updated: 2026-10-16

## Current main and merged work

Checkpoint base for this batch: Linux main `6bfaea5`. Earlier candidate branches still rooted at `b107792` remain separate and unmerged; their receipts below are historical for those exact tips. Lane B history is retained compactly here; older Windows worktree results are historical and are not the active handoff.

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

## Next action

- After the pending DimVal proposal approval, the coordinator should rebase both reviewed runtime batches onto current main, review the complete final diff and conflict resolutions, preserve their separate squashed commits on an integration branch, and run the combined full runtime gate under the current PLAN. Merge only if that fresh full audit passes; a scoped pass is not a substitute.
- If approval does not arrive, leave both pending batches out and continue independent PLAN work.

## MatVal pointer helpers

The `mirror/b-matval-pointer-helpers` batch is a direct-test and evidence follow-up from main `6bfaea5`; production source is unchanged. The installed Prime 12.0.1.0 `mplrun.dll` identity matches the MatVal evidence: SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. The four selected original members and tokens are `Canonicalize()` (0x0600043b), `TidyPtrs(int32,int32,Mcd.Dim,bool)` (0x06000442), `IsUniformPtrs()` (0x06000458), and `IsUniformPtrsNoChange()` (0x06000459). Contracts and case limits are in [MatVal evidence](../evidence/Mcd.MatVal.md).

Validation on the branch:

- `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj --filter 'FullyQualifiedName~MatValAccessTests' -m:1 -nr:false -p:UseSharedCompilation=false` — 8 passed, 0 failed, 0 skipped.
- `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj --filter 'FullyQualifiedName~MatValPointerHelpersDifferentialTests' -m:1 -nr:false -p:UseSharedCompilation=false` — 4 facts passed, 0 failed, 0 skipped; 34 direct comparisons (10 Canonicalize, 11 TidyPtrs, 7 IsUniformPtrs, 6 IsUniformPtrsNoChange). These finite cases do not establish whole-type or phase completion.
- Local scoped mutation was not run; no survivor or unreachable-line proposal is included. The final runtime gate remains authoritative.

Independent review of the whole diff passed after three requested cases were added: full TidyPtrs coverage with null common dimension, partial compatible zero-pointer mutation with another pointer present, and pointer-object identity in state comparison. No `src/` code or acceptance files changed. `CanonicalizePtrs()` remains a separate direct-comparison gap; its execution through `Canonicalize()` does not establish a direct method comparison. Windows phase closure has not run.

The next MatVal pointer-related properties (`HasPtrs`, uniformity, and dimension queries) remain a separate batch. Their tests need a new branch from then-current main; the [Lane B pointer-helper handoff](../../../../quire-handoffs/lane-b/matval-pointer-helpers.md) records the shared MatVal evidence-file change for integration. Keep the queued branch frozen.

Keep this checkpoint authoritative for Lane B; WORK is an index. Do not repeat old run logs here.
