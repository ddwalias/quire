#nullable disable
#pragma warning disable
using System;
using System.Xml;
using System.Xml.Serialization;

namespace Quire.PersistentDataObjects;

public partial class axes3Type
{
    private static readonly Type _typeOfThis = typeof(axes3Type);
    private const string _rootAttributeName = "axes";
    private static XmlSerializer _xmlSerializer;

    [XmlIgnore]
    public XmlNode xAxisRaw;
    [XmlIgnore]
    public XmlNode yAxisRaw;
    [XmlIgnore]
    public XmlNode zAxisRaw;
}
