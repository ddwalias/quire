# Engine.AlgoInfo

- **Original:** `iengine.dll` 1.1.0.0, SHA-256 `e34c1c468b07e2016dcd98c4397a139ca6c3a30cd2a14d95563db52e3419947b`, MVID `eefa0fd5-7185-4381-9183-8ef64202914c`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `iengine.dll`.
- **Quire:** `src/Quire.Engine/AlgoInfo.cs`; tests: `Quire.Differential.Tests.EngineDataMechanicalDifferentialTests`.

`AlgoInfo` is a serializable data transfer object storing metadata about an engine algorithm instance: its Name, Tag, and Region.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060000b4 | Default parameterless constructor; calls `object..ctor()`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `get_Name()` | 0x060000ae | Returns `m_name`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `set_Name(string)` | 0x060000af | Sets `m_name = value`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `get_Tag()` | 0x060000b0 | Returns `m_tag`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `set_Tag(int)` | 0x060000b1 | Sets `m_tag = value`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `get_Region()` | 0x060000b2 | Returns `m_region`. | Exact Release IL; direct mechanical differential comparison. | Verified |
| `set_Region(int)` | 0x060000b3 | Sets `m_region = value`. | Exact Release IL; direct mechanical differential comparison. | Verified |

## Test cases

`EngineDataDifferentialTests` directly compares all members of `AlgoInfo` against Mathcad Prime 12.0.1.0 `iengine.dll`:
- `.ctor()`, `get_Name`, `set_Name`, `get_Tag`, `set_Tag`, `get_Region`, `set_Region`: verified across generated distinct inputs comparing target field states and return values.

## Open

None.
