# Ptc.Wpf.WindowLocatedRelativelyToPositionProvider

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcWpfWindowLocatedRelativelyToPositionProviderMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060005d8 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PositionProvider get` | 0x060005ce | MemberGen: `protected IPositionProvider PositionProvider { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PositionProvider set` | 0x060005cf | MemberGen: `protected IPositionProvider PositionProvider { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DpiFactors: .ctor(double, double)` | 0x060012e2 | MemberGen: `DpiFactorX = dpiFactorX; DpiFactorY = dpiFactorY;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcWpfWindowLocatedRelativelyToPositionProviderMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
