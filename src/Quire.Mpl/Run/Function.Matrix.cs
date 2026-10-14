#nullable disable
using Quire.Mcd;
using Quire.Mirror;

namespace Quire.Mpl.Run;

public abstract partial class Function
{
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    private static object OptArg;
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    private static char[] label_delimiter;
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    static Function() { OptArg = null; label_delimiter = new[] { '#' }; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public bool IsFullyApplied(int n) => m_varargs || (m_minargs <= n && n <= m_purity);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void AssertFullApp(int n) { if (!IsFullyApplied(n)) BadCall(n < m_minargs ? m_minargs : m_purity, n); }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void AssertFullApp(int n, uint errpos) { if (!IsFullyApplied(n)) BadCall(n < m_minargs ? m_minargs : m_purity, n, errpos); }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override object Apply(object[] args) { appVec(out object result, args); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual F<R, A> AsF<R, A>(uint errpos)
    {
        AssertFullApp(1, errpos);
        return value => { inv(out R result, value); return result; };
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual F<R, A, B> AsF<R, A, B>(uint errpos)
    {
        AssertFullApp(2, errpos);
        return (first, second) => { inv(out R result, first, second); return result; };
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public F<object, object> AsDelegate_1() => AsF<object, object>(0u);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public F<object, object, object> AsDelegate_2() => AsF<object, object, object>(0u);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public F<object, object> AsDelegate_1(uint errpos) => AsF<object, object>(errpos);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public F<object, object, object> AsDelegate_2(uint errpos) => AsF<object, object, object>(errpos);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    protected virtual Function clo<CLO>(CLO value) => new Fn_clo<CLO>(this, value);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    protected virtual Function clovec<CLO>(CLO[] args)
    {
        Function result = this;
        foreach (CLO value in args) result = result.clo(value);
        return result;
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R>(out R result) => invVec(out result, empty_vec);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R>(out R result)
    {
        if (m_purity == 0 || m_varargs) inv(out result);
        else if (m_minargs == 0) app(out result, OptArg);
        AppException.Raise("null_application");
        result = default;
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A>(out R result, A aa) => invVec(out result, new object[] { aa });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void app<R, A>(out R result, A aa)
    {
        if (m_purity == 1 || m_varargs) inv(out result, aa);
        else if (m_minargs > 1) result = (R)(object)clo(aa);
        else app(out result, aa, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A>(A aa) { app(out object result, aa); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B>(out R result, A aa, B bb) => invVec(out result, new object[] { aa, bb });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void app<R, A, B>(out R result, A aa, B bb)
    {
        if (m_purity == 2 || m_varargs) inv(out result, aa, bb);
        else if (m_purity < 2 || m_minargs > 2) ((Function)apply(aa)).app(out result, bb);
        else app(out result, aa, bb, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B>(A aa, B bb) { app(out object result, aa, bb); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C>(out R result, A aa, B bb, C cc) => invVec(out result, new object[] { aa, bb, cc });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void app<R, A, B, C>(out R result, A aa, B bb, C cc)
    {
        if (m_purity == 3 || m_varargs) inv(out result, aa, bb, cc);
        else if (m_purity < 3 || m_minargs > 3) ((Function)apply(aa, bb)).app(out result, cc);
        else app(out result, aa, bb, cc, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C>(A aa, B bb, C cc) { app(out object result, aa, bb, cc); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D>(out R result, A aa, B bb, C cc, D dd) => invVec(out result, new object[] { aa, bb, cc, dd });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void app<R, A, B, C, D>(out R result, A aa, B bb, C cc, D dd)
    {
        if (m_purity == 4 || m_varargs) inv(out result, aa, bb, cc, dd);
        else if (m_purity < 4 || m_minargs > 4) ((Function)apply(aa, bb, cc)).app(out result, dd);
        else app(out result, aa, bb, cc, dd, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D>(A aa, B bb, C cc, D dd) { app(out object result, aa, bb, cc, dd); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E>(out R result, A aa, B bb, C cc, D dd, E ee) => invVec(out result, new object[] { aa, bb, cc, dd, ee });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E>(out R result, A aa, B bb, C cc, D dd, E ee)
    {
        if (m_purity == 5 || m_varargs) inv(out result, aa, bb, cc, dd, ee);
        else if (m_purity < 5 || m_minargs > 5) ((Function)apply(aa, bb, cc, dd)).app(out result, ee);
        else app(out result, aa, bb, cc, dd, ee, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E>(A aa, B bb, C cc, D dd, E ee) { app(out object result, aa, bb, cc, dd, ee); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F>(out R result, A aa, B bb, C cc, D dd, E ee, F ff) => invVec(out result, new object[] { aa, bb, cc, dd, ee, ff });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F>(out R result, A aa, B bb, C cc, D dd, E ee, F ff)
    {
        if (m_purity == 6 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff);
        else if (m_purity < 6 || m_minargs > 6) ((Function)apply(aa, bb, cc, dd, ee)).app(out result, ff);
        else app(out result, aa, bb, cc, dd, ee, ff, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F>(A aa, B bb, C cc, D dd, E ee, F ff) { app(out object result, aa, bb, cc, dd, ee, ff); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg) => invVec(out result, new object[] { aa, bb, cc, dd, ee, ff, gg });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg)
    {
        if (m_purity == 7 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg);
        else if (m_purity < 7 || m_minargs > 7) ((Function)apply(aa, bb, cc, dd, ee, ff)).app(out result, gg);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G>(A aa, B bb, C cc, D dd, E ee, F ff, G gg) { app(out object result, aa, bb, cc, dd, ee, ff, gg); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh) => invVec(out result, new object[] { aa, bb, cc, dd, ee, ff, gg, hh });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh)
    {
        if (m_purity == 8 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh);
        else if (m_purity < 8 || m_minargs > 8) ((Function)apply(aa, bb, cc, dd, ee, ff, gg)).app(out result, hh);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii) => invVec(out result, new object[] { aa, bb, cc, dd, ee, ff, gg, hh, ii });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii)
    {
        if (m_purity == 9 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii);
        else if (m_purity < 9 || m_minargs > 9) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh)).app(out result, ii);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj) => invVec(out result, new object[] { aa, bb, cc, dd, ee, ff, gg, hh, ii, jj });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj)
    {
        if (m_purity == 10 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj);
        else if (m_purity < 10 || m_minargs > 10) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii)).app(out result, jj);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk) => invVec(out result, new object[] { aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk)
    {
        if (m_purity == 11 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk);
        else if (m_purity < 11 || m_minargs > 11) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj)).app(out result, kk);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K, L>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll) => invVec(out result, new object[] { aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K, L>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll)
    {
        if (m_purity == 12 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll);
        else if (m_purity < 12 || m_minargs > 12) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk)).app(out result, ll);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K, L>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm) => invVec(out result, new object[] { aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K, L, M>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm)
    {
        if (m_purity == 13 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm);
        else if (m_purity < 13 || m_minargs > 13) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll)).app(out result, mm);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K, L, M>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn) => invVec(out result, new object[] { aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn)
    {
        if (m_purity == 14 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn);
        else if (m_purity < 14 || m_minargs > 14) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm)).app(out result, nn);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K, L, M, N>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo) => invVec(out result, new object[] { aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo)
    {
        if (m_purity == 15 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo);
        else if (m_purity < 15 || m_minargs > 15) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn)).app(out result, oo);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, OptArg);
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo, P pp) => invVec(out result, new object[] { aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp });
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo, P pp)
    {
        if (m_purity == 16 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp);
        else if (m_purity < 16 || m_minargs > 16) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo)).app(out result, pp);
        else result = default;
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo, P pp) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp); return result; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void appVec<R>(out R result, object[] args)
    {
        int count = args.Length;
        if (m_varargs) { invVec(out result, args); return; }
        switch (count)
        {
            case 1: app(out result, args[0]); return;
            case 2: app(out result, args[0], args[1]); return;
            case 3: app(out result, args[0], args[1], args[2]); return;
            case 4: app(out result, args[0], args[1], args[2], args[3]); return;
            case 5: app(out result, args[0], args[1], args[2], args[3], args[4]); return;
            case 6: app(out result, args[0], args[1], args[2], args[3], args[4], args[5]); return;
            case 7: app(out result, args[0], args[1], args[2], args[3], args[4], args[5], args[6]); return;
            case 8: app(out result, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]); return;
            case 9: app(out result, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]); return;
            case 10: app(out result, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]); return;
            case 11: app(out result, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]); return;
            case 12: app(out result, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]); return;
            case 13: app(out result, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]); return;
            case 14: app(out result, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]); return;
            case 15: app(out result, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14]); return;
            case 16: app(out result, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14], args[15]); return;
        }
        if (m_purity == count) invVec(out result, args);
        else if (m_purity < count)
        {
            invVec(out object next, take(m_purity, args));
            ((Function)next).appVec(out result, but(m_purity, args));
        }
        else if (m_minargs <= count)
        {
            var padded = new object[m_purity];
            Array.Copy(args, padded, count);
            invVec(out result, padded);
        }
        else result = (R)(object)clovec(args);
    }
}

