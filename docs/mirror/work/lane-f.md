# Lane F — Quire.Mpl

- Owner: Lane F. Branch `feature/lane-f-mpl-20261114` (worktree `C:/Users/DuyDang/orca/workspaces/Quire/lane-f-mpl-20261114`), from `main` 4c9d51d3f.
- Assembly: `Quire.Mpl` (depends on `Quire.Mpl.Emit`, read-only for this lane).

## Members

- Landed earlier as `a5a946a60` (already on `main`): Mpl.Run.Keyword consts, Mpl.Run.BindingInfo consts, Mpl.Run.Primitive fields, Mpl.Run.Clo{Dbl,Int,Obj}Function `m_clo` fields. Declarations moved to hand-written partials; stubs removed from the `.g.cs`.
- This pass: no new members. Verification only.

## Release IL (current main 4c9d51d3f)

- `release_il.py --base main --project Quire.Mpl`: PASS; 1867 checked; 1867 exact; 0 failures.
- `release_il.py --base main --project Quire.Mpl --all`: PASS; 2032 checked; 1867 exact; 165 declaration-only; 0 failures, 0 missing, 0 skipped, 0 unfinished.
- `[MirrorStub]` markers left in `src/Quire.Mpl`: 0.

## Remaining mismatches

- None.
