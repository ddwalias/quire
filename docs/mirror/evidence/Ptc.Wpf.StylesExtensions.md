# Ptc.Wpf.StylesExtensions

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** ILSpy 11.1.0.9782 C# and IL from `.mirror-cache/Ptc.Core/d60141dbcf757ae6/Ptc.Wpf.StylesExtensions.{cs,il}`; private predicate body also inspected.
- **Quire:** `src/Quire.Core/Wpf/StylesExtensions.cs`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetSetterValue(Style, DependencyProperty)` | `0x060005a8` | Enumerate `style.Setters` as `Setter`, select the last setter whose `Property == property`, and return its `Value`; return null when there is no match. The predicate compares dependency properties with `==`. | Direct Mathcad comparison: last matching setter wins; absent property returns null. Release IL opcode sequence, including the generated predicate, matches Mathcad. Null-style behavior was not exercised. | Verified for recorded cases |

## Test cases

`PtcWpfStylesExtensionsTierADifferentialTests.GetSetterValue_060005a8` directly invokes both artifacts for a style with two setters for the same property and for a property with no setter. The comparison checks each result against Mathcad Ptc.Core.
