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
            foreach (XElement item2 in thisElement.Descendants())
            {
                UnitDescriptor item = DefaultUnitDescriptorTable.Instance[(string)item2.Attribute("unit-name")];
                UnitDescriptor unitDescriptor = new UnitDescriptor
                {
                    CanonicalName = (string)item2.Attribute("unit-name"),
                    Quantity = (string)item2.Attribute("quantity-name")
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
                List<object> list2 = _engineeringDocument.CustomBaseUnitDescriptors.Select(x => new UnitSystemSerializer(x, _nameSpaceUri)).ToList<object>();
                Children = new object[list2.Count];
                list2.CopyTo(Children, 0);
            }
            else
            {
                if (_engineeringDocument.CustomDerivedUnitDescriptors == null)
                {
                    return;
                }
                List<object> list3 = _engineeringDocument.CustomDerivedUnitDescriptors.Select(x => new UnitSystemSerializer(x, _nameSpaceUri)).ToList<object>();
                Children = new object[list3.Count];
                list3.CopyTo(Children, 0);
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
