# Engine.RedefInfo

- **Original:** `iengine.dll` 1.1.0.0, SHA-256 `e34c1c468b07e2016dcd98c4397a139ca6c3a30cd2a14d95563db52e3419947b`, MVID `eefa0fd5-7185-4381-9183-8ef64202914c`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `iengine.dll`.
- **Quire:** `src/Quire.Engine/RedefInfo.cs`; tests: `Quire.Differential.Tests.EngineDataDifferentialTests`.

`RedefInfo` is a serializable data transfer object storing redefinition warning details: Name, OldType, OldLocation, and NewType.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060000ad | Default parameterless constructor; calls `object..ctor()`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `get_Name()` | 0x060000a5 | Returns `m_name`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `set_Name(string)` | 0x060000a6 | Sets `m_name = value`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `get_OldType()` | 0x060000a7 | Returns `m_oldtype`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `set_OldType(string)` | 0x060000a8 | Sets `m_oldtype = value`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `get_OldLocation()` | 0x060000a9 | Returns `m_oldlocation`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `set_OldLocation(string)` | 0x060000aa | Sets `m_oldlocation = value`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `get_NewType()` | 0x060000ab | Returns `m_newtype`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `set_NewType(string)` | 0x060000ac | Sets `m_newtype = value`. | Exact Release IL; direct mechanical differential comparison. | Verified |

## Test cases

`EngineDataDifferentialTests` directly compares all members of `RedefInfo` against Mathcad Prime 12.0.1.0 `iengine.dll`:
- `.ctor()`, `get_Name`, `set_Name`, `get_OldType`, `set_OldType`, `get_OldLocation`, `set_OldLocation`, `get_NewType`, `set_NewType`: verified across generated distinct inputs comparing target field states and return values.

## Open

None.
