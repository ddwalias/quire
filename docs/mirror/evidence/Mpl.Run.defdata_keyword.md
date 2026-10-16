# Mpl.Run.defdata_keyword

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Original inspection:** `tools/reference/reference.py 'Mpl.Run.defdata_keyword' --il`, ilspycmd 11.1.0.9782, 2026-10-16; cached C#, member index and IL under `.mirror-cache/mplrun/15274355cb262d96/`
- **Quire:** `src/Quire.Mpl/Run/DefdataKeyword.cs`; tests: `Quire.Differential.Tests.DefdataKeywordDifferentialTests`

`defdata_keyword` is the `defdata` keyword with key number 3. The IL initializes a singleton, forwards token creation to `Tok(Atom,uint)`, wraps declaration forms in a one-cell `Cons` with null tail, and returns definition forms unchanged.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `singleton` | 0x0400003e | Static singleton instance. | High | Verified |
| `.cctor()` | 0x060000a0 | Initializes `singleton = new defdata_keyword()`. | High | Verified |
| `.ctor()` | 0x060000a1 | Calls `base("defdata", 3)`. | High | Verified |
| `NewTok(uint)` | 0x060000a2 | Returns `new Tok(singleton, handle)`. | High | Verified |
| `walk_declaration_as(MplEnv, SCons)` | 0x060000a3 | Returns `new Cons(form, null)`. | High | Verified |
| `walk_definition_as(MplEnv, SCons)` | 0x060000a4 | Returns `form`. | High | Verified |

## Test cases

`DefdataKeywordDifferentialTests.Singleton_tokens_and_declaration_walks` directly compares the type initializer, constructor, token factory at handles 0, 1, 37 and `uint.MaxValue`, both walks with a representative form, and both walks with a null form. The runtime comparison uses the installed artifact identified above.
