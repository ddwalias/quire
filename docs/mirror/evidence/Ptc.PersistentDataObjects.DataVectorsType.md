# Ptc.PersistentDataObjects.DataVectorsType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000b86 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Count get` | 0x06000b7c | MemberGen: `countField; set => countField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Count set` | 0x06000b7d | MemberGen: `countField; set => countField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ValuesType get` | 0x06000b7e | MemberGen: `valuesTypeField; set => valuesTypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ValuesType set` | 0x06000b7f | MemberGen: `valuesTypeField; set => valuesTypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `VectorLength get` | 0x06000b80 | MemberGen: `vectorLengthField; set => vectorLengthField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `VectorLength set` | 0x06000b81 | MemberGen: `vectorLengthField; set => vectorLengthField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `VectorsType get` | 0x06000b82 | MemberGen: `vectorsTypeField; set => vectorsTypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `VectorsType set` | 0x06000b83 | MemberGen: `vectorsTypeField; set => vectorsTypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x06000b84 | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x06000b85 | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
