# Ptc.Controls.IO.IOItem

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsIOIOItemMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Region get` | 0x06000246 | MemberGen: `_region` | Mechanical; Release IL equals Mathcad's | Verified |
| `IdSexp get` | 0x06000247 | MemberGen: `_idSExpression` | Mechanical; Release IL equals Mathcad's | Verified |
| `IdName get` | 0x06000248 | MemberGen: `_idName` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsInput get` | 0x06000249 | MemberGen: `_isIn` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged add` | 0x0600024d | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged remove` | 0x0600024e | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsIOIOItemMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
