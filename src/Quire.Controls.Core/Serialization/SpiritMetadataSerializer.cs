#nullable disable
using System.Globalization;
using System.Xml;
using Quire.Serialization;

namespace Quire.Controls.Core.Serialization;

public partial class SpiritMetadataSerializer : MetadataSerializer
{
    private const string schemaNamespaceURI = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";

    protected override void CreateSchemas(XmlDocument xmlMetadata)
    {
        SpiritResolver.InitializeDocument(xmlMetadata, "OpenPackagingConventions_XMLSchema.opc-coreProperties.xsd");
    }

    protected override void FillInMetadataModel(XmlDocument xmlMetadata, IMetadataModel metadataModel)
    {
        XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlMetadata.NameTable);
        xmlNamespaceManager.AddNamespace("cp", "http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
        xmlNamespaceManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
        xmlNamespaceManager.AddNamespace("dcterms", "http://purl.org/dc/terms/");
        xmlNamespaceManager.AddNamespace("dcmitype", "http://purl.org/dc/dcmitype/");
        xmlNamespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        foreach (XmlNode item in xmlMetadata.SelectNodes("/cp:coreProperties/*", xmlNamespaceManager))
        {
            string name = item.Name;
            string innerText = item.InnerText;
            switch (name)
            {
                case "dc:creator":
                    metadataModel.Creator = innerText;
                    break;
                case "cp:lastModifiedBy":
                    break;
                case "cp:revision":
                    metadataModel.Version = int.Parse(innerText, CultureInfo.InvariantCulture);
                    break;
            }
        }
    }

    protected override void FillInXmlMetadata(XmlDocument xmlMetadata, IMetadataModel metadataModel)
    {
        XmlNode newChild = xmlMetadata.CreateNode(XmlNodeType.Element, "cp", "coreProperties", "http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
        newChild = xmlMetadata.AppendChild(newChild);
        XmlNode xmlNode = xmlMetadata.CreateNode(XmlNodeType.Element, "dc", "creator", "http://purl.org/dc/elements/1.1/");
        xmlNode.InnerText = metadataModel.Creator;
        newChild.AppendChild(xmlNode);
        xmlNode = xmlMetadata.CreateNode(XmlNodeType.Element, "cp", "lastModifiedBy", "http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
        xmlNode.InnerText = metadataModel.LastModified;
        newChild.AppendChild(xmlNode);
        xmlNode = xmlMetadata.CreateNode(XmlNodeType.Element, "cp", "revision", "http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
        xmlNode.InnerText = metadataModel.Version.ToString();
        newChild.AppendChild(xmlNode);
    }

    public SpiritMetadataSerializer()
    {
    }
}
