# Mcd.Utilities

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Mcd/Utilities.cs`, `src/Quire.Mpl/Generated/Mirror/Mcd.Utilities.g.cs` (stubs);
  tests: `Quire.Mpl.Tests.Mcd.UtilitiesTests`, `Quire.Differential.Tests.UtilitiesDifferentialTests`

A sealed class of static numeric helpers with a public parameterless constructor and no state. Every
member below is differential-verified for the cases in "Test cases".

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `ScaleVector(double[], double)` | 0x06000708 | `6a15fd7e` | Divides every element by the scalar in place, last to first, with IEEE results (division by 0 gives ±∞ or NaN). Touches element 0 first, so an empty array throws `IndexOutOfRangeException` and null `NullReferenceException`. | High | Verified |
| `Divide(object, double)` | 0x06000709 | `6157f65c` | Returns value unchanged when scalar is 1. Raises AppException "illegal factor" when scalar is 0. Divides NumVal or MatVal by scalar, and returns any other value unchanged. | High | Verified |
| `Multiply(object, double)` | 0x0600070a | `37ec8044` | Returns value unchanged when scalar is 1; otherwise delegates to Divide(value, 1.0 / scalar). | High | Verified |
| `GetUnits(Type.Product)` | 0x0600070b | `320efee7` | Null for a dimensionless or polymorphic product; otherwise the product normalized to basis units in the calling thread's unit system. A null product throws `NullReferenceException`; an unknown dimension `ArgumentException` "bad dimension". | High; needs a synced thread | Verified |
| `IsFinite(double)` | 0x0600070c | `9f109053` | Neither infinite nor NaN (any NaN payload). | High | Verified |
| `AssertFinite(double)` | 0x0600070d | `ffc796c4` | Raises `AppException` "floating point error" (no argument) for an infinite or NaN value; returns otherwise. | High | Verified |
| `AssertFinite(num_val)` | 0x0600070e | `bc320517` | `AssertFinite` of the real part, then of the imaginary part. | High | Verified |
| `.ctor()` | 0x0600070f | `41ad57d1` | No state. | High | Verified |

## Test cases

**Observed** (`UtilitiesDifferentialTests`, against the installed `mplrun.dll`; outcomes compared as values or
exception type and message):
- `IsFinite` and `AssertFinite(double)` on 17 doubles: ±0, ±1, ±1e308, `double.MaxValue`, `double.Epsilon`, NaN, ±∞,
  a signalling-NaN bit pattern, a negative quiet NaN, and NaNs with bit 32 set;
- `AssertFinite(num_val)` on each of those as the real part with imaginary parts 0, NaN and +∞;
- `ScaleVector` on `[1, 2, 3]`, `[1, 2]`, `[]`, `[NaN, 0]` and `[-4]` by 2, 0, −0.5 and NaN, comparing the array afterwards,
  and on null;
- `Divide` on `NumVal`, `MatVal`, string and double by 1, 0, 2, -0.5 and NaN;
- `Multiply` on `NumVal`, `MatVal`, string and double by 1, 0, 2, -0.5 and NaN;
- `GetUnits` of the products of `V{Length 60000}`, `V{}`, `V{a Length 60000}`, `V{Length 30000 Time-60000}`,
  `V{Foo 60000}` and `V{Money 60000 Mass-120000}`, and of null, on the default SI system;
- the constructor.

**Inferred** (`UtilitiesTests`): scaling divides in place, and non-finite values are floating point errors.

## Open

- `GetUnits` was compared only under the default SI system.

## Release IL shape follow-up (2026-10-19)

`GetUnits(Type.Product)` (`0x0600070b`) now reports an exact Release instruction match in
`python tools\validation\il_diff.py` against the Mathcad artifact above. Its existing direct comparison covers
dimensionless, polymorphic, ordinary and unknown products, plus null; the required Release
`UtilitiesDifferentialTests` passed 3/3 with Mathcad. The default SI unit system is the only directly compared system.
