#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls.Core.Excel;

public partial class GetCellValueAtColumnConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IDictionary<string, ExcelCell> dictionary)
        {
            dictionary.TryGetValue((string)parameter, out var cell);
            if (cell == null)
            {
                return null;
            }
            return cell.Value;
        }
        return value;
    }

    [MirrorVerified("PtcControlsCoreExcelGetCellValueAtColumnConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsCoreExcelGetCellValueAtColumnConverterMechanicalDifferentialTests")]
    public GetCellValueAtColumnConverter() { }
}
