using System.Xml;
using Quire.Core;

namespace Quire.EquationEditor.Tree.Transform;

public partial class XmlNodeToMathNodeTransform
{
    private static bool f(string text)
    {
        return !text.IsNullOrEmpty() && text[0].Equals('-');
    }

    public static bool IsMC14Document(XmlNode node)
    {
        return node.NamespaceURI == "http://schemas.mathsoft.com/math30";
    }

    public static string RemoveMinusSignFromString(string text)
    {
        if ((AssertionE)DebugTools.DevAssert(text[0].Equals('-'), "This number should be negative with a minus sign in the start.") == AssertionE.Passed)
        {
            text = text.Substring(1);
        }

        return text;
    }
}
