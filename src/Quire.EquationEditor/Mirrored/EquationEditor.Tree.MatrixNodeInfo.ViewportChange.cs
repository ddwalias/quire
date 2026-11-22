namespace Quire.EquationEditor.Tree;

public partial class MatrixNodeInfo
{
    private void a(string propertyName, object value)
    {
        if (propertyName == "Size")
        {
            var size = (TableSize)value;
            var offset = ViewportOffset;
            if (ViewportOffset.Row + size.Rows > FullSize.Rows)
            {
                offset.Row = FullSize.Rows - size.Rows;
            }

            if (ViewportOffset.Column + size.Columns > FullSize.Columns)
            {
                offset.Column = FullSize.Columns - size.Columns;
            }

            if (offset != ViewportOffset)
            {
                ViewportOffset = offset;
            }
        }

        if (propertyName == "Offset")
        {
            var offset = (TableCoordinate)value;
            var size = ViewportSize;
            if (offset.Row + ViewportSize.Rows > FullSize.Rows)
            {
                size.Rows = FullSize.Rows - offset.Row;
            }

            if (offset.Column + ViewportSize.Columns > FullSize.Columns)
            {
                size.Columns = FullSize.Columns - offset.Column;
            }

            if (size != ViewportSize)
            {
                ViewportSize = size;
            }
        }
    }
}
