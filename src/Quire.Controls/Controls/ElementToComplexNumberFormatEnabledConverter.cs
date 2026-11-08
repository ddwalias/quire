#nullable disable
using System;
using System.Globalization;
using System.Windows;
using Quire.Controls.Plots;
using Quire.Controls.Worksheet;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls;

public partial class ElementToComplexNumberFormatEnabledConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return false;
        }
        if (!(value is PlotControl))
        {
            if (value is EquationControl && (value as DependencyObject).GetValue(EquationControl.DisplayNumberFormatProperty) != null)
            {
                return (value as DependencyObject).TryFindAncestorIncludingSelf<IHeaderFooter>() == null;
            }
            return true;
        }
        return false;
    }

    [MirrorVerified("PtcControlsElementToComplexNumberFormatEnabledConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsElementToComplexNumberFormatEnabledConverterMechanicalDifferentialTests")]
    public ElementToComplexNumberFormatEnabledConverter() { }
}
