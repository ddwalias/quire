# Ptc.Wpf.RoutedUICommandWithFunctionalityParameterKey

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcWpfRoutedUICommandWithFunctionalityParameterKeyMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600059c | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PrimaryParameterToFunctionalityKeyDictionary get` | 0x0600059a | MemberGen: `protected Dictionary<object, EnumFunctionalityKey> PrimaryParameterToFunctionalityKeyDictionary { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PrimaryParameterToFunctionalityKeyDictionary set` | 0x0600059b | MemberGen: `protected Dictionary<object, EnumFunctionalityKey> PrimaryParameterToFunctionalityKeyDictionary { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcWpfRoutedUICommandWithFunctionalityParameterKeyMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
