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

## Batch 6 — MathboardModel

- Members:
  - `MathboardModel`: `.ctor`, `InitializeChildManager`, `HandleItemsListChanged`, `AddItems`, `RemoveItems`, `MoveItems`, `IsEmpty`, `CommitItem`, `IoRegionManager`, `BookmarksManager`, `CalculatableChildren`, `FireCalculatableAdded`, `FireCalculatableRemoved`, `FireCalculatableMoved`, `FireCalculatableHiddenStatusChanged`, `HandleShadowCalculationsChanged`, `GetChildShadowCalculatables`, `GetCalculatableOverrides`, `GetWrappedCalculatable`, `IsInPrinting`
  - Events: `CalculatableHiddenStatusChanged`, `CalculatableAdded`, `CalculatableRemoved`, `CalculatableMoved`
  - Closures / iterators: `<AddItems>b__9_0`, `<AddItems>b__9_1`, `<RemoveItems>b__10_0`, `<RemoveItems>b__10_1`, `<MoveItems>b__11_0`, `<MoveItems>b__11_1`, `<>c.<CommitItem>b__14_0`, `<>c__DisplayClass17_0.<set_IoRegionManager>b__0`, `<>d__31` (`CalculatableChildren`), `<HandleShadowCalculationsChanged>b__36_0`, `<HandleShadowCalculationsChanged>b__36_1`, `<>c__DisplayClass37_0.<GetChildShadowCalculatables>b__0`, `<>d__38` (`GetCalculatableOverrides`)
- Hand-written: `Mirrored/Controls.Mathboard.MathboardModel.cs`.
- Stubs removed: `Generated/Mirror/Controls.Mathboard.MathboardModel.g.cs`.
- Shapes:
  - Declaration sequence matching Mathcad source order to yield exact ordinals for all lambdas (`b__9_0/1`, `b__10_0/1`, `b__11_0/1`, `b__14_0`, `DisplayClass17_0`, `b__36_0/1`, `DisplayClass37_0`) and iterators (`d__31`, `d__38`).
  - Pre-switch local declarations in `HandleItemsListChanged` to position compiler-generated `Action` temp local at slot `[3]`.
  - Nested `if` statements in `GetCalculatableOverrides` iterator without early `yield break` to reproduce single exit jump target.
  - Type-pattern switch in `set_BookmarksManager` (`case IMathboardControl ... when ...: break; case IRichTextWithEmbeddedItem ... when ...: break;`) matching Roslyn pattern matching branch and local slot assignment (`[0]` enumerator, `[1]` `IMathboardControl`, `[2]` `IRichTextWithEmbeddedItem`, `[3]` `IWhiteboardItem`).
- Release IL: PASS — 311 checked, 0 failures, 551 unfinished (`release-il-237a61e589d8`).
- Remaining mismatches in this batch: none.

## Batch 7 — WhiteboardBehavior

- Members:
  - `WhiteboardBehavior`: `UIEventsHandlerProperty`, `_itemManager`, `SetUIEventsHandler`, `GetUIEventsHandler`, `ItemManager`, `OnUIEventsHandlerChanged`, `HookupMouseEvents`, `.cctor`
  - Closures:
    - `<>c__DisplayClass6_0`: `<OnUIEventsHandlerChanged>b__0` (`SizeChanged`), `<OnUIEventsHandlerChanged>b__1` (`GotKeyboardFocus`), `<OnUIEventsHandlerChanged>b__2` (`KeyDown`), `<OnUIEventsHandlerChanged>b__3` (`RequestMoveAndResizeEvent`), `<OnUIEventsHandlerChanged>b__4` (`CaretLeavesTextAreaEvent`), `<OnUIEventsHandlerChanged>b__5` (`TabOutOfTextEvent`)
    - `<>c__DisplayClass7_0`: `<HookupMouseEvents>g__PreviewMouseDownHandler|0` (local function), `<HookupMouseEvents>b__1` (`MouseDown`), `<HookupMouseEvents>b__2` (`MouseUp`)
- Hand-written: `Mirrored/Controls.Whiteboard.WhiteboardBehavior.cs`.
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.WhiteboardBehavior.g.cs`.
- Project reference: added `Quire.Controls.Text` to `src/Quire.Controls.Whiteboard/Quire.Controls.Whiteboard.csproj`.
- Shapes:
  - Declaration order `UIEventsHandlerProperty`, `_itemManager`, `SetUIEventsHandler`, `GetUIEventsHandler`, `ItemManager`, `OnUIEventsHandlerChanged`, `HookupMouseEvents` to position `OnUIEventsHandlerChanged` at ordinal 6 and `HookupMouseEvents` at ordinal 7.
  - Local function `PreviewMouseDownHandler` declared before `MouseDown` and `MouseUp` handlers in `HookupMouseEvents` to produce `<HookupMouseEvents>g__PreviewMouseDownHandler|0`, `<HookupMouseEvents>b__1`, and `<HookupMouseEvents>b__2`.
  - In `<OnUIEventsHandlerChanged>b__1`, `if (eventArgs.NewFocus == element) goto HandleFocus; return; HandleFocus: ImeHandler.HandleImeFocus();` to match the `beq.s/ret/call/ret` branch shape.
  - In `<OnUIEventsHandlerChanged>b__2`, `switch (eventArgs.ImeProcessedKey) { case Key.ImeProcessed: break; }` which Roslyn lowers to `callvirt get_ImeProcessedKey; ldc.i4 155; pop; pop; ret;`.
- Release IL: PASS — 328 checked, 0 failures, 534 unfinished (`release-il-e87a089e2a84`).
- Remaining mismatches in this batch: none.

## Batch 8 — EquationTypeIndicatorControl, WrappedChildManager

- Members:
  - `EquationTypeIndicatorControl`: `IndicatorTextProperty`, `TopProperty`, `MainGrid`, `_contentLoaded`, `IndicatorText`, `Top`, `IndicatorColor` (auto-property), `.ctor()`, `.cctor()`, `InitializeComponent()`, `IComponentConnector.Connect(int, object)`
  - `WrappedChildManager`:
    - Fields: `_createCalculatableWrapper`, `_lookupCalculatableWrapper`, `_removeCalculatableWrapper`, `_notifyHiddenStateChanged`, `_notifyContainerChildrenChanged`
    - Methods: `.ctor`, `EffectiveChildren` (iterator `d__6`), `ChildAddedCore`, `ChildAdded` (lambda `b__8_0`), `ChildRemovedCore`, `ChildRemoved` (lambda `b__10_0`), `ChildMoved` (lambda `b__11_0`), `HandleCalculatableMovedInChildContainer`, `HandleCalculatableAddedToChildContainer`, `HandleCalculatableRemovedFromChildContainer`, `GetContainerChildren` (iterator `d__15`), `HiddenStateChanged`
- Hand-written: `Mirrored/Controls.Mathboard.EquationTypeIndicatorControl.cs`, `Mirrored/Controls.Mathboard.WrappedChildManager.cs`.
- Stubs removed: `Generated/Mirror/Controls.Mathboard.EquationTypeIndicatorControl.g.cs`, `Generated/Mirror/Controls.Mathboard.WrappedChildManager.g.cs`.
- Shapes:
  - `EquationTypeIndicatorControl`: declaration order matching Mathcad order; `.cctor` registering `IndicatorTextProperty` and `TopProperty` via `Canvas.TopProperty.AddOwner`; `InitializeComponent` with early return on `_contentLoaded`.
  - `WrappedChildManager`: declaration order matching Mathcad source order so all closure/iterator ordinals match (`d__6`, `b__8_0`, `b__10_0`, `b__11_0`, `d__15`); `ChildMoved` using `if (calculatable is not ICalculationHideableChild hideable || !hideable.HideFromCalculationContext)`.
- Release IL: PASS — 371 checked, 0 failures, 499 unfinished (`release-il-0e18f40b7986`).
- Remaining mismatches in this batch: none.

## Batch 9 — UIElementManager

- Members:
  - `UIElementManager`: `SetFocusOnItem`, `GetItemLocation`, `SetItemLocation`, `ArrangeItem`, `GetItemRenderSize`, `GetItemDesiredSize`, `SetItemSize`, `SetRenderTransform`, `BlockUntilLoaded`, `GetDependencyPropertyValueOnItem`, `GetUIElement`, `.ctor()`
- Hand-written: `Mirrored/Controls.Whiteboard.UIElementManager.cs`.
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.UIElementManager.g.cs`.
- Shapes:
  - `SetItemSize`: `as FrameworkElement` with early null return (`if (frameworkElement == null) return;`) matching the `brtrue.s IL_000b; ret;` shape.
  - `BlockUntilLoaded`: `WpfUtils.DoEvents(Dispatcher.CurrentDispatcher, (DispatcherPriority)6)`.
