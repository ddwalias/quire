# Mcd.Units

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Mcd/Units.cs`; tests: `Quire.Mpl.Tests.Mcd.UnitSystemTests`,
  `Quire.Differential.Tests.UnitSystemDifferentialTests`

Covers `Mcd.Unit` and `Mcd.Units`. A `Unit` has five public readonly fields: name, symbol, quantity, magnitude in
base units, and the `Type` parsed from its signature. A `Units` is a public readonly array of (unit, `Rational`
exponent) `Power`s and its magnitude. Neither validates its unit's dimension. Every member below is
differential-verified for the cases in "Test cases".

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Unit.ctor(string, string, string, double, string)` | 0x060005ba | `cab33a39` | Stores name, symbol, quantity and magnitude as given (zero, negative, infinite and NaN magnitudes included) and parses the signature with `Type.Parse`, so a bad signature throws the parser's exception. | High | Verified |
| `Unit.Dimensions` | 0x060005bb | `bca4ee97` | The type's dimension product (null for terms that have none); computed on each call. | High | Verified |
| `Unit.ToString()` | 0x060005bc | `411316bf` | `Unit{name="…",symbol="…",quantity="…",magnitude=…,type="…"}`, the magnitude in the current culture and the type in signature syntax. | High; current culture only | Verified |
| `Units.ctor(Power[])` | 0x060005bd | `53132444` | Keeps the array and multiplies each unit's magnitude raised to its exponent (as a double), starting from 1. Raises "floating point error" when the product, or its reciprocal, is infinite or NaN, so zero magnitudes fail. A null array throws `NullReferenceException`; an empty one gives magnitude 1. | High | Verified |
| `Units.ctor(Unit, Rational)` | 0x060005be | `7539aa67` | One power; magnitude is the unit's raised to the exponent. Raises "illegal factor" when that is exactly 0, "floating point error" when infinite or NaN. An exponent of 1/0 converts to +∞, so it fails for any magnitude other than ±1 ("illegal factor" for \|magnitude\| < 1, "floating point error" above). | High | Verified |
| `Units.ToString()` | 0x060005bf | `f5d5e3b2` | `{` then each unit's symbol, separated by spaces, followed by a space and the exponent's `Rational` text unless it equals 1, then `}`: `{kg m 2 s -3}`. | High | Verified |
| `Units.magnitude` | 0x060005c0 | `3d09bf0a` | The `Magnitude` field. | High | Verified |

**Errors.** "floating point error" and "illegal factor" are raised through `AppException.Raise`, so they arrive
as `AppException` with no argument (see `Mpl.Run.AppException.md`). The array constructor tests the
reciprocal as well, which is what rejects a zero product; the one-power constructor checks zero explicitly and
doesn't test the reciprocal, so a subnormal magnitude whose reciprocal overflows is accepted there.

Fields: `Unit.Name`/`Symbol`/`Quantity`/`Magnitude`/`Type` and `Units.Powers`/`Magnitude`, `Units.Power.Base`/`Exponent` are Verified (read directly by `UnitSystemDifferentialTests`).

## Test cases

**Observed** (`UnitSystemDifferentialTests.Units_and_unit_members`, against the installed `mplrun.dll`; outcomes
compared as text or exception type and message):
- `new Unit` on 35 specs: the eight SI-style base units and twelve derived units used by the unit-system tests, plus
  zero, +∞, NaN, negative, 1e-300 and 1e300 magnitudes, polymorphic (`V a`), matrix, function, bare-constant,
  malformed (`)`) and null signatures, and a fractional magnitude; for each that constructs, `ToString` and
  `Dimensions`;
- `new Units(unit, n/d)` for every such unit with exponents 1, 2, −1, 0, 1/2, −3/2, 2/3, 1/0, 1000 and −1000;
- 200 seeded random `Units(Power[])` of zero to three powers drawn from the same units and exponents, and
  `Units(null)`; each result shown as `ToString`, `Magnitude`, `magnitude` and the power list.

`Simplify`, `Normalize` and `GetUnits` results in the `UnitSystem` cases exercise both constructors and `ToString`
further (see `Mcd.UnitSystem.md`).

**Inferred** (`UnitSystemTests`): `Units` magnitude and text, `Unit` text and dimensions, and units of products.

## Open

- `Unit.ToString` and `Units.ToString` under a culture with a different decimal separator or negative sign were
  not compared.
