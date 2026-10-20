# Ptc.Calculation.ValueProxyWrapper

- **Original:** `Ptc.Calculation.dll` 12.0.1.0, SHA-256 `fe8cf4f1b2791b7a223d203785600667bb00a3e2b30e975a949fe412cbf07e4b`, MVID `82391b36-0d84-45c3-b21a-f7dccf288c68`
- **Reference:** ilspycmd 11.1.0.9782; bodies reconstructed from the bound IL
- **Quire:** `src/Quire.Calculation`; tests: `Quire.Differential.Tests.PtcCalculationValueProxyWrapperMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Proxy get` | 0x0600006a | Return the `<Proxy>k__BackingField` value | Direct cases compare null and opaque interface proxies | Verified |
| `Proxy set` | 0x0600006b | Store the supplied proxy in `<Proxy>k__BackingField` | Private setter; direct cases compare null and opaque interface proxies and target state | Verified |
| `.ctor(IValueProxy)` | 0x0600006c | Call `object..ctor`, then call `set_Proxy(proxy)` | Direct cases compare null and opaque interface proxies; constructor result state is observed | Verified |

## Test cases

**Observed** (`PtcCalculationValueProxyWrapperMechanicalDifferentialTests`, Release with Mathcad required): direct paired `MethodComparison.Compare`
cases cover the constructor, getter, and private setter with both null and opaque `IValueProxy` instances. Target state is compared before and after
instance methods. The original interface methods themselves are not invoked by the proxy cases.
