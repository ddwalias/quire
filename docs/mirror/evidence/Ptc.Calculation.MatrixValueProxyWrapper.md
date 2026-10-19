# Ptc.Calculation.MatrixValueProxyWrapper

- **Original:** `Ptc.Calculation.dll` 12.0.1.0, SHA-256 `fe8cf4f1b2791b7a223d203785600667bb00a3e2b30e975a949fe412cbf07e4b`, MVID `82391b36-0d84-45c3-b21a-f7dccf288c68`
- **Reference:** `tools/reference/reference.py Ptc.Calculation.MatrixValueProxyWrapper --assembly Ptc.Calculation --il`; IL output remains in ignored `.mirror-cache`
- **Quire:** `src/Quire.Calculation`; test: `Quire.Differential.Tests.PtcCalculationMatrixValueProxyWrapperCctorMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.cctor()` | 0x06000077 | Initializes `PlainDataConversionOptions.ImaginaryThreshold` to `double.NaN` and stores the options in `_conversionOptions` | Directly compares the initialized threshold; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationMatrixValueProxyWrapperCctorMechanicalDifferentialTests`): directly invokes `.cctor()` on both artifacts and compares the options threshold.
