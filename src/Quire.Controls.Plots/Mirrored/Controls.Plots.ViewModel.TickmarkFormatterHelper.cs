using System.Collections.Generic;
using Quire.Controls.Core;

namespace Quire.Controls.Plots.ViewModel;

public partial class TickmarkFormatterHelper : IFormatterHelper
{
    public const char DotOperator = '⋅';

    public const char SuperScriptZero = '⁰';

    public const char SuperScriptOne = '¹';

    public const char SuperScriptTwo = '²';

    public const char SuperScriptThree = '³';

    public const char SuperScriptFour = '⁴';

    public const char SuperScriptFive = '⁵';

    public const char SuperScriptSix = '⁶';

    public const char SuperScriptSeven = '⁷';

    public const char SuperScriptEight = '⁸';

    public const char SuperScriptNine = '⁹';

    public const char SuperScriptPlus = '⁺';

    public const char SuperScriptMinus = '⁻';

    public static readonly Dictionary<char, char> SuperScriptDigits;

    static TickmarkFormatterHelper()
    {
        SuperScriptDigits = new Dictionary<char, char>();
        SuperScriptDigits.Add('0', '⁰');
        SuperScriptDigits.Add('1', '¹');
        SuperScriptDigits.Add('2', '²');
        SuperScriptDigits.Add('3', '³');
        SuperScriptDigits.Add('4', '⁴');
        SuperScriptDigits.Add('5', '⁵');
        SuperScriptDigits.Add('6', '⁶');
        SuperScriptDigits.Add('7', '⁷');
        SuperScriptDigits.Add('8', '⁸');
        SuperScriptDigits.Add('9', '⁹');
        SuperScriptDigits.Add('-', '⁻');
        SuperScriptDigits.Add('+', '⁺');
    }

    public string Mult(string left, string right)
    {
        return left + "⋅" + right;
    }

    public string Exponent(string left, string right)
    {
        return left + ExponentToSuperscript(right);
    }

    public string Neg(string arg)
    {
        return "-" + arg;
    }

    public string Percent(string arg)
    {
        return arg + "%";
    }

    private static string ExponentToSuperscript(string exponent)
    {
        string text = string.Empty;
        foreach (char key in exponent)
        {
            text += SuperScriptDigits[key];
        }
        return text;
    }
}
