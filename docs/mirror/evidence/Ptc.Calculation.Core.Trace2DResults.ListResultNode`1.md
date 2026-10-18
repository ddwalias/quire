# Ptc.Calculation.Core.Trace2DResults.ListResultNode`1

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Count get` | 0x06000254 | MemberGen: `_data.Count` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item get` | 0x06000255 | MemberGen: `_data[index]; set => _data[index] = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item set` | 0x06000256 | MemberGen: `_data[index]; set => _data[index] = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `List get` | 0x06000257 | MemberGen: `_data` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
