# Ptc.PersistentDataObjects.resultFormatPercentType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `radix get` | 0x06000adb | MemberGen: `radixField; set => radixField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `radix set` | 0x06000adc | MemberGen: `radixField; set => radixField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `radixSpecified get` | 0x06000add | MemberGen: `radixFieldSpecified; set => radixFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `radixSpecified set` | 0x06000ade | MemberGen: `radixFieldSpecified; set => radixFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `showtrailingzeros get` | 0x06000adf | MemberGen: `showtrailingzerosField; set => showtrailingzerosField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `showtrailingzeros set` | 0x06000ae0 | MemberGen: `showtrailingzerosField; set => showtrailingzerosField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `imaginaryvalue get` | 0x06000ae1 | MemberGen: `imaginaryvalueField; set => imaginaryvalueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `imaginaryvalue set` | 0x06000ae2 | MemberGen: `imaginaryvalueField; set => imaginaryvalueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `precision get` | 0x06000ae3 | MemberGen: `precisionField; set => precisionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `precision set` | 0x06000ae4 | MemberGen: `precisionField; set => precisionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `complexthreshold get` | 0x06000ae5 | MemberGen: `complexthresholdField; set => complexthresholdField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `complexthreshold set` | 0x06000ae6 | MemberGen: `complexthresholdField; set => complexthresholdField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `zerothreshold get` | 0x06000ae7 | MemberGen: `zerothresholdField; set => zerothresholdField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `zerothreshold set` | 0x06000ae8 | MemberGen: `zerothresholdField; set => zerothresholdField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
