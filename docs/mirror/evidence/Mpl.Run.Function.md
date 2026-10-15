# Mpl.Run.Function

- **Original:** mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **Reference:** ilspycmd 11.1.0.9782 member index and C# view from `tools/reference/reference.py`.
- **Quire:** [Function.cs](../../../src/Quire.Mpl/Run/Function.cs); tests: `FunctionDifferentialTests`.

`Function` stores purity, minimum arity, varargs mode, vectorization preference, and an optional name. `format_vector` renders a bracketed comma-separated list. `canonical_name` removes internal prefixes and converts `#`-separated names to braces. `IsFullyApplied` accepts counts in the inclusive minimum-to-purity interval (or all counts for varargs); `AssertFullApp` raises the runtime's argument error outside that interval. `OptFunc` sets the minimum arity, and `Apply` delegates to the vector application path. `DeclareInverse` links the two functions in both directions.

The protected state fields `m_purity`, `m_n`, `m_no_vectorize`, `m_minargs`, `m_varargs`, and `m_name` store those values and are classified as analyzed data members.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `get_Purity()` | 0x06000149 | Returns the stored purity. | Direct comparison on a two-argument delegate fixture. | Verified |
| `get_MinArgs()` | 0x0600014a | Returns the minimum accepted arity. | Direct comparison before optional arity is lowered. | Verified |
| `get_Varargs()` | 0x0600014b | Returns whether the function accepts arbitrary arity. | Direct comparison on a fixed-arity delegate fixture. | Verified |
| `get_IsFunction()` | 0x0600015a | Returns this function instance. | Direct comparison on a concrete delegate function. | Verified |
| `get_Name()` | 0x0600014c | Returns the stored name or the virtual subclass name. | Base implementation is not directly invoked here because concrete Function subclasses override this getter. | Analyzed |
| `set_Name(string)` | 0x0600014d | Stores the supplied name. | Direct comparison and target-state rendering. | Analyzed |
| `ToString()` | 0x0600014e | Returns `Function: ` followed by `Name`. | Direct comparison after setting an explicit name. | Analyzed |
| `format_vector(object[])` | 0x0600014f | Returns `[]` for null or empty input; a singleton is interpolated; multiple values call `ToString()` on the first value and interpolate subsequent values between brackets with commas. | Direct comparisons cover null, empty, singleton, multiple values, first-element null exception and later null interpolation. | Analyzed |
| `canonical_name(string)` | 0x06000150 | Removes `mc_`, maps `ui_` names to `anonymous`, and converts `#`-separated suffixes to braces. | Direct comparisons cover each prefix, labels, simple and null names. | Analyzed |
| `IgnoreVectorization(bool)` | 0x06000155 | Stores whether matrix vectorization should be bypassed. | Direct comparison in the matrix-call tests. | Analyzed |
| `OptFunc(int)` | 0x06000156 | Sets the minimum arity used by optional-argument padding. | Direct comparison followed by an optional-argument application. | Analyzed |
| `DeclareInverse(Mpl.Run.Function)` | 0x06000159 | Sets reciprocal inverse-function references and returns the supplied inverse. | Direct comparison with separate corresponding Quire and Mathcad function instances; state is rendered recursively with a depth limit. | Analyzed |
| `Mpl.Run.DelegateFunction.get_Name()` | 0x060001f7 | Returns the explicitly stored name when present; otherwise canonicalizes the wrapped delegate method name. | Direct comparisons exercise the cached and fallback paths on corresponding concrete delegate functions. | Analyzed |
| `IsFullyApplied(int)` | 0x060001a6 | Tests whether an argument count lies within the allowed arity interval. | Direct comparisons exercise below, at, within and above the range. | Analyzed |
| `AssertFullApp(int)` | 0x060001a7 | Returns normally for an accepted count; otherwise reports the minimum or maximum allowed count through `BadCall`. | Direct comparisons exercise success and both failure sides. | Analyzed |
| `Apply(object[])` | 0x060001d3 | Applies the supplied vector through the function's vector dispatch. | Direct comparisons cover optional padding and exact arity. | Analyzed |
| `InvokeVec(object[])` | 0x060001d4 | Delegates the supplied vector to inherited `DimVal.DynApply`. | Direct comparison on a two-argument delegate function; source structure follows the original inherited call. | Verified |
| `VecApply(object[])` | 0x060001d0 | Applies vectorization or bypasses it when vectorization is ignored. | Direct comparisons exercise the vectorized and ignored-vectorization paths. | Analyzed |
| `AsDelegate_1()` | 0x060001dd | Returns the one-argument delegate projection. | Direct comparison with a function that accepts one argument. | Analyzed |
| `AsDelegate_2()` | 0x060001de | Returns the two-argument delegate projection. | Direct comparison with a two-argument function. | Analyzed |

## Test cases

`Function_formatting_identity_and_inverse_links` compares formatting, canonical names, the actual `DelegateFunction.get_Name` override and both of its branches, string conversion, and inverse linking. `Function_application_optional_padding_and_delegate_projection` compares arity accessors, `get_IsFunction`, `OptFunc`, application, `InvokeVec`, and delegate projections.

## Open

The tests in this record cover the listed members and their stated branches. Other `Function` dispatch overloads remain outside this unit's direct comparisons.
