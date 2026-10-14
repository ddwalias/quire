# Execution checkpoint

Resume with [PLAN.md](PLAN.md). Phase 1 remains open and uncommitted; phase closure checks have not run.

## Batch 2b: MatVal / MatDim / Function matrix slice (completed)

**Completed work:** [Mcd.MatVal](evidence/Mcd.MatVal.md), [Mcd.MatDim](evidence/Mcd.MatDim.md),
[Mcd.MatVal.Access](evidence/Mcd.MatVal.md), [Mcd.MatVal.Collections](evidence/Mcd.MatVal.Collections.md),
[Mcd.MatVal.Dimensions](evidence/Mcd.MatVal.Dimensions.md), [Mcd.MatVal.Growth](evidence/Mcd.MatVal.Growth.md),
[Mcd.MatVal.Views](evidence/Mcd.MatVal.Views.md), [Mpl.Run.Cons](evidence/Mpl.Run.Cons.md), and
[Mpl.Run.Function.Matrix](evidence/Mpl.Run.Function.Matrix.md) bind exact signatures/tokens/hashes to the installed original.
- All original `MatVal` (293 members) and `MatDim` (69 members) generated method stubs have been replaced.
- Nested classes implemented: `c_dim`, `crazy_dim`, `r_dim`, `rc_dim`, `uniform_dim`, `AssignMatrixPtrs`, `ResizeMatrixPtrs`, `TrimSubMatrixPtrs`, and `arg_desc`.
- Matrix function application, delegate generation, vectorization, and `Cons` linked list manipulation are implemented.
- `docs/mirror/evidence/Mcd.RngVal.md` was created to restore the missing evidence record inherited from batch 2a, completely resolving the failing status gate.

**Live status:** `status/Quire.Mpl.txt` and `coverage.md`:
- Manifest across 30 projects: 5,384 mirrored types, 56,862 members:
  54,573 Stub, 1,027 Unclassified, 0 Blocked, 0 Documented, 534 Analyzed, 105 Tested, 546 Verified, 0 Deviation, 77 Abstract, 0 Quire-only.
- Settled members: 728 (1.3%).
- Shape test / manifest drift check: 30 projects, 0 missing manifest members.
- Status reference gate: 0 missing status references.

**Recorded validation:**
- In-process behavior tests (`Quire.Mpl.Tests`): **123 passed / 0 failed / 0 skipped**.
- Focused differential tests (`Quire.Focused.Differential.Tests` against Mathcad Prime 12.0.1.0): **24 passed / 0 failed / 0 skipped**.
- In-process engine tests (`Quire.Engine.Tests`): **94 passed / 0 failed / 0 skipped**.
- Metadata and shape check (`check.dll`): **passed, 0 mismatches, 0 missing references**.
- `git diff --check`: passed cleanly.

---

## Current batch 2c: Function and FunctionUtilities (callable runtime surface)

**Scope and objectives:**
Complete the remaining 86 stubs in `Mpl.Run.Function` and 13 stubs in `Mpl.Run.FunctionUtilities` to finish the core callable runtime in `Quire.Mpl`.

**Next exact members:**
1. `Mpl.Run.Function`:
   - Property: `InverseFunction` { get; set; } (0x06000786, 0x06000787)
   - Helpers: `format_vector(object[])` (0x06000788), `canonical_name(string)` (0x06000789)
   - Flags & options: `IgnoreVectorization()` (0x0600078b), `IgnoreVectorization(bool)` (0x0600078c), `OptFunc(int)` (0x0600078d), `DeclareInverse(Function)` (0x0600078e)
   - Conversions: `asFunction(string, uint)` (0x0600078f), `IsFunctionN(int)` (0x06000790), `asFunctionN(int, uint)` (0x06000791)
   - Licensing checks: `CheckPremium()` (0x06000792), `Premium()` (0x06000793)
   - Execution dispatch: `DynCall(...)` (0 to 16 arguments) (0x06000794–0x060007a4), `Call(...)` (0 to 16 arguments) (0x060007a5–0x060007b5)
   - Generic dispatch: `Call_0<R>()` through `Call_16<R, ...>()` (0x060007b6–0x060007c6), `Call_vec<R>(object[])` (0x060007cb)
   - Closure: `closure<CLO>(Delegate, CLO, int)` (0x060007c7)
   - Vector calling: `VecCall(object)` / `VecCall(object, object)` / `VecCall(object, object, object)` (0x060007c8–0x060007ca), `VecApply(object[])` (0x060007cc), `DynApply(object[])` (0x060007cd)
   - Matrix currying: `CurryMatVal()` (0x060007ce), `UnCurryMatVal()` (0x060007cf)
   - Factories: `ConstantFunction(object)` (0x060007d0), `VectorFunction(object)` (0x060007d1), `Fixpoint(Function)` (0x060007d8), `Compose(Function, Function)` (0x060007d9)
   - Delegates: `AsF<R, A, B, C>` (0x060007d2), `AsF<R, A, B, C, D>` (0x060007d3), `AsDelegate_3(...)` (0x060007d4, 0x060007d6), `AsDelegate_4(...)` (0x060007d5, 0x060007d7)
   - Apply variants: `Apply1(object)` (0x060007da), `Apply1(int)` (0x060007db), `Apply1(double)` (0x060007dc)

2. `Mpl.Run.FunctionUtilities`:
   - Array helpers: `concat(object[], object[])` (0x0600076a), `Push(object[], object)` (0x0600076d), `Push(object[], object, object)` (0x0600076e), `PushFront(object, object, object[])` (0x0600076c)
   - Method / constructor reflection caches: `FcnCtor`, `CloCtorGeneric`, `MethCtor`, `ObjMethCtor`, `CallMethod`, `CloCtor(Type)`, `InstMethCtor(Type)`, `GetCallMethod(Type, Type[])`

**Next action:**
1. Implement the remaining `FunctionUtilities` and `Function` members based on the same-artifact decompiled contracts and REA metadata.
2. Create/update evidence records `docs/mirror/evidence/Mpl.Run.Function.md` and `Mpl.Run.FunctionUtilities.md`.
3. Add unit and differential tests for `Function` and `FunctionUtilities`.
4. Refresh `MirrorGen manifest` and verify shape and status gates pass.
