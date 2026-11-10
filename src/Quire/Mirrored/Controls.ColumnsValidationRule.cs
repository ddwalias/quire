using System.Text.RegularExpressions;
using Quire.Controls.Core.Excel;

namespace Quire.Controls;

public partial class ColumnsValidationRule : ColumnsRowsValidationRule<string>
{
    private static readonly Regex _columnsFormat = new Regex("^\\s*(?<first>[a-zA-Z]+)\\s*($|-\\s*(?<last>[a-zA-Z]+)\\s*$)");
    private static readonly string _errorStringColumns = new ExcelGrid().Resources["Ptc.Controls.Core.ExcelGrid.ColumnsError"].ToString();

    protected override Regex Format => _columnsFormat;
    protected override string ErrorString => _errorStringColumns;

    public bool GetFirstLastColumns(string value, out string firstColumn, out string lastColumn)
    {
        return GetFirstLastRowsOrColumns(value, out firstColumn, out lastColumn);
    }

    protected override bool TryParseFirstLastRowsOrColumns(string firstArg, string lastArg, out string firstColumn, out string lastColumn)
    {
        firstColumn = firstArg.ToUpper();
        lastColumn = string.IsNullOrEmpty(lastArg) ? firstColumn : lastArg.ToUpper();
        return ExcelReader.CompareColumn(lastColumn, firstColumn) >= 0;
    }

    public ColumnsValidationRule()
    {
    }
}
