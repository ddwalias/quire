#nullable disable
#pragma warning disable
using System.Text.RegularExpressions;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class OleComponentRlParser
{
    private const string DefineGroupName = "def";

    private static readonly Regex _defineExpression = new Regex("^\\(define(:0x[0-9a-fA-F]{8})? (" + GroupNamePattern("def") + ".*)\\)");

    protected static string MatchPattern(string groupName)
    {
        return "${" + groupName + "}";
    }

    protected static string GroupNamePattern(string groupName)
    {
        return "?<" + groupName + ">";
    }

    private static void SeparateExpressionBySpaceOrBrackets(string expression, out string lhsExp, out string rhsExp)
    {
        int num = expression.StartsWith("(") ? FindPairCloseBracket(expression) : expression.IndexOf(" ");
        lhsExp = (num < expression.Length) ? expression.Remove(num) : expression;
        rhsExp = (num + 1 < expression.Length) ? expression.Remove(0, num + 1) : string.Empty;
    }

    private static int FindPairCloseBracket(string def)
    {
        int num = 1;
        int num2 = 1;
        char[] anyOf = new char[2] { '(', ')' };
        while (num != 0)
        {
            num2 = def.IndexOfAny(anyOf, num2 + 1);
            if (num2 == -1)
            {
                break;
            }
            if (def[num2].Equals('('))
            {
                num++;
            }
            else if (def[num2].Equals(')'))
            {
                num--;
            }
        }
        return num2 + 1;
    }

    protected static bool ParseOleComponentEquationRl(string rl, Regex leftRegex, Regex rightRegex, out bool isInput, out string lhsExpression, out string rhsExpression)
    {
        bool flag = false;
        isInput = false;
        lhsExpression = string.Empty;
        rhsExpression = string.Empty;
        Match match = _defineExpression.Match(rl);
        if (match.Success)
        {
            SeparateExpressionBySpaceOrBrackets(match.Result(MatchPattern("def")), out lhsExpression, out rhsExpression);
            bool flag2 = leftRegex != null && leftRegex.Match(lhsExpression).Success;
            bool flag3 = rightRegex != null && rightRegex.Match(rhsExpression).Success;
            flag = flag2 ^ flag3;
            if (flag)
            {
                isInput = flag2;
            }
        }
        return flag;
    }

    public OleComponentRlParser()
    {
    }
}
