# Lane Quire.Controls.Plots, half A Checkpoint

- Owner: Antigravity worker (Orca task `task_1145e28e3ba2`)
- Branch: `mirror/plots-a-batch4`
- Scope: Half A types up to and including `Controls.Plots.Plot3D.View.PlotAxesWidget3D.g.cs`

## Landed Commits
- Batch 1: `07bd98389` — AxisLimitsErrorsDictionary, PlotErrorCodes, ZoomTrackball (47 stubs removed)
- Batch 2: `c02f47e40` — CameraControlWidget (12 exact members, 1 stub parked)
- Batch 3: `33b86e258` — EquationListItem, ContourLogicalDataCollection (36 stubs removed, all 36 members exact)

## Batch 4 — PlottingPointsFilter
- PlottingPointsFilter: exact — all 23 members (33 stubs removed):
  - 8 fields: `_plottingPoints`, `_containes`, `_filteringAllowed`, `_filteredPointCollection`, `_propertyChanged`, `_propertyWasChangedDuringInitialization`, `_notNullPointsCount`, `FilteredPlottingPointsChanged`
  - Properties: `PlottingPoints`, `NotNullPointsCount`, `PlottingPointsCount`, `FilteringAllowed`, `FilteredPointsEnumeration`, `FilteredPointsCollection`
  - Methods: `.ctor()`, `GetIndexesOfPointsInRange()`, `ScanPoints()`, `InRangeOrNone()`, `UpdateFilteredPointCollection()`, `BeginInit()`, `EndInit()`
  - Closures: `<GetIndexesOfPointsInRange>b__18_0` on `<>c`, `<get_FilteredPointsEnumeration>d__22`, `<BeginInit>b__27_0`
- Release IL Report: PASS — 1541 checked, 0 failures. Unfinished stubs: 2932.
