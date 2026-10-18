# Ptc.PersistentDataObjects.resultFormatType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000a90 | MemberGen: `numericonlyField = true;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item get` | 0x06000a91 | MemberGen: `itemField; set => itemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item set` | 0x06000a92 | MemberGen: `itemField; set => itemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `matrix get` | 0x06000a93 | MemberGen: `matrixField; set => matrixField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `matrix set` | 0x06000a94 | MemberGen: `matrixField; set => matrixField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `unit get` | 0x06000a95 | MemberGen: `unitField; set => unitField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `unit set` | 0x06000a96 | MemberGen: `unitField; set => unitField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `table get` | 0x06000a97 | MemberGen: `tableField; set => tableField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `table set` | 0x06000a98 | MemberGen: `tableField; set => tableField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `numericonly get` | 0x06000a99 | MemberGen: `numericonlyField; set => numericonlyField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `numericonly set` | 0x06000a9a | MemberGen: `numericonlyField; set => numericonlyField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
