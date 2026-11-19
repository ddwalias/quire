using System;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualMultiLineAligningBinaryOperatorNode
{
    private double _secondArgumentVOffsetForArrange;
    private double _firstArgumentVOffsetForArrange;
    private double _secondArgumentHOffsetForArrange;
    private double _selfHoffsetForArrange;
    private Point _operatorTopLeft;

    private double CurrentBaseLine => Math.Max(FirstVariantLineNode.BottomLineBaseLine, SecondVariantLineNode.TopLineBaseLine);

    private double FirstNodeAndOperandWidth
    {
        get
        {
            double bottomLineWidth = FirstVariantLineNode.BottomLineWidth;
            Size desiredSize = _operator.DesiredSize;
            return bottomLineWidth + desiredSize.Width;
        }
    }

    protected override bool HasFirstOperandAtLeft => true;

    protected override bool HasFirstOperandAbove => FirstVariantLineNode.IsMultiline;

    public VisualMultiLineAligningBinaryOperatorNode(VisualSymbolNode opSymbol)
        : base(opSymbol)
    {
    }

    private double ComputeTopLineBaseLine()
    {
        if (!FirstVariantLineNode.IsMultiline)
        {
            if (!SecondVariantLineNode.IsMultiline)
            {
                return Math.Max(FirstMathNode.TextBaselineY, SecondMathNode.TextBaselineY);
            }

            return Math.Max(FirstMathNode.TextBaselineY, SecondVariantLineNode.TopLineBaseLine);
        }

        return FirstVariantLineNode.TopLineBaseLine;
    }
}
