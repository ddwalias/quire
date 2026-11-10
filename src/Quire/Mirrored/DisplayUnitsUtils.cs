#nullable disable

using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire;

internal static partial class DisplayUnitsUtils
{
    public static readonly double CmToMmMult = 10.0;

    public static double DipsToDisplayUnits(double numInDips, PageSizeUnits pageSizeUnits)
    {
        double displayUnits;
        if (pageSizeUnits == PageSizeUnits.Inches)
        {
            displayUnits = WpfUtils.DipsToInches(numInDips);
        }
        else
        {
            displayUnits = WpfUtils.DipsToCm(numInDips) * CmToMmMult;
        }

        return displayUnits;
    }
}
