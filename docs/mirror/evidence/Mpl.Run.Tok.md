# Mpl.Run.Tok

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Sexpr.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

**Status rule used in this batch.** *Verified*: the differential test runs every branch of the member's own body
against Mathcad. *Tested*: Quire's behaviour tests run every branch (with test doubles where a branch needs a virtual
callee), but the differential doesn't reach all of them. *Analyzed*: implemented from the contract, but a branch
calls a member of another batch that is still a stub in Quire, so it can't run yet.

An atom with the source handle it came from.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1, Field2` | 0x04000019, 0x0400001a | — | Public mutable atom and handle. | High | Verified |
| `tok` | 0x0400001b | — | Private static `new Sym("Tok", "MplMacro")` (qualifier dropped). | High | Verified |
| `.ctor(object, int)` | 0x06000069 | `c7365c5c` | `as Atom`, so other objects give a null atom; the int is reinterpreted as uint. | High | Verified |
| `.ctor(Atom, int), .ctor(Atom, uint)` | 0x0600006a, 0x0600006b | `52e399c1` | Tag 2; identical IL. | High | Verified |
| `handle` | 0x06000062 | `b7f6669c` | `Field2`. | High | Verified |
| `atom` | 0x06000063 | `ec029916` | `Field1`. | High | Verified |
| `Key, sym` | 0x06000064, 0x06000065 | `41f3eb74` | `Field1 as Sym` (identical IL). | High | Verified |
| `str` | 0x06000066 | `d29e20e9` | `Field1.StrCon`: null for non-string atoms (no exception), `NullReferenceException` for a null atom. | High | Verified |
| `quote` | 0x06000067 | `8146d0d4` | `list3(new Tok(tok, Field2), Field1.quote_atom, int_constant.zero)`. | High | Verified |
| `.cctor` | 0x06000068 | — | Initialises tok. | High | Verified |
| `ToString()` | 0x0600006c | `3d4570c0` | The atom's text; with a non-zero handle, `"#T" + handle in upper-case hex + " " + atom`. A null atom throws (pre-§3c Quire printed ""). | High | Verified |
| `walk_pattern(MplEnv)` | 0x0600006d | `7fcc54c9` | Not a symbol: this. An upper-case symbol: its macro's `ExpandRef` walked as a pattern, or this with no macro. Any other symbol: `env.AddLocal(key, handle)`, then this. Empty or null names throw from the case test. | High; the macro branch calls `MplEnv.LoadMacro`, still a stub in Quire (Keyword/env batch), so only the other branches ran | Analyzed |
| `walk_expression(MplEnv)` | 0x0600006e | `d60e61f0` | Not a symbol, or not lower case: this. A lower-case symbol: its macro's `ExpandRef` walked as an expression, or this. | High | Verified |

## Test cases

As in Mpl.Run.Sexpr.md. Differential tests in `SexprDifferentialTests` verify `walk_expression` across non-symbols (int, real, str, char, null), uppercase symbols, unbound lowercase symbols in empty `LocalEnv()`, bound local symbols (`AddLocal`), bound symbol macros (`AddSymbolMacro`), chained symbol macros, function macros throwing `AppException("illegal macro reference")` with exact handle matching, and scoped parent resolution and shadowing in nested `LocalEnv(parent)`.

## Open

- Null-environment cases (`SexprDifferentialTests`) pass a null `MplEnv` to both sides; they show only that the
  environment is dereferenced before any other observable effect, not which branch follows.
- The macro branch of `walk_pattern` waits for `MplEnv.LoadMacro`/`MplMacro.ExpandRef`; `walk_pattern`'s local branch for `LocalEnv.AddLocal`.
