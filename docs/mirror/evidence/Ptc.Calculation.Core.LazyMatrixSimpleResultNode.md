# Ptc.Calculation.Core.LazyMatrixSimpleResultNode

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Equals(object)` | 0x060001d6 | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |
| `Accept(IResultNodeVisitor)` | 0x060001e4 | MemberGen: `visitor.Visit(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns get` | 0x060001dc | MemberGen: `public int Columns { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns set` | 0x060001dd | MemberGen: `public int Columns { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rows get` | 0x060001de | MemberGen: `public int Rows { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rows set` | 0x060001df | MemberGen: `public int Rows { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MatrixProxy get` | 0x060001e0 | MemberGen: `public IMatrixValueProxyWrapper MatrixProxy { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MatrixProxy set` | 0x060001e1 | MemberGen: `public IMatrixValueProxyWrapper MatrixProxy { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TransformValueToResult get` | 0x060001e2 | MemberGen: `public Func<IValueProxyWrapper, IResultNode> TransformValueToResult { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TransformValueToResult set` | 0x060001e3 | MemberGen: `public Func<IValueProxyWrapper, IResultNode> TransformValueToResult { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ToString()` | 0x060001e5 | MemberGen: `string.Format("LazyMatrix: ({0},{1})", Rows, Columns)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Xml get` | 0x060001d5 | MemberGen: `MatrixProxy.Xml` | Mechanical; Release IL equals Mathcad's | Verified |
| `Count get` | 0x060001da | MemberGen: `Rows * Columns` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

MemberGen members are compared by `Quire.Differential.Tests.PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests`.
