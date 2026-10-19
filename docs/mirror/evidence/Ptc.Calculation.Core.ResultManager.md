# Ptc.Calculation.Core.ResultManager

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreResultManagerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `CreateOldResultRefId(IResultNode, ICalculatableResultOwner)` | 0x0600015f | MemberGen: `AddOldResult(resultNode, calculatable)` | Direct comparison currently calls the Tier B `AddOldResult` mirror stub | Blocked |
| `.ctor()` | 0x0600014e | `Clear()` | Reconstructed from original IL; constructor and field state compared directly | Verified |
| `Clear()` | 0x0600014f | `_resultKey = (_oldResultKey = -1)` and new empty `_results` dictionary | Reconstructed from original IL; Release IL equals Mathcad's | Verified |
| `.cctor()` | — | `_instance = new ResultManager()` | Original type initializer IL is `newobj`, `stsfld _instance`, `ret`; direct comparison also checks initialized counters and empty dictionary | Verified |
| `Instance get` | 0x06000150 | MemberGen: `_instance` | Mechanical; Release IL equals Mathcad's | Verified |
| `Results get` | 0x06000164 | MemberGen: `_results` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreResultManagerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's. Constructor and `Clear` direct comparisons include the `_oldResultKey`, `_resultKey`, and `_results` state. The `.cctor()` comparison records the singleton's type, both counters, and dictionary count against Mathcad.

**Original evidence:** `python tools\reference\reference.py --assembly Ptc.Calculation.Core Ptc.Calculation.Core.ResultManager --il`; artifact version 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`. The `.ctor()` calls `Object::.ctor()` then `Clear()`. `Clear()` stores -1 to `_oldResultKey` and `_resultKey`, creates an empty dictionary, and returns. `.cctor()` constructs `ResultManager`, stores it in `_instance`, and returns.
