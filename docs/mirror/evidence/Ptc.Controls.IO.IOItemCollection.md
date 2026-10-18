# Ptc.Controls.IO.IOItemCollection

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsIOIOItemCollectionMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetItem(string, bool)` | 0x06000253 | MemberGen: `_aliasManager.GetItem(alias, compareAliasCaseInsensitively)` | Mechanical; Release IL equals Mathcad's | Verified |
| `HandleTaggedRegionAdded(DependencyObject)` | 0x06000255 | MemberGen: `CreateIOItems(region)` | Mechanical; Release IL equals Mathcad's | Verified |
| `AliasChanged add` | 0x0600024f | MemberGen: `public event Action AliasChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `AliasChanged remove` | 0x06000250 | MemberGen: `public event Action AliasChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsIOIOItemCollectionMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
