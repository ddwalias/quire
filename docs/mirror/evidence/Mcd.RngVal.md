# Mcd.RngVal and iteration prerequisites

- **Original:** Mathcad Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (`inspect_managed_members`), same-artifact managed metadata, instruction anchors, call edges and body hashes. Decompiled view: retained ILSpy files `mplrun/Mcd/RngVal.cs`, `RngValInterpolate.cs`, `Iterator.cs`, `RangeIterator.cs` in the preceding implementation's external scratchpad. No proprietary bodies are retained here.
- **Quire:** `src/Quire.Mpl/Mcd/RngVal.cs`, `src/Quire.Mpl/Mcd/Iterator.cs`; existing differential suite: `RngValDifferentialTests`.

This record restores the missing evidence reference without changing behavior or classifications. **Verified (existing annotation)** below reports the inherited source/status classification and named test suite; it does not assert a new execution result. The focused rerun is pending at record creation. Analyzed matrix conversions and matrix iteration have static evidence only here. Managed numeric conversion behavior, especially NaN/infinity/out-of-range double-to-int results, requires runtime corroboration on the comparison host.

Signatures retain REA's exact decoded metadata types (`class:<coded-index>`); tokens and hashes are meaningful only with the artifact identity above. Nested iterator names are unqualified in REA's retained inventory; their type tokens bind them to `Mcd.Iterator`'s same-artifact nested declarations.

## Mcd.RngVal (`0x02000095`)

