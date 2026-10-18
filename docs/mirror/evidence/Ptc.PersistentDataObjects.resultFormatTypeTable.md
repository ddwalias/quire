# Ptc.PersistentDataObjects.resultFormatTypeTable

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsresultFormatTypeTableMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000b11 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `itemidref get` | 0x06000b0f | MemberGen: `itemidrefField; set => itemidrefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `itemidref set` | 0x06000b10 | MemberGen: `itemidrefField; set => itemidrefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsresultFormatTypeTableMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
