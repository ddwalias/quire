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
                    string text2 = string.Empty;
                    int powerDenominator = unit.PowerDenominator;
                    int powerNumerator = unit.PowerNumerator;
                    if (powerDenominator != 1)
                    {
                        text2 = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", powerNumerator, powerDenominator);
                    }
                    else if (powerNumerator != 1)
                    {
                        text2 = string.Format(CultureInfo.InvariantCulture, "{0}", powerNumerator);
                    }
                    string text3 = string.IsNullOrEmpty(text2) ? symbol : string.Format("{0}^({1})", symbol, text2);
                    text = string.IsNullOrEmpty(text) ? text3 : string.Format("{0}*{1}", text, text3);
                }
            }
        }
        return text;
    }
}
