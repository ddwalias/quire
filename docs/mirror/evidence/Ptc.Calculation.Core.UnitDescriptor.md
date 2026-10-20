# Ptc.Calculation.Core.UnitDescriptor

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000222 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CanonicalName get` | 0x06000200 | MemberGen: `public string CanonicalName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CanonicalName set` | 0x06000201 | MemberGen: `public string CanonicalName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Symbol get` | 0x06000202 | MemberGen: `public string Symbol { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Symbol set` | 0x06000203 | MemberGen: `public string Symbol { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Quantity get` | 0x06000205 | MemberGen: `public string Quantity { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Quantity set` | 0x06000206 | MemberGen: `public string Quantity { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Category get` | 0x06000207 | MemberGen: `public string Category { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Category set` | 0x06000208 | MemberGen: `public string Category { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DisplayName get` | 0x06000209 | MemberGen: `public string DisplayName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DisplayName set` | 0x0600020a | MemberGen: `public string DisplayName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BaseDimensions get` | 0x06000219 | MemberGen: `_baseUnitSystems` | Mechanical; Release IL equals Mathcad's | Verified |
| `DerivedDimensions get` | 0x0600021a | MemberGen: `_derivedUnitSystems` | Mechanical; Release IL equals Mathcad's | Verified |
| `ExcludeUnitSystems get` | 0x0600021b | MemberGen: `_excludeUnitSystems` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsFunction get` | 0x0600021d | MemberGen: `public bool IsFunction { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsFunction set` | 0x0600021e | MemberGen: `public bool IsFunction { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsSymbolResultNode get` | 0x0600021f | MemberGen: `public bool IsSymbolResultNode { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsSymbolResultNode set` | 0x06000220 | MemberGen: `public bool IsSymbolResultNode { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CompareTo(object)` | 0x06000221 | MemberGen: `ToString().CompareTo(obj.ToString())` | Mechanical; Release IL equals Mathcad's | Verified |
| `FullName get` | 0x0600020d | MemberGen: `DisplayName + " (" + Symbol + ")"` | Mechanical; Release IL equals Mathcad's | Verified |
| `SymbolResultNode get` | 0x06000204 | Return `RL` when `IsSymbolResultNode`, otherwise `Symbol` | Blocked while the `RL` property remains dependent on an unimplemented external helper | Blocked |
| `Key get` | 0x0600020b | Return `CanonicalName` when `_key` is null or empty, otherwise `_key` | Calls `Ptc.EnumerableExtensions.IsNullOrEmpty<char>`, a Tier A Quire.Core member outside Lane C that is still a stub | Blocked |
| `Key set` | 0x0600020c | Store the value in `_key` | Kept with the generated property stub because its getter dependency prevents replacing the property pair safely in this batch | Blocked |
| `RL get` | 0x0600020e | Return `Symbol` when `_rl` is null or empty, otherwise `_rl` | Calls `Ptc.EnumerableExtensions.IsNullOrEmpty<char>`, a Tier A Quire.Core member outside Lane C that is still a stub | Blocked |
| `RL set` | 0x0600020f | Store `Regex.Unescape(value)` in `_rl` | Kept with the generated property stub because its getter dependency prevents replacing the property pair safely in this batch | Blocked |
| `SetUnitSystem(ref List<UnitSystems>, UnitSystems)` | 0x06000210 | Create a one-item list when null; otherwise append the value | Direct Mathcad comparison; Release IL equals Mathcad's | Verified |
| `DerivedUnitSystem set` | 0x06000211 | Append to `_derivedUnitSystems` through `SetUnitSystem` | Direct Mathcad comparison; Release IL equals Mathcad's | Verified |
| `DerivedUnitSystem1 set` | 0x06000212 | Append to `_derivedUnitSystems` through `SetUnitSystem` | Direct Mathcad comparison; Release IL equals Mathcad's | Verified |
| `DerivedUnitSystem2 set` | 0x06000213 | Append to `_derivedUnitSystems` through `SetUnitSystem` | Direct Mathcad comparison; Release IL equals Mathcad's | Verified |
| `BaseUnitSystem set` | 0x06000214 | Append to `_baseUnitSystems` through `SetUnitSystem` | Direct Mathcad comparison; Release IL equals Mathcad's | Verified |
| `BaseUnitSystem1 set` | 0x06000215 | Append to `_baseUnitSystems` through `SetUnitSystem` | Direct Mathcad comparison; Release IL equals Mathcad's | Verified |
| `BaseUnitSystem2 set` | 0x06000216 | Append to `_baseUnitSystems` through `SetUnitSystem` | Direct Mathcad comparison; Release IL equals Mathcad's | Verified |
| `ExcludeUnitSystem set` | 0x06000217 | Append to `_excludeUnitSystems` through `SetUnitSystem` | Direct Mathcad comparison; Release IL equals Mathcad's | Verified |
| `ExcludeUnitSystem1 set` | 0x06000218 | Append to `_excludeUnitSystems` through `SetUnitSystem` | Direct Mathcad comparison; Release IL equals Mathcad's | Verified |
| `ToString()` | 0x0600021c | Concatenate `Category`, `" - "`, `DisplayName`, `" ("`, `Symbol`, and `")"` | Direct Mathcad comparison; preserves null concatenation semantics | Verified |

## Test cases

**Observed** (`PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

The `Key` and `RL` property pairs remain generated stubs until the external `Ptc.EnumerableExtensions.IsNullOrEmpty<char>` dependency is implemented; `SymbolResultNode` depends on `RL` for its true branch. No out-of-scope helper or Tier B member was implemented.

MemberGen members are compared by `Quire.Differential.Tests.PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests`.
