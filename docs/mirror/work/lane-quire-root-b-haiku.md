# Lane Quire root, half B (MathcadPrime.dll) — Quire

- Owner: Orca task `task_24a89889550b`.
- Branch: `mirror/quire-root-b-haiku-20261119`, from main `a4f8fa23b`.
- Scope: generated stubs `DropDownSelectorItem.g.cs` through `Zoom.g.cs` (175 files, about 114 with stubs).

## Batch 1 — small converters, ctors, fields, interface members

- Members moved to hand-written partials (stub removed from `.g.cs`):
  - `StatusBarControls.GotoPageValidationRule` `.ctor`; `StatusBarControls.GotoPageValidatorHelper` `.ctor`.
  - `WindowTitleConverter` `.ctor`, `ConvertBack`. Parked: `Convert` stays a stub. It calls `SpiritMainWindow.GetApplicationFullWindowTitle`, which has no Quire declaration.
  - `ProductInfo.Constants` constants and `.ctor`.
  - `LICINFO` fields; `SpiritOptions._EnableGettingStartedTab`; `IntegrationCommands._wcEnable`; `WindowTabTupleData._order`.
  - `ITabManagerItem.WrappedItem` (abstract declaration); `IConvertableToDocumentReadonlyOptions.ConvertToDocumentReadonlyOptions` (abstract declaration).
  - `PythonDirectoryPathConverter` `Convert`, `ConvertBack`, `.ctor`.
  - `StyleFontFamilyConverter` `Convert`, `ConvertBack`, `.ctor`.
- Parked: `WindowTabTupleData.Header` stays a stub. Mathcad's getter reads `Wnd` typed `DevComponents.WpfDock.DockWindow`; Quire's `Wnd` is `LayoutAnchorable`, so the callee token differs.
- Release IL: PASS (`--project Quire`), 759 checked, 0 failures.
- Remaining mismatches in this batch: none.

## Next candidates (not started)

`TextStyleProperties`, `TextStylePropertyToVisibility` (closure shapes), `UniqueTextStyleNameRule.Validate` (bool and `and` shape), `ProofingLanguageDataItem.GetCultureName` with its fields, `ScalarStateExtensions`, `TabManagerItem`.
