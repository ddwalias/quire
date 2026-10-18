# Execution checkpoint

Phase 1 remains open. The Windows phase closure audit has not completed; see [validation workflow](work/validation-flow.md)
for its current blocker. Read the execution rules in [PLAN.md](PLAN.md#execute-load--mirror--checkpoint--merge); use the
lane checkpoints below only for the active batch and its dependencies. Older receipts are historical.

| Lane | Current batch | Checkpoint |
|---|---|---|
| A | Continue the in-progress `MplForeignMacro` batch on `mirror/a-mpl-foreign-macro` at `/mnt/c/Projects/Quire-wt/a-mpl-foreign-macro`; it has uncommitted source, test and evidence changes in the WSL checkout. Do not duplicate or open it with Windows Git. `MplDestructuringBind.ExpandCall` remains blocked by `define_keyword.MakeDefine`; skip it. | [Lane A](work/lane-a.md) |
| B | `FnCompose` is already merged to current WSL `main` at `bf985ce`. The old `mirror/b-fncompose` note is stale. Select the next unowned Engine interface/factory batch after checking the tier inventory and ownership. | [Lane B](work/lane-b.md) |

## Fast lane loop

1. Use one branch and worktree per agent from the shared clone for that OS: Windows agents share `C:\Projects\Quire`;
   WSL agents use the repository at `/root/src/Quire` and its main worktree `/root/src/quire-main`. Create new WSL
   worktrees under `/root/src/worktrees/`. Do not share one checkout or worktree registration between Windows Git and
   WSL Git.
2. Fetch `origin`, then take one coherent class or functional batch (about 15–30 methods). Check ownership and
   dependencies before editing.
3. Run only the affected behavior and differential test classes. The full focused validator, `quick_check.py`, local
   mutation, and the Python harness unit suite are diagnostic/maintainer checks, not routine batch checks.
4. Commit the batch and update its lane checkpoint with scope, evidence, tests, remaining work and blockers. Read the
   complete diff yourself. The merge gate is the pre-merge check; no separate reviewer or user approval is required.
5. Queue at most two ready batches for one final gate. While a gate runs, continue on a fresh branch/worktree only for
   work that does not touch the active batch's files or shared helpers. Never edit the candidate being validated.
6. A gate merges and pushes `main` only after the complete required validation passes. If main moves first, rebase via
   `lane_merge.py`; rerun affected tests only if the rebase changed source/test hunks.

The lane proves any proposed equivalent mutants or unreachable lines; the maintainer updates the protected acceptance
lists, then the gate checks them. See [D14 and D16](decisions.md).

Keep member contracts in [evidence](evidence/README.md). Record only active batch state here and in the lane files;
completed receipts stay in Git history and broader project status is in [STATE.md](STATE.md).