| Member/signature | Token | Normalized IL prefix | Contract | Current classification / limits |
|---|---|---|---|---|
| `m_init : r8` | 0x0400014d | field | Initial numeric value. | Verified (existing annotation); rerun pending |
| `m_incr : r8` | 0x0400014e | field | Numeric increment. | Verified (existing annotation); rerun pending |
| `m_n : i4` | 0x0400014f | field | Unchecked stored element count. | Verified (existing annotation); rerun pending |
| `m_integral : bool` | 0x04000150 | field | Stored integral-start/increment flag. | Verified (existing annotation); rerun pending |
| `.ctor(r8,r8,i4,class:568) -> void` | 0x060006d9 | 5c956ce72fe39556 | Four-argument constructor stores initial value, increment, unchecked count and dimension; integral flag compares initial value and increment to their int conversions. Copy constructor copies all range fields and dimension without preserving subclass interpolation. | Verified (existing annotation); rerun pending |
| `.ctor(class:596) -> void` | 0x060006da | c290b8210832a00b | Four-argument constructor stores initial value, increment, unchecked count and dimension; integral flag compares initial value and increment to their int conversions. Copy constructor copies all range fields and dimension without preserving subclass interpolation. | Verified (existing annotation); rerun pending |
| `asResult() -> class:584` | 0x060006db | 57b5b50622666d54 | Return a new plain RngVal copied from this instance; interpolation subclass is not retained. | Verified (existing annotation); rerun pending |
| `get_Integral() -> bool` | 0x060006dc | 7870d0ce49e40a9f | Read stored integral flag. | Verified (existing annotation); rerun pending |
| `get_PhysicalDimension() -> class:568` | 0x060006dd | 2d540f09d3ac3eaf | Return identity when polymorphic, otherwise stored dimension. | Verified (existing annotation); rerun pending |
| `Equals(object) -> bool` | 0x060006de | 4ab4b980a845ee1a | Require other object to be a range with Dim.Eq_b-compatible dimensions and exactly equal initial value, increment and count. NaN comparisons remain false. | Verified (existing annotation); rerun pending |
| `GetHashCode() -> i4` | 0x060006df | 9414bec899dfe5e6 | Combine initial/increment double hashes with stored dimension hash plus count by XOR. Null dimension naturally throws. | Verified (existing annotation); rerun pending |
| `get_Rsize() -> i4` | 0x060006e0 | 65963bff88719d17 | Return stored count, including zero or negative counts. | Verified (existing annotation); rerun pending |
| `get_Isize() -> i4` | 0x060006e1 | 65963bff88719d17 | Return stored count, including zero or negative counts. | Verified (existing annotation); rerun pending |
| `get(i4) -> r8` | 0x060006e2 | 580322101641261f | Return initial value advanced by index times increment. No bounds check. | Verified (existing annotation); rerun pending |
| `range_get(i4) -> class:584` | 0x060006e3 | 260afd2d6a98653c | Return fresh real NumVal for initial plus index times increment with stored dimension. No bounds check; plain implementation does not dispatch through virtual get. | Verified (existing annotation); rerun pending |
| `iterator_get(i4) -> class:584` | 0x060006e4 | 260afd2d6a98653c | Return fresh real NumVal for initial plus index times increment with stored dimension. No bounds check; plain implementation does not dispatch through virtual get. | Verified (existing annotation); rerun pending |
| `mkVec2(class:584,class:584) -> class:596` | 0x060006e5 | bc57908e1ff8783a | Check both operands as real at argument positions 1/2, then dimensionless with the three-argument-units error text. Construct identity-dimension unit-step range in endpoint direction using the sequential-values error text. | Verified (existing annotation); rerun pending |
| `mkVec3(class:584,class:584,class:584) -> class:596` | 0x060006e6 | 84ec1f741a9f131b | Check three operands as real at arguments 1/2/3, reconcile initial/next dimensions then endpoint at argument 3. Use next minus initial as increment and sequential-values error text. | Verified (existing annotation); rerun pending |
| `mkRange2(class:584,class:584) -> class:596` | 0x060006e7 | 5d3b97983d5eb361 | Check both operands as real at arguments 1/2 then dimensionless with bad_range_dimensionless_argument; construct endpoint-direction unit-step range. | Verified (existing annotation); rerun pending |
| `mkRange3(class:584,class:584,class:584) -> class:596` | 0x060006e8 | 9269ab5444cefc2b | Check operands as real at arguments 1/2/3, reconcile dimensions and use next value minus initial as step. | Verified (existing annotation); rerun pending |
| `Range(r8,r8) -> class:596` | 0x060006e9 | ec1895f226ec936b | Two-number overload chooses increment +1 when endpoint is at least initial, otherwise -1, with identity dimension. Four-argument overload uses second minus first and supplied dimension. | Verified (existing annotation); rerun pending |
| `Range(r8,r8,r8,class:568) -> class:596` | 0x060006ea | cd54b49aca3b261f | Two-number overload chooses increment +1 when endpoint is at least initial, otherwise -1, with identity dimension. Four-argument overload uses second minus first and supplied dimension. | Verified (existing annotation); rerun pending |
| `IncrRange(r8,r8,r8,class:568,string) -> class:596` | 0x060006eb | 6e54842bad0729c5 | Endpoint overload computes int-converted step count from span/increment enlarged by factor 1.00000000001. Zero increment, NaN operands or negative nonsentinel count raise supplied error; int extrema raise range too many elements. Integer increment or endpoint discrepancy greater than signed span times 5e-12 returns plain range; otherwise interpolate. Four-argument endpoint overload supplies illegal range. Int-count overload constructs plain range without validation. | Verified (existing annotation); rerun pending |
| `IncrRange(r8,r8,r8,class:568) -> class:596` | 0x060006ec | 3205876242870ce8 | Endpoint overload computes int-converted step count from span/increment enlarged by factor 1.00000000001. Zero increment, NaN operands or negative nonsentinel count raise supplied error; int extrema raise range too many elements. Integer increment or endpoint discrepancy greater than signed span times 5e-12 returns plain range; otherwise interpolate. Four-argument endpoint overload supplies illegal range. Int-count overload constructs plain range without validation. | Verified (existing annotation); rerun pending |
| `IncrRange(r8,r8,i4,class:568) -> class:596` | 0x060006ed | 6c92ae5e0337eb68 | Endpoint overload computes int-converted step count from span/increment enlarged by factor 1.00000000001. Zero increment, NaN operands or negative nonsentinel count raise supplied error; int extrema raise range too many elements. Integer increment or endpoint discrepancy greater than signed span times 5e-12 returns plain range; otherwise interpolate. Four-argument endpoint overload supplies illegal range. Int-count overload constructs plain range without validation. | Verified (existing annotation); rerun pending |
| `asVector(string,u4) -> class:532` | 0x060006ee | 79bcec737c1fe2b0 | Delegate to asRealVector with supplied error arguments. | Analyzed; static only |
| `asMatrix(string,u4) -> class:532` | 0x060006ef | 79bcec737c1fe2b0 | Delegate to asRealVector with supplied error arguments. | Analyzed; static only |
| `asRealMatrix(string,u4) -> class:532` | 0x060006f0 | 79bcec737c1fe2b0 | Delegate to asRealVector with supplied error arguments. | Analyzed; static only |
| `asRealVector(string,u4) -> class:532` | 0x060006f1 | 4fc8aeafde722e8c | Allocate a one-column matrix of stored count, fill through virtual get and raw numeric set_1, then assign stored dimension. Error text/position unused. Negative count fails through matrix allocation. | Analyzed; static only |
| `asRange(string,u4) -> class:596` | 0x060006f2 | 339a830e88d83bd3 | Return this; supplied error arguments unused. | Verified (existing annotation); rerun pending |

