# Mpl.Run.case_keyword

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection:** `ilspycmd -il -t Mpl.Run.case_keyword`; reference IL is in `/tmp/a-case-keyword-il.txt` (scratch only).
- **Quire:** `src/Quire.Mpl/Run/case_keyword.cs`; differential cases: `CaseKeywordDifferentialTests.Singleton_constructor_tokens_and_unimplemented_walk`.

The `case` keyword has key number 10 and a public singleton.

| Member | Token | Contract | Status |
|---|---|---|---|
| `singleton` | 0x04000048 | Static instance initialized by `.cctor()`. | Verified |
| `.cctor()` | 0x060000dd | `singleton = new case_keyword()`. | Verified |
| `.ctor()` | 0x060000de | Calls `Keyword("case", 10)`. | Verified |
| `NewTok(uint)` | 0x060000df | Returns `new Tok(singleton, handle)`. | Verified |
| `walk_expression_as(MplEnv, SCons)` | 0x060000e0 | Calls `Sexpr.NotImplemented(form)`, followed by the unreachable `return null`. | Tested |

## Differential cases

- Direct `MethodComparison.Compare` calls for the type initializer, constructor, and `NewTok` with handles 0, 17 and `uint.MaxValue`.
- `walk_expression_as` receives a regular symbol-headed form and compares the thrown result on Quire and Mathcad. The environment is null because the original body does not read it before throwing.
- Explicit renderers capture the singleton, constructor, token and form state.

The original `walk_expression_as` IL is `ldarg.2; call Sexpr.NotImplemented; ldnull; ret`; the trailing return is unreachable because `NotImplemented` always throws.
