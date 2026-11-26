# Quire root windows and ribbon glue (haiku, 2026-11-27)

- Owner: dispatched worker `task_b0e51ad5e663`. Branch `mirror/quire-windows-haiku-20261127`, from main `139714554`.
- Members: `InitializeComponent` of 37 ribbon bars and windows (`src/Quire/Mirrored/<Type>.cs`, stub removed from `Generated/Mirror/<Type>.g.cs`); `_CreateDelegate` for the 12 types that have it; the `ctor` and `Connect` of `DebugPropertyWindow` and `FunctionBrowserPopupWindow`.
- Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 1447 checked, 0 failures.
- Parked (stubs kept): the constructors of the ribbon bars (base class: Mathcad `DevComponents.WpfRibbon.RibbonBar`/`RibbonGroupBox` vs Quire `Fluent.RibbonGroupBox`), and `Connect` of the ribbons whose `stfld` targets are `DevComponents.WpfRibbon.ButtonEx`/`ButtonDropDown` while Quire fields are `Fluent.Button`/`Fluent.DropDownButton`. Both need the type-map decision for the Fluent/DevComponents replacement (D6).
- Still open in this lane: the remaining window, dialog and view types (`License.LicenseOptionsPage`, `InputOutputDesignationWindow`, `TextStyleEditDialog`, `FunctionBrowser`, `OptionsWindow`, `ImportExcel`, `Freemium.FreemiumAboutBox`, `StatusBarControls.StatusRibbonBar`, `NumberFormatRibbonBar`, `PlotCreationRibbonBar` methods other than glue, and `MathFunctionsRibbonBar`).
