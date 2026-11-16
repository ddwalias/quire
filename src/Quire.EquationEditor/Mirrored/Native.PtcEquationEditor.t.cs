using System;
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;

namespace Quire.Native.PtcEquationEditor;

internal abstract partial class t : IResultNode, IVisitable<IResultNodeVisitor>, ICloneable
{
    public abstract MathNode bo(IResultTreeFormatter A_0);

    public abstract t bp();

    object ICloneable.Clone()
    {
        return bp();
    }

    public abstract void Accept(IResultNodeVisitor visitor);

    protected t()
    {
    }
}
