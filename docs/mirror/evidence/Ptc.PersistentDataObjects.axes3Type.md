# Ptc.PersistentDataObjects.axes3Type

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsaxes3TypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000c67 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `xAxis get` | 0x06000c60 | MemberGen: `xAxisField; set => xAxisField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `xAxis set` | 0x06000c61 | MemberGen: `xAxisField; set => xAxisField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `yAxis get` | 0x06000c62 | MemberGen: `yAxisField; set => yAxisField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `yAxis set` | 0x06000c63 | MemberGen: `yAxisField; set => yAxisField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `zAxis get` | 0x06000c64 | MemberGen: `zAxisField; set => zAxisField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `zAxis set` | 0x06000c65 | MemberGen: `zAxisField; set => zAxisField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsaxes3TypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
