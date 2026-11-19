# Lane Quire root, plain files (MathcadPrime.dll) — Quire

- Owner: Orca task `task_87291e5b13e6`.
- Branch: `mirror/quire-root-plain-haiku-20261119`, from main `74e9462c4`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/quire-root-plain-haiku`.
- Set: generated stubs in `src/Quire/Generated/Mirror` from `AboutWindow` to `DropDownSelector`, non-Automation, whose file has no `InitializeComponent` (19 files). Parked by a prior lane and kept as stubs: CompositSymbolDescriptorCollection, CMathcadPrimeWorksheetReadonlyOptions, ButtonDropDownSelection, DimensionEditDialogViewModel.

## Batch 1 — CMathcadPrimeWorksheets (`ef1e25385`)

- Members: field `_isUiInteractiveModeDisabled`, `.ctor`, `Count`, `DoGetCount`, `Item`, `DoGetOpenItemByIndex`, and the `<Item>b__0` closure (through the `try` return shape).
- Release IL: PASS (`--type Quire.CMathcadPrimeWorksheets`), 799 checked, 0 failures.

## Batch 2 — UIFunctionalityWithoutCommandAttachedBehavior (`dd012ed6f`)

- Members: field `_uiElementBehaviorKey`, `FunctionalityKey` get/set, `OnFunctionalityServicePropertyChanged`, `.ctor`.
- Parked: `ButtonDropDownOnPreviewOpen` — `AssociatedObject` resolves to `Microsoft.Xaml.Behaviors.Behavior`1` in Quire, but Mathcad calls `System.Windows.Interactivity.Behavior`1`; the base class is in `Behaviors.BaseUIFunctionalityAttachedBehavior.g.cs` (not this lane's file).
- Parked: `SetAndUpdateElementBehavior`, `OnLoaded` — `DevComponents.WpfRibbon.ButtonDropDown` (`isinst`) is not referenced by Quire; the Fluent `DropDownButton` has no `Role`, `IsCheckable`, `OptionGroup`, `Image`.
- Parked: `FunctionalityKeyProperty` and `.cctor` (WPF DependencyProperty static).
- Release IL: the three changed members are not in the failure list (805 checked); the remaining failures are stubs.

## Batch 3 — UIFunctionalityWithCommandAttachedBehavior (`b346d5a42`)

- Members: `AssociatedObjectAsCommandSource` (auto-property), `CommandTarget` get/set, `ShouldReplaceToFreemiumIcon`, `OnCommandTargetChanged`, `.ctor`.
- Parked: `OnLoaded` (closure `<OnLoaded>b__0`, and the `Behavior`1` base mismatch above), `UpdateElementBehavior` (`DevComponents.WpfRibbon.ButtonDropDown` isinst), `CommandTargetProperty` and `.cctor`.
- Release IL: the five changed members are not in the failure list (810 checked).

## Batch 4 — ScrollViewerGrid (`880152f40`)

- Members: `_scrollViewer`, `_clippingRectangle`, `_scrollViewerToolTipHeader` fields; `EngineeringDocumentInstance` (auto-property, private setter); `ScrollViewerToolTipHeader` (`??=` resource lookup); `Worksheet`; `.ctor`.
- Parked: `SetTooltipAndMinScrollbarThumbSize`, `UpdateEngineeringDocument`, `OnStartingPageNumberChanged`, `StopUpdates`, `OnScrollChanged`, `UpdatePageNumbersTooltip`, `OnPagesCollectionChanged` (not started in this batch).
- Release IL: the five changed members are not in the failure list (820 checked).

## Remaining

Not started: Controls.FlexingStackPanel, Behaviors.BaseUIFunctionalityAttachedBehavior, Behaviors.AllFunctionsBrowserTreeViewUIFunctionalityAttachedBehavior, App, AppCommands, AddonsProperties, AddonsControls, Addon, AddonsCollection, CMathcadPrimeApplication, CMathcadPrimeWorksheet.
