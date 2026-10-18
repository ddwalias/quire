# Ptc.Matrices.SparseMatrix`1

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcMatricesSparseMatrix1MechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `PopulatedCount get` | 0x06000eb6 | MemberGen: `_itemMap.Count` | Mechanical; Release IL equals Mathcad's | Verified |
| `PopulatedPositions get` | 0x06000eb7 | MemberGen: `_itemMap.Keys` | Mechanical; Release IL equals Mathcad's | Verified |
| `Changed add` | 0x06000eb2 | MemberGen: `public event SparseMatrix<TItem>.SparseMatrixChangedEventHandler Changed;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Changed remove` | 0x06000eb3 | MemberGen: `public event SparseMatrix<TItem>.SparseMatrixChangedEventHandler Changed;` | Mechanical; Release IL equals Mathcad's | Verified |
| `SparseMatrixItemChangedEventArgs: Position get` | 0x0600138d | MemberGen: `public TableCoordinate Position { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SparseMatrixItemChangedEventArgs: Position set` | 0x0600138e | MemberGen: `public TableCoordinate Position { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcMatricesSparseMatrix1MechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
