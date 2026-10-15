# Mpl.Run.Function matrix call bridge

- **Original:** mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **Reference:** ilspycmd 11.1.0.9782 member index and C# view from `tools/reference/reference.py`.
- **Quire:** [Function.cs](../../../src/Quire.Mpl/Run/Function.cs), [Function.Matrix.cs](../../../src/Quire.Mpl/Run/Function.Matrix.cs); tests: `FunctionMatrixDifferentialTests`.

`Function`'s matrix-call bridge routes `VecCall` through `MatVal.vectorize` when at least one argument is a matrix and vectorization is enabled. Scalar arguments use the matching dynamic call. `VecApply` vectorizes its argument vector unless `IgnoreVectorization(true)` has disabled that path. `IsFullyApplied` and `AssertFullApp` enforce the stored arity range; `OptFunc` changes its lower bound. The delegate projections validate their requested arity and return typed invocation delegates.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `IgnoreVectorization(bool)` | 0x06000155 | Stores whether vectorization should be bypassed. | Current focused run directly compared this setter and exercised the enabled bypass through `VecApply`. | Verified |
| `OptFunc(int)` | 0x06000156 | Sets the minimum arity. | Current focused run directly compared the setter and padded application. | Verified |
| `get_Varargs()` | 0x0600014b | Returns whether the function accepts arbitrary arity. | Current focused run compared false and true values. | Verified |
| `IsFullyApplied(int)` | 0x060001a6 | Accepts counts between minimum arity and purity, inclusive, or any count when varargs is enabled. | Current focused run compared below, at and above the fixed range, and out-of-range counts with varargs. | Verified |
| `AssertFullApp(int)` | 0x060001a7 | Calls `BadCall` when a count is outside the accepted interval; varargs accepts every count. | Current focused run compared too few, accepted, too many and varargs counts. | Verified |
| `Apply(object[])` | 0x060001d3 | Applies an object vector through `appVec`, including optional padding and varargs dispatch. | Current focused run compared optional padding, exact arity and varargs invocation. | Verified |
| `VecCall(object)` | 0x060001cd | Vectorizes a matrix argument when enabled; otherwise calls the function dynamically. | Current focused run compared scalar and matrix input with vectorization on and off; a matrix-sensitive delegate distinguishes the paths. | Verified |
| `VecCall(object, object)` | 0x060001ce | Vectorizes when either argument is a matrix; otherwise calls dynamically. | Current focused run compared every scalar/matrix mask with vectorization on and every nonempty matrix mask with it off; an ordered matrix-sensitive delegate distinguishes dispatch and argument positions. | Verified |
| `VecCall(object, object, object)` | 0x060001cf | Vectorizes when any argument is a matrix; otherwise calls dynamically. | Current focused run compared every scalar/matrix mask with vectorization on and every nonempty matrix mask with it off; an ordered matrix-sensitive delegate distinguishes dispatch and argument positions. | Verified |
| `VecApply(object[])` | 0x060001d0 | Vectorizes the argument array unless vectorization is disabled, then calls `Apply`. | Current focused run compared matrix vectorization and matrix bypass when vectorization is disabled. | Verified |
| `AsDelegate_1()` | 0x060001dd | Returns the one-argument delegate projection after checking arity. | Current focused run compared success and too-few-arguments exception, including its reported error position. | Verified |
| `AsDelegate_2()` | 0x060001de | Returns the two-argument delegate projection after checking arity. | Current focused run compared success and too-few-arguments exception, including its reported error position. | Verified |

## Test cases

`Function_arity_checks_apply_and_delegate_projection` compares the arity predicates, assertion, optional padding, exact application, successful delegate projections, and their too-few-arguments exceptions. `Function_matrix_call_bridge_vectorizes_only_matrix_inputs` compares all three `VecCall` overloads over every scalar/matrix argument mask with vectorization on and every nonempty matrix mask with vectorization disabled. Matrix-sensitive ordered delegates distinguish elementwise vectorization from passing the whole matrix to dynamic dispatch. It also covers both `VecApply` branches and uses real `MatVal` values with `DelegateFunction` fixtures. No emitted probe types are used.

Current focused run `/tmp/quire-b-function-verify-scratch/function-verify-8da5693-r2` passed Harness 27/27, Mpl 159/159, and six selected Differential tests, with lint 0, ratchet ok and zero uncovered lines/branches. The wider audit remains incomplete for methods outside this record.

## Open

The direct calls cover the bridge branches listed above. `appVec`'s high-arity, captured-function and varargs cases remain outside this test unit.
