#nullable disable
#pragma warning disable
using System;
using System.Xml.Serialization;

namespace Quire.PersistentData;

public abstract partial class RegionCollectionSerializer<TCollection>
{
    private static readonly Type _typeOfCollection = typeof(TCollection);
    private static XmlSerializer _worksheetXmlSerializerWithOverrides;
    private static XmlSerializer _worksheetXmlSerializerDefault;
}