public abstract partial class FunctionUtilities
{
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public static object[] empty_vec = new object[0];
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public static T[] take<T>(int i, T[] a)
    {
        if (i >= a.Length) return a;
        var result = new T[i];
        Array.Copy(a, result, i);
        return result;
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public static T[] but<T>(int i, T[] a)
    {
        if (i >= a.Length) return new T[0];
        var result = new T[a.Length - i];
        Array.Copy(a, i, result, 0, result.Length);
        return result;
    }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public static object[] PushFront(object b, object[] a)
    {
        var result = new object[a.Length + 1];
        a.CopyTo(result, 1);
        result[0] = b;
        return result;
    }
}

public partial class Fn_clo<CLO> : Function
{
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    private Function m_func;
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    private CLO m_clo;
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override string Name => $"{m_func.Name}[{m_clo.ToString()}]";
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public Fn_clo(Function f, CLO clo) : base(f.Purity, 1) { m_func = f; m_clo = clo; }
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void invVec<R>(out R result, object[] args) => m_func.appVec(out result, PushFront(m_clo, args));
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R>(out R result) => m_func.app(out result, m_clo);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A>(out R result, A aa) => m_func.app(out result, m_clo, aa);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B>(out R result, A aa, B bb) => m_func.app(out result, m_clo, aa, bb);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C>(out R result, A aa, B bb, C cc) => m_func.app(out result, m_clo, aa, bb, cc);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D>(out R result, A aa, B bb, C cc, D dd) => m_func.app(out result, m_clo, aa, bb, cc, dd);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D, E>(out R result, A aa, B bb, C cc, D dd, E ee) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D, E, F>(out R result, A aa, B bb, C cc, D dd, E ee, F ff) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D, E, F, G>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D, E, F, G, H>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D, E, F, G, H, I>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn);
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo) => m_func.app(out result, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo);
}
