namespace Quire.EquationEditor.Tree;

public partial class StringToComboBoxMathTransformerForConvertor
{
    // Mathcad's static initializer has .initlocals False.
    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    static StringToComboBoxMathTransformerForConvertor()
    {
        l = "(?:[Ee]" + b + "?\\d+)?";
        m = "\\d+(?:\\.\\d*)?" + l + "|\\.\\d+" + l;
        n = "ImaginarySuffix";
    }

    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    public static string ExponentNotationToPrime(string value)
    {
        if (global::Quire.DebugTools.AssertNotNull(value, null).Failed ||
            global::Quire.DebugTools.DevAssert(new global::System.Text.RegularExpressions.Regex("^(" + m + ")$").Match(value).Success, null).Failed)
        {
            return value;
        }

        return global::System.Text.RegularExpressions.Regex.Replace(value.ToUpper(), "E(?!" + b + ")", "E+");
    }
}
