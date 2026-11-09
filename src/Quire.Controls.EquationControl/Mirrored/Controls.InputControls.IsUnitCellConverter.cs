using System;
using System.Globalization;

namespace Quire.Controls.InputControls;

public partial class IsUnitCellConverter
{
    public static IsUnitCellConverter Instance = new IsUnitCellConverter();

    private IsUnitCellConverter()
    {
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values[0] is int row && values[1] is int column && EditableComboBoxViewModel.IsUnitCell(row, column);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
