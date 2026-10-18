# Ptc.PersistentData.chartComponentType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDatachartComponentTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060008ed | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions get` | 0x060008e3 | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions set` | 0x060008e4 | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputSection get` | 0x060008e5 | MemberGen: `inputSectionField; set => inputSectionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputSection set` | 0x060008e6 | MemberGen: `inputSectionField; set => inputSectionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRef get` | 0x060008e7 | MemberGen: `resultRefField; set => resultRefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRef set` | 0x060008e8 | MemberGen: `resultRefField; set => resultRefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRefSpecified get` | 0x060008e9 | MemberGen: `resultRefFieldSpecified; set => resultRefFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRefSpecified set` | 0x060008ea | MemberGen: `resultRefFieldSpecified; set => resultRefFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDatachartComponentTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
