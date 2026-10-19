# Engine.ComponentHelpers

- **Original:** `iengine.dll` 1.1.0.0, SHA-256 `e34c1c468b07e2016dcd98c4397a139ca6c3a30cd2a14d95563db52e3419947b`, MVID `eefa0fd5-7185-4381-9183-8ef64202914c`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `iengine.dll`.
- **Quire:** `src/Quire.Engine/ComponentHelpers.cs`; tests: `Quire.Engine.Tests.ComponentHelpersTests`, `Quire.Differential.Tests.EngineDataDifferentialTests`.

`ComponentHelpers` provides utility methods to interact with `IMathcadComponent2` instances: locking/unlocking the component and mapping get/set value calls with `AppException` error tagging and argument translation.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Lock(IMathcadComponent2)` | 0x060000ca | Calls `c.LockComponent()`; returns 1 on success, throws `AppException("cannot_lock_component")` on false. | Exact Release IL; behavioral tests pass. | Verified |
| `Unlock(IMathcadComponent2)` | 0x060000cb | Calls `c.UnlockComponent()`; returns 0. | Exact Release IL; mechanical differential comparison. | Verified |
| `SetValue(IMathcadComponent2, uint, uint, string, object)` | 0x060000cc | Calls `c.SetValue(place, v as IValue)`; catches `AppException`, wraps/retags with tag1 (if argument is 1) or tag2 otherwise. | Exact Release IL; behavioral tests pass. | Verified |
| `GetValue(IMathcadComponent2, string)` | 0x060000cd | Calls `c.GetValue(place)`; catches `AppException`, converts argument 1 to 2 before rethrowing. | Exact Release IL; behavioral tests pass. | Verified |

## Test cases

- `ComponentHelpersTests`: verifies all branches of `Lock`, `Unlock`, `SetValue`, and `GetValue` including success and `AppException` translation with argument checks.
- `EngineDataDifferentialTests`: direct mechanical comparison of `Unlock` (0x060000cb) against Mathcad Prime 12.0.1.0 `iengine.dll`.

## Open

None.
