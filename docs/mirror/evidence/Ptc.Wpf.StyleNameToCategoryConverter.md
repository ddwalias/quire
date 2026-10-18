# Ptc.Wpf.StyleNameToCategoryConverter

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcWpfStyleNameToCategoryConverterMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ConvertBack(object, Type, object, CultureInfo)` | 0x06000432 | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x06000433 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CategoryLegacy get` | 0x0600042d | MemberGen: `public string CategoryLegacy { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CategoryLegacy set` | 0x0600042e | MemberGen: `public string CategoryLegacy { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CategoryStandard get` | 0x0600042f | MemberGen: `public string CategoryStandard { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CategoryStandard set` | 0x06000430 | MemberGen: `public string CategoryStandard { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcWpfStyleNameToCategoryConverterMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
