namespace Quire.EquationEditor.Tree.Navigation;

public partial struct NavigatorState
{
    static NavigatorState()
    {
        Empty = new NavigatorState(null, global::Quire.Direction.Indeterminate, global::Quire.Direction.Indeterminate);
    }
}
