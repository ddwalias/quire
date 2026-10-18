# Mpl.Run.MplMacro

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Env.cs`; tests: `Quire.Mpl.Tests.Run.EnvTests`, `Quire.Differential.Tests.EnvDifferentialTests`

Base class for MPL macros, providing standard expand hook defaults, error dispatch, and string formatting.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `m_name` | 0x040000ae | — | Public `Sym` name. | High | Verified |
| `.ctor(Sym)` | 0x06000126 | `d20c28eb` | Stores `m_name = sym`. | High | Tested |
| `ExpandRef(MplEnv, Tok)` | 0x06000127 | `5e9c992b` | Calls `IllegalMacroRef(s)`. | High | Tested |
| `ExpandCall(MplEnv, Sexpr)` | 0x06000128 | `12ffe11e` | Calls `IllegalMacroCall(form)`. | High | Tested |
| `IllegalMacroCall(Sexpr)` | 0x06000129 | `64c02848` | Throws `AppException("illegal macro call", 0u, form.handle)`. | High | Verified |
| `IllegalMacroRef(Tok)` | 0x0600012a | `fed031ac` | Throws `AppException("illegal macro reference", 0u, form.handle)`. | High | Verified |
| `NotAtTopLevel(Sym, uint)` | 0x0600012b | `a775f6fe` | Throws `AppException("function macro definition not at top level", 0u, handle)`. | High | Verified |
| `CanNotExpand(uint)` | 0x0600012c | `3e55906f` | Throws `AppException("Can not expand this macro in the current compilation unit.", 0u, handle)`. | High | Verified |
| `ToString()` | 0x0600012d | `1f4c6078` | MemberGen: `m_name.ToString()` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

- **Observed** (`EnvDifferentialTests`): Direct comparisons of `IllegalMacroCall`, `IllegalMacroRef`, `NotAtTopLevel`, and `CanNotExpand`.
- **Inferred** (`EnvTests`): Base `MplMacro` virtual methods (`ExpandRef`, `ExpandCall`) and constructor tested via test subclass.

MemberGen members are compared by `Quire.Differential.Tests.MplRunMplMacroMechanicalDifferentialTests`.
