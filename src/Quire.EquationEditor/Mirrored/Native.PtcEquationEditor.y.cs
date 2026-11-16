using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;

namespace Quire.Native.PtcEquationEditor;

internal partial class y : t, IComplexResultNode, IResultNode, IVisitable<IResultNodeVisitor>
{
    private double a;
    private double b;

    public double RValue
    {
        get => a;
        set => a = value;
    }

    public double IValue
    {
        get => b;
        set => b = value;
    }

    public override MathNode bo(IResultTreeFormatter A_0)
    {
        return A_0.FormatResult(this);
    }

    public override t bp()
    {
        return new y { RValue = RValue, IValue = IValue };
    }

    public override void Accept(IResultNodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public y()
    {
    }
}
