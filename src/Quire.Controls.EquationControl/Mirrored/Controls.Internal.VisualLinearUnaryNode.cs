using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualLinearUnaryNode
{
    public override double EffectiveBaselineY => ((VisualUnaryOperatorNodeForEditor)this).Child.EffectiveBaselineY;

    public override double EffectiveScriptBaselineY => ((VisualUnaryOperatorNodeForEditor)this).Child.EffectiveScriptBaselineY;

    public override double TextBaselineY => ((VisualUnaryOperatorNodeForEditor)this).Child.TextBaselineY;

    public override VisualMathNode OperatorElement => _operator;

    public IVisualMathNode Operator => _operator;
}
