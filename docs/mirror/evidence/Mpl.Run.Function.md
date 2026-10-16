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
| `closure<CLO>(Delegate,CLO,int)` | 0x060001a3 | Returns specialized closure wrappers for purities 2–5; all other purities construct `Fn_clo<CLO>` with the supplied delegate, captured value, and purity. | Original IL SHA-256 `46e4a99f5e55c1802db02473bce3befda14a3975de4da8f96ebea6dc79cf2aba`; 14 direct comparisons cover purities 1–6 (each specialized arm and both default-boundary examples), a non-`F<>` delegate at purity 2, and downstream `Apply`; `CLO` is a reference object, and both side-specific wrappers are checked for retaining its input by reference. Original `switch` and Quire conditional branches differ structurally; see below. The original caller `DelegateFunction.clo<CLO>` is outside this direct comparison. | Analyzed |
| `IgnoreVectorization(bool)` | 0x06000155 | Stores whether matrix vectorization should be bypassed. | Current focused run directly compared the matrix-call tests. | Verified |
| `OptFunc(int)` | 0x06000156 | Sets the minimum arity used by optional-argument padding. | Current focused run directly compared this setter and optional-argument application. | Verified |
| `DeclareInverse(Mpl.Run.Function)` | 0x06000159 | Sets reciprocal inverse-function references and returns the supplied inverse. | Current focused run directly compared separate corresponding instances; target state uses a recursion limit. | Verified |
| `Mpl.Run.DelegateFunction.get_Name()` | 0x060001f7 | Returns the explicitly stored name when present; otherwise canonicalizes the wrapped delegate method name. | Current focused run directly compared cached and fallback paths. IL uses `call` versus `callvirt` and different branch counts; recorded below. | Verified |
| `IsFullyApplied(int)` | 0x060001a6 | Tests whether an argument count lies within the allowed arity interval. | Current focused run directly compared counts below, at, within and above the range. | Verified |
| `AssertFullApp(int)` | 0x060001a7 | Returns normally for an accepted count; otherwise reports the minimum or maximum allowed count through `BadCall`. | Current focused run directly compared success and both failure sides. | Verified |
| `Apply(object[])` | 0x060001d3 | Applies the supplied vector through the function's vector dispatch. | Current focused run directly compared optional padding and exact arity. | Verified |
| `InvokeVec(object[])` | 0x060001d4 | Delegates the supplied vector to inherited `DimVal.DynApply`. | Direct comparison on a two-argument delegate function; source structure follows the original inherited call. | Verified |
| `VecApply(object[])` | 0x060001d0 | Applies vectorization or bypasses it when vectorization is ignored. | Current focused run directly compared vectorized and ignored-vectorization paths. | Verified |
| `AsDelegate_1()` | 0x060001dd | Returns the one-argument delegate projection. | Current focused run directly compared success and too-few-arguments failure. | Verified |
| `AsDelegate_2()` | 0x060001de | Returns the two-argument delegate projection. | Current focused run directly compared success and too-few-arguments failure. | Verified |

## Test cases

`Function_formatting_identity_and_inverse_links` compares formatting, canonical names, the actual `DelegateFunction.get_Name` override and both of its branches, string conversion, and inverse linking. `Function_application_optional_padding_and_delegate_projection` compares arity accessors, `get_IsFunction`, `OptFunc`, application, `InvokeVec`, and delegate projections.

## Historical focused audit

Earlier run `/tmp/quire-b-function-verify-scratch/function-verify-8da5693-r2` passed Harness 27/27, Mpl 159/159, and six selected Differential tests; lint was 0, ratchet passed, and the selected slice had zero uncovered lines/branches. Its type-wide inventory (167 missing, 9 structure mismatches) is historical; the newer scoped Function receipt and remaining gaps are recorded in `docs/mirror/work/lane-b.md`.

Recorded structure differences: `.ctor()` calls `Function(int,int)` in Quire and `FunctionUtilities()` in Mathcad; `.ctor(int,int)` has a field-access difference at instruction 1; base `get_Name()` ends in Quire where Mathcad reads `m_name`; `format_vector` uses interpolated-string handlers rather than `String.Format`; `canonical_name` uses `Concat` where Mathcad uses `String.Format`; `closure<T>` has two conditional branches in Quire, while Mathcad uses a switch; `ToString()` uses `Concat` in Quire and `String.Format` in Mathcad. Direct closure cases cover purities 1–6, every specialized branch and the default branch at both tested bounds; the p=2 helper fallback and resulting application are also compared. `DelegateFunction.get_Name()` uses `call` where Mathcad uses `callvirt` and has three conditional branches rather than one; its cached and fallback direct cases passed. `DelegateFunction.inv<T>` calls `Array.Empty<object>` and unboxes in Quire, while Mathcad calls `Delegate.DynamicInvoke` and allocates an object array; direct cases cover this dispatch difference. Direct cases for `format_vector`, `canonical_name`, `closure<T>`, `ToString`, `DelegateFunction.get_Name`, and `DelegateFunction.inv<T>` passed. Constructor and base getter differences have no direct cases in this unit and remain Analyzed/outside this comparison set. The closure method remains Analyzed due to the recorded control-flow structure difference; no deviation or acceptance exception is proposed.

## Open

The tests in this record cover the listed members and their stated branches. Other `Function` dispatch overloads remain outside this unit's direct comparisons.
