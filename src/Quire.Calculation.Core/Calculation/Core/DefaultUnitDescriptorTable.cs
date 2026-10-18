#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class DefaultUnitDescriptorTable
{
    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, this, handler);

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    private UnitDescriptorTable _siBase;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    public UnitDescriptorTable SystemSIBase => _siBase;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    private UnitDescriptorTable _siDerived;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    public UnitDescriptorTable SystemSIDerived => _siDerived;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    private UnitDescriptorTable _usBase;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    public UnitDescriptorTable SystemUSBase => _usBase;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    private UnitDescriptorTable _usDerived;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    public UnitDescriptorTable SystemUSDerived => _usDerived;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    private UnitDescriptorTable _cgsBase;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    public UnitDescriptorTable SystemCGSBase => _cgsBase;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    private UnitDescriptorTable _cgsDerived;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    public UnitDescriptorTable SystemCGSDerived => _cgsDerived;
}