- Release IL: PASS — 382 checked, 0 failures, 476 unfinished (`release-il-309209ec385c`).
- Remaining mismatches in this batch: none.







## Batch 10 — AggregateMathboardViewModel

- Members:
  - `AggregateMathboardViewModel`:
    - Fields: `_sectionIndicators`, `_hideFromCalculationContext`
    - Constructors: `.ctor(IAggregateMathboardModel, Size?)`, `.ctor(IAggregateMathboardModel, IAlignmentGrid)`, `.ctor(AggregateMathboardViewModel)`
    - Methods: `Clone()`, `CloneAggregateMathboardViewModel()`, `Initialize(IAggregateMathboardModel)`, `MeasureOverrideHelper(Size)`, `ArrangeOverride(Size)`, `HandleGridCellSizeChanged(Size, Size)`, `SetIndicatorsDimensions()`, `UpdateHideFromCalculationButton()`, `ToggleHideFromCalculationContext()`, `IWhiteboardViewModel.CommandBindings` (explicit interface implementation)
    - Properties: `SectionIndicators` (get), `HideFromCalculationContext` (get/set)
  - Closures:
    - `<>c__DisplayClass16_0`: `<set_HideFromCalculationContext>b__0` (`action`), `<set_HideFromCalculationContext>b__1` (`doMethod`), `<set_HideFromCalculationContext>b__2` (`undoMethod`)
- Hand-written: `Mirrored/Controls.Mathboard.AggregateMathboardViewModel.cs`.
- Stubs removed: `Generated/Mirror/Controls.Mathboard.AggregateMathboardViewModel.g.cs`.
- Shapes:
  - Member declaration sequence placing `HideFromCalculationContext` setter at syntax ordinal 16, producing `<>c__DisplayClass16_0`.
  - `.ctor(AggregateMathboardViewModel)` ternary expression `original.IsInPrinting ? new SectionIndicatorInfo(...) : new SectionIndicatorInfo { ... }` compiling to `brtrue.s` branch and exact instruction sequence.
  - `SetIndicatorsDimensions` declaring `Pair<double, double> pair;` before `double height = ContentSize.Height;` to place `pair` at local slot `[1]` and temporary `Size` struct local at slot `[4]`.
- Release IL: PASS — 401 checked, 0 failures, 453 unfinished (`release-il-ba9d960dfb3e`).
- Remaining mismatches in this batch: none.

## Batch 11 — AggregateMathboardModel

- Members:
  - `AggregateMathboardModel`:
    - Constructor: `.ctor(IWhiteboardItemPositionComparer)`
    - Events: `RequestEvaluation`, `GetValueOfIdentifierBefore`, `TreatAsUpToDate`, `SectionsMaybeChanged`
    - Abstract members: `Rl` (get), `BlockSectionCount` (get), `GetBlockSectionItems(uint)`, `GetBlockSectionName(uint)`, `GetBlockSectionMinHeightRatio(uint)`
    - Methods: `Commit()`, `SetStatusSynchronized()`, `SetSynchronizationStatus(SynchronizationStatus)`, `SetCalculatingStatus(CalculatingStatus)`, `SetResult(IResultNode)`, `SetError(EngineError)`, `SetIdentifierLabels(IDictionary<ResolvedId, string[]>)`, `SetAutoSelectAlgorithm(IDictionary<int, string>)`, `SetRedefinitionWarnings(IList<EngineRedefInfo>)`, `FireRequestEvaluation()`, `FireGetValueOfIdentifierBefore(string)`, `FireTreatAsUpToDate()`, `FireSectionsMaybeChanged()`
    - Property: `ContainsSymbolicEvaluation`
  - Closure: `<>c.<get_ContainsSymbolicEvaluation>b__34_0`
- Hand-written: `Mirrored/Controls.Mathboard.AggregateMathboardModel.cs`.
- Stubs removed: `Generated/Mirror/Controls.Mathboard.AggregateMathboardModel.g.cs`.
- Shapes:
  - Member declaration sequence placing `ContainsSymbolicEvaluation` at member ordinal 34 to match `<>c.<get_ContainsSymbolicEvaluation>b__34_0`.
  - Prototype `ICalculatable` interface satisfaction members (`SetResult(CalculationResult)`, `SynchronizationStatus`, `RlChanged`) placed after `ContainsSymbolicEvaluation` at the bottom of the hand-written file, keeping `AggregateMathboardModel.g.cs` empty so `.g.cs` compilation order does not shift member ordinals.
- Release IL: PASS — 419 checked, 0 failures, 435 unfinished (`release-il-dd006b1334d3`).
- Remaining mismatches in this batch: none.

## Batch 12 — WhiteboardItemResizer

