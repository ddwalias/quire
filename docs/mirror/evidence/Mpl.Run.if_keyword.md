# Mpl.Run.if_keyword

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Original inspection:** `tools/reference/reference.py 'Mpl.Run.if_keyword' --il`, ilspycmd 11.1.0.9782, 2026-10-17; cached C#, member index and IL under `.mirror-cache/mplrun/15274355cb262d96/`
- **Quire:** `src/Quire.Mpl/Run/IfKeyword.cs`; tests: `Quire.Differential.Tests.IfKeywordDifferentialTests`

`if_keyword` is the `if` keyword with key number 9. Its expression walk recursively walks the form tail and returns a new `SCons` with the original head and walked tail. The two `MakeIf` overloads create an `if` form directly or collect the condition, then branch and else branch into a three-cell list.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `singleton` | 0x04000045 | Static singleton instance. | High | Verified |
| `.cctor()` | 0x060000ca | Initializes `singleton = new if_keyword()`. | High | Verified |
| `.ctor()` | 0x060000cb | Calls `base("if", 9)`. | High | Verified |
| `NewTok(uint)` | 0x060000cc | Returns `new Tok(singleton, handle)`. | High | Verified |
| `walk_expression_as(MplEnv, SCons)` | 0x060000cd | Walks `form.tail` with `SCons.walk_expressions(env, ...)`, then returns `new SCons(form.head, rest)`. | High | Verified |
| `MakeIf(uint, Sexpr)` | 0x060000ce | Returns `new SCons(new Tok(singleton, handle), forms)`. | High | Verified |
| `MakeIf(uint, Sexpr, Sexpr, Sexpr)` | 0x060000cf | Calls the two-argument overload with `SCons.list3(cond_expr, then_expr, else_expr)`. | High | Verified |

## Test cases

`IfKeywordDifferentialTests.Singleton_factories_and_walk` directly compares initialization, construction, `NewTok` at handles 0, 9 and `uint.MaxValue`, both `MakeIf` overloads at those handles, and expression walking over a form with literal condition and then expressions. The walk observer distinguishes returning the original input form from constructing a new form.
