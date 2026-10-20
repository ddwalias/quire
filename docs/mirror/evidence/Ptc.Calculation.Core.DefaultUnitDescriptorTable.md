# Ptc.Calculation.Core.DefaultUnitDescriptorTable

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000008 | Calls `InitializeComponent()` then `InitializeUnitSystems()` | Blocked: `InitializeComponent` loads `/Ptc.Calculation.Core;component/defaultunitdescriptortable.xaml`; Quire has no matching XAML resource | Blocked |
| `InitializeUnitSystems()` | 0x06000009 | Iterates this dictionary and partitions each descriptor into six unit-system tables according to base and derived dimensions | Direct Release cases cover all three systems, both dimensions, and null dimensions | Verified |
| `_CreateDelegate(Type, string)` | 0x06000011 | MemberGen: `Delegate.CreateDelegate(delegateType, this, handler)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SystemSIBase get` | 0x0600000a | MemberGen: `_siBase` | Mechanical; Release IL equals Mathcad's | Verified |
| `SystemSIDerived get` | 0x0600000b | MemberGen: `_siDerived` | Mechanical; Release IL equals Mathcad's | Verified |
| `SystemUSBase get` | 0x0600000c | MemberGen: `_usBase` | Mechanical; Release IL equals Mathcad's | Verified |
| `SystemUSDerived get` | 0x0600000d | MemberGen: `_usDerived` | Mechanical; Release IL equals Mathcad's | Verified |
| `SystemCGSBase get` | 0x0600000e | MemberGen: `_cgsBase` | Mechanical; Release IL equals Mathcad's | Verified |
| `SystemCGSDerived get` | 0x0600000f | MemberGen: `_cgsDerived` | Mechanical; Release IL equals Mathcad's | Verified |
| `InitializeComponent()` | 0x06000010 | Sets `_contentLoaded` then calls `Application.LoadComponent` once for the table XAML resource | Blocked: the WPF component resource is absent from the Quire project | Blocked |
| `IComponentConnector.Connect(int, object)` | 0x06000012 | Sets `_contentLoaded` to true regardless of the connection id or target | Direct Release cases cover null and non-null targets and two ids | Verified |
| `.cctor()` | 0x06000013 | Constructs `Instance` with `new DefaultUnitDescriptorTable()` | Blocked by the constructor's WPF component resource dependency | Blocked |

## Test cases

**Observed** (`PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

**Observed** (`PtcCalculationCoreDefaultUnitDescriptorTableInitializationMechanicalDifferentialTests`, Release with Mathcad required): directly invokes
`InitializeUnitSystems` on paired tables seeded with three descriptors whose base and derived system lists include all systems, subsets, and nulls;
the result compares all six populated maps. It directly invokes the explicit connector method for two connection ids and null/non-null targets.
