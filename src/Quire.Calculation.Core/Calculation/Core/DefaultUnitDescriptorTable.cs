#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class DefaultUnitDescriptorTable
{
    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableInitializationMechanicalDifferentialTests")]
    private void InitializeUnitSystems()
    {
        using Dictionary<string, UnitDescriptor>.Enumerator enumerator = GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, UnitDescriptor> current = enumerator.Current;
            if (current.Value.BaseDimensions != null)
            {
                if (current.Value.BaseDimensions.Contains(UnitSystems.SI))
                {
                    _siBase[current.Key] = current.Value;
                }
                if (current.Value.BaseDimensions.Contains(UnitSystems.US))
                {
                    _usBase[current.Key] = current.Value;
                }
                if (current.Value.BaseDimensions.Contains(UnitSystems.CGS))
                {
                    _cgsBase[current.Key] = current.Value;
                }
            }
            if (current.Value.DerivedDimensions != null)
            {
                if (current.Value.DerivedDimensions.Contains(UnitSystems.SI))
                {
                    _siDerived[current.Key] = current.Value;
                }
                if (current.Value.DerivedDimensions.Contains(UnitSystems.US))
                {
                    _usDerived[current.Key] = current.Value;
                }
                if (current.Value.DerivedDimensions.Contains(UnitSystems.CGS))
                {
                    _cgsDerived[current.Key] = current.Value;
                }
            }
        }
    }

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableInitializationMechanicalDifferentialTests")]
    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) => _contentLoaded = true;

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
