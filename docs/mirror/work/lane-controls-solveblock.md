# Lane Controls SolveBlock

- Owner: SolveBlock lane; branch: `mirror/solveblock-20261108`.
- Batches 1–5 passed/landed; batch 5 `9481c27a`.
- Batch 6 passthroughs: PASS (`36 checked`); initial MSB4166, retry PASS; landed `e5f6a6e4`.
- Batch 7 `WrappedCalculatable`: PASS (`37 checked`); landed `64b35a02`.
- Batch 8 constructor: PASS (`38 checked`); landed `f1756f18`.
- Batch 9 simple equation update: PASS (`39 checked`); landed `1a947618`.
- Batch 10 update override: PASS (`40 checked`); landed `adcea50f`.
- Batch 11 field: PASS (`40 checked`); landed `47c1dc7e`.
- Batch 12 notifier: PASS (`41 checked`); landed `91575596`.
- Batch 13 evaluation request: PASS (`42 checked`); landed `1ab5bfb`.
- Batch 14 error ownership: PASS (`44 checked`); landed `c9460106`.
- Batch 15 restore result: PASS (`45 checked`); landed `bfc84597`.
- Batch 16 equation property: PASS (`47 checked`); landed `9e93b697`.
- Batch 17 aggregate updater: PASS (`49 checked`); landed `aa6074cc`.
- Batch 18 aggregate lifecycle: PASS (`52 checked`); landed `c7f265ad`.
- Batch 19 aggregate properties: PASS (`54 checked`); landed `c4bf799e`.
- Batch 20 deserialization strategy blocked by mapped type mismatch (`release-il-e1aad10a62c3`).
- Batch 21 child wrapper error: PASS (`56 checked`); landed `6480c502`.
- Batch 22 child wrapper status: PASS (`59 checked`); landed `6e60fc2b`.
- Batch 23 child wrapper format and constructor: PASS (`60 checked`; `.mirror-cache/release-il-6cfd2635c8f4`).
- Batch 24 `HideableSolverAggregate`: PASS (`65 checked`; `.mirror-cache/release-il-46127dde98e9`); landed `4e856283` (`Mirror HideableSolverAggregate`). Initial setter conditional had inverted branch layout; a negative type-pattern guard with early return matched the reference.
- Batch 25 `HideableSolveBlockCalculatable`: PASS (`74 checked`; `.mirror-cache/release-il-05e5e4f024de`); landed `0482bcd6`. Initial setter `if` emitted a branch to the return instead of the update body; the explicit branch shape matched the reference.
- Batch 26 `HideableSolveBlockCalculatableAggregate`: PASS (`83 checked`; `.mirror-cache/release-il-2e1e5969feb2`); landed `08d1aab9`. Reused the setter branch shape required by the reference.
- Batch 27 `SolveBlockSerializationStrategy`: PASS (`96 checked`; `.mirror-cache/release-il-883c363870e9`); landed `6c54eaff`.
- Batch 28 `SolverAggregateChildWrapper.Rl`: PASS (`97 checked`; `.mirror-cache/release-il-5faff5a2879d`); landed `8d2d79e0`.
- Batch 29 `SolveBlockViewModel`: PASS (`106 checked`; `.mirror-cache/release-il-05a67683ef31`); landed `5a797c91`. The constructor uses direct grid-size expressions to preserve operand order, and the preview handler keeps its work in the non-null branch to match Release IL.
- Batch 30 `SolveBlockCalculatableAggregate.GetSymbolicEvalExpression`: PASS (`107 checked`; `.mirror-cache/release-il-f6b964389fe0`); landed `daa08f8a`. Candidate `GetDefinedVariable` differed because Roslyn emitted `call` where Mathcad emitted `callvirt`; it remains stubbed.
- Batch 31 aggregate event forwarding handlers: PASS (`110 checked`; `.mirror-cache/release-il-c16aec751984`); landed `cb356540`.
- Batch 32 `SolverAggregateChildWrapperAggregate` constructor and child properties: PASS (`113 checked`; `.mirror-cache/release-il-801b248be88f`); landed `df3f6eff`.
- Batch 33 `SolveBlockChildManager` constructor and core child forwarding methods: PASS (`116 checked`; `.mirror-cache/release-il-f0548100770c`); landed `ccce1d80`.
- Batch 34 `SolverAggregateChildWrapperAggregate` events: PASS (`116 checked`; `.mirror-cache/release-il-653f04a8db56`); landed `7673d853` (checkpoint correction landed `4ba6e731`).
- Batch 35 `SolverAggregate` constructor, child list state, `LastChild` and undefined-subscript predicate: PASS (`119 checked`; `.mirror-cache/release-il-75986ce1838a`); landed `7aea21fb`.
- Batch 36 `SolverAggregate` parsed-RL auto-properties: PASS (`133 checked`; `.mirror-cache/release-il-5184cf1aa8c5`); landed `f0fbc4c6`.
- Batch 37 `SolverAggregate` child mutation methods: PASS (`138 checked`; `.mirror-cache/release-il-1e314b488ba6`); landed `31364399`. Initial `RemoveChild` conditional reversed the branch layout; an explicit branch to the removal body matched Release IL.
- Batch 38 `SolverAggregate` child event forwarding: PASS (`141 checked`; `.mirror-cache/release-il-934544828a00`); landed `af0afa75`.
- Batch 39 `SolverAggregate.Rl` and `CalculatableChildren`: PASS (`143 checked`; `.mirror-cache/release-il-b23c4da260a1`); landed `5c1e261f`.
- Batch 40 `SolverAggregate` algorithm tables and constants: PASS (`144 checked`; `.mirror-cache/release-il-2b0e013795b7`); landed `5175aaf2`.
- Batch 41 `SolverAggregate` equation-type updates: PASS (`146 checked`; `.mirror-cache/release-il-9c898d8c699d`); landed `c90271e4`.
- Batch 42 `SolverAggregate.AddAlgorithmIfRequired`: PASS (`147 checked`; `.mirror-cache/release-il-4b809f515a69`); landed `e7985d0c`.
- Batch 43 `SolverAggregate.SetError`: PASS (`148 checked`; `.mirror-cache/release-il-2d8ddf037f16`); landed `1f61cacfe`.
- Batch 44 `SolverAggregate.SetResult`: PASS (`149 checked`; `.mirror-cache/release-il-cdaf32494cf4`); landed `88a76ac6`.
- Batch 45 `SolveBlockModel` add/remove item solver refresh: PASS (`151 checked`; `.mirror-cache/release-il-80b2ae7d189d`); landed `2d5b4fd3`.
- Batch 46 `SolveBlockModel.Clone`: PASS (`152 checked`; `.mirror-cache/release-il-0c7a3db54fcd`); landed `06fc4504`.
- Batch 47 `SolveBlockModel.GetWrappedCalculatable`: PASS (`153 checked`; `.mirror-cache/release-il-219c7a052aea`); landed `1c4950bc`.
- Batch 48 `SolveBlockModel` section constants and metadata: PASS (`157 checked`; `.mirror-cache/release-il-7ac5fb483945`); landed `8891da66`. The switch expression shape differed; per-case returns matched Release IL.
- Batch 49 `SolveBlockModel.IsSolverAggregateChildType` overloads and equation lookup: PASS (`160 checked`; `.mirror-cache/release-il-8d5c71dac741`); landed `74afcb07`.
- Batch 50 `ISolverAggregate` abstract declarations: PASS (`172 checked`; `.mirror-cache/release-il-9da6a44700b9`); landed `d1b4cacb`.
- Batch 51 `ISolveBlockModel`, `ISolveBlockControl` and `ISolveBlockCalculatable` abstract declarations: PASS (`173 checked`; `.mirror-cache/release-il-a0b3bebc0cb2`); landed `9048cbd2`.
- Batch 52 `SolveBlockModel.GetBlockEquationTypeAttributeName`: PASS (`161 checked`; `.mirror-cache/release-il-26226c91b272`); landed `bc4faf17`.
- Batch 53 `SolveBlockModel.InitializeChildManager`: PASS (`162 checked`; `.mirror-cache/release-il-0213f662ee69`); landed `1217a000`. `MatchEquationType` candidate local-slot order differed and remains stubbed.
- Batch 54 `SolverAggregate` events: PASS (`162 checked`; `.mirror-cache/release-il-3bcd30fe0f33`); landed `5f9ef9db`.
- Batch 55 `SolveBlockModel` map/solver state, direct properties and constructor: PASS (`173 checked`; `.mirror-cache/release-il-5976c47faf3a`); landed `344d5100`.
- Batch 56 `SolveBlockModel.GetWhiteboardItemThatOwnsCalculatable`: PASS (`174 checked`; `.mirror-cache/release-il-086fcbe8e404`); landed `4d58fd65`.
- Batch 57 `SolveBlockCalculatableAggregate` event storage and constructor: PASS (`176 checked`; `.mirror-cache/release-il-8846e7c796a7`); landed `1088313f`.
- Batch 58 `SolveBlockCalculatableAggregate.IsRlForSymbolicEval`: PASS (`177 checked`; `.mirror-cache/release-il-3d4aad107e7a`); landed `006e39a0`.
- Batch 59 `SolveBlockCalculatableAggregate.ComposeSolveblockChildRlFromEquationControl`: PASS (`178 checked`; `.mirror-cache/release-il-34584a0a16a5`); landed `760065ea`.
- Batch 60 `SolveBlockModel` remove/lookup wrapper methods: PASS (`180 checked`; `.mirror-cache/release-il-db71addd6deb`); landed `c5e24cf9`.
- Batch 61 `SolveBlockModel.UnitTest_GetSolveBlockCalculatableWrapper`: PASS (`181 checked`; `.mirror-cache/release-il-9c9680d4620e`); landed `242baa57`.
- Batch 62 model child/event routing and result-definition cleanup: PASS (`187 checked`; `.mirror-cache/release-il-585d8f2523bf`); landed `452cf9b6`. The initial `SolverIsNotInADefinition` conditional had the opposite branch layout; a guarded branch to cleanup matched Release IL.
- Batch 63 `SolveBlockModel` calculator events: PASS (`190 checked`; `.mirror-cache/release-il-7e05cb9c9a7d`). Initial addition guard had opposite branch direction; a direct branch to the addition body matched Release IL.
- `GetEquationTypeOfRl` remains stubbed due enum/closure mismatch.
- Remaining: 86 stub markers in generated types; known blockers: `GetDefinedVariable` call vs. callvirt mismatch, `GetEquationTypeOfRl` enum/closure mismatch, `MatchEquationType` local-slot order mismatch, deserialization strategy mapped type mismatch, `SolveBlockChildManager.Rewrap` anonymous helper without a Mathcad counterpart and `SolverAggregate.Children` closure ordinal mismatch.
## Remaining stub classification (2026-11-10)

