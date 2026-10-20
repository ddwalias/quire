#nullable disable
using System.Text.RegularExpressions;
using Quire;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class UnitDescriptor
{
    private string _rl;
    private string _key;

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

    public string SymbolResultNode
    {
        get
        {
            if (IsSymbolResultNode)
            {
                return RL;
            }
            return Symbol;
        }
    }

    public string Key
    {
        get
        {
            if (_key.IsNullOrEmpty())
            {
                return CanonicalName;
            }
            return _key;
        }
        set
        {
            _key = value;
        }
    }

    public string RL
    {
        get
        {
            if (_rl.IsNullOrEmpty())
            {
                return Symbol;
            }
            return _rl;
        }
        set
        {
            _rl = Regex.Unescape(value);
        }
    }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public int CompareTo(object obj) => ToString().CompareTo(obj.ToString());

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public string FullName => DisplayName + " (" + Symbol + ")";

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    private void SetUnitSystem(ref List<UnitSystems> unitSystems, UnitSystems value)
    {
        if (unitSystems == null)
        {
            unitSystems = new List<UnitSystems> { value };
        }
        else
        {
            unitSystems.Add(value);
        }
    }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public UnitSystems DerivedUnitSystem { set => SetUnitSystem(ref _derivedUnitSystems, value); }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public UnitSystems DerivedUnitSystem1 { set => SetUnitSystem(ref _derivedUnitSystems, value); }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public UnitSystems DerivedUnitSystem2 { set => SetUnitSystem(ref _derivedUnitSystems, value); }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public UnitSystems BaseUnitSystem { set => SetUnitSystem(ref _baseUnitSystems, value); }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public UnitSystems BaseUnitSystem1 { set => SetUnitSystem(ref _baseUnitSystems, value); }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public UnitSystems BaseUnitSystem2 { set => SetUnitSystem(ref _baseUnitSystems, value); }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public UnitSystems ExcludeUnitSystem { set => SetUnitSystem(ref _excludeUnitSystems, value); }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public UnitSystems ExcludeUnitSystem1 { set => SetUnitSystem(ref _excludeUnitSystems, value); }

    [MirrorVerified("PtcCalculationCoreUnitDescriptorMechanicalDifferentialTests")]
    public override string ToString() => Category + " - " + DisplayName + " (" + Symbol + ")";
}
