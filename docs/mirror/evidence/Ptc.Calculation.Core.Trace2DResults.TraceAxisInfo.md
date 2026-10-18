# Ptc.Calculation.Core.Trace2DResults.TraceAxisInfo

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsTraceAxisInfoMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Max get` | 0x060002aa | MemberGen: `public double Max { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Max set` | 0x060002ab | MemberGen: `public double Max { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Min get` | 0x060002ac | MemberGen: `public double Min { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Min set` | 0x060002ad | MemberGen: `public double Min { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Units get` | 0x060002ae | MemberGen: `public IList<Unit> Units { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Units set` | 0x060002af | MemberGen: `public IList<Unit> Units { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsTraceAxisInfoMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
