# Mpl.Run.do_keyword

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Original inspection:** `ilspycmd` 11.1.0.9782 (`-t`), 2026-10-16
- **Quire:** `src/Quire.Mpl/Run/DoTryKeywords.cs`; tests: `Quire.Differential.Tests.DoTryKeywordDifferentialTests`

`do_keyword` is the `do` keyword with key number 11. Its expression walk calls `Sexpr.NotImplemented(form)` and retains the original trailing null return.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `singleton` | 0x0400004a | Static singleton instance. | High | Verified |
| `.cctor()` | 0x060000e5 | Initializes `singleton = new do_keyword()`. | High | Verified |
| `.ctor()` | 0x060000e6 | Calls `base("do", 11)`. | High | Verified |
| `NewTok(uint)` | 0x060000e7 | Returns `new Tok(singleton, handle)`. | High | Verified |
| `walk_expression_as(MplEnv, SCons)` | 0x060000e8 | Calls `Sexpr.NotImplemented(form)`, then returns null if the call returns. | High | Tested |

## Test cases

`DoTryKeywordDifferentialTests.Singleton_constructors_tokens_and_unimplemented_walks` directly compares the initializer, constructor, `NewTok` at three handles and the throwing walk.
