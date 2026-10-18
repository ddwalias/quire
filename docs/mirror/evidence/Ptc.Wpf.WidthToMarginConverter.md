# Ptc.Wpf.WidthToMarginConverter

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcWpfWidthToMarginConverterMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ConvertBack(object, Type, object, CultureInfo)` | 0x0600047c | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Multiplier get` | 0x06000477 | MemberGen: `public double Multiplier { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Multiplier set` | 0x06000478 | MemberGen: `public double Multiplier { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Addition get` | 0x06000479 | MemberGen: `public double Addition { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Addition set` | 0x0600047a | MemberGen: `public double Addition { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcWpfWidthToMarginConverterMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
