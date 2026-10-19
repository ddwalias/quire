# Ptc.Calculation.Core.Trace2DResults.VectorsConverterBase

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsVectorsConverterBaseMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000272 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetVectorsValueType(PlainDataTypes)` | 0x06000270 | Classify uninitialized, NaN-only, numeric, uniformly united numeric, string, mixed supported values, and unknown flag combinations | Original IL returns `Numeric=1`, `Mixed=3`, and `NaNs=4`, but Quire's local `ResultValueType` has conflicting names at those values and lacks these categories. Keep this method blocked until the enum mapping is resolved; raw numeric casts would conceal the name/semantic mismatch. | Blocked |
| `IsOfExactPlainType(PlainDataTypes, PlainDataTypes)` | 0x06000271 | True only when the requested flag is present and no other flags are set | Direct Release Mathcad comparisons for exact zero/NaN, overlapping, and absent-flag combinations | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsVectorsConverterBaseMechanicalDifferentialTests`): the constructor remains directly compared by `Mechanical`; `IsOfExactPlainType` is invoked directly through `MethodComparison` against the installed Mathcad artifact. Cases cover exact zero/NaN, absent bits, and multi-bit inputs. `GetVectorsValueType` remains blocked because Quire's current `ResultValueType` labels do not match the numeric categories emitted by Mathcad's original IL.
