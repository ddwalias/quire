# Ptc.Calculation.Core.SyncStatusChangedEventArgs

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreSyncStatusChangedEventArgsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetArgs()` | 0x060000b6 | MemberGen: `_singleton` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x060000b7 | MemberGen: `: base("SynchronizationStatus") { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.cctor()` | 0x060000b8 | Initializes `_singleton` with a new `SyncStatusChangedEventArgs` | Directly compared; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreSyncStatusChangedEventArgsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

**Observed** (`PtcCalculationCoreSingletonCctorsDifferentialTests`): directly invokes `.cctor()` on both artifacts and compares whether the singleton field is null, its runtime type, and its inherited `PropertyName`. The constructor's Release IL is an exact match.