- Members:
  - `WhiteboardItemResizer`:
    - Fields: `_mode`, `_container`, `_startPoint`, `_currentPoint`, `_started`, `_bringIntoViewDispatcherTimer`, `_pt`
    - Property: `StartPointInElementCoordinates`
    - Event: `DragDone`
    - Constructor: `.ctor(FrameworkElement, Point, DragMode, FrameworkElement, bool, bool)`
    - Methods: `ResizeEnd`, `MouseMove(Point)`, `StartCaptureEvents`, `EndCaptureEvents`, `MouseUp(object, MouseButtonEventArgs)`, `MouseMove(object, MouseEventArgs)`, `NotifyResizeProgress`, `BringIntoViewCallback`, `OffsetTo`, `MouseUpResizeRightMode`, `MouseMoveResizeRightMode`, `MouseUpResizeBottomMode`, `MouseMoveResizeBottomMode`, `MouseUpResizeBottomRightMode`, `MouseMoveResizeBottomRightMode`, `ResizeLeft`, `MouseMoveResizeLeftMode`, `ResizeBy`, `TransformToElement`, `HandleMouseUp`, `HandleMouseMove`
    - Nested delegates: `ResizeEndHandler`, `ResizeProgressHandler` (retained in `.g.cs` with `MirrorOf` attributes)
- Hand-written: `Mirrored/Controls.Whiteboard.WhiteboardItemResizer.cs`.
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.WhiteboardItemResizer.g.cs`.
- Shapes:
  - `StartCaptureEvents`: early return `if (_started) return;` matching Mathcad's `brfalse.s IL_0009; ret;` branch layout.
- Release IL: PASS — 442 checked, 0 failures, 412 unfinished (`release-il-8608bd007943`).
- Remaining mismatches in this batch: none.

## Batch 13 — WhiteboardItemManager

- Members:
  - `WhiteboardItemManager`:
    - Methods: `GetItemIsInPasting(IWhiteboardItem)`, `SetItemIsInPasting(IWhiteboardItem, bool)`, `ClearItemIsInPasting(IWhiteboardItem)`, `GetItemIsInPrinting(IWhiteboardItem)`, `SetItemIsInPrinting(IWhiteboardItem, bool)`, `ClearItemIsInPrinting(IWhiteboardItem)`, `GetItemLocation(IWhiteboardItem)`, `GetItemLocation(IWhiteboardItem, Func<UIElement, double>, Func<UIElement, double>)`, `GetItemLocation(IWorksheetPageBreak, Func<UIElement, double>, Func<UIElement, double>)`, `GetItemLocation(UIElement, Func<UIElement, double>, Func<UIElement, double>)`, `SetItemLocation(IWhiteboardItem, Point)`, `GetItemPresenter(IWhiteboardItem)`, `ShouldItemBeDeletedIfEmpty(IWhiteboardItem)`, `SetShouldItemBeDeletedIfEmpty(IWhiteboardItem, bool)`, `ClearShouldItemBeDeletedIfEmpty(IWhiteboardItem)`, `IsResizingItem(IWhiteboardItem)`, `GetIgnoreForHorizontalMovementAndWidth(IWhiteboardItem)`, `ItemsOverlapped(IEnumerable<IWhiteboardItem>)`, `GetItemRect(IWhiteboardItem)`, `GetItemTop(IWhiteboardItem)`, `GetItemBottom(IWhiteboardItem)`, `SortItemsByLocation(IEnumerable<IWhiteboardItem>, bool)`, `.ctor()`
  - Closures: `<SortItemsByLocation>b__21_0`, `<SortItemsByLocation>b__21_1`
- Hand-written: `Mirrored/Controls.Whiteboard.WhiteboardItemManager.cs`.
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.WhiteboardItemManager.g.cs`.
- Shapes:
  - Declaration sequence placing `SortItemsByLocation` at member index 21, generating exact closure names `<SortItemsByLocation>b__21_0` and `<SortItemsByLocation>b__21_1`.
  - `SetItemLocation`: `IWhiteboardItemPresenter itemPresenter = GetItemPresenter(item); if (itemPresenter != null) itemPresenter.HandleItemLocationChanged(point);` to produce local slot 0 and `brfalse.s` branching instead of `?.` operator's stack dup.
  - `GetIgnoreForHorizontalMovementAndWidth`: `return item is IOleContainer || item is IPageWideSizable;` matching `brtrue.s` jump to `ldc.i4.1; ret;`.
- Release IL: PASS — 466 checked, 0 failures, 388 unfinished (`release-il-729bae86dfa0`).
- Remaining mismatches in this batch: none.

## Batch 14 — WhiteboardDragger

- Members:
  - `WhiteboardDragger`:
    - Fields: `_default`, `IsDragSourceProperty`, `IsDropTargetProperty`, `_dragDropManager`, `_whiteboardControl`, `_startLocation`, `_dataFormat`, `_dragData`, `_whiteboardItemManager`
    - Properties: `Default`, `ViewModel`
    - Event: `OnNoDrag`
    - Methods: `MouseDown`, `GetIsDragSource`, `SetIsDragSource`, `IsDragSourceChanged`, `GetIsDropTarget`, `SetIsDropTarget`, `IsDropTargetChanged`, `FireOnNoDrag`, `EndDrag`, `DragSourceMouseLeftButtonDown(object, MouseButtonEventArgs)`, `DragSourceMouseLeftButtonDown(FrameworkElement)`, `DragSourceMouseLeftButtonUp`, `DragSourceMouseMove`, `TopWindowQueryContinueDrag`, `TopWindowDragEnter`, `TopWindowDragOver`, `TopWindowDragLeave`, `VisualCleanup`, `DoDrag`, `DropTargetDragEnter`, `DropTargetDragOver`, `DropTargetDragLeave`, `HookupDragSourceEvents`, `UnhookDragSourceEvents`, `HookupDropTargetEvents`, `UnhookDropTargetEvents`, `.ctor()`
    - Closures: `<EndDrag>b__25_0`, `<TopWindowQueryContinueDrag>b__30_0`, `<VisualCleanup>b__34_0`, `<>c__DisplayClass35_0` (`<DoDrag>b__0`)
    - Stub preserved: `DropTargetDrop` (Roslyn devirtualizes `new MoveItemsCommand(...).Execute()` to non-virtual `call`, whereas Mathcad IL calls `callvirt WhiteboardCommand.Execute`).
