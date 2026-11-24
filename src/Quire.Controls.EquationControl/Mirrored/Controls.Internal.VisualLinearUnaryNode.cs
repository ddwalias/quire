using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualLinearUnaryNode
{
    public override double EffectiveBaselineY => ((VisualUnaryOperatorNodeForEditor)this).Child.EffectiveBaselineY;

    public override double EffectiveScriptBaselineY => ((VisualUnaryOperatorNodeForEditor)this).Child.EffectiveScriptBaselineY;

    public override double TextBaselineY => ((VisualUnaryOperatorNodeForEditor)this).Child.TextBaselineY;

    public override VisualMathNode OperatorElement => _operator;

    public IVisualMathNode Operator => _operator;

    public override IVisualMathNode this[int index]
    {
        get
        {
            if (index == 0)
            {
                // Mathcad reads the base accessor with callvirt; the cast binds the call to VisualUnaryOperatorNodeForEditor.Child.
                return ((VisualUnaryOperatorNodeForEditor)this).Child;
            }

            return null;
        }

        set
        {
            if (index == 0)
            {
                // Same accessor binding as the getter above.
                ((VisualUnaryOperatorNodeForEditor)this).Child = value as VisualMathNode;
                return;
            }

            base[index] = value;
        }
    }
}
