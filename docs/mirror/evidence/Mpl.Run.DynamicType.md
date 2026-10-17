# Mpl.Run.DynamicType

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.MplRunDynamicTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600041a | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TypeParameterName(int)` | 0x0600040c | Calls `TypeParameterName("T", i)` | Original IL; direct cases include zero and distinct positive integers | Verified |
| `TypeParameterName(string, int)` | 0x0600040d | Calls `String.Concat(object, object)` with `prefix` and boxed `i + 1` | Original IL; direct cases include null and distinct string/integer arguments | Verified |
| `FieldName(int)` | 0x06000417 | Calls `String.Concat(object, object)` with `"Field"` and boxed `i + 1` | Original IL; direct cases include zero and distinct positive integers | Verified |
| `.cctor()` | 0x060003fd | Initializes `TuplePrefix`, `FieldNameTable`, `DelimiterString`, and both type-name lookup tables in declaration order | Directly compared static field and table contents; Release IL equals Mathcad's | Verified |
| `FieldNames(int)` | 0x06000418 | Returns the cached array for `n`; otherwise allocates `string[n]`, fills it with `FieldName(i)`, stores and returns it | Direct cases cover 0, 1, 4, -1 and cached-array identity; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`MplRunDynamicTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

**Observed** (`DynamicTypeFieldNamesDifferentialTests`): the type initializer's static fields and all entries in its three
tables; `FieldNames` results for 0, 1 and 4; matching negative-length exceptions for -1; and reference identity on a repeated count.
