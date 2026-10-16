# Mpl.Run.lambda_keyword

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Original inspection:** `tools/reference/reference.py 'Mpl.Run.lambda_keyword' --il`, ilspycmd 11.1.0.9782, 2026-10-17; cached C#, member index and IL under `.mirror-cache/mplrun/15274355cb262d96/`
- **Quire:** `src/Quire.Mpl/Run/LambdaKeyword.cs`; tests: `Quire.Differential.Tests.LambdaKeywordDifferentialTests`

The `lambda` keyword has key number 6. Its expression walker evaluates the body in a fresh local scope populated by the walked patterns, then constructs the lambda using the original form handle. The two factories create a lambda from an existing tail or from a pattern/body pair.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `singleton` | 0x04000044 | Static singleton instance. | High | Verified |
| `.cctor()` | 0x060000c4 | Initializes `singleton = new lambda_keyword()`. | High | Verified |
| `.ctor()` | 0x060000c5 | Calls `base("lambda", 6)`. | High | Verified |
| `NewTok(uint)` | 0x060000c6 | Returns `new Tok(singleton, handle)`. | High | Verified |
| `walk_expression_as(MplEnv, SCons)` | 0x060000c7 | Calls `form.third.walk_with_bindings(env, form.second.scons, out xpats)`, then `MakeLambda(form.handle, xpats, body)`. | High | Verified |
| `MakeLambda(uint, Sexpr)` | 0x060000c8 | Returns a new cons of `Tok(singleton, handle)` and `forms`. | High | Verified |
| `MakeLambda(uint, Sexpr, Sexpr)` | 0x060000c9 | Calls the two-argument overload with `SCons.list2(pats, body)`. | High | Verified |

## Test cases

`LambdaKeywordDifferentialTests.Singleton_factories_and_walk` directly compares initialization, construction, `NewTok` at handles 0, 6 and `uint.MaxValue`, both factory overloads at those handles, and walking `(lambda (x) x)` so the body is resolved within the pattern-bound local scope.
