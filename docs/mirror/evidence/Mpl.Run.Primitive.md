# Mpl.Run.Primitive

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** ilspycmd 11.1.0.9782 (tools/reference)
- **Quire:** `src/Quire.Mpl/Run/Primitive.cs`, `src/Quire.Mpl/Generated/Mirror/Mpl.Run.Primitive.g.cs` (stubs);
  tests: `Quire.Differential.Tests.PrimitiveDifferentialTests`

The twelve comparisons, eleven arithmetic helpers, two reference-selection helpers, two conversion helpers, three array constructors, eleven array element operations, four primitive-kind predicates and five typed cast helpers are implemented alongside the
listed MemberGen members; other Primitive members remain stubs. Each comparison is a single
`ceq`/`cgt`/`clt` (negated with `ceq 0` for `!=`, `<=` and `>=`, using `cgt.un`/`clt.un` for doubles so NaN gives 0)
returning 1 or 0, which the Release build of `x OP y ? 1 : 0` reproduces exactly.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ceq_i4(int, int)` | 0x0600036f | 1 when `x == y`, else 0. | High | Verified |
| `cne_i4(int, int)` | 0x06000370 | 1 when `x != y`, else 0. | High | Verified |
| `clt_i4(int, int)` | 0x06000371 | 1 when `x < y` (signed), else 0. | High | Verified |
| `cle_i4(int, int)` | 0x06000372 | 1 when `x <= y` (signed), else 0. | High | Verified |
| `cgt_i4(int, int)` | 0x06000373 | 1 when `x > y` (signed), else 0. | High | Verified |
| `cge_i4(int, int)` | 0x06000374 | 1 when `x >= y` (signed), else 0. | High | Verified |
| `ceq_f8(double, double)` | 0x0600037a | 1 when `x == y` (IEEE: -0 equals 0, NaN equals nothing), else 0. | High | Verified |
| `cne_f8(double, double)` | 0x0600037b | 1 when `x != y` (true whenever either is NaN), else 0. | High | Verified |
| `clt_f8(double, double)` | 0x0600037c | 1 when `x < y`; 0 when either is NaN. | High | Verified |
| `cle_f8(double, double)` | 0x0600037d | 1 when `x <= y`; 0 when either is NaN. | High | Verified |
| `cgt_f8(double, double)` | 0x0600037e | 1 when `x > y`; 0 when either is NaN. | High | Verified |
| `cge_f8(double, double)` | 0x0600037f | 1 when `x >= y`; 0 when either is NaN. | High | Verified |
| `as_matrix(object)` | 0x06000395 | Returns the input when it is a `MatVal`; otherwise null. | High | Verified |
| `as_number(object)` | 0x06000396 | Returns the input when it is a `NumVal`; otherwise null. | High | Verified |
| `as_string(object)` | 0x06000397 | Returns the input when it is a string; otherwise null. | High | Verified |
| `as_function(object)` | 0x06000398 | Returns the input when it is a `Function`; otherwise null. | High | Verified |
| `as_range(object)` | 0x06000399 | Returns the input when it is an `RngVal`; otherwise null. | High | Verified |
| `IsOpCodePrimitive()` | 0x06000359 | True when `m_code` is null and `m_opcodes` is non-null. | High | Verified |
| `IsCodePrimitive()` | 0x0600035a | True when `m_code` is non-null. | High | Verified |
| `IsMethodPrimitive()` | 0x0600035b | True when `m_method` is non-null, `m_code` is null and `m_opcodes` is null. | High | Verified |
| `IsForeignPrimitive()` | 0x0600035c | True when `m_foreign` is non-null, including the empty string. | High | Verified |
| `or_ref(object, object)` | 0x06000366 | Returns `y` when `x` is null; otherwise returns `x`. | High; direct null branch | Verified |
| `and_ref(object, object)` | 0x06000367 | Returns `y` when `x` is non-null; otherwise returns null. | High; direct null branch | Verified |
| `conv_i4(double)` | 0x06000380 | Converts the double to int using the runtime's `conv.i4` behavior. | High; direct cast | Verified |
| `conv_f8(int)` | 0x06000381 | Converts the int to double using `conv.r8`. | High; direct widening conversion | Verified |
| `arr_i4(int)` | 0x06000384 | Creates a zero-initialized `int[]` of the requested length. | High; direct `newarr int` | Verified |
| `arr_f8(int)` | 0x06000385 | Creates a zero-initialized `double[]` of the requested length. | High; direct `newarr double` | Verified |
| `arr_ref(int)` | 0x06000386 | Creates a null-initialized `object[]` of the requested length. | High; direct `newarr object` | Verified |
| `ldelem_i4(int[], int)` | 0x06000387 | Returns the integer element at `index`. | High; direct typed array load | Verified |
| `stelem_i4_void(int[], int, int)` | 0x06000388 | Stores `val` at `index` in `arr`. | High; direct typed array store | Verified |
| `stelem_i4(int[], int, int)` | 0x06000389 | Stores `val` at `index` and returns the same `arr` reference. | High; direct typed array store and argument return | Verified |
| `ldelem_f8(double[], int)` | 0x0600038a | Returns the double element at `index`. | High; direct typed array load | Verified |
| `stelem_f8_void(double[], int, double)` | 0x0600038b | Stores `val` at `index` in `arr`. | High; direct typed array store | Verified |
| `stelem_f8(double[], int, double)` | 0x0600038c | Stores `val` at `index` and returns the same `arr` reference. | High; direct typed array store and argument return | Verified |
| `ldelem_ref(object[], int)` | 0x0600038d | Returns the object element at `index`. | High; direct reference array load | Verified |
| `stelem_ref_void(object[], int, object)` | 0x0600038e | Stores `val` at `index` in `arr`. | High; direct reference array store | Verified |
| `stelem_ref(object[], int, object)` | 0x0600038f | Stores `val` at `index` and returns the same `arr` reference. | High; direct reference array store and argument return | Verified |
| `stelem_vec(object, object[], int)` | 0x06000390 | Stores `val` at `index` in `clo`. | High; direct reference array store | Verified |
| `ldlen(object[])` | 0x06000391 | Returns the array length. | High; direct `ldlen` and `conv.i4` | Verified |
| `neg_i4(int)` | 0x06000369 | Returns the unchecked integer negation of `x`. | High; direct `neg` opcode | Verified |
| `add_i4(int, int)` | 0x0600036a | Returns the unchecked integer sum of `x` and `y`. | High; direct `add` opcode | Verified |
| `sub_i4(int, int)` | 0x0600036b | Returns the unchecked integer difference of `x` and `y`. | High; direct `sub` opcode | Verified |
| `mul_i4(int, int)` | 0x0600036c | Returns the unchecked integer product of `x` and `y`. | High; direct `mul` opcode | Verified |
| `div_i4(int, int)` | 0x0600036d | Returns signed integer division; zero divisor and `int.MinValue / -1` throw `DivideByZeroException` and `OverflowException`. | High; direct `div` opcode | Verified |
| `rem_i4(int, int)` | 0x0600036e | Returns signed integer remainder; zero divisor and `int.MinValue % -1` throw `DivideByZeroException` and `OverflowException`. | High; direct `rem` opcode | Verified |
| `neg_f8(double)` | 0x06000375 | Returns the IEEE double negation of `x`. | High; direct `neg` opcode | Verified |
| `add_f8(double, double)` | 0x06000376 | Returns the IEEE double sum of `x` and `y`. | High; direct `add` opcode | Verified |
| `sub_f8(double, double)` | 0x06000377 | Returns the IEEE double difference of `x` and `y`. | High; direct `sub` opcode | Verified |
| `mul_f8(double, double)` | 0x06000378 | Returns the IEEE double product of `x` and `y`. | High; direct `mul` opcode | Verified |
| `div_f8(double, double)` | 0x06000379 | Returns the IEEE double quotient; zero divisors produce infinity or NaN as defined by IEEE arithmetic. | High; direct `div` opcode, no finite-result check | Verified |
| `Throw(Exception)` | 0x06000368 | MemberGen: `throw e` | Mechanical; Release IL equals Mathcad's | Verified |
| `identity(object)` | 0x06000382 | MemberGen: `o` | Mechanical; Release IL equals Mathcad's | Verified |
| `dup(object)` | 0x06000383 | MemberGen: `throw new AppException("may not capture pseudo function")` | Mechanical; Release IL equals Mathcad's | Verified |
| `build(object, object)` | 0x06000392 | MemberGen: `throw new AppException("build: must inline")` | Mechanical; Release IL equals Mathcad's | Verified |
| `dfoldr(object, object, object)` | 0x06000393 | MemberGen: `throw new AppException("dfoldr: must inline")` | Mechanical; Release IL equals Mathcad's | Verified |
| `conv_void(object)` | 0x06000394 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `invalid(object)` | 0x0600039a | MemberGen: `throw new AppException("compiler error")` | Mechanical; Release IL equals Mathcad's | Verified |
| `mpl_type(object)` | 0x0600039b | MemberGen: `throw new AppException("compiler error")` | Mechanical; Release IL equals Mathcad's | Verified |
| `rv(double)` | 0x0600039d | MemberGen: `new NumVal(r, 0.0)` | Mechanical; Release IL equals Mathcad's | Verified |
| `rrv(double, double)` | 0x0600039e | MemberGen: `new NumVal(r, i)` | Mechanical; Release IL equals Mathcad's | Verified |
| `seq(object, object)` | 0x060003a5 | MemberGen: `y` | Mechanical; Release IL equals Mathcad's | Verified |
| `seq_void(object, object)` | 0x060003a6 | MemberGen: `throw new AppException("compiler error")` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PrimitiveDifferentialTests`, against the installed `mplrun.dll`; results compared as values):
- each `_i4` comparison on every ordered pair of `int.MinValue`, -1, 0, 1 and `int.MaxValue`;
- each `_f8` comparison on every ordered pair of -∞, -1, -0, 0, 1, +∞ and NaN.
- each cast helper with a matching target instance, rendered as the same reference only when the result is reference-identical to its input.
- primitive-kind predicates with states that independently vary `m_code`, `m_opcodes`, `m_method`, and `m_foreign`, including an empty foreign string.
- integer arithmetic with overflow wraparound, signed division/remainder, zero divisors and the `int.MinValue` / `-1` edge;
- double arithmetic with NaN, infinities, signed zero, zero divisors and overflow to infinity.
- reference selection with all nullness combinations; output renderers distinguish the first input, second input and null by reference identity.
- double-to-int conversion with fractional values, integer bounds, out-of-range values, NaN and infinities; int-to-double conversion at signed bounds and representative values.
- integer, double and object array creation with lengths 0, 1, 3 and -1; renderers compare runtime array type, element values and resulting exceptions.
- typed integer/double/object array loads and stores, mutation, returned-array identity, index failures, and object-array length including null input.

MemberGen members and null/wrong-type cast-helper cases are compared by `Quire.Differential.Tests.MplRunPrimitiveMechanicalDifferentialTests`.
Primitive-kind predicates are compared by `Quire.Differential.Tests.PrimitivePredicateDifferentialTests`.
Arithmetic helpers are compared by `Quire.Differential.Tests.PrimitiveArithmeticDifferentialTests`.
Reference-selection helpers are compared by `Quire.Differential.Tests.PrimitiveReferenceDifferentialTests`.
Conversion helpers are compared by `Quire.Differential.Tests.PrimitiveConversionDifferentialTests`.
Array constructors are compared by `Quire.Differential.Tests.PrimitiveArrayDifferentialTests`.
Array element operations are compared by `Quire.Differential.Tests.PrimitiveArrayAccessDifferentialTests`.