- Hand-written: `Mirrored/Controls.Whiteboard.WhiteboardDragger.cs`.
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.WhiteboardDragger.g.cs`.
- Shapes:
  - Member sequence placing `EndDrag` at member 25, `TopWindowQueryContinueDrag` at 30, `VisualCleanup` at 34, `DoDrag` at 35, generating exact closure names `<EndDrag>b__25_0`, `<TopWindowQueryContinueDrag>b__30_0`, `<VisualCleanup>b__34_0`, `<>c__DisplayClass35_0`.
  - Early returns `if (dragSource == null) return;` and `if (dropTarget == null) return;` in `IsDragSourceChanged` and `IsDropTargetChanged` to match `brtrue.s IL_000b; ret;`.
  - Early return `if (_whiteboardControl == null) return;` in `DragSourceMouseLeftButtonDown`.
  - Split guards `if (_whiteboardControl == null || ViewModel == null || ViewModel.CurrentDragMode != DragMode.Move) return;` followed by `if (!WpfUtils.IsSignificantMouseMove(...)) return;` in `DragSourceMouseMove`.
  - Early return `if (_whiteboardControl == null || whiteboardViewModel == null || whiteboardViewModel.ActiveOrSelectedItems.Count() == 0) return;` in `DoDrag`.
- Release IL: PASS — 502 checked, 0 failures, 350 unfinished (`release-il-fa956f357501`).
- Remaining mismatches in this batch: none.

## Batch 15 — AggregateMathboardControl

- Members:
  - `AggregateMathboardControl`:
    - Properties: `AggregateModel`, `AggregateViewModel`, `HideFromCalculationContext` (get/set), `Rl`, `SynchronizationStatus`, `CalculatingStatus`, `ErrorCount`, `ErrorSExpression`, `ErrorCode`, `ErrorText`, `IsCommitted`, `Error`, `EngineResult` (get/set), `InterceptAndUpdateResult`, `UseICalculatableToRestoreResult`, `SectionIndicators`, `ContainsSymbolicEvaluation`
    - Constructors: `.ctor(IMathboardViewModel, ResourceDictionary)`
    - Events: `RequestEvaluation`, `GetValueOfIdentifierBefore`, `TreatAsUpToDate`
    - Methods: `ViewModelPropertyChangedHandler(object, PropertyChangedEventArgs)`, `BindProperties()`, `HookupEvents()`, `BindCommands()`, `Commit()`, `SetStatusSynchronized()`, `SetSynchronizationStatus(SynchronizationStatus)`, `SetCalculatingStatus(CalculatingStatus)`, `SetResult(IResultNode)`, `SetError(EngineError)`, `SetIdentifierLabels(IDictionary<ResolvedId, string[]>)`, `SetAutoSelectAlgorithm(IDictionary<int, string>)`, `SetRedefinitionWarnings(IList<EngineRedefInfo>)`, `RestoreResult()`, `MeasureOverride(Size)`, `FireRequestEvaluation()`, `FireGetValueOfIdentifierBefore(string)`, `FireTreatAsUpToDate()`, `DeserializeRegionsEpilog(IDeS11NProvider, mathBoardType)`, `SerializeRegionsEpilog(IS11NProvider, mathBoardType)`
  - Closures: `<HookupEvents>b__10_0`
- Hand-written: `Mirrored/Controls.Mathboard.AggregateMathboardControl.cs`.
- Stubs removed: `Generated/Mirror/Controls.Mathboard.AggregateMathboardControl.g.cs`.
- Shapes:
  - Member declaration sequence placing `HookupEvents` at syntax member index 10, generating exact closure name `<HookupEvents>b__10_0`.
  - Constants `(SynchronizationStatus)3` and `(CalculatingStatus)2` matching Mathcad enum values for `Synchronized` and `NotComputing`.
  - `ErrorCount` ternary `(Error != null) ? 1 : 0` emitting `ldnull; cgt.un; ret;`.
  - `ErrorCode` and `ErrorText` accessing `Error` twice (`if (Error == null) return string.Empty; return Error.ErrorCode;` / `return Error.ResourceString;`) without local slot.
  - Direct field reads in event firing methods (`if (RequestEvaluation != null) RequestEvaluation(this);`) without temporary local slots.
  - Reassignment of `constraint` parameter in `MeasureOverride` matching `starg.s constraint`.
- Release IL: PASS — 543 checked, 0 failures, 308 unfinished (`release-il-0b69db0490a3`).
- Remaining mismatches in this batch: none.

## Batch 16 — WhiteBoardSerializationStrategy, WhiteBoardDeserializationStrategy

- Members:
  - `WhiteBoardSerializationStrategy`:
    - Field: `_whiteboard`
    - Constructor: `.ctor(WhiteboardControl)`
    - Methods: `SerializeRegionsEpilog(XmlDocument)` (abstract), `ConvertToRegionPersistantData(IS11NProvider, IWhiteboardItem)` (abstract), `GetRegions(IS11NProvider)`, `GetLocation(IWhiteboardItem)`
  - `WhiteBoardDeserializationStrategy`:
    - Field: `_whiteboard`
    - Constructor: `.ctor(WhiteboardControl)`
    - Property: `Regions` (get)
    - Methods: `DeserializeRegionProlog(UIElement)`, `AddRegion(UIElement, Point)`, `DeleteRegion(UIElement)`, `IsRegionNotAffectingCalculation(UIElement)`
- Project references: added `Quire.Controls.Include` and `Quire.Controls.EquationControl` to `Quire.Controls.Whiteboard.csproj`.
- Hand-written: `Mirrored/Controls.Whiteboard.WhiteBoardSerializationStrategy.cs`, `Mirrored/Controls.Whiteboard.WhiteBoardDeserializationStrategy.cs`.
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.WhiteBoardSerializationStrategy.g.cs`, `Generated/Mirror/Controls.Whiteboard.WhiteBoardDeserializationStrategy.g.cs`.
- Shapes:
  - Member sequence matching Mathcad metadata tokens in both classes.
  - In `WhiteBoardDeserializationStrategy.DeleteRegion`, `if (control is IWhiteboardItem) _whiteboard.RemoveItem(control as IWhiteboardItem);` reproducing double `isinst` check before exception throw.
  - In `WhiteBoardDeserializationStrategy.IsRegionNotAffectingCalculation`, `if (element is EquationControl equationControl) return equationControl.EquationType == EquationTypes.Evaluation; return false;` matching `ceq; ret`.
- Release IL: PASS — 552 checked, 0 failures, 290 unfinished (`release-il-728b337697d3`).
- Remaining mismatches in this batch: none.

## Batch 17 — MathboardViewModel

