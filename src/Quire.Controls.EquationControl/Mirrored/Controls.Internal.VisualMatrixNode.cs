using System.Windows.Controls;
using Quire.EquationEditor.Tree;
using Quire.Controls.ControlAdorners;

namespace Quire.Controls.Internal;

internal partial class VisualMatrixNode
{
    internal MatrixResizeAdorner ResizeAdorner { get; set; }

    public int RowCount => _matrixGrid.Rows;

    public int ColumnCount
    {
        get => _matrixGrid.Columns;
        set => _matrixGrid.Columns = value;
    }

    public UIElementCollection Cells => _matrixGrid.Children;

    public string[] MatrixHeadingRowIndices => _rowIndices;

    public string[] MatrixHeadingColumnIndices => _columnIndices;

    public System.Nullable<TableCoordinate> ErrorCell { get; set; }

    public TableSize OriginalSize { get; set; }

    public StackPanel RowHeading => _rowHeading;

    public StackPanel ColumnHeading => _columnHeading;

    public override double EffectiveBaselineY => DesiredSize.Height;

    public override double EffectiveScriptBaselineY => 0.0;

    public override double TextBaselineY => _effectiveBaselineY;
}
