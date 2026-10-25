#nullable disable
using System.Xml.Linq;
using Quire.Calculation.Core;
using Quire.Controls.Core.Serialization;

namespace Quire.Controls.Core.Settings;

public partial class UnitSystemSerializer : XElementSerializable
{
    public const string localName = "unit";

    private UnitDescriptor _unitDescriptor;

    public UnitSystemSerializer(UnitDescriptor ud, string nameSpaceUri)
        : base(XName.Get("unit", nameSpaceUri))
    {
        _unitDescriptor = ud;
    }

    protected override void CreateChildren(XElement thisElement = null)
    {
        Children = new object[2]
        {
            new PropertySerializableToXmlAttribute(_unitDescriptor, "Key", "unit-name"),
            new PropertySerializableToXmlAttribute(_unitDescriptor, "Quantity", "quantity-name")
        };
    }
}
