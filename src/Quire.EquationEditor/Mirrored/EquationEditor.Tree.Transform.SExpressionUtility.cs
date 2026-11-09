using System.Text.RegularExpressions;

namespace Quire.EquationEditor.Tree.Transform;

public partial class SExpressionUtility
{
    public static string EscapeStringValue(string str)
    {
        str = str.Replace("\\", "\\\\");
        str = str.Replace("\"", "\\\"");
        return str;
    }

    private static string a(string text)
    {
        text = Regex.Replace(text, "\\(\\s+", "(");
        text = Regex.Replace(text, "\\s+\\)", ")");
        return Regex.Replace(text, "\\s+", " ");
    }
}
