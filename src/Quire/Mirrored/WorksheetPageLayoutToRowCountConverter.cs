#nullable disable

using System;
using System.Globalization;
using Quire.Controls.Core;

namespace Quire;

internal partial class WorksheetPageLayoutToRowCountConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is WorksheetPageLayout worksheetPageLayout)
        {
            return worksheetPageLayout.LinesPerPage;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
