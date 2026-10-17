# Execution checkpoint

Read [PLAN.md](PLAN.md). Phase 1 remains open; Windows phase closure checks have not run.

| Active work | Canonical checkpoint |
|---|---|
| Lane A: runtime helpers and remaining batch members | [Lane A](work/lane-a.md) |
| Lane B: matrix, dimensions, functions and engine | [Lane B](work/lane-b.md) |
| Workflow simplification | [Validation workflow](work/validation-flow.md) |

Next: implement the independent `MplForeignMacro` members. `MplDestructuringBind.as_pattern` is implemented and locally validated; its `ExpandCall` remains blocked on `define_keyword.MakeDefine`. The DynamicType name-map proposal is user-approved and accepted in its protected unreachable-line entry on the batch branch; the runtime gate is pending. The field-name cache passed its runtime gate and merged at `d8f2e26`; the `Primitive` static initializer and `DynamicType` name helpers are already on main. Bitwise, shift and nth-root helpers passed review and the runtime gate, merged to main at `771dbe07`; main was pushed. Array element operations merged at `307ff708`; array constructors at `9e5f3e3`; numeric conversions at `2fe5ed63`; reference-selection helpers at `fbf7627d`; arithmetic helpers at `faf91ae3`; classification predicates at `ca3f68b8`; the five `Primitive` cast helpers at `168be17`. `CodeFunction` merged at `244324a4deae`; `MplFunctionMacro` at `24668bd`; `CodeArg` was rebased after MemberGen merged, kept only its two constructors, passed post-rebase validation and the runtime gate, and is on main. `Code.NullCode` is on main at `85003e9`; the private `Primitive` constructors are on main at `f4188dd`. Keep the approved proposals in the Lane A checkpoint until maintainer updates the protected acceptance lists. `let_keyword` remains blocked on `define_keyword` and `lambda_keyword` reaching main. Record each batch's commands, counts, limits and blockers once in its checkpoint. Keep member contracts in [evidence](evidence/README.md) and link run results to the lane checkpoint. Completed checkpoint detail remains in Git history; broader context is in [history.md](history.md) and [STATE.md](STATE.md).
