# Ptc.PersistentDataObjects.resultFormatTypeUnit

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsresultFormatTypeUnitMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `formatunits get` | 0x06000b09 | MemberGen: `formatunitsField; set => formatunitsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `formatunits set` | 0x06000b0a | MemberGen: `formatunitsField; set => formatunitsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `simplifyunits get` | 0x06000b0b | MemberGen: `simplifyunitsField; set => simplifyunitsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `simplifyunits set` | 0x06000b0c | MemberGen: `simplifyunitsField; set => simplifyunitsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `fractionalunitexponent get` | 0x06000b0d | MemberGen: `fractionalunitexponentField; set => fractionalunitexponentField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `fractionalunitexponent set` | 0x06000b0e | MemberGen: `fractionalunitexponentField; set => fractionalunitexponentField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsresultFormatTypeUnitMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
