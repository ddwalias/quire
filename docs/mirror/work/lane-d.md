# Lane D — Quire.Engine.Utl

- Owner: Lane D (Orca worker task_01ad2eae1770). Branch: `feature/lane-d-engine-utl-20261114` (worktree `C:/Users/DuyDang/orca/workspaces/Quire/lane-d-engine-utl-20261114`), from `main` 31bf923f7.
- Assembly: `Quire.Engine.Utl` (utl-mgd.dll, C++/CLI mixed-mode).

## Members

- No `MirrorStub` or `NotImplemented` markers in `src/Quire.Engine.Utl` (managed partials, `Generated/` and `MixedMode/UtlPort.cpp`).
- Original `utl-mgd.dll` inventory: 370 methods (297 IL bodies, 55 native-code globals, 18 declaration-only interface/delegate members). The candidate inventory has the same 370 rows, each paired to an original.
- This pass: verification only; no new members needed as full original inventory is already implemented and matched.

## Release IL (current main 31bf923f7)

- `python tools/validation/release_il.py --base main --project Quire.Engine.Utl`: PASS; 352 checked.
  - exact: 346 (297 IL bodies plus 49 native-code globals)
  - native_pairs: 55 (matched by declaration)
  - declaration-only: 18
  - failures: 0, unfinished: 0, missing: 0
  - skipped: 3 native bodies unchanged and never matched Mathcad (`__std_mismatch_2`, `new`, `delete[]`)
- Report: `.mirror-cache/release-il-c4cc4c029867/release-il.json`

## Remaining mismatches

- None on the Release IL gate.
