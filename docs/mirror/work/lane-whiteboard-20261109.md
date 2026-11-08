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

## Batch 3 — IWhiteboardViewModel

- Members: 115 interface declarations on `IWhiteboardViewModel` (properties, events, methods).
- Hand-written: `Mirrored/Controls.Whiteboard.IWhiteboardViewModel.cs` (abstract interface declarations, no bodies).
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.IWhiteboardViewModel.g.cs`.
- Shapes: pure interface declarations without bodies; matched first time.
- Release IL: PASS — 339 checked, 0 failures, 632 unfinished (`release-il-282180e10a25`).
- Remaining mismatches in this batch: none.

## Batch 4 — WhiteboardCommand, WhiteboardCommands

- Members:
  - `WhiteboardCommand`: `ViewModel`, `Name` (get/set), `.ctor`, `Execute`, abstract `Do`, `Undo`, `Redo`
  - `WhiteboardCommands`: `.cctor` (static commands `DeleteActiveItem`, `DeleteSelectedItems`, `DeleteActiveOrSelectedItems`, `InsertVerticalSpace`, `DeleteVerticalSpaceAbove`, `DeleteVerticalSpaceBelow`, `SeparateOverlappedItemsVertically`, `SeparateOverlappedItemsHorizontally`, `SelectAllRegions`)
- Hand-written: `Mirrored/Controls.Whiteboard.Commands.WhiteboardCommand.cs`, `Mirrored/Controls.Whiteboard.WhiteboardCommands.cs`.
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.Commands.WhiteboardCommand.g.cs`, `Generated/Mirror/Controls.Whiteboard.WhiteboardCommands.g.cs`.
- Shapes: `ViewModel` as `_viewModel as WhiteboardViewModel`; `Execute` calling `CommandBuffer.Execute(this)`; `.cctor` initializing commands and input gestures matching IL.
- Release IL: PASS — 230 checked, 0 failures, 627 unfinished (`release-il-b8d8f167d702`).
- Remaining mismatches in this batch: none.

## Batch 5 — WhiteboardModel

- Members:
  - `WhiteboardModel`: `WhiteboardItemMgr` (protected static get), `WhiteboardItems` (get/private set), `PositionComparer` (get/private set), `.ctor`, `Clone` (virtual), `CopyStateTo` (protected), `ShouldCloneItem` (protected virtual), `InsertItemAtPosition`, `RemoveItem` (virtual), `GetItemLocation` (protected virtual), `AddItemAtLocation` (virtual), `InsertItemAtAppropriatePosition` (protected), `SetItemLocation` (virtual), `UpdatePosition` (protected), `HasOpenDescendantOleObjects` (get), `IsDisposed` (protected get/private set), `Dispose` (virtual)
  - Closures: `<>c__DisplayClass20_0` (`<UpdatePosition>b__0`), `<>c` (`<get_HasOpenDescendantOleObjects>b__22_0`)
- Hand-written: `Mirrored/Controls.Whiteboard.WhiteboardModel.cs`.
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.WhiteboardModel.g.cs`.
- Shapes:
  - Declaration order matching Mathcad source order so `<UpdatePosition>b__0` gets ordinal 20 and `<get_HasOpenDescendantOleObjects>b__22_0` gets ordinal 22.
  - `CopyStateTo` using `if (cloneable != null)` statement (emits `brfalse.s` branching to else) rather than ternary.
  - `SetItemLocation` using early `if (WhiteboardItemMgr.GetItemLocation(item) == location) return;` matching IL.
- Release IL: PASS — 255 checked, 0 failures, 607 unfinished (`release-il-5ba6d854e46e`).
- Remaining mismatches in this batch: none.


