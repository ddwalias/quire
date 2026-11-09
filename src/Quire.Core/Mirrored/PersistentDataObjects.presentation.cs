#nullable disable
#pragma warning disable
using System;
using System.Xml.Serialization;

namespace Quire.PersistentDataObjects;

public partial class presentation
{
    private static readonly Type _typeOfThis = typeof(presentation);
    private const string _rootAttributeName = "presentation";
    private static XmlSerializer _xmlSerializer;
}
