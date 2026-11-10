using System.Linq;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualSymKeywordsStackNode
{
    private VisualMathNode[] _nodeChildren;
    private VisualMatrixContents _operandsPanel;

    public override double EffectiveBaselineY => DesiredSize.Height;

    public override double EffectiveScriptBaselineY => 0.0;

    public override double TextBaselineY => _nodeChildren[0].TextBaselineY;

    public override IVisualMathNode this[int index]
    {
        get => _nodeChildren[index];
        set
        {
            VisualMathNode visualMathNode = (VisualMathNode)value;
            if (visualMathNode is VisualPlaceholderNode && _nodeChildren.Count() == 1)
            {
                visualMathNode.ShowOperatorOnlyWhenActive = true;
            }
            _nodeChildren[index] = visualMathNode;
            _operandsPanel.Children.Add(visualMathNode);
        }
    }

    public VisualSymKeywordsStackNode(VisualMathRegionNode mathRegion)
    {
        _operandsPanel = new VisualMatrixContents();
        _operandsPanel.Columns = 1;
        _operandsPanel.ColumnAlignment = ColumnAlignments.LeftAlign;
        Children.Add(_operandsPanel);
        if (!mathRegion.SymbolicEvalKeywordsVisibilityState && !mathRegion.IsFocused)
        {
            Visibility = Visibility.Collapsed;
        }
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        Point point = TransformToDescendant(_operandsPanel).Transform(hitPoint);
        VisualMathNode node = _operandsPanel.CellFromPoint(point);
        GeneralTransform transform = TransformToDescendant(node);
        return node.GetNodeAtPoint(transform.Transform(hitPoint));
    }

    public override void BuildChildren(IVisualMathNode[] children)
    {
        _nodeChildren = new VisualMathNode[children.Length];
        base.BuildChildren(children);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        _operandsPanel.Measure(availableSize);
        Size result = new Size(_operandsPanel.DesiredSize.Width, _operandsPanel.DesiredSize.Height);
        FireMeasureOverideCompleted();
        return result;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _operandsPanel.Arrange(new Rect(new Point(0.0, 0.0), _operandsPanel.DesiredSize));
        return DesiredSize;
    }
}