- Members:
  - `MathboardViewModel`:
    - Fields: `_currentNumberFormat`, `_currentMatrixFormat`, `_currentTextFormat`, `_hideFromCalculationButtonEnabled`, `_hideFromCalculationButtonChecked`, `_containerFrameThickness`, `_mathboardModel`
    - Nested types: `PropertyNames`, `RemoveMathboardItemsCommand`, `PasteMathboardItemsCommand`
    - Constructors: `.ctor(IMathboardModel, Size?)`, `.ctor(IMathboardModel, IAlignmentGrid)`, `.ctor(MathboardViewModel)`
    - Properties: `ContainerFrameThickness` (get/set), `Model` (get), `CurrentNumberFormat` (get/set), `CurrentMatrixFormat` (get/set), `CurrentTextFormat` (get/set), `HideFromCalculationButtonEnabled` (get/set), `HideFromCalculationButtonChecked` (get/set), `ActiveItem` (set), `IndicatorRange` (get)
    - Methods: `Initialize(IMathboardModel)`, `HookupEventHandlers()`, `Commit()`, `RecalculateCmdHandler()`, `Commit(IWhiteboardItem)`, `CanToggleRegionsAsInput()`, `ToggleInputRegions()`, `CanToggleRegionsAsOutput()`, `ToggleOutputRegions()`, `ToggleHideFromCalculationContext()`, `OnActiveTextRegionContentChanged(object, RoutedEventArgs)`, `HandleChildItemGotFocus(IWhiteboardItem)`, `GetRemoveItemsCommand(string, IDictionary<IWhiteboardItem, Point>)`, `GetPasteItemsCommand(string, IDictionary<IWhiteboardItem, Point>)`, `SetupCommandBindings()`, `CanBookmarkTag(object, CanExecuteRoutedEventArgs)`, `HandleBookmarkTag(object, ExecutedRoutedEventArgs)`, `CanRemoveBookmarkTag(object, CanExecuteRoutedEventArgs)`, `HandleRemoveBookmarkTag(object, ExecutedRoutedEventArgs)`, `CreateSeparateOverlappedItemsCommand(IEnumerable<IWhiteboardItem>, Dimension)`, `UpdateHideFromCalculationButton()`, `FormatChangedHandler(object, EventArgs)`, `UpdateActiveItemFormat()`, `UpdatePropertiesFormat()`, `GetDefaultDimensions(IWhiteboardItem, out double, out double)`, `UpdateTabStopStatus(IWhiteboardItem)`
  - Closures:
    - `<SetupCommandBindings>b__15_0`, `_1`, `_2`, `_3`
    - `<CanRemoveBookmarkTag>b__18_0`
    - `<ToggleHideFromCalculationContext>b__39_0`, `_1`, `_2`, `_3`, `_4`, `_5`
    - `<>c__DisplayClass39_0` (`<ToggleHideFromCalculationContext>b__6`)
    - `<UpdateHideFromCalculationButton>b__71_0`
    - `<>c__DisplayClass72_0` (`<GetWhiteboardItem>b__0`)
    - Nested command DisplayClasses: `PasteMathboardItemsCommand.<>c__DisplayClass1_0` (`<DoWithResize>b__0`)
- Hand-written: `Mirrored/Controls.Mathboard.MathboardViewModel.cs`.
- Stubs removed: `Generated/Mirror/Controls.Mathboard.MathboardViewModel.g.cs`.
- Shapes:
  - Exact Roslyn member ordinal matching via deliberate ordering of fields, methods, properties, and nested types, matching closure names `<SetupCommandBindings>b__15_*`, `<CanRemoveBookmarkTag>b__18_0`, `<ToggleHideFromCalculationContext>b__39_*`, `<UpdateHideFromCalculationButton>b__71_0`, and `<>c__DisplayClass72_0`.
  - Added `[MirrorOf]` attributes to nested commands `RemoveMathboardItemsCommand` and `PasteMathboardItemsCommand` to resolve counterpart mapping.
  - Early returns in `set_ActiveItem` (`if (value == base.ActiveItem) return;`) and `HandleBookmarkTag` (`if (DebugTools.AssertNotNull(whiteboardItem, null).Failed) return;`).
- Release IL: PASS — 633 checked, 0 failures, 209 unfinished (`release-il-ef910bf63b4d`).
- Remaining mismatches in this batch: none.

## Batch 18 — MathboardControl

- Members:
  - `MathboardControl`:
    - Field: `_model`
    - Constructors: `.cctor()`, `.ctor(IMathboardViewModel, ResourceDictionary)`
    - Properties: `MathboardViewModel` (get), `SerializationStrategy` (abstract get/set), `DeserializationStrategy` (abstract get/set), `CurrentNumberFormat` (get/set), `CurrentMatrixFormat` (get/set), `CurrentTextFormat` (get/set), `HideFromCalculationButtonEnabled` (get), `HideFromCalculationButtonChecked` (get), `CanHideFromCalculationContext` (get), `HideFromCalculationContext` (abstract get/set), `CalculatableChildren` (get), `CurrentError` (get), `VerticalAlignmentOffset` (get), `IsEmpty` (get), `InputOutputRegionManager` (get/set), `BookmarksManager` (get/set), `IORegions` (get), `RegionFactory` (get/set), `DeserializeMathFormat` (set), `DeserializeLabeledIdFormat` (set)
    - Events: `PropertyChanged`, `CalculatableAdded`, `CalculatableRemoved`, `CalculatableMoved`, `OnGotFocusEvent`, `RequestRemove`
    - Methods: `SetupInputGestures()`, `BindProperties()`, `BindProperty(string, DependencyProperty, BindingMode)`, `HookupEvents()`, `BindCommands()`, `ViewModelPropertyChangedHandler(object, PropertyChangedEventArgs)`, `OnGotFocus(RoutedEventArgs)`, `OnSetNumberFormatField(object, ExecutedRoutedEventArgs)`, `OnSetMatrixFormatField(object, ExecutedRoutedEventArgs)`, `OnSetTextFormatField(object, ExecutedRoutedEventArgs)`, `CanChangeTextStyle(object, CanExecuteRoutedEventArgs)`, `DispatchCommandWithinSolveBlock(RoutedCommand, object, PropertiesForUpdate)`, `HandleCalculatableAdded(ICalculatableContainer, ICalculatable)`, `HandleCalculatableRemoved(ICalculatable)`, `HandleCalculatableMoved(ICalculatable)`, `OnToggleHideFromCalculationContext(object, RoutedEventArgs)`, `FirePropertyChanged(PropertyChangedEventArgs)`, `ContainsError(EngineError)`, `ShowError(EngineError)`, `GetBlockEquationTypeAttributeName(IWhiteboardItem)` (abstract), `Activate(bool)`, `NavigateIntoItem(Direction)`, `SimulateMouseClick(Point, int)`, `CanToggleInputRegion()`, `CanToggleOutputRegion()`, `ToggleInputRegion()`, `ToggleOutputRegion()`, `CanInsertVerticalSpace(object, CanExecuteRoutedEventArgs)`, `CanDeleteVerticalSpaceAbove(object, CanExecuteRoutedEventArgs)`, `CanDeleteVerticalSpaceBelow(object, CanExecuteRoutedEventArgs)`, `InsertVerticalSpace(object, ExecutedRoutedEventArgs)`, `DeleteVerticalSpaceAbove(object, ExecutedRoutedEventArgs)`, `DeleteVerticalSpaceBelow(object, ExecutedRoutedEventArgs)`, `FireOnGotFocus()`, `ShouldSerializeContent()`, `GetInternalHelpContext()` (abstract), `GetHelpContext()`, `OnIsInPrintingPropertyChanged(DependencyObject, DependencyPropertyChangedEventArgs)`, `HasItemInViewPort(Rect)`, `IsItemInViewPort(IWhiteboardItemManager, IWhiteboardItem, Rect, out Rect)`
  - Closures:
    - `<>c`: `<.ctor>b__23_0`, `_1`, `_2`, `_3`, `_4`, `_5`
    - `<BindCommands>b__28_0`
    - `<>c__DisplayClass109_0`: `<HasItemInViewPort>b__0`
    - `<>c__DisplayClass109_1`: `<HasItemInViewPort>b__1`
