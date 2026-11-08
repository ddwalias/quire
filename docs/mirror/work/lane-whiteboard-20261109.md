# Lane Whiteboard — Quire.Controls.Whiteboard

- Owner: Orca task `task_0c9ead988868` (maintainer-assigned; no WORK.md lane row).
- Branch: `mirror/whiteboard-20261109`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/whiteboard-20261109`.
- Baseline (main `990cd4f0`): 215 exact, 785 unfinished.

## Batch 1 — PositionComparer, WhiteboardCanvas, WhiteboardSelectionDragger

- Members:
  - `PositionComparer`: `Compare`
  - `WhiteboardCanvas`: `.ctor`, `InitializeComponent`, `IComponentConnector.Connect`
  - `WhiteboardSelectionDragger`: `.ctor`, `DragDone` event (`add`/`remove`), `MouseUp(object, MouseButtonEventArgs)`, `MouseMove(object, MouseEventArgs)`, `MouseUp(Point)`, `MouseMove(Point)`
- Hand-written: `PositionComparer.cs`, `Mirrored/Controls.Whiteboard.WhiteboardCanvas.cs`, `Mirrored/Controls.Whiteboard.WhiteboardSelectionDragger.cs`.
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.WhiteboardCanvas.g.cs`, `Generated/Mirror/Controls.Whiteboard.WhiteboardSelectionDragger.g.cs`.
- Shapes:
  - `PositionComparer.Compare` using `DoubleComparer.DefaultDoubleComparer.Compare` and `-1` fallback.
  - `WhiteboardCanvas` using `DebugTools.AssertNotNull` and `LoadComponent`.
  - `WhiteboardSelectionDragger` using `WpfUtils.IsSignificantMouseMove`, `SelectionDraggerRubberBand`, and direct `DragDone` invocation.
- Release IL: PASS — 224 checked, 224 exact, 0 failures, 776 unfinished (`release-il-43956f0820fa`).
- Remaining mismatches in this batch: none.

## Batch 2 — IWhiteboardModel, IMathboardModel, IMathboardViewModel

- Members:
  - `IWhiteboardModel`: `WhiteboardItems` get-only, `PositionComparer` get-only, `InsertItemAtPosition`, `RemoveItem`, `AddItemAtLocation`, `SetItemLocation`
  - `IMathboardModel`: `IsEmpty` get-only, `IoRegionManager` get/set, `BookmarksManager` get/set, `IsInPrinting` get/set, `CalculatableHiddenStatusChanged` event, `CommitItem`
  - `IMathboardViewModel`: `Model` get-only, `HideFromCalculationButtonEnabled` get-only, `HideFromCalculationButtonChecked` get-only, `IsInPrinting` get/set, `Commit()`, `RecalculateCmdHandler()`, `Commit(IWhiteboardItem)`, `ToggleInputRegions()`, `ToggleOutputRegions()`, `CanToggleRegionsAsInput()`, `CanToggleRegionsAsOutput()`, `ToggleHideFromCalculationContext()`
- Hand-written: `Mirrored/Controls.Whiteboard.IWhiteboardModel.cs`, `Mirrored/Controls.Mathboard.IMathboardModel.cs`, `Mirrored/Controls.Mathboard.IMathboardViewModel.cs` (abstract interface declarations, no bodies).
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.IWhiteboardModel.g.cs`, `Generated/Mirror/Controls.Mathboard.IMathboardModel.g.cs`, `Generated/Mirror/Controls.Mathboard.IMathboardViewModel.g.cs`.
- Shapes: pure interface declarations without bodies; matched first time.
- Release IL: PASS — 253 checked, 0 failures, 747 unfinished (`release-il-190595613d5f`).
- Remaining mismatches in this batch: none.
