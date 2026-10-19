# Ptc.Calculation.Core.EngineErrorExtensions

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** `python tools\reference\reference.py --assembly Ptc.Calculation.Core Ptc.Calculation.Core.EngineErrorExtensions --il`; ILSpy 11.1.0.9782
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreEngineErrorExtensionsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `IsPlaceholderError(EngineError)` | 0x06000040 | Return false for null; otherwise compare `ErrorCode` to `bad%_placeholder` | Direct cases cover null, matching and nonmatching codes; `EngineError.ErrorCode` getter is independently exact in Release | Verified |

## Test cases

**Observed** (`PtcCalculationCoreEngineErrorExtensionsMechanicalDifferentialTests`): null input, a private `_code` value equal to the placeholder string, and a different value. The renderer compares the relevant error code rather than unrelated Tier B EngineError state.
