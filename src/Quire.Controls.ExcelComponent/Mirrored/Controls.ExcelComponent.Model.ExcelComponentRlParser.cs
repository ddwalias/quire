#nullable disable
#pragma warning disable
using System.Text.RegularExpressions;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class ExcelComponentRlParser : OleComponentRlParser
{
    private const string _subscriptExcelExpressionString = "\\(subscript(:0x[0-9a-fA-F]{8})? mc_excel(\\{\\*\\}|\\{VARIABLE\\})(:0x[0-9a-fA-F]{8})? (?<subscript>.*)\\)";

    private static readonly Regex _leftHandSubscriptExcelExpression = new Regex("^\\(subscript(:0x[0-9a-fA-F]{8})? mc_excel(\\{\\*\\}|\\{VARIABLE\\})(:0x[0-9a-fA-F]{8})? (?<subscript>.*)\\)");

    private static readonly Regex _rightHandsubscriptExcelExpression = new Regex("\\(subscript(:0x[0-9a-fA-F]{8})? mc_excel(\\{\\*\\}|\\{VARIABLE\\})(:0x[0-9a-fA-F]{8})? (?<subscript>.*)\\)");

    public const string ExcelVariableWithLabel = "mc_excel{VARIABLE}";

    internal static bool ParseExcelComponentEquationRl(string rl, out bool isInput, out string lhsExpression, out string rhsExpression)
    {
        return OleComponentRlParser.ParseOleComponentEquationRl(rl, _leftHandSubscriptExcelExpression, _rightHandsubscriptExcelExpression, out isInput, out lhsExpression, out rhsExpression);
    }

    public ExcelComponentRlParser()
    {
    }
}
