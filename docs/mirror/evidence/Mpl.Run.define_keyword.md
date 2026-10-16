# Mpl.Run.define_keyword

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Original inspection:** `tools/reference/reference.py 'Mpl.Run.define_keyword' --il`, ilspycmd 11.1.0.9782, 2026-10-16; cached C#, member index and IL under `.mirror-cache/mplrun/15274355cb262d96/`
- **Quire:** `src/Quire.Mpl/Run/DefineKeyword.cs`; tests: `Quire.Differential.Tests.DefineKeywordDifferentialTests`

`define_keyword` is the `define` keyword with key number 1. Its declaration walk requires a three-cell form, walks the pattern in cell two, and wraps either the original or rebuilt form in a one-cell `Cons`. Its definition walk processes cell three and keeps the original form when that expression is unchanged. The two `MakeDefine` overloads construct `(define forms)` directly or by collecting the pattern and definition into a two-cell form.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `singleton` | 0x04000041 | Static singleton instance. | High | Verified |
| `.cctor()` | 0x060000af | Initializes `singleton = new define_keyword()`. | High | Verified |
| `.ctor()` | 0x060000b0 | Calls `base("define", 1)`. | High | Verified |
| `NewTok(uint)` | 0x060000b1 | Returns `new Tok(singleton, handle)`. | High | Verified |
| `walk_declaration_as(MplEnv, SCons)` | 0x060000b2 | Calls `Sexpr.BadDeclaration(form)` unless length is 3; walks `form.second` as a pattern and returns a one-cell declaration list, rebuilding the form only when the pattern changed. | High | Verified |
| `walk_definition_as(MplEnv, SCons)` | 0x060000b3 | Walks `form.third` as an expression and rebuilds the form only when the expression changed. | High | Verified |
| `MakeDefine(uint, Sexpr)` | 0x060000b4 | Returns `new SCons(new Tok(singleton, handle), forms)`. | High | Verified |
| `MakeDefine(uint, Sexpr, Sexpr)` | 0x060000b5 | Calls the two-argument overload with `SCons.list2(pat, def)`. | High | Verified |

## Test cases

`DefineKeywordDifferentialTests.Singleton_factories_and_walks` directly compares type initialization, construction, token handles 0, 17 and `uint.MaxValue`, both factory overloads at those handles, declaration walking with an unchanged pattern, a rebuilt pattern and an invalid-length form, and definition walking with unchanged and transformed expressions. The unchanged declaration observer checks that the returned wrapper contains the original form instance, and the unchanged definition observer checks that the form itself is returned by identity.
