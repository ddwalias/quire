# Lane Whiteboard — Quire.Controls.Whiteboard

- Owner: Orca task `task_5da5143048d1` (maintainer-assigned; no WORK.md lane row).
- Branch: `mirror/whiteboard-20261108`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/whiteboard-20261108-b`.
- Baseline (main `5c997717`): not measured before batch 1; the batch 1 report counts 926 unfinished after the change.

## Batch 1 — small leaf types

- Members: `WhiteboardItemPositionComparer` (`.ctor`, `Compare`), `WhiteboardElementXamlHelper` (`.ctor`, `GetPrintView`), `ItemsControlCanvas` (`.ctor`, `ArrangeOverride`), `ProductInfo.Constants` (`.ctor`; `ConverterProductName`, `ConverterExe` as `const`).
- Hand-written: `src/Quire.Controls.Whiteboard/Mirrored/*.cs`; stubs removed from the matching `Generated/Mirror/*.g.cs`.
- Shapes: `Compare` as the decompiled `if (a == b) return 0;` form (matched first time). `GetPrintView` as `if (element is not IWhiteboardControl) return null;` then `as` + null test (matched first time). `ArrangeOverride` as `as` + `!= null` (matched first time).
- Release IL: PASS — 82 checked, 82 exact, 0 failures, 926 unfinished after this batch (`release-il-59487b3819e9`).
- Remaining mismatches in this batch: none.

## Batch 2 — SectionIndicatorInfo

- Members: `.ctor` x2, `Readonly` (auto, private set), `Text`, `Top`, `Height`, `MinHeightRatio` (get/set), `PropertyChanged` (`= delegate { }`), `FirePropertyChanged`; fields `_text`, `_top`, `_height`, `_minHeightRatio`.
- Hand-written: `Mirrored/Controls.Mathboard.SectionIndicatorInfo.cs`; stubs removed from its `.g.cs`.
- Shapes: first try declared the event and `_minHeightRatio` in source order → gate failed only on closure names (`<.ctor>b__24_0` vs Mathcad `<.ctor>b__4_0`). Lambda names use the containing constructor's ordinal, which counts members declared before it (fields included, auto-property backing fields not). Moving the ctors after `_text/_top/_height` and the event after the properties matched first time.
- Release IL: PASS — 99 checked, 0 failures (`release-il-af69fc44bf5e`).
- Remaining mismatches in this batch: none.

## Batch 3 — interface declarations

- Members: `IWhiteboardItemsSeparator` (`CanSeparate`, `Separate`); `ISectionIndicatorInfo` (`Text`, `Top`, `Height` get-only).
- Hand-written: `Mirrored/Controls.Whiteboard.IWhiteboardItemsSeparator.cs`, `Mirrored/Controls.Mathboard.ISectionIndicatorInfo.cs` (abstract declarations, no bodies); stubs removed from the `.g.cs` interfaces.
- Release IL: PASS — 104 checked, 0 failures (`release-il-bce29b42fd10`).
- Remaining mismatches in this batch: none.

## Batch 4 — interface declarations

- Members: `IVerticalWhiteboardSections` (`BlockSectionCount`, `SectionsMaybeChanged` event, `GetBlockSectionName`, `GetBlockSectionItems`, `GetBlockSectionMinHeightRatio`); `IUIElementManager` (nine abstract methods).
- Hand-written: `Mirrored/Controls.Mathboard.IVerticalWhiteboardSections.cs`, `Mirrored/Controls.Whiteboard.IUIElementManager.cs`; stubs removed from the `.g.cs` interfaces.
- Release IL: PASS — 114 checked, 0 failures (`release-il-ce8278883795`).
- Remaining mismatches in this batch: none.

## Batch 5 — aggregate mathboard interface declarations

- Members: `IAggregateMathboardViewModel` (`SectionIndicators` get-only, `HideFromCalculationContext` get/set, `MeasureOverrideHelper`); `IAggregateMathboardControl` (`SectionIndicators`, `IEnumerable<ISectionIndicatorInfo>`).
- Hand-written: `Mirrored/Controls.Mathboard.IAggregateMathboardViewModel.cs`, `Mirrored/Controls.Mathboard.IAggregateMathboardControl.cs`; stubs removed from the `.g.cs` interfaces.
- Release IL: PASS — 104 checked, 0 failures; unfinished 888 (`release-il-1a10f861c49c`). The checked list is the changed-member set, so the interface declarations appear there only as changes.
- Remaining mismatches in this batch: none.

## Batch 6 — WhiteboardViewModel accessors

- Members: `IsFocused`, `IsCrosshairVisible` (virtual), `ForbidGrowingMovementRight` (auto-property), `CanPasteSelection`, `CanInsertVerticalSpace` (virtual). Hand-written in `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs`; stubs removed from the `.g.cs`. Fields stay in the `.g.cs`.
- Shapes: `OnPropertyChanged("IsFocused")` with an explicit name, because the class's override takes a required parameter. `OnPropertyChanged()` through CallerMemberName failed to compile, not the gate.
- Release IL: PASS — 107 checked, 0 failures; 880 unfinished (`release-il-4f0098591619`).
- Note for later closure members: a lambda's `b__N_M` ordinal counts every member before it in compile order (generated partials first), so moving a stub into the hand-written partial changes the ordinal of later lambda-containing members. Lambda members are left for a placeholder-ordering pass.

## Batch 7 — WhiteboardViewModel property accessors

- Members: `WhiteboardItems`, `OverRightMargin` (auto), `InsertionPoint` (set through `SetInsertionPoint`), `HasSelectedItems` (protected), `ActiveDescendant`, `Grid`, `IsSelected` (private get, auto), `DefaultInsertionPoint` (virtual), `ContentSize`, `ContentHeight`/`ContentWidth` (set through the virtual `SetContent*`), `CurrentDragMode` (auto), `GridPosition`, `HandleUserInput` (protected set, auto).
- Shapes: `ActiveDescendant` as `as` tests with `??` fallbacks matched first time. Non-virtual declarations (no `virtual` keyword) emit `call` in Mathcad's IL even though the interface accessor is `final virtual`; keeping the `.g.cs` modifiers matched.
- Release IL: PASS — 130 checked, 0 failures (`release-il-d3e4b7d8135c`).
- Lambda-containing members (`Initialize`, `IsTabNavigatable`, command factories) are not started.

## Batch 8 — WhiteboardViewModel size limits and selection accessors

- Members: `SelectedItems`, `ActiveOrSelectedItems`, `SelectedDescendants`, `InsertionPointForResize` (private set, auto), `MinimumSize`, `MinimumWidth`, `MinimumHeight`, `StrictMinimumSize` (protected virtual), `MaximumSize`, `MaximumWidth`, `MaximumHeight`.
- Shapes: `MinimumWidth` setter as `Math.Max(value > MaximumSize.Width ? MaximumSize.Width : value, StrictMinimumSize.Width)` (the IL uses the ternary and `Math.Max`, not `Math.Min`); `MinimumHeight` as `Math.Max(value, StrictMinimumSize.Height)`. Both matched first time.
- Release IL: PASS — 148 checked, 0 failures (`release-il-11d2535cc461`).

## Batch 9 — WhiteboardViewModel active item and content size

- Members: `ActiveItem` (get/set, virtual), `FirstSelectedItem` (protected auto), `SetContentHeight`/`SetContentWidth` (protected virtual), `FireActivated` (`Activated?.Invoke()`), `GetItemIsInPasting`, `GetItemGridPosition`, `GetItemIgnoreForHorizontalMovementAndWidth`.
- Shapes: `SetContent*` as `Math.Max(MinimumX, value)` into a local, then the `IsEqualTo` early return matched first time. `ActiveItem`'s setter as the sequence of early returns, the `value != null && !Contains` throw, and the nested `if` blocks matched first time, including `Rect itemRect` for the `Contains` test.
- Release IL: PASS — 158 checked, 0 failures (`release-il-6b3aabd7c0ca`).

## Batch 10 — WhiteboardViewModel selection, delete and item geometry helpers

- Members: `GetItemLocation`, `GetItemDesiredSize`, `GetItemRect` (virtual/non-virtual as in the `.g.cs`), `GetItemBottom`, `GetItemTop`, `GetItemRightEdge`, `SelectAllItems`, `HandleSelectAll`, `SelectItem`, `UnSelectItem`, `CanDelete`, `CanBackspace`, `CanCopySelection`, `CanCutSelection`, `HandleBackspace`, `HandleDelete`.
- Shapes tried: `CanDelete` / `CanBackspace` as `if (SelectedItems.Any()) return true; return CanDeleteVerticalSpace…;` → "Release IL differs" (Mathcad branches to a shared `ldc.i4.1; ret` on `brtrue`). `return SelectedItems.Any() || CanDeleteVerticalSpace…;` → exact. `HandleBackspace`/`HandleDelete` as `if (…) { Delete…(); return; } DeleteVerticalSpace…(1);` matched first time.
- Release IL: PASS — 174 checked, 0 failures (`release-il-a984642d6bf5`).
- Batch 9 landed on the second try: the first `lane_merge` run failed on a base-build crash (MSBuild child nodes exited prematurely, free RAM about 6 GB, no disk error); the retry passed (`c80e837f`).

## Batch 11 — WhiteboardViewModel event firing, saved state and activation

- Members: `FireChildCausedResize`, `FireRequestMaximumSize`, `FireChanged`, `ClearSavedState`, `ClearSelectedItems`, `ClearState`, `UnSelectItems`, `ToggleItemSelection`, `Activate`, `DeactivateActiveItem`, `SetItemSize` (optional `DragMode` default), `SetItemLocationToNearestGridIntersection`.
- Shapes: `FireRequestMaximumSize` as a direct call (the IL has no null test); `FireChanged` as `if (Changed != null) Changed(this, EventArgs.Empty);` after the `HandleUserInput` return. `ToggleItemSelection` and `Activate` matched first time with the `if / else if / else` and `if (A && B) … else …` shapes.
- Not started: `AddItemAndActivate`, `RemoveWhiteboardItem`, `SetItemGridPosition` (they build nested command types, `AddItemsCommand`, `RemoveItemsCommand`, `MoveItemsCommand`).
- Release IL: PASS — 186 checked, 0 failures (`release-il-072e5205fe42`).

## Batch 12 — WhiteboardViewModel resize info, cut and delete guards

- Members: `ActivateItem`, `BoundsOutsideContentSize`, `CutSelection`, `SaveResizeInformation`, `RestoreResizeInformation`, `DeleteActiveOrSelectedItems`, `CanDeleteActiveItem`, `CanDeleteSelectedItems`, `CanDeleteActiveOrSelectedItems`.
- Shape tried: `BoundsOutsideContentSize` as `return a || b || c || d;` → "conditional branches: Quire 3, Mathcad 4". `if (a || b || c || d) { return true; } return false;` → exact.
- Release IL: PASS — 195 checked, 0 failures (`release-il-8a1bffbaa875`).
- Not started: `DeleteSelectedItems`, `DeleteActiveItem` (nested `RemoveSelectedItemsCommand`, `RemoveItemsCommand`).

## Batch 13 — WhiteboardViewModel members in Mathcad order (placeholders)

- Moved every WhiteboardViewModel member (fields, properties, events, methods, constructors) from the `.g.cs` into `Mirrored/Controls.Whiteboard.WhiteboardViewModel.cs`, in the reconstructed Mathcad source order. Unfinished members are unmarked placeholders (`=> throw NotImplemented`). Nested command types stay in the `.g.cs`.
- Order reconstruction: each Release IL closure name `<Method>b__N_M` gives the member count before its containing method. Counting method bodies in metadata order, the auto-property backing fields, each property (1), each field and each event's backing field, that count matched all 26 closure constraints when auto-property backing fields and events count (backing field + event) except `Changed` (1). A plain "fields first" layout failed at `GetInsertionPointAfterCarriageReturn` (86 vs 85).
- Explicit `IWhiteboardViewModel.CommandBindings` (`=> base.CommandBindings`) added here and to `MathboardViewModel` and `AggregateMathboardViewModel`, because the derived classes lost their compiler-generated re-implementation bridges when the base declared one.
- Release IL: PASS — 195 checked, 0 failures (`release-il-f1055934f2c1`). No lambda method is implemented yet, so the closure ordinals are not yet exercised.

## Batch 14 — WhiteboardViewModel movement and traversal commands

- Members: `MoveDownCommand`, `MoveUpCommand`, `MoveRightCommand`, `MoveLeftCommand`, `CarriageReturnCommand`, `TraverseForwardCommand`, `TraverseBackwardCommand` (lazy `RelayCommand` getters with two lambdas each, or one for carriage return).
- Closure ordinals verified: `<get_MoveDownCommand>b__10_0` and the others match Mathcad after the batch-13 reordering.
- Finding: nested types declared in the `.g.cs` are counted as members for Roslyn's closure ordinal (they compile first). Moved them to the end of the hand-written partial so they do not shift the earlier closures. Their own closures are unaffected.
- Release IL: PASS — 215 checked, 0 failures (`release-il-5a9efd6b8a04`).
- Still placeholders: the other lambda-containing methods (`Initialize`, `SetInsertionPoint`, `HookupActiveItem`, `SelectItems`, `TabInto`, `get_IsTabNavigatable`, and others).
