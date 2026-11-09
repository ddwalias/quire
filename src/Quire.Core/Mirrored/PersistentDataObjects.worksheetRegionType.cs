#nullable disable
#pragma warning disable
using System;
using System.Xml.Serialization;

namespace Quire.PersistentDataObjects;

public partial class worksheetRegionType
{
    private static readonly Type _typeOfThis = typeof(worksheetRegionType);
    private static XmlSerializer _xmlSerializerWithOverrides;
    private static XmlSerializer _xmlSerializerDefault;
    public const string RootAttributeName = "region";
}
