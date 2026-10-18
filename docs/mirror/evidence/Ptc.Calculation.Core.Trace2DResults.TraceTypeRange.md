# Ptc.Calculation.Core.Trace2DResults.TraceTypeRange

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsTraceTypeRangeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Start get` | 0x060002c4 | MemberGen: `public int Start { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Start set` | 0x060002c5 | MemberGen: `public int Start { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `End get` | 0x060002c6 | MemberGen: `public int End { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `End set` | 0x060002c7 | MemberGen: `public int End { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsTraceTypeRangeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
