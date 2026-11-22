using System;
using System.Globalization;
using Quire.Controls.CollapsibleArea;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial class CollapsibleAreaHeightConverter
{
    public CollapsibleAreaHeightConverter()
    {
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        WorksheetPageLayout worksheetPageLayout = values[0] as WorksheetPageLayout;
        if (!(values[1] is CollapsibleAreaControl collapsibleAreaControl))
        {
            return 0;
        }

        if (worksheetPageLayout == null)
        {
            return collapsibleAreaControl.Height;
        }

        double height = worksheetPageLayout.GridCellSize.Height;
        double height2 = collapsibleAreaControl.Height;
        double num = height2 % height;
        if (num.IsEqualTo(0.0) || num.IsEqualTo(height))
        {
            return height2;
        }

        return height2 + (height - num);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