The 51 generated stub markers are classified as follows; “unblocked” means no checkpointed IL/ownership obstacle or recorded closure-ordinal failure is known for that member.

- Checkpointed blockers (10 markers): `SolveBlockCalculatable.GetEquationTypeOfRl`; `SolveBlockCalculatableAggregate.GetDefinedVariable`; `SolveBlockChildManager.Rewrap`; all four `SolveBlockDeserializationStrategy` markers (mapped-type mismatch); `SolveBlockModel.MatchEquationType`.
- Closure-ordinal issues (13 markers): `SolverAggregate.Children` (recorded closure ordinal mismatch) `SolverAggregate.ClearError` (candidate lambda ordinal `b__88_0` vs. Mathcad `b__73_0`) and the six `SolveBlockControl` indicator properties (`GuessValuesIndicatorTop/Bottom`, `ConstraintsIndicatorTop/Bottom`, `SolverIndicatorTop/Bottom`), whose prior candidate shifted `get_EngineResults` closure from `b__62_0` to `b__50_0`.
- Unblocked (28 markers): all remaining markers in `SolveBlockCalculatable`, `SolveBlockCalculatableAggregate`, `SolveBlockControl`, `SolveBlockModel`, and `SolverAggregate`, excluding the members listed in the two categories above. This includes fields/constants, constructors, simple accessors, overrides, and methods; each is eligible for a batch after its Release IL is read. No new closure blocker has yet been inferred for these members.

