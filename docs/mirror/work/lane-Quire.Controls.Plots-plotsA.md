# Lane Quire.Controls.Plots, half A Checkpoint

- Owner: Antigravity worker (Orca task `task_1145e28e3ba2`)
- Branch: `mirror/plots-a-batch9`
- Scope: Half A types up to and including `Controls.Plots.Plot3D.View.PlotAxesWidget3D.g.cs`

## Landed Commits
- Batch 1: `07bd98389` — AxisLimitsErrorsDictionary, PlotErrorCodes, ZoomTrackball (47 stubs removed)
- Batch 2: `c02f47e40` — CameraControlWidget (12 exact members, 1 stub parked)
- Batch 3: `33b86e258` — EquationListItem, ContourLogicalDataCollection (36 stubs removed, all 36 members exact)
- Batch 4: `a4f8fa23b` — PlottingPointsFilter (33 stubs removed, all 23 members exact)
- Batch 5: `5576c159c` — ImageCreator (35 stubs removed, all 18 members + PropertyNames exact)
- Batch 6: `9d4428059` — BaseMarker (25 stubs removed, all 17 members + nested PropertyNames, DoActionWhenEquationProxyChanged, closures exact)
- Batch 7: `073dd9159` — TrackingHelper (26 stubs removed, all 22 members + closures exact; receipt `gate-il-22aa45222cfb.json`)
- Batch 8: `51c53aede` — PlotTitleModel (22 stubs removed), PlotLegendModel (25 stubs removed); 47 stubs removed, all members exact; receipt `gate-il-17e43d101d4f.json`
- Batch 9 (pending landing): ContourCalculatable (31 stubs removed), PlotModelBase (37 stubs removed); 68 stubs removed, all members exact; report `release-il-12f4d1e0289a`

## Parked Members
- `Quire.Controls.Plots.Plot3D.View.CameraControlWidget.OnApplyTemplate()`: Mathcad emits call to base.OnApplyTemplate() before GetTemplateChild calls; Roslyn orders base call after null checks or stack differences.

## Remaining Stubs in Half A
- See remaining generated files in Half A.