- Hand-written: `Mirrored/Controls.Mathboard.MathboardControl.cs`.
- Stubs removed: `Generated/Mirror/Controls.Mathboard.MathboardControl.g.cs`.
- Shapes:
  - Exact source order matching `Ptc.Controls.Mathboard.MathboardControl.members.tsv`, placing `.ctor` after `HideFromCalculationButtonChecked` to match lambda ordinal `<.ctor>b__23_*` and `BindCommands` to match `<BindCommands>b__28_0`, while preserving `<>c__DisplayClass109_*` for `HasItemInViewPort`.
  - Sequential guards `if (this != e.Source) return; RoutedUICommand routedUICommand = e.Command as RoutedUICommand; if (routedUICommand == null) return;` matching `beq.s IL_next; ret; ... brtrue.s IL_body; ret;`.
  - Unary negation `-itemRect.X` and `-itemRect.Y` in `IsItemInViewPort` avoiding `0.0` double constant load.
  - Early return `if (d is not MathboardControl mathboardControl) return;` in `OnIsInPrintingPropertyChanged`.
- Release IL: PASS — 709 checked, 0 failures, 133 unfinished (`release-il-d01b30b9a937`).
- Remaining mismatches in this batch: none.

## Batch 19 — WhiteboardControl

- Members:
  - `WhiteboardControl`:
    - Fields: `DeleteEmptySpaceDefaultLinesCount`, `DeleteIfEmptyProperty`, `DragDropManagerProperty`, `IsActiveProperty`, `ChangedEvent`, `InsertionPointPropertyName`, `WhiteboardItemsForXamlSerializationPropertyName`, `_snappingHelper`
    - Constructors: `.cctor()`, `.ctor(IWhiteboardViewModel)`
    - Properties: `WhiteboardItemManager` (get), `IWhiteboardItemContainer.ContainerItemManager` (get), `IHasChildArrangeInfoProvider.ChildArrangeInfoProvider` (get), `ViewModel` (protected get/set), `IsTabNavigatable` (get), `GridPosition` (get/set), `InsertionPoint` (get/set), `DefaultInsertionPoint` (get), `IsCrosshairVisible` (get/set), `ActiveItem` (get/set), `ActiveOrSelectedItems` (get), `ActiveDescendant` (get), `SelectedDescendants` (get), `WhiteboardItems` (get), `WhiteboardItemsForXamlSerialization` (virtual get/set), `GridCellSize` (get/set), `CanAddItems` (get), `SelectedItems` (get), `InsertionPointForResize` (get), `ForbidGrowingMovementRight` (set), `HasRegionsWithConverterAnnotations` (get), `RegionsWithConverterAnnotations` (get), `TargetMathFormat` (get), `TargetLabeledIdFormat` (get), `SnappingHelper` (get)
    - Events: `RequestActivatePartContainingChild`, `RequestScrollTo`, `DragSelectBeyondBorder`, `ChildTriggeredResize`, `RequestMaximumSize`, `Activated`
    - Methods: `RecursivelyEnumerateDescendantItemssBypassWhiteboard()`, `CanSeparateOverlappedItems(Dimension)`, `SeparateOverlappedItems(Dimension)`, `HandleIsSelectedPropertyChanged(DependencyObject, DependencyPropertyChangedEventArgs)`, `SetIsSelected(bool)`, `GetIsSelected(UIElement)`, `SetIsSelected(UIElement, bool)`, `GetIsActive(UIElement)`, `SetIsActive(UIElement, bool)`, `OnIsInCopyPropertyChanged(DependencyObject, DependencyPropertyChangedEventArgs)`, `HookupCustomEventHandlers()`, `BringIntoView(IEnumerable<IWhiteboardItem>)`, `FireCopyWhiteboardItems(IEnumerable<IWhiteboardItem>)`, `HandleRequestScrollTo(IWhiteboardControl, Rect)`, `FireChanged(object, EventArgs)`, `ChildCausedResizeHandler(Size, Size)`, `RequestMaximumSizeHandler()`, `OnTraversedFocusEvent(object, CustomRoutedEventArgs<FocusNavigationDirection>)`, `SetupBindings()`, `CopySelection()`, `CutSelection()`, `TabInto(FocusNavigationDirection)`, `PasteItems(IDictionary<IWhiteboardItem, Point>)`, `OnDragSelectBeyondBorder(Rect)`, `ViewModelPropertyChangedHandler(object, PropertyChangedEventArgs)`, `GetItemRect(IWhiteboardItem)`, `GetItemLocation(IWhiteboardItem)`, `SetItemLocation(IWhiteboardItem, Point)`, `GetItemGridPosition(IWhiteboardItem)`, `ActivateWhiteboard(bool)`, `SetInsertionPointWhenLoaded(Point)`, `SetItemGridPosition(IWhiteboardItem, Point)`, `AddItem(IWhiteboardItem)`, `AddItems(Dictionary<IWhiteboardItem, Point>)`, `RemoveItem(IWhiteboardItem)`, `AddItemAtLocation(IWhiteboardItem, Point, bool)`, `ToggleItemSelection(IWhiteboardItem, bool)`, `SelectContiguous(IWhiteboardItem)`, `ClearSelection()`, `SelectItems(IEnumerable<IWhiteboardItem>)`, `UnSelectItems(IEnumerable<IWhiteboardItem>)`, `SelectItem(IWhiteboardItem)`, `UnSelectItem(IWhiteboardItem)`, `SaveVisualState()`, `RestoreVisualState()`, `ClearVisualState()`, `SaveResizeInformation()`, `RestoreResizeInformation()`, `GridCellSizeChangedHandler(Size, Size)`, `NavigateInto(Direction, Point)`, `UpdateSizeLimits(Size)`, `GetNavigationStartLocation(Direction)`, `SelectAllItems()`, `GetDescendantInsertionPoint()`, `FindOncePrivate(string, bool, LogicalDirection, FindOptions, List<Pair<int, int>>)`, `HighlightAll(string, FindOptions)`, `CanInsertVerticalSpace(object, CanExecuteRoutedEventArgs)`, `CanDeleteVerticalSpaceAbove(object, CanExecuteRoutedEventArgs)`, `CanDeleteVerticalSpaceBelow(object, CanExecuteRoutedEventArgs)`, `InsertVerticalSpace(object, ExecutedRoutedEventArgs)`, `DeleteVerticalSpaceAbove(object, ExecutedRoutedEventArgs)`, `DeleteVerticalSpaceBelow(object, ExecutedRoutedEventArgs)`, `GetImeWindowPosition()`, `Dispose()`
  - Closures:
    - `<>c`: `<.cctor>b__41_0`, `<.ctor>b__43_0`, `<.ctor>b__43_1`, `<.ctor>b__43_2`, `<.ctor>b__43_3`
    - `<>c__DisplayClass42_0`: `<OnIsInCopyPropertyChanged>b__0`, `<OnIsInCopyPropertyChanged>b__1`
    - Instance lambdas: `<HookupCustomEventHandlers>b__44_0`, `<HookupCustomEventHandlers>b__44_1`, `<ActivateWhiteboard>b__100_0`
    - `<>c__DisplayClass101_0`: `<SetInsertionPointWhenLoaded>b__0`
    - `<>c__DisplayClass123_0`: `<GridCellSizeChangedHandler>b__0`
