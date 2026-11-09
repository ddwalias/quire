using System;
using System.Globalization;
using System.Windows.Data;
using System.Xml;

namespace Quire.Controls;

public partial class SymbolDescriptorConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        XmlNode xmlNode = (XmlNode)value;
        SymbolDescriptor symbolDescriptor = new SymbolDescriptor();
        XmlAttribute xmlAttribute = xmlNode.Attributes["DisplayName"];
        if (xmlAttribute != null)
        {
            symbolDescriptor.DisplayName = xmlAttribute.Value;
        }

        xmlAttribute = xmlNode.Attributes["SymbolicRepresentation"];
        if (xmlAttribute != null)
        {
            symbolDescriptor.SymbolicRepresentation = xmlAttribute.Value;
        }

        XmlNode description = xmlNode.SelectSingleNode("description");
        if (description != null)
        {
            symbolDescriptor.Description = description.InnerText;
        }

        return symbolDescriptor;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
