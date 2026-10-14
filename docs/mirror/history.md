# Phase 1 implementation history

Historical snapshot, retained for implementation context. Current execution status is in [PLAN.md](PLAN.md) and [WORK.md](WORK.md). The descriptions below are provisional Quire behavior unless a current member evidence record establishes otherwise.

## 3a. Phase 1: engine runtime (type level done; method level open, §3c)

`Quire.Values` and the old `Quire.Mpl.Rl` S-expressions are gone. Their behaviour now lives in the mirrored `Mcd` and
`Mpl.Run` types inside `Quire.Mpl`. The interpreter sits behind Mathcad's `Engine.IEngine`/`IFactory` API. These
describe current Quire behavior, not blanket method-by-method equivalence; Phase 1's method-level review remains open
under §3c. Stubs fell
from 56,114 to 55,766. `Quire.Mpl` is now 21.1% implemented and `Quire.Engine` 13.7% (see
[`coverage.md`](coverage.md)).

### Quire behavior currently implemented (method-level equivalence not established)

| Mirrored type(s) | Behaviour |
|---|---|
| `Mcd.Dim` | Current Quire behavior interns dimensions, so two dimensions are equal only when they are the same object. Seven base exponents (length, mass, time, current, temperature, substance, luminosity) are stored as multiples of 1/2520, so roots and fractional powers stay exact; `Pwr` throws when the result isn't exact. The special markers are `identity`, `polymorphic` (null), `mcd_zero`, `string_dim`, `function_dim` and `mixed_matrix_dim`. |
| `Mcd.Type`, `Named`, `Constant`, `Product` | Dimension signatures such as `length*mass/time^2` (`1` = dimensionless). `Dim.New(Product)` interns one. |
| `Mcd.Rational` | Reduced fractions, plus a continued-fraction approximation from `double`. |
| `Mcd.Unit`, `Units`, `UnitSystem` | Units carry a signature `Type` and an SI magnitude. `UnitSystem.Simplify` returns the system's default unit for an exact dimension match, and otherwise base units with rational exponents. |
| `Mcd.DimVal`, `NumVal` (+ `num_val`), `StrVal`, `RngVal`, `MatVal` (+ `mat_val`) | `NumVal` is complex (`rp`/`ip`) with a `Dim`. `MatVal` is column-major (`m_rarr`/`m_iarr`, `Index = offset + c·anr + r`). Elements whose dimension differs from the matrix's, or that aren't numbers (nested matrices, strings, functions), go in the `m_parr` pointer table, with NaN in their slot. A matrix holding any such element has `mixed_matrix_dim`. `RngVal` is arithmetic (`m_init`, `m_incr`, `m_n`). |
| `Mpl.Run.Tag`, `Sexpr`, `Nul`, `SCons`, `Tok`, `Atom`, `Sym`, `int_constant`, `real_constant`, `str_constant` | Lists are `SCons`/`Nul`. Each atom is wrapped in a `Tok` that also holds its source handle (1-based offset). |
| `Mpl.Run.FunctionUtilities`, `Function`, `Rts` | User functions and built-ins are `Function`s. `Rts.RegisterThreadForCancellationChecking`/`IsCancellationRequested` carry cancellation per thread. |
| `Engine.Factory`, `IFactoryFactory`, `IFactory`, `IEngine`, `NodeCallback`, `BooleanCallback`, `NodeStatus`, `NodeType` | Mathcad's API only: `AddAfter`/`AddBefore`/`Move*`/`Update`/`Replace`/`Remove` with priorities and client cookies, `SetAutoCalc`, `CalculateNow`, and `GetValueSync`/`GetValueBeforeAsync`/`IsBoundAt`/`GetTopNodes`/`GetDefined`. Node callbacks report `(handle, status, cookie)`; the trigger arguments are always 0. The member signatures are Mathcad's. The *contents* of the returned dictionaries and the meaning of each status are not yet verified (§3b). |

### Decisions made during Phase 1

