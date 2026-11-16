# Calculation.Core wave 2: checkpoint

- Owner: Claude worker, dispatch task_1262888faeba (coordinator `orca_session_id:claude_32e98c9e_919d_4823_b7ed_5529f2b64c32`).
- Branch `mirror/m-calc-core-wave2-claude-20261117`, from main.
- Scope: deleted the 187 members with no Mathcad counterpart in `Quire.Calculation.Core` (18 parked stubs, 169 product-only members). These were the product result records, their Quire-only interface members, and `EngineError(string, string)` with `Symbol`.
- Callers changed in the same change: `ResultFormatter` (EquationEditor) now formats the mirrored `IUnitedValueResultNode`, `IMatrixResultNode`, `IRealResultNode`, `IStringResultNode`, and `IFunctionResultNode`; `ValueToIResultNodeTransform.Transform` (Quire.Calculation, product-only, used only by a test) removed; 11 `IResultNode.ValueType` implementations and 5 `IMatrixResultNode` stub sets in Quire.EquationEditor removed; `TransformTests` lost one end-to-end test that depended on the removed product transform and one unit-helper test.
- Generated `.g.cs` stubs removed for every deleted member, as regeneration would.

## Release IL (`release_il.py --base main`)

| Project | Result |
|---|---|
| Quire.Calculation.Core `--all` | PASS: 824 checked, 574 exact, 0 failures, 0 missing, 0 unfinished (was FAIL: 187 product-only failures) |
| Quire.Calculation | PASS: 102 checked, 102 exact, 0 failures |
| Quire.EquationEditor | PASS: 911 checked, 909 exact, 0 failures |
| Quire.Controls.Core | PASS: 3228 checked, 3228 exact, 0 failures |
| Quire.Controls.Plots | PASS: 861 checked, 861 exact, 0 failures |
| Quire.Controls.ExcelComponent | PASS: 558 checked, 558 exact, 0 failures |

## Not verified

- `tests/Quire.EquationEditor.Tests` does not build in this environment: `dotnet build` stops at MSB4018 (SDK workload-resolution exception in ProcessFrameworkReferences) before compiling. Its edits were checked by reading only.

## Remaining

- Stubs in Quire.Calculation.Core: 0. Product-only members: 0.
- Behavior dropped by the deletion: `EngineError.Symbol` (the editor's highlighted symbol) and the product display path in `ValueToIResultNodeTransform`. Mathcad's mirrored path (`IValueToIResultNodeTransform.Transform`) is the one remaining.