## Mcd.RngValInterpolate (`0x02000096`)

| Member/signature | Token | Normalized IL prefix | Contract | Current classification / limits |
|---|---|---|---|---|
| `m_last : r8` | 0x04000151 | field | Interpolation endpoint. | Verified (existing annotation); rerun pending |
| `m_ninterval : i4` | 0x04000152 | field | Interpolation interval count. | Verified (existing annotation); rerun pending |
| `.ctor(r8,r8,i4,class:568) -> void` | 0x060006f3 | 2f4c9d1927a4d4a6 | Initialize base with increment span divided by interval count and element count intervals plus one; store last endpoint and interval count. No validation, including zero intervals. | Verified (existing annotation); rerun pending |
| `get(i4) -> r8` | 0x060006f4 | 8abf536410f8d31a | Compute endpoint-weighted interpolation using stored interval count; no bounds check. Zero intervals preserve floating-point division behavior. | Verified (existing annotation); rerun pending |
| `range_get(i4) -> class:584` | 0x060006f5 | bb6f71c2dfd4dbc4 | Wrap virtual get in fresh real NumVal using stored dimension. | Verified (existing annotation); rerun pending |
| `iterator_get(i4) -> class:584` | 0x060006f6 | bb6f71c2dfd4dbc4 | Wrap virtual get in fresh real NumVal using stored dimension. | Verified (existing annotation); rerun pending |

## Mcd.Iterator (`0x02000097`)

