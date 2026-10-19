# Mpl.Run.SList

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Sexpr.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

A runtime `Cons` list in front of an S-expression tail.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1, Field2` | 0x0400001e, 0x0400001f | — | Public mutable `Cons` and tail. | High | Verified |
| `.ctor(object, object)` | 0x06000073 | `ce6adb89` | `as Cons`, `as Sexpr`, then the other constructor. | High | Verified |
| `.ctor(Cons, Sexpr)` | 0x06000074 | `55920cd6` | Tag 8. | High | Verified |
| `FormatCons(Cons)` | 0x06000075 | `d6b9fbb1` | Private: `" " + item` for each cell, following `Field2 as Cons` (a non-list tail ends it). | High | Verified |
| `ToString()` | 0x06000076 | `78dfdcc3` | `"(SLIST " + FormatCons(Field1) + ")"` when `Field2` is `Nul.singleton` itself, else `"(SLIST# " + FormatCons(Field1) + " " + Field2 + ")"`. Both have two spaces before the first item. | High | Verified |

## Test cases

`SexprDifferentialTests.SAppend_and_SList_members` directly compares `FormatCons` on null, a proper two-cell `Cons`
chain, and a `Cons` with a non-list tail. `SexprTests.Appends_and_spliced_lists_print_their_parts` checks proper lists,
the empty list, and that formatting stops at an improper tail.
