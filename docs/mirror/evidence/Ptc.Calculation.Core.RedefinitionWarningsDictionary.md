# Ptc.Calculation.Core.RedefinitionWarningsDictionary

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** `tools/reference/reference.py Ptc.Calculation.Core.RedefinitionWarningsDictionary --assembly Ptc.Calculation.Core --il`; IL output remains in ignored `.mirror-cache`
- **Quire:** `src/Quire.Calculation.Core`; test: `Quire.Differential.Tests.PtcCalculationCoreRedefinitionWarningsDictionaryCctorDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.cctor()` | 0x06000148 | Creates `KeysOwnerTypes` as a one-element `Type[]` containing `RedefinitionWarningMsgCodes` | Directly compares the field's type-name sequence; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreRedefinitionWarningsDictionaryCctorDifferentialTests`): invokes `.cctor()` on both artifacts and compares the type-name sequence in `KeysOwnerTypes`.
