#nullable disable
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Quire.Mirror;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.PersistentData;

public partial class ProtectedItemType
{
    [MirrorVerified("PtcPersistentDataProtectedItemTypeMechanicalDifferentialTests")]
    private string itemtypeField;

    [MirrorVerified("PtcPersistentDataProtectedItemTypeMechanicalDifferentialTests")]
    public string itemtype { get => itemtypeField; set => itemtypeField = value; }

    public global::Quire.PersistentData.IProtectableDataType DataToProtect
    {
        get
        {
            if (_dataToProtect == null)
            {
                IDeS11NProvider deS11NProvider = DeS11NProvider;
                SchemasManager.SchemaData schemaData = SchemasManager.GetSchemaData((SchemasManager.SchemaData data) => data.SchemaName == SchemasManager.Worksheet50);
                using Stream input = deS11NProvider.UnpackProtectedPart(dataidref);
                XmlReader xmlReader = XmlReader.Create(input, new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Fragment,
                    ValidationType = ValidationType.Schema,
                    Schemas = schemaData.CompiledSchemaSet
                });
                XmlSerializer xmlSerializer = ProtectableTypesRegistry.GetXmlSerializer(itemtype, deS11NProvider.UseOverrides);
                _dataToProtect = (IProtectableDataType)xmlSerializer.Deserialize(xmlReader);
            }
            return _dataToProtect;
        }
        set => _dataToProtect = value;
    }

    public IS11NProvider S11NProvider { get; set; }

    public IDeS11NProvider DeS11NProvider { get; set; }
}
