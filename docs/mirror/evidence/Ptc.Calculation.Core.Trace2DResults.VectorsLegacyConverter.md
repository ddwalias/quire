# Ptc.Calculation.Core.Trace2DResults.VectorsLegacyConverter

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ReadAsColumns get` | 0x06000265 | MemberGen: `_readAsColumns` | Directly compared on an uninitialized target with `_readAsColumns = true` | Verified |
| `ReadDoubleOrNone(IResultNode, out double)` | 0x0600026c | Initializes the out value to NaN; complex/imaginary and real nodes return true, with real values copied; other nodes return false | Direct Mathcad cases cover null, finite and NaN real, complex, imaginary and string nodes; Release IL exact | Verified |
| `ReadString(IResultNode, out string)` | 0x0600026d | Initializes the out value to null; strings return true and their value; complex/imaginary or NaN-real return true with null; other nodes return false | Direct Mathcad cases cover null, string, finite and NaN real, complex and imaginary nodes; Release IL exact | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests`): `ReadDoubleOrNone` and `ReadString` are compared directly against Mathcad with interface proxies. Both return values and mutated `out` values match across the listed cases. `ReadAsColumns` is compared on uninitialized targets with matching `_readAsColumns` fields.

Release MethodAudit inspected 13 declarations in this type and reports exact IL for ReadAsColumns, ReadDoubleOrNone, and ReadString. Nine ancillary assembly inspection errors are outside this type. The current Release Mathcad class run passed 3/3; the normal gate is pending.
