#nullable disable
using System.Xml;
using System.Xml;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Xml;

public static partial class XmlWorksheetData
{
    [MirrorVerified("Release IL")]
    public static string BuildId
    {
        get
        {
            if (_buildId == null)
            {
                Quire.AssemblyBuildIdAttribute assemblyBuildIdAttribute = System.Attribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(Quire.AssemblyBuildIdAttribute)) as Quire.AssemblyBuildIdAttribute;
                _buildId = assemblyBuildIdAttribute != null && assemblyBuildIdAttribute.BuildId != null ? assemblyBuildIdAttribute.BuildId : string.Empty;
            }
            return string.IsNullOrEmpty(_buildId) ? null : _buildId;
        }
    }

    [MirrorVerified("Release IL")]
    public static string Stream
    {
        get
        {
            if (_stream == null)
            {
                Quire.AssemblyStreamAttribute assemblyStreamAttribute = System.Attribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(Quire.AssemblyStreamAttribute)) as Quire.AssemblyStreamAttribute;
                _stream = assemblyStreamAttribute != null && assemblyStreamAttribute.Stream != null ? assemblyStreamAttribute.Stream : "Not Specified";
            }
            return _stream;
        }
    }

    [MirrorVerified("Release IL")]
    public static XmlNamespaceManager createMathcad14NamespaceManager(XmlDocument xmlDocument)
    {
        xmlDocument ??= new XmlDocument();
        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
        namespaceManager.AddNamespace(string.Empty, worksheet30);
        namespaceManager.AddNamespace("xsi", xmlSchemaInstance);
        namespaceManager.AddNamespace("ve", markupCompatibility);
        namespaceManager.AddNamespace("r", officeDocumentRelationships);
        namespaceManager.AddNamespace("ws", worksheet30);
        AddMathcad14MLNamespaces(namespaceManager);
        return namespaceManager;
    }

    [MirrorVerified("Release IL")]
    public static XmlNamespaceManager createMcdxNamespaceManager(XmlDocument xmlDocument)
    {
        xmlDocument ??= new XmlDocument();
        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
        namespaceManager.AddNamespace(string.Empty, worksheet50);
        namespaceManager.AddNamespace("xsi", xmlSchemaInstance);
        namespaceManager.AddNamespace("ve", markupCompatibility);
        namespaceManager.AddNamespace("r", officeDocumentRelationships);
        namespaceManager.AddNamespace("ws", worksheet50);
        namespaceManager.AddNamespace("cl", calculation10);
        AddMcdxMLNamespaces(namespaceManager);
        return namespaceManager;
    }

    [MirrorVerified("PtcXmlXmlWorksheetDataMechanicalDifferentialTests")]
    public static XmlElement CreateMathcad14MLElement(XmlDocument document, string localName) => CreateMLElement(document, localName, "http://schemas.mathsoft.com/math30");

    [MirrorVerified("PtcXmlXmlWorksheetDataMechanicalDifferentialTests")]
    public static XmlElement CreateMLElement(XmlDocument document, string localName, string namespaceURI) => document.CreateElement("ml", localName, namespaceURI);

    [MirrorVerified("PtcXmlXmlWorksheetDataMechanicalDifferentialTests")]
    public static XmlElement CreateMcdxMLElement(XmlDocument document, string localName) => CreateMLElement(document, localName, "http://schemas.mathsoft.com/math50");

    [MirrorVerified("Release IL")]
    public static void AddMathcad14MLNamespaces(XmlNamespaceManager nmsmgr)
    {
        nmsmgr.AddNamespace("ml", "http://schemas.mathsoft.com/math30");
        nmsmgr.AddNamespace("u", "http://schemas.mathsoft.com/units10");
        nmsmgr.AddNamespace("p", "http://schemas.mathsoft.com/provenance10");
    }

    [MirrorVerified("Release IL")]
    public static void AddMcdxMLNamespaces(XmlNamespaceManager nmsmgr)
    {
        nmsmgr.AddNamespace("ml", "http://schemas.mathsoft.com/math50");
        nmsmgr.AddNamespace("u", "http://schemas.mathsoft.com/units10");
        nmsmgr.AddNamespace("p", "http://schemas.mathsoft.com/provenance10");
    }
}
