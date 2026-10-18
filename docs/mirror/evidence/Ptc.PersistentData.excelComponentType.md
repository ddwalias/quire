# Ptc.PersistentData.excelComponentType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060008c0 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions get` | 0x060008af | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions set` | 0x060008b0 | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputSection get` | 0x060008b1 | MemberGen: `inputSectionField; set => inputSectionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputSection set` | 0x060008b2 | MemberGen: `inputSectionField; set => inputSectionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OutputSection get` | 0x060008b3 | MemberGen: `outputSectionField; set => outputSectionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OutputSection set` | 0x060008b4 | MemberGen: `outputSectionField; set => outputSectionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRef get` | 0x060008b5 | MemberGen: `resultRefField; set => resultRefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRef set` | 0x060008b6 | MemberGen: `resultRefField; set => resultRefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRefSpecified get` | 0x060008b7 | MemberGen: `resultRefFieldSpecified; set => resultRefFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRefSpecified set` | 0x060008b8 | MemberGen: `resultRefFieldSpecified; set => resultRefFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HeightAfterConversion get` | 0x060008bb | MemberGen: `public double HeightAfterConversion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HeightAfterConversion set` | 0x060008bc | MemberGen: `public double HeightAfterConversion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotations get` | 0x060008bd | MemberGen: `MC14ConversionLossAnnotationsLocal` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal get` | 0x060008be | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal set` | 0x060008bf | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
