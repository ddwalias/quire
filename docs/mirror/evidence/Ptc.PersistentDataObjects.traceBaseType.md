# Ptc.PersistentDataObjects.traceBaseType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectstraceBaseTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000d14 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `traceStyle get` | 0x06000d0c | MemberGen: `traceStyleField; set => traceStyleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `traceStyle set` | 0x06000d0d | MemberGen: `traceStyleField; set => traceStyleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `numofpoints get` | 0x06000d0e | MemberGen: `numofpointsField; set => numofpointsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `numofpoints set` | 0x06000d0f | MemberGen: `numofpointsField; set => numofpointsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRef get` | 0x06000d10 | MemberGen: `resultRefField; set => resultRefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRef set` | 0x06000d11 | MemberGen: `resultRefField; set => resultRefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRefSpecified get` | 0x06000d12 | MemberGen: `resultRefFieldSpecified; set => resultRefFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRefSpecified set` | 0x06000d13 | MemberGen: `resultRefFieldSpecified; set => resultRefFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectstraceBaseTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
