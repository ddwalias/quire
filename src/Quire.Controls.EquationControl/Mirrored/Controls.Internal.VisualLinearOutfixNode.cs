using Quire.Controls.Core;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualLinearOutfixNode
{
    public override double TextBaselineY => _child.TextBaselineY + _y;

    public override double EffectiveBaselineY => _child.EffectiveBaselineY + _y;

    public override double EffectiveScriptBaselineY => ExtendedFont.SafeComputeInkBoundingBox(Font.n_Run).Height;

    public override IVisualMathNode LeftFence => _leftFence;

    public override IVisualMathNode RightFence => _rightFence;

    public IVisualMathNode Child
    {
        get => _child;
        set
        {
            Children.Clear();
            _child = (VisualMathNode)value;
            Children.Add(_leftFence);
            Children.Add(_child);
            Children.Add(_rightFence);
        }
    }
}
