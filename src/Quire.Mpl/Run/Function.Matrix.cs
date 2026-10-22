#nullable disable
using Quire.Mcd;
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class Fn_clo<CLO> : Function
{
    [MirrorAnalyzed("Mpl.Run.Fn_clo")]
    private Function m_func;
    [MirrorAnalyzed("Mpl.Run.Fn_clo")]
    private CLO m_clo;
    [MirrorVerified("FnCloDifferentialTests")]
    public override string Name => string.Format("{0}[{1}]", m_func.Name, m_clo.ToString());
    [MirrorVerified("FnCloDifferentialTests")]
    public Fn_clo(Function f, CLO clo) : base(f.Purity, 1) { m_func = f; m_clo = clo; }
    [MirrorVerified("FnCloDifferentialTests")]
    public Fn_clo(Delegate f, CLO clo, int function_purity) : base(function_purity, 1)
    {
        m_func = DelegateFunction.make(f, function_purity);
        m_clo = clo;
    }
    [MirrorVerified("FnCloDifferentialTests")]
    public new static Function Fixpoint(Function f)
    {
        Fn_clo<object> fn_clo = new Fn_clo<object>(f, null);
        fn_clo.m_clo = fn_clo;
        return fn_clo;
    }
    [MirrorVerified("FnCloDifferentialTests")]
    public override void invVec<R>(out R result, object[] args) => m_func.appVec(out result, FunctionUtilities.PushFront(m_clo, args));
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R>(out R result) => m_func.app(out result, m_clo);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A>(out R result, A aa) => m_func.app(out result, m_clo, aa);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B>(out R result, A aa, B bb) => m_func.app(out result, m_clo, aa, bb);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C>(out R result, A aa, B bb, C cc) => m_func.app(out result, m_clo, aa, bb, cc);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D>(out R result, A aa, B bb, C cc, D dd) => m_func.app(out result, m_clo, aa, bb, cc, dd);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D, E>(out R result, A aa, B bb, C cc, D dd, E ee) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D, E, F>(out R result, A aa, B bb, C cc, D dd, E ee, F ff) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D, E, F, G>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D, E, F, G, H>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D, E, F, G, H, I>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn);
    [MirrorVerified("FnCloDifferentialTests")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo);
}
