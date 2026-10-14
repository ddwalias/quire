# Mpl.Run.Sym

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Atom.cs`; tests: `Quire.Mpl.Tests.Run.AtomTests`, `Quire.Differential.Tests.SexprDifferentialTests`

**Status rule used in this batch.** *Verified*: the differential test runs every branch of the member's own body
against Mathcad. *Tested*: Quire's behaviour tests run every branch (with test doubles where a branch needs a virtual
callee), but the differential doesn't reach all of them. *Analyzed*: implemented from the contract, but a branch
calls a member of another batch that is still a stub in Quire, so it can't run yet.

A symbol: name, qualifier and an unmanaged id. **No interning**: mplrun keeps no symbol table; `Equals` is
reference equality, and every constructor or factory call makes a new symbol. **The qualifier is dropped** by
`.ctor(string, string)` (it always stores null), and so by `.ctor(object)`, `.ctor(string, string, uint)`, every
`NewTok` and `MakeSymQ`; only `.ctor(string, string, int tag)` keeps it. Observed directly: Mathcad's
`new Sym("a", "b").qualifier` is null. Pre-§3c Quire kept the qualifier, printed `|…|`-quoted names and turned null
names into "".

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1, Field2` | 0x04000026, 0x04000027 | — | Public mutable name and qualifier. | High | Verified |
| `m_umgd` | 0x04000028 | — | Private uint behind `uid`. | High | Verified |
| `make_sym, make_symq` | 0x04000029, 0x0400002a | — | Static readonly `MakeSymQ("Sym", "MplMacro")` and `MakeSymQ("make_symbolq", "MplMacro")`: tokens of unqualified symbols. | High | Verified |
| `.ctor(object)` | 0x06000089 | `4d8224c9` | `nm as string` (null for anything else), no qualifier. | High | Verified |
| `.ctor(string, string)` | 0x0600008a | `49cb44d4` | Tag 0, name as given (null allowed), qualifier null whatever `q` is. | High; observed | Verified |
| `.ctor(string, string, uint)` | 0x0600008b | `c031e6a9` | The two-argument constructor, then `uid = id`. | High | Verified |
| `.ctor(string, string, int)` | 0x0600008c | `26f5ada6` | The given tag, name and qualifier. | High | Verified |
| `name, qualifier` | 0x06000080, 0x06000081 | `e71fa0d7` | `Field1`, `Field2`. | High | Verified |
| `UpId, LowId` | 0x06000082, 0x06000083 | `405c0eaf` | `char.IsUpper/IsLower(Field1, 0)`: empty names throw `ArgumentOutOfRangeException`, null `ArgumentNullException`. | High | Verified |
| `long_name` | 0x06000084 | `4e4bd143` | With a qualifier, `name + "." + qualifier` (name first); else the name. | High | Verified |
| `uid` | 0x06000085, 0x06000086 | `aea73ba4` | Reads and writes `m_umgd`. | High | Verified |
| `KeyNum` | 0x06000087 | `0a5611c7` | −1. | High | Verified |
| `NewTok(string, uint)` | 0x0600008d | `7afd4a17` | `new Tok(new Sym(nm, null), tag)`. | High | Verified |
| `NewTok(string, string, uint)` | 0x0600008e | `f9e13e64` | `new Tok(new Sym(nm, q), tag)`: qualifier dropped. | High | Verified |
| `NewTok(string, uint, uint)` | 0x0600008f | `ea6fa6a4` | With the uid. | High | Verified |
| `NewTok(string, string, uint, uint)` | 0x06000090 | `bc4e9ca2` | With the uid; qualifier dropped. | High | Verified |
| `MakeSym(string)` | 0x06000091 | `4be6555b` | `NewTok(nm, 0)`. | High | Verified |
| `MakeSymQ(string, string)` | 0x06000092 | `26421653` | `NewTok(nm, q, 0)`. | High | Verified |
| `ToString()` | 0x06000093 | `e71fa0d7` | The name alone (same IL as `name`); null for a null name. | High | Verified |
| `quote_atom` | 0x06000094 | `4b6bb7cf` | Qualified: `(make_symbolq "name" "qualifier")`; else `(Sym "name")`, the strings as `str_constant` tokens with handle 0. | High | Verified |
| `walk_declaration_as(MplEnv, SCons)` | 0x06000095 | `3df98eb8` | The symbol's macro (`env.LoadMacro(this)`) expands the form, whose left-hand side is then walked; with no macro, `BadDeclaration(form)`. | High; every path calls `MplEnv.LoadMacro`, a stub in Quire | Analyzed |
| `walk_definition_as(MplEnv, SCons)` | 0x06000096 | `1072839c` | `BadDeclaration(form)`; keywords override it. | High | Verified |
| `walk_expression_as(MplEnv, SCons)` | 0x06000097 | `b8c8b715` | A lower-case symbol with a macro expands the form and walks the result. Otherwise the arguments (`form.tail`) are walked as expressions: unchanged gives `form`, changed a new cell of `form.head` and the new arguments. | High; the macro branch calls `MplEnv.LoadMacro`, still a stub in Quire (Keyword/env batch), so only the other branches ran | Analyzed |

## Test cases

As in Mpl.Run.Sexpr.md and Mpl.Run.Atom.md; constructors and factories are compared on names "a", null, "" and
qualifiers "b", null, "".

## Open

- `walk_declaration_as` and the macro branch of `walk_expression_as` wait for `MplEnv.LoadMacro` and
  `MplMacro.ExpandCall` (Keyword/env batch).