- Hand-written: `Mirrored/Controls.Whiteboard.WhiteboardControl.cs`.
- Stubs removed: `Generated/Mirror/Controls.Whiteboard.WhiteboardControl.g.cs`.
- Shapes:
  - Exact member declaration ordering matching Roslyn syntax ordinals `<.cctor>b__41_0`, `<>c__DisplayClass42_0`, `<.ctor>b__43_*`, `<HookupCustomEventHandlers>b__44_*`, `<ActivateWhiteboard>b__100_0`, `<>c__DisplayClass101_0`, and `<>c__DisplayClass123_0`.
  - Field placement: `ChangedEvent`, `InsertionPointPropertyName`, and `WhiteboardItemsForXamlSerializationPropertyName` declared in the middle of the class before their consuming members to match the 41/59/23 ordinal interval structure; `_snappingHelper` declared immediately preceding `SnappingHelper`.
  - `BringIntoView` early return on `items.IsNullOrEmpty()` emitting `brfalse.s; ret`.
  - `HandleRequestScrollTo` guard as `control as IWhiteboardItem; if (item == null) return;` emitting `brtrue.s; ret`.
  - Property metadata overrides in `.cctor` without extra `PropertyMetadata` casts.
  - Direct `this` references passed to `SerializationHelperUtils.IsConvertedFromMathcad14` and `MathFormatDobjController`.
- Release IL: PASS — 824 checked, 0 failures, 175 unfinished (`release-il-e0c1ee32981f`).
- Remaining mismatches in this batch: none.









## Batch 20 — WhiteboardItemsSeparator constructor, CanSeparate

