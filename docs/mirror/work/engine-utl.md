# Quire.Engine.Utl (utl-mgd.dll) checkpoint

- Owner: Orca worker task_03ed4bdb50ab (assembly `Quire.Engine.Utl`, C++/CLI).
- Branch: `feature/engine-utl-20261113-r4`, based on `main` at `875e769df`.
- Members: no `MirrorStub`/`NotImplemented` markers in `src/Quire.Engine.Utl` (managed partials, `Generated/` and `MixedMode/UtlPort.cpp`). No member code changed in this run; the assembly already carries an implementation for every original member. Original `utl-mgd.dll` has 370 methods (297 IL bodies, 55 native-code globals, 18 declaration-only interface/delegate members); the candidate inventory has the same 370 rows, each paired to an original.
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Engine.Utl` → `Release IL: PASS; 352 checked`. Report at `.mirror-cache/release-il-209cc4a66795/release-il.json`: failures 0, unfinished 0, exact 346 (297 IL bodies plus 49 native-code globals), native_pairs 55 (matched by declaration), declaration-only 18 (6 native toolset names gate-pass as native pairs).
- Remaining mismatches: none on the IL gate. Not covered by the gate: the 55 native-code globals have no IL and are matched by declaration only (`native_pairs`); field layout is not compared; `docs/mirror/type-map.csv` and `assembly-map.md` still label this assembly `missing`/`deferred` (shared docs, not edited here).
