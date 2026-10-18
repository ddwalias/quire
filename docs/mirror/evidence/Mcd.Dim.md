# Mcd.Dim

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Mcd/Dim.cs`; tests: `Quire.Mpl.Tests.Mcd.DimTests`,
  `Quire.Differential.Tests.DimDifferentialTests`

A sealed class holding one int coordinate per base dimension of the unit system, 60000 per unit power, so
fractional powers stay integral. Dimensions built from coordinates are interned in a shared table keyed by the
private `Hash` comparer, so equal coordinates give the same object and most operations compare by reference.
`null` is the polymorphic dimension. `identity` is the interned all-zero dimension; `mcd_zero` (a zero that adapts to
any dimension), `string_dim`, `function_dim` and `mixed_matrix_dim` are markers: distinct, uninterned objects with
all-zero coordinates. The thread-static `s_vec` is the scratch vector that `Mult`, `Div` and `Pwr` fill before
interning. Every member below except the type initializer is differential-verified for the cases in "Test cases".

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `.cctor` | 0x060005ea | `8a9aa295` | Takes the rank from the initialising thread's unit system, creates the interning table, gives that thread a scratch vector, interns the all-zero `identity`, then creates the four markers. | High for the synced test thread; directly compared with rank, map count, scratch/marker coordinates, marker-array identity relationships and `identity` registration observed after invocation. | Verified |
| `ThreadInit()` | 0x060005eb | `6ac66288` | Gives the calling thread a rank-length scratch vector if it has none; a second call keeps the existing one. | High | Verified |
| `.ctor(int[])` | 0x060005ec | `3839c7fb` | Private: keeps the array (no copy) and registers itself in the table under it; an equal key already present throws `ArgumentException`. | High | Verified |
| `.ctor()` | 0x060005ed | `8704077c` | Private: a marker with a fresh zero vector, not registered. | High | Verified |
| `intern(int[])` | 0x060005ee | `c0163159` | The registered dimension for these coordinates, or a new one that keeps this very array. When the array is the thread's scratch vector, the thread first gets a fresh one. Arrays are compared over the rank only, so longer arrays match on their prefix and shorter ones throw `IndexOutOfRangeException`. | High | Verified |
| `intern()` | 0x060005ef | `074cafb8` | Interns the thread's scratch vector. | High | Verified |
| `Product` | 0x060005f0 | `bf88e0db` | The coordinates as a monomorphic product in the calling thread's unit system (markers and identity give an empty one). | High | Verified |
| `Coordinates` | 0x060005f1 | `9678b8a1` | The coordinate array itself, not a copy. | High | Verified |
| `Equals(object)` | 0x060005f2 | `76c901cb` | Any `Dim` with equal coordinates; so markers equal `identity` and each other. Non-`Dim` and null give false. | High | Verified |
| `GetHashCode()` | 0x060005f3 | `d4102943` | `Hash.GetHashCode` of the coordinates. | High | Verified |
| `ToString()` | 0x060005f4 | `ad485c62` | The product's signature text, e.g. `{Length 60000 Time -120000}`; `{}` for identity and markers. | High | Verified |
| `Format()` | 0x060005f5 | `10617956` | `String` for `string_dim`, `Function` for `function_dim`, otherwise the product's display form (`Unitless`, `Force`, `Length^2`, …). | High | Verified |
| `OnError(string, Dim, uint)` | 0x060005f6 | `fcbed9ce` | `OnError(m, identity, d, a)`. | High | Verified |
| `OnError(string, Dim, Dim, uint)` | 0x060005f7 | `1eba5010` | Raises an `AppException` whose message is the text, then a tab and each dimension's `Format` (polymorphic shown as identity), for argument `a`. The message goes through `AppException.Raise(string, uint)`, so a tag prefix is parsed and, with argument `uint.MaxValue`, an `@argument` suffix too. | High | Verified |
| `Parse(string)` | 0x060005f8 | `a80c239a` | `New` of the parsed signature's dimension product; parser exceptions propagate, and a term without a product throws `NonCriticalException` "ArgumentIsNull". | High | Verified |
| `New(Type.Product)` | 0x060005f9 | `478903fc` | Null throws `NonCriticalException` "ArgumentIsNull"; a polymorphic product gives null; a dimensionless one `identity`; otherwise the interned dimension of its coordinates (an unknown dimension throws `ArgumentException` "bad dimension"). | High | Verified |
| `intern(Dim)` | 0x060005fa | `1d86d154` | `mcd_zero` becomes `identity`; anything else as is. Same contract as `ValDim`. | High | Verified |
| `ValDim(Dim)` | 0x060005fb | `e3445f4a` | `mcd_zero` becomes `identity`; anything else as is. | High | Verified |
| `IsDimensioned(Dim)` | 0x060005fc | `f6cdf3bd` | A nonzero coordinate; false for null, identity and markers. | High | Verified |
| `Id_b(Dim)` | 0x060005fd | `00643333` | Identity, polymorphic or `mcd_zero` (by reference). | High | Verified |
| `Eq_b(Dim, Dim)` | 0x060005fe | `ada2543a` | Same object, or either is polymorphic or `mcd_zero`. | High | Verified |
| `Eq_b1(Dim, Dim)` | 0x060005ff | `977cbf07` | Same object, or either is polymorphic. | High | Verified |
| `NonScalar_b(Dim)` | 0x06000600 | `5edd6dc0` | `function_dim`, `string_dim` or `mixed_matrix_dim`. | High | Verified |
| `IsSpecial(Dim)` | 0x06000601 | `7dc17316` | `mcd_zero`, polymorphic, or a non-scalar marker. | High | Verified |
| `Id(Dim)` | 0x06000602 | `e6b5826d` | `Id(x, 1)`. | High | Verified |
| `Id(Dim, uint)` | 0x06000603 | `a7188494` | `identity` when `Id_b` holds; otherwise raises "bad_dimensions_unitless" with x for argument `a`. | High | Verified |
| `Eq(Dim, Dim)` | 0x06000604 | `653ae725` | `Eq(x, y, 2)`. | High | Verified |
| `Eq(Dim, Dim, uint)` | 0x06000605 | `e4604390` | y when x is `mcd_zero`; x when y is `mcd_zero`; x when they are the same object or y is polymorphic; y when x is polymorphic; otherwise raises "bad_dimensions" with both for the argument. | High | Verified |
| `FixDimension(Dim)` | 0x06000606 | `829ec657` | Polymorphic becomes `identity`; anything else as is. | High | Verified |
| `Mult(Dim, Dim)` | 0x06000607 | `23897a15` | When x is identity or y polymorphic, y (with `mcd_zero` turned into identity); when y is identity or x polymorphic, x likewise. Otherwise the interned sum of the coordinates, markers counting as zero (so `mcd_zero · mcd_zero` is identity). Uses the thread's scratch vector: an unsynced thread throws `NullReferenceException` there. | High | Verified |
| `Div(Dim, Dim)` | 0x06000608 | `5ad41eb2` | x when y is identity or x polymorphic; polymorphic when y is; identity when they are the same object; otherwise the interned difference, markers counting as zero. | High | Verified |
| `Pwr(Dim, double)` | 0x06000609 | `04a76565` | x itself for identity, polymorphic or an exponent of exactly 1; identity for 0 (−0 included); otherwise each coordinate times e rounded half to even and converted to int, NaN to 0 and out-of-range values saturating, then interned. Inexact powers aren't detected; markers come back as identity. | High; relies on the runtime's saturating conversion, which Quire shares | Verified |
| `Root(Dim, double)` | 0x0600060a | `f0885f9f` | `Pwr(x, 1 / n)`, so n = 0 is a power of +∞. | High | Verified |
| `Hash.Equals(object, object)` | 0x06000797 | `4a3e0a59` | Casts both to `int[]` (`InvalidCastException` otherwise) and compares them. | High | Verified |
| `Hash.GetHashCode(object)` | 0x06000798 | `1ed5896b` | Casts to `int[]` and hashes it. | High | Verified |
| `Hash.Equals(int[], int[])` | 0x06000799 | `af6ca910` | Equal over the first rank entries; shorter arrays throw `IndexOutOfRangeException`, entries past the rank are ignored. | High | Verified |
| `Hash.GetHashCode(int[])` | 0x0600079a | `94ce8c15` | Xor over the first rank entries of each whole unit power (truncated division by 60000, signed remainder mod 8) shifted into a 4-bit field by its index mod 8. Null throws `NullReferenceException`. | High | Verified |
| `Hash.ctor()` | 0x0600079b | `41ad57d1` | No state. | High | Verified |

**Coordinates.** The rank is fixed when the type initialises, from the initialising thread's unit system; with
Mathcad's SI that is 8, Money included, and coordinates are in sorted dimension-name order: Current, Length,
Luminosity, Mass, Money, Substance, Temperature, Time.

**Changes from the earlier Quire `Dim`** (pre-§3c): `ToString` now prints the signature-syntax product
(`{Length 60000 Time -120000}`) as Mathcad does, and `NumVal` text built on it changed with it (for example
`3-4i [{Length 60000}]`). The Quire-only extras `Denominator`, `BaseNames`, `Symbols`, `Combine` and `Require` were
removed.

Fields: the five markers, `polymorphic` and the thread scratch `s_vec` are Verified (compared directly by `DimDifferentialTests`); `s_rank`, `s_map` and `m_vec` are private state, Tested through the members that use them.

## Test cases

**Observed** (`DimDifferentialTests`, against the installed `mplrun.dll`; dimensions shown as a marker's name or
coordinates plus whether they are the interned object, exceptions as type and message):
- operands: polymorphic null, the five markers and 15 parsed dimensions (single and compound SI dimensions, half
  and third powers, `{Length 20000 Time-7}`, money over current, `{Length 1}` and ±int.MaxValue lengths);
- on each operand: `IsDimensioned`, `Id_b`, `NonScalar_b`, `IsSpecial`, `ValDim`, `FixDimension`, `intern(Dim)`,
  `Id` with and without an argument; and for non-null ones `Format`, `ToString`, `Product`, `Coordinates` (and
  that it is the live array), `GetHashCode`, `Equals(null)`, `Equals("x")`, `Pwr` at 14 exponents (0, 1, 2, −1, ½,
  ⅓, −1.5, π, ±1e10, NaN, +∞, −0, 2.5e-5) and `Root` at 8 (1, 2, 3, −2, 0, ½, NaN, 7); plus `Pwr` and `Root` of null;
- on every ordered pair of operands: `Eq_b`, `Eq_b1`, `Mult`, `Div`, `Eq` with and without an argument, the
  four-argument `OnError` and `Equals`; and the three-argument `OnError` on each operand with argument 4 and with a
  tagged message `12>m@3` and argument `uint.MaxValue`;
- `Parse` and `New` of each of the 98 signatures of `TypeDifferentialTests`, and `New(null)`;
- the type initializer, directly invoked on both artifacts after thread sync; afterward rank, interning map count, scratch and marker coordinates, marker-array identity relationships and `identity` registration are compared;
- the private constructors: a fresh vector's resulting dimension keeps the exact supplied array; an equal zero vector
  throws `ArgumentException`; the parameterless marker has fresh zero coordinates and is not interned;
- the private interning: a new vector is kept, an equal vector finds the first, interning the scratch vector keeps
  it and hands the thread a fresh zeroed one, and vectors longer and shorter than the rank;
- `Hash` on 404 vectors (empty, ranks 7 to 9, and 400 seeded random ones mixing zero, whole unit powers, mid-range
  and full-range ints): both `GetHashCode` overloads, `Equals(int[], int[])` against a changed copy and a zero vector against one whose first coordinate is 1,
  `Equals(object, object)` against an equal copy, a non-array argument and a null vector;
- on a new thread: the scratch vector and unit system before sync, `Mult` with identity and with two markers before
  `ThreadInit`, then `ThreadInit` twice (keeping the vector), `Mult` and `Parse` afterwards;
- `intern(int[])` of a new vector that is the thread's scratch vector (the result must not keep sharing it),
  `IsDimensioned` of a dimension whose only nonzero coordinate is the first, and `Pwr(Length, 0)` on a thread with
  no scratch vector (identity, without touching it).

**Unreachable statements:** `Id(Dim, uint)` and `Eq(Dim, Dim, uint)` end in `return null` after `OnError`, which
always throws; the original has the same `call void OnError; ldnull; ret` tail. Coverage reports those lines as unexecuted; no
input reaches them.

**Inferred** (`DimTests`): equal coordinates giving the same object, interning handing the thread a new scratch
vector, markers as distinct zero vectors equal to identity, hash packing of truncated unit powers, products and
quotients of special dimensions, round-half-to-even powers without complaint, predicates on special dimensions,
tab-separated mismatch errors, polymorphic and null products, and other threads needing their own scratch vector.

## Open

- The interning table is a shared, unlocked `Hashtable`, as in the original; concurrent interning from several
  threads is not thread-safe and was not tested.
- The rank is taken once, from whichever thread initialises the type; initialising under a unit system whose rank
  isn't 8 was not exercised.
- The type initializer remains Tested; constructor cases cover the recorded fresh, duplicate and marker scenarios only.
