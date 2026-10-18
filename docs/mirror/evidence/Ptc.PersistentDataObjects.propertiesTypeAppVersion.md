# Ptc.PersistentDataObjects.propertiesTypeAppVersion

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectspropertiesTypeAppVersionMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060009c5 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `release get` | 0x060009c1 | MemberGen: `releaseField; set => releaseField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `release set` | 0x060009c2 | MemberGen: `releaseField; set => releaseField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x060009c3 | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x060009c4 | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectspropertiesTypeAppVersionMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
