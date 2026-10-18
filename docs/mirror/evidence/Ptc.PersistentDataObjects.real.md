# Ptc.PersistentDataObjects.real

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsrealMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `font get` | 0x06000b48 | MemberGen: `fontField; set => fontField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `font set` | 0x06000b49 | MemberGen: `fontField; set => fontField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `exprid get` | 0x06000b4a | MemberGen: `expridField; set => expridField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `exprid set` | 0x06000b4b | MemberGen: `expridField; set => expridField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `base get` | 0x06000b4c | MemberGen: `baseField; set => baseField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `base set` | 0x06000b4d | MemberGen: `baseField; set => baseField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x06000b4e | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x06000b4f | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsrealMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
