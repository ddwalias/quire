# Ptc.PersistentDataObjects.pictureType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000c16 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackExternalObject(string, string, ExternalObjectType)` | 0x06000c1c | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackIncludedWorksheet(string, DateTime, string, Func)` | 0x06000c1d | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackFlowDocument(FlowDocument, string)` | 0x06000c1e | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackProtectedPart(Stream, string, string)` | 0x06000c1f | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item get` | 0x06000c12 | MemberGen: `itemField; set => itemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item set` | 0x06000c13 | MemberGen: `itemField; set => itemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ItemElementName get` | 0x06000c14 | MemberGen: `itemElementNameField; set => itemElementNameField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ItemElementName set` | 0x06000c15 | MemberGen: `itemElementNameField; set => itemElementNameField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TextStyleChangedHandler get` | 0x06000c18 | MemberGen: `public ITextStyleChangedHandler TextStyleChangedHandler { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PictureSourceUri get` | 0x06000c19 | MemberGen: `private Uri PictureSourceUri { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PictureSourceUri set` | 0x06000c1a | MemberGen: `private Uri PictureSourceUri { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `S11NProvider get` | 0x06000c25 | MemberGen: `public IS11NProvider S11NProvider { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `S11NProvider set` | 0x06000c26 | MemberGen: `public IS11NProvider S11NProvider { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InnerXml get` | 0x06000c27 | MemberGen: `public XmlNode[] InnerXml { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InnerXml set` | 0x06000c28 | MemberGen: `public XmlNode[] InnerXml { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
