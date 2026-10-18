#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class Fn_CurryTuple
{
    [MirrorAnalyzed("Mpl.Run.Fn_CurryTuple")]
    private Function m_f;

    [MirrorVerified("FnCurryTupleDifferentialTests")]
    public override string Name => m_name;

    [MirrorVerified("FnCurryTupleDifferentialTests")]
    public Fn_CurryTuple(Function f, int purity, object name)
        : base(purity, 0)
    {
        m_name = name.ToString();
        m_varargs = false;
        m_f = f;
    }

    [MirrorVerified("FnCurryTupleDifferentialTests")]
    public override void invVec<R>(out R r, object[] args)
    {
        AssertFullApp(args.Length);
        m_f.inv<R, object[]>(out r, args);
    }

    [MirrorVerified("FnCurryTupleDifferentialTests")]
    public override object Apply(object[] args)
    {
        invVec<object>(out var r, args);
        return r;
    }

    [MirrorVerified("FnCurryTupleDifferentialTests")]
    public override object DynApply(object[] args)
    {
        invVec<object>(out var r, args);
        return r;
    }
}
