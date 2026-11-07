namespace Quire.EquationEditor
{
    public partial class TypeoverChangeEventArgs
    {
        public TypeoverChangeEventArgs(Tree.Scoping.TypeoverState typeoverState)
        {
            State = typeoverState;
        }
    }

    public partial class NavigatorStateChangeEventArgs
    {
        public NavigatorStateChangeEventArgs(Tree.Navigation.NavigatorState navigatorState)
        {
            NavigatorState = navigatorState;
        }
    }
}

namespace Quire.EquationEditor.Equation
{
    public partial class ScopeStateChangeEventArgs
    {
        public ScopeStateChangeEventArgs(global::Quire.Direction caretSide, global::Quire.EquationEditor.Tree.Scoping.ScopedNodes scopedNodes)
        {
            CaretSide = caretSide;
            ScopedNodes = scopedNodes;
        }
    }
}
