#nullable disable
using System.Collections.Generic;
using System.Globalization;
using Quire.Calculation.Core;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class UnitMonomialBuilder
{
    [MirrorVerified("PtcControlsIOUnitMonomialBuilderMechanicalDifferentialTests")]
    private readonly Dictionary<string, UnitDescriptor> _descriptors;

    private static UnitMonomialBuilder _instance;

    [MirrorVerified("PtcControlsIOUnitMonomialBuilderMechanicalDifferentialTests")]
    private UnitMonomialBuilder()
    {
        _descriptors = DefaultUnitDescriptorTable.Instance;
    }

    public static UnitMonomialBuilder Instance => _instance ?? (_instance = new UnitMonomialBuilder());

    internal string ConstructUnitMonomial(IEnumerable<Unit> units)
    {
        string text = string.Empty;
        if (units != null)
        {
            foreach (Unit unit in units)
            {
                string symbol = _descriptors[unit.CanonicalName].Symbol;
                if (DebugTools.DevAssert(unit.PowerDenominator != 0, null).Passed)
                {
                    string powerText = string.Empty;
                    int powerDenominator = unit.PowerDenominator;
                    int powerNumerator = unit.PowerNumerator;
                    if (powerDenominator != 1)
                    {
                        powerText = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", powerNumerator, powerDenominator);
                    }
                    else if (powerNumerator != 1)
                    {
                        powerText = string.Format(CultureInfo.InvariantCulture, "{0}", powerNumerator);
                    }
                    string poweredSymbol = string.IsNullOrEmpty(powerText) ? symbol : string.Format("{0}^({1})", symbol, powerText);
                    text = string.IsNullOrEmpty(text) ? poweredSymbol : string.Format("{0}*{1}", text, poweredSymbol);
                }
            }
        }
        return text;
    }
}
