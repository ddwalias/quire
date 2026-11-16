namespace Quire.EquationEditor.Tree.Scoping;

public partial class DetachedTypeoverState
{
    private bool a;
    private readonly Quire.Native.PtcEquationEditor.br b;
    private readonly Quire.Native.PtcEquationEditor.br c;
    private readonly Quire.Native.PtcEquationEditor.br d;
    private readonly bool e;

    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    public DetachedTypeoverState(TypeoverState typeoverState)
    {
        a = typeoverState == null;
        if (!a)
        {
            b = new Quire.Native.PtcEquationEditor.br(typeoverState.LeftNavigatorState);
            c = new Quire.Native.PtcEquationEditor.br(typeoverState.RightNavigatorState);
            d = new Quire.Native.PtcEquationEditor.br(typeoverState.NavState);
            e = typeoverState.IncludePrimalChildren;
        }
    }

    public TypeoverState GetClosestTypeoverState()
    {
        if (a)
        {
            return null;
        }

        return new TypeoverState(b.a(), c.a(), d.a(), e);
    }
}
