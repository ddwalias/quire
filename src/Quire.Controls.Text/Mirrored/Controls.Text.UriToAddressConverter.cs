#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows.Data;
using Quire.IO;

namespace Quire.Controls.Text;

public partial class UriToAddressConverter : IValueConverter
{
    public static UriToAddressConverter Instance = new UriToAddressConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        UrlUtils.GetStringAddressFromUri(value as Uri) ?? string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    public UriToAddressConverter()
    {
    }
}
