# Ptc.Calculation.ValueToIResultNodeTransform

- **Original:** Ptc.Calculation 12.0.1.0, SHA-256 `fe8cf4f1b2791b7a223d203785600667bb00a3e2b30e975a949fe412cbf07e4b`, MVID `82391b36-0d84-45c3-b21a-f7dccf288c68`
- **Reference:** ilspycmd 11.1.0.9782 (`tools/reference/reference.py`)
- **Quire:** `src/Quire.Calculation/ValueToIResultNodeTransform.Mirror.cs`, `src/Quire.Calculation/Generated/Mirror/Calculation.ValueToIResultNodeTransform.g.cs`; tests: `PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---:|---|---|---|
| `.ctor(IResultNodeFactory)` | `0x06000083` | Calls `object::.ctor`, then stores the supplied factory in `_resultNodeFactory`. | Constructor and factory field state are compared directly. | Verified |
| `get_SimplifyUnits` | `0x06000084` | Returns the `SimplifyUnits` backing field. | Compared at default and after both true and false assignments. | Verified |
| `set_SimplifyUnits(bool)` | `0x06000085` | Stores the supplied bool in the backing field. | Both bool values are compared and the value is observed by the next `GetProxy` call. | Verified |
| `Transform(IAutomationValue)` | `0x06000086` | A null value returns `CreateEmptyResultNode`; otherwise delegates to `MakeResultNode(IAutomationValue)`. | Direct null and non-null string proxy cases. Matrix-valued input is not claimed. | Verified |
| `MakeResultNode(IAutomationValue)` | `0x06000087` | A null value returns an empty node; otherwise calls `GetProxy(SimplifyUnits)` and dispatches the returned proxy. | Direct null and non-null cases record the simplify flag passed to `GetProxy`. | Verified |
| `MakeResultNode(IValueProxy)` | `0x06000089` | Selects the first non-null scalar, string, function, matrix, or symbolic subproxy; unrecognized proxies throw `InvalidOperationException` with the reference message. | Compared string, function display, symbolic XML, real scalar and unknown proxy cases. Matrix case is blocked by the `BuildLazyMatrix` dependency below. | Verified for recorded cases |
| `CleanLineBreake(string,string)` | `0x0600008a` | Returns null/empty input unchanged; otherwise replaces CRLF, CR, and LF sequences with the supplied replacement using `Regex.Replace`. | Compared null, empty, plain, and mixed line endings. | Verified |
| `BuildScalar(IScalarValueProxy)` | `0x0600008b` | Creates complex when both parts are nonzero, imaginary when only the imaginary part is nonzero, otherwise real; wraps the result as united when `Units` is nonempty. | Compared real, imaginary, complex, and two-unit real inputs. | Verified |
| `MakeUnitedValue(IResultNode,UnitPower[])` | `0x0600008d` | Creates a united result, assigns `Value`, then adds every unit name and exact rational numerator/denominator in array order. | Compared two units including negative and fractional powers. | Verified |
| `HasUnits(IValueProxy)` | `0x0600008e` | Returns whether `Units.Length` is greater than zero. | Compared empty and two-unit arrays. A null `Units` array still throws as in the original. | Verified |

## Test cases

`$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests'` — 7 passed, 0 failed, 0 skipped. Every selected member is invoked through `MethodComparison.Compare` against Mathcad; dispatch and result-factory interfaces use test-only `DispatchProxy` implementations. The function dispatch cases include wrapped, plain, prefix-only, and suffix-only `ToString()` forms to cover both `StartsWith` and `EndsWith` branches.

## Open

- `MakeResultNode(IValueProxyWrapper)` (`0x06000088`) remains a generated stub because Mathcad calls `ValueProxyWrapper.get_Proxy`, a Tier B generated property accessor. The accessor is not hand-written here.
- `BuildLazyMatrix(IMatrixValueProxy)` (`0x0600008c`) remains a generated stub because its first operation constructs `MatrixValueProxyWrapper`; that class is owned by the active `mirror/c-calc-matvalue-proxy-cctor` worktree. This batch does not edit that class or assert matrix-path equivalence.
