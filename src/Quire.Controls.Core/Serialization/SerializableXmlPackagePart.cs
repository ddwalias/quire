#nullable disable
using System;
using System.IO;
using System.IO.Packaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls.Core.Serialization;

public partial class SerializableXmlPackagePart : IPackagePartSerializable
{
    private readonly IXmlAndXElementSerializable _xmlSerializable;

    protected Action<XmlSchemaException> OnWriteValidationFailed;

    public static void OnWriteValidationFailedDevAssert(XmlSchemaException ex)
    {
        DebugTools.DevAssert(false, ex.Message);
    }

    public SerializableXmlPackagePart(IXmlAndXElementSerializable xmlSerializable)
        : this(xmlSerializable, null)
    {
    }

    public SerializableXmlPackagePart(IXmlAndXElementSerializable xmlSerializable, Action<XmlSchemaException> onWriteValidationFailed)
    {
        _xmlSerializable = xmlSerializable;
        OnWriteValidationFailed = onWriteValidationFailed;
    }

    public void Write(PackagePart part, bool useTempFile)
    {
        XDocument xDocument = new XDocument();
        _xmlSerializable.WriteXml(xDocument);
        if (_xmlSerializable is IShouldValidate && (_xmlSerializable as IShouldValidate).ShouldValidateOnWrite)
        {
            Validate(xDocument, OnWriteValidationFailedHandler, false);
        }
        if (useTempFile)
        {
            SerializationHelperUtils.SaveToPartUsingTemporaryFile((string tempFile) =>
            {
                xDocument.Save(tempFile, SaveOptions.DisableFormatting);
            }, part, null);
            return;
        }
        using (XmlWriter writer = XmlWriter.Create(part.GetStream(FileMode.Create), new XmlWriterSettings
        {
            CloseOutput = true
        }))
        {
            xDocument.Save(writer);
        }
    }

    private void OnWriteValidationFailedHandler(object sender, ValidationEventArgs args)
    {
        if (OnWriteValidationFailed != null)
        {
            OnWriteValidationFailed(args.Exception);
            return;
        }
        throw args.Exception;
    }

    public void Read(PackagePart part)
    {
        XDocument xDocument;
        using (XmlReader xmlReader = XmlReader.Create(part.GetStream(FileMode.Open), new XmlReaderSettings
        {
            CloseInput = true
        }))
        {
            xmlReader.Read();
            xDocument = XDocument.Load(xmlReader);
        }
        if (_xmlSerializable is IXDocumentRepairer)
        {
            (_xmlSerializable as IXDocumentRepairer).RepairXDocument(xDocument);
        }
        if (_xmlSerializable is IShouldValidate && (_xmlSerializable as IShouldValidate).ShouldValidateOnRead)
        {
            Validate(xDocument, null, true);
        }
        _xmlSerializable.ReadXml(xDocument.Root);
    }

    private void Validate(XDocument xDocument, ValidationEventHandler validationFailedHandler, bool addSchemaInfo)
    {
        XmlSchema schema = _xmlSerializable.GetSchema();
        string targetNamespace = (schema != null) ? schema.TargetNamespace : null;
        if (!string.IsNullOrEmpty(targetNamespace))
        {
            SchemasManager.SchemaData schemaData = SchemasManager.GetSchemaData((SchemasManager.SchemaData data) => data.Schema.TargetNamespace == targetNamespace);
            if (schemaData != null)
            {
                xDocument.Validate(schemaData.CompiledSchemaSet, validationFailedHandler, addSchemaInfo);
                return;
            }
            DebugTools.DevAssert(false, "Cannot find schema with target namespace: " + targetNamespace);
        }
        else
        {
            DebugTools.DevAssert(false, "No schema or target namespace for serializable: " + _xmlSerializable.GetType()?.ToString());
        }
    }
}
