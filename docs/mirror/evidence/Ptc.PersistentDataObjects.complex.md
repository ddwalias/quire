# Ptc.PersistentDataObjects.complex

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectscomplexMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000b46 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `real get` | 0x06000b42 | MemberGen: `realField; set => realField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `real set` | 0x06000b43 | MemberGen: `realField; set => realField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `imag get` | 0x06000b44 | MemberGen: `imagField; set => imagField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `imag set` | 0x06000b45 | MemberGen: `imagField; set => imagField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectscomplexMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
