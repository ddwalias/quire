namespace Quire.EquationEditor.Tree.Scoping;

public partial class LinearSegment
{
    public MathNode GetNodeFromDirection(global::Quire.Direction side)
    {
        if (side == global::Quire.Direction.Indeterminate)
        {
            throw new global::System.ArgumentException("choose a side dammit!", "side");
        }

        return c[a(side)].b();
    }

    // Mathcad's setter has .initlocals False.
    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    public void SetNode(global::Quire.Direction side, MathNode newNode)
    {
        if (newNode == null)
        {
            throw new global::System.ArgumentNullException("newNode", "must specify a valid node");
        }

        if (side == global::Quire.Direction.Indeterminate)
        {
            throw new global::System.ArgumentException("must specify a valid side of the segment to change", "side");
        }

        c[a(side)] = new global::Quire.Native.PtcEquationEditor.s(newNode);
    }

    private int a(global::Quire.Direction side)
    {
        return side != global::Quire.Direction.Left ? 1 : 0;
    }
}
