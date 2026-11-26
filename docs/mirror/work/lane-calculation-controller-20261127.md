# Quire.Calculation CalculationController checkpoint (calc-controller-wt)

- Owner: Claude worker in `C:/Users/DuyDang/orca/workspaces/Quire/calc-controller-wt`, coordinator `orca_session_id:fdd6235e-0c22-404c-a221-3b766724035d`.
- Branch `feature/calculation-controller`, landed from main.
- Landed members (`src/Quire.Calculation/00_CalculationController.Callbacks.cs`): `HandleEngineUpToDate`, `HandleGetEngineVersion`, `HandleStartEvaluating`, `HandleStopEvaluating`, `HandleRecalculateAll`, `HandleStartAutoCalc`, `HandleStopAutoCalc`, `HandleSimplifyUnitSystemUpdated`, `HandleCalculationOptionUpdated`, `HandleSystemVariableUpdated`, `HookupContextEvents`, `UnhookFromContextEvents`, `UNITTEST_InitBuilder`, `BindTo`, `HookupAndInitializeEngine`, `HandleContextUnloaded`.
- Release IL: PASS, 225 checked, 0 failures (last gate before commit).
- Blocked by modopt IsLong on `IEngine` handle calls (one-line note each): `HandleCalculateNow` (CalculateNow), `HandleUnitSystemUpdated` (Update), `HandleCalculatableMoved` (MoveAfter), `HandleCalculatableRemoved` (Remove), `HandleCalculatableAdded` (AddRegionA), `AddCalculatableChild` (AddChild).
- Blocked by D7 embedded NoPIA `Mathcad.IValue`: `DataExchangeHandler.SetValue`, `DataExchangeHandler.GetValue`.
- Parked, closure/method ordinal (logic matches, ordinal differs): `Dispose` (lambda `<Dispose>b__12_0` vs ours 21), `HandleEngineBusy` (display class 36 vs ours 27).
- Parked, IL shape: `DumpGetHandleToCalculatableMap` (foreach loop body two instructions longer than Mathcad), `HandleWaitForCalculationIdle` (first mismatch in the `!_evaluatingEnabled` branch store order after two passes).
- Not yet attempted: `HandleEngineNodeCallback` lives in the Mirrored file owned by another lane.
