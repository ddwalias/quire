# Mpl.Run.VecBuilder

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Cons.cs`; tests: `Quire.Mpl.Tests.Run.MplRunHelpersTests`, `Quire.Differential.Tests.MplRunHelpersDifferentialTests`

Fills an object array from the front.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1, Field2` | 0x040000bc, 0x040000bd | — | Public mutable count and array. | High | Verified |
| `.ctor(int, object[])` | 0x060003f2 | `2e5d037c` | Stores both, unchecked. | High | Verified |
| `make(int)` | 0x060003f3 | `d1930694` | Count 0, a new array of n. | High | Verified |
| `vector, AsVector()` | 0x060003f4, 0x060003f7 | `bf6e3f2a` | The whole array (identical IL). | High | Verified |
| `cnt` | 0x060003f5 | `8c0667ba` | The count. | High | Verified |
| `AsList()` | 0x060003f6 | `4b930a3f` | A `Cons` list of the first `cnt` slots, null when 0; a count past the array throws `IndexOutOfRangeException`. | High | Verified |
| `Push1 … Push4` | 0x060003f8–0x060003fb | — | Store into the next slots one by one, post-incrementing the count, and return this. Past the end the store throws after the count has moved on. | High | Verified |

## Test cases

**Observed** (`MplRunHelpersDifferentialTests.VecBuilder_members`): sizes 0, 1, 3, 4, 10 with every push width twice, overflow included, and hand-built builders with short, negative and null arrays.
**Inferred** (`MplRunHelpersTests`).
