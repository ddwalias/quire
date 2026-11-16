using Quire.EquationEditor.Tree.Navigation;

namespace Quire.Native.PtcEquationEditor;

internal abstract partial class bf : a0
{
    protected bf(a5 A_0)
        : base(A_0)
    {
    }

    public new NavigatorState b()
    {
        ar ar2 = base.b();
        if (ar2.d())
        {
            return ar2.e();
        }

        return NavigatorState.Empty;
    }

    public new NavigatorState a()
    {
        ar ar2 = base.a();
        if (ar2.d())
        {
            return ar2.e();
        }

        return NavigatorState.Empty;
    }

    protected override bool br()
    {
        NavigatorState navigatorState = bq();
        if (NavigatorState.Empty.Equals(navigatorState) || navigatorState.Current == null || !navigatorState.Current.IsValid)
        {
            a(new ar(NavigatorState.Empty));
            return false;
        }

        a(new ar(navigatorState));
        return true;
    }

    protected abstract NavigatorState bq();
}
