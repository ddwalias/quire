#nullable disable
using System.Xml.Linq;
using System.Xml.Schema;
using Quire.Controls.Core.IORegions;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls.Core.Serialization;

internal partial class IntegrationElementsSerializer : XElementSerializable, IShouldValidate, IXDocumentRepairer
{
    public bool ShouldValidateOnWrite => true;

    public bool ShouldValidateOnRead => true;

    public IntegrationElementsSerializer(IIORegionContainer area, ISerializationHelper helper)
        : base(IntegrationDataSerializer.GetXNameWithIntegrationNameSpace("regions"), new IntegrationRegionSerializer(area, helper))
    {
    }

    public override XmlSchema GetSchema()
    {
        return SchemasManager.Integration10.Schema;
    }

    public void RepairXDocument(XDocument xDocument)
    {
        Integration10Repairer.RepairXDocument(xDocument);
    }
}
