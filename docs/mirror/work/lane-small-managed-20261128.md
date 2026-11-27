# Small managed projects checkpoint (2026-11-28, not landed)

- Owner: dispatched worker `task_c389de95aad1` (coordinator `orca_session_id:fdd6235e-0c22-404c-a221-3b766724035d`).
- Branch `feature/small-managed-lane`, worktree `C:/Projects/Quire-wt/small-managed-lane`, from `main` 1e181b8de.
- Scope asked: Quire.Engine, Quire.Calculation, Quire.Controls.Text, Quire.Controls, Quire.Controls.SolveBlock, Quire.Controls.Core, Quire.Componentsmgd.
- Landed: nothing. No batch reached 20 members and no candidate matched Release IL.

## Trial results

- `Quire.Controls.Help.EngNoteContextData.KeyValuePairRegex` as `[GeneratedRegex]`: Release IL FAIL. Roslyn adds `RegexGenerator` runner types with no Mathcad counterpart. Reverted.
- `Quire.Controls.Core.RtfDocConstructor.PrepareRtf` (body written from the reference IL): Release IL FAIL at call #5, `call` vs Mathcad `callvirt` on `new RtfDocConstructor(...).PopulateRtfTree`. Roslyn omits `callvirt` for a `new` receiver. This is the parked item in `lane-controls-nrtftree-luna.md`. Reverted.

## Remaining, by the blockers recorded in the lane docs

- Quire.Engine (315 stubs in `Generated/Mirror`): the hosted `iengine`/`engutils-mgd` are MSVC C++/CLI (D19/D20). `IEngine` members, `modopt(IsLong)` handle signatures, `.maxstack`/fat-header shapes, `fault` blocks and `std::basic_string` types cannot be emitted from C#. Per `lane-engine-20261109.md`, these are not matchable. Not attempted.
- Quire.Calculation (25 generated + 18 hand-written): `modopt(IsLong)` on `IEngine` handle calls (`HandleCalculateNow`, `HandleUnitSystemUpdated`, `HandleCalculatableAdded/Moved/Removed`, `AddCalculatableChild`, `CreateEngine`); D7 NoPIA `Mathcad.IValue` (`DataExchangeHandler.SetValue/GetValue`); closure ordinals (`Dispose`, `HandleEngineBusy`); `IEngine` handle members (`ProcessResult`, `ProcessIdBoundLabels`, `ProcessAlgorithms`, `ProcessWarningRedefinition`, `ParentGlobalErrorResultsDictionary`, `SetResult`).
- Quire.Controls.SolveBlock: `SolveBlockChildManager.Rewrap` uses an anonymous type `<>f__AnonymousType0` with no Quire counterpart; `TheSolverMoved` and `CreateCalculatableWrapper` are closure-ordinal items (`lane-controls-solveblock.md`).
- Quire.Controls (7 stubs): `EngNoteContextData.KeyValuePairRegex` (GeneratedRegex, above); `EngineeringDocument.SaveFileAndUpdateCWDState` (Roslyn `dup` vs `stloc`, `lane-controls-20261109.md`); `DimensionSelectorButton` ctor and `IDimensionSelectorView.CommandParameter` (base-class type-map decision).
- Quire.Controls.Core: `PrepareRtf` (above); `OleContainerModel_1` setter and `McdxSerializer.PackImage` parked on local-slot and display-class shape.
- Quire.Componentsmgd: `VSDataValue` (9 members) and `McxComponent.FixInputs` are blocked by the `Mathcad.IMatrixValue` mapping (`interop.mathcad` vs `automationmgd`); `VSValueFactory.VSDataToAutomation` shares that blocker (`lane-automation-native.md`). `McxComponent.running` field is not in a prior-verified shape.
- Quire.Controls.Text (≈12 generated + ≈35 hand-written placeholders): closure-ordinal layout in `lane-controls-text-richtext-ordinal.md`, plus the receiver-retention and stack-across-statement families in `lane-controls-text-agy.md`.

## Next

Each remaining item needs a new shape or a mapping decision that the lane docs record as unresolved. The next step is a maintainer decision on the IEngine/IValue/IMatrixValue type mappings, not more member-level attempts.
