# Ptc.PersistentDataObjects.resultFormatDecimalType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `radix get` | 0x06000a9c | MemberGen: `radixField; set => radixField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `radix set` | 0x06000a9d | MemberGen: `radixField; set => radixField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `showtrailingzeros get` | 0x06000a9e | MemberGen: `showtrailingzerosField; set => showtrailingzerosField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `showtrailingzeros set` | 0x06000a9f | MemberGen: `showtrailingzerosField; set => showtrailingzerosField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `imaginaryvalue get` | 0x06000aa0 | MemberGen: `imaginaryvalueField; set => imaginaryvalueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `imaginaryvalue set` | 0x06000aa1 | MemberGen: `imaginaryvalueField; set => imaginaryvalueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `precision get` | 0x06000aa2 | MemberGen: `precisionField; set => precisionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `precision set` | 0x06000aa3 | MemberGen: `precisionField; set => precisionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `complexthreshold get` | 0x06000aa4 | MemberGen: `complexthresholdField; set => complexthresholdField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `complexthreshold set` | 0x06000aa5 | MemberGen: `complexthresholdField; set => complexthresholdField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `zerothreshold get` | 0x06000aa6 | MemberGen: `zerothresholdField; set => zerothresholdField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `zerothreshold set` | 0x06000aa7 | MemberGen: `zerothresholdField; set => zerothresholdField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
