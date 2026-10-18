# Ptc.Calculation.Core.SimpleResultNodeFactory

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `CreateEmptyResultNode()` | 0x060001ed | MemberGen: `new EmptySimpleResultNode()` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateRealResultNode()` | 0x060001ee | MemberGen: `new RealSimpleResultNode()` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateImaginaryResultNode()` | 0x060001f0 | MemberGen: `new ImaginarySimpleResultNode()` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateComplexResultNode()` | 0x060001f2 | MemberGen: `new ComplexSimpleResultNode()` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateStringResultNode()` | 0x060001f4 | MemberGen: `new StringSimpleResultNode()` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateDisplayMatrixResultNode(int, int, TableCoordinate, TableSize)` | 0x060001f7 | MemberGen: `new DisplayMatrixSimpleResultNode(rows, cols, viewportOffset, viewportSize)` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateEagerMatrixResultNode(int, int)` | 0x060001f8 | MemberGen: `new EagerMatrixSimpleResultNode(rows, cols)` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateLazyMatrixResultNode(IMatrixValueProxyWrapper, Func)` | 0x060001f9 | MemberGen: `new LazyMatrixSimpleResultNode(matrix, transformValueToResult)` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateUnitedValueResultNode()` | 0x060001fa | MemberGen: `new UnitedValueSimpleResultNode()` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x060001fc | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
