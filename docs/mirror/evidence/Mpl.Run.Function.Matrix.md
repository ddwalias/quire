# Mpl.Run.Function matrix call bridge

- **Original:** mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **Reference:** ilspycmd 11.1.0.9782 member index and C# view from `tools/reference/reference.py`.
- **Quire:** [Function.cs](../../../src/Quire.Mpl/Run/Function.cs), [Function.Matrix.cs](../../../src/Quire.Mpl/Run/Function.Matrix.cs); tests: `FunctionMatrixDifferentialTests`.

`Function`'s matrix-call bridge routes `VecCall` through `MatVal.vectorize` when at least one argument is a matrix and vectorization is enabled. Scalar arguments use the matching dynamic call. `VecApply` vectorizes its argument vector unless `IgnoreVectorization(true)` has disabled that path. `IsFullyApplied` and `AssertFullApp` enforce the stored arity range; `OptFunc` changes its lower bound. The delegate projections validate their requested arity and return typed invocation delegates.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `IgnoreVectorization(bool)` | 0x06000155 | Stores whether vectorization should be bypassed. | Direct comparison; the following `VecApply` case exercises the enabled bypass. | Analyzed |
| `OptFunc(int)` | 0x06000156 | Sets the minimum arity. | Direct comparison followed by a padded application. | Analyzed |
| `get_Varargs()` | 0x0600014b | Returns whether the function accepts arbitrary arity. | Direct comparisons cover false and true values. | Analyzed |
| `IsFullyApplied(int)` | 0x060001a6 | Accepts counts between minimum arity and purity, inclusive, or any count when varargs is enabled. | Direct comparisons cover below, at and above the fixed range, and out-of-range counts with varargs. | Analyzed |
| `AssertFullApp(int)` | 0x060001a7 | Calls `BadCall` when a count is outside the accepted interval; varargs accepts every count. | Direct comparisons cover too few, accepted, too many and varargs counts. | Analyzed |
| `Apply(object[])` | 0x060001d3 | Applies an object vector through `appVec`, including optional padding and varargs dispatch. | Direct comparisons cover optional padding, exact arity and varargs invocation. | Analyzed |
| `VecCall(object)` | 0x060001cd | Vectorizes a matrix argument when enabled; otherwise calls the function dynamically. | Direct comparisons cover enabled matrix/scalar routing and a matrix with vectorization disabled. | Analyzed |
| `VecCall(object, object)` | 0x060001ce | Vectorizes when either argument is a matrix; otherwise calls dynamically. | Direct comparisons cover enabled matrix/scalar routing and a matrix with vectorization disabled. | Analyzed |
| `VecCall(object, object, object)` | 0x060001cf | Vectorizes when any argument is a matrix; otherwise calls dynamically. | Direct comparisons cover enabled matrix/scalar routing and a matrix with vectorization disabled. | Analyzed |
| `VecApply(object[])` | 0x060001d0 | Vectorizes the argument array unless vectorization is disabled, then calls `Apply`. | Direct comparisons cover matrix vectorization and matrix bypass when vectorization is disabled. | Analyzed |
| `AsDelegate_1()` | 0x060001dd | Returns the one-argument delegate projection after checking arity. | Direct comparison with a two-argument function whose minimum arity is lowered to one. | Analyzed |
| `AsDelegate_2()` | 0x060001de | Returns the two-argument delegate projection after checking arity. | Direct comparison with a two-argument function. | Analyzed |

## Test cases

`Function_arity_checks_apply_and_delegate_projection` compares the arity predicates, assertion, optional padding, exact application and delegate projections. `Function_matrix_call_bridge_vectorizes_only_matrix_inputs` compares fixed and varargs arity, application, all three `VecCall` overloads, both `VecApply` branches, and uses real `MatVal` values with `DelegateFunction` fixtures. For each `VecCall` overload it covers matrix vectorization, scalar dispatch, and the vectorization-disabled matrix path. No emitted probe types are used.

## Open

The direct calls cover the bridge branches listed above. `appVec`'s high-arity, captured-function and varargs cases remain outside this test unit.
