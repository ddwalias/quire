# Ptc.Controls.DitaWriter

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsDitaWriterMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `WriteId(string)` | 0x06000056 | MemberGen: `base.WriteAttributeString("id", value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `WriteEndElement()` | 0x06000065 | MemberGen: `base.WriteFullEndElement()` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackImage(ImagePackagingInformation)` | 0x06000066 | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackExternalObject(string, string, ExternalObjectType)` | 0x06000067 | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackIncludedWorksheet(string, DateTime, string, Func)` | 0x06000068 | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackFlowDocument(FlowDocument, string)` | 0x0600006e | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackProtectedPart(Stream, string, string)` | 0x0600006f | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `TextStyleChangedHandler get` | 0x06000055 | MemberGen: `public ITextStyleChangedHandler TextStyleChangedHandler { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DirectoryName get` | 0x0600005b | MemberGen: `Path.Combine(_filePath, "graphics")` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackagePart get` | 0x06000069 | MemberGen: `null` | Mechanical; Release IL equals Mathcad's | Verified |
| `Helper get` | 0x0600006a | MemberGen: `_helper` | Mechanical; Release IL equals Mathcad's | Verified |
| `NameSpaceManager get` | 0x0600006b | MemberGen: `XmlWorksheetData.McdxNameSpaceManager` | Mechanical; Release IL equals Mathcad's | Verified |
| `XmlContentDocument get` | 0x0600006c | MemberGen: `public XmlDocument XmlContentDocument { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `XmlContentDocument set` | 0x0600006d | MemberGen: `public XmlDocument XmlContentDocument { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UseOverrides get` | 0x06000070 | MemberGen: `true` | Mechanical; Release IL equals Mathcad's | Verified |
| `Element: Dispose()` | 0x060002e1 | MemberGen: `_writer.WriteEndElement()` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsDitaWriterMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
