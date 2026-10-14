# Mpl.Run.Atom

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Atom.cs`; tests: `Quire.Mpl.Tests.Run.AtomTests`, `Quire.Differential.Tests.SexprDifferentialTests`

Base of symbols and constants. The typed accessors build an `AppException` and discard it, then return a default: they never throw (pre-§3c Quire threw `InvalidCastException`). Building the exception matters once `AppException` is implemented, since its constructor resets the runtime stack depth.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `SymTag … KeyTag` | 0x04000020–0x04000025 | — | Constants 0 (Sym), 1 (Int), 2 (Real), 3 (Char), 4 (Str), 5 (Key). | High | Verified |
| `.ctor(int)` | 0x06000077 | `3a7dcc25` | Passes the tag on. | High | Tested |
| `KeyNum` | 0x06000078 | `86eff5bd` | Builds `AppException("not a keyword", tag)`, returns 0. | High | Verified |
| `RealCon` | 0x06000079 | `083d7686` | Builds `AppException("not a real constant", tag)`, returns 0.0. | High | Verified |
| `IntCon` | 0x0600007a | `64f77b8f` | Builds `AppException("not an integer constant", tag)`, returns 0. | High | Verified |
| `CharCon` | 0x0600007b | `f972bb53` | Builds `AppException("not a character constant")`, returns `\0`. | High | Verified |
| `StrCon` | 0x0600007c | `afe2fd08` | Builds `AppException("not a string constant")`, returns null. | High | Verified |
| `quote_atom` | 0x0600007d | `b440aa4e` | Throws `AppException("quotation not implemented", 0, 0)`. | High | Verified |
| `sym` | 0x0600007e | `e0498a9f` | `this as Sym`. | High | Verified |
| `Key` | 0x0600007f | `0400af05` | `this as Keyword`. | High | Verified |

## Test cases

**Observed** (`SexprDifferentialTests.Atom_members`): every accessor on 22 atoms (symbols of each case, empty, null,
qualified, keyword-tagged, with a uid; ints, reals incl. −∞ and ε, strings incl. null, chars incl. `\0`).
**Inferred** (`AtomTests`): defaults from the wrong kind of atom, tags.
