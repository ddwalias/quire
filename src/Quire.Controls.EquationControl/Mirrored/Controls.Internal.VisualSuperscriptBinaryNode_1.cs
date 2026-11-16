using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.Controls.Internal;

internal partial class VisualSuperscriptBinaryNode<OperatorType> : VisualLinearBinaryOperatorNode<OperatorType>
    where OperatorType : VisualSymbolNode, new()
{
    protected double _pushFirstOpDown;

    public override IEnumerable<UIElement> CoreElements
    {
        get
        {
            yield return OperatorElement;
            yield return (UIElement)SecondOperand;
        }
    }

    public override double TextBaselineY => FirstMathNode.TextBaselineY + _pushFirstOpDown;

    public override double EffectiveBaselineY => FirstMathNode.EffectiveBaselineY + _pushFirstOpDown;

    public override double EffectiveScriptBaselineY => SecondMathNode.EffectiveScriptBaselineY;

    public override void ModifyChildMathFormat(int childIndex, MathFormatValues childMathFormat)
    {
        if (childIndex == 1)
        {
            childMathFormat.FontSize = FontManager.GetScaledFontSize(MathFormat, FontConstants.ScriptFontSizeRatio);
        }
    }

    protected override void OnSecondOperandChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnSecondOperandChanged(e);
        double scaledFontSize = FontManager.GetScaledFontSize(MathFormat, FontConstants.ScriptFontSizeRatio);
        DebugTools.DevAssert(SecondMathNode.MathFormat.FontSize == scaledFontSize, null);
    }

    public VisualSuperscriptBinaryNode()
    {
    }
}
