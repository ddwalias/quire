# Ptc.PersistentDataObjects.unitedValue

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000b6f | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `real get` | 0x06000b65 | MemberGen: `realField; set => realField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `real set` | 0x06000b66 | MemberGen: `realField; set => realField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `imag get` | 0x06000b67 | MemberGen: `imagField; set => imagField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `imag set` | 0x06000b68 | MemberGen: `imagField; set => imagField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `complex get` | 0x06000b69 | MemberGen: `complexField; set => complexField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `complex set` | 0x06000b6a | MemberGen: `complexField; set => complexField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `str get` | 0x06000b6b | MemberGen: `strField; set => strField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `str set` | 0x06000b6c | MemberGen: `strField; set => strField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `unitMonomial get` | 0x06000b6d | MemberGen: `unitMonomialField; set => unitMonomialField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `unitMonomial set` | 0x06000b6e | MemberGen: `unitMonomialField; set => unitMonomialField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
