# Mpl.Run.BindingInfo

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Env.cs`; tests: `Quire.Mpl.Tests.Run.EnvTests`, `Quire.Differential.Tests.EnvDifferentialTests`

Represents a symbol binding in an environment. `BindingInfo` represents a value binding, while subclass `MacroBindingInfo` wraps an `MplMacro`.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Value` | 0x040000a6 | — | Constant 0. | High | Verified |
| `Macro` | 0x040000a7 | — | Constant 1. | High | Verified |
| `m_handle` | 0x040000a8 | — | Private handle uint. | High | Verified |
| `m_zone` | 0x040000a9 | — | Private zone int. | High | Verified |
| `macro` | 0x06000115 | `25a61114` | Virtual getter returning null on base `BindingInfo`. | High | Verified |
| `.ctor()` | 0x06000116 | `58d2bd4d` | Calls `this(0u, 0)`. | High | Verified |
| `.ctor(uint, int)` | 0x06000117 | `5eb211fd` | Sets `m_handle` and `m_zone`. | High | Verified |
| `.ctor(uint)` | 0x06000118 | `452bd54c` | Calls `this(handle, 0)`. | High | Verified |
| `m_macro` | 0x040000aa | — | Private `MplMacro` on `MacroBindingInfo`. | High | Verified |
| `macro (override)` | 0x06000119 | `a07dd00c` | Override getter returning `m_macro`. | High | Verified |
| `.ctor(uint, MplMacro)` | 0x0600011a | `1e191f51` | Calls `base(handle, 1)` and sets `m_macro`. | High | Verified |

## Test cases

- **Observed** (`EnvDifferentialTests`): Direct comparisons of all constructors and `macro` property getters on both `BindingInfo` and `MacroBindingInfo`.
- **Inferred** (`EnvTests`): Constructors and properties tested against edge cases.
