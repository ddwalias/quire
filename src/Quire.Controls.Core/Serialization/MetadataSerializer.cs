#nullable disable
using System;
using System.IO;
using System.Xml;

namespace Quire.Controls.Core.Serialization;

public abstract partial class MetadataSerializer : IMetadataSerializer
{
    public void Serialize(IMetadataModel metadataModel, TextWriter writer)
    {
        if (metadataModel == null || writer == null)
        {
            throw new ArgumentNullException("metadataModel or writer cannot be null");
        }
        XmlDocument xmlDocument = new XmlDocument();
        FillInXmlMetadata(xmlDocument, metadataModel);
        xmlDocument.Save(writer);
    }

    public void Deserialize(TextReader reader, IMetadataModel metadataModel)
    {
        if (metadataModel == null || reader == null)
        {
            throw new ArgumentNullException("metadataModel or reader cannot be null");
        }
        try
        {
            XmlDocument xmlDocument = new XmlDocument();
            CreateSchemas(xmlDocument);
            xmlDocument.Load(reader);
            xmlDocument.Validate(null);
            FillInMetadataModel(xmlDocument, metadataModel);
        }
        catch (Exception innerException)
        {
            throw new FormatException("Incorrect serialization format", innerException);
        }
    }

    protected abstract void CreateSchemas(XmlDocument xmlMetadata);

    protected abstract void FillInMetadataModel(XmlDocument xmlMetadata, IMetadataModel metadataModel);

    protected abstract void FillInXmlMetadata(XmlDocument xmlMetadata, IMetadataModel metadataModel);

    protected MetadataSerializer()
    {
    }
}
