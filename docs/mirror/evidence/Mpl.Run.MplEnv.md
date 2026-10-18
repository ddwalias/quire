# Mpl.Run.MplEnv

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Env.cs`; tests: `Quire.Mpl.Tests.Run.EnvTests`, `Quire.Differential.Tests.EnvDifferentialTests`

Base environment contract for symbol binding and macro lookup. The base class represents an immutable / import environment that rejects local modifications.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Level` | 0x06000109 | `7b93d2fb` | Returns 0. | High | Tested |
| `AddLocal(Sym, uint)` | 0x0600010a | `09fba597` | Calls `MayNotModify()`, returns null. | High | Tested |
| `AddLocalEquation(Sym, uint)` | 0x0600010b | `09fba597` | Calls `MayNotModify()`, returns null. | High | Tested |
| `AddSymbolMacro(Sym, uint, Sexpr)` | 0x0600010c | `d1808769` | Calls `MayNotModify()`, returns null. | High | Tested |
| `AddFunctionMacro(Sym, uint, MplEnv, Sexpr)` | 0x0600010d | `09fba597` | Calls `MayNotModify()`, returns null. | High | Tested |
| `AddForeignMacro(Sym, uint, string)` | 0x0600010e | `d1808769` | Calls `MayNotModify()`, returns null. | High | Tested |
| `lookup(Sym)` | 0x0600010f | — | Abstract: looks up symbol binding info. | High | Verified |
| `LoadMacro(Sym)` | 0x06000110 | `4a4cc9a3` | If `s is Keyword`, returns null; otherwise returns `lookup(s)?.macro`. | High | Tested |
| `LoadForeignMacro(string)` | 0x06000111 | `655a98a0` | Calls `CanNotResolve(foreign_id)`, returns null. | High | Tested |
| `CanNotResolve(string)` | 0x06000112 | `96800dec` | Throws `AppException("Can not resolve signature")`. | High | Verified |
| `MayNotModify()` | 0x06000113 | `2fe00d11` | Throws `AppException("May not modify import environment")`. | High | Verified |
| `.ctor()` | 0x06000114 | `dca389a4` | Protected parameterless constructor. | High | Tested |

## Test cases

- **Observed** (`EnvDifferentialTests`): Direct comparisons for static exception helpers `CanNotResolve` and `MayNotModify`, and `LoadMacro` across Keyword, bound symbol macro, local binding, and unbound symbol lookup.
- **Inferred** (`EnvTests`): Base `MplEnv` virtual methods (`Level`, `AddLocal`, `AddLocalEquation`, `AddSymbolMacro`, `AddFunctionMacro`, `AddForeignMacro`, `LoadForeignMacro`) tested via a test subclass to verify default 0 level and modification rejection exceptions.

`LoadMacro` remains Tested because the existing root `LocalEnv` throws on an unbound lookup, so its non-keyword/no-binding null return has not been exercised. Its direct comparisons cover the Keyword, unbound-root exception, local-value and symbol-macro cases.