- Members: `WhiteboardItemsSeparator`: `.ctor(IEnumerable<IWhiteboardItem>, Dimension, IAlignmentGrid, IWhiteboardItemManager)`, `CanSeparate()`, plus the six private fields.
- Skipped: `Separate()` (the coordinator's task note marks `WhiteboardItemsSeparator.Separate` as deferred; it stays a generated stub).
- Hand-written: `Mirrored/Controls.Whiteboard.WhiteboardItemsSeparator.cs`; the class is now `partial` in both files.
- Stubs removed: fields, constructor and `CanSeparate` from `Generated/Mirror/Controls.Whiteboard.WhiteboardItemsSeparator.g.cs`.
- Shapes: `_oldItemRectMap`/`_newItemRectMap` use field initializers (IL stores them before the base call).
- Release IL: PASS — 826 checked, 0 failures (`release-il-606dee4f3d1b`). First run `release-il-efe12761865e` failed with MSBuild `MSB4166` child-node exits and no C# errors; the rerun passed unchanged.
- Remaining mismatches in this batch: none.

## Batch 21 — WhiteboardViewModel.WhiteboardItemMgr, CanAddItems

- Members: `WhiteboardViewModel`: static `WhiteboardItemMgr` get (`AppServiceRegistry.GetService<IWhiteboardItemManager>()` with the `IsConnected` re-fetch), `CanAddItems` get.
- Hand-written: `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs` (`using Quire.Services;` added). The stubs were placeholders in that file, so nothing was removed from `Generated/`.
- Shapes: `CanAddItems` uses the `is IWhiteboardItemContainer` pattern with a local; `SelectedItems.Count()` binds to the instance `INotifyCollectionRangeChanged.Count()` as Mathcad does.
- Rejected shape: `IsTabNavigatable => WhiteboardItems.Any(item => WhiteboardItemMgr.IsTabNavigatable(item))` produced the exact IL, but its lambda is `<get_IsTabNavigatable>b__230_0` where Mathcad has `b__236_0`. The class has six fewer preceding member declarations than Mathcad, so this needs placeholder members in Mathcad order first. It stays a stub.
- Release IL: PASS — 828 checked, 0 failures (`release-il-7f8a8eaf6c19`).
- Remaining mismatches in this batch: none. Blocked and skipped: `DropTargetDrop` (recorded devirtualization blocker, batch 14), `Separate` (deferred).

## Batch 22 — WhiteboardViewModel plain members (no closures)

- Members: `HandleChildSizeChangedUnitTestWrapper`, `UpdateSizeLimits`, `ActivateActiveItem`, `HandleChildItemGotFocus`, `IsValidInsertionPoint`, `DeleteActiveItem`, `HandleItemMoved`.
- Hand-written in `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs` (placeholder stubs in place; nothing removed from `Generated/`).
- Shapes: `ActivateActiveItem` uses the guard `if (ActiveItem == null) return;` (Mathcad `brtrue; ret`), reading `ActiveItem` again for each call; the other members are direct.
- Skipped (closure-ordinal or devirtualization): `IsTabNavigatable` (ordinal), the `new XCommand(...).Execute()` members (devirtualization blocker from batch 14).
- Release IL: PASS — 835 checked, 0 failures (`release-il-ad06e789ccfe`).

## Batch 23 — WhiteboardViewModel plain members (no closures)

- Members: `RemoveWhiteboardItem`, `GetItemAtLocation` (private, collection-initializer dictionary), `AddItemAtLocation`, `SetItemLocation`, `HandleTabOutOfText`, `CanTraverse`.
- Hand-written in `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs`; placeholder stubs replaced, nothing in `Generated/` changed.
- Shapes: `HandleTabOutOfText` uses the guard `if (!CanTraverse(...)) return;` (Mathcad `brtrue; ret`). The first version, with the body nested under the `if`, gave the wrong branch layout.
- Release IL: PASS — 841 checked, 0 failures (`release-il-010a8aa8a15f`).

## Batch 24 — WhiteboardViewModel.CanMove

- Member: `CanMove(Direction)` (protected virtual), direct form with the guard `if (!HandleUserInput) return false;`.
- Hand-written in `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs`; placeholder stub replaced.
- Blocked and left as stubs in this batch:
  - `DeleteItemIfNeeded`: the Mathcad body keeps the `RemoveItemsCommand` on the stack with `dup` (set property, then `Execute`, then `pop`). The idiomatic local form emits `stloc`/`ldloc`, so it does not match.
  - `IsValidLocation`: the Mathcad IL calls `GetItemIgnoreForHorizontalMovementAndWidth` with `call` (non-virtual), but the candidate declares it `public virtual` and implements `IWhiteboardViewModel` with it. Changing that flag is an interface/ownership decision, so it is skipped.
- Release IL: PASS — 842 checked, 0 failures (`release-il-f291ef01626c`).

## Batch 25 — WhiteboardViewModel plain members

- Members: `SaveState`, `GetDescendantInsertionPoint` (pattern `is IWhiteboardControl` with `return default;`), `HandleChildRequestRemove`, `DirectionalMoveHandler(Direction)` (calls the two-argument overload, still a stub), `ItemsOverlapped`.
- Hand-written in `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs`; placeholder stubs replaced.
- Skipped in this group: `HandleChildSizeChanged` (raises the `ChildCausedResize` event and compares `Size` values, shape not yet checked), `HandleCarriageReturn` and `DeleteSelectedItems`/`DeleteVerticalSpaceAbove`/`Below` (`new XCommand(...).Execute()` devirtualization blocker).
- Release IL: PASS — 847 checked, 0 failures (`release-il-5903c6f14cc8`).

## Batch 26 — WhiteboardViewModel plain members

- Members: `GetRemoveItemsCommand`, `GetPasteItemsCommand` (virtual, `new` returned, no call on the new object), `SelectedItemsAddNew`, `GetNextIntersectionForMove`, `HandleTraverse`, `UpdateSizeLimit`, `Paste`, `CopySelection`, `ChildWhiteboardPropertyChangedHandler`, `TrySelectRegionsInBounds`.
- Hand-written in `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs`; placeholder stubs replaced.
- Note: `ChildWhiteboardPropertyChangedHandler` needs `global::System.ComponentModel.PropertyChangedEventArgs`; the short name is ambiguous with the generic `PropertyChangedEventArgs<T>`.
- Gate note: the first run failed only on `AggregateMathboard` `ICalculatable` members, because the worktree was behind `main` (`b63122fbe`/`a1717832d`). A fast-forward of the worktree cleared it; no member change caused it.
- Release IL: PASS — 857 checked, 0 failures (`release-il-a045db3a8f24`).

## Batch 27 — WhiteboardViewModel selection, arrange and caret members

- Members: `SelectRegionsInBounds`, `HandleChildSizeChanged`, `ArrangeOverride`, `GetNavigationStartLocation`, `HandleCaretLeavesTextArea`.
- Hand-written in `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs` (`using System.Windows.Media;` added for `TranslateTransform`); placeholder stubs replaced.
- Shapes that Release IL requires:
  - `GetNavigationStartLocation` negates the offset (`-ActiveItem.VerticalAlignmentOffset`); `0.0 - x` loads an extra `0.0` constant.
  - `ArrangeOverride` enumerates the copy through `(IEnumerable<IWhiteboardItem>)` (interface call sites), not the `List<T>` enumerator.
  - `HandleCaretLeavesTextArea` returns early with `if (flag) return;` (Mathcad `brfalse` to the body; `ret`).
- Release IL: PASS — 862 checked, 0 failures (`release-il-045cf01c884b`).

## Batch 28 — WhiteboardViewModel mouse no-op handlers, sorting, resize notify

- Members: `PreviewMouseRightButtonDownHandler`, `PreviewMouseRightButtonUpHandler`, `PreviewMouseLeftButtonUpHandler` (empty bodies), `SimulateItemClickAndFireActivated`, `SortItemsByLocation`, `CreateSeparateOverlappedItemsCommand` (returns `new`), `ResizeAndNotify`.
- Hand-written in `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs`; placeholder stubs replaced.
- Skipped: `PreviewMouseLeftButtonDownHandler` (large pattern-matching body, not yet shaped), `InsertVerticalSpace` (`new InsertSpaceCommand(...).Execute()`, devirtualization blocker), `FindSizeLimit` (closure), `HandleMouseUp`, `NavigateInto`, `SelectedItemsChangedHandler`, `HandleItemsListChanged`, `FindMinimumSize`, `RecursivelyEnumerate*` (iterators).
- Release IL: PASS — 869 checked, 0 failures (`release-il-437cf4bdc018`).

## Batch 29 — WhiteboardViewModel HandleMouseUp, HandleItemsListChanged, SelectedItemsChangedHandler

- Members: `HandleMouseUp` (sets `CurrentDragMode`, returns false), `HandleItemsListChanged` (switch with `Each` method groups and `FailAssertion` default), `SelectedItemsChangedHandler` (large switch; `Each(WhiteboardItemMgr.SelectItem)` method groups).
- Hand-written in `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs` (`using System.Collections.Specialized;` added); placeholder stubs replaced.
- Skipped: `FindMinimumSize` (decompiled temporaries, not shaped yet), `NavigateInto` (`new TranslateTransform(...).Transform`, devirtualization risk), `PreviewMouseLeftButtonDownHandler`, `FindSizeLimit` (closure), `RecursivelyEnumerate*` (iterators).
- Release IL: PASS — 872 checked, 0 failures (`release-il-1a8d7d1a12d6`).

## Batch 30 — WhiteboardViewModel.PreviewMouseLeftButtonDownHandler

- Member: `PreviewMouseLeftButtonDownHandler(object, MouseButtonEventArgs)`, with `as DependencyObject` casts (Mathcad `isinst`; the `is ... ? x : null` form compiles to `castclass`), `((int)Keyboard.Modifiers & n)` (ModifierKeys to int), and the `IIncludeWorksheetControl` pattern from `Quire.Controls.Include`.
- Hand-written in `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs`; placeholder stub replaced.
- Not landed (still stubs), with the reason:
  - `FindMinimumSize`: structure matches; local slot numbering differs (Mathcad stores `Size val` in local 0, ours in local 2). Not resolved.
  - `NavigateInto`: the first differing instruction is a branch offset (`brtrue.s` target off by one byte) before the body; the source of the size difference is not located.
  - `FindSizeLimit` and `RecursivelyEnumerate*`: closure or iterator state-machine ordinals (skipped as ordinal-dependent).
- Release IL: PASS — 873 checked, 0 failures (`release-il-2fa6f6b87089`).
