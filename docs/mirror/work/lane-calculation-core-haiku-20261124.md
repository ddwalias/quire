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

## Remaining

- `CalculatingStatus` has the same class of mismatch: Quire `Idle = 0, Calculating = 1`; Mathcad `NotComputing = 2, Computing = 1`. Not changed here: `Idle = 0` is the zero default and a change needs a separate decision. Callers keep their `(CalculatingStatus)2` casts.
- Stale comments in other assemblies still say Quire has not assigned 3 (`Quire.Controls/IO/IOManager.cs`); those owners should drop them when they touch those files.
