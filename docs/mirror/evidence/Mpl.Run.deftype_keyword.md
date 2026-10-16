# Mpl.Run.deftype_keyword

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Original inspection:** `tools/reference/reference.py 'Mpl.Run.deftype_keyword' --il`, ilspycmd 11.1.0.9782, 2026-10-17; cached C#, member index and IL under `.mirror-cache/mplrun/15274355cb262d96/`
- **Quire:** `src/Quire.Mpl/Run/DeftypeKeyword.cs`; tests: `Quire.Differential.Tests.DeftypeKeywordDifferentialTests`

`deftype_keyword` is the `deftype` keyword with key number 2. Declaration walking wraps the supplied form in a one-cell `Cons`; definition walking returns the same form instance unchanged.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `singleton` | 0x0400003f | Static singleton instance. | High | Verified |
| `.cctor()` | 0x060000a5 | Initializes `singleton = new deftype_keyword()`. | High | Verified |
| `.ctor()` | 0x060000a6 | Calls `base("deftype", 2)`. | High | Verified |
| `NewTok(uint)` | 0x060000a7 | Returns `new Tok(singleton, handle)`. | High | Verified |
| `walk_declaration_as(MplEnv, SCons)` | 0x060000a8 | Returns `new Cons(form, null)`. | High | Verified |
| `walk_definition_as(MplEnv, SCons)` | 0x060000a9 | Returns `form` unchanged. | High | Verified |

## Test cases

`DeftypeKeywordDifferentialTests.Singleton_factory_and_walks` directly compares type initialization, construction, token handles 0, 2 and `uint.MaxValue`, the declaration wrapper and its reference to the original form, and definition identity reuse.
