#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Xml;

namespace Quire.Controls.Plots;

public partial class PointCollectionXml
{
    public static double Parse(string text)
    {
        if (!(text == "NaN"))
        {
            return double.Parse(text);
        }
        return double.NaN;
    }

    private static void Validate(XmlNode x, string paramName)
    {
        if (x == null)
        {
            throw new ArgumentNullException(paramName);
        }
        if (x.LocalName != "real")
        {
            throw new ArgumentOutOfRangeException(paramName, x.LocalName, "LocalName should be \"real\"");
        }
    }

    public void Add(XmlNode x, XmlNode y)
    {
        Validate(x, "x");
        Validate(y, "y");
        Add(new Point(Parse(x.InnerText), Parse(y.InnerText)));
    }

    public PointCollectionXml()
    {
    }
}
