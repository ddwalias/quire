# Ptc.PersistentData.chartComponentRegionType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDatachartComponentRegionTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060008f2 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item get` | 0x060008ee | MemberGen: `itemField; set => itemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item set` | 0x060008ef | MemberGen: `itemField; set => itemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `mc14conversionlossannotations get` | 0x060008f0 | MemberGen: `public override string[] mc14conversionlossannotations { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `mc14conversionlossannotations set` | 0x060008f1 | MemberGen: `public override string[] mc14conversionlossannotations { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDatachartComponentRegionTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
