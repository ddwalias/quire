# Ptc.Controls.Core.ResultFormat

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreResultFormatMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetSchema()` | 0x0600064b | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `NumberFormatWasRead get` | 0x06000644 | MemberGen: `public bool NumberFormatWasRead { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `NumberFormatWasRead set` | 0x06000643 | MemberGen: `public bool NumberFormatWasRead { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MatrixFormatWasRead get` | 0x06000646 | MemberGen: `public bool MatrixFormatWasRead { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MatrixFormatWasRead set` | 0x06000645 | MemberGen: `public bool MatrixFormatWasRead { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RootNodeFormatName get` | 0x06000649 | MemberGen: `public string RootNodeFormatName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RootNodeFormatName set` | 0x0600064a | MemberGen: `public string RootNodeFormatName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreResultFormatMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
