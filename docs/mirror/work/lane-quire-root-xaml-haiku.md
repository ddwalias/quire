# Lane Quire root XAML files (AboutWindow through DropDownSelector)

- Owner: Orca task `task_8de67ffa93f6` (dispatched worker).
- Branch: `mirror/quire-root-xaml-haiku-20261119`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/quire-root-xaml-haiku`.
- Set: non-Automation `src/Quire/Generated/Mirror/*.g.cs` from AboutWindow through DropDownSelector that contain `InitializeComponent` (24 files). Constructors calling `InitializeComponent`, `InitializeComponent`, `Connect`, `_contentLoaded`, `_this` and `_CreateDelegate` stay stubs.

## Landed (main)

- Batch A (748bae85c): field declarations moved to hand-written partials for DocumentViewRibbonBar, DocumentRegionsRibbonBar, DocumentInsertRibbonBar, DataMatrixRowsAndColumnsRibbonBar, DataMatrixFileAccessRibbonBar, DataMatrixDataAndMatricesRibbonBar, CalculationOptionsRibbonBar, CalculationErrorTracingRibbonBar, DialogManagerResourceDictionary (`AccessDeniedToBrowsePath` const), DimensionsEditDialog. Release IL: no new failures.
- CustomMarginsDialog (fd71495da): fields, `NewPageMargin`, `ShowDialog`, `MarginTextBox_TextChanged`, `OkButton_Click`, `OnRenderSizeChanged` (nested `SizeToContent` guard matches the `bne.un` shape). Release IL: those members match.
- CalculationWarningRibbonBar (82d4e1181): `SystemRedefinitionWarningsGroup`, `UserRedefinitionWarningsGroup`, `WarningsTypesFilter` get/set, `PropertyChanged` event. Release IL: those members match.
- ApplicationMenuControl (dcea22279): 23 `Part_ApplicationMenu_*` fields (the `Part_ApplicationMenu_NewDocument` field came with the same change). Release IL: no new failures.
- CustomColorCanvas (landing in progress, branch commit 8e0e7f163): fields, consts, `Saturation`, `Hue`, `Canvas`, `Dispose`, `PropertyChanged`, `Canvas_OnMouseUp`, `Canvas_OnPreviewKeyDown`, `Canvas_OnMouseEnter`. Release IL gate: all implemented members match; 940 of 950 checked exact.

## Parked (stays stub, one attempt each)

- CustomColorCanvas `OnHslChanged`: Release IL keeps the `WorkArea` read in a different position (first difference at IL #27).
- CustomColorCanvas `Canvas_OnMouseMove`, `Canvas_OnMouseLeftButtonDown`, `Canvas_OnSizeChanged`, `Canvas_OnGotKeyboardFocus`, `HandleArrowKeys`: Release IL differs at the first instructions (argument/local order; idiomatic forms tried).
- CustomColorCanvas finalizer: Mathcad calls `Object.Finalize` through `callvirt` on `this`; `base.Finalize()` emits `call`.
- CalculationWarningRibbonBar `WarningsTypesFilterProperty`, CustomMarginsDialogResourceDictionary `KeysOwnerTypes`, Controls.VisualCGSUnitDescriptorTable `Instance`: static initializers (`.cctor`), kept as stubs.

## Not started (remaining types in the set)

- Controls.ColorPicker (54 stubs), Controls.CustomColorPicker (42), Controls.ExcelGrid (59), Controls.CustomNumericUpDown.CustomNumericUpDown (97), DropDownSelector (46). Most of their stubs are DependencyProperty fields (static, stay stubs) and XAML-touching members; the remaining plain members have not been attempted.
- Types with no non-XAML stubs left: CalculationControlRibbonTab, DebugPropertyWindow, AboutWindow.

## Release IL (latest)

- CustomColorCanvas gate: FAIL at type level (expected while stubs remain). Failures listed: ctor, finalizer, InitializeComponent, Connect and the six parked methods above. No previously exact method regressed.
