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
| `clo<CLO>(CLO)` | 0x060001a4 | Constructs a closure wrapping this function and one argument. | Current focused run directly compared `clo` returning `Fn_clo<CLO>` wrapping receiver and argument. Exact IL match. | Verified |
| `clovec<CLO>(CLO[])` | 0x060001a5 | Iteratively wraps each argument in `clo`. | Current focused run directly compared `clovec` returning chained `Fn_clo<CLO>` wrappers. Exact IL match. | Verified |
| `appVec<R>(out R, object[])` | 0x060001a9 | Dispatches argument vectors across arities 1–16, varargs, exact purity, overapplication, optional padding, and partial closure. | Current focused run directly compared count 0..18, varargs, overapplication remainder arguments, high-arity optional padding, and below-minimum partial closures. Exact IL match. | Verified |

## Test cases

`Function_arity_checks_apply_and_delegate_projection` compares the arity predicates, assertion, optional padding, exact application, successful delegate projections, and their too-few-arguments exceptions. `Function_matrix_call_bridge_vectorizes_only_matrix_inputs` compares all three `VecCall` overloads over every scalar/matrix argument mask with vectorization on and every nonempty matrix mask with vectorization disabled. Matrix-sensitive ordered delegates distinguish elementwise vectorization from passing the whole matrix to dynamic dispatch. It also covers both `VecApply` branches and uses real `MatVal` values with `DelegateFunction` fixtures. No emitted probe types are used.
`FunctionAppVecDirectDifferentialTests` directly compares `Function.clo<CLO>`, `Function.clovec<CLO>`, and `Function.appVec<R>` across 11 test facts covering counts 0..18, boundary arities 16 and 17, varargs precedence, overapplication remainder preservation, optional argument padding, below-minimum closure creation, null arguments, and delegate exception preservation.

Current focused run passed Harness, Mpl, and the differential tests, with lint 0, ratchet ok and exact IL matches for `clo<CLO>`, `clovec<CLO>`, and `appVec<R>`.

## Open

The direct calls cover the bridge and dispatch branches listed above.
