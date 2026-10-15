# Lane B checkpoint

Updated: 2026-10-15

## Current main and merged work

Checkpoint base: Linux main `b107792` (after the NumVal scope correction). Lane B history is retained compactly here; older Windows worktree results are historical and are not the active handoff.

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

The next small Lane B batch is the four MatVal pointer helpers: `Canonicalize()`, `TidyPtrs(int,int,Dim,bool)`, `IsUniformPtrs()`, and `IsUniformPtrsNoChange()`. Reconstruct direct tests from original source/cases. Read the exact original signatures/tokens and existing MatVal evidence first; add artifact-bound direct comparisons for each private helper and its state effects. Treat pointer-related properties (`HasPtrs`, uniformity and dimension queries) as a separate follow-up. Use targeted local comparisons and the final gate under PLAN.

Keep this checkpoint authoritative for Lane B; WORK is an index. Do not repeat old run logs here.
