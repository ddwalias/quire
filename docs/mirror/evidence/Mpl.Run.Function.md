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
| `set_Name(string)` | 0x0600014d | Stores the supplied name. | Current focused run directly compared the setter and rendered target state. | Verified |
| `ToString()` | 0x0600014e | Returns `Function: ` followed by `Name`. | Current focused run directly compared the result. IL differs in string construction; recorded below. | Verified |
| `format_vector(object[])` | 0x0600014f | Returns `[]` for null or empty input; a singleton is interpolated; multiple values call `ToString()` on the first value and interpolate subsequent values between brackets with commas. | Current focused run directly compared null, empty, singleton, multiple values, first-element null exception and later null interpolation. IL differs in string construction; recorded below. | Verified |
| `canonical_name(string)` | 0x06000150 | Removes `mc_`, maps `ui_` names to `anonymous`, and converts `#`-separated suffixes to braces. | Current focused run directly compared each prefix, labels, simple and null names. IL differs in string construction; recorded below. | Verified |
| `IgnoreVectorization(bool)` | 0x06000155 | Stores whether matrix vectorization should be bypassed. | Current focused run directly compared the matrix-call tests. | Verified |
| `OptFunc(int)` | 0x06000156 | Sets the minimum arity used by optional-argument padding. | Current focused run directly compared this setter and optional-argument application. | Verified |
| `DeclareInverse(Mpl.Run.Function)` | 0x06000159 | Sets reciprocal inverse-function references and returns the supplied inverse. | Current focused run directly compared separate corresponding instances; target state uses a recursion limit. | Verified |
| `closure<CLO>(Delegate,CLO,int)` | 0x060001a3 | Returns specialized closure wrappers for purities 2–5; all other purities construct `Fn_clo<CLO>` with the supplied delegate, captured value, and purity. | 14 direct comparisons covering purities 1–6 (each specialized arm and both default-boundary examples), a non-F delegate at purity 2, and downstream Apply. | Verified |
| `Mpl.Run.DelegateFunction.make(Delegate, int)` | 0x060001ef | Wraps a delegate in a DelegateFunction, specializing into Unary/Binary/Ternary/QuartnaryDelegateFunction when matching F delegate. | Direct comparison across purity 1..5. | Verified |
| `Mpl.Run.DelegateFunction.makeF(F<object, object>, string)` | 0x060001f0 | Factory creating UnaryDelegateFunction with canonicalized name. | Direct comparison with named delegate. | Verified |
| `Mpl.Run.DelegateFunction.makeF<A, B, R>(F<A, B, R>, string)` | 0x060001f1 | Factory creating BinaryDelegateFunction with canonicalized name. | Direct comparison with named delegate. | Verified |
| `Mpl.Run.DelegateFunction.makeF<A, B, C, R>(F<A, B, C, R>, string)` | 0x060001f2 | Factory creating TernaryDelegateFunction with canonicalized name. | Direct comparison with named delegate. | Verified |
| `Mpl.Run.DelegateFunction.makeF<A, B, C, D, R>(F<A, B, C, D, R>, string)` | 0x060001f3 | Factory creating QuartnaryDelegateFunction with canonicalized name. | Direct comparison with named delegate. | Verified |
| `Mpl.Run.DelegateFunction..ctor(Delegate, int)` | 0x060001f4 | Initializes DelegateFunction with underlying delegate and purity, minArgs 0. | Direct comparison of constructor invocation. | Verified |
| `Mpl.Run.DelegateFunction.app0()` | 0x060001f5 | Invokes 0-purity delegate with empty vector, or returns this. | Direct comparison of purity 0 and 1. | Verified |
| `Mpl.Run.DelegateFunction.clo<CLO>(CLO)` | 0x060001f6 | Creates closure binding argument a to m_f with m_purity. | Direct comparison with bound argument. | Verified |
| `Mpl.Run.DelegateFunction.get_Name()` | 0x060001f7 | Returns the explicitly stored name when present; otherwise canonicalizes the wrapped delegate method name. | Current focused run directly compared cached and fallback paths. IL uses `call` versus `callvirt` and different branch counts; recorded below. | Verified |
| `Mpl.Run.DelegateFunction.remove_module_suffix(string)` | 0x060001f8 | Strips "R.dll" or "D.dll" suffix (case-insensitive). | Direct comparison on various filenames. | Verified |
| `Mpl.Run.DelegateFunction.invVec<R>(out R, object[])` | 0x060001f9 | DynamicInvoke wrapper returning cast result. | Direct comparison with object[] args. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R>(out R)` | 0x060001fa | Direct call via F<R> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A>(out R, A)` | 0x060001fb | Direct call via F<R, A> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B>(out R, A, B)` | 0x060001fc | Direct call via F<R, A, B> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C>(out R, A, B, C)` | 0x060001fd | Direct call via F<R, A, B, C> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D>(out R, A, B, C, D)` | 0x060001fe | Direct call via F<R, A, B, C, D> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E>(out R, A, B, C, D, E)` | 0x060001ff | Direct call via F<R, A, B, C, D, E> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E, F>(out R, A, B, C, D, E, F)` | 0x06000200 | Direct call via F<R, A, B, C, D, E, F> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E, F, G>(out R, A, B, C, D, E, F, G)` | 0x06000201 | Direct call via F<R, A, B, C, D, E, F, G> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E, F, G, H>(out R, A, B, C, D, E, F, G, H)` | 0x06000202 | Direct call via F<R, A, B, C, D, E, F, G, H> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E, F, G, H, I>(out R, A, B, C, D, E, F, G, H, I)` | 0x06000203 | Direct call via F<R, A, B, C, D, E, F, G, H, I> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E, F, G, H, I, J>(out R, A, B, C, D, E, F, G, H, I, J)` | 0x06000204 | Direct call via F<R, A, B, C, D, E, F, G, H, I, J> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E, F, G, H, I, J, K>(out R, A, B, C, D, E, F, G, H, I, J, K)` | 0x06000205 | Direct call via F<R, A, B, C, D, E, F, G, H, I, J, K> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E, F, G, H, I, J, K, L>(out R, A, B, C, D, E, F, G, H, I, J, K, L)` | 0x06000206 | Direct call via F<R, A, B, C, D, E, F, G, H, I, J, K, L> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M>(out R, A, B, C, D, E, F, G, H, I, J, K, L, M)` | 0x06000207 | Direct call via F<R, A, B, C, D, E, F, G, H, I, J, K, L, M> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>(out R, A, B, C, D, E, F, G, H, I, J, K, L, M, N)` | 0x06000208 | Direct call via F<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(out R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O)` | 0x06000209 | Direct call via F<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(out R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P)` | 0x0600020a | Direct call via F<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P> cast, or fallback DynamicInvoke. | Direct comparison of direct and fallback dispatch. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.AsF<A, R>(uint)` | 0x0600020b | Returns m_f as F<A, R> if matching, else delegates to base. | Direct comparison of matching and fallback. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.AsF<A, B, R>(uint)` | 0x0600020c | Returns m_f as F<A, B, R> if matching, else delegates to base. | Direct comparison of matching and fallback. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.AsF<A, B, C, R>(uint)` | 0x0600020d | Returns m_f as F<A, B, C, R> if matching, else delegates to base. | Direct comparison of matching and fallback. Exact IL match. | Verified |
| `Mpl.Run.DelegateFunction.AsF<A, B, C, D, R>(uint)` | 0x0600020e | Returns m_f as F<A, B, C, D, R> if matching, else delegates to base. | Direct comparison of matching and fallback. Exact IL match. | Verified |
| `clo<CLO>(CLO)` | 0x060001a4 | Constructs a closure wrapping this function and one argument. | Current focused run directly compared `clo` returning `Fn_clo<CLO>` wrapping receiver and argument. Exact IL match. | Verified |
| `clovec<CLO>(CLO[])` | 0x060001a5 | Iteratively wraps each argument in `clo`. | Current focused run directly compared `clovec` returning chained `Fn_clo<CLO>` wrappers. Exact IL match. | Verified |
| `IsFullyApplied(int)` | 0x060001a6 | Tests whether an argument count lies within the allowed arity interval. | Current focused run directly compared counts below, at, within and above the range. | Verified |
| `AssertFullApp(int)` | 0x060001a7 | Returns normally for an accepted count; otherwise reports the minimum or maximum allowed count through `BadCall`. | Current focused run directly compared success and both failure sides. | Verified |
| `appVec<R>(out R, object[])` | 0x060001a9 | Dispatches argument vectors across arities 1–16, varargs, exact purity, overapplication, optional padding, and partial closure. | Current focused run directly compared count 0..18, varargs, overapplication remainder arguments, high-arity optional padding, and below-minimum partial closures. Exact IL match. | Verified |
| `Apply(object[])` | 0x060001d3 | Applies the supplied vector through the function's vector dispatch. | Current focused run directly compared optional padding and exact arity. | Verified |
| `InvokeVec(object[])` | 0x060001d4 | Delegates the supplied vector to inherited `DimVal.DynApply`. | Direct comparison on a two-argument delegate function; source structure follows the original inherited call. | Verified |
| `VecApply(object[])` | 0x060001d0 | Applies vectorization or bypasses it when vectorization is ignored. | Current focused run directly compared vectorized and ignored-vectorization paths. | Verified |
| `AsDelegate_1()` | 0x060001dd | Returns the one-argument delegate projection. | Current focused run directly compared success and too-few-arguments failure. | Verified |
| `AsDelegate_2()` | 0x060001de | Returns the two-argument delegate projection. | Current focused run directly compared success and too-few-arguments failure. | Verified |
| `Mpl.Run.UnaryDelegateFunction..ctor(F<object, object>)` | 0x0600020f | Initializes UnaryDelegateFunction with delegate and purity 1. | Direct comparison of constructor invocation. Exact IL match. | Verified |
| `Mpl.Run.UnaryDelegateFunction.inv<R, A>(out R, A)` | 0x06000210 | Invokes m_F with aa and casts to R. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.UnaryDelegateFunction.app<R, A>(out R, A)` | 0x06000211 | Invokes m_F with aa and casts to R. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.UnaryDelegateFunction.DynCall(object)` | 0x06000212 | Invokes m_F with aa. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.UnaryDelegateFunction.Premium()` | 0x06000213 | Wraps m_F with Delegates.Premium_1 and preserves MinArgs and m_no_vectorize. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.BinaryDelegateFunction..ctor(F<object, object, object>)` | 0x06000214 | Initializes BinaryDelegateFunction with delegate and purity 2. | Direct comparison of constructor invocation. Exact IL match. | Verified |
| `Mpl.Run.BinaryDelegateFunction.inv<R, A, B>(out R, A, B)` | 0x06000215 | Invokes m_F with aa, bb and casts to R. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.BinaryDelegateFunction.app<R, A, B>(out R, A, B)` | 0x06000216 | Invokes m_F with aa, bb and casts to R. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.BinaryDelegateFunction.DynCall(object, object)` | 0x06000217 | Invokes m_F with aa, bb. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.BinaryDelegateFunction.Premium()` | 0x06000218 | Wraps m_F with Delegates.Premium_2 and preserves MinArgs and m_no_vectorize. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.TernaryDelegateFunction..ctor(F<object, object, object, object>)` | 0x06000219 | Initializes TernaryDelegateFunction with delegate and purity 3. | Direct comparison of constructor invocation. Exact IL match. | Verified |
| `Mpl.Run.TernaryDelegateFunction.inv<R, A, B, C>(out R, A, B, C)` | 0x0600021a | Invokes m_F with aa, bb, cc and casts to R. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.TernaryDelegateFunction.app<R, A, B, C>(out R, A, B, C)` | 0x0600021b | Invokes m_F with aa, bb, cc and casts to R. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.TernaryDelegateFunction.DynCall(object, object, object)` | 0x0600021c | Invokes m_F with aa, bb, cc. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.TernaryDelegateFunction.Premium()` | 0x0600021d | Wraps m_F with Delegates.Premium_3 and preserves MinArgs and m_no_vectorize. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.QuartnaryDelegateFunction..ctor(F<object, object, object, object, object>)` | 0x0600021e | Initializes QuartnaryDelegateFunction with delegate and purity 4. | Direct comparison of constructor invocation. Exact IL match. | Verified |
| `Mpl.Run.QuartnaryDelegateFunction.inv<R, A, B, C, D>(out R, A, B, C, D)` | 0x0600021f | Invokes m_F with aa, bb, cc, dd and casts to R. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.QuartnaryDelegateFunction.app<R, A, B, C, D>(out R, A, B, C, D)` | 0x06000220 | Invokes m_F with aa, bb, cc, dd and casts to R. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.QuartnaryDelegateFunction.DynCall(object, object, object, object)` | 0x06000221 | Invokes m_F with aa, bb, cc, dd. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.QuartnaryDelegateFunction.Premium()` | 0x06000222 | Wraps m_F with Delegates.Premium_4 and preserves MinArgs and m_no_vectorize. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction..ctor(Function)` | 0x06000238 | Initializes PremiumFunction wrapping inner function. | Direct comparison of constructor invocation. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.get_Name()` | 0x06000239 | Forwards to m_func.Name. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.invVec<R>(out R, object[])` | 0x0600023a | Checks premium then forwards to m_func.invVec. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object)` | 0x0600023b | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object)` | 0x0600023c | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object)` | 0x0600023d | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object)` | 0x0600023e | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object)` | 0x0600023f | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object, object)` | 0x06000240 | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object, object, object)` | 0x06000241 | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object, object, object, object)` | 0x06000242 | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object, object, object, object, object)` | 0x06000243 | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object, object, object, object, object, object)` | 0x06000244 | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object, object, object, object, object, object, object)` | 0x06000245 | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object, object, object, object, object, object, object, object)` | 0x06000246 | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x06000247 | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x06000248 | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x06000249 | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |
| `Mpl.Run.PremiumFunction.DynCall(object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x0600024a | Checks premium then forwards to m_func.DynCall. | Direct comparison. Exact IL match. | Verified |

## Test cases

`Function_formatting_identity_and_inverse_links` compares formatting, canonical names, the actual `DelegateFunction.get_Name` override and both of its branches, string conversion, and inverse linking. `Function_application_optional_padding_and_delegate_projection` compares arity accessors, `get_IsFunction`, `OptFunc`, application, `InvokeVec`, and delegate projections.
`DelegateFunctionDifferentialTests` compares all 31 remaining methods of `DelegateFunction`: 17 generic `inv` overloads (both direct `F<...>` and fallback `DynamicInvoke` paths, matching Release IL opcodes), `invVec`, `app0` (purity 0 and 1), `clo`, `remove_module_suffix`, `.ctor(Delegate, int)`, 5 `make`/`makeF` factory methods, and 4 `AsF` delegate projections (both match and fallback paths). All 32 methods of `DelegateFunction` are verified.
`DelegateSubclassesDifferentialTests` compares all 39 methods of `UnaryDelegateFunction`, `BinaryDelegateFunction`, `TernaryDelegateFunction`, `QuartnaryDelegateFunction`, and `PremiumFunction`: constructors, `inv`, `app`, `DynCall`, `Premium`, `get_Name`, `invVec`, and all 16 `DynCall` overloads. All 39 methods are verified.

## Current focused audit

Run `/tmp/quire-b-function-verify-scratch/function-verify-8da5693-r2` passed Harness 27/27, Mpl 159/159, and the six selected Differential tests; lint was 0, ratchet passed, and the selected slice had zero uncovered lines/branches. The type-wide audit remains incomplete (167 missing, 9 structure mismatches).

Recorded structure differences: `.ctor()` calls `Function(int,int)` in Quire and `FunctionUtilities()` in Mathcad; `.ctor(int,int)` has a field-access difference at instruction 1; base `get_Name()` ends in Quire where Mathcad reads `m_name`; `format_vector` uses interpolated-string handlers rather than `String.Format`; `canonical_name` uses `Concat` where Mathcad uses `String.Format`; `closure<T>` has two conditional branches in Quire and none in Mathcad; `ToString()` uses `Concat` in Quire and `String.Format` in Mathcad. `DelegateFunction.get_Name()` uses `call` where Mathcad uses `callvirt` and has three conditional branches rather than one; its cached and fallback direct cases passed. All 17 `DelegateFunction.inv<...>` overloads, `invVec`, and all 4 `AsF` projections have exact opcode matches against Mathcad in Release builds.

## Open

The tests in this record cover the listed members and their stated branches. Other `Function` dispatch overloads remain outside this unit's direct comparisons.
