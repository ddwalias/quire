# Ptc.Calculation.Core.Trace2DResults.StringListResultNode

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsStringListResultNodeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(IList)` | 0x0600025d | MemberGen: `: base(data, -1) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor(IList, string, int)` | 0x0600025e | MemberGen: `: base(data, paddingValue, minLength) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `NeutralValue get` | 0x06000260 | MemberGen: `null` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsMeaningfulValue(string)` | 0x0600025f | Return whether the string reference is non-null | ILSpy C# is `value != null`; Release IL exactly matches Mathcad | Verified (null and non-null values) |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsStringListResultNodeMechanicalDifferentialTests`): the three MemberGen comparisons are recorded above; `IsMeaningfulValue` directly compares null and non-null string inputs. Its Release Mathcad test passed 1/1, and Release MethodAudit reports Quire token `0x06000315` exactly matching original token `0x0600025f` (`ldarg.1; ldnull; cgt.un; ret`).
