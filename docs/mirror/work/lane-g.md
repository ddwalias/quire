# Lane G — COM Interop Assemblies (D22)

- Owner: Lane G. Branch `feature/lane-g-20261113-r2`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/lane-g-20261113-r2`.
- Assemblies: `Quire.AutomationPrivate`, `Quire.Interop.Mathcad`, `Quire.Interop.MathcadOld`, `Quire.Interop.VSInterfacesLib`.
- Members: all managed members of the four assemblies are claimed and implemented. No unowned member remains. No batch landed in this session.

## Release IL report (base `main` = `c740bb713`)

| Project | Members | Declarations shape-matching | Bodies not exact | Gate line |
|---|---:|---:|---:|---|
| `Quire.AutomationPrivate` | 51 | 51 / 51 | 0 | `Release IL: PASS; 0 checked` (`release-il-f4d0654fa3b3`) |
| `Quire.Interop.MathcadOld` | 20 | 20 / 20 | 0 | `Release IL: PASS; 0 checked` (`release-il-8ceabcf5866f`) |
| `Quire.Interop.VSInterfacesLib` | 212 | 212 / 212 | 0 | `Release IL: PASS; 0 checked` (`release-il-0b313a3955ad`) |
| `Quire.Interop.Mathcad` | 545 | 459 / 545 | 86 | `Release IL: PASS; 0 checked` (`release-il-3d881d9ddd7b`) |

"0 checked" means no changed methods were selected, so the gate passes without comparing any body.

## Remaining mismatches (`Quire.Interop.Mathcad`, 86)

These are not exact and cannot be made exact with idiomatic C#. Rewriting any of them would make it a changed method that must match, so the gate would fail.

- 10 coclass constructors (`*Class::.ctor`). Mathcad has an `internalcall` assembly-visibility constructor. CS0669 forbids user constructors on ComImport classes.
- 11 delegate constructors plus 22 `BeginInvoke`/`EndInvoke`. Mathcad (TlbImp) emits `.ctor(object, native uint)` and no async members. Roslyn always emits `native int` and the async pair.
- 26 `_EventProvider` bodies (Init, add_*, remove_*, Finalize). They contain `castclass System.Object` before `Advise`/`Add`, `Delegate::Equals` binding, and TlbImp's manual byte-array and `ref bool` shapes. Roslyn does not emit these.
- 17 `_EventProvider`/`_SinkHelper` members whose Mathcad local signatures are unreadable (`Signature type sequence must have at least one element`). The gate cannot read the counterpart.
