#nullable disable
using System.Xml.Linq;
using Quire.Controls.Core.Serialization;

namespace Quire.Controls.Core.Settings;

public partial class BuiltInVariables : XElementSerializable
{
    private const string localName = "builtInVariables";

    public BuiltInVariables(object o, string nameSpaceUri)
        : base(XName.Get("builtInVariables", nameSpaceUri),
               new PropertySerializableToXmlAttribute(o, "IndexOrigin", "array-origin"),
               new PropertySerializableToXmlAttribute(o, "Tol", "convergence-tolerance"),
               new PropertySerializableToXmlAttribute(o, "CTol", "constraint-tolerance"),
               new PropertySerializableToXmlAttribute(o, "PRNColWidth", "prn-col-width"),
               new PropertySerializableToXmlAttribute(o, "PRNPrecision", "prn-precision"),
               new PropertySerializableToXmlAttribute(o, "RandSeed", "random-seed"))
    {
    }
}
