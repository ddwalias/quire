# Ptc.Calculation.Core.UnitedValueSimpleResultNode

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreUnitedValueSimpleResultNodeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Accept(IResultNodeVisitor)` | 0x06000196 | MemberGen: `visitor.Visit(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x0600018f | MemberGen: `public SimpleResultNode Value { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x06000190 | MemberGen: `public SimpleResultNode Value { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Units get` | 0x06000194 | MemberGen: `public IList<Unit> Units { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Units set` | 0x06000195 | MemberGen: `public IList<Unit> Units { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreUnitedValueSimpleResultNodeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
