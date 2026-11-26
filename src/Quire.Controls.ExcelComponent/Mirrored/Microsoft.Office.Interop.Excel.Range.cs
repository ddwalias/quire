#nullable disable
#pragma warning disable

namespace Quire.Microsoft.Office.Interop.Excel;

public partial interface Range
{
    [System.Runtime.CompilerServices.IndexerName("Address")]
    string this[object RowAbsolute, object ColumnAbsolute, XlReferenceStyle ReferenceStyle, object External, object RelativeTo] { get; }

    Areas Areas { get; }
    Range Columns { get; }
    int Count { get; }
    object Height { get; }
    Range Rows { get; }
    object Select();
    object Text { get; }
    object Width { get; }

    Worksheet Worksheet { get; }

    object get__Default(object RowIndex, object ColumnIndex);
    object get_Value(object RangeValueDataType);
    void set_Value(object RangeValueDataType, object Value);
    Range get_Resize(object RowSize, object ColumnSize);
}
