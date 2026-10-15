# Lane B checkpoint

Updated: 2026-10-15

## Merged to main

- Function / FunctionUtilities / function matrix bridge (`mirror/b-function`, `a6ca35c0845c`). The merge gate passed after the VecCall and AsDelegate cases were sharpened: Harness 27, Mpl 159, Differential 74; 92/92 mutants killed; lint 0; ratchet passed.
- Engine interfaces (`IEngine`, `IFactory`, `NodeCallback`, `NodeStatus`; `mirror/b-engine`, `3924a7d091a3`). Focused validation passed: Harness 27, Mpl 159, Differential 74; lint 0; ratchet passed. The gate used the documented `--allow-unvalidated` option after the Windows phase exposed an unchanged Lane A shape mismatch, independently reviewed as pre-existing.
- MatVal collection/growth (`mirror/b-matval-collections`, `dfb65f3cd776`). The merge gate passed with Harness 27, Mpl 159, Differential 74, 60/60 mutants killed, lint 0, and ratchet passed. Main advanced to `e2532e3eacb1`.
- MatVal views/export (`mirror/b-matval-view`, `e168ce6f7b4e`). The merge gate passed with Harness 27, Mpl 159, Differential 77, 158/158 mutants killed, lint 0, and ratchet passed. Main advanced to `459eb2362f58`; nine documented structure mismatches remain recorded in the evidence as reconstruction limits.

## MatVal core

- Worktree: `/mnt/c/Projects/Quire-wt/b-matval-core`, branch `mirror/b-matval-core`, rebased onto current main `459eb23`; fresh focused validation and the merge gate remain pending.
- Added direct comparisons and sharper boundary cases for `Fill`, `Shared`, `Isize`, and `ColOf`; evidence records were aligned to the current format. The post-rebase focused run is `/tmp/quire-b-matval-core-after-rebase`: Harness 27/27, Mpl 159/159, Differential 73/73, no failures or skips, lint 0, ratchet passed. The audit reports 354 compared, 165 declaration-only, 100 excluded, 1,276 missing, 166 structure mismatches, 27 uncovered, and 355 verified unsupported; the run is incomplete because the wider mirror audit has outstanding methods.
- The branch has one equivalent mutant in `MatVal.get_Shared`: changing `(m_flags & 4) > 0` to `> 1` cannot change the result because the masked value is only 0 or 4. Other previously surviving killable MatVal mutants have direct cases. The maintainer approved this entry and the three previously confirmed equivalent MatDim mutations; their exact entries are committed in `accepted-mutants.txt`. The merge gate rejects protected-file changes unless `--allow-protected` is supplied, which the lane instructions prohibit; this branch cannot merge in its current form through the authorized gate. Final rebased gates remain pending.

## Function follow-up

- Worktree: `/mnt/c/Projects/Quire-wt/b-function-followup`, branch `mirror/b-function-followup`, based on current main `459eb23`. Added direct comparisons for `Function.get_IsFunction` and `Function.InvokeVec`, and classified previously unclassified Function fields/accessors. Focused validation `/mnt/c/Projects/Quire-wt/b-function-followup/scratch/function-followup-459-r4` passed Harness 27/27, Mpl 159/159, and the selected Differential tests 5/5; lint 0, ratchet ok, and all 28 compared methods in the selected types are covered. The audit remains incomplete for unrelated Function methods (137 missing, 7 structure mismatches). `FunctionUtilities`' protected constructor remains Analyzed because reflection cannot directly invoke a constructor on an abstract type; this limit is recorded in its evidence. Merge gate pending.

## Other Lane B worktrees

- MatVal collection/growth is merged. Its `Fill` case distinguishes multiplication from division (2x1 matrix, stride 2, filling indices 0/4/8). Focused validation is recorded at `/tmp/quire-matval-collections-fill-validation-20261015`; the passing gate with 60/60 mutants killed is at `/tmp/quire-gate-results-i8ny5j9s/run`. The worktree and branch were removed after merge.
- MatDim and MatVal dimension/index: commits `0d5c824` and `7e3bcb0` on `/mnt/c/Projects/Quire-wt/b-matval-dimensions`, branch `mirror/b-matval-dimensions`; rebased onto current main `459eb23`. The previous focused run exited 3: Harness 27/27, Mpl 159/159, Differential 71/71, lint 0, ratchet passed. All assigned MatDim 49 and MatVal 39 tokens have complete line/branch coverage; the six structure mismatches per evidence record are documented. The latest completed gate `/tmp/quire-gate-results-9aoo69qr/run` killed 188/191 mutants. The three remaining mutations were approved as equivalent and recorded on the MatVal core branch; fresh validation and gate remain pending. An experimental subtype case could distinguish `Pwr` but was removed because Reflection.Emit triggers forbidden test API lint. The focused run raised 27 uncovered methods outside this assigned set.
- `DimVal.ApplyMatVal`: commit `5ba83a5` on `/mnt/c/Projects/Quire-wt/b-dimval-apply`, branch `mirror/b-dimval-apply`. Added direct cases for row-major 2x2 argument packing and scalar conversion failure. The Quire.Mpl build is blocked by 53 compile errors in Lane A helper files/symbols (`MplEnv`, `Keyword`, `Rep`, and `F` generic arity); no ApplyMatVal errors were reported. The record remains Analyzed pending a successful focused run. No Lane A files were edited.

## Reviewed out-of-scope blocker

The Windows Phase 1 run of the Engine branch passed Harness 27, Mpl 159, Differential 75, Engine 94, Calculation 8, EquationEditor 88, Core 7, and Controls 16. Mirror.Tests had 31/32 pass; the sole failure expected `Mcd.NumVal.Compare`, which the manifest lists but the unchanged Lane A source and generated file do not define. The `Quire.Mcd.NumVal` source, generated file, manifest, and shape test were identical on main and the validated Engine revision. An independent reviewer confirmed this is pre-existing and outside Lane B; it remains for Lane A.

## Working rules

No IL or decompiled code is committed. Reference output is local in `.mirror-cache/`. `WORK.md` remains untouched. All pending changes are on Lane B worktrees; no remote pushes were made.
