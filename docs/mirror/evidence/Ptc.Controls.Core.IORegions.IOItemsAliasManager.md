# Ptc.Controls.Core.IORegions.IOItemsAliasManager

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `SetInputAliases(DependencyObject, List)` | 0x06000e70 | MemberGen: `SetAliases(element, aliases, true)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetOutputAliases(DependencyObject, List)` | 0x06000e71 | MemberGen: `SetAliases(element, aliases, false)` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsExist(string)` | 0x06000e72 | MemberGen: `_aliasesToItemsMap.ContainsKey(alias)` | Mechanical; Release IL equals Mathcad's | Verified |
| `InAliasMapChanged add` | 0x06000e62 | MemberGen: `public static event Action<DependencyObject, DependencyPropertyChangedEventArgs> InAliasMapChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InAliasMapChanged remove` | 0x06000e63 | MemberGen: `public static event Action<DependencyObject, DependencyPropertyChangedEventArgs> InAliasMapChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `OutAliasMapChanged add` | 0x06000e64 | MemberGen: `public static event Action<DependencyObject, DependencyPropertyChangedEventArgs> OutAliasMapChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `OutAliasMapChanged remove` | 0x06000e65 | MemberGen: `public static event Action<DependencyObject, DependencyPropertyChangedEventArgs> OutAliasMapChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
