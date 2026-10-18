# Ptc.PersistentDataObjects.MatrixOfDoubles

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsMatrixOfDoublesMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000b30 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Cells get` | 0x06000b2a | MemberGen: `cellsField; set => cellsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Cells set` | 0x06000b2b | MemberGen: `cellsField; set => cellsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rows get` | 0x06000b2c | MemberGen: `rowsField; set => rowsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rows set` | 0x06000b2d | MemberGen: `rowsField; set => rowsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns get` | 0x06000b2e | MemberGen: `columnsField; set => columnsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns set` | 0x06000b2f | MemberGen: `columnsField; set => columnsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsMatrixOfDoublesMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
