# Ptc.Calculation.Core.Trace2DResults.VectorListResultNode

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Accept(IResultNodeVisitor)` | 0x0600024d | MemberGen: `visitor.Visit(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SingleVectorLength get` | 0x06000241 | MemberGen: `public int SingleVectorLength { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SingleVectorLength set` | 0x06000242 | MemberGen: `public int SingleVectorLength { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `VectorsType get` | 0x06000243 | MemberGen: `public VectorsType VectorsType { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `VectorsType set` | 0x06000244 | MemberGen: `public VectorsType VectorsType { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Vectors get` | 0x06000247 | MemberGen: `_vectors` | Mechanical; Release IL equals Mathcad's | Verified |
| `Count get` | 0x0600024a | MemberGen: `_vectors.Count` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item get` | 0x0600024b | MemberGen: `_vectors[index]; set => _vectors[index] = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item set` | 0x0600024c | MemberGen: `_vectors[index]; set => _vectors[index] = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetVectorValuesType(int)` | 0x06000249 | MemberGen: `GetResultType(Vectors[index])` | Mechanical; Release IL equals Mathcad's | Verified |
| `ElementsCount get` | 0x06000240 | MemberGen: `Count * SingleVectorLength` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

MemberGen members are compared by `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests`.
