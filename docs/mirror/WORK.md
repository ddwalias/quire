# Execution checkpoint

Read [PLAN.md](PLAN.md). Phase 1 remains open; Windows phase closure checks have not run.

| Active work | Canonical checkpoint |
|---|---|
| Lane A: runtime helpers and remaining batch members | [Lane A](work/lane-a.md) |
| Lane B: matrix, dimensions, functions and engine | [Lane B](work/lane-b.md) |
| Workflow simplification | [Validation workflow](work/validation-flow.md) |

Next: continue Lane A with the `Primitive` array element operations in [Lane A](work/lane-a.md). Array constructors passed review and the runtime gate, merged to main at `9e5f3e3`, and main was pushed. Numeric conversions merged at `2fe5ed63`; reference-selection helpers at `fbf7627d`; arithmetic helpers at `faf91ae3`; classification predicates at `ca3f68b8`; the five `Primitive` cast helpers at `168be17`. `CodeFunction` merged at `244324a4deae`; `MplFunctionMacro` at `24668bd`; `CodeArg` was rebased after MemberGen merged, kept only its two constructors, passed post-rebase validation and the runtime gate, and is on main. Keep the approved proposals in the Lane A checkpoint until maintainer updates the protected acceptance lists. `let_keyword` remains blocked on `define_keyword` and `lambda_keyword` reaching main. Record each batch's commands, counts, limits and blockers once in its checkpoint. Keep member contracts in [evidence](evidence/README.md) and link run results to the lane checkpoint. Completed checkpoint detail remains in Git history; broader context is in [history.md](history.md) and [STATE.md](STATE.md).
