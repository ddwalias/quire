#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class DefaultUnitDescriptorTable
{
    public static readonly DefaultUnitDescriptorTable Instance = new DefaultUnitDescriptorTable();

    private UnitDescriptorTable _siBase = new UnitDescriptorTable();
    private UnitDescriptorTable _siDerived = new UnitDescriptorTable();
    private UnitDescriptorTable _usBase = new UnitDescriptorTable();
    private UnitDescriptorTable _usDerived = new UnitDescriptorTable();
    private UnitDescriptorTable _cgsBase = new UnitDescriptorTable();
    private UnitDescriptorTable _cgsDerived = new UnitDescriptorTable();

    private bool _contentLoaded;

    public DefaultUnitDescriptorTable()
    {
        InitializeComponent();
        InitializeUnitSystems();
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Ptc.Calculation.Core;component/defaultunitdescriptortable.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

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
    public UnitDescriptorTable SystemSIBase => _siBase;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    public UnitDescriptorTable SystemSIDerived => _siDerived;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    public UnitDescriptorTable SystemUSBase => _usBase;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    public UnitDescriptorTable SystemUSDerived => _usDerived;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    public UnitDescriptorTable SystemCGSBase => _cgsBase;

    [MirrorVerified("PtcCalculationCoreDefaultUnitDescriptorTableMechanicalDifferentialTests")]
    public UnitDescriptorTable SystemCGSDerived => _cgsDerived;
}
