# Quire automation batch 6 (20261127)

- Owner: Claude worker in `C:/Users/DuyDang/orca/workspaces/Quire/quire-automation5-wt`, coordinator `orca_session_id:fdd6235e-0c22-404c-a221-3b766724035d`.
- Branch `feature/quire-automation-batch6-20261127`, from main.

## Landed (non-Plus and Plus, 14 members)

- `AutomationSpiritValuesSetter.SetValues`: `DelegateFunction0` lambda over `secondsArg` now matches. The whole class is in one hand-written partial in Mathcad member order (field, ctor, four Add methods, SetValues, DoSetValues). Its display class is `DisplayClass6_0` in both Mathcad and Quire.
- `AutomationSpiritInputsOutputsStates`: ScalarInputsCount, ScalarOutputsCount, and the four `GetXScalarStateByIndex/ByAlias` methods, in both flavors. The whole class is in one hand-written partial in Mathcad member order.

## Ordinal rule (verified by Release IL)

- Closure ordinal = 0-based position among all class members in Mathcad declaration order, with fields counted.
- In `ValuesSetter`, the field is position 0 and `SetValues` is position 6, which matches `DisplayClass6_0`.
- In `InputsOutputsStates`, the lambdas `<GetInputItem>b__14_0` and `<GetOutputItem>b__15_0` match only when the interface properties (`ScalarInputsCount`, `ScalarOutputsCount`) are declared in the same class before them, in Mathcad order.

## Parked

- Root `CMathcadPrimeApplication` (57) and `CMathcadPrimeWorksheet` (98): structural mismatch (no `Ping` member; constructor with no Mathcad counterpart).
- Remaining non-parked automation members in the AutomationSpirit family are exhausted after this batch.
