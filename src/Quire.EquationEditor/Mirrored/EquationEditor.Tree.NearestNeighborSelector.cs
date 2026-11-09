namespace Quire.EquationEditor.Tree;

public partial class NearestNeighborSelector
{
    public MathNode[] SelectNeighbors(MathNode current)
    {
        bool neighborsShareParent;
        return SelectNeighbors(current, out neighborsShareParent);
    }
}
