# Execution checkpoint

Resume with [PLAN.md](PLAN.md). Phase 1 remains open and uncommitted; phase closure checks have not run.

## Batch 2d: S-expression runtime types and helpers (completed)

**Completed work:** [Mpl.Run.Sexpr](evidence/Mpl.Run.Sexpr.md), [Mpl.Run.SCons](evidence/Mpl.Run.SCons.md),
[Mpl.Run.SList](evidence/Mpl.Run.SList.md), [Mpl.Run.Sym](evidence/Mpl.Run.Sym.md), [Mpl.Run.Atom](evidence/Mpl.Run.Atom.md),
[Mpl.Run.Tok](evidence/Mpl.Run.Tok.md), [Mpl.Run.Nul](evidence/Mpl.Run.Nul.md), [Mpl.Run.Qt](evidence/Mpl.Run.Qt.md),
[Mpl.Run.Bq](evidence/Mpl.Run.Bq.md), [Mpl.Run.Cm](evidence/Mpl.Run.Cm.md), [Mpl.Run.Ca](evidence/Mpl.Run.Ca.md),
[Mpl.Run.SAppend](evidence/Mpl.Run.SAppend.md), [Mpl.Run.Tag](evidence/Mpl.Run.Tag.md),
[Mpl.Run.char_constant](evidence/Mpl.Run.char_constant.md), [Mpl.Run.int_constant](evidence/Mpl.Run.int_constant.md),
[Mpl.Run.real_constant](evidence/Mpl.Run.real_constant.md), [Mpl.Run.str_constant](evidence/Mpl.Run.str_constant.md),
[Mpl.Run.Cons](evidence/Mpl.Run.Cons.md), [Mpl.Run.VecBuilder](evidence/Mpl.Run.VecBuilder.md),
[Mpl.Run.True](evidence/Mpl.Run.True.md), [Mpl.Run.Tuple](evidence/Mpl.Run.Tuple.md), and
[Mpl.MplSignature](evidence/Mpl.MplSignature.md) bind exact signatures/tokens/hashes to the installed original.
- S-expression hierarchy with `Tag`, `Sexpr`, `Nul`, `SCons`, `SList`, `Atom`, `Sym`, `Tok`, `Qt`, `Bq`, `Cm`, `Ca`, `SAppend`, and the constant tokens.
- Syntax tree walking, quoting, unquoting, pattern matching, constructor emission, and lexical symbol resolution.
- Runtime helpers: `Cons` lists with in-place mutation and length calculation, `VecBuilder` front-buffered arrays, `Tuple` / generic tuple wrappers `Tuple<T1..T5>`, and `MplSignature`.
- Quire engine Rl serialization cleanly separated into `Quire.Engine.Internal.Rl.Write` preserving original `ToString()` debugging semantics.
- Evidence records created: 22 records in `docs/mirror/evidence/`.
- Status references and metadata shape gate fully resolved: 0 missing status references, 0 missing manifest members across 30 projects.

**Live status:** `status/Quire.Mpl.txt` and `coverage.md`:
- Manifest across 30 projects: 5,384 mirrored types, 56,862 members:
  54,137 Stub, 953 Unclassified, 0 Blocked, 0 Documented, 786 Analyzed, 108 Tested, 801 Verified, 0 Deviation, 77 Abstract, 0 Quire-only.
- Settled members: 986 (1.7%).
- Shape test / manifest drift check: 30 projects, 0 missing manifest members.
- Status reference gate: 0 missing status references.

**Recorded validation:**
- In-process behavior tests (`Quire.Mpl.Tests` via `run.dll`): **159 passed / 0 failed / 0 skipped**.
- Focused differential tests (`Quire.Focused.Differential.Tests` via `run.dll` against Mathcad Prime 12.0.1.0): **72 passed / 0 failed / 0 skipped**.
- In-process engine tests (`Quire.Engine.Tests` via `run.dll`): **94 passed / 0 failed / 0 skipped**.
- Harness tests (`Quire.Harness.Tests` via `run.dll`): **11 passed / 0 failed / 0 skipped**.
- Python validation unit tests (`tools/validation`): **3 passed / 0 failed / 0 skipped**.
- Metadata and shape check (`check.dll`): **passed, 0 mismatches, 0 missing references**.
- `git diff --check`: **passed cleanly**.

---

## Current batch 2e: Remaining Mpl.Run unclassified & IEngine interfaces

**Scope and objectives:**
1. Settle remaining unclassified / stub members in `Quire.Mpl`:
   - `Mcd.DimVal.ApplyMatVal`
   - `Mpl.Run.Rts.GetTypeInitializer`
   - Annotate/settle remaining unclassified fields and helpers in `MatVal`, `NumVal`, `RngVal`, `Function`, `FunctionUtilities`.
2. Implement core engine interfaces in `Quire.Engine` / `Quire.Mpl` (`IEngine`, `IFactory`, `NodeCallback`, `NodeStatus`).

**Next action:**
1. Inspect decompiled contracts and REA metadata for `ApplyMatVal`, `GetTypeInitializer`, and the unclassified members in `mplrun.dll`.
2. Implement or annotate exact contracts in `src/Quire.Mpl/`.
3. Add tests and regenerate manifest status.
