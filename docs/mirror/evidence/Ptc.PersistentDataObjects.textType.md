# Ptc.PersistentDataObjects.textType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectstextTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000e4f | MemberGen: `istextblockField = false;` | Mechanical; Release IL equals Mathcad's | Verified |
| `AddAnnotation(MC14ConversionAnnotationType)` | 0x06000e68 | MemberGen: `IMC14AnnotationProviderExtensions.AddMC14ConversionLossAnnotations(this, annotation)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Any get` | 0x06000e50 | MemberGen: `anyField; set => anyField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Any set` | 0x06000e51 | MemberGen: `anyField; set => anyField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions get` | 0x06000e52 | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions set` | 0x06000e53 | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `istextblock get` | 0x06000e54 | MemberGen: `istextblockField; set => istextblockField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `istextblock set` | 0x06000e55 | MemberGen: `istextblockField; set => istextblockField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `itemidref get` | 0x06000e56 | MemberGen: `itemidrefField; set => itemidrefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `itemidref set` | 0x06000e57 | MemberGen: `itemidrefField; set => itemidrefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `VisualNumberOfLines get` | 0x06000e5a | MemberGen: `public int VisualNumberOfLines { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `VisualNumberOfLines set` | 0x06000e5b | MemberGen: `public int VisualNumberOfLines { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontNames get` | 0x06000e5c | MemberGen: `public List<string> FontNames { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontNames set` | 0x06000e5d | MemberGen: `public List<string> FontNames { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Document get` | 0x06000e5f | MemberGen: `_document; set => SetDocument(value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Document set` | 0x06000e5e | MemberGen: `_document; set => SetDocument(value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DocumentWithTextFormat get` | 0x06000e61 | MemberGen: `private global::System.Windows.Documents.FlowDocument DocumentWithTextFormat { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DocumentWithTextFormat set` | 0x06000e62 | MemberGen: `private global::System.Windows.Documents.FlowDocument DocumentWithTextFormat { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotations get` | 0x06000e69 | MemberGen: `MC14ConversionLossAnnotationsLocal` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal get` | 0x06000e6a | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal set` | 0x06000e6b | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectstextTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
