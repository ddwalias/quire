# Ptc.WpfClipboard

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcWpfClipboardMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Clear()` | 0x06000381 | MemberGen: `Clipboard.Clear()` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetText()` | 0x06000382 | MemberGen: `Clipboard.GetText()` | Mechanical; Release IL equals Mathcad's | Verified |
| `CopySExpressionToDataObject(IDataObject, string)` | 0x06000386 | MemberGen: `dataObject.SetData(_mathNodeSExpression, sExpression); return true;` | Mechanical; Release IL equals Mathcad's | Verified |
| `CopyRtfStringToDataObject(DataObject, string)` | 0x06000388 | MemberGen: `dataObject.SetData(DataFormats.Rtf, rtfString); return true;` | Mechanical; Release IL equals Mathcad's | Verified |
| `HasText()` | 0x0600038a | MemberGen: `Clipboard.ContainsText()` | Mechanical; Release IL equals Mathcad's | Verified |
| `HasImage()` | 0x06000391 | MemberGen: `Clipboard.ContainsImage()` | Mechanical; Release IL equals Mathcad's | Verified |
| `HasPrimeObject()` | 0x06000392 | MemberGen: `Clipboard.ContainsData(_primeXamlStreamCompressed)` | Mechanical; Release IL equals Mathcad's | Verified |
| `HasXMLSpreadsheet()` | 0x06000393 | MemberGen: `Clipboard.ContainsData(_xmlSpreadsheetDataType)` | Mechanical; Release IL equals Mathcad's | Verified |
| `HasSExpression()` | 0x06000397 | MemberGen: `Clipboard.ContainsData(_mathNodeSExpression)` | Mechanical; Release IL equals Mathcad's | Verified |
| `HasTextWithEmbeddedItemsInfo()` | 0x06000399 | MemberGen: `Clipboard.ContainsData(_richTextEmbeddedItemsInfo)` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x0600039b | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ClipboardOverride: .ctor(IClipboard)` | 0x06001160 | MemberGen: `_oldClipboard = oldClipboard;` | Mechanical; Release IL equals Mathcad's | Verified |
| `ClipboardOverride: Dispose()` | 0x06001161 | MemberGen: `_current = _oldClipboard` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcWpfClipboardMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
