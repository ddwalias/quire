#nullable disable
using System.Xml.Linq;
using Quire.Controls.Core.Serialization;

namespace Quire.Controls.Core.Settings;

public partial class UnitSerializer : XElementSerializable
{
    public const string localName = "unit";

    public UnitSerializer(IEngineeringDocument component, string nameSpaceUri)
        : base(XName.Get("unit", nameSpaceUri), new PropertySerializableToXmlAttribute(component, "SimplifyUnitSystem", "simplify-units", new XmlBooleanConverter()))
    {
    }
}
