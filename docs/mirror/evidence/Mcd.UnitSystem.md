# Mcd.UnitSystem

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Mcd/Units.cs`; tests: `Quire.Mpl.Tests.Mcd.UnitSystemTests`,
  `Quire.Differential.Tests.UnitSystemDifferentialTests`

A unit system (a `MarshalByRefObject`): a basis of units, the base dimensions they span, the matrices between
dimension coordinates ("global", one int per base dimension, 60000 per unit power) and basis-unit exponents
("local", doubles), default units keyed by dimension direction, and the set of unit symbols seen. Coordinates are
indexed by the basis's dimension names in sorted order. The nested private `Default` splits a dimension into a
direction and a scalar. Nothing is locked. Public members are differential-verified for the cases in "Test
cases"; `GetDimensionIndex`, `GetDimensions(string)`, `Inverse`, `IsSmall` and both `Count` overloads are also directly compared there. The nested `Default` methods also have direct comparisons; other private members are covered through public callers.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `.ctor(string, uint, string, Unit[], Unit[])` | 0x060005c1 | `d8b0faff` | Stores name, code and guid with options number 0, sets `Basis` then `Defaults` (so their exceptions surface here), then remembers both arrays as given for `Reset`. | High | Verified |
| `ToString()` | 0x060005c2 | `b8cf44cd` | The name. Same body as `Name`. | High | Verified |
| `Reset()` | 0x060005c3 | `36d9d4be` | Forgets all unit symbols, sets the construction-time basis and defaults again (padding the basis again), and returns this system. | High | Verified |
| `Name` | 0x060005c4 | `b8cf44cd` | The name. | High | Verified |
| `Code` | 0x060005c5 | `579faa70` | The code. | High | Verified |
| `Guid` | 0x060005c6 | `c4138c8c` | The guid text. | High | Verified |
| `Rank` | 0x060005c7 | `b9610d21` | The number of basis units after padding, not the number of distinct dimensions. | High | Verified |
| `Dimensions` | 0x060005c8 | `2bb62e07` | The base dimensions in coordinate order; the live array. | High | Verified |
| `Basis` (get) | 0x060005c9 | `51de32dd` | The basis array (the padded copy when padding happened). | High | Verified |
| `Basis` (set) | 0x060005ca | `8059552d` | A basis of fewer than eight units is copied into eight slots with an ampere (`Current`) in the last. Rank, basis, a new dimension array and a new forward matrix are stored before anything can fail. Each unit's symbol is recorded, the dimension names of all constant powers are collected and sorted, each unit's coordinates become a matrix column, and the matrix is inverted. An `ArgumentException` from that work (unknown dimension, "singular matrix") is rethrown as `ArgumentException` "not a basis" for parameter `value` with the original inside; other failures (null units, more distinct dimensions than slots) propagate as they are. | High | Verified |
| `Defaults` (get) | 0x060005cb | `6f332c89` | The default units, group by group in hashtable order, each group by descending order. The result buffer holds two units per direction, so more units than that in total throws `IndexOutOfRangeException`. | High | Verified |
| `Defaults` (set) | 0x060005cc | `f6752459` | A new table: each unit's symbol is recorded and it joins the group for its dimension's direction; each group is then sorted by descending order. A unit whose dimension isn't a base dimension throws `ArgumentException` "bad dimension"; null throws `NullReferenceException`. | High | Verified |
| `RO_number` (get) | 0x060005cd | `35013f12` | The runtime-options number. | High | Verified |
| `RO_number` (set) | 0x060005ce | `16bc1265` | Stores it; no validation. | High | Verified |
| `Simplify(string)` | 0x060005cf | `b44d6505` | Parses the signature, takes its dimension product, then simplifies its coordinates. Polymorphic variables are ignored; a term without a product throws `NullReferenceException`. | High | Verified |
| `Simplify(Type.Product)` | 0x060005d0 | `42bf5304` | Simplifies the product's coordinates. | High | Verified |
| `Simplify(int[])` | 0x060005d1 | `2d64da3d` | Looks up the defaults with the same direction. Of those with a nonzero scalar dividing the dimension's scalar, picks the first, then replaces it with one whose integer power is larger while the best is negative, or smaller and positive. A `Units` of the chosen unit at that power is built for every candidate that becomes best (so a zero-magnitude default raises "illegal factor"). Returns it unless the chosen unit's symbol is `NOSIMPLIFY`; otherwise, or with no candidate, `Normalize`. | High | Verified |
| `Normalize(string)` | 0x060005d2 | `d3048b4f` | As `Simplify(string)`, but normalizes. | High | Verified |
| `Normalize(Type.Product)` | 0x060005d3 | `0aa4c804` | Normalizes the product's coordinates. | High | Verified |
| `Normalize(int[])` | 0x060005d4 | `d2aec681` | The basis units for the coordinates: `GetUnits(ChangeBasis(global))`. | High | Verified |
| `GetCoordinates(Type.Product)` | 0x060005d5 | `1c39f58a` | A rank-length array summing each constant power's exponent at its dimension's index; variables ignored. An unknown dimension throws `ArgumentException` "bad dimension". | High | Verified |
| `GetUnits(double[])` | 0x060005d6 | `ec2f1131` | One power per nonzero local exponent, in basis order, each exponent converted to a `Rational` (error 1e-5), then `new Units(powers)` (which may raise "floating point error"). Nonzero entries past the rank throw. | High | Verified |
| `GetDimensions(int[])` | 0x060005d7 | `4236cca5` | A monomorphic product of the nonzero coordinates in coordinate order, bases being the dimension constants. | High | Verified |
| `ChangeBasis(int[])` | 0x060005d8 | `c41daabf` | Coordinates to local exponents through the inverse matrix. Reads exactly rank entries: shorter arrays throw `IndexOutOfRangeException`, extra entries are ignored. | High | Verified |
| `ChangeBasis(double[])` | 0x060005d9 | `824c5707` | Local exponents to coordinates through the forward matrix, each product term truncated to int before it is added. | High | Verified |
| `IsUnitSymbol(string)` | 0x060005da | `21e6973f` | Whether a basis or default unit set since construction or the last `Reset` had this symbol; setting new units never forgets old symbols. Null throws `ArgumentNullException`. | High | Verified |
| `GetDimensionIndex(Type)` | 0x060005db | `54b0899a` | Binary search of the sorted dimensions (name order of `Named.CompareTo`); a miss throws `ArgumentException` "bad dimension" naming the dimension. | High | Verified |
| `GetDimensions(string)` | 0x060005dc | `4fe25c2e` | Private static: the parsed signature's dimension product. | High | Verified |
| `Inverse(double[,])` | 0x060005dd | `5307792d` | Gauss–Jordan inversion by columns on a copy. A pivot within 1e-12 of zero first adds in the first later column whose entry in the pivot row isn't small; if the pivot is still small, throws `ArgumentException` "singular matrix". | High | Verified |
| `IsSmall(double)` | 0x060005de | `d39e96df` | Strictly between −1e-12 and 1e-12 (NaN is not small). | High | Verified |
| `Count(int[])` | 0x060005df | `5f37b750` | The number of nonzero entries. | High | Verified |
| `Count(double[])` | 0x060005e0 | `4f20f9f4` | The number of nonzero entries (NaN counts). | High | Verified |
| `Default.ctor(int[], Unit)` | 0x06000792 | `b5df5311` | As the one-argument constructor, then keeps the unit. | High; directly compared over the 24 selected coordinate vectors in `UnitSystem_Default_methods`; other vectors/units are not established. | Verified |
| `Default.ctor(int[])` | 0x06000793 | `14e2df4d` | Copies the coordinates. The scalar is the GCD of the nonzero ones, negated when the first nonzero one is negative; order is its magnitude. The copy is divided by the scalar unless the scalar is 0 or +1, so the direction's first nonzero coordinate is positive. | High; directly compared over 24 selected vectors spanning empty/zero, sign, GCD and multiple-rank cases; other vectors are not established. | Verified |
| `Default.GetHashCode()` | 0x06000794 | `97519dce` | Over the direction: rotate the hash left by 4 bits and xor in each coordinate's signed remainder mod 16. | High; directly compared for those same 24 selected vectors; other vectors are not established. | Verified |
| `Default.Equals(object)` | 0x06000795 | `8ab1de7f` | Same direction, compared over this one's length; the scalar and unit are ignored. Casts without checking. | High; directly compared for eight self, same/different-direction, and differing-coordinate cases, including one-element vectors; other vector lengths and null/wrong types are not established. | Verified |
| `Default.CompareTo(object)` | 0x06000796 | `01fc23b2` | Descending order (larger GCD first). | High; directly compared for larger, smaller and equal order cases; other objects are not established. | Verified |

**Choosing a default.** `Simplify` therefore prefers, among dividing defaults in one direction, the smallest
positive power, and among negative powers the one nearest zero; once a positive power is chosen a negative one
never replaces it. Groups are sorted by descending order, so ties go to the unit with the larger GCD. The
`NOSIMPLIFY` check is on the winner only: a direction whose best unit is `NOSIMPLIFY` always normalizes.

**Private members.** `GetDimensionIndex`, `GetDimensions(string)`, `Inverse`, `IsSmall` and both `Count`s are directly compared. `Default` constructors, hash, equality and ordering are directly compared in `UnitSystemDifferentialTests`; other private members are exercised through the public members that use them (construction, `Basis`, `Defaults`, `Simplify`, `Normalize`, `GetCoordinates`, `GetUnits`, `GetDimensions`).

Fields: all `UnitSystem` fields and `Default`'s are private state, Tested through the members that use them.

## Test cases

**Observed** (`UnitSystemDifferentialTests`, against the installed `mplrun.dll`; outcomes compared line by line as
text or exception type and message):
- construction of 11 systems: SI-style (8 base units with 12 derived defaults, among them kilonewton, liter,
  hectare, minute, a root-length unit, an odd `{Length 7 Time-3}` unit and a `NOSIMPLIFY` velocity), US-style (foot,
  pound, rankine), seven base units (padded), six without current, nine (adding information), a singular basis
  (two lengths), a composite basis (hertz in place of second), three forces in one direction, an unknown-dimension
  default, a zero-magnitude default, and an empty basis; plus Mathcad's built-in default SI system;
- for each system that constructs: `Name`, `Code`, `Guid`, `ToString`, `RO_number`, `Rank`, `Dimensions`, `Basis`,
  `Defaults`, `IsUnitSymbol` on nine symbols (empty included); for 300+ coordinate vectors (zero, each axis at ten
  steps from 7 to ±180000 including half and third powers, 300 seeded random combinations, and vectors shorter and
  longer than the rank) `Simplify`, `Normalize`, `ChangeBasis` both ways, `GetDimensions` and `GetUnits`; for 14
  signatures (dimensionless, polymorphic, unknown dimension, matrix, function, `F`/bare products, odd exponents)
  `Simplify` and `Normalize` by string and by product, and `GetCoordinates`;
- a setter script: replacing the defaults, `IsUnitSymbol` afterwards, simplifying a force, a US basis, a singular
  basis and the rank, basis and dimensions it leaves behind, `Reset` (returning the same object) and the basis,
  defaults and symbols after it, `RO_number`, and null basis and defaults.
- Direct private-helper cases: `GetDimensionIndex` for every known dimension and unknown dimensions; `GetDimensions(string)` for a dimensional and plain signature; 16 `Inverse` matrices covering ordinary, pivoting and singular inputs; `IsSmall` at zero, around both strict thresholds and NaN; both `Count` overloads on zero and nonzero vectors, including NaN for doubles. Separate direct cases compare `Default` constructors, hash, equality and ordering.
- `Simplify(int[])` on the SI basis with two defaults of the time direction whose scalars differ in sign or size
  (per-square-second with second, per-square-second with hertz, square-second with hertz) and with a dimensionless
  default beside second, for time exponents -6..6 (zero included): the chosen power changing sign, moving towards
  zero, staying, and a zero-scalar default being skipped.

**Unreachable branch outcome.** In `Simplify(int[])` a later candidate is never both positive and smaller than
the best: `Defaults` sorts each direction's list by descending `Order`, so later candidates have powers of equal or
larger magnitude. The `power > 0 && power < bestPower` true outcome needs a list mutated after sorting.

**Inferred** (`UnitSystemTests`): coordinates in sorted dimension-name order, simplification preferring a dividing
default, the smallest power winning among several defaults, basis change through a composite basis, padding of
short bases with an ampere, and `Reset` restoring the construction units in place.

## Open

- Dimension order follows the culture-sensitive `Named.CompareTo`; only the test process's culture was compared.
- `MarshalByRefObject` remoting and concurrent use (nothing is locked) were not exercised.
- `Defaults` enumerates a `Hashtable`, so its order depends on `Default.GetHashCode` and the table's growth; it
  matches for the systems above but isn't otherwise pinned down.