No classification changes the generated files; this inventory records the current state before further implementation.
- Batch 64 `SolveBlockModel.UpdateTheSolverIfNeededToReflectMovedCalculatable`: PASS (`191 checked`; `.mirror-cache/release-il-281b1938a523`). The explicit return after `MirrorChildInSolverAggregate` preserves the IL branch target followed by a separate method return.
- Batch 65 `SolverAggregate.SetSynchronizationStatus` and `SetCalculatingStatus`: PASS (`191 checked`; `.mirror-cache/release-il-209bb1d0ae6e`). The first gate attempt failed compilation because the partial declaration used public accessibility; matching the generated internal declaration fixed it.

## Per-member classification ledger

### Controls.SolveBlockCalculatable
- `PropertyChanged` — unblocked
- `GetEquationTypeOfRl` — checkpointed blocker

### Controls.SolveBlockCalculatableAggregate
- `GetDefinedVariable` — checkpointed blocker
- `ComposeRlForSymEval` — unblocked
- `ComposeRlForEvalOrDefineEval` — unblocked

### Controls.SolveBlockChildManager
- `Rewrap` — checkpointed blocker

### Controls.SolveBlockControl
- `_sbModel` — unblocked
- `GuessSectionIndex` — unblocked
- `ConstraintSectionIndex` — unblocked
- `SolverSectionIndex` — unblocked
- `_engineResults` — unblocked
- `IsSolveBlockLabelVisible` — unblocked
- `GuessValues` — unblocked
- `Constraints` — unblocked
- `Solver` — unblocked
- `GuessValuesIndicatorTop` — closure-ordinal issue
- `GuessValuesIndicatorBottom` — closure-ordinal issue
- `ConstraintsIndicatorTop` — closure-ordinal issue
- `ConstraintsIndicatorBottom` — closure-ordinal issue
- `SolverIndicatorTop` — closure-ordinal issue
- `SolverIndicatorBottom` — closure-ordinal issue
- `ErrorCount` — unblocked
- `Error` — closure-ordinal issue (`b__56_0` vs Mathcad `b__52_0`; candidate reverted)
- `ResultOwners` — unblocked
- `SolveBlockControl` — closure-ordinal issue (constructor lambda target ordinal; candidate `b__26_0` vs Mathcad `b__17_0`)
- `SolveBlockControl` — closure-ordinal issue (constructor lambda target ordinal; candidate `b__26_0` vs Mathcad `b__17_0`)
- `UpdateGridCellSizeIfNeeded` — unblocked
- `SolveBlockControl` — closure-ordinal issue (constructor lambda target ordinal; candidate `b__26_0` vs Mathcad `b__17_0`)
- `Clone` — unblocked
- `Clone` — unblocked
- `HookupEvents` — unblocked
- `HandleChildCalcErrorChanged` — unblocked
- `SetRegionPersistentData` — checkpointed blocker (type-map: `Ptc.PersistentDataObjects.solveBlockType` is missing)
- `GetRegionPersistentData` — checkpointed blocker (type-map: `Ptc.PersistentDataObjects.solveBlockType` is missing)
- `GetSolveBlockEquationType` — unblocked
- `GetBlockEquationTypeAttributeName` — unblocked
- `GetInternalHelpContext` — unblocked
- `ReplaceCalculatablesWithWrappersInResultManager` — unblocked
- `ReplaceCalculatablesAndResultWithWrappersInResultManager` — unblocked
- `EndInit` — unblocked

