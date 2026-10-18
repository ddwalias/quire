# Ptc.Core.Serialization.ApplicationPropertiesXmlSerializable

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(ISerializationHelper)` | 0x0600108b | MemberGen: `_helper = helper;` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsFormatVersionCompatible(Version)` | 0x06001092 | MemberGen: `XmlWorksheetData.CurrentFormatVersion >= fileFormatVersion` | Mechanical; Release IL equals Mathcad's | Verified |
| `ShouldValidateOnWrite get` | 0x0600108f | MemberGen: `true` | Mechanical; Release IL equals Mathcad's | Verified |
| `ShouldValidateOnRead get` | 0x06001090 | MemberGen: `true` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
