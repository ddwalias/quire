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

## Test cases

**Observed** (`PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
