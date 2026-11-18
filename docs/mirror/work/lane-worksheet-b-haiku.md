# Lane Quire.Controls.Worksheet, half B (haiku takeover)

- Owner: Claude worker (task_5dd9db6a59d5), taking over the stalled Codex half-B worker.
- Branch: `mirror/worksheet-b-haiku-20261119`
- Scope: `Controls.Worksheet.RtfItemWrapperHelper` through the end of the generated stub list (WorksheetViewModel first).

## Batch A: WorksheetViewModel (19 members, Release IL exact)

- Members: `ActivateContent`, `ToggleShowGrid`, `ToggleShowFrame`, `ToggleShowMajorGridlines`, `CanToggleIsSolveBlockLabelVisible`, `HandleStopRunningCalculations`, `FireCalculateNow`, `GoToNextError`, `GoToFirstError`, `GoToPreviousError`, `GoToLastError`, `MoveItem`, `InsertFileName`, `GetDescendantInsertionPoint`, `CanToggleRegionsAsInput`, `CanToggleRegionsAsOutput`, `ToggleHeaderActivation`, `ToggleFooterActivation`, `PopulateZombies`.
- Release IL gate: 19 changed members went from generated stub to exact; no regressions. The whole-type line fails only because other stubs remain.

## Parked (one attempt each, stubs kept)

- `ActivateHeader`, `ActivateFooter`: the `IsFirstPageHeaderFooterDifferent` branch order differs. The `if`/`else` form puts the `get_Header` call before Mathcad's `get_ActivePage` at the first difference; the first try used a ternary and differed at the `brfalse`.
- `MoveItemsBy`: `WorksheetCommand.Execute()` is emitted as `call`; Mathcad emits `callvirt`.
- `CanGotopage`: Mathcad has 3 conditional branches for the `TryParse && > 0 && <=` condition; the `&&` form gives 2.
- `FireRecalculateAll`, `CommitActive` (earlier attempt, dropped): lambda closure ordinal (`b__628_0` vs `b__424_0`) depends on the whole class's method order; `isinst` order differs for `CommitActive`. Not reshaped.

## Remaining

- WorksheetViewModel: roughly 800 stub lines remain, most of them behind nested command classes, lambdas (closure ordinals) and placeholder ordering.
- Other types in the half: WorksheetControlContent, WorksheetControl, WorksheetControlDebugger, WorksheetPageManager, WorksheetCommands, ZombieCreator, WorksheetItemManager, WorksheetVisualHelper, and the rest after RtfItemWrapperHelper. Not started.
- WorksheetModel: parked (constructor closure ordinals).
