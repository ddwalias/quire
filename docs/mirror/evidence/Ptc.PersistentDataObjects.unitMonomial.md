# Ptc.PersistentDataObjects.unitMonomial

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsunitMonomialMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000b74 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `unitReference get` | 0x06000b70 | MemberGen: `unitReferenceField; set => unitReferenceField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `unitReference set` | 0x06000b71 | MemberGen: `unitReferenceField; set => unitReferenceField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `system get` | 0x06000b72 | MemberGen: `systemField; set => systemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `system set` | 0x06000b73 | MemberGen: `systemField; set => systemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsunitMonomialMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
