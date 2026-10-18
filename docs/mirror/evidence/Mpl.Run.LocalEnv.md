# Mpl.Run.LocalEnv

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Env.cs`; tests: `Quire.Mpl.Tests.Run.EnvTests`, `Quire.Differential.Tests.EnvDifferentialTests`

Scoped local environment supporting parent chaining, local variable bindings, equation bindings, and symbol macros.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `m_level` | 0x040000ab | — | Private nesting level int. | High | Verified |
| `m_parent` | 0x040000ac | — | Private parent `MplEnv`. | High | Verified |
| `m_table` | 0x040000ad | — | Private `SortedDictionary<string, BindingInfo>`. | High | Verified |
| `Level` | 0x0600011b | `3bce8ef3` | Returns `m_level`. | High | Verified |
| `.ctor(MplEnv)` | 0x0600011c | `b5223c14` | Sets `m_level = parent.Level + 1`, `m_parent = parent`, allocates `m_table`. | High | Verified |
| `.ctor()` | 0x0600011d | `ce0e3e24` | Sets `m_level = 0`, `m_parent = null`, allocates `m_table`. | High | Verified |
| `lookup_local(Sym)` | 0x0600011e | `5a294127` | Looks up `s.name` in `m_table`. | High | Verified |
| `lookup(Sym)` | 0x0600011f | `3fb6be4f` | Returns `lookup_local(s) ?? m_parent.lookup(s)`. Throws `NullReferenceException` if parent is null and key is absent. | High | Verified |
| `LoadForeignMacro(string)` | 0x06000120 | `27c0e5e0` | Calls `m_parent.LoadForeignMacro(sig)`. Throws `NullReferenceException` if parent is null. | High | Verified |
| `AddLocal(Sym, uint)` | 0x06000121 | `0dc6628b` | `m_table.Add(s.name, new BindingInfo(handle))`; returns `this`. Duplicate key throws `ArgumentException`. | High | Verified |
| `AddLocalEquation(Sym, uint)` | 0x06000122 | `d18d0b3e` | `m_table[s.name] = new BindingInfo(handle)`; returns `this`. Overwrites existing binding. | High | Verified |
| `AddSymbolMacro(Sym, uint, Sexpr)` | 0x06000123 | `11e25260` | Adds `MacroBindingInfo` wrapping `MplSymbolMacro(s, e)`; returns the binding. Duplicate key throws `ArgumentException`. | High | Verified |
| `AddFunctionMacro(Sym, uint, MplEnv, Sexpr)` | 0x06000124 | `0baab39c` | Stub (pending function macro batch). | High | Blocked |
| `AddForeignMacro(Sym, uint, string)` | 0x06000125 | `d8a66716` | Stub (pending foreign macro batch). | High | Blocked |

## Test cases

- **Observed** (`EnvDifferentialTests`): Direct comparisons of constructors, `Level`, `lookup_local`, `lookup`, `LoadForeignMacro`, `AddLocal`, `AddLocalEquation`, and `AddSymbolMacro` across hierarchy chains, duplicate key additions, overwrites, and parent delegating lookups.
- **Inferred** (`EnvTests`): Behavioural unit tests for scoping, parent chaining, and dictionary lookup edge cases.
