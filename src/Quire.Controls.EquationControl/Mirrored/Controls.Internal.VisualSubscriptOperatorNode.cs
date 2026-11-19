using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Core;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualSubscriptOperatorNode
{
    private double _stapleTop;
    private double _secondOperandTop;
    private double _effectiveBaseline;
    private const double PAD = 1d;

    public override double TextBaselineY => FirstMathNode.TextBaselineY;

    public override double EffectiveBaselineY => _effectiveBaseline;

    public override double EffectiveScriptBaselineY => FirstMathNode.EffectiveScriptBaselineY;

    public VisualSubscriptOperatorNode()
        : base(new VisualSymbolNode('⊏'))
    {
        SetOperatorSymbolFontSize(MathFormat);
    }

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

    public override void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e)
    {
        base.HandleMathFormatChanged(e);
        SetOperatorSymbolFontSize(MathFormat);
    }

    private void SetOperatorSymbolFontSize(IMathFormat mathFormat)
    {
        _operator.MathFormat.FontSize = FontManager.GetScaledFontSize(mathFormat, FontConstants.ScriptFontSizeRatio);
    }
}
