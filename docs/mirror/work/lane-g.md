# Lane G — COM Interop Assemblies (D22)

- Owner: Lane G (task `task_f73903056c42`, follow-up to `task_c2a776706862`).
- Branch: `feature/lane-g-interop-20261114`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/lane-g-interop-20261114`.
- Assemblies: `Quire.AutomationPrivate`, `Quire.Interop.Mathcad`, `Quire.Interop.MathcadOld`, `Quire.Interop.VSInterfacesLib`.
- Managed bodies of these assemblies are unchanged from `main`. The only code change is the MethodAudit reader fix below.

## Release IL report

Completeness gate (`release_il.py --base main --project <p> --all`), judged by the base:

| Project | Checked | Gate line |
|---|---:|---|
| `Quire.AutomationPrivate` | 51 / 51 exact | `Release IL: PASS; 51 checked` |
| `Quire.Interop.MathcadOld` | 20 / 20 exact | `Release IL: PASS; 20 checked` |
| `Quire.Interop.VSInterfacesLib` | 212 / 212 exact | `Release IL: PASS; 212 checked` |
| `Quire.Interop.Mathcad` | 448 / 545 exact | `Release IL: FAIL; 545 checked` (97 failing rows) |

With the MethodAudit fix judged by the candidate (`--judge candidate --all`), all three passing projects still pass, `verdict_changes` is empty, and `Interop.Mathcad` still has 97 failing rows, now with real reasons instead of `IL unreadable`.

## Reader fix (shared tool, for maintainer review)

- **Cause of the 17 "unreadable" rows.** Mathcad's `_IApplicationEvents_SinkHelper` (13) and `_IWorksheetEvents_SinkHelper` / `_*EventProvider` ctor and `Dispose` (4) carry `.locals init ()`: a LOCAL_SIG with zero locals. `DecodeLocalSignature` throws `Signature type sequence must have at least one element`. The type map and the reference lookup are correct (type map rows present, `interop.mathcad`, not nested); the reference tool prints the same placeholder comment but still emits the IL.
- **Fix.** `tools/Quire.Tools.MethodAudit/Structure.cs`: `HasLocals` reads the LOCAL_SIG count and treats zero as no locals, in both the Release `.locals` line and the body text. Only the 17 rows change behavior; no verdict of an unchanged member changes.

## Remaining mismatches (`Quire.Interop.Mathcad`, 97 rows)

Only 43 of the 97 have an IL body to compare. The other 54 fail on declaration or pairing. Each row below is classified by its exact difference; "impossible" means no C# source can produce it under Roslyn (net10).

### Bodied (43)

**A. Locals-free bodies (17): SinkHelper `Quit`/`Idle`/`Changed`/`Recalculated`/`WorksheetClosing`/... (13 incl. `.ctor`s), EventProvider `.ctor`/`Dispose` (4).**
- Mathcad: fat header, `.locals init ()` (InitLocals on, zero locals), exact `.maxstack` (1, 2, 4, 6). Roslyn: tiny header for a body without locals, `.maxstack 8`, no InitLocals flag (lab: `SinkA.Quit` header size 1; a local-bearing body gets exact maxstack, but adding a local changes `.locals`).
- Encoding: Mathcad emits the long `ldarg N` form for every argument load (272 uses, 0 `ldarg.N`); Roslyn emits `ldarg.0`. The gate compares raw opcode names, so every `ldarg` differs.
- `Quit`-style bodies also differ in shape: Mathcad has a `ret` inside the null-check branch plus a trailing `ret`. The early-return form reproduces it (lab-verified), so only the header and encoding remain.
- `SinkHelper` `.ctor`: explicit stores (`m_dwCookie = 0`, then `= null` per delegate) reproduce Mathcad's stores; field initializers are dropped by Roslyn.
- `EventProvider.Dispose`: Mathcad `callvirt Finalize`; Roslyn `call` (Quire's `Finalize` is non-virtual; Mathcad's is `public virtual`). Blocked by the header rules above regardless.
- Verdict: **impossible** under the strict rules.

**B. `add_*`/`remove_*` (22), `EventProvider`.**
- `add_*`: Mathcad enters the monitor inside `try` with no `lockTaken` store; Roslyn emits `ldc.i4.0; stloc` for `lockTaken`. Passing `lockTaken` by `ref` without assigning it is CS0165 (lab-verified), so the store cannot be removed.
- `add_*`: `castclass System.Object` before `Advise` and `Add`; Roslyn emits none for `(object)` or implicit conversions (lab-verified).
- `remove_*`: `callvirt System.Delegate::Equals`; Roslyn emits `System.Object::Equals` even through a `Delegate`-typed receiver (the net10 reference surface does not redeclare `Delegate.Equals`; lab-verified).
- All of them carry the long `ldarg N` form.
- Verdict: **impossible** (three independent reasons plus the encoding).

**C. `Init`/`Finalize` (4).**
- `Init`: Mathcad `castclass IConnectionPoint` on a same-typed local (reproducible as `(IConnectionPoint)(object)x`), and per-element `stelem.i1` for the GUID bytes (reproducible as explicit stores). Still blocked by `ldarg 0` long form.
- `Finalize`: `lockTaken` store and nested `try`/`catch` shape, plus the `ldarg` encoding and the `public virtual` declaration.
- Verdict: **impossible** (encoding alone blocks `Init`).

### Non-bodied (54)

| Count | Row | Exact difference | Verdict |
|---:|---|---|---|
| 10 | coclass `*Class::.ctor` | Mathcad `assembly specialname rtspecialname ... runtime managed internalcall` (no body). `ComImport` classes reject any user constructor (CS0669, build-verified for `internal extern` form; plain forms are rejected by the same rule). | Impossible |
| 11 | `*EventHandler::.ctor(object, native uint)` (no Quire pair) | Roslyn emits `(object, native int)` (lab-verified). Pairing would need a judge rule change. | Impossible; maintainer rule decision |
| 22 | `*EventHandler::BeginInvoke`/`EndInvoke` | Roslyn emits both for any delegate declared in source (lab-verified). Mathcad's delegates have only `.ctor` and `Invoke`. | Impossible |
| 11 | `*EventHandler::.ctor(object, native int)` | Compiler-generated, Mathcad counterpart is `native uint` (row above). | Quire-only; not implemented |

## Maintainer decisions (not implemented)

- Whether the gate should treat a short and a long opcode form (`ldarg.0` vs `ldarg 0`, `ldc.i4.0` vs `ldc.i4 0`) as one instruction. The PLAN currently counts opcode forms. Without that decision, every Mathcad body that loads an argument (`ldarg N`) is unmatchable.
- Whether delegate constructors should pair `native uint` with `native int`. Even if paired, the Mathcad delegates lack `BeginInvoke`/`EndInvoke`, so the delegate rows stay incomplete.
- Whether locals-free methods should be judged on `maxstack` and InitLocals (Mathcad's exact values vs Roslyn's fixed 8/no-init). Under the current rules these 17 rows are unmatchable.

## Quire-only members

`BeginInvoke`/`EndInvoke` (22) and the `(object, native int)` delegate constructors (11) are compiler-generated for the delegate types. They have no Mathcad counterpart and are not implemented.
