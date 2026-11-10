using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Quire.Controls;

public abstract partial class ColumnsRowsValidationRule<TArg> : ValidationRule
{
    protected const string FirstMatchExpr = "first";
    protected const string LastMatchExpr = "last";

    protected abstract Regex Format { get; }
    protected abstract string ErrorString { get; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (!GetFirstLastRowsOrColumns((string)value, out var _, out var _))
        {
            return new ValidationResult(false, ErrorString);
        }
        return new ValidationResult(true, null);
    }

    protected bool GetFirstLastRowsOrColumns(string value, out TArg firstRowOrColumn, out TArg lastRowOrColumn)
    {
        firstRowOrColumn = default;
        lastRowOrColumn = default;
        Match match = Format.Match(value);
        bool flag = match.Success;
        if (flag)
        {
            string firstArg = match.Result("${first}");
            string lastArg = match.Result("${last}");
            flag = TryParseFirstLastRowsOrColumns(firstArg, lastArg, out firstRowOrColumn, out lastRowOrColumn);
        }
        return flag;
    }

    protected abstract bool TryParseFirstLastRowsOrColumns(string firstArg, string lastArg, out TArg firstObj, out TArg lastObj);

    protected ColumnsRowsValidationRule()
    {
    }
}
