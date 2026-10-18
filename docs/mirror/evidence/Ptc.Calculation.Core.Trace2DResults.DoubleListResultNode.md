# Ptc.Calculation.Core.Trace2DResults.DoubleListResultNode

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsDoubleListResultNodeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(IList)` | 0x06000259 | MemberGen: `: base(doubleValues, -1) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor(IList, double, int)` | 0x0600025a | MemberGen: `: base(doubleValues, paddingValue, minLength) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsMeaningfulValue(double)` | 0x0600025c | MemberGen: `DoubleExtension.IsValid(value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `NeutralValue get` | 0x0600025b | MemberGen: `double.NaN` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsDoubleListResultNodeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
