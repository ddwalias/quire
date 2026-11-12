# Lane C checkpoint: Automation assemblies

- Owner: Lane C (`Quire.Automation`, `Quire.EngNotebook.Automation`, `Quire.EngNotebookPlus.Automation`).
- Branch: `feature/lane-c-20261113-r2`. Worktree: `C:/Users/DuyDang/orca/workspaces/Quire/lane-c-20261113-r2`.

## Members

- Methods: all managed members of the three assemblies are already exact (carried from earlier batches).
- Constants (no IL, so the gate cannot see them): `Quire.Automation` now declares by hand every constant
  MirrorGen had stubbed: `ComClassesGuides.GetValueResult`, `ComClassesProgIds.GetValueResult`,
  `ComEnumerationsGuides.ValueResultTypes`, `HRESULT.CLASS_E_NOAGGREGATION`, `HRESULT.E_CONNECTIONFAILED`,
  `HRESULT.E_NOTIMPL`, `HRESULT.CLASS_E_NOTLICENSED`. The four `Generated/Mirror/Automation.{ComClassesGuides,
  ComClassesProgIds,ComEnumerationsGuides,HRESULT}.g.cs` files held only these stubs and are removed, as regeneration would.

## Release IL report

- `Quire.Automation`: PASS; 232 checked, 232 exact, 0 failures, 0 unfinished.
- `Quire.EngNotebook.Automation`: PASS; 216 checked, 216 exact, 0 failures, 0 unfinished.
- `Quire.EngNotebookPlus.Automation`: PASS; 215 checked, 215 exact, 0 failures, 0 unfinished.

## Remaining mismatches

- `HRESULT.E_UNEXPECTED`, `RPC_E_SERVER_DIED`, `RPC_E_DISCONNECTED` are declared in `Quire.Automation/HRESULT.cs`
  but are not Mathcad members. They are unused. They are kept because `Quire.Mirror.Tests` pins them as mirrored
  members; removing them needs the maintainer to update that shape snapshot.
- `dotnet test tests/Quire.Mirror.Tests -c Release`: 17 failures in projects outside lane C (for example
  `Quire.Calculation.Core`, `Quire.Controls.*`, `Quire.Mcd`) and `Every_in_scope_Mathcad_type_has_a_Quire_type`
  (msclr types). No lane C assembly fails.
