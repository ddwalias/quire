using System;
using System.Windows;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualDerivativeOperatorNodeBase
{
    public VisualMathNode[] NodeChildren => _nodeChildren;

    public VisualDerivativeSymbolNode OperatorSymbol => _derivativeSymbol;

    public override double TextBaselineY
    {
        get
        {
            if (_layout == null)
            {
                return 0.0;
            }

            return _layout.TextBaselineOffset;
        }
    }

    public override double EffectiveBaselineY => Math.Max(((VisualMathNode)_derivativeSymbol).EffectiveBaselineY, _nodeChildren[2].EffectiveBaselineY);

    public override double EffectiveScriptBaselineY => Math.Min(((VisualMathNode)_derivativeSymbol).EffectiveScriptBaselineY, _nodeChildren[2].EffectiveScriptBaselineY);

    public override VisualMathNode OperatorElement => OperatorSymbol;

    public IVisualMathNode Operator => OperatorSymbol;

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[] { ((VisualUnaryOperatorNodeForEditor)this).OperatorElement };
}
