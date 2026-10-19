# Ptc.Calculation.Core.RequestShowErrorEventArgs

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreRequestShowErrorEventArgsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(EngineError)` | 0x060000c5 | `Error = error` after `EventArgs::.ctor()` | Reconstructed from original IL; direct comparison checks whether the constructed event args retain the exact argument reference | Verified |
| `Error get` | 0x060000c6 | MemberGen: `public EngineError Error { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Error set` | 0x060000c7 | MemberGen: `public EngineError Error { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreRequestShowErrorEventArgsConstructorTests`): non-null and null `EngineError` inputs; the constructor result renderer compares that the exact supplied reference is stored. Existing Tier B getter/setter comparisons remain in `PtcCalculationCoreRequestShowErrorEventArgsMechanicalDifferentialTests`.

**Original evidence:** `python tools\reference\reference.py --assembly Ptc.Calculation.Core Ptc.Calculation.Core.RequestShowErrorEventArgs --il`; artifact version 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`. Constructor IL calls `EventArgs::.ctor()`, loads the `error` argument, calls `set_Error`, and returns.
