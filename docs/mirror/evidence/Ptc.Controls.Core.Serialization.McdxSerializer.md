# Ptc.Controls.Core.Serialization.McdxSerializer

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `TextStyleChangedHandler get` | 0x06000b2a | MemberGen: `_textStyleChangedHandler` | Mechanical; Release IL equals Mathcad's | Verified |
| `XmlContentDocument get` | 0x06000b32 | MemberGen: `public XmlDocument XmlContentDocument { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `XmlContentDocument set` | 0x06000b33 | MemberGen: `public XmlDocument XmlContentDocument { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UseOverrides get` | 0x06000b34 | MemberGen: `public bool UseOverrides { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UseOverrides set` | 0x06000b35 | MemberGen: `public bool UseOverrides { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackagePart get` | 0x06000b36 | MemberGen: `_packagePart` | Mechanical; Release IL equals Mathcad's | Verified |
| `Helper get` | 0x06000b37 | MemberGen: `_serializationHelper` | Mechanical; Release IL equals Mathcad's | Verified |
| `NameSpaceManager get` | 0x06000b38 | MemberGen: `XmlWorksheetData.McdxNameSpaceManager` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
