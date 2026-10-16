# Execution checkpoint

Read [PLAN.md](PLAN.md). Phase 1 remains open; Windows phase closure checks have not run.

| Active work | Canonical checkpoint |
|---|---|
| Lane A: runtime helpers and remaining batch members | [Lane A](work/lane-a.md) |
| Lane B: matrix, dimensions, functions and engine | [Lane B](work/lane-b.md) |
| Workflow simplification | [Validation workflow](work/validation-flow.md) |

Next: keep advancing independent Lane A and Lane B batches in their checkpoints while reviewed proposals await maintainer acceptance. Lane A's `MplFunctionMacro` batch merged at `24668bd`; `mirror/a-codearg` is rebased and ready for review. `let_keyword` is blocked on the unmerged `define_keyword` and `lambda_keyword` implementations. Record each batch's commands, counts, limits and blockers once in its checkpoint. Keep member contracts in [evidence](evidence/README.md) and link run results to the lane checkpoint. Completed checkpoint detail remains in Git history; broader context is in [history.md](history.md) and [STATE.md](STATE.md).
