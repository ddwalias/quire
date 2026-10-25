# Lane M — Quire.Calculation.Core and Quire.Media3D

- Owner: Lane M; branch `mirror/m-calc-core-20261025-m01`; worktree `C:/Projects/Quire-wt/lane-m-calc-core`.
- Exact members:
  - `Quire.Media3D`: All 635 managed methods with bodies are exact (7 declaration-only). Assembly is COMPLETE.
    - `Media3D.Wires.WireDashes`: implemented `PitchProperty`, `get_Pitch`, `set_Pitch`, `AddDashedLine`, `Generate`, `.ctor`.
    - `Media3D.Wires.WirePoints`: implemented `Recalculate`; adjusted declaration order so `PositionsPropertyChanged` precedes `UpdateColor` to match Mathcad closure ordinal `DisplayClass7_0`.
- Release IL report:
  - `Quire.Media3D`: PASS; 635 checked, 635 exact, 0 failures, 0 unfinished (`.mirror-cache/release-il-c7d7c7d0e54b/release-il.json`). Full Release inventory confirms exact parity for every managed body (642 declarations, 635 exact bodies, 7 declaration-only, 0 stubs, 0 unresolved).
- Remaining:
  - `Quire.Media3D`: 0 mismatches. COMPLETE.
  - `Quire.Calculation.Core`: 528 exact, 228 declaration-only, 274 unresolved and 72 stub bodies.
