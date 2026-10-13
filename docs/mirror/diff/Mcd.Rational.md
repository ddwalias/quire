# Mcd.Rational: black-box findings (in progress)

Observed by calling `mplrun.dll` from the local Mathcad Prime 12.0.1.0 install through reflection (PLAN.md §3c;
the licence permits local tests, per the user, 2026-10-14). No method bodies were read. `mplrun.dll` is IL-only
and targets net10.0, so it loads in a plain `AssemblyLoadContext`, even under Linux `dotnet`. `iengine.dll` is
mixed-mode and needs Windows x64.

Quire's current `Rational` differs from all of the following; none of it is implemented yet.

| Member | Observed behaviour |
|---|---|
| `.ctor(int n, int d)` | `g = GCD(n, d)` (below); `n /= g; d /= g` (so `(0,0)` throws `DivideByZeroException`); then if `d < 0`, checked negation of both (`(1, int.MinValue)` → `OverflowException`). `(5,0)` → `1/0`, `(-5,0)` → `-1/0`, `(0,-5)` → `0/1`. |
| `GCD(m, n)` | Euclid on the signed values (`(m, n) = (n, m % n)` until `n == 0`), then `Math.Abs` of the result. `GCD(0,0) = 0`; `GCD(int.MinValue, 0)` throws `Math.Abs`'s `OverflowException`. |
| `.ctor(int)`, `op_Implicit(int)` | `(n, 1)`. |
| `op_Multiply(R, R)` / `op_Division(R, R)` | Unchecked `(a.N*b.N, a.D*b.D)` / `(a.N*b.D, a.D*b.N)`, then the `(n, d)` constructor. 65536·65536 → `0/1`; `1/0 * 0` throws. |
| `op_Multiply(R, int)` / `op_Division(R, int)` | Unchecked `(a.N*b, a.D)` / `(a.N, a.D*b)`, then the constructor. |
| `op_Equality`, `op_Inequality`, `Equals(object)` | Field-wise (`0/2 != 0/1`, `2/4 != 1/2`, `0/0 == 0/0`); `Equals` is false for non-`Rational`. |
| `op_GreaterThan`, `op_LessThanOrEqual`, `op_GreaterThanOrEqual` | Compare `(double)N / D`: `1/0` = +∞, `0/0` = NaN (all false), and values equal as doubles compare equal. |
| `op_LessThan` | **Returns `a > b`** (identical truth table to `op_GreaterThan`, NaN included). A Mathcad bug the mirror must reproduce; check Quire callers before changing it. |
| `op_Implicit → double` | `(double)N / D`. |
| `op_Explicit → int`, `GetHashCode` | `N / D` (truncating; `D == 0` throws `DivideByZeroException`). |
| `IsIntegral` | `D == 1`. |
| `ToString` | `"N"` when `D == 1`, else `"N/D"` (`1/0`, `0/0` as is). Culture behaviour not yet checked. |
| `.ctor(double)`, `op_Implicit(double)` | `.ctor(x, 1e-5)`. |
| `.ctor(double x, double err)` | Continued-fraction convergents of `|x|` (floor recursion `r = 1/(r - a)` in doubles), sign restored afterwards. Stops at the first convergent with `diff <= err` (`x == err` → `0/1`; NaN `err` stops at the first convergent). NaN `x` → `0/1`; `|x| > int.MaxValue` → `±int.MaxValue/1`. Matches a straightforward implementation on 36,184 fuzzed cases for err ≥ 1e-9; **open** for err ≤ 1e-12, 0 or negative, see below. |

### Open: the constructor's limits at tiny `err`

- With `err = 0`, Mathcad goes past the first convergent whose `|x − p/q|` is 0 in doubles. Hypothesis: the
  difference is measured on the fractional part `x − floor(x)`, which keeps more precision. Not yet tested.
- With `err = -1` (never satisfied), it runs to an overflow bound. A numerator just over `int.MaxValue` is accepted
  and saturated (√2 → `2147483647/1757974584`, with the sign applied after saturation); `4500675881` is rejected.
  The bound looks like 2³² on the double numerator/denominator. Not yet tested.
- A partial quotient ≥ 2³⁰ is rejected: `1/2^30` → `0/1`, `1/(2^30−1)` → `1/1073741823`. But
  `1/(2+1/k)` → `1/2` for every `k` ≥ 2³⁰−1, and `5+1/k` → `5/1`, so the rule may be about `a` itself. Not yet
  settled.
