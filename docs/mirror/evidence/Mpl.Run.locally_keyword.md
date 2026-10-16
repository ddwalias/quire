# Mpl.Run.locally_keyword

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** `ilspycmd 11.1.0.9782`, original C# and IL saved temporarily under `/tmp/quire-a-locally-keyword-original.*`
- **Quire:** `src/Quire.Mpl/Run/locally_keyword.cs`; tests: `LocallyKeywordDifferentialTests` and `MplRunlocally_keywordMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.cctor` | 0x060000d0 | Creates the public singleton with `new locally_keyword()`. | High; directly reruns the type initializer and observes singleton state. | Verified |
| `.ctor()` | 0x060000d1 | Calls `Keyword("locally", 8)`. | High; existing mechanical direct comparison. | Verified |
| `NewTok(uint)` | 0x060000d2 | Returns a token whose atom is `singleton` and whose handle is the input. | High; handles 0, 1, 42 and `uint.MaxValue`. | Verified |
| `walk_expression_as(MplEnv,SCons)` | 0x060000d3 | Requires a three-element form. Walks its definitions as a block in a child environment, then walks the body there. Returns the body result if the block produced no declarations; otherwise returns `(locally <definitions> <body>)` at the form's handle. | Direct cases for an empty block and invalid arity. The declaration-producing branch depends on `define_keyword.MakeDefine`, still a stub on this base. | Tested |
| `MakeLocally(uint,Sexpr)` | 0x060000d4 | Returns `(locally forms)` with the input handle on the keyword token. | High; null and non-empty forms. | Verified |
| `MakeLocally(uint,Sexpr,Sexpr)` | 0x060000d5 | Builds a two-element forms list and delegates to the one-forms overload. | High; direct comparison. | Verified |
| `MakeLocally(uint,Cons,Sexpr)` | 0x060000d6 | Converts the `Cons` definitions to an S-expression list, appends the body, and delegates to the one-forms overload. | High; direct comparison. | Verified |

## Test cases

**Observed** against the installed Mathcad artifact: type initializer; constructor; `NewTok` at four handles;
all three factory overloads; `walk_expression_as` with an empty block and with a two-element malformed form.
`MethodComparison` compares returned values, arguments, and target state.

## Open

- The valid form with a declaration has not been compared yet. On this base, `define_keyword.MakeDefine` is a generated
  stub, so it cannot produce the non-null declarations list needed to exercise the wrapping branch. Add that direct case
  after the `define_keyword` dependency is available; the branch is reachable in Mathcad and cannot be proposed as
  unreachable.
