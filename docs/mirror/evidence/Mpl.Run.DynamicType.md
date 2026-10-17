# Mpl.Run.DynamicType

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.MplRunDynamicTypeMechanicalDifferentialTests`, `DynamicTypeConcat2MiDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600041a | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TypeParameterName(int)` | 0x0600040c | Calls `TypeParameterName("T", i)` | Original IL; direct cases include zero and distinct positive integers | Verified |
| `TypeParameterName(string, int)` | 0x0600040d | Calls `String.Concat(object, object)` with `prefix` and boxed `i + 1` | Original IL; direct cases include null and distinct string/integer arguments | Verified |
| `FieldName(int)` | 0x06000417 | Calls `String.Concat(object, object)` with `"Field"` and boxed `i + 1` | Original IL; direct cases include zero and distinct positive integers | Verified |
| `NameToType(string)` | 0x06000410 | Looks up `tyName` in `NameToTypeTable`; calls `AppException.Raise("demangle error")` when no value exists, then returns the `Type` | Original IL; direct cases cover all mappings, unknown and null keys | Verified |
| `TypeToName(Type)` | 0x06000411 | Looks up `ty` in `TypeToNameTable`; returns the mapped string or `.ref` | Original IL; direct cases cover all mappings, an unmapped type and a null key | Verified |
| `.cctor()` | 0x060003fd | Initializes `TuplePrefix`, `FieldNameTable`, `DelimiterString`, and both type-name lookup tables in declaration order | Directly compared static field and table contents; Release IL equals Mathcad's | Verified |
| `FieldNames(int)` | 0x06000418 | Returns the cached array for `n`; otherwise allocates `string[n]`, fills it with `FieldName(i)`, stores and returns it | Direct cases cover 0, 1, 4, -1 and cached-array identity; Release IL equals Mathcad's | Verified |
| `MplConcat3(object, object, object)` | 0x06000405 | Replaces each null argument with the string "null", then calls `String.Concat(object, object, object)` | Direct cases cover all-null, each null position, the literal string "null", and mixed boxed values; Release IL equals Mathcad's | Verified |
| `Concat2Mi()` | 0x06000407 | Resolves and returns `System.String.Concat(System.Object, System.Object)` through `Type.GetMethod(string, Type[])` | High; direct comparison records declaring type, method name, parameter types, return type and static flag | Verified |

## Test cases

**Observed** (`MplRunDynamicTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

**Observed** (`DynamicTypeConcat2MiDifferentialTests`): Directly invokes the private helper on each assembly and compares the resolved method signature.

**Observed** (`DynamicTypeFieldNamesDifferentialTests`): the type initializer's static fields and all entries in its three
tables; `FieldNames` results for 0, 1 and 4; matching negative-length exceptions for -1; and reference identity on a repeated count.

**Observed** (`DynamicTypeNameMapsDifferentialTests`): `NameToType` for `I`, `F`, `O`, an unknown name and a null key; `TypeToName` for `int`, `double`, `object`, `string` and a null key. Each method is directly invoked against the installed Mathcad artifact.

**Observed** (`DynamicTypeMplConcat3DifferentialTests`): all-null, each individual null position, a literal `"null"` value,
and mixed strings and boxed values.
