# Quire automation try-return wrappers: null-test pattern (20261127)

- Owner: Claude worker in `C:/Users/DuyDang/orca/workspaces/Quire/quire-automation4-wt`, coordinator `orca_session_id:fdd6235e-0c22-404c-a221-3b766724035d`.
- Branch `feature/quire-automation-batch4-20261127`, from main.

## Pattern (verified by Release IL)

A value-returning wrapper that reads a document or object and returns a default on failure matches Mathcad when the null test is written with `== null` and the default on the true branch, with the object held in a local:

```csharp
try
{
    IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
    return engineeringDocument == null ? null : engineeringDocument.X;
}
catch
{
}
return null;
```

Shapes that did NOT match (first difference shown):

- `GetX()?.Y` on a call result: `dup` where Mathcad has `stloc.0`.
- `x != null ? x.Y : default`: `brtrue` where Mathcad has `brfalse`.
- Single-use local, Release: the local is elided, same as `?.`.

Plain `try { return X; } catch { } return default;` with no null test also matches (AutomationSpiritInputResult/OutputResult/... explicit getters). The earlier note that Roslyn drops that branch is not correct for these members.

## Landed in this lane

- Root and non-Plus `AutomationSpirit{Input,Output}Result`, `{Input,Output}MatrixResult`, `OutputMatrixResultAs`, `OutputResultAs` explicit/public getters (try-return).
- Plus variants of the same six types (this batch).
- `AutomationServiceEngNote(Plus).CMathcadPrimeWorksheet` try-return getters and wrappers (`DoGetModified`, `DoGetTitle`, `DoGetWorksheetTabIcon`, `DoGetWorksheetTabName`, `DoGetClosingPrompt`, `DoGetWorksheetDisplayedFilePath`, `DoGetWorksheetWorkingDirectory`, `DoGetDefaultCalculationTimeout`, `InputGetStringValue`, `OutputGetStringValue`, `InputGetSExprValue`).
- `CMathcadPrimeWorksheets.Count` and `DoGetCount`; `CMathcadPrimeWorksheetReadonlyOptions` fields, `TryConvertToBool`, `ConvertToDocumentReadonlyOptions` (both variants).

## Still parked

- `CMathcadPrimeWorksheetReadonlyOptions.SetOptionValue`: first difference at #4. Mathcad keeps its int return temporary in local 0 before the block locals; Roslyn allocates the first block local in slot 0. Candidate fix (untested): hoist a declaration so the return slot is allocated first.
- `CMathcadPrimeWorksheets.Item`, `CMathcadPrimeWorksheet` setters, `Visible`, `Quit`, `CloseAll`, `UpdateInputs`, `SetContextData`, `Open`/`OpenEx`: `Delegate0`/`DelegateFunction0` lambdas (display-class ordinals, needs the compile-order device).
- Root `CMathcadPrimeApplication` (57) and `CMathcadPrimeWorksheet` (98): structural mismatch (no `Ping` member; constructor with no Mathcad counterpart).
