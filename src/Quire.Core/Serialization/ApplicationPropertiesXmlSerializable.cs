#nullable disable
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Xml;
using System.Xml.Schema;
using Quire.Mirror;
using Quire.Mirror;
using Quire.IO;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Serialization;
using Quire.Xml;
using Quire.Xml;

namespace Quire.Core.Serialization;

public partial class ApplicationPropertiesXmlSerializable
{
    [MirrorVerified("PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests")]
    private readonly ISerializationHelper _helper;

    [MirrorVerified("PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests")]
    public ApplicationPropertiesXmlSerializable(ISerializationHelper helper)
    {
        _helper = helper;
    }

    [MirrorVerified("PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests")]
    public static bool IsFormatVersionCompatible(Version fileFormatVersion) => XmlWorksheetData.CurrentFormatVersion >= fileFormatVersion;

    [MirrorVerified("PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests")]
    public bool ShouldValidateOnWrite => true;

    [MirrorVerified("PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests")]
    public bool ShouldValidateOnRead => true;

    [MirrorVerified("Release IL")]
    public XmlSchema GetSchema() => SchemasManager.App.Schema;

    [MirrorVerified("Release IL")]
    private static propertiesType DeserializeAppProperties(XmlReader reader) => (propertiesType)propertiesType.XmlSerializer.Deserialize(reader);

    [MirrorVerified("Release IL")]
    public void ReadXml(XmlReader reader)
    {
        propertiesType propertiesType2 = DeserializeAppProperties(reader);
        _helper.ApplicationName = propertiesType2.application;
        _helper.AppVersion = new Version(propertiesType2.appVersion.Value);
        _helper.Release = propertiesType2.appVersion.release;
        _helper.FormatVersion = new Version(propertiesType2.serializationVersion.Value);
        _helper.EngineVersion = propertiesType2.engineVersion;
        if (!IsFormatVersionCompatible(_helper.FormatVersion))
        {
            throw new IncompatibleFormatWorksheetException();
        }
        if (propertiesType2.schemaPropertiesList != null)
        {
            schemaPropertiesType[] schemaPropertiesList = propertiesType2.schemaPropertiesList;
            foreach (schemaPropertiesType schemaPropertiesType2 in schemaPropertiesList)
            {
                if (EnumHelper.TryParse<SchemasManager.SchemaNameEnum>(schemaPropertiesType2.name, out var enumValue))
                {
                    _helper.SerializedSchemaVersions.Add(enumValue, new Version(schemaPropertiesType2.version));
                }
            }
        }
    }

    [MirrorVerified("Release IL")]
    public void WriteXml(XmlWriter writer)
    {
        List<schemaPropertiesType> list = new List<schemaPropertiesType>();
        foreach (KeyValuePair<SchemasManager.SchemaNameEnum, Version> serializedSchemaVersion in _helper.SerializedSchemaVersions)
        {
            list.Add(new schemaPropertiesType
            {
                name = serializedSchemaVersion.Key.ToString(),
                version = serializedSchemaVersion.Value.ToString()
            });
        }
        propertiesType propertiesType2 = new propertiesType
        {
            application = _helper.ApplicationName,
            appVersion = new propertiesTypeAppVersion
            {
                Value = _helper.AppVersion.ToString(),
                release = _helper.Release
            },
            serializationVersion = new propertiesTypeSerializationVersion
            {
                Value = _helper.FormatVersion.ToString(),
                architecture = XmlWorksheetData.Architecture,
                architectureSpecified = true,
                Culture = CultureInfo.CurrentCulture.ToString(),
                UiCulture = CultureInfo.CurrentUICulture.ToString()
            },
            engineVersion = _helper.EngineVersion,
            build = XmlWorksheetData.BuildId,
            schemaPropertiesList = list.ToArray()
        };
        propertiesType.XmlSerializer.Serialize(writer, propertiesType2);
    }

    [MirrorVerified("Release IL")]
    public static bool TryGetFormatVersionFromFile(string filePath, out Version formatVersion)
    {
        formatVersion = null;
        try
        {
            using Package package = PackageOperationsProvider.Instance.GetPackageForRead(filePath);
            PackagePart part = PackageOperationsProvider.Instance.GetSinglePartByContentType(package, "application/mathcad.extended-properties+xml");
            if (part == null)
            {
                return false;
            }
            using Stream input = part.GetStream(FileMode.Open, FileAccess.Read);
            using XmlReader reader = XmlReader.Create(input);
            string text = DeserializeAppProperties(reader)?.serializationVersion?.Value;
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            formatVersion = new Version(text);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
