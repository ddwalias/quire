using System.Xml;

namespace Quire.Controls.Plots;

public partial interface IEquationSerializationProxy
{
    bool IsCleared { get; }
    XmlNode EquationXmlNode { get; set; }
    string EquationSExpression { get; set; }
}
