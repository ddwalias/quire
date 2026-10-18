# Ptc.PersistentDataObjects.calculationUnits

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectscalculationUnitsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000bf5 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `customUnitSystem get` | 0x06000bf1 | MemberGen: `customUnitSystemField; set => customUnitSystemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `customUnitSystem set` | 0x06000bf2 | MemberGen: `customUnitSystemField; set => customUnitSystemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `currentUnitSystem get` | 0x06000bf3 | MemberGen: `currentUnitSystemField; set => currentUnitSystemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `currentUnitSystem set` | 0x06000bf4 | MemberGen: `currentUnitSystemField; set => currentUnitSystemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectscalculationUnitsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
