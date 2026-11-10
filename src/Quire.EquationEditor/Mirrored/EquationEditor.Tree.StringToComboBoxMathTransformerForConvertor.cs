namespace Quire.EquationEditor.Tree;

public partial class StringToComboBoxMathTransformerForConvertor
{
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
