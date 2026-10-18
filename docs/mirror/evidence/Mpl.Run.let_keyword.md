# Mpl.Run.let_keyword

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.MplRunlet_keywordMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `singleton` | — | Public static singleton instance, initialized with `new let_keyword()`. | High | Verified |
| `.cctor()` | 0x060000d7 | Constructs and stores `singleton`. | High; Release body is `newobj`, `stsfld`, `ret` | Verified |
| `.ctor()` | 0x060000d8 | MemberGen: `: base("let", 7) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `NewTok(uint)` | 0x060000d9 | Returns `new Tok(singleton, handle)`. | High; Release body is `ldsfld`, `ldarg.0`, `newobj`, `ret` | Verified |
| `ExpandDefs(uint, SCons)` | 0x060000da | Recursively turns let bindings into definitions; calls `define_keyword.MakeDefine` and `lambda_keyword.MakeLambda`. | High; direct branches remain blocked by those unmerged dependencies | Stub |
| `ExpandLet(MplEnv, SCons)` | 0x060000db | Expands definitions, then calls `locally_keyword.MakeLocally` with the form's body. | High; blocked while `ExpandDefs` dependencies are unmerged | Stub |
| `walk_expression_as(MplEnv, SCons)` | 0x060000dc | Calls `ExpandLet(env, form).walk_expression(env)`. | High; blocked by `ExpandLet` | Stub |

## Test cases

**Observed** (`SimpleKeywordDifferentialTests`): directly compares the type initializer, constructor, singleton state,
and `NewTok` for handles 0, 1, 37 and `uint.MaxValue` against the installed Mathcad artifact.

**Observed** (`MplRunlet_keywordMechanicalDifferentialTests`): constructor comparison on generated inputs, comparing
the result, arguments and target fields with Mathcad's.

## Open

`ExpandDefs`, `ExpandLet` and `walk_expression_as` remain stubs until `define_keyword.MakeDefine` and
`lambda_keyword.MakeLambda` merge; `ExpandLet` also requires the already-merged `locally_keyword.MakeLocally`.
