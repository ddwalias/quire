# Ptc.Controls.Core.Serialization.Property

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSerializationPropertyMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Descriptor get` | 0x06000b85 | MemberGen: `protected internal PropertyDescriptor Descriptor { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Descriptor set` | 0x06000b86 | MemberGen: `protected internal PropertyDescriptor Descriptor { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Converter get` | 0x06000b89 | MemberGen: `protected internal TypeConverter Converter { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Converter set` | 0x06000b8a | MemberGen: `protected internal TypeConverter Converter { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UseDependencyPropertyDefaultValue get` | 0x06000b8f | MemberGen: `public bool UseDependencyPropertyDefaultValue { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UseDependencyPropertyDefaultValue set` | 0x06000b90 | MemberGen: `public bool UseDependencyPropertyDefaultValue { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DontResetValue get` | 0x06000b91 | MemberGen: `public bool DontResetValue { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DontResetValue set` | 0x06000b92 | MemberGen: `public bool DontResetValue { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSerializationPropertyMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
