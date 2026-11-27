#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Quire.Calculation.Core;
using Quire.Controls.Core.Serialization;

namespace Quire.Controls.Core.Settings;

internal partial class UnitSystemCustomized : XElementSerializable
{
    private IEngineeringDocument _engineeringDocument;
    private string _localName;
    private string _nameSpaceUri;

    public UnitSystemCustomized(IEngineeringDocument engineeringDocument, string localName, string nameSpaceUri)
        : base(XName.Get(localName, nameSpaceUri), System.Array.Empty<object>())
    {
        _engineeringDocument = engineeringDocument;
        _localName = localName;
        _nameSpaceUri = nameSpaceUri;
    }

    protected override void CreateChildren(XElement thisElement = null)
    {
        IList<UnitDescriptor> list = new List<UnitDescriptor>();
        if (thisElement != null)
        {
            foreach (XElement unitElement in thisElement.Descendants())
            {
                UnitDescriptor item = DefaultUnitDescriptorTable.Instance[(string)unitElement.Attribute("unit-name")];
                UnitDescriptor unitDescriptor = new UnitDescriptor
                {
                    CanonicalName = (string)unitElement.Attribute("unit-name"),
                    Quantity = (string)unitElement.Attribute("quantity-name")
                };
                list.Add(item);
            }
        }
        if (list.Count == 0)
        {
            if (_localName == "baseUnits")
            {
                if (_engineeringDocument.CustomBaseUnitDescriptors == null)
                {
                    return;
                }
                List<object> baseUnitSerializers = _engineeringDocument.CustomBaseUnitDescriptors.Select(x => new UnitSystemSerializer(x, _nameSpaceUri)).ToList<object>();
                Children = new object[baseUnitSerializers.Count];
                baseUnitSerializers.CopyTo(Children, 0);
            }
            else
            {
                if (_engineeringDocument.CustomDerivedUnitDescriptors == null)
                {
                    return;
                }
                List<object> derivedUnitSerializers = _engineeringDocument.CustomDerivedUnitDescriptors.Select(x => new UnitSystemSerializer(x, _nameSpaceUri)).ToList<object>();
                Children = new object[derivedUnitSerializers.Count];
                derivedUnitSerializers.CopyTo(Children, 0);
            }
        }
        else if (_localName == "baseUnits")
        {
            _engineeringDocument.CustomBaseUnitDescriptors = new List<UnitDescriptor>(list);
        }
        else
        {
            _engineeringDocument.CustomDerivedUnitDescriptors = new List<UnitDescriptor>(list);
        }
    }
}
