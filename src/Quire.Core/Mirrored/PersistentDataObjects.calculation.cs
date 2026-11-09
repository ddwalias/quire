#nullable disable
#pragma warning disable
using System;
using System.Xml.Serialization;

namespace Quire.PersistentDataObjects;

public partial class calculation
{
    private static readonly Type _typeOfThis = typeof(calculation);
    public const string RootAttributeName = "calculation";
    private static XmlSerializer _xmlSerializer;
}
