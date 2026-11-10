namespace Quire.EquationEditor.Tree.Scoping;

public partial class LinearSegment
{
    private int a(global::Quire.Direction side)
    {
        return side != global::Quire.Direction.Left ? 1 : 0;
    }
}
