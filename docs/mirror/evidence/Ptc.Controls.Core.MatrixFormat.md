# Ptc.Controls.Core.MatrixFormat

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreMatrixFormatMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `FormatChanged add` | 0x0600046c | MemberGen: `public event ActionByPropertyNameAndValue FormatChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `FormatChanged remove` | 0x0600046d | MemberGen: `public event ActionByPropertyNameAndValue FormatChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged add` | 0x06000487 | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged remove` | 0x06000488 | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreMatrixFormatMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
