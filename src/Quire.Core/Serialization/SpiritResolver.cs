#nullable disable
using Quire.Mirror;

namespace Quire.Serialization;

public partial class SpiritResolver
{
    protected override string SchemaName(System.Uri absoluteUri)
    {
        string[] segments = absoluteUri.Segments;
        int num = segments.Length - 1;
        string text = segments[num];
        string text2 = string.Empty;
        if ("EN" == text)
        {
            text = "XMLSchema.dtd";
            num = 0;
        }
        else if ("datatypes.dtd" == text)
        {
            num = 0;
        }
        else if ("Math50.xsd" == text || "units10.xsd" == text || "provenance10.xsd" == text || "worksheet50.xsd" == text || "SimpleTypes.xsd" == text || "calculation10.xsd" == text)
        {
            text2 = "Mathcad";
        }
        if (string.Empty == text2)
        {
            if (num > 0)
            {
                text2 = segments[num - 1].Trim("/".ToCharArray());
            }
            else
            {
                DebugTools.DevAssert(false, "Unknown schema filename - if new schema file was added, add its path here...");
                text2 = "W3C";
            }
        }
        text = string.Format("{0}.{1}", text2, text);
        return text;
    }

    public static void InitializeDocument(System.Xml.XmlDocument xmlDoc, string schemaName)
    {
        Quire.Xml.SchemasManager.SchemaData schemaData = Quire.Xml.SchemasManager.GetSchemaData((Quire.Xml.SchemasManager.SchemaData data) => data.SchemaName == schemaName);
        if (schemaData == null)
        {
            schemaData = new Quire.Xml.SchemasManager.SchemaData(schemaName);
            Quire.Xml.SchemasManager.RuntimeSchemaDatas.Add(schemaData);
        }
        xmlDoc.Schemas = schemaData.CompiledSchemaSet;
    }

    public static string SimpleXpathCreator(string prefix, string ns, params string[] parameters)
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.Append(prefix);
        foreach (string text in parameters)
        {
            stringBuilder.Append(ns + ":" + text + "/");
        }
        stringBuilder.Remove(stringBuilder.ToString().LastIndexOf("/"), 1);
        return stringBuilder.ToString();
    }

    public static System.Xml.XmlNode WorksheetRootNode(System.Xml.XmlNode xmlContentDocument, System.Xml.XmlNamespaceManager nameSpaceManager, string rootNodeName)
    {
        string xpath = "//ws:" + rootNodeName;
        return xmlContentDocument.SelectSingleNode(xpath, nameSpaceManager);
    }

    [MirrorVerified("PtcSerializationSpiritResolverMechanicalDifferentialTests")]
    private SpiritResolver() : base("Ptc.Schemas.{0}") { }

}
