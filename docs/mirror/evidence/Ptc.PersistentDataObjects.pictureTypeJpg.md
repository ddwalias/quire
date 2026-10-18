# Ptc.PersistentDataObjects.pictureTypeJpg

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectspictureTypeJpgMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000c36 | MemberGen: `qualityField = "75";` | Mechanical; Release IL equals Mathcad's | Verified |
| `quality get` | 0x06000c37 | MemberGen: `qualityField; set => qualityField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `quality set` | 0x06000c38 | MemberGen: `qualityField; set => qualityField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectspictureTypeJpgMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
