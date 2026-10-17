# Mpl.Run.MplForeignMacro

- **Original:** `mplrun` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **Reference:** `ilspycmd` 11.1.0.9782 via `tools/reference/reference.py`; member tokens and IL read from the installed artifact.
- **Quire:** `src/Quire.Mpl/Run/MplForeignMacro.cs`; tests: `MplForeignMacroDifferentialTests`.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(MplEnv, Sym, string)` | `0x0600013b` | Calls `MplMacro(Sym)`, then stores the environment, signature, and a null macro cache in that order. | Exact Release IL reconstruction; tested with a null environment and direct call state. | Verified for constructor state. |
| `get_Macro()` | `0x0600013c` | Returns the cached macro when present. Otherwise calls `m_env.LoadForeignMacro(m_sig)`, stores the returned macro, and returns the field. Exceptions propagate without populating the cache. | Cached return and null-environment exception compared directly. The successful miss needs an original host environment not available in the `mplrun` artifact. | Tested; successful cold-cache path blocked. |
| `ExpandCall(MplEnv, Sexpr)` | `0x0600013d` | Gets `Macro` and forwards the supplied environment and form to `MplMacro.ExpandCall`; exceptions propagate. | Successful cold-cache forwarding and null-environment failure compared directly. | Verified for recorded cases. |
| `ExpandRef(MplEnv, Tok)` | `0x0600013e` | Gets `Macro` and forwards the supplied environment and token to `MplMacro.ExpandRef`; exceptions propagate. | Successful cold-cache forwarding and null-environment failure compared directly. | Verified for recorded cases. |

## Test cases

`MplForeignMacroDifferentialTests` compares constructor state, cache-hit return without an environment, forwarding through cached `MplSymbolMacro` instances, and null-environment failures from the getter and both forwarding methods. The forwarding comparisons use `MplSymbolMacro` to observe the call and reference arguments.

## Open

The successful cache-miss path, including the assignment and return after `LoadForeignMacro`, remains blocked. In `mplrun`, the only `MplEnv` implementations found are the base implementation, which throws, and `LocalEnv`, which forwards to its parent. `LocalEnv` can therefore reach the successful path only when given a parent environment supplied by another assembly. The installed original `mplrun` artifact does not provide such an environment, and the test linter forbids generating one at runtime. The path is not claimed unreachable in Mathcad and no unreachable-line proposal is made; validation reports those lines until a suitable original host environment can be compared.

### Host-environment search update (2026-10-18)

REA's execution-free managed inspection identified installed `iengine.dll` as mixed C++/CLI (SHA-256 `e34c1c468b07e2016dcd98c4397a139ca6c3a30cd2a14d95563db52e3419947b`, MVID `eefa0fd5-7185-4381-9183-8ef64202914c`; Evidence `ev_e0c8f513a82d9d54bdc33a95482df72239a3fff47642216ca3a6341c75146be4`). Its managed-member inspection returned 218 method records with partial coverage and no `MplEnv` method references (Evidence `ev_94dd6839de48104feff155f7a163092b4a5c6738543e2165fe4d9df42f1974c7`). A bounded raw-text scan of installed `*.dll` and `*.exe` files found the string `MplEnv` only in `mplrun.dll`, `msgen.dll`, and `mplmacro.dll`; managed-member inspection of the latter two found no matching methods. This does not exclude native host code or dynamically supplied environments, so the successful miss remains an unresolved, potentially reachable path.
