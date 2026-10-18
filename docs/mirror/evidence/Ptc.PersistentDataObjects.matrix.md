# Ptc.PersistentDataObjects.matrix

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsmatrixMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000b3b | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `display get` | 0x06000b31 | MemberGen: `displayField; set => displayField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `display set` | 0x06000b32 | MemberGen: `displayField; set => displayField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Items get` | 0x06000b33 | MemberGen: `itemsField; set => itemsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Items set` | 0x06000b34 | MemberGen: `itemsField; set => itemsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `rows get` | 0x06000b35 | MemberGen: `rowsField; set => rowsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `rows set` | 0x06000b36 | MemberGen: `rowsField; set => rowsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `cols get` | 0x06000b37 | MemberGen: `colsField; set => colsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `cols set` | 0x06000b38 | MemberGen: `colsField; set => colsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsmatrixMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
