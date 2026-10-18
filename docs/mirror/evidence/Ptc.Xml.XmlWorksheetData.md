# Ptc.Xml.XmlWorksheetData

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcXmlXmlWorksheetDataMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `CreateMathcad14MLElement(XmlDocument, string)` | 0x060003da | MemberGen: `CreateMLElement(document, localName, "http://schemas.mathsoft.com/math30")` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateMLElement(XmlDocument, string, string)` | 0x060003db | MemberGen: `document.CreateElement("ml", localName, namespaceURI)` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateMcdxMLElement(XmlDocument, string)` | 0x060003dc | MemberGen: `CreateMLElement(document, localName, "http://schemas.mathsoft.com/math50")` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcXmlXmlWorksheetDataMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
