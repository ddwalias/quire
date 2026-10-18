# Ptc.Xml.XmlNodeWriter

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcXmlXmlNodeWriterMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `WriteStartDocument()` | 0x060003bc | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteStartDocument(bool)` | 0x060003bd | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteEndDocument()` | 0x060003be | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteDocType(string, string, string, string)` | 0x060003bf | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteCData(string)` | 0x060003c5 | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteComment(string)` | 0x060003c6 | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteProcessingInstruction(string, string)` | 0x060003c7 | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteEntityRef(string)` | 0x060003c8 | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteCharEntity(char)` | 0x060003c9 | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteWhitespace(string)` | 0x060003ca | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteSurrogateCharEntity(char, char)` | 0x060003cc | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteChars(char[], int, int)` | 0x060003cd | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteRaw(char[], int, int)` | 0x060003ce | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Flush()` | 0x060003d2 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `LookupPrefix(string)` | 0x060003d3 | MemberGen: `ns` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteState get` | 0x060003d4 | MemberGen: `_writeState` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcXmlXmlNodeWriterMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
