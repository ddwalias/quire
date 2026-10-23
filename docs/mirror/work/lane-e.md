# Lane E — Quire.Mpl

- Owner: Lane E (Claude).
- Branch: `mirror/e-mpl-final-20261023-e9`, worktree `C:/Projects/Quire-wt/lane-e-mpl-final`, from `main` 4fe52924.
  The earlier worktree `C:/Projects/Quire-wt/lane-e-mpl` is untouched.
- Status: the 2,032 Mathcad-mapped members were already exact or declared in Mathcad order (f0ead371, bfe192a8).
  The checkpoint below replaces the 34-member list from 10-21, which was fixed on 10-22.
- Relocated (no Mathcad counterpart), 73 members:
  - `src/Quire.Mpl/Internal/BuiltinUnits.cs` → `src/Quire.Mpl.Runtime/BuiltinUnits.cs`: `BuiltinUnits`, its `<>c` and `<>c__DisplayClass45_0`, `AffineUnit`, and the 32 `<>z__ReadOnlyArray<T>` methods.
  - `src/Quire.Mpl/Internal/RuntimeThread.cs` → `src/Quire.Mpl.Runtime/RuntimeThread.cs`: `RuntimeThread.Ensure`.
  - The new project `src/Quire.Mpl.Runtime` is support (no mapped originals), like `Quire.Mirror`. It references `Quire.Mpl`, and `Quire.Mpl` no longer references it.
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Mpl --project Quire.Mpl.Runtime` PASS; 1,867 checked; `.mirror-cache/release-il-e-mpl-final/release-il.json` (tip a8156ba0).
  - `Quire.Mpl`: 2,105 methods, 1,867 exact, 165 declaration-only, 0 unfinished. The set matches the f0ead371 report with nothing lost.
  - `Quire.Mpl.Runtime`: 73 unmapped members (support, not a mirror).
  - Quire.Mpl alone fails with 29 "method removed" rows, because the moved members only appear in Quire.Mpl.Runtime. The two-project run records them as moves.
- Other touched projects, Release IL PASS on the same tree before commit: Quire.Engine, Quire.Calculation, Quire.EquationEditor, Quire.
- One-off behavior check: Quire.Mpl.Tests 188/188, Quire.Engine.Tests 97/97, Quire.EquationEditor.Tests 88/88.
- `dotnet build Quire.slnx -c Release`: 0 errors.
- `assembly_map.py` (output to scratch, `docs/mirror/assembly-map.md` not regenerated): `Quire.Mpl` complete at a8156ba0. 1,867 exact, 165 declarations, 0 remaining, 0 missing original methods, 0 stubs, current revision.
- Edits outside `src/Quire.Mpl`: project and solution registration only.
  - `Quire.slnx` and ProjectReference to `Quire.Mpl.Runtime` in `Quire.Engine`, `Quire.Calculation`, `Quire.EquationEditor`, `Quire`, `Quire.Engine.Tests`, `Quire.EquationEditor.Tests`, `Quire.Mpl.Tests`.
  - `Quire.Mpl.csproj`: `InternalsVisibleTo` grants for Engine, Calculation, EquationEditor, Quire and their test projects removed. The build needs none. The generated `Mirror.props` grants are unchanged.
  - `docs/mirror/evidence/Mcd.RuntimeOptions.md`: path updated.
- Remaining mismatches: none in `Quire.Mpl`.
