# Ptc.Calculation.Core.RequestShowErrorEventArgs

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreRequestShowErrorEventArgsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(EngineError)` | 0x060000c5 | `Error = error` after `EventArgs::.ctor()` | Reconstructed from original IL; direct comparison checks whether the constructed event args retain the exact argument reference | Verified |
| `Error get` | 0x060000c6 | Returns the stored Error property value | Direct Release state/reference comparison; Release IL equals Mathcad original | Verified |
| `Error set` | 0x060000c7 | Stores the supplied Error property value | Direct Release state/reference comparison; Release IL equals Mathcad original | Verified |

## Test cases

Getter and setter compare exact EngineError reference storage and null state with Mathcad. Constructor comparisons cover null and non-null inputs; all renderers normalize EngineError to whether the exact expected reference is returned or stored.

**Original evidence:** `python tools\reference\reference.py --assembly Ptc.Calculation.Core Ptc.Calculation.Core.RequestShowErrorEventArgs --il`; artifact version 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`. Constructor IL calls `EventArgs::.ctor()`, loads the `error` argument, calls `set_Error`, and returns.