### Controls.SolveBlockDeserializationStrategy
- `_control` — checkpointed blocker (mapped type)
- `RegionFactory` — checkpointed blocker (mapped type)
- `SolveBlockDeserializationStrategy` — checkpointed blocker (mapped type)
- `DeserializeRegionsEpilog` — checkpointed blocker (mapped type)

### Controls.SolveBlockModel
- `RlFormat` — unblocked
- `RlOdeFormat` — unblocked
- `SolverResultIdName` — unblocked
- `GuessValueItems` — unblocked
- `ConstraintItems` — unblocked
- `CalculatableChildren` — unblocked
- `ChildErrorCountTotal` — unblocked
- `Rl` — unblocked
- `CalculatableChildrenAboveSolver` — unblocked
- `ChildCalcErrorChanged` — unblocked
- `MoveItems` — unblocked
- `ListenForErrorChanges` — unblocked
- `StopListeningForErrorChanges` — unblocked
- `HandleEquationControlCommitted` — unblocked
- `HandleWrappedCalcPropertyChanged` — unblocked
- `FireChildCalcErrorChanged` — unblocked
- `UpdateWhichItemIsSolver` — unblocked
- `MatchEquationType` — checkpointed blocker
- `TheSolverMoved` — unblocked
- `FindSolverChildAfterModelChild` — unblocked
- `GetCalculatableToAddToSolverAggregate` — unblocked
- `CreateCalculatableWrapper` — unblocked
- `IsChildAboveSolver` — unblocked
- `HandleChildWrapperPropertyChanged` — unblocked
- `HandleSolverRequestEvaluation` — unblocked

