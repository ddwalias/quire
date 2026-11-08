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
