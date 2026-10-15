# Lane A work checkpoint

## DimVal differential sensitivity follow-up

Branch `mirror/a-dimval-sensitivity` restored on `c011f35` and rebased onto `b107792`, contains only the stronger
`tests/Quire.Differential.Tests/DimValDifferentialTests.cs` from `73c53d5`; it does not import the old
branch's ApplyMatVal implementation, generated declaration deletion or status changes. Relevant case
contracts and renderer limits are in [DimVal evidence](../evidence/Mcd.DimVal.md#differential-test-sensitivity-follow-up-2026-10-15).
This batch is Lane A's DimVal base-test sensitivity work; Lane B retains `DimVal.ApplyMatVal` ownership.

Historical run `/tmp/quire-b-dimval-rebased-1ab5e0f/mutation.json` recorded **111 mutants: 92 killed,
19 proposed equivalents, 0 accepted, 0 other survivors; complete=false**. These are old-run results,
not a current branch pass. Its summary used focused validation with `method_types=[Quire.Mcd.DimVal]`
and `/tmp/quire-dimval-proposed-mutants.txt`. The original branch was not merged; current main has the
older test cases. Targeted direct comparisons on restoration tip `e1f548c` passed: `QUIRE_REQUIRE_MATHCAD=1 dotnet test
tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj --filter
'FullyQualifiedName~DimValDifferentialTests' -m:1 -nr:false -p:UseSharedCompilation=false` ran 4 facts,
0 failures and 0 skips. Independent review confirmed the tests and all 19 proofs; no new mutation or
full-gate pass is claimed. Next: obtain the required user approval, then pass the authoritative full gate. Do not add proposal keys to protected acceptance
without the required review and user approval. Pending proposals do not prevent unrelated work.

### Existing 19 DimVal proposals, still pending

Exact historical scratch file: `/tmp/quire-dimval-proposed-mutants.txt`. Its contents are preserved
below, rather than treating a temporary path as durable evidence. All mutate only the actual-arity
literal at IL_0002 in base `DynCall` (1–16 arguments) or `VecCall` (1–3 arguments). In the mirrored
base implementations, the required arity remains -1; nonvirtual `BadCall(-1,actual)` calls
`OnError(errpos,"bad_function",this)`, which throws before `WrongNumberOfArguments(required,actual)`.
Consequently the changed actual count is not observed. The old Lane B checkpoint at `73c53d5`
records independent proof review, but these keys remain proposals, not accepted or newly approved.
Confirm the same original-artifact call path before any maintainer acceptance.

```text
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.1 -> ldc.i4.0 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.2 -> ldc.i4.3 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.3 -> ldc.i4.4 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.4 -> ldc.i4.5 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.5 -> ldc.i4.6 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.6 -> ldc.i4.7 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.7 -> ldc.i4.8 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.8 -> ldc.i4.7 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.s 9 -> 10 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.s 10 -> 11 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.s 11 -> 12 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.s 12 -> 13 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.s 13 -> 14 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.s 14 -> 15 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.s 15 -> 16 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.s 16 -> 17 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::VecCall`0([System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.1 -> ldc.i4.0 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::VecCall`0([System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.2 -> ldc.i4.3 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
Quire.Mcd.DimVal::VecCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)->[System.Runtime]System.Object|False@IL_0002 ldc.i4.3 -> ldc.i4.4 # base overload calls OnError with bad_function before WrongNumberOfArguments, so the actual-arity constant is unreachable
```

Updated: 2026-10-15. Phase 1 remains open; Windows phase closure has not run.
Read [PLAN](../PLAN.md) before starting a batch.

## Ownership

Lane A owns the S-expression family and runtime helpers (`Cons`, `VecBuilder`, `True`, `Tuple`,
`MplSignature`), `Rational`, `Utilities`, `AppException`, `Type`, `UnitSystem`, `RuntimeOptions`,
and remaining runtime batch members outside Lane B. Lane B owns `MatVal*`, `MatDim`, `Function*`,
`FunctionUtilities`, `DimVal.ApplyMatVal`, and `Quire.Engine`.

## Tok.walk_pattern batch

Merged to `main` at commit `71c16c5dc2fb` after passing independent review and the runtime merge gate (Harness 36, Mpl 160, Differential 88, Mutation 110/111 killed, 0 survivors).

## SCons.walk_pattern batch

Merged to `main` at commit `92e763bf581a` after passing independent review and the runtime merge gate.

## Environment helpers batch (mirror/a-env-helpers)

- Branch: `mirror/a-env-helpers`
- Worktree: `/root/src/quire-env-helpers`
- Scope: Implementation of `MplEnv`, `BindingInfo`, `MacroBindingInfo`, `LocalEnv` (leaving `AddFunctionMacro` and `AddForeignMacro` stubs for subsequent batches), `MplMacro`, and `MplSymbolMacro` in `src/Quire.Mpl/Run/Env.cs`.
- Contracts & Behavior:
  - `Mpl.Run.MplEnv`: `Level => 0`; base mutation methods (`AddLocal`, `AddLocalEquation`, `AddSymbolMacro`, `AddFunctionMacro`, `AddForeignMacro`) reject modifications by calling `MayNotModify()`; `LoadMacro(Sym)` returns `null` for `Keyword` symbols and otherwise delegates to `lookup(s)?.macro`; `LoadForeignMacro` calls `CanNotResolve(sig)`; `CanNotResolve` throws `AppException("Can not resolve signature")`; `MayNotModify` throws `AppException("May not modify import environment")`; protected parameterless constructor.
  - `Mpl.Run.BindingInfo`: `m_handle`, `m_zone`, `macro => null`, constructors `BindingInfo()`, `BindingInfo(uint)`, `BindingInfo(uint, int)`. Constants `Value = 0` and `Macro = 1` preserved in `.g.cs`. Subclass `MacroBindingInfo`: `m_macro`, `macro => m_macro`, constructor `(handle, macro)` calling `base(handle, 1)`.
  - `Mpl.Run.LocalEnv`: `m_level`, `m_parent`, `m_table` (`SortedDictionary<string, BindingInfo>`); `Level => m_level`; constructors `LocalEnv(parent)` and `LocalEnv()`; `lookup_local` searches `m_table`; `lookup` queries `lookup_local` then delegates to `m_parent.lookup` (null parent throws `NullReferenceException` on miss); `LoadForeignMacro` delegates to `m_parent.LoadForeignMacro`; `AddLocal` adds new binding and throws `ArgumentException` on duplicates; `AddLocalEquation` sets/overwrites binding; `AddSymbolMacro` registers `MacroBindingInfo` wrapping `MplSymbolMacro`.
  - `Mpl.Run.MplMacro`: `m_name`, constructor `(sym)`, default `ExpandRef` / `ExpandCall` calling `IllegalMacroRef` / `IllegalMacroCall` throwing `AppException("illegal macro reference" / "illegal macro call", 0u, handle)`; static `NotAtTopLevel` and `CanNotExpand` throwing `AppException`; `ToString() => m_name.ToString()`.
  - `Mpl.Run.MplSymbolMacro`: `m_replacement`, constructor `(s, rep)` calling `base(s)`, `ExpandRef => m_replacement`, `ExpandCall` replaces head with `m_replacement` while preserving tail, static `make(Atom s, int n)` wrapping `int_constant.NewTok`, `ToString() => "MplSymbolMacro: " + m_name + " " + m_replacement`.
- Evidence files:
  - [Mpl.Run.MplEnv](../evidence/Mpl.Run.MplEnv.md)
  - [Mpl.Run.BindingInfo](../evidence/Mpl.Run.BindingInfo.md)
  - [Mpl.Run.LocalEnv](../evidence/Mpl.Run.LocalEnv.md)
  - [Mpl.Run.MplMacro](../evidence/Mpl.Run.MplMacro.md)
  - [Mpl.Run.MplSymbolMacro](../evidence/Mpl.Run.MplSymbolMacro.md)
- Validation receipts:
  - `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 166 passed, 0 failed, 0 skipped.
  - `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 94 passed, 0 failed, 0 skipped.
  - `git diff --check` — passed cleanly.
  - `python3 tools/validation/merge_gate.py mirror/a-env-helpers` — passed (90/90 mutants evaluated, 0 unaccepted survivors, ratchet ok, 0 lint findings).
- Unblocks: direct comparisons and full coverage of lowercase symbol handling in all AST walk methods (`Tok.walk_expression`, `Tok.walk_pattern`, `SCons.walk_pattern`, `SCons.walk_expression`, `Sym.walk_*`).

## Tok.walk_expression batch (mirror/a-tok-expression)

- Branch: `mirror/a-tok-expression`
- Worktree: `/root/src/quire-tok-expression`
- Scope: Verification of `Tok.walk_expression(MplEnv)` in `src/Quire.Mpl/Run/Sexpr.cs`.
- Contracts & Behavior:
  - `Mpl.Run.Tok.walk_expression(MplEnv env)`:
    - If `Key == null || !Key.LowId`: returns `this` immediately without environment lookup.
    - If `Key.LowId`: loads macro via `env.LoadMacro(Key)`.
    - If macro is null: returns `this`.
    - If macro is present: returns `macro.ExpandRef(env, this).walk_expression(env)`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x0600006e.
  - Exception and handle propagation:
    - When `ExpandRef` is called on an `MplMacro` that does not override `ExpandRef` (function macro), it invokes base `MplMacro.ExpandRef`, throwing `AppException("illegal macro reference", 0u, handle)`.
    - Unbound lowercase symbols in empty `LocalEnv()` throw `NullReferenceException` due to null parent dereference on miss, matching Mathcad.
    - Empty or null symbol names throw `ArgumentOutOfRangeException` / `ArgumentNullException` from `LowId` (`char.IsLower`), matching Mathcad.
- Evidence files:
  - [Mpl.Run.Tok](../evidence/Mpl.Run.Tok.md): `walk_expression` updated to `Verified`.
- Test counts & Validation receipts:
  - Unit tests: `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 172 passed, 0 failed, 0 skipped.
  - Differential tests: `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 93 passed, 0 failed, 0 skipped.
  - Direct comparisons added: bound symbol macro, chained symbol macros, bound function macro (`AppException` "illegal macro reference"), scoped `LocalEnv(parent)` resolution and shadowing, bound local variable returning `this`, and enabled lowercase symbol comparison in empty `LocalEnv()`.
  - Format checks: `git diff --check` passed cleanly.

## SCons.walk_expression and Sym.walk_expression_as batch (mirror/a-scons-expression)

- Branch: `mirror/a-scons-expression`
- Worktree: `/root/src/quire-scons-expression`
- Scope: Verification of `SCons.walk_expression(MplEnv)`, `SCons.walk_expressions(MplEnv, SCons)` in `src/Quire.Mpl/Run/Sexpr.cs`, and `Sym.walk_expression_as(MplEnv, SCons)` in `src/Quire.Mpl/Run/Atom.cs`.
- Contracts & Behavior:
  - `Mpl.Run.Sym.walk_expression_as(MplEnv env, SCons form)`:
    - If `LowId`: loads macro via `env.LoadMacro(this)`.
    - If macro is present: invokes `macro.ExpandCall(env, form).walk_expression(env)`.
    - If macro is null (or !LowId): walks arguments with `SCons.walk_expressions(env, form.tail)`. If arguments changed, returns `new SCons(form.head, expr)`; if unchanged, returns `form`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x06000097. Attribute updated to `[MirrorVerified("SexprDifferentialTests")]`.
  - `Mpl.Run.SCons.walk_expression(MplEnv env)`:
    - If `head.is_sym`: delegates to `head.sym.walk_expression_as(env, this)`.
    - If `head` is not a symbol (e.g. integer constant, nested list): wraps as `(apply e1 e2 ...)` via `new SCons(new Tok(apply_sym, head.handle), this).walk_expression(env)`.
    - Null head throws `NullReferenceException` when checking `head.is_sym`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x0600004b. Already `[MirrorVerified]`.
  - `Mpl.Run.SCons.walk_expressions(MplEnv env, SCons exps)`:
    - If `exps == null`: returns `Nul.singleton`.
    - Walks `exps.head.walk_expression(env)` and `walk_expressions(env, exps.tail)`. If neither changed (by reference equality), returns `exps` unchanged; otherwise constructs and returns `new SCons(expr, expr2)`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x0600004a. Already `[MirrorVerified]`.
  - Exception and handle propagation:
    - `ExpandCall` on base `MplMacro` (or function macro not overriding `ExpandCall`) invokes `IllegalMacroCall`, throwing `AppException("illegal macro call", 0u, form.handle)`.
    - Unbound lowercase symbols in empty `LocalEnv()` throw `NullReferenceException` due to null parent dereference on miss, matching Mathcad.
    - Non-symbol heads preserve source handle in generated `apply` token (`head.handle`).
- Evidence files:
  - [Mpl.Run.Sym](../evidence/Mpl.Run.Sym.md): `walk_expression_as` updated to `Verified`.
  - [Mpl.Run.SCons](../evidence/Mpl.Run.SCons.md): `walk_expression` and `walk_expressions` verified.
- Test counts & Validation receipts:
  - Unit tests: `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 181 passed, 0 failed, 0 skipped.
  - Differential tests: `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 100 passed (focused on SexprDifferentialTests: 9 passed, 0 failed, 0 skipped).
  - Direct comparisons added:
    - `Sym_members`: bound local (unchanged vs changed args), bound symbol macro, chained symbol macros, function macro throwing `AppException("illegal macro call", 0u, form.handle)`, nested `LocalEnv(parent)` scoping and shadowing.
    - `SCons_members`: symbol macro call, function macro call throwing `AppException("illegal macro call")`, non-symbol heads `(1 2 3)` and `((a b) c)` wrapping in `apply` token, `walk_expressions` returning new chain when macro args change.
  - Format checks: `git diff --check` passed cleanly.

## DimVal and RngVal caller coverage batch (mirror/a-caller-coverage)

- Branch: `mirror/a-caller-coverage`
- Worktree: `/root/src/quire-caller-coverage`
- Scope: `tests/Quire.Differential.Tests/DimValDifferentialTests.cs` and `tests/Quire.Differential.Tests/RngValDifferentialTests.cs`.
- Objective: Eliminate latent caller survivors in `DimVal` (`get_IsDimlessReal`, `asDimlessReal`, `matrix_set0_1`, `matrix_set0_2`) and `RngVal` (`asRealVector`) that blocked Lane B integration of `mirror/b-matval-pointer-slot-writers`.
- Contracts & Behavior:
  - Detailed matrix element formatting: `ShowVal` in `DimValDifferentialTests` and `RngValDifferentialTests` now delegates matrix formatting to `MatValDifferentialTests.Storage(side, v)`, making differential assertions sensitive to matrix dimensions, strides, flags, and individual real/imag/pointer array elements.
  - Value classifier sensitivity: Added direct comparisons on `NumVal` (dimensionless real, dimensioned real, complex) and `StrVal` for `get_IsDimlessReal` and `asDimlessReal` overloads.
  - Loop-start sensitivity: Added non-zero initial range test case (`MakeRange(2.5, 1.0, 3, "L")`) for `RngVal.asRealVector` to kill `i = 0 -> 1` loop-start mutations that were hidden when element 0 was already `0.0`.
- Validation receipts:
  - Scoped mutation run on `RngVal` (`/tmp/quire-validation-6yq8d9_v/summary.json`): 87/88 killed (0 survivors in `asRealVector`).
  - Scoped mutation run on `DimVal` (`/tmp/quire-validation-pxnmzxm4/summary.json`): 85/111 killed (0 survivors in `get_IsDimlessReal`, `asDimlessReal`, `matrix_set0_1`, `matrix_set0_2`; all 11 former Lane B blockers eliminated).
  - Unit tests: `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 181 passed, 0 failed, 0 skipped.
  - Differential tests: `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 104 passed, 0 failed, 0 skipped.
  - Format checks: `git diff --check` passed cleanly.

## Current method audit, separate from historical merges

Source: `/tmp/quire-workflow-trim-runtime-gate/run/method-coverage.json`, run
`e4ecd053-57b6-4787-a8ac-e284bebae831`, carried into baseline commit `c011f35`.
This is a previously executed runtime gate report, not a new validation run for this documentation change.
Its project-wide counts are 719 compared, 925 missing, 137 structure mismatches, 42 uncovered and
81 unsupported Verified statuses; `complete` is false. A passing ratchet permits existing gaps and
never establishes phase completion.

| Lane A type | Missing direct comparisons | Uncovered methods | Unsupported Verified statuses |
|---|---:|---:|---:|
| `Atom` | 1 | 0 | 1 |
| `Sexpr` | 1 | 6 | 6 |
| `SCons` | 0 | 3 | 1 |
| `Sym` | 0 | 3 | 1 |
| `Tok` | 0 | 2 | 0 |
| `AppException` | 0 | 1 | 1 |
| `Mcd.Type` | 1 | 0 | 0 |

Exact remaining members in those types:

- Missing direct comparisons: `Atom..ctor(int)`, `Sexpr..ctor(int)`, `Mcd.Type..ctor()`.
  Abstract base construction needs a direct comparison approach; caller coverage does not satisfy it.
- `Sexpr`: `get_quotation`, `get_quote`, `get_scons`, `WalkModule(MplEnv,Sexpr)`,
  `walk_pattern(MplEnv)`, `walk_expression(MplEnv)`.
- `SCons`: `walk_declaration_lhs(MplEnv)`, `walk_pattern(MplEnv)`, `walk_expression(MplEnv)`.
- `Sym`: `walk_declaration_as(MplEnv,SCons)`, `walk_definition_as(MplEnv,SCons)`,
  `walk_expression_as(MplEnv,SCons)`.
- `Tok`: `walk_pattern(MplEnv)`, `walk_expression(MplEnv)`.
- `AppException.Raise(object[],uint)`.

Some uncovered locations follow helpers that throw. Verify original reachability before proposing exact
unreachable-line entries; never reshape source to hide them. No new acceptance proposal is made here.
This is a queue for the listed scopes, not an exhaustive list of all remaining Lane A runtime types.
`Rational`, `Utilities`, `UnitSystem` and `RuntimeOptions` have no missing/structure/uncovered methods in
this report; their passing recorded cases still do not prove every possible input.

## NumVal.Compare scope correction

Prepared `mirror/a-numval-compare` on `c011f35`, rebased onto `7c5d9aa`, and merged the reviewed
`57872bf` through the runtime gate to main `b107792`. WORK retains its short index. The requested `Mcd.NumVal.Compare` has no source/generated declaration, current manifest
row or member/token in the installed original. No implementation or test should be invented for it.
Artifact identity and the original inspection command are in [NumVal evidence](../evidence/Mcd.NumVal.md).
The obsolete Lane B blocker is corrected in [Lane B](lane-b.md).

Changes: this checkpoint, the NumVal evidence scope note and Lane B's NumVal note. No runtime behavior,
tests, manifest or protected policy changed. Source/manifest inspection and `git diff --check` passed;
the final evidence-change runtime gate passed Harness 36, Mpl 159 and Differential 79, with no failures
or skips, lint 0 and ratchet ok. No methods were selected for mutation. Receipt:
`/tmp/quire-numval-final-runtime-gate/gate.json`; validator exit 3 records existing project-wide gaps,
not phase completion. The reviewed branch and gate-approved main were pushed to origin.

## Historical batch results

These are results recorded at the named historical merges, not current completion claims. The old
Unit 4 claim of full settlement was a classification checkpoint and is superseded by the audit above.

| Unit | Types / Scope | Branch | Status | Direct Comparisons | Structure Mismatches | Lint | Ratchet | Mutants Killed |
|---|---|---|---|---|---|---|---|---|
| **Unit 1** | Sexpr family (18 types) | `main` (`929fc6d`) | **Merged to main** via `merge_gate.py --apply` | 157 | 0 | 0 | ok | 106 / 107 (1 accepted survivor) |
| **Unit 2** | `Cons`, `VecBuilder`, `True`, `Tuple`, `MplSignature` | `main` (`20b2017`) | **Merged to main** via `merge_gate.py --apply` | 40 | 0 | 0 | ok | 87 / 87 (0 survivors) |
| **Unit 3a** | `Rational`, `Utilities` | `main` (`4fc4a72`) | **Merged to main** via `merge_gate.py --apply` | 31 | 0 | 0 | ok | 114 / 117 (3 accepted survivors) |
| **Unit 3b** | `AppException`, `Type`, `UnitSystem`, `RuntimeOptions` | `main` (`d7b5a63`) | **Merged to main** via `merge_gate.py --apply` | 165 | 0 | 0 | ok | 655 / 678 (23 accepted survivors) |
| **Unit 4** | Remaining batch 2e unclassified members | `main` | **Historical classification checkpoint only; current audit gaps remain** | Not established | Not established | Not rerun | Not rerun | N/A |


Historical mutant proposals and reasoning remain in Git history. They are not a current approval queue;
consult `accepted-mutants.txt` for existing accepted keys. Under current PLAN rules, test-host crashes
count as killed mutants and must never be proposed for equivalence acceptance.
