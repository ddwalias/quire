using System;
using System.Globalization;
using System.Windows.Data;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial class DraftModeGutterWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return WorksheetPageLayout.DraftModeGutterWidthInDips;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
