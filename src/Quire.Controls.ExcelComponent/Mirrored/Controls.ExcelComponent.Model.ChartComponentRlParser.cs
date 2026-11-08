#nullable disable
#pragma warning disable
using System;
using System.Text.RegularExpressions;
using Quire.Controls.Core;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class ChartComponentRlParser : OleComponentRlParser
{
    public const uint MaxIndex = 50u;

    private const string IdentifierWithHandleGroupName = "identifierWithHandle";

    private const string IdentifierGroupName = "identifier";

    private const string IndexValueGroupName = "indexValue";

    private const string SubscriptDefinitionGroupName = "subscriptDefinition";

    private const string SubscriptGroupName = "subscript";

    private const string LabelGroupName = "label";

    private const string OptionalHandleRegex = "(:0x[0-9a-fA-F]+)?";

    private static readonly string _optionalSubscriptDefinition = "^(" + OleComponentRlParser.GroupNamePattern("subscriptDefinition") + "\\(subscript(:0x[0-9a-fA-F]+)? )?";

    private static readonly string _optionalSubscriptIndex = "(" + OleComponentRlParser.GroupNamePattern("subscript") + " \\(number (" + OleComponentRlParser.GroupNamePattern("indexValue") + "[0-9]+)(:0x[0-9a-fA-F]+)?\\)\\))?";

    private static readonly Regex _rhsNotAllowed = new Regex("^[\\s]*\\([\\s]*lambda(:0x[0-9a-fA-F]+)?[\\s]*\\(");

    private static readonly Regex _identifierIndexExpression = new Regex(_optionalSubscriptDefinition + "(" + OleComponentRlParser.GroupNamePattern("identifierWithHandle") + "mc_(" + OleComponentRlParser.GroupNamePattern("identifier") + "[^\\{: ]+)(\\{(" + OleComponentRlParser.GroupNamePattern("label") + "[^\\}]+)\\})?(:0x[0-9a-fA-F]+)?)" + _optionalSubscriptIndex);

    internal static string SeparateIdentifierVsIndex(string definition, out string identifierWithHandle, out string identifier, out uint? index)
    {
        identifier = (identifierWithHandle = null);
        index = null;
        string result = "WronglyFormattedInput.Error.Key";
        Match match = _identifierIndexExpression.Match(definition);
        if (match.Success)
        {
            identifier = match.Result(OleComponentRlParser.MatchPattern("identifier"));
            identifierWithHandle = match.Result(OleComponentRlParser.MatchPattern("identifierWithHandle"));
            string items = match.Result(OleComponentRlParser.MatchPattern("subscriptDefinition"));
            string items2 = match.Result(OleComponentRlParser.MatchPattern("subscript"));
            string text = match.Result(OleComponentRlParser.MatchPattern("indexValue"));
            bool flag = items.IsNotEmpty();
            if (flag == items2.IsNotEmpty())
            {
                if (text.IsNotEmpty())
                {
                    result = ((uint.TryParse(text, out var result2) && result2 <= 50) ? null : "chart_invalid_index");
                    index = result2;
                }
                else
                {
                    result = (items.IsNullOrEmpty() ? null : "chart_invalid_index");
                }
            }
            else if (flag)
            {
                result = "chart_invalid_index";
            }
        }
        return result;
    }

    internal static bool ParseChartComponentEquationRl(string rl, out bool isInput, out string lhsExpression, out string rhsExpression)
    {
        bool flag = OleComponentRlParser.ParseOleComponentEquationRl(rl, _identifierIndexExpression, null, out isInput, out lhsExpression, out rhsExpression);
        if (flag)
        {
            if (_rhsNotAllowed.Match(rhsExpression).Success)
            {
                flag = false;
            }
            else
            {
                Match match = _identifierIndexExpression.Match(lhsExpression);
                if (match.Success)
                {
                    string text = match.Result(OleComponentRlParser.MatchPattern("label"));
                    if (DebugTools.DevAssert(text.IsNotEmpty(), null).Passed)
                    {
                        IdLabels? idLabels = IdLabelToStringConverter.ConvertBack(text, false);
                        if (DebugTools.AssertNotNull(idLabels, null).Failed || idLabels != IdLabels.Variable)
                        {
                            Group obj = match.Groups["label"];
                            lhsExpression = lhsExpression.Remove(obj.Index, obj.Length).Insert(obj.Index, IdLabelToStringConverter.Convert(IdLabels.Variable, true));
                        }
                    }
                }
                else
                {
                    flag = false;
                }
            }
        }
        return flag;
    }

    public ChartComponentRlParser()
    {
    }
}
