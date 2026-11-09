#nullable disable
#pragma warning disable
using System;
using System.Xml.Serialization;

namespace Quire.PersistentDataObjects;

public partial class axisBaseTypeMarker
{
    private static readonly Type _typeOfThis = typeof(axisBaseTypeMarker);
    private const string _rootAttributeName = "marker";
    private static XmlSerializer _xmlSerializer;
}
