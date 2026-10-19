# Execution checkpoint

Phase 1 remains open. The Windows phase closure audit has not completed; see [validation workflow](work/validation-flow.md)
for its current blocker. Read the execution rules in [PLAN.md](PLAN.md#execute-load--mirror--checkpoint--merge); use the
selected lane checkpoint's **Active handoff** section only, plus the entries needed to verify its active batch and
dependencies. Do not load dated historical receipts as agent context.

| Lane | Current batch | Checkpoint |
|---|---|---|
| A | `mirror/a-scons-il-shape` merged at `b83cc5d`; `mirror/a-slist-format-cons` is active on the next unowned Tier A member. `MplForeignMacro` and the remaining AppException, DynamicType and InstructionSeq scopes have other owners. | [Lane A](work/lane-a.md) |
| B | `FnCompose` is already merged to `main` at `bf985ce`; the old `mirror/b-fncompose` note is stale. Select the next unowned Engine interface/factory batch after checking the tier inventory and ownership. | [Lane B](work/lane-b.md) |
| C | Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. `ResultNodeUtils.TryParsePlottableValue` passed current-worktree Release Mathcad cases (1/1, five inputs) and its Release IL audit is exact. Normal gate pending on current main `3ed37be1`. `ParsePlottableValue` remains blocked by out-of-scope `DebugTools.DevAssert`. Other C gates remain phase/test-host blocked; see lane-c.md. | [Lane C](work/lane-c.md) |

## Fast lane loop

1. Use native Windows Git. The shared clone is `C:\Projects\Quire`; create a separate branch and worktree per agent
   under `C:\Projects\Quire-wt\<branch>`. Never share a checkout or worktree between agents.
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
