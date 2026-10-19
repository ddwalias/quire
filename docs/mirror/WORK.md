# Execution checkpoint

Phase 1 remains open. The Windows phase closure audit has not completed; see [validation workflow](work/validation-flow.md)
for its current blocker. Read the execution rules in [PLAN.md](PLAN.md#execute-load--mirror--checkpoint--merge); use the
selected lane checkpoint's **Active handoff** section only, plus the entries needed to verify its active batch and
dependencies. Do not load dated historical receipts as agent context.

| Lane | Current batch | Checkpoint |
|---|---|---|
| A | SCons, SList, Utilities.GetUnits, Tok, MplSignature and NumVal are merged. Five Lane A batches still need full gate revalidation; no unowned Lane A Tier A file scope remains in the current inventory. See receipts and ownership in the checkpoint. | [Lane A](work/lane-a.md) |
| B | `FnCompose` is already merged to `main` at `bf985ce`; the old `mirror/b-fncompose` note is stale. Select the next unowned Engine interface/factory batch after checking the tier inventory and ownership. | [Lane B](work/lane-b.md) |
| C | Lane C owns Tier A mirroring for `Ptc.Calculation.dll` and `Ptc.Calculation.Core.dll`. `c-calc-engine-error-placeholder` passed current-worktree Release Mathcad tests (1/1) and exact Release IL audit; normal gate pending on current main `10deda91`. TraceDataPoints has focused tests and exact IL but phase validation is blocked by unrelated full-suite failures; vectors-legacy has a Release test-host crash before its class ran. Exact reports are in lane-c.md. | [Lane C](work/lane-c.md) |

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
