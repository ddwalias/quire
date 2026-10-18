# Ptc.PersistentDataObjects.axisGridType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsaxisGridTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000cab | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `gridFrequency get` | 0x06000ca3 | MemberGen: `gridFrequencyField; set => gridFrequencyField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `gridFrequency set` | 0x06000ca4 | MemberGen: `gridFrequencyField; set => gridFrequencyField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `gridLabels get` | 0x06000ca5 | MemberGen: `gridLabelsField; set => gridLabelsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `gridLabels set` | 0x06000ca6 | MemberGen: `gridLabelsField; set => gridLabelsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `gridLines get` | 0x06000ca7 | MemberGen: `gridLinesField; set => gridLinesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `gridLines set` | 0x06000ca8 | MemberGen: `gridLinesField; set => gridLinesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `tickMarks get` | 0x06000ca9 | MemberGen: `tickMarksField; set => tickMarksField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `tickMarks set` | 0x06000caa | MemberGen: `tickMarksField; set => tickMarksField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsaxisGridTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
