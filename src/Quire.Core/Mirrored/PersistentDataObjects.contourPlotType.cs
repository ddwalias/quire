#nullable disable
#pragma warning disable
using System.Xml;
using System.Xml.Serialization;

namespace Quire.PersistentDataObjects;

public partial class contourPlotType
{
    [XmlIgnore]
    public XmlNode plotEquationRaw;
}
