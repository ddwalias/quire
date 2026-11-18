# Lane Quire.Controls.Plots, half A Checkpoint

- Owner: Antigravity worker (Orca task `task_1145e28e3ba2`)
- Branch: `mirror/plots-a-20261119a`
- Scope: Half A types up to and including `Controls.Plots.Plot3D.View.PlotAxesWidget3D.g.cs`

## Batch 1 — AxisLimitsErrorsDictionary, PlotErrorCodes, ZoomTrackball

- AxisLimitsErrorsDictionary: exact — all 10 constant fields moved to hand-written partial, stubs removed from .g.cs.
- PlotErrorCodes: exact — all 24 constant fields moved to hand-written partial, stubs removed from .g.cs.
- ZoomTrackball: exact — all 13 members (`_timer`, `CLICK_ZOOM_FACTOR`, `_selectionDraggerRubberBand`, `.ctor`, `TrackStart`, `GetNumTicksForScaleClick`, `Track`, `Detach`, `Clean`, `TrackEnd`, `CoercePosition`, `UpdateSelectionRubberBand`, `RemoveSelectionRubberBand`) plus compiler-generated closure `<.ctor>b__3_0`.
- Release IL Report: PASS — 1320 checked, 0 failures. Unimplemented stubs reduced from 3161 to 3135 (-26 stubs + 34 constant fields).
- Remaining mismatches / blockers (this lane): none in Batch 1. Remaining half-A types continue in subsequent batches.
