# Mpl.Run.defun_keyword

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection:** `tools/reference/reference.py 'Mpl.Run.defun_keyword' --il`, ilspycmd 11.1.0.9782; cached original C#, member index and IL under `.mirror-cache/mplrun/15274355cb262d96/`
- **Quire:** `src/Quire.Mpl/Run/DefunKeyword.cs`; direct comparisons: `Quire.Differential.Tests.DefunKeywordDifferentialTests`

`defun_keyword` is the `defun` keyword (key number 0). A declaration adds the declared symbol as a local equation at the form handle and returns a one-cell `Cons` containing the form. A definition requires a four-element form, walks its parameter list in a nested local scope and walks the body in that scope, then returns a normalized `defun` form with the walked patterns and expression. `MakeDefun` either attaches a prebuilt tail to a keyword token or builds that tail from the identifier, patterns and expression.

| Member | Original token | Contract | Status |
|---|---:|---|---|
| `singleton` | `0x04000042` | Static singleton. | Verified |
| `.cctor()` | `0x060000b6` | Initializes the singleton with a new `defun_keyword`. | Verified |
| `.ctor()` | `0x060000b7` | Calls `Keyword("defun", 0)`. | Verified |
| `NewTok(uint)` | `0x060000b8` | Returns a token containing the singleton and handle. | Verified |
| `walk_declaration_as(MplEnv, SCons)` | `0x060000b9` | Gets `form.second.AsSym`, calls `AddLocalEquation` with `form.handle`, and returns `new Cons(form, null)`. | Verified |
| `walk_definition_as(MplEnv, SCons)` | `0x060000ba` | Requires length 4; gets the identifier and pattern list, calls `walk_with_bindings` on the body, then calls the three-part factory. | Verified |
| `MakeDefun(uint, Sexpr)` | `0x060000bb` | Returns a new `SCons` with a singleton token at `handle` and `forms` as its tail. | Verified |
| `MakeDefun(uint, Sexpr, Sexpr, Sexpr)` | `0x060000bc` | Builds a three-element tail with `SCons.list3` and calls the two-argument overload. | Verified |

`DefunKeywordDifferentialTests.Singleton_factories_and_walks` compares the initializer, constructor, `NewTok` at handles 0, 13 and `uint.MaxValue`, both factories at handles 0, 19 and `uint.MaxValue`, a declaration that registers its symbol, and a definition with one bound parameter used in the body. The test constructs Mathcad keyword-headed forms explicitly because `MplRunSides.ToMathcad` does not convert keyword tokens.
