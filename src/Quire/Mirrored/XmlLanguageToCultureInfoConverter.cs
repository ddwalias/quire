using System;
using System.Globalization;
using System.Windows.Markup;

namespace Quire;

public partial class XmlLanguageToCultureInfoConverter
{
    public static XmlLanguageToCultureInfoConverter Instance = new XmlLanguageToCultureInfoConverter();
    private XmlLanguageToCultureInfoConverter() { }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return global::Quire.Controls.Text.XmlLanguageHelper.ConvertXmlLanguageToCultureInfo(value as XmlLanguage);
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
