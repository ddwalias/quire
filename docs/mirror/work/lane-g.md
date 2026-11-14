# Lane G — COM Interop Assemblies (D22)

- Owner: Lane G (this session, task `task_c2a776706862`).
- Branch: `feature/lane-g-interop-20261114`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/lane-g-interop-20261114`.
- Assemblies: `Quire.AutomationPrivate`, `Quire.Interop.Mathcad`, `Quire.Interop.MathcadOld`, `Quire.Interop.VSInterfacesLib`.
- Base: `main` = `31bf923f7`. No managed member body changed in this session; the code on main is unchanged.

## Release IL report

Default completeness gate (`release_il.py --base main --project <p> --all`):

| Project | Checked | Gate line |
|---|---:|---|
| `Quire.AutomationPrivate` | 51 / 51 exact | `Release IL: PASS; 51 checked` |
| `Quire.Interop.MathcadOld` | 20 / 20 exact | `Release IL: PASS; 20 checked` |
| `Quire.Interop.VSInterfacesLib` | 212 / 212 exact | `Release IL: PASS; 212 checked` |
| `Quire.Interop.Mathcad` | 448 / 545 exact | `Release IL: FAIL; 545 checked` (97 failing rows) |

## Remaining mismatches (`Quire.Interop.Mathcad`, 97 rows)

| Count | Members | Cause | Result |
|---:|---|---|---|
| 10 | `*Class::.ctor` (coclass) | Mathcad's constructor is assembly-visible `internalcall`. `ComImport` classes reject any user constructor (CS0669). Tried: `[MethodImpl(InternalCall)] internal extern` ctor (CS0669, build fails). A plain or public constructor is rejected by the same rule; not built separately. | Not fixable from source. |
| 11 | `_IApplicationEvents_*`/`_IWorksheetEvents_*` `EventHandler::.ctor(object, UIntPtr)` (no Quire pair) | The compiler always emits `.ctor(object, IntPtr)` for a delegate. | Not fixable from source. |
| 22 | `*EventHandler::BeginInvoke`/`EndInvoke` | Compiler-generated for every C# delegate. Mathcad has none (product-only, D21). | Quire-only, not implemented; report. |
| 11 | `*EventHandler::.ctor(object, IntPtr)` | Compiler-generated; Mathcad's counterpart is `UIntPtr` (see row above). | Quire-only, not implemented; report. |
| 17 | `_*EventProvider` ctor/Dispose and `_*SinkHelper` members | Gate cannot decode Mathcad's local signatures (`Signature type sequence must have at least one element`). | Tool limitation; the counterpart is unreadable. |
| 22 | `_*EventProvider` `add_*`/`remove_*` | `castclass System.Object` on each sink/value argument (Roslyn never emits it). `remove_*` calls `Delegate::Equals`; Roslyn emits `Object::Equals`. Mathcad enters the monitor inside `try` with no `lockTaken = false` store (`bool` local would need definite assignment). | Tried 3 forms (below); 0 / 22 exact. |
| 4 | `_*EventProvider::Init`/`Finalize` | `Init` keeps `castclass IConnectionPoint` on a same-typed local after `FindConnectionPoint` (Roslyn emits no cast there); `Finalize` has the same `lockTaken` shape as `add_*` plus a nested `try`/`catch`. | Tried 3 forms (below); 0 / 4 exact. |

### Forms tried for the 26 EventProvider bodies

All three forms are gated on the same 30 selected methods (`--type Quire._IApplicationEvents_EventProvider --type Quire._IWorksheetEvents_EventProvider`); the 4 `IL unreadable` ctor/Dispose rows are included in the 30.

1. **Form 1 — explicit conversions.** Byte-by-byte `riidBytes[n] = …` in `Init`; `((Delegate)x).Equals((object)value)`; `(object)` on `Advise`/`Add` arguments. Result: `Release IL: FAIL; 30 checked`, 0 pass. `castclass Object` and `Object::Equals` persist.
2. **Form 2 — `lock (this)`.** Form 1 plus the `lock` statement in place of `Monitor.Enter`/`try`/`finally` for `add_*`/`remove_*`. Result: `Release IL: FAIL; 30 checked`, 0 pass. Same castclass and `Object::Equals` differences.
3. **Form 3 — `Monitor.Enter` before `try`.** Form 1 plus `bool lockTaken = false; Monitor.Enter(...); try { … }`. Result: `Release IL: FAIL; 30 checked`, 0 pass. Same castclass and `Object::Equals` differences; the `ldc.i4.0` lockTaken store still differs at #0.

The unfinished members are restored to main's source; no candidate code is committed.

## Quire-only members

`BeginInvoke`/`EndInvoke` (22) and the `(object, IntPtr)` delegate constructors (11) are compiler-generated for the delegate types. They have no Mathcad counterpart and are not implemented. The 11 `(object, IntPtr)` constructors and the 22 `BeginInvoke`/`EndInvoke` rows were not gated with alternative source forms, because the compiler emits them for every C# delegate declaration.