| Member/signature | Token | Normalized IL prefix | Contract | Current classification / limits |
|---|---|---|---|---|
| `first() -> object` | 0x060006f7 | no body | Abstract declaration: read current value supplied by concrete iterator. | Abstract; declaration only |
| `next() -> class:604` | 0x060006f8 | no body | Abstract declaration: concrete iterator advances and returns iterator or null. | Abstract; declaration only |
| `it_incr(object,class:604) -> object` | 0x060006f9 | 6fdee8c182e4f47d | Read current value, advance once while discarding returned iterator, return saved value; first argument unused. | Verified (existing annotation); rerun pending |
| `forRange(r8,r8,i4) -> class:604` | 0x060006fa | d59d45c8aa5e4cdd | Dispatch typed numeric and DimVal overloads to corresponding range factories, using identity dimension for numeric ranges; RngVal overload constructs RangeIterator, even for zero/negative counts. | Verified (existing annotation); rerun pending |
| `forRange(r8,r8) -> class:604` | 0x060006fb | 103e851cc9c14837 | Dispatch typed numeric and DimVal overloads to corresponding range factories, using identity dimension for numeric ranges; RngVal overload constructs RangeIterator, even for zero/negative counts. | Verified (existing annotation); rerun pending |
| `forRange(r8,r8,r8) -> class:604` | 0x060006fc | af4b745bfe015c91 | Dispatch typed numeric and DimVal overloads to corresponding range factories, using identity dimension for numeric ranges; RngVal overload constructs RangeIterator, even for zero/negative counts. | Verified (existing annotation); rerun pending |
| `forRange(class:584,class:584,class:584) -> class:604` | 0x060006fd | 458d2183e02521fc | Dispatch typed numeric and DimVal overloads to corresponding range factories, using identity dimension for numeric ranges; RngVal overload constructs RangeIterator, even for zero/negative counts. | Verified (existing annotation); rerun pending |
| `forRange(class:584,class:584) -> class:604` | 0x060006fe | d7e65248053ac155 | Dispatch typed numeric and DimVal overloads to corresponding range factories, using identity dimension for numeric ranges; RngVal overload constructs RangeIterator, even for zero/negative counts. | Verified (existing annotation); rerun pending |
| `forRange(class:596) -> class:604` | 0x060006ff | bfa3467db2579dcb | Dispatch typed numeric and DimVal overloads to corresponding range factories, using identity dimension for numeric ranges; RngVal overload constructs RangeIterator, even for zero/negative counts. | Verified (existing annotation); rerun pending |
| `forValues(class:604[]) -> class:604` | 0x06000700 | e77a584a69b3dc3a | Construct sequence cursor from supplied array without validating emptiness or entries. | Verified (existing annotation); rerun pending |
| `forValue(object) -> class:604` | 0x06000701 | 3716b2c713442d6a | Nonempty matrix gets MatrixIterator; matrix with either nonpositive dimension returns null. Range gets RangeIterator; every other object, including null, gets ScalarIterator. | Analyzed; static only |
| `.ctor() -> void` | 0x06000702 | 41ad57d14cd73988 | Protected default constructor initializes no iterator state. | Verified (existing annotation); rerun pending |

## Mcd.RangeIterator (`0x02000098`)

| Member/signature | Token | Normalized IL prefix | Contract | Current classification / limits |
|---|---|---|---|---|
| `m_rng : class:596` | 0x04000153 | field | Range reference. | Verified (existing annotation); rerun pending |
| `m_it : i4` | 0x04000154 | field | Current index, initialized zero then incremented. | Verified (existing annotation); rerun pending |
| `m_n : i4` | 0x04000155 | field | Count captured at construction. | Verified (existing annotation); rerun pending |
| `.ctor(class:596) -> void` | 0x06000703 | d51be1468891f9b1 | Store range and snapshot its Rsize, initialize index zero; null range fails while querying Rsize. | Verified (existing annotation); rerun pending |
| `first() -> object` | 0x06000704 | 274bbaea1d3c18bc | Delegate to ri_first. | Verified (existing annotation); rerun pending |
| `ri_first() -> object` | 0x06000705 | cf61d46dd52329b6 | Return range.range_get at current index, including past end; count is not checked. | Verified (existing annotation); rerun pending |
| `next() -> class:604` | 0x06000706 | cba2e8e655af3943 | Delegate to ri_next. | Verified (existing annotation); rerun pending |
| `ri_next() -> class:604` | 0x06000707 | 717acbee468f6140 | Increment index first; return null when at or beyond captured count, otherwise this. No terminal-state guard. | Verified (existing annotation); rerun pending |

## Mcd.Iterator+SeqIterator (`0x020000cd`)

| Member/signature | Token | Normalized IL prefix | Contract | Current classification / limits |
|---|---|---|---|---|
| `m_its : class:604[]` | 0x040001b4 | field | Supplied iterator array. | Verified (existing annotation); rerun pending |
| `m_curr : i4` | 0x040001b5 | field | Current array index. | Verified (existing annotation); rerun pending |
| `.ctor(class:604[]) -> void` | 0x060007e8 | 8e79712e147cbab0 | Store supplied iterator array and initialize current array index zero; no validation. | Verified (existing annotation); rerun pending |
| `first() -> object` | 0x060007e9 | d126f5118ff7caf5 | Delegate to current array entry. Empty array throws; null entry naturally fails. | Verified (existing annotation); rerun pending |
| `next() -> class:604` | 0x060007ea | 7c46ee28aef8a5f7 | Advance current child; if child returns null, increment current array index once. Return null when array exhausted, otherwise this. Does not skip empty children or replace child with a different nonnull return value. | Verified (existing annotation); rerun pending |

