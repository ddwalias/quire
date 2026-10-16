# Mpl.Run.MplFunctionMacro

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** inspected original C# and Release IL with ilspycmd before reconstruction.
- **Quire:** `src/Quire.Mpl`; direct tests: `MplFunctionMacroDifferentialTests`; existing member comparisons: `MplRunMplFunctionMacroMechanicalDifferentialTests`.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(Sym, Function)` | 0x06000136 | Store `expander` after initializing the base macro with `name` | Mechanical; exact Release IL | Verified |
| `.ctor(string, Function)` | 0x06000137 | Delegate to `.ctor(new Sym(name, null), expander)` | Direct comparisons for ordinary, empty and null names | Verified |
| `make(string, Function)` | 0x06000138 | Return `new MplFunctionMacro(name, expander)` | Direct comparisons for ordinary, empty and null names | Verified |
| `ExpandCall(MplEnv, Sexpr)` | 0x06000139 | Call `m_expander.Call(e, form)`, cast to `Sexpr`; wrap `AppException` as `AppException("illegal macro call", inner, form.handle)` | Direct comparisons for a successful identity expander and a directly thrown `AppException`; exceptions and target state compare | Verified |
| `ExpandRef(MplEnv, Tok)` | 0x0600013a | Call `base.IllegalMacroCall(s)` | Mechanical; exact Release IL | Verified |

## Test cases

`MplFunctionMacroDifferentialTests.String_constructor_and_factory` directly compares both newly implemented members for three names with a null expander. `ExpandCall_returns_the_result_and_wraps_AppException` directly compares a successful two-argument identity expander and wrapping an `AppException` thrown directly by the expander. Instance-state observation includes the macro name and expander purity, minimum arity and varargs setting; the callback result or exception is compared by `MethodComparison`.

The two previously implemented members remain directly compared by `MplRunMplFunctionMacroMechanicalDifferentialTests` on generated inputs.
