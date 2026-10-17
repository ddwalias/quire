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

## Test cases

**Observed** (`MplRunDynamicTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
