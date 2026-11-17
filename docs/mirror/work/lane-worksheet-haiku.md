# Lane Quire.Controls.Worksheet (haiku takeover)

- Owner: Claude worker (task_a4b72e175505), takeover of the stopped Antigravity lane (`docs/mirror/work/lane-quire-controls-worksheet.md`, which is historical only).
- Branch: `mirror/worksheet-haiku-20261117`
- Worktree: `C:/Users/DuyDang/orca/workspaces/Quire/claude-worksheet-haiku`

## Batch 1 — CombinedInvalidConverter

- Members: `CombinedInvalidConverter.ConvertBack`, `.ctor` (implicit default constructor).
- Parked: `CombinedInvalidConverter.Convert`. Idiomatic `values[0] is bool a && a && values[1] is bool b && b` gives 3 conditional branches; Mathcad has 2 and materializes the `is bool` tests with `ldc.i4.1; br; ldc.i4.0` followed by non-short-circuit `and`. Stub left in place.
- Release IL: `python tools/validation/release_il.py --base main --project Quire.Controls.Worksheet`: PASS; 436 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated` after batch 1: 2203 (2152 after batch 3) (`[global::Quire.Mirror.MirrorStub]` markers).

## Batch 2 — WorksheetViewModel (delegating and unit-test members)

- Members: `VisiblePages`, `Pages`, `TrailingEmptyPageCount`, `IsAutoCalcChecked`, `IsCalculating`, `UNITTEST_Evaluate`, `UNITTEST_DirectionalMoveHandler`, `UNITTEST_CanMoveInDirection`, `UNITTEST_CanPaste`, `UNITTEST_Paste`, `HandleSelectAll`, `ToggleItemSelection`, `SelectContiguous`, `SelectItem`, `SelectItems`, `ClearSelection`, `PageDown`, `PageUp`.
- Release IL: PASS; 454 checked; 0 failures.

## Batch 3 — WorksheetViewModel (auto-properties and one-call members)

- Members: auto-properties `HasScriptableControls`, `CurrentErrorChain`, `Header`, `Footer`, `FirstPageHeader`, `FirstPageFooter`, `IsSavedWorksheet`, `WorksheetContent`, `DisplayNumberFormat`, `ResultMatrixFormat`, `CurrentDragMode`; methods `CollapseIfPresent`, `ShowIfPresent`, `ResetVisualBrushIfPresent`, `OnStyleCollectionChanged`, `IsValidInsertionPoint`, `CanCreateOleObjectControl`, `CanCreateChartComponent`, `CreatePageBreakControl`, `HandleToggleAutoCalculation`, `CanReCalculateAll`, `CanRemoveSpace`, `CanCopySelection`, `CanCutSelection`, `GetItemRect`, `ItemsOverlapped`, `IsPageWideSizablePredicate`, `ImportData`, `GetDefaultLabeledIdFormat`, `GetInputRegions`, `GetOutputRegions`, `HandleSetCurrentProofingLanguage`, `OnRequestRestoreInsertionPoint`.
- Release IL: PASS; 498 checked; 0 failures.
- Remaining stubs in `src/Quire.Controls.Worksheet/Generated`: 2152.