### Controls.SolverAggregate
- `Children` — closure-ordinal issue
- `UpdateSolverRL` — unblocked
- `GetSolvingAlgorithmRl` — unblocked
- `IsSolverOfSymbolicContainsFind` — unblocked
- `GetSolverWrappedAggregate` — closure-ordinal issue (candidate lambda `b__75_0` vs. Mathcad `b__56_0`; reverted)
- `HandleRequestEvaluation` — unblocked
- `IsChild` — unblocked
- `GetIndexOfChild` — unblocked
- `CommitAllChildren` — unblocked
- `GetChildIndexForResult` — unblocked
- `ClearError` — closure-ordinal issue (`b__88_0` vs Mathcad `b__73_0`; candidate reverted)
- `ParseWrappedRl` — unblocked
- `ContainsError` — unblocked
- `ShowError` — unblocked


- Rejected candidate: `SolverAggregate.ClearError` used the expected `Each` lambda shape but failed Release IL at `b__88_0` versus Mathcad `b__73_0`; its source and generated-stub changes were reverted, and it is now classified as a closure-ordinal issue.
- Batch 66 `SolveBlockModel.HandleSolverRequestEvaluation`: PASS (`192 checked`; `.mirror-cache/release-il-4625aac2c439`). Rejected first candidate spelling `SimpleCalculatable` under `Quire.Calculation.Core` (actual type is under `.Support`); the next gate rejected interpolated-string IL, so `String.Format("(define {0} {1})", ...)` was used to match the literal format call.

- Batch 67 `SolveBlockModel` error-change subscriptions and equation commit handler: PASS (`195 checked`; `.mirror-cache/release-il-91501b23eca3`). The initial property-pattern form differed in local-slot order; separate negative-pattern guards fixed both subscriptions. The handler's nested `if` did not match its branch distance, so a direct goto to the removal body with a shared early return was required by Release IL.

- Batch 68 `SolveBlockControl.GetSolveBlockEquationType`, `GetBlockEquationTypeAttributeName`, and `GetInternalHelpContext`: PASS (`198 checked`; `.mirror-cache/release-il-ddd467f3afde`). The first candidate file placed these members after `EngineResults`, shifting its closure from `b__62_0` to `b__59_0`; moving the members before `EngineResults` in the existing control partial restored the required ordinal.


- Batch 69 `SolveBlockControl.ErrorCount` and `ResultOwners`: PASS (`200 checked`; `.mirror-cache/release-il-b09e1dc4902c`). A paired `Error` getter candidate failed because its compiler closure was `b__56_0` instead of Mathcad's `b__52_0`; it was reverted and classified as a closure-ordinal issue.

- Batch 70 `SolveBlockControl.ReplaceCalculatablesWithWrappersInResultManager` and `ReplaceCalculatablesAndResultWithWrappersInResultManager`: PASS (`202 checked`; `.mirror-cache/release-il-b4aead464619`). Implemented the two direct loops over `_sbModel.AllCalculatables` and `ResultManager.Instance`.

