# Mpl.Run.block_keyword

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Atom.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

MPL `block` keyword symbol (`KeyBlock = 4`), subclassing `Keyword`. Represents block expressions and declarations.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `singleton` | 0x04000040 | — | Static singleton instance. | High | Verified |
| `.cctor()` | 0x060000aa | `2fbf11` | Initializes `singleton = new block_keyword()`. | High | Verified |
| `.ctor()` | 0x060000ab | `2fcb13` | Calls `base("block", 4)`. | High | Verified |
| `NewTok(uint)` | 0x060000ac | `2fd912` | Returns `new Tok(singleton, handle)`. | High | Verified |
| `MakeBlock(uint, Sexpr)` | 0x060000ae | `302511` | Returns `new SCons(new Tok(singleton, handle), forms)`. | High | Verified |
| `walk_declaration_as(MplEnv, SCons)` | 0x060000ad | `2fe831` | If `form.tail != null`, appends `tail.head.walk_declaration_lhs(env)` to recursive `walk_declaration_as(env, tail)`; otherwise returns `form.Field2.walk_declaration_lhs(env)`. | High | Verified |

## Test cases

- **Observed** (`SexprDifferentialTests`): Direct comparisons of `.cctor()`, `.ctor()`, `singleton` identity, `NewTok(uint)` across handles, `MakeBlock(uint, Sexpr)` across empty and populated forms, `walk_declaration_as` on empty block `(block)` and block with declarations, and `Sexpr.WalkModule` producing `(block ...)` with 100% verified status.
- **Inferred** (`SexprTests`): Unit tests covering `singleton` initialization, name `"block"`, keynum 4, `NewTok` handle preservation, `MakeBlock` list structure, and recursive declaration walking.
