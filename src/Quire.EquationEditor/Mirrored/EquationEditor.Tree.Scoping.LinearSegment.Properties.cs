namespace Quire.EquationEditor.Tree.Scoping;

public partial class LinearSegment
{
    public bool SegmentIsSingleNode => LeftNode == RightNode;

    internal bool HasNodes
    {
        get
        {
            if (LeftNode != null)
            {
                return RightNode != null;
            }

            return false;
        }
    }
}
