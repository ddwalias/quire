# Ptc.Calculation.Core.Support.CalculatableWrapper

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `HandleRequestEvaluation(ICalculatable)` | 0x060002e3 | MemberGen: `FireRequestEvaluation(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `RequestEvaluation add` | 0x060002e7 | MemberGen: `public event Action<ICalculatable> RequestEvaluation;` | Mechanical; Release IL equals Mathcad's | Verified |
| `RequestEvaluation remove` | 0x060002e8 | MemberGen: `public event Action<ICalculatable> RequestEvaluation;` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetValueOfIdentifierBefore add` | 0x060002e9 | MemberGen: `public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetValueOfIdentifierBefore remove` | 0x060002ea | MemberGen: `public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;` | Mechanical; Release IL equals Mathcad's | Verified |
| `TreatAsUpToDate add` | 0x060002eb | MemberGen: `public event Action<ICalculatable> TreatAsUpToDate;` | Mechanical; Release IL equals Mathcad's | Verified |
| `TreatAsUpToDate remove` | 0x060002ec | MemberGen: `public event Action<ICalculatable> TreatAsUpToDate;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
