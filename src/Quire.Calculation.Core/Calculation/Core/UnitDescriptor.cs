#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class UnitDescriptor
{
    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public UnitDescriptor() { }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public string CanonicalName { get; set; }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public string Symbol { get; set; }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public string Quantity { get; set; }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public string Category { get; set; }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public string DisplayName { get; set; }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    private List<UnitSystems> _baseUnitSystems;

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public List<UnitSystems> BaseDimensions => _baseUnitSystems;

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    private List<UnitSystems> _derivedUnitSystems;

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public List<UnitSystems> DerivedDimensions => _derivedUnitSystems;

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    private List<UnitSystems> _excludeUnitSystems;

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public List<UnitSystems> ExcludeUnitSystems => _excludeUnitSystems;

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public bool IsFunction { get; set; }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public bool IsSymbolResultNode { get; set; }
}
