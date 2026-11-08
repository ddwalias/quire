namespace Quire.EquationEditor.Tree.Scoping;

public partial class DetachedTypeoverState
{
    private bool a;
    private readonly Quire.Native.PtcEquationEditor.br b;
    private readonly Quire.Native.PtcEquationEditor.br c;
    private readonly Quire.Native.PtcEquationEditor.br d;
    private readonly bool e;

    public TypeoverState GetClosestTypeoverState()
    {
        if (a)
        {
            return null;
        }

        return new TypeoverState(b.a(), c.a(), d.a(), e);
    }
}
