# Ptc.Controls.Core.Serialization.XElementSerializable

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSerializationXElementSerializableMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetSchema()` | 0x06000c19 | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Children get` | 0x06000c0b | MemberGen: `public object[] Children { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Children set` | 0x06000c0c | MemberGen: `public object[] Children { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name get` | 0x06000c0d | MemberGen: `_name` | Mechanical; Release IL equals Mathcad's | Verified |
| `Namespace get` | 0x06000c0e | MemberGen: `_name.Namespace` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSerializationXElementSerializableMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
