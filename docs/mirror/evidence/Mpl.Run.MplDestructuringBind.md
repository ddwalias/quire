# Mpl.Run.MplDestructuringBind

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Quire:** `src/Quire.Mpl/Run/MplDestructuringBind.AsPattern.cs`; tests: `Quire.Differential.Tests.MplDestructuringBindDifferentialTests`

Converts destructuring forms to pattern lists.

| Member | Token | Contract | Evidence and limits | Status |
|---|---|---|---|---|
| `as_pattern(Sexpr)` | 0x06000144 | For a cons whose head is a `Ca`, returns the unquoted head value. Otherwise recursively converts cons elements and returns `SCons.list3(SCons.token, head, tail)`. For `Nul`, returns `quote`; for `Cm`, returns its wrapped value; other expressions are returned unchanged. | Original C# and IL; direct cases cover every tag branch, recursion, null input and the splice-head path. | Verified |

## Test cases

- **Observed** (`MplDestructuringBindDifferentialTests`): direct calls against Mathcad for an empty list, `Cm`, `Ca`, a recursive list containing a splice, an ordinary recursive list, an unchanged `Qt`, and null (matching `NullReferenceException`).