## Mcd.Iterator+ScalarIterator (`0x020000ce`)

| Member/signature | Token | Normalized IL prefix | Contract | Current classification / limits |
|---|---|---|---|---|
| `m_init : object` | 0x040001b6 | field | Supplied scalar object. | Verified (existing annotation); rerun pending |
| `.ctor(object) -> void` | 0x060007eb | 7f653efdc404879e | Store supplied object, including null. | Verified (existing annotation); rerun pending |
| `first() -> object` | 0x060007ec | d568e1acb60223dc | Return stored object. | Verified (existing annotation); rerun pending |
| `next() -> class:604` | 0x060007ed | ce2965e0f5bbb04a | Return null without changing stored value. | Verified (existing annotation); rerun pending |

## Mcd.Iterator+MatrixIterator (`0x020000cf`)

| Member/signature | Token | Normalized IL prefix | Contract | Current classification / limits |
|---|---|---|---|---|
| `m_mat : class:532` | 0x040001b7 | field | Matrix reference. | Analyzed; static only |
| `m_row : i4` | 0x040001b8 | field | Current row. | Analyzed; static only |
| `m_col : i4` | 0x040001b9 | field | Current column. | Analyzed; static only |
| `.ctor(class:532) -> void` | 0x060007ee | 9fb8262546ff42cc | Store matrix and initialize row/column zero without validating shape. | Analyzed; static only |
| `first() -> object` | 0x060007ef | c502343e728aeadd | Read matrix.get_2 at current row and column. | Analyzed; static only |
| `next() -> class:604` | 0x060007f0 | ee0b15624f44f86f | Increment row; when past current row count, increment column and either return null at current column count or reset row zero. Return this otherwise; matrix shape is read live. | Analyzed; static only |

## Existing differential cases

The suite contains four scenarios, pending the focused rerun in this session:

- `Construction`: 22 explicit initial/increment/endpoint tuples across increasing and decreasing ranges, fractional/integer steps, same endpoint, contradictory direction, zero step, NaNs, infinities and excessively large counts. Exercises default/custom errors, two/four-argument Range, endpoint and explicit-count IncrRange with counts 0, 1, 5 and -3. Queries index -1, first, second, last and one-past-last, sizes, integral flag, dimension, hash, copy and asRange identity.
- `Range_operators`: 11 typed values (unitless, units, polymorphic/zero dimensions, complex and string), all pairs for mkVec2/mkRange2 and each pair with four endpoint choices for mkVec3/mkRange3. Captures values and error identities/argument positions.
- `Equality`: plain/interpolating ranges with matching/different dimensions, count and increment; comparison to number/null; polymorphic hash and physical dimension.
- `Iterators`: every forRange overload, explicit zero count, descending/fractional ranges, units/errors, forValue for range/scalar/string/null and 2x3, 1x3, 3x1 and 0x0 matrices, `GetHashCode` of a range whose hash bits overlap its count, interpolation `get` from a nonzero start, `asRealVector` elements of a range starting at 2, sequence cursors with empty and nonempty children, it_incr past end, and direct RangeIterator first/next past end. Traversal is bounded at 30 values.

The `Iterators` scenario compares the RngVal matrix conversions (`asVector`, `asMatrix`, `asRealMatrix` on one range; `asRealVector` on two, with elements shown), which stay Analyzed because only those cases are recorded; MatrixIterator is not exercised and remains Analyzed. Field annotations derive from constructors/query/iteration scenarios; they do not establish all arbitrary reflected mutations.

## Not yet comparable

- `Quire.Mpl::Quire.Mcd.Iterator::.ctor`0()->[System.Runtime]System.Void|False`: Cannot be invoked directly through reflection `ConstructorInfo.Invoke` because `Quire.Mcd.Iterator` is an abstract class (`MemberAccessException: Cannot create an instance of Quire.Mcd.Iterator because it is an abstract class`). Subclass constructors invoke it as base constructor.