- Batch 71 `SolveBlockControl.IsSolveBlockLabelVisible`, `GuessValues`, `Constraints`, and `Solver`: PASS (`207 checked`; `.mirror-cache/release-il-e51e08be9018`). These accessors were placed before `EngineResults` to preserve its existing closure ordinal.

- Batch 72 `SolveBlockControl.HookupEvents` and `HandleChildCalcErrorChanged`: PASS (`209 checked`; `.mirror-cache/release-il-3c884664a18c`).

- Batch 73 `SolveBlockControl.EndInit`: PASS (`210 checked`; `.mirror-cache/release-il-7b3f3bd43163`).

- Batch 74 `SolveBlockControl.UpdateGridCellSizeIfNeeded`: PASS (`211 checked`; `.mirror-cache/release-il-3092e075c6bf`).


- Rejected candidate: both public `SolveBlockControl` constructors were restored after gate `release-il-8be1c23d43e7` failed at the parameterized constructor closure name `b__26_0` vs. Mathcad `b__17_0`; constructor members are now classified as closure-ordinal issues.
- Batch 75 `SolveBlockCalculatable.PropertyChanged`: PASS (`211 checked`; `.mirror-cache/release-il-d5f58f177005`).

- Batch 76 `SolveBlockModel.HandleWrappedCalcPropertyChanged` and `FireChildCalcErrorChanged`: PASS (`213 checked`; `.mirror-cache/release-il-ef003c9dd4df`). The initial positive branch emitted the error call before the test; the inverted branch then reversed the two calls. Direct gotos to ordered error and committed bodies reproduced the reference branch layout.


- Rejected candidate: `SolverAggregate.GetSolverWrappedAggregate` gate failed at `b__75_0` vs. Mathcad `b__56_0` (`release-il-e74a3459abeb`, 215 checked); the method implementation was reverted and classified as a closure-ordinal issue.
- Batch 77 `SolverAggregate.GetChildIndexForResult`: PASS (`214 checked`; `.mirror-cache/release-il-2c6434f5abbb`). Direct construction followed by `.Match` emitted `call`; a per-iteration local `Regex` is required for Mathcad's `callvirt` shape.

- Batch 78 `SolverAggregate.IsSolverOfSymbolicContainsFind`: PASS (`215 checked`; `.mirror-cache/release-il-fe1d79ef4f06`). The two symbolic operator regex checks and case-insensitive `find` expression test match Release IL.

- Batch 79 `SolveBlockControl.ICloneable.Clone` and `Clone`: PASS (`217 checked`; `.mirror-cache/release-il-26d7154e62b3`). The public clone method constructs the private clone overload; that constructor remains a closure-ordinal blocker.

- Batch 80 `SolveBlockModel.HandleChildWrapperPropertyChanged`: PASS (`218 checked`; `.mirror-cache/release-il-840d4d998ee0`). The initial negative guard emitted `brtrue` to the return; a positive `if (PropertyName == "EquationType")` body matched the reference's branch to the trailing return.

- Batch 81 `SolveBlockModel.CalculatableChildren` and `MoveItems`: PASS (`218 checked`; `.mirror-cache/release-il-4b7d8f50894e`). Both overrides forward directly to the base implementation.

- Batch 82 `SolveBlockModel.ChildCalcErrorChanged`: PASS (`218 checked`; `.mirror-cache/release-il-9a95e14ed40a`). The field-like event declaration was already present and used by mirrored methods; cleared only its stub marker.

- Batch 83 `SolveBlockModel.Rl`: PASS (`219 checked`; `.mirror-cache/release-il-b1e551912dce`). The getter returns `nil` without a solver definition and formats the defined RL with the result ID.

- Batch 84 `SolveBlockCalculatableAggregate.ComposeRlForEvalOrDefineEval`: PASS (`220 checked`; `.mirror-cache/release-il-34791708`). The method validates the four RL entries, handles `define-eval`/`eval`, and appends the optional `ui_scale` expression.

