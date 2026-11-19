# Lane Quire root — Automation wrapper types (haiku)

- Owner: Orca task `task_eb81fcf92ea0` (dispatched worker).
- Branch: `mirror/quire-root-automation-haiku-20261119`, from `main` `6619ab43a`.
- Scope: `src/Quire/Generated/Mirror/Automation*.g.cs` (three namespace variants each).

## Landed (judged exact with `release_il.py --project Quire --type ...`)

- Constructors only: AutomationSpiritInputResult, AutomationSpiritOutputResult, AutomationSpiritInputMatrixResult, AutomationSpiritOutputMatrixResult, AutomationSpiritOutputMatrixResultAs, AutomationSpiritInputs, AutomationSpiritOutputs, SpiritMainWindowAutomationPeer, CMathcadPrimeWorksheets (field and ctor).
- SetValueResults: field and constructor.
- AutomationSpiritMatrixResult: fields, `ErrorCodeInternal`, `ErrorCode`, `MatrixResult`, `Units`, constructor.
- AutomationService.AutomationSpiritAliasesList: both constructors, `Count`, `GetAliasByIndex`. Shapes: typed `foreach (IOItem item ...)` (Mathcad casts the loop variable); `0 <= index && Count > index` (Mathcad tests the bounds in that order).
- AutomationSpiritInputsOutputsConflicts: field, constructor, four methods (void `try { call } catch { }` matches).
- AutomationSpiritValuesSetter: field, constructor, `AddScalarValue`, `AddMatrixValue`, `AddStringValue`, `AddSExprValue`.
- ScalarStateExtensions: `GetOriginalScalarState`, `GetScalarState`.
- AutomationGetValueResult: field, constructor, `GetErrorCode` (root only; variants have no such stub), `ResultType`, `ErrorCode`, `Units`, `RealResult`, `StringResult`, `MatrixResult`. `ResultType` uses `ErrorState != null && ErrorState.ErrorCode != 0` inside `DevAssert`.
- AutomationSpiritMatrix: constructors, field, `Matrix`, indexer, `Rows`, `Columns`, `GetMatrixSize`. Shapes: `GetMatrixElement(...) != -1` with `return value` first; `_matrix != null ? Length : -1`.
- AutomationSpiritInputsOutputsStates: fields and constructor.

## Parked (stubs kept, exact blocker)

- Value-returning `try { return x; } catch { } return default;` wrappers: Mathcad keeps the catch's `leave` to the next instruction; Roslyn drops it. Affects: explicit getters of InputResult, OutputResult, InputMatrixResult, OutputMatrixResult, OutputResultAs (`ErrorCode`, `RealResult`); Inputs/Outputs `Count` and `GetAliasByIndex`; SetValueResults `Count`, `GetResultByIndex`, `GetResultByAlias`; ValuesSetter `DoSetValues`; AutomationSpiritMatrix `GetMatrixElement`, `SetMatrixElement`; InputsOutputsStates `ScalarInputsCount`, `ScalarOutputsCount`, `Get*ByIndex`, `Get*ByAlias`. Two forms were tried in the earlier batch; no new form found.
- ValuesSetter `SetValues`: compiler display class `<>c__DisplayClass6_0` for the lambda; compile-order device not attempted.
- InputsOutputsStates `GetInputItem`/`GetOutputItem`: lambda `<>c` ordinals (`b__14_0`, `b__15_0`); not attempted.

## Not started

- `CMathcadPrimeApplication` (116 stub lines, 100 try regions, 70 closure display classes, COM event subscription).
- `CMathcadPrimeWorksheet` (201 stub lines).
- `CMathcadPrimeWorksheetReadonlyOptions`: `SetOptionValue` parked earlier; other stubs not reviewed.
- `IConvertableToDocumentReadonlyOptions` (interface stub); `CMathcadPrimeWorksheets` `Item`, `get_Count`, `DoGetCount`, `DoGetOpenItemByIndex` (closure and try-value).

## Release IL

- Each landed type's gate: changed members exact; other failures are the remaining stubs (a whole-type FAIL is expected).
- Remaining Automation stub attributes across `Automation*.g.cs`: about 470 (includes the CMathcad types above).
