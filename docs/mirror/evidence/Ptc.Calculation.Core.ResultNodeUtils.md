# Ptc.Calculation.Core.ResultNodeUtils

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** `python tools\reference\reference.py --assembly Ptc.Calculation.Core Ptc.Calculation.Core.ResultNodeUtils --il`; ILSpy 11.1.0.9782
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreResultNodeUtilsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ParsePlottableValue(IResultNode)` | 0x060001fe | Assert that `TryParsePlottableValue` succeeds, then return the value | Calls `Quire.DebugTools.DevAssert`, a Quire.Core mirror stub outside Lane C ownership | Blocked |
| `TryParsePlottableValue(IResultNode, out double)` | 0x060001ff | Real returns its value; complex, imaginary, and united values are plottable; unknown types return false and NaN | Direct comparisons cover real, complex, imaginary, nested united-real, and unknown interface proxies; Release IL equals Mathcad's exactly | Verified |

## Test cases

**Observed** (`PtcCalculationCoreResultNodeUtilsMechanicalDifferentialTests`): five direct cases for the private helper; input proxies implement the relevant Mathcad and Quire interfaces and render their semantic kind/value, while `MethodComparison` compares the returned bool and mutated `out` value.
