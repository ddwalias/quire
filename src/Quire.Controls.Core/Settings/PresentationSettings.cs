#nullable disable
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Quire.Controls.Core.Serialization;
using Quire.Serialization;

namespace Quire.Controls.Core.Settings;

public abstract partial class PresentationSettings : IXmlAndXElementSerializable, IXmlSerializable, IXElementSerializable, IXDocumentRepairer, IShouldValidate
{
    public const string pageModelName = "pageModel";

    public const string textStyleName = "textStyle";

    protected internal readonly IEngineeringDocument document;

    public static string localName = "presentation";

    protected readonly string _namespaceName;

    private readonly string _resultsNS;

    protected internal XElementSerializable pageModel;

    private readonly MathRendering mathRendering;

    protected internal XName PageModelName => GetXName("pageModel");

    private XName MathRenderingName => GetXName("mathRendering");

    public bool ShouldValidateOnWrite => true;

    public bool ShouldValidateOnRead => true;

    protected internal PresentationSettings(IEngineeringDocument document, string namespaceName, string resultsNS)
    {
        this.document = document;
        _namespaceName = namespaceName;
        _resultsNS = resultsNS;
        mathRendering = new MathRendering(document, MathRenderingName, resultsNS);
        pageModel = document.GetMcdxPageModel(PageModelName);
    }

    public abstract XmlSchema GetSchema();

    private XName GetXName(string name)
    {
        return XName.Get(name, _namespaceName);
    }

    public void ReadXml(XmlReader reader)
    {
        XElement xElement = XElement.Load(reader);
        ReadXml(xElement);
    }

    public void WriteXml(XmlWriter writer)
    {
        CreateXElement().Save(writer);
    }

    private XElement CreateXElement()
    {
        XElement xElement = new XElement(GetXName(localName));
        if (!_namespaceName.Equals(_resultsNS))
        {
            xElement.Add(new XAttribute(XName.Get("ws", "http://www.w3.org/2000/xmlns/"), _resultsNS));
        }
        WriteXml(xElement);
        return xElement;
    }

    protected static void OnSchemaValidation(object sender, ValidationEventArgs e)
    {
    }

    public virtual void ReadXml(XElement xElement)
    {
        pageModel.ReadXml(xElement);
        mathRendering.ReadXml(xElement);
    }

    public virtual void WriteXml(XElement xElement)
    {
        pageModel.WriteXml(xElement);
        mathRendering.WriteXml(xElement);
    }

    public void WriteXml(XDocument xDocument)
    {
        xDocument.Add(CreateXElement());
    }

    public void RepairXDocument(XDocument xDocument)
    {
        Presentation10Repairer.RepairXDocument(xDocument);
    }
}
