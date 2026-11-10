using System.Text.RegularExpressions;

namespace Quire.Controls;

public partial class RowsValidationRule : ColumnsRowsValidationRule<int>
{
    private static readonly Regex _rowsFormat = new Regex("^\\s*(?<first>[0-9]+)\\s*($|-\\s*(?<last>[0-9]+)\\s*$)");
    private static readonly string _errorStringRows = new ExcelGrid().Resources["Ptc.Controls.Core.ExcelGrid.RowsError"].ToString();

    protected override Regex Format => _rowsFormat;
    protected override string ErrorString => _errorStringRows;

    public bool GetFirstLastRows(string value, out int firstRow, out int lastRow)
    {
        return GetFirstLastRowsOrColumns(value, out firstRow, out lastRow);
    }

    protected override bool TryParseFirstLastRowsOrColumns(string firstArg, string lastArg, out int firstRow, out int lastRow)
    {
        bool flag = int.TryParse(firstArg, out firstRow);
        if (!int.TryParse(lastArg, out lastRow) & flag)
        {
            lastRow = firstRow;
        }
        if (flag && lastRow >= firstRow)
        {
            return firstRow > 0;
        }
        return false;
    }

    public RowsValidationRule()
    {
    }
}
