# Lane Quire.Controls.Plots, half A Checkpoint

- Owner: Antigravity worker (Orca task `task_1145e28e3ba2`)
- Branch: `mirror/plots-a-batch7`
- Scope: Half A types up to and including `Controls.Plots.Plot3D.View.PlotAxesWidget3D.g.cs`

## Landed Commits
- Batch 1: `07bd98389` — AxisLimitsErrorsDictionary, PlotErrorCodes, ZoomTrackball (47 stubs removed)
- Batch 2: `c02f47e40` — CameraControlWidget (12 exact members, 1 stub parked)
- Batch 3: `33b86e258` — EquationListItem, ContourLogicalDataCollection (36 stubs removed, all 36 members exact)
- Batch 4: `a4f8fa23b` — PlottingPointsFilter (33 stubs removed, all 23 members exact)
- Batch 5: `5576c159c` — ImageCreator (35 stubs removed, all 18 members + PropertyNames exact)
- Batch 6: `9d4428059` — BaseMarker (25 stubs removed, all 17 members + nested PropertyNames, DoActionWhenEquationProxyChanged, closures exact)

## Batch 7 — TrackingHelper
- TrackingHelper: exact — all 22 members + closures/lambdas (26 stubs removed):
  - Fields: `_trackball`, `_wTransform`, `_hitTestTarget`, `_trackingPointsArray`, `_trackingLinesArray`
  - Property: `Viewport`
  - Methods: `.ctor()`, `Attach()`, `Detach()`, `Reset()`, `ClearTrackingVisuals()`, `TrackStart()`, `TrackEnd()`, `SetTrackingPoint()`, `AddTrackingPoint()`, `SetTrackingLine()`, `AddTrackingLine()`, `SetTrackingPoint2D()`, `AddTrackingPoint2D()`, `SetTrackingLine2D()`, `AddTrackingLine2D()`, `PerformHitTest()`, `HitTestResultCallback()`, `UpdateHitTestTarget(object, EventArgs)`, `UpdateHitTestTarget()`, `GetHitTestTargetTransformed()`
  - Closures / Lambdas: `<Attach>b__6_0`, `<ClearTrackingVisuals>b__9_0`, `<ClearTrackingVisuals>b__9_1`, `<PerformHitTest>b__0` on `<>c__DisplayClass20_0`, `<GetHitTestTargetTransformed>b__0` on `<>c__DisplayClass26_0`
- Release IL Report: PASS — 1699 checked, 0 failures.