- **Results are `IDictionary`s**, because Mathcad's `GetValueSync` and its relatives are declared to return
  `IDictionary`. That matches the signature only. The keys, value types, optional entries, nesting and status
  meanings are Quire's provisional choices (`Quire.Engine.Internal.ResultKeys`), not verified Mathcad equivalents.
  Establishing the real contract is tracked as unfinished work in §3b.
- **The interpreter is still real-only.** `NumVal` has an imaginary part, but no operation produces one yet.
- **Mathcad's `NodeType` is the auto-eval flag** (`AUTO_EVAL_ON`/`OFF`). The engine's old region-kind enum is
  now the internal `NodeKind`.
- **Hand-written enums and delegates carry no `[MirrorOf]`.** The generator treats a tagged enum or delegate as
  its own output (`HandWritten.Find`). `NodeCallback`, `BooleanCallback`, `NodeStatus` and `NodeType` are therefore
  untagged, so the shape test doesn't cover them. They were checked by hand against `MirrorGen dump iengine`.
- **Stub overloads can capture calls.** A generated stub silently drops its arguments. The literal `0` in
  `new Tok(atom, 0)` binds to the `(Atom, int)` stub rather than the hand-written `(Atom, uint)` constructor.
  Overloads like that are now written by hand to forward their arguments; check for the same trap when a call
  site passes literals.
- **Edits never fail after taking effect.** With auto-calc on, a cancelled pass inside `AddAfter`/`Update`/… is
  swallowed and leaves the remaining nodes dirty. `GetValueSync` reports any node at or below the first dirty node as
  `NODE_NOT_COMPUTING`, with its previous value.
- **`Update` with an unchanged Rl sends no callback**, so the controller reposts the node's last result itself;
  otherwise a cosmetic edit would leave the region Pending.
- **The Rl reader is internal** (`Quire.Engine.Internal.RlReader`) until a later phase needs `Mpl.Run.Io`.

### Left open from the Phase 1 review

- `default(Rational)` is 0/0, because Mathcad's public `Numerator`/`Denominator` fields leave no room for a hidden
  offset. Mathcad's implicit `double → Rational` conversion is kept, and it throws for values that don't fit.
- Every `Dim.Mult`/`Div` allocates and takes the intern lock. Matrix display re-runs `UnitSystem.Simplify` for
  each element. Both are fast enough for worksheets today; cache per dimension if profiling says otherwise.
- Several generated stub constructors (`MatVal(double[])`, `MatVal(MatVal)`, `real_constant(string)`, …) would
  return empty values if hand-written code called them. None is called today. A check that fails on such calls
  would be a worthwhile shape-test addition.

### Quire-only code

These are internal and have no Mathcad counterpart:
- `Quire.Mpl.Internal.BuiltinUnits`: the unit table, the SI/USCS/CGS systems and constants.
- `Quire.Engine.Internal`:
  - `WorksheetEngine` and `EngineFactory`, which implement the API.
  - `Interpreter`, `Builtins`, `Ops`, `MatrixOps`, `Numerics` and `Env`/`UserFunction`/`BuiltinFunction`.
  - `NodeResult`/`ResultKeys`, `TraceVal` and the `Rl`/`Val` helpers.

On mirrored types, the Quire-only members are:
- On `Dim`: `Parse`, `Format`, `Coordinates`, `Denominator` and `BaseNames`.
- On `UnitSystem`: `m_defaults`.
- The convenience accessors on `MatVal` (`Rows`, `Cols`, `Row`, `Column`, `SubMatrix`) and `SCons` (`first`…`fifth`, `list1`–`list4`).

The manifest lists all of them.

## Reconciliation on 2026-10-14

Method review has started: [Mcd.Rational](evidence/Mcd.Rational.md) records 23 differential-verified members. It supersedes the old Rational behavior description and the claim that double conversion throws for out-of-range values. A differential harness exists in `tests/Quire.Differential.Tests`. Generated coverage now includes distinct member statuses, so the old implementation percentages and stub totals above are historical.
