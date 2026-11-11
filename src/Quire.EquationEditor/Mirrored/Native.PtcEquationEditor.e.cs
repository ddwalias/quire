namespace Quire.Native.PtcEquationEditor;

internal static partial class e
{
    // Mathcad's regex initializer has .initlocals False.
    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    static e()
    {
        a = new global::System.Text.RegularExpressions.Regex("^[1-9][0-9]*(\\.[0-9]*)?$|^0(\\.[0-9]*)?$|^(\\.[0-9]*)$", global::System.Text.RegularExpressions.RegexOptions.Singleline);
        b = new global::System.Text.RegularExpressions.Regex("^[\\p{L}_][\\w_]*$", global::System.Text.RegularExpressions.RegexOptions.Singleline);
        c = new global::System.Text.RegularExpressions.Regex("^\\((.*\\))$", global::System.Text.RegularExpressions.RegexOptions.Singleline);
        d = new global::System.Text.RegularExpressions.Regex("^[\\+\\^\\/\\-\\*\\,\\:]$", global::System.Text.RegularExpressions.RegexOptions.Singleline);
    }
}
