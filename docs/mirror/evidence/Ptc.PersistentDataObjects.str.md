# Ptc.PersistentDataObjects.str

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsstrMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000b64 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `exprid get` | 0x06000b5e | MemberGen: `expridField; set => expridField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `exprid set` | 0x06000b5f | MemberGen: `expridField; set => expridField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `space get` | 0x06000b60 | MemberGen: `spaceField; set => spaceField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `space set` | 0x06000b61 | MemberGen: `spaceField; set => spaceField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x06000b62 | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x06000b63 | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsstrMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
