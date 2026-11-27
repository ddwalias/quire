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
        int delimiterIndex = 1;
        char[] anyOf = new char[2] { '(', ')' };
        while (num != 0)
        {
            delimiterIndex = def.IndexOfAny(anyOf, delimiterIndex + 1);
            if (delimiterIndex == -1)
            {
                break;
            }
            if (def[delimiterIndex].Equals('('))
            {
                num++;
            }
            else if (def[delimiterIndex].Equals(')'))
            {
                num--;
            }
        }
        return delimiterIndex + 1;
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
            bool leftMatches = leftRegex != null && leftRegex.Match(lhsExpression).Success;
            bool rightMatches = rightRegex != null && rightRegex.Match(rhsExpression).Success;
            flag = leftMatches ^ rightMatches;
            if (flag)
            {
                isInput = leftMatches;
            }
        }
        return flag;
    }

    public OleComponentRlParser()
    {
    }
}
