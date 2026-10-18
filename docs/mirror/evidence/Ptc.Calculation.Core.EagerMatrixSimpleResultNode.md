# Ptc.Calculation.Core.EagerMatrixSimpleResultNode

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Accept(IResultNodeVisitor)` | 0x060001d0 | MemberGen: `visitor.Visit(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Count get` | 0x060001c6 | MemberGen: `_children.Count` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns get` | 0x060001c8 | MemberGen: `public int Columns { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns set` | 0x060001c9 | MemberGen: `public int Columns { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
