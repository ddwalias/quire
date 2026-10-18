# Mcd.Rational

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Mcd/Rational.cs`; tests: `Quire.Mpl.Tests.Mcd.RationalTests`,
  `Quire.Differential.Tests.RationalDifferentialTests`

A struct with two public readonly int fields, `Numerator` and `Denominator`, and no hidden state, so `default` is
0/0. Every member below is differential-verified for the cases in "Test cases".

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `.ctor(int)` | 0x06000595 | `211fa244` | `n/1`. | High | Verified |
| `.ctor(int, int)` | 0x06000596 | `a49ec6b1` | `g = GCD(n, d)`, negated when `d < 0`; then `n / g`, `d / g` with ordinary int division. So `(0, 0)` throws `DivideByZeroException`, `(5, 0)` gives 1/0, and dividing `int.MinValue` by −1 throws `OverflowException`. | High | Verified |
| `.ctor(double)` | 0x06000597 | `a60ccc04` | `.ctor(x, 1e-5)`. | High | Verified |
| `.ctor(double, double)` | 0x06000598 | `701ce3cd` | Continued fraction of the fractional part of \|x\|. Iterates while the current convergent `p/q` differs from the fractional part by more than `err` **and** the remainder exceeds 2⁻³⁰. Each step inverts the remainder, takes its floor `a`, and forms the next convergent; it stops, keeping the current one, when `max(p + whole·q_next, q_next)` exceeds int.MaxValue (current numerator with the next denominator). The result is `(int)(p + whole·q) / (int)q`, negated afterwards when `x < 0`. Conversions saturate (.NET 9+), so NaN → 0/1 and \|x\| > int.MaxValue → ±int.MaxValue/1. | High; relies on the runtime's saturating double→int conversion, which Quire shares | Verified |
| `IsIntegral` | 0x06000599 | `fdf218bb` | `Denominator == 1`. | High | Verified |
| `op_Implicit(int)` | 0x0600059a | `647b1903` | `.ctor(int)`. | High | Verified |
| `op_Implicit(double)` | 0x0600059b | `955160bf` | `.ctor(double)`. | High | Verified |
| `op_Implicit → double` | 0x0600059c | `27b74016` | `(double)N / D` (so 1/0 → +∞, 0/0 → NaN). | High | Verified |
| `op_Explicit → int` | 0x0600059d | `d7bd05ad` | `N / D`, truncating; throws for `D == 0`. Same body as `GetHashCode` (equal hashes). | High | Verified |
| `op_Equality` | 0x0600059e | `8c132856` | Both fields equal. | High | Verified |
| `op_Inequality` | 0x0600059f | `17512f03` | Either field differs. | High | Verified |
| `op_LessThanOrEqual` | 0x060005a0 | `e03a7d4f` | Compares the doubles. | High | Verified |
| `op_GreaterThanOrEqual` | 0x060005a1 | `1740f5c3` | Compares the doubles. | High | Verified |
| `op_LessThan` | 0x060005a2 | `45afbcb0` | **Returns `a > b`**: its normalized IL equals `op_GreaterThan`'s. Kept as is (see below). | High | Verified |
| `op_GreaterThan` | 0x060005a3 | `45afbcb0` | Compares the doubles. | High | Verified |
| `op_Multiply(R, int)` | 0x060005a4 | `4614a923` | `(N·k, D)` unchecked, then `.ctor(int, int)`. | High | Verified |
| `op_Division(R, int)` | 0x060005a5 | `9796b1f2` | `(N, D·k)` unchecked, then `.ctor(int, int)`. | High | Verified |
| `op_Multiply(R, R)` | 0x060005a6 | `3bc0b7e2` | `(Na·Nb, Da·Db)` unchecked, then `.ctor(int, int)`; no cross-cancelling, so 65536·65536 → 0/1. | High | Verified |
| `op_Division(R, R)` | 0x060005a7 | `530674ae` | `(Na·Db, Da·Nb)` unchecked, then `.ctor(int, int)`; dividing by 0 gives `x/0`, it doesn't throw. | High | Verified |
| `Equals(object)` | 0x060005a8 | `f2298645` | `o is Rational` and `==`. | High | Verified |
| `GetHashCode` | 0x060005a9 | `d7bd05ad` | `N / D`; throws for `D == 0`, `default` included. | High | Verified |
| `ToString` | 0x060005aa | `2a8523d2` | `N` when `D == 1`, else `N/D`; ints formatted with the current culture. | High; non-invariant cultures not compared | Verified |
| `GCD(int, int)` | 0x060005ab | `b90fbcf0` | Euclid on the signed values (`(m, n) ← (n, m mod n)` until `n == 0`), then `Math.Abs(m)`: `GCD(0, 0) = 0`, `GCD(int.MinValue, 0)` throws `OverflowException`. | High | Verified |

**`op_LessThan`.** This is Mathcad's behaviour, so the mirror reproduces it; it is not a Quire deviation. Quire code
that needs a real less-than compares the doubles. No Quire caller uses `<` on `Rational` today.

**Changes from the earlier Quire `Rational`** (pre-§3c, unclassified): it rejected zero denominators, reduced with
checked long arithmetic, cancelled across before multiplying, compared exactly, threw on non-integral `(int)`
casts, used an error bound of 1e-9 and threw for NaN, ∞ and out-of-range doubles. All of these differed from
Mathcad and are gone. Its only callers (`Dim.Format`, `Units`) build small reduced fractions, which behave the same.

## Test cases

**Observed** (`RationalDifferentialTests`, against the installed `mplrun.dll`; outcomes compared as fields, values or
exception type):
- integer constructors, `op_Implicit(int)` and `GCD` on all pairs of 18 ints, including 0, ±1, 65536, 46341,
  ±int.MaxValue and int.MinValue;
- `.ctor(double)`, `op_Implicit(double)` and `.ctor(double, double)` on 26 chosen doubles (NaN, ±∞, ε, 1e-300,
  1e10, 4500675881, 2⁻³⁰, 1/(2³⁰−1), …) plus 3,000 seeded random ones, each with errors 1e-5, 1e-2, 0.5, 1e-9, 1e-12,
  0, −1 and NaN;
- every operator, `Equals`, `GetHashCode`, `ToString` and `IsIntegral` on all pairs of the rationals built from 9
  ints (plus `default`), and the int overloads against all 18 ints.

The first run of the double case found Quire using the next numerator in the overflow bound; Mathcad uses the
current one. Fixed before recording.

**Inferred** (`RationalTests`, expected values worked out from the contract above): reduction and sign, zero
denominators, GCD signs, wrap-around products, field-wise equality, the inverted `<`, conversions, hash, text,
convergents of π at 1e-5 and 1e-2, and the limits at NaN, ±1e10 and 2⁻³⁰.

## Open

- `ToString` under cultures with a non-ASCII negative sign was not compared.
