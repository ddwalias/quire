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

- Remaining non-parked automation members in the AutomationSpirit family are exhausted after this batch.

### Root `Quire.CMathcadPrimeApplication` (57): maps to `Spirit.CMathcadPrimeApplication`

- Mathcad declares two constructors: `public .ctor()` and `internal .ctor(bool isUiInteractiveModeDisabled)`. The generated root file (`src/Quire/Generated/Mirror/CMathcadPrimeApplication.g.cs`) declares only `public CMathcadPrimeApplication()`. The `(bool)` constructor has no Quire counterpart, so it cannot be implemented in the root partial.
- The Mathcad root has no `Ping` member, in the members table or the decompiled source. The Ping handling (`_isSubscribedPing`, `_isPinging`, `OnPingClient`) lives in `src/Quire/Mirrored/AutomationServiceEngNote.CMathcadPrimeApplication.cs`, which mirrors the non-root `Spirit.AutomationServiceEngNote.CMathcadPrimeApplication`. The root partial has no such code to match.
- No hand-written root partial exists (`src/Quire/Mirrored/CMathcadPrimeApplication.cs` is absent), so its generated stubs (106 `MirrorStub` lines, attributes included) remain stubs.

### Root `Quire.CMathcadPrimeWorksheet` (98): maps to `Spirit.CMathcadPrimeWorksheet`

- Mathcad's constructor is `public .ctor(IEngineeringDocument, bool isUiInteractiveModeDisabled)`. The generated root file declares the same two-parameter signature, but there is no hand-written root partial to implement it.
- The hand-written counterpart (`src/Quire/Mirrored/AutomationServiceEngNote.CMathcadPrimeWorksheet.cs` and the Plus variant) declares a three-parameter constructor with an extra `bool isComClientControlled = false`. That signature does not exist in the Mathcad root, so the non-root partial cannot be reused for the root type.
- The nested `WorksheetData` type is mirrored under the root (`Spirit.CMathcadPrimeWorksheet+WorksheetData`, generated at `CMathcadPrimeWorksheet.g.cs:200`). Its implementation exists only in the non-root partial, so the root mapping does not reach it.
- The root generated file still has 191 `MirrorStub` lines, attributes included.

Mapping decision for the MirrorGen side (maintainer): the root Quire types need their own mapping, or the non-root partials need a documented policy for the root `Spirit.*` types. Until then both root types stay parked.
