#nullable disable
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Quire.Controls.Core.Serialization;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

internal partial class MathStyles : IXmlSerializable, IXElementSerializable
{
    public const string localName = "mathStyles";

    private readonly IEngineeringDocument document;

    private readonly XNamespace nameSpace;

    private readonly XName MathStyleName;

    public MathStyles(IEngineeringDocument document, XNamespace nameSpace)
    {
        this.document = document;
        this.nameSpace = nameSpace;
        MathStyleName = nameSpace + "mathStyle";
    }

    [MirrorVerified("PtcControlsCoreSettingsMathStylesMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => throw new NotImplementedException();

    public void ReadXml(XmlReader reader)
    {
        XElement xElement = XElement.Load(reader);
        ReadXml(xElement);
    }

    public void WriteXml(XmlWriter writer)
    {
        XElement xElement = new XElement(XName.Get("mathStyles", nameSpace.NamespaceName));
        WriteXml(xElement);
        xElement.WriteTo(writer);
    }

    public void ReadXml(XElement xElement)
    {
        IDictionary<IdLabels, ExtendedFont> dictionary = new Dictionary<IdLabels, ExtendedFont>();
        if (DebugTools.DevAssert(nameSpace.NamespaceName != "http://schemas.mathsoft.com/worksheet30", null).Failed)
        {
            return;
        }
        XElement xElement2 = xElement.Element(XName.Get("mathStyles", nameSpace.NamespaceName));
        if (xElement2 != null)
        {
            foreach (XElement item in xElement2.Elements(MathStyleName))
            {
                MathStyle mathStyle = new MathStyle(nameSpace);
                mathStyle.ReadXml(item);
                dictionary.Add(mathStyle.Label, mathStyle.Font);
            }
        }
        if (dictionary.Count > 0)
        {
            document.LabeledIdFormat = dictionary;
        }
    }

    public void WriteXml(XElement xElement)
    {
        XElement xElement2 = new XElement(XName.Get("mathStyles", nameSpace.NamespaceName));
        foreach (KeyValuePair<IdLabels, ExtendedFont> item in document.LabeledIdFormat)
        {
            new MathStyle(item.Key, item.Value, nameSpace).WriteXml(xElement2);
        }
        xElement.Add(xElement2);
    }
}
