#nullable disable
using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Quire.Controls.Core.Serialization;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

public partial class ResultsSerializer
{
    private readonly XNamespace nameSpace;

    private readonly NumberFormatSerializer numberFormatSerializer;

    private readonly MatrixFormatSerializer matrixFormatSerializer;

    private readonly UnitSerializer unitSerializer;

    internal const string localName = "results";

    public ResultsSerializer(IEngineeringDocument document, XNamespace nameSpace)
    {
        this.nameSpace = nameSpace;
        numberFormatSerializer = new NumberFormatSerializer(document.MathDisplayNumberFormat, nameSpace.NamespaceName);
        matrixFormatSerializer = new MatrixFormatSerializer(document.MathResultMatrixFormat, nameSpace.NamespaceName);
        unitSerializer = new UnitSerializer(document, nameSpace.NamespaceName);
    }

    [MirrorVerified("PtcControlsCoreSettingsResultsSerializerMechanicalDifferentialTests")]
    public XmlSchema GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        XElement xElement = XElement.Load(reader);
        ReadXml(xElement);
    }

    public void WriteXml(XmlWriter writer)
    {
        XElement xElement = new XElement(nameSpace + "results");
        WriteXml(xElement);
        xElement.WriteTo(writer);
    }

    public void ReadXml(XElement xElement)
    {
        foreach (XElement item in xElement.Elements())
        {
            switch (item.Name.LocalName)
            {
            case "results":
                ReadXml(item);
                break;
            case "decimal":
            case "percent":
            case "general":
            case "engineering":
            case "scientific":
            case "fraction":
            {
                using (XmlReader xmlReader2 = item.CreateReader())
                {
                    xmlReader2.Read();
                    numberFormatSerializer.ReadXml(xmlReader2);
                }
                break;
            }
            case "matrix":
            {
                using (XmlReader xmlReader = item.CreateReader())
                {
                    xmlReader.Read();
                    matrixFormatSerializer.ReadXml(xmlReader);
                }
                break;
            }
            case "unit":
                unitSerializer.ReadXml(xElement);
                break;
            }
        }
    }

    public void WriteXml(XElement xElement)
    {
        XElement xElement2 = new XElement(XName.Get("results", nameSpace.NamespaceName));
        using (XmlWriter writer = xElement2.CreateWriter())
        {
            numberFormatSerializer.WriteXml(writer);
            matrixFormatSerializer.WriteXml(writer);
        }
        unitSerializer.WriteXml(xElement2);
        xElement.Add(xElement2);
    }
}
