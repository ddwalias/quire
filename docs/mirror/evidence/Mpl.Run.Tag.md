# Mpl.Run.Tag

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Sexpr.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

The common base of S-expressions and atoms: a `DimVal` with an int tag naming the subclass.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `m_tag` | 0x040000be | — | Public mutable int. | High | Verified |
| `.ctor(int)` | 0x060003fc | `3cfe71f9` | Stores the tag; the `DimVal` base has no state of its own here. | High | Verified |

## Test cases

Observed: `SexprDifferentialTests.Constructors_and_factories` (tags 9 and −1) and `m_tag` on every tree.
