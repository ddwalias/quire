# Ptc.Calculation.Core.DisplayMatrixSimpleResultNode

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Accept(IResultNodeVisitor)` | 0x060001c3 | MemberGen: `visitor.Visit(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rows get` | 0x060001b9 | MemberGen: `public int Rows { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rows set` | 0x060001ba | MemberGen: `public int Rows { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns get` | 0x060001bb | MemberGen: `public int Columns { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns set` | 0x060001bc | MemberGen: `public int Columns { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ViewportOffset get` | 0x060001bd | MemberGen: `public TableCoordinate ViewportOffset { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ViewportOffset set` | 0x060001be | MemberGen: `public TableCoordinate ViewportOffset { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Viewport get` | 0x060001bf | MemberGen: `public IEagerMatrixResultNode Viewport { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Viewport set` | 0x060001c0 | MemberGen: `public IEagerMatrixResultNode Viewport { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
