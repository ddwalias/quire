#nullable disable
using System.Xml.Linq;
using System.Xml.Schema;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls.Core.Serialization;

internal partial class ResultElementsSerializer : XElementSerializable, IShouldValidate, IXDocumentRepairer
{
    public const string ResultsElemName = "resultsList";

    public bool ShouldValidateOnWrite => true;

    public bool ShouldValidateOnRead => false;

    public ResultElementsSerializer()
        : base(ResultDataSerializer.GetXNameWithResultNameSpace("resultsList"), new ResultSerializer())
    {
    }

    public override XmlSchema GetSchema()
    {
        return SchemasManager.Result10.Schema;
    }

    public void RepairXDocument(XDocument xDocument)
    {
        Result10Repairer.RepairXDocument(xDocument);
    }
}
