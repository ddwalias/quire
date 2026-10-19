# Ptc.Calculation.EngineBuilder

- **Original:** `Ptc.Calculation.dll` 12.0.1.0, SHA-256 `fe8cf4f1b2791b7a223d203785600667bb00a3e2b30e975a949fe412cbf07e4b`, MVID `82391b36-0d84-45c3-b21a-f7dccf288c68`
- **Reference:** `tools/reference/reference.py Ptc.Calculation.EngineBuilder --assembly Ptc.Calculation --il`; IL output remains in ignored `.mirror-cache`
- **Quire:** `src/Quire.Calculation`; test: `Quire.Differential.Tests.PtcCalculationEngineBuilderCctorMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.cctor()` | 0x0600005d | Initializes six static request/unit-system strings | Directly compares all six fields; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationEngineBuilderCctorMechanicalDifferentialTests`): directly invokes `.cctor()` on both artifacts and compares all six initialized strings.
