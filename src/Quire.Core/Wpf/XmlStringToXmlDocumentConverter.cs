#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class XmlStringToXmlDocumentConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        string text = value.ToString();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }
        System.Xml.XmlDocument document = new System.Xml.XmlDocument();
        document.LoadXml(text);
        return document;
    }

    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (!(value is System.Xml.XmlDocument document))
        {
            return string.Empty;
        }
        return document.Value;
    }

    [MirrorVerified("PtcWpfXmlStringToXmlDocumentConverterMechanicalDifferentialTests")]
    public XmlStringToXmlDocumentConverter() { }
}
