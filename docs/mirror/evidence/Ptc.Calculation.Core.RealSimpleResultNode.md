# Ptc.Calculation.Core.RealSimpleResultNode

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreRealSimpleResultNodeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Accept(IResultNodeVisitor)` | 0x0600019b | MemberGen: `visitor.Visit(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x0600019c | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x06000197 | MemberGen: `public double Value { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x06000198 | MemberGen: `public double Value { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ToString()` | 0x06000199 | MemberGen: `string.Format("Real: {0}", Value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Equals(object)` | 0x0600019a | Same type only; compare `Value` with `double.Equals` | Direct Mathcad comparison; NaN values compare equal | Verified |

## Test cases

**Observed** (`PtcCalculationCoreRealSimpleResultNodeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

MemberGen members are compared by `Quire.Differential.Tests.PtcCalculationCoreRealSimpleResultNodeMechanicalDifferentialTests`.
