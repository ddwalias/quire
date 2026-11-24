# Quire.Calculation.Core: SynchronizationStatus value checkpoint

- Owner: Orca worker task_a309f34e82b7 (haiku-calculation-core-high).
- Branch `feature/haiku-calculation-core-high`, rebased onto `main` fb49a0225.
- Scope: `SynchronizationStatus.Synchronized` was 1 (implicit) in Quire; Mathcad `Ptc.Calculation.Core.SynchronizationStatus` is `Pending = 0, Synchronized = 3` (reference.py IL: `Synchronized = int32(3)`). Set the Quire enum to Mathcad's values. Replaced the four `(SynchronizationStatus)3` casts in this assembly with `SynchronizationStatus.Synchronized` (same `ldc.i4.3` IL).
- Callers: no code in the repo compares by name to `SynchronizationStatus.Synchronized` (checked src and tests). Stored results serialize by name (`ToString`/`Enum.Parse`), so the numeric change does not alter saved data. Other assemblies keep their `(SynchronizationStatus)3` casts; they now match the enum value.

## Release IL (`release_il.py --base main`)

| Run | Result |
|---|---|
| `--project Quire.Calculation.Core` | PASS: 574 checked, 574 exact, 0 failures, 0 missing, 0 unfinished |
| `--project Quire.Calculation.Core --all` | PASS: 824 checked, 574 exact, 0 failures, 0 missing, 0 unfinished, 0 skipped |

Affected dependents (`lane_merge` revalidates them): every project whose reference closure reaches Quire.Calculation.Core, including the UI projects Quire.Controls.*, Quire.EquationEditor, Quire.Calculation and Quire. Not run locally; they need a UI landing slot.

## CalculatingStatus (second batch)

- Mathcad `Ptc.Calculation.Core.CalculatingStatus` is `NotComputing = 2, Computing = 1` (reference.py IL). Quire `Calculating = 1` already matches `Computing`. Quire `Idle` was 0 and is now 2 (`NotComputing`). Quire usages back the mapping: new calculatables and removed children are set to 2 (`SimpleCalculatable`, `CalculatableChildHider`, `IncludedCalculatableWrapper`), and the engine reports 1 while computing.
- Names are kept (`Idle`, `Calculating`); renaming would break `Quire.Controls.Include` (another lane). No repo code names `Idle` or prints or parses the enum by name, and the only name-based reference in a dependent is `CalculatingStatus.Calculating` (1, unchanged).
- Zero-default fields (`SpecTableCalculatableWrapper.g`, `IncludeWorksheetCalculatableHandler._calculatingStatus` before its ctor sets it) read numeric 0 before and after this change, so their runtime values do not change.
- Gate: `--project Quire.Calculation.Core --all` PASS, 824 checked, 574 exact, 0 failures, 0 missing, 0 unfinished, 0 skipped.

## Remaining

- `Quire.Controls/IO/IOManager.cs` still has a stale comment about Quire not yet assigning 3; its owner should drop it.
- Dependents are revalidated by `lane_merge`; they need the UI landing slot.
