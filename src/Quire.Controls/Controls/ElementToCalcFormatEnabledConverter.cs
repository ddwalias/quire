#nullable disable
using System;
using System.Globalization;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots;
using Quire.Mirror;

namespace Quire.Controls;

public partial class ElementToCalcFormatEnabledConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return false;
        }
        if (value is ICalculatable || value is PlotControl || value is SpecTableControl || (value is IRegionCollectionWithProperty && (value as RegionCollectionWithProperty).AreAllMathRegions))
        {
            return true;
        }
        return false;
    }

    [MirrorVerified("PtcControlsElementToCalcFormatEnabledConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsElementToCalcFormatEnabledConverterMechanicalDifferentialTests")]
    public ElementToCalcFormatEnabledConverter() { }
}
