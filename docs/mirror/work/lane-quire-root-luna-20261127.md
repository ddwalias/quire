# Quire root batch — Find and plot controls

- Owner: luna, task `task_c53d1b661ac1`.
- Branch: `feature/luna-quire-root-b1`, from main `4316d4fa8`.
- Members: `FindUserProperties` (accessors, constructor, callbacks), `FunctionLabel` (accessors, constructor, command state/event handling), and `NumberOfPointsComboBox` (validation, selection, binding, and input handlers).
- Parked: `FunctionLabel.ExecuteCommandWithParameter`; Roslyn assigns the `ICommand` local to slot 1 while Mathcad assigns it to slot 3, including with a type-pattern switch.
- Release IL: PASS (`python tools/validation/release_il.py --base main --project Quire`), 1,443 checked; report `.mirror-cache/release-il-d8e98dd46ab3/release-il.json`.
