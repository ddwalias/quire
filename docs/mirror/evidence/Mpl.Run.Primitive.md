# Mpl.Run.Primitive

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** ilspycmd 11.1.0.9782 (tools/reference)
- **Quire:** `src/Quire.Mpl/Run/Primitive.cs`, `src/Quire.Mpl/Generated/Mirror/Mpl.Run.Primitive.g.cs` (stubs);
  tests: `Quire.Differential.Tests.PrimitiveDifferentialTests`

The twelve comparisons, four primitive-kind predicates and five typed cast helpers are implemented alongside the
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

MemberGen members and null/wrong-type cast-helper cases are compared by `Quire.Differential.Tests.MplRunPrimitiveMechanicalDifferentialTests`.
Primitive-kind predicates are compared by `Quire.Differential.Tests.PrimitivePredicateDifferentialTests`.
