# Mpl.Run.List_keyword

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Original Inspection record:** `ilspycmd` 11.1.0.9782 (`-t` and `-il`), 2026-10-16; temporary output `/tmp/a-list-keyword.cs` and `/tmp/a-list-il.txt`
- **Quire:** `src/Quire.Mpl/Run/ListAndTupleKeywords.cs`; tests: `Quire.Differential.Tests.SimpleKeywordDifferentialTests`

`List_keyword` is the `List` keyword with key number 13.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `singleton` | 0x0400004b | Static singleton instance. | High | Verified |
| `.cctor()` | 0x060000e9 | Initializes `singleton = new List_keyword()`. | High | Verified |
| `.ctor()` | 0x060000ea | Calls `base("List", 13)`. | High | Verified |
| `NewTok(uint)` | 0x060000eb | Returns `new Tok(singleton, handle)`. | High | Verified |

## Test cases

`SimpleKeywordDifferentialTests.Singleton_constructors_and_tokens` directly compares the type initializer, constructor,
and `NewTok` at handles 0, 1, 37 and `uint.MaxValue`. It also checks the singleton's key number.
