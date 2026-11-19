using System.Windows.Controls;
using Quire.Controls.ControlAdorners;

namespace Quire.Controls.Internal;

internal partial class VisualMatrixNode
{
    internal MatrixResizeAdorner ResizeAdorner { get; set; }

    public StackPanel RowHeading => _rowHeading;

    public StackPanel ColumnHeading => _columnHeading;

    public override double EffectiveBaselineY => DesiredSize.Height;

    public override double EffectiveScriptBaselineY => 0.0;

    public override double TextBaselineY => _effectiveBaselineY;
}
