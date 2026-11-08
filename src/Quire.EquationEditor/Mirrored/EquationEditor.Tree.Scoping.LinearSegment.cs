namespace Quire.EquationEditor.Tree.Scoping;

public partial class LinearSegment
{
    public MathNode LeftNode
    {
        get => c[0].b();
        set
        {
            if (value == null)
            {
                throw new System.ArgumentNullException("value", "must specify a valid left node");
            }

            c[0] = new Quire.Native.PtcEquationEditor.s(value);
        }
    }

    public MathNode RightNode
    {
        get => c[1].b();
        set
        {
            if (value == null)
            {
                throw new System.ArgumentNullException("value", "must specify a valid right node");
            }

            c[1] = new Quire.Native.PtcEquationEditor.s(value);
        }
    }
}
