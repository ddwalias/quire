# Mpl.Run.Cons

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Cons.cs`; tests: `Quire.Mpl.Tests.Run.MplRunHelpersTests`, `Quire.Differential.Tests.MplRunHelpersDifferentialTests`

A runtime list of objects, ended by null.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1, Field2` | 0x040000ba, 0x040000bb | — | Public mutable item and rest. | High | Verified |
| `.ctor(object, object)` | 0x060003eb | `ce7bf576` | Stores both. | High | Verified |
| `head` | 0x060003ec | `62cc2629` | `Field1`. | High | Verified |
| `tail` | 0x060003ed | `85e073b7` | `Field2 as Cons`. | High | Verified |
| `Length(Cons)` | 0x060003ee | `1ad36226` | Cells reached through `tail`; 0 for null; a non-list tail ends the count. | High | Verified |
| `FormatItems()` | 0x060003ef | `b657978a` | Private: the item, then `", "` and the rest while `Field2` isn't null. A non-list `Field2` or a null item throws `NullReferenceException`. | High | Verified |
| `ToString()` | 0x060003f0 | `22af0120` | `"[" + FormatItems() + "]"`. | High | Verified |
| `destructive_append(Cons, Cons)` | 0x060003f1 | `cca6cb55` | null `xs` gives `ys`; otherwise walks to the cell whose `Field2` is null, links `ys` there and returns `xs`. A non-list `Field2` on the way throws. | High | Verified |

## Test cases

**Observed** (`MplRunHelpersDifferentialTests.Cons_members`): 8 lists (null, proper, nested, improper, null items) for every member, and `destructive_append` on all 64 pairs of fresh copies, comparing the result and the mutated first list.
**Inferred** (`MplRunHelpersTests`): printing, counting, appending.
