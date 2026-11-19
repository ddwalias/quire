# Lane Quire root part 3 (b3, second half) — Quire

- Owner: Orca task `task_c6708c7d45f5` (dispatched worker).
- Branch: `mirror/quire-root-b3-haiku-20261119` from `main` (`7277247f3`); worktree `C:/Users/DuyDang/orca/workspaces/Quire/quire-root-b3-haiku`.
- Scope: stub files under `src/Quire/Generated/Mirror` from `ProductInfo.Constants.g.cs` through `Zoom.g.cs` that still contained `MirrorStub` (32 files, about 2,500 stub lines).

## Batch 14 — ProofingLanguage, ScalarStateExtensions, ResourceProviderService, UniqueTextStyleNameRule

- ProofingLanguageDataItem: fields `_culture`, `_isChecked`; `GetCultureName`.
- ProofingLanguageDropDownSelector: `NullLanguage`, `EmptyLanguage`, `_itemsManager`, `CultureItemsManager._sortedCultures`, `EnsureItemHeader`. Shape: the `CultureInfo` local is declared before the item local, and the null check is a guard return, which matches Mathcad's slots and branches.
- ScalarStateExtensions: `GetOriginalScalarState`, `GetScalarState` (new hand-written partial).
- ResourceProviderService: `IsConnected`, `RichTextContextMenuItemsDictionary`, `.ctor`.
- UniqueTextStyleNameRule: `_helper` field.
- Commit `6bfcb6194`.

## Batch 15 — small converters, helpers, field holders

- TextStylePropertyToVisibility: `Convert`, `ConvertBack`, `.ctor`.
- TextStyleProperties: `ConvertBack`, `.ctor`.
- UniqueTextStyleNameHelper: `TextStyleNames`, `Validator`, `SetValidator`, `CreateInstanceCore`, `.ctor`.
- TabManagerItem: `WrappedItem`, `OnMouseDown`, `.ctor`.
- StickyDropDownSelector: `StickyValue`, `UpdateHeaderFromChildItem`, `SelectionChanged`, `.ctor`.
- SystemUnitEditDialogViewModel: fields `_currentUnitSystem`, `_customBaseUnitSystem`, `_owner`, `_isCustomDataChanged`, `_derivedDimSelectedItem`.
- TextStyleEditDialogViewModel: consts and fields.
- Commit `0b016273c`.

## Batch 16 — Properties.Settings

- All 38 settings properties (`this["..."]` accessors), `Default`, `.ctor`. The static `defaultInstance` field stays a stub (static constructor).
- Commit `c8805d466`.

## Batch 17 — Zoom

- Members: `ZoomValue`, `MinZoomValue`, `MaxZoomValue`, `BindCommands`, `ChangeZoomBySteps`, `ChangeZoom`, `slider_ValueChanged`, `slider_MouseDoubleClick`, `SliderValueToZoom`, `ZoomToSliderValue`, `DecreaseZoomLarge`, `IncreaseZoomLarge`; fields `displayedValue`, `decreaseZoomButton`, `slider`, `increaseZoomButton`; const `DefaultZoomValue`.
- Shapes: `SliderValueToZoom` and `ZoomToSliderValue` use unary minus (`neg`), not `0.0 - x`.

## Parked (named blockers, left as stubs)

- SavedDateIsCheckedConverter `.ctor`: Mathcad stores the dictionary before the base `Object` constructor call; C# cannot emit that order (carried from the first half of the split).
- StyleFontFamilyValidationRule `Validate`: shared failure return and `brtrue` after a `FirstOrDefault` closure (carried from the first half).
- UniqueTextStyleNameRule `Validate`: after `text != null &&`, Roslyn emits `callvirt` for `string.IndexOfAny` where Mathcad emits `call`, and the `brfalse`/`brtrue` shape differs. One attempt (idiomatic `&` form) failed.
- UniqueTextStyleNameHelper `TextStyleNamesProperty`, TabManagerItem `TabManagerItemClickedEvent`, StickyDropDownSelector `StickyValueProperty`, Zoom `ZoomValueProperty`, Zoom `ApplyZoomCommand`: static DependencyProperty/RoutedEvent/command fields with a static constructor.
- WindowTabTupleData `Header`: Mathcad reads `DockWindow.Header` (DevComponents); Quire's `Wnd` is AvalonDock `LayoutAnchorable`, which has no `Header`. Needs the `Wnd` type mapping resolved first.
- WindowTitleConverter `Convert`: calls `SpiritMainWindow.GetApplicationFullWindowTitle`, which Quire does not mirror.
- TextStyleProperties `Convert`: Mathcad tests `TextDecorationCollection` before `Thickness` in its type-pattern chain; two shapes tried (else-if chain, and the nested form with a re-cast), both mismatched.
- StickyDropDownSelector `EndInit`, `OnClick`: Mathcad's base `DevComponents.WpfRibbon.ButtonDropDown` has `Role` and `Command`; Quire's base `Fluent.DropDownButton` has neither.
- SystemUnitEditDialogViewModel `HandleDialogAction`: switch on `Nullable<DialogAction>` with block-scoped cases; the switch temporary and case locals take different slots from Mathcad's (first difference at #20, `stloc.1` vs `stloc.3`). One attempt (`switch (action)` form); reverted to stub.
- Zoom `HandleZoomValueChange`: `d as Zoom` with a guard return now matches the branches, but the `num * 100.0` temporary takes slot 4 where Mathcad uses slot 3 (`stloc.s 4` vs `stloc.3` at #16). Declaring `num2` early did not change the slot; reverted to stub.

## Not yet attempted in this lane (remaining)

- SpiritMainWindow (about 555 stub lines), TabManager (50), TextStyleEditDialog (51), StatusBarControls.StatusRibbonBar (44), TextStylesRibbonBar (24), TextStyleComboBox (18), SystemUnitEditDialog (23), TextAndRegionFormatRibbonBar (15), StylesAndLabelsFormatRibbonBar (15), QatControl (11), SpellingRibbonBar (11), RegionBorderRibbonBar (8), RibbonResources (8), RichTextContextMenuItemsDictionary (7). XAML bars: their `.ctor`, `InitializeComponent`, `Connect`, `_contentLoaded` stay stubs by rule.

## Gates

- Each type gated with `python tools/validation/release_il.py --base main --project Quire --type Quire.<Type>`; the project-wide FAIL is expected while other stubs remain. Judged by changed members only: no changed member fails.
