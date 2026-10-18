# Ptc.Calculation.Core.Support.CalculatableChildHider

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060002c9 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HandleCalculatableAddedToChildContainer(ICalculatableContainer, ICalculatable)` | 0x060002d6 | MemberGen: `_fireCalculatableAdded.Invoke(calc)` | Mechanical; Release IL equals Mathcad's | Verified |
| `HandleCalculatableRemovedFromChildContainer(ICalculatable)` | 0x060002d7 | MemberGen: `_fireCalculatableRemoved.Invoke(calc)` | Mechanical; Release IL equals Mathcad's | Verified |
| `HandleCalculatableMovedInChildContainer(ICalculatable)` | 0x060002d8 | MemberGen: `_fireCalculatableMoved.Invoke(calc)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetContainerChildren(ICalculatableContainer)` | 0x060002d9 | MemberGen: `container.CalculatableChildren` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
