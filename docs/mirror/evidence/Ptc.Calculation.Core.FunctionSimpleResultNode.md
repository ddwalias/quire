# Ptc.Calculation.Core.FunctionSimpleResultNode

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreFunctionSimpleResultNodeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Accept(IResultNodeVisitor)` | 0x060001b4 | MemberGen: `visitor.Visit(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x060001b5 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Description get` | 0x060001b1 | MemberGen: `public string Description { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Description set` | 0x060001b2 | MemberGen: `public string Description { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ToString()` | 0x060001b3 | MemberGen: `string.Format("Function Description: \"{0}\"", Description)` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreFunctionSimpleResultNodeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

MemberGen members are compared by `Quire.Differential.Tests.PtcCalculationCoreFunctionSimpleResultNodeMechanicalDifferentialTests`.
