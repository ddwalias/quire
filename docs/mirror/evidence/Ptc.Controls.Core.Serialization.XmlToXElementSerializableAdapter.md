# Ptc.Controls.Core.Serialization.XmlToXElementSerializableAdapter

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSerializationXmlToXElementSerializableAdapterMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(IXmlSerializable)` | 0x06000ac6 | MemberGen: `_xmlSerializable = xmlSerializable;` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetSchema()` | 0x06000ac7 | MemberGen: `_xmlSerializable.GetSchema()` | Mechanical; Release IL equals Mathcad's | Verified |
| `ReadXml(XmlReader)` | 0x06000ac8 | MemberGen: `_xmlSerializable.ReadXml(reader)` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteXml(XmlWriter)` | 0x06000ac9 | MemberGen: `_xmlSerializable.WriteXml(writer)` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteXml(XElement)` | 0x06000acb | MemberGen: `WriteXmlToXContainer(xElement)` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteXml(XDocument)` | 0x06000acc | MemberGen: `WriteXmlToXContainer(xDocument)` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSerializationXmlToXElementSerializableAdapterMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
