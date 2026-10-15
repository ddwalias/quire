# Lane B checkpoint

Updated: 2026-10-15

## Merged to main

- Function / FunctionUtilities / function matrix bridge (`mirror/b-function`, `a6ca35c0845c`). The merge gate passed after the VecCall and AsDelegate cases were sharpened: Harness 27, Mpl 159, Differential 74; 92/92 mutants killed; lint 0; ratchet passed.
- Engine interfaces (`IEngine`, `IFactory`, `NodeCallback`, `NodeStatus`; `mirror/b-engine`, `3924a7d091a3`). Focused validation passed: Harness 27, Mpl 159, Differential 74; lint 0; ratchet passed. The gate used the documented `--allow-unvalidated` option after the Windows phase exposed an unchanged Lane A shape mismatch, independently reviewed as pre-existing.
- MatVal collection/growth (`mirror/b-matval-collections`, `dfb65f3cd776`). The merge gate passed with Harness 27, Mpl 159, Differential 74, 60/60 mutants killed, lint 0, and ratchet passed. Main advanced to `e2532e3eacb1`.
- MatVal views/export (`mirror/b-matval-view`, `e168ce6f7b4e`). The merge gate passed with Harness 27, Mpl 159, Differential 77, 158/158 mutants killed, lint 0, and ratchet passed. Later Lane B merges advanced main to `8da569367595`; nine documented structure mismatches remain recorded in the evidence as reconstruction limits.
- Function follow-up (`mirror/b-function-followup`, `65abb4afadbc`). The merge gate passed with Harness 27, Mpl 159, Differential 77, 92/92 mutants killed, lint 0, and ratchet passed. Main advanced to `8da569367595`; five accessor/dispatch methods are Verified, and the abstract `FunctionUtilities` constructor remains Analyzed because reflection cannot directly invoke it.
- Function evidence refresh (`mirror/b-function-verify`, `b8cd1747a908`) is merged to main. Its run logs are preserved under `/tmp/quire-b-function-verify-scratch/function-verify-8da5693-r2`; the gate passed Harness 27, Mpl 159, Differential 77, lint 0, and ratchet. The selected audit had zero uncovered lines/branches and recorded nine structure differences. The documentation checkpoint was merged at `26e402e`.

## MatVal core

- The MatVal core commits are rebased onto main `387a7f9` and combined with the dimension/index work on `/mnt/c/Projects/Quire-wt/b-matval-dimensions`, branch `mirror/b-matval-dimensions`. Fresh combined validation at `scratch/matval-dimensions-core-387a` exited 3 on the type-wide inventory: Harness 27/27, Mpl 159/159, selected Differential tests 12/12, lint 0, ratchet ok, zero uncovered lines/branches, 107 compared, 134 missing, 120 structure mismatches, 19 verified unsupported. All 120 current mismatches are recorded in the MatDim/MatVal evidence registers.
- The combined branch includes four approved equivalent mutants: `MatVal.get_Shared` (`(m_flags & 4) > 0` to `> 1`; the masked value is only 0 or 4), `MatDim.IsScalarMultiple` and `MatDim.Mult` (loop start 1 to 0 repeats the first item), and `MatDim.Pwr` (nullability probe exponent 2 to 3). Other previously surviving killable MatVal mutants have direct cases. Their entries are in protected `accepted-mutants.txt`; the gate requires `--allow-protected` to accept that path, which the lane instructions prohibit. This combined branch cannot merge through the authorized gate.

## Other Lane B worktrees

- Function evidence refresh and its checkpoint are merged to main (`b8cd1747a908`, `26e402e`); the fresh run and gate details are recorded above.
- MatVal collection/growth is merged. Its `Fill` case distinguishes multiplication from division (2x1 matrix, stride 2, filling indices 0/4/8). Focused validation is recorded at `/tmp/quire-matval-collections-fill-validation-20261015`; the passing gate with 60/60 mutants killed is at `/tmp/quire-gate-results-i8ny5j9s/run`. The worktree and branch were removed after merge.
- MatDim and MatVal dimension/index: commits `0d5c824`, `7e3bcb0`, and `3e47966` were rebased together with MatVal core onto main `387a7f9` on `/mnt/c/Projects/Quire-wt/b-matval-dimensions`, branch `mirror/b-matval-dimensions`. The combined focused run is summarized above; all 120 structure mismatches are listed in the evidence registers. The earlier gate `/tmp/quire-gate-results-3gokqvbn/run` passed tests, lint, and ratchet but had the three approved equivalent MatDim survivors. The combined branch now carries the accepted entries, but the protected-path rule prevents using them at the gate without `--allow-protected`.
- `DimVal.ApplyMatVal`: commits `1e91e98` and `b9986cd` on `/mnt/c/Projects/Quire-wt/b-dimval-apply`, branch `mirror/b-dimval-apply`, rebased onto main `387a7f9`. Removed the replaced generated stub member, added direct cases for row-major 2x2 argument packing and scalar conversion failure, and marked the method Verified after direct comparisons. Fresh focused validation at `scratch/dimval-apply-387a-r4` passed Harness 27/27, Mpl 159/159, and Differential 79/79, with lint 0 and ratchet ok; it exits 3 because unrelated DimVal methods remain uncovered/missing. The prior-base merge gate `/tmp/quire-gate-results-vfzqua0o/run` blocked on 36/111 surviving mutants in other `DimVal` methods (including `get_IsDimlessReal`, `Rfinal`, `NearlyEqual`, `DynCall`, `VecCall`, `extend_*`, and `matrix_set0_*`). No ApplyMatVal mutant survived. These neighboring methods are outside this Lane B unit; no Lane A files were edited.

## Reviewed out-of-scope blocker

The Windows Phase 1 run of the Engine branch was recorded as having a Mirror.Tests failure for `Mcd.NumVal.Compare`, with the checkpoint claiming the manifest listed the member. Rechecked after the workflow-rules update on `local/windows-main` (`08946db27e3950d51001192cfa98898d812c40b7`), the `Quire.Mpl` manifest has no `Mcd.NumVal.Compare` row; the current source and generated declaration also omit it. The installed `mplrun.dll` member index for `Mcd.NumVal` has no `Compare` method. See the scope reconciliation in [Mcd.NumVal evidence](../evidence/Mcd.NumVal.md). There is no current member to implement or validate; the earlier checkpoint result does not establish a current blocker.

## Working rules

No IL or decompiled code is committed. Reference output is local in `.mirror-cache/`. `WORK.md` remains untouched. All pending changes are on Lane B worktrees; no remote pushes were made.
