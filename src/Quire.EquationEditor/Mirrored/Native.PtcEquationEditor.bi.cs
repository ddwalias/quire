using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;

namespace Quire.Native.PtcEquationEditor;

internal partial class bi
{
    private readonly bool a;
    private readonly MathNode b;
    private readonly MathNode c;
    private readonly a2 d;
    private readonly NavigatorState? e;

    public bi(a5 host, bool flag, MathNode start, MathNode end)
        : base(host)
    {
        d = new a2(host);
        a = flag;
        b = start;
        e = null;
        c = end;
    }

    public bi(a5 host, bool flag, MathNode start, NavigatorState? state, MathNode end)
        : base(host)
    {
        d = new a2(host);
        a = flag;
        b = start;
        e = state;
        c = end;
    }
}
