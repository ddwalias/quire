# Mpl.Run.Keyword

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Atom.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

Base class for MPL keyword symbols, subclassing `Sym` with `SymTag = 5` and `qualifier = "Keyword"`.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `keyword` | 0x04000030 | — | Static string interned as `"Keyword"`. | High | Verified |
| `key_tok` | 0x0400003f | — | Static Tok `Sym.MakeSymQ("Key", "MplMacro")`. | High | Verified |
| `m_keynum` | 0x0400003e | — | Protected readonly integer keyword index. | High | Verified |
| `.cctor()` | 0x06000099 | `4c849103` | Initializes `key_tok = Sym.MakeSymQ("Key", "MplMacro")` and `keyword = string.Intern("Keyword")`. | High | Verified |
| `.ctor(string, int)` | 0x0600009a | `581897e0` | Calls `base(nm, keyword, 5)` and stores `m_keynum = keynum`. | High | Verified |
| `get_KeyNum()` | 0x06000098 | `f796bc15` | Returns `m_keynum`. | High | Verified |
| `get_quote_atom()` | 0x0600009d | `35f9cebc` | Returns `SCons.list2(key_tok, int_constant.NewTok(m_keynum, 0u))`. | High | Verified |
| `walk_declaration_as(MplEnv, SCons)` | 0x0600009e | `a01b1b4c` | Calls `Sexpr.BadDeclaration(form)` (throwing `AppException("illegal declaration", 0u, form.handle)`) and returns `null`. | High | Tested |
| `walk_expression_as(MplEnv, SCons)` | 0x0600009f | `d927a5d3` | Walks tail expressions via `SCons.walk_expressions(env, form.tail)`; if unchanged returns `form`, otherwise returns `new SCons(form.head, walkedTail)`. | High | Verified |
| `key(int)` | 0x0600009b | — | Dispatches 0..14 to keyword singletons. | Medium | Stub |
| `keytok(int)` | 0x0600009c | — | Returns `new Tok(key(n), 0)`. | Medium | Stub |

## Test cases

- **Observed** (`SexprDifferentialTests`): Direct comparisons of `.cctor()`, static fields `keyword` and `key_tok`, `.ctor(string, int)` across multiple names and key numbers, `KeyNum`, `quote_atom`, `walk_declaration_as` throwing `illegal declaration` with form handle, and `walk_expression_as` across unchanged and changed arguments.
- **Inferred** (`SexprTests`): Unit tests covering keyword construction, tag 5 propagation, `KeyNum` accessor, `quote_atom` shape, `walk_declaration_as` exception message and handle, and `walk_expression_as` structure.
