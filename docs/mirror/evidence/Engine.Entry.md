# Engine.Entry

- **Original:** `iengine.dll` 1.1.0.0, SHA-256 `e34c1c468b07e2016dcd98c4397a139ca6c3a30cd2a14d95563db52e3419947b`, MVID `eefa0fd5-7185-4381-9183-8ef64202914c`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `iengine.dll`.
- **Quire:** `src/Quire.Engine/Entry.cs`; tests: `Quire.Differential.Tests.EngineDataMechanicalDifferentialTests`.

`Entry` is a sequential value type representing a notification message item in `EngMailbox`. Note: In the original Mathcad compiled C++/CLI metadata and IL, the `.ctor` assigns `_tc` to both `trigger_handle` and `trigger_cookie`.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(int, NodeStatus, int, int, int)` | 0x060000c1 | Initializes `h = _h`, `s = _s`, `client_cookie = _cc`, `trigger_handle = _tc`, `trigger_cookie = _tc`. | Exact Release IL; direct mechanical differential comparison. | Verified |

## Test cases

`EngineDataMechanicalDifferentialTests` directly compares `Entry` against Mathcad Prime 12.0.1.0 `iengine.dll`:
- `.ctor(int, NodeStatus, int, int, int)`: verified across generated distinct inputs comparing field values with Mathcad's.

## Open

None.
