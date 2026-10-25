#nullable disable
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ToolTipContentExtractor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is ToolTip toolTip))
        {
            return value;
        }
        if (!(toolTip is PrimeToolTip primeToolTip))
        {
            return toolTip.Content;
        }
        return primeToolTip.ContentBody;
    }

    [MirrorVerified("PtcControlsCoreToolTipContentExtractorMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsCoreToolTipContentExtractorMechanicalDifferentialTests")]
    public ToolTipContentExtractor() { }
}
