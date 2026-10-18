# Ptc.Controls.Core.SubElementToResizeData

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSubElementToResizeDataMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Element get` | 0x06000806 | MemberGen: `public FrameworkElement Element { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Element set` | 0x06000807 | MemberGen: `public FrameworkElement Element { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SerializeDimensions get` | 0x06000808 | MemberGen: `public bool SerializeDimensions { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SerializeDimensions set` | 0x06000809 | MemberGen: `public bool SerializeDimensions { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsFullSize get` | 0x0600080a | MemberGen: `public bool IsFullSize { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsFullSize set` | 0x0600080b | MemberGen: `public bool IsFullSize { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSubElementToResizeDataMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
