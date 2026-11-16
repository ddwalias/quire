# Lane Quire.Controls.Worksheet

- Owner: task_dd83619a6812
- Branch: feature/controls-worksheet-stubs-agy-20261116
- Worktree: C:/Users/DuyDang/orca/workspaces/Quire/lane-quire-controls-worksheet

## Batch 1 — Constants, ContourPlotRegionPersistentData, Plot3DRegionPersistentData, DraftModeGutterWidthConverter, FileNameConverter

- Members:
  - `Quire.ProductInfo.Constants`: `ConverterProductName`, `ConverterExe`, `.ctor`
  - `Quire.Controls.Worksheet.ContourPlotRegionPersistentData`: `.ctor`, `DeserializeDimensions`
  - `Quire.Controls.Worksheet.Plot3DRegionPersistentData`: `.ctor`, `DeserializeDimensions`
  - `Quire.Controls.Worksheet.DraftModeGutterWidthConverter`: `Convert`, `ConvertBack`, `.ctor`
  - `Quire.Controls.Worksheet.FileNameConverter`: `Instance`, `.ctor`, `Convert`, `ConvertBack`, `.cctor`
- Release IL: PASS; 193 checked; 0 failures.
- Landed commit: `ee29bbbbf2186c23d0355dbd9b77cddb382ef5a9`
- Remaining mismatches: none in this batch (3046 stubs remaining in Quire.Controls.Worksheet).

## Batch 2 — NonClippingStackPanel, WorksheetPageLayoutExtensions, SavedDateConverter, FileNameControl, FilePathControl

- Members:
  - `Quire.Controls.Worksheet.NonClippingStackPanel`: `.ctor`, `GetLayoutClip`
  - `Quire.Controls.Worksheet.WorksheetPageLayoutExtensions`: `SetGridSize`
  - `Quire.Controls.Worksheet.SavedDateConverter`: `Convert`, `ConvertBack`, `.ctor`
  - `Quire.Controls.Worksheet.FileNameControl`: `GetRegionPersistentData`, `.ctor`
  - `Quire.Controls.Worksheet.FilePathControl`: `GetRegionPersistentData`, `.ctor`
- Release IL: PASS; 199 checked; 0 failures.
- Landed commit: `35b689c41f75bf1c98fbd4b96a9b0547bf1c0236`
- Remaining mismatches: none in this batch (3040 stubs remaining in Quire.Controls.Worksheet).

## Batch 3 — NonPrintableSpaceIndicatorPositionConverter, HeaderFooterRoleExtensions, FirstPageHeaderFooterConditionConverter, WorksheetItemPresenter

- Members:
  - `Quire.Controls.Worksheet.NonPrintableSpaceIndicatorPositionConverter`: `Convert`, `ConvertBack`, `.ctor`
  - `Quire.Controls.Worksheet.HeaderFooterRoleExtensions`: `IsHeaderRole`, `IsFooterRole`
  - `Quire.Controls.Worksheet.FirstPageHeaderFooterConditionConverter`: `Convert`, `ConvertBack`, `.ctor`
  - `Quire.Controls.Worksheet.WorksheetItemPresenter`: `Clip` (get, set), `HandleItemLocationChanged`, `.ctor`
- Release IL: PASS; 208 checked; 0 failures.
- Landed commit: `340d0f97ce93ee490ee2f6b0c49c3185376b84ef`
- Remaining mismatches: none in this batch (3031 stubs remaining in Quire.Controls.Worksheet).

## Batch 4 — HeaderFooterItemsControl, WorksheetPageExtensions, WorksheetRegionsInfo, PageBodyViewModel

- Members:
  - `Quire.Controls.Worksheet.HeaderFooterItemsControl`: `.ctor`
  - `Quire.Controls.Worksheet.WorksheetPageExtensions`: `GetViewModel`, `GetPageBody`
  - `Quire.Controls.Worksheet.WorksheetRegionsInfo`: `_isScriptableControlExists`, `get_IsScriptableControlExists`, `set_IsScriptableControlExists`
  - `Quire.Controls.Worksheet.PageBodyViewModel`: `.ctor`, `GetChildItemFromLocation`
- Release IL: PASS; 214 checked; 0 failures.
- Landed commit: `43a42ae89cfd313d11b33215893a9d94944ca324`
- Parked members:
  - `Quire.Controls.Worksheet.CollapsibleAreaHeightConverter`: blocked by missing project reference to `Quire.Controls.CollapsibleArea` in `Quire.Controls.Worksheet.csproj`.
- Remaining mismatches: none in this batch (3025 stubs remaining in Quire.Controls.Worksheet).

## Batch 5 — ItemLocationWorksheetStrategyHelper, PrintGridSettings, MouseUtilities, SwitchToDraftModeButtonAdorner, ZombieAdorner, DragZombieAdorner, WorksheetItemPositionComparer, SavedDateControl, FilePathConverter, HeaderFooterSelectorConverter

- Members:
  - `Quire.Controls.Worksheet.ItemLocationWorksheetStrategyHelper`: `GetItemContentClipping`
  - `Quire.Controls.Worksheet.PrintGridSettings`: `MajorLinesThickness`, `MinorLinesThickness`, `MajorLinesColor`, `MinorLinesColor`, `FrameColor`
  - `Quire.Controls.Worksheet.MouseUtilities`: `Win32Point.X`, `Win32Point.Y`, `GetCursorPos`, `GetMousePosition`, `.ctor`
  - `Quire.Controls.Worksheet.SwitchToDraftModeButtonAdorner`: `_rect`, `.ctor`, `MeasureOverride`, `ArrangeOverride`
  - `Quire.Controls.Worksheet.ZombieAdorner`: `Visual`, `_bounds`, `.ctor`, `OnRender`
  - `Quire.Controls.Worksheet.DragZombieAdorner`: `_childBoundaries`, `_visual`, `.ctor`, `OnRender`
  - `Quire.Controls.Worksheet.WorksheetItemPositionComparer`: `ItemManager`, `Compare`, `.ctor`
  - `Quire.Controls.Worksheet.SavedDateControl`: `GetRegionPersistentData`, `SetRegionPersistentData`, `.ctor`
  - `Quire.Controls.Worksheet.FilePathConverter`: `Convert`, `ConvertBack`, `.ctor`
  - `Quire.Controls.Worksheet.HeaderFooterSelectorConverter`: `Convert`, `ConvertBack`, `.ctor`
- Release IL: PASS; 233 checked; 0 failures.
- Landed commit: (in-progress)
- Parked members:
  - `Quire.Controls.Worksheet.CollapsibleAreaHeightConverter`: blocked by missing project reference to `Quire.Controls.CollapsibleArea` in `Quire.Controls.Worksheet.csproj`.
- Remaining mismatches: none in this batch (3007 stubs remaining in Quire.Controls.Worksheet).
