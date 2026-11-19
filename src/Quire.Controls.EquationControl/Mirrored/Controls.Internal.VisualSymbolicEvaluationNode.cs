using System;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualSymbolicEvaluationNode
{
    public override bool IsMonoLine
    {
        get => VisualSymbolNode.GetIsMonoLine(SymEvalSymbol);
        set => VisualSymbolNode.SetIsMonoLine(SymEvalSymbol, value);
    }

    public override IVisualMathNode this[int index]
    {
        get => _nodeChildren[index];
        set
        {
            if (index > _symEvalDescriptor.ChildCount)
            {
                throw new InvalidOperationException(string.Format("Index cannot be greater than {0} when adding child nodes to a symbolic evaluation.", _symEvalDescriptor.ChildCount - 1));
            }

            VisualMathNode visualMathNode = (VisualMathNode)value;
            if (index == 1)
            {
                if (visualMathNode is VisualPlaceholderNode)
                {
                    visualMathNode.ShowOperatorOnlyWhenActive = true;
                }
                else
                {
                    visualMathNode.ShowOperatorOnlyWhenActive = false;
                }
            }

            _nodeChildren[index] = visualMathNode;
            Children.Add(visualMathNode);
        }
    }

    private SymbolicEvaluationLayoutManager Layout
    {
        get
        {
            if (_layout == null)
            {
                Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return _layout;
        }
        set => _layout = value;
    }

    public VisualMathNode[] NodeChildren => _nodeChildren;

    public Fence SymEvalSymbol => _symEvalSymbol;

    public Point SymEvalSymbolTopLeft
    {
        get
        {
            Rect symEvalSymbolRect = Layout.SymEvalSymbolRect;
            return symEvalSymbolRect.TopLeft;
        }
    }

    public override double TextBaselineY
    {
        get
        {
            if (_layout == null)
            {
                return 0.0;
            }

            return Layout.TextBaselineOffset;
        }
    }

    public override double EffectiveBaselineY => TextBaselineY;

    public override double EffectiveScriptBaselineY => _nodeChildren[2].EffectiveScriptBaselineY;

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[1] { SymEvalSymbol };

    public void SetVisibilityOfOperatorSymbol(Visibility visibility)
    {
        SymEvalSymbol.Visibility = visibility;
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        Layout.FindClosestChild(hitPoint, out var childType, out var childIndex, out var _);
        if (childType == LayoutChildType.Child)
        {
            VisualMathNode visualMathNode = _nodeChildren[childIndex];
            GeneralTransform generalTransform = TransformToDescendant(visualMathNode);
            return visualMathNode.GetNodeAtPoint(generalTransform.Transform(hitPoint));
        }

        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, childIndex, hitPoint);
    }
}
