# Lane Quire.Controls.Plots, half A Checkpoint

- Owner: Antigravity worker (Orca task `task_1145e28e3ba2`)
- Branch: `mirror/plots-a-batch6`
- Scope: Half A types up to and including `Controls.Plots.Plot3D.View.PlotAxesWidget3D.g.cs`

## Landed Commits
- Batch 1: `07bd98389` — AxisLimitsErrorsDictionary, PlotErrorCodes, ZoomTrackball (47 stubs removed)
- Batch 2: `c02f47e40` — CameraControlWidget (12 exact members, 1 stub parked)
- Batch 3: `33b86e258` — EquationListItem, ContourLogicalDataCollection (36 stubs removed, all 36 members exact)
- Batch 4: `a4f8fa23b` — PlottingPointsFilter (33 stubs removed, all 23 members exact)
- Batch 5: `5576c159c` — ImageCreator (35 stubs removed, all 18 members + PropertyNames exact)

## Batch 6 — BaseMarker
- BaseMarker: exact — all 17 members + nested PropertyNames (3 consts) + nested DoActionWhenEquationProxyChanged (3 members) + 2 closures (<set_EquationSExpression>b__0, <set_ExpressionXml>b__0) (25 stubs removed):
  - Nested type `PropertyNames`: 3 const strings (`Color`, `ShowCallout`, `IsUserSetValue`)
  - Nested type `DoActionWhenEquationProxyChanged`: `_action`, `_baseMarker`, `.ctor()`, `ActionWrapper()`
  - Fields: `_color`, `_showCallout`, `_equationProxy`, `_calculatable`, `_isUserSetValue`, `PropertyChanged`, `EquationProxyChanged`
  - Properties: `Color`, `Calculatable`, `ShowCallout`, `IsUserSetValue`, `EquationSExpression`, `ExpressionXml`
  - Methods: `CalculatableRequestEvaluation()`, `SetIsUserSetValue()`, `OnPropertyChanged()`, `SetEquationProxy()`, `.ctor()`
  - Closures: `<>c__DisplayClass22_0::<set_EquationSExpression>b__0`, `<>c__DisplayClass25_0::<set_ExpressionXml>b__0`
- Release IL Report: PASS — 1656 checked, 0 failures.
