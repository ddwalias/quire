# Mpl.Run.FunctionUtilities

- **Original:** mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **Reference:** ilspycmd 11.1.0.9782 member index and C# view from `tools/reference/reference.py`.
- **Quire:** [FunctionUtilities.cs](../../../src/Quire.Mpl/Run/FunctionUtilities.cs); tests: `FunctionDifferentialTests`.

`FunctionUtilities` caches reflection handles for delegate closure construction and fixed-arity calls. Its array helpers copy, take, drop, prepend and append arguments. `take` returns the original array when the requested count is at least its length; `but` returns an empty array when the requested drop count is at least the length. `GetCallMethod` selects fixed-arity dispatch through 16 arguments and `Call_vec` above that boundary.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `CloCtor(System.Type)` | 0x06000276 | Closes the cached generic closure factory with the supplied type. | Current focused run directly compared `int`; static initialization remains implicit. | Verified |
| `InstMethCtor(System.Type)` | 0x06000277 | Returns the cached instance-method wrapper constructor. | Current focused run directly compared the result; the type argument does not affect it. | Verified |
| `GetCallMethod(System.Type, System.Type[])` | 0x06000278 | Selects fixed-arity `Call_N` through 16 arguments and `Call_vec` above 16. | Current focused run directly compared arities 1 and 17. | Verified |
| `concat(object[], object[])` | 0x06000279 | Allocates and copies the two arrays in order. | Current focused run directly compared nonempty and empty second arrays. | Verified |
| `but(int, !!0[])` | 0x0600027a | Copies the suffix when the drop count is less than the source length; otherwise returns an empty array. | 10 direct comparisons in `FunctionUtilitiesButDifferentialTests` covering negative counts, boundaries, null inputs, and array identity. | Verified |
| `take(int, !!0[])` | 0x0600027b | Copies a prefix when the count is less than the source length; otherwise returns the original array. | 7 direct comparisons in `FunctionUtilitiesTakeNegativeDifferentialTests` covering negative counts, null inputs, boundaries, and array identity. | Verified |
| `PushFront(object, object[])` | 0x0600027c | Allocates an array with one leading value and copies the source after it. | Current focused run directly compared a two-element source. | Verified |
| `PushFront(object, object, object[])` | 0x0600027d | Allocates an array with two leading values and copies the source after them. | Current focused run directly compared a two-element source. | Verified |
| `Push(object[], object)` | 0x0600027e | Copies the source then appends one value. | Current focused run directly compared a two-element source. | Verified |
| `Push(object[], object, object)` | 0x0600027f | Copies the source then appends two values. | Current focused run directly compared a two-element source. | Verified |
| `.ctor()` | 0x06000280 | Protected base constructor with no additional initialization. | Reflection cannot directly invoke a constructor whose declaring type is abstract; a derived constructor does not count as direct invocation. | Analyzed |

## Test cases

`FunctionUtilities_array_operations_and_caches` directly compares the callable listed methods with the same mplrun member. Inputs cover the copy and non-copy branches for `take`, both branches for `but`, empty/nonempty concatenation, all push overloads, and `GetCallMethod` on either side of the fixed-arity limit.

Current focused run `/tmp/quire-b-function-verify-scratch/function-verify-8da5693-r2` passed Harness 27/27, Mpl 159/159, and six selected Differential tests, with lint 0, ratchet ok, and zero uncovered lines/branches. The protected abstract constructor remains Analyzed because it cannot be directly invoked.

## Open

The protected constructor remains Analyzed: direct reflection invocation is impossible because `FunctionUtilities` is abstract. The type initializer is observed through the methods above.
