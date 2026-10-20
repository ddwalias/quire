# Ptc.Calculation.Core.StringSimpleResultNode

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreStringSimpleResultNodeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Accept(IResultNodeVisitor)` | 0x060001af | MemberGen: `visitor.Visit(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x060001b0 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x060001ab | MemberGen: `public string Value { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x060001ac | MemberGen: `public string Value { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ToString()` | 0x060001ad | MemberGen: `string.Format("String: \"{0}\"", Value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Equals(object)` | 0x060001ae | Same type only; call the other node's `Value.Equals(Value)` | Direct Mathcad comparison; a null other-node value throws as in Mathcad | Verified |

## Test cases

**Observed** (`PtcCalculationCoreStringSimpleResultNodeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

MemberGen members are compared by `Quire.Differential.Tests.PtcCalculationCoreStringSimpleResultNodeMechanicalDifferentialTests`.
