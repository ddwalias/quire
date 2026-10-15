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

- Branch: `mirror/a-tok-pattern`
- Scope: `Quire.Mpl.Run.Tok.walk_pattern(MplEnv)` and tests in `tests/Quire.Differential.Tests/SexprDifferentialTests.cs` and `tests/Quire.Mpl.Tests/Run/SexprTests.cs`.
- Contract & Behavior:
  - Non-symbol tokens return `this`.
  - Symbols with empty/null name throw argument exceptions on `UpId` / `LowId` access, matching Mathcad.
  - Uppercase symbols dispatch to `env.LoadMacro(key)`. In Mathcad with `new LocalEnv()`, this raises `NullReferenceException` on `m_parent` lookup; in Quire `LoadMacro` remains a stub (`[MirrorAnalyzed]`).
  - Lowercase symbols dispatch to `env.AddLocal(key, handle)` and return `this`. In Quire behaviour tests, `TrackingEnv` verifies local registration and instance return (covering lines 518–519).
- Validation receipts:
  - `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj --filter 'FullyQualifiedName~SexprTests' -m:1 -nr:false -p:UseSharedCompilation=false` — 16 passed, 0 failed, 0 skipped.
  - `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj --filter 'FullyQualifiedName~SexprDifferentialTests' -m:1 -nr:false -p:UseSharedCompilation=false` — 9 passed, 0 failed, 0 skipped.
  - `git diff --check` — passed.
- Next: independent review, then merge gate. Subsequent batches: `SCons.walk_pattern(MplEnv)`, then `Tok.walk_expression(MplEnv)`.

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
