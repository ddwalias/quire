#nullable disable
using System;
using System.Reflection;
using Quire.Mcd;
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>
/// A callable value. <see cref="MinArgs"/> arguments are required; a <see cref="Varargs"/> function accepts more.
/// Subclasses implement <see cref="invVec{R}"/>; typed call forms also support virtual overrides.
/// </summary>
public abstract partial class Function : FunctionUtilities
{
    [MirrorAnalyzed("Mpl.Run.Function")]
    protected int m_purity;

    [MirrorAnalyzed("Mpl.Run.Function")]
    protected int m_n;

    [MirrorAnalyzed("Mpl.Run.Function")]
    protected bool m_no_vectorize;

    [MirrorAnalyzed("Mpl.Run.Function")]
    protected int m_minargs;

    [MirrorAnalyzed("Mpl.Run.Function")]
    protected bool m_varargs;

    [MirrorAnalyzed("Mpl.Run.Function")]
    protected string m_name;

    [MirrorAnalyzed("Mpl.Run.Function")]
    private Function m_inverse_function;

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    private static object OptArg;

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    private static char[] label_delimiter;

    [MirrorVerified("FunctionDifferentialTests")]
    public int Purity => m_purity;

    [MirrorVerified("FunctionDifferentialTests")]
    public int MinArgs => m_minargs;

    [MirrorVerified("FunctionDifferentialTests")]
    public bool Varargs => m_varargs;

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual string Name
    {
        get => m_name == null ? "<anonymous>" : m_name;
        set => m_name = value;
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override string ToString() => string.Format("Function: {0}", Name);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static string format_vector(object[] v)
    {
        if (v == null || v.Length == 0)
            return "[]";
        if (v.Length == 1)
            return string.Format("[{0}]", v[0]);
        string text = v[0].ToString();
        for (int i = 1; i < v.Length; i++)
            text = string.Format("{0},{1}", text, v[i]);
        return string.Format("[{0}]", text);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static string canonical_name(string nm)
    {
        if (nm == null)
            return null;
        string text = nm;
        if (text.StartsWith("mc_"))
            text = nm.Substring(3);
        else if (nm.StartsWith("ui_"))
            text = "anonymous";
        if (nm.IndexOf('#') > 0)
        {
            string[] array = text.Split(label_delimiter);
            text = string.Format("{0}{1}{2}", array[0], "{", array[1]);
            for (int i = 2; i < array.Length; i++)
                text = string.Format("{0},{1}", text, array[i]);
            text = string.Format("{0}{1}", text, "}");
        }
        return text;
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    static Function() { OptArg = null; label_delimiter = ['#']; }

    /// <param name="function_purity">The original function purity/arity counter before captures.</param>
    /// <param name="n">The captured-argument count subtracted from that counter.</param>
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    protected Function(int function_purity, int n)
    {
        m_n = n;
        m_purity = function_purity - m_n;
        m_minargs = m_purity;
        m_no_vectorize = false;
        m_dim = Dim.function_dim;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    private Function() { }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public Function IgnoreVectorization()
    {
        m_no_vectorize = true;
        return this;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public Function IgnoreVectorization(bool b)
    {
        m_no_vectorize = b;
        return this;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public Function OptFunc(int minargs)
    {
        m_minargs = minargs;
        return this;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public Function InverseFunction
    {
        get
        {
            if (m_inverse_function == null)
                AppException.Raise("no inverse declared");
            return m_inverse_function;
        }
        set => m_inverse_function = value;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public Function DeclareInverse(Function f)
    {
        InverseFunction = f;
        f.InverseFunction = this;
        return f;
    }

    [MirrorVerified("FunctionDifferentialTests")]
    public override Function IsFunction => this;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override Function asFunction(string m, uint a) => this;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override Function IsFunctionN(int argc)
    {
        if (!IsFullyApplied(argc))
        {
            return null;
        }
        return this;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override Function asFunctionN(int argc, uint errpos)
    {
        AssertFullApp(argc, errpos);
        return this;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static void CheckPremium()
    {
        if (RuntimeOptions.s_restricted_mode)
            AppException.Raise("premium_only");
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public virtual Function Premium() => new PremiumFunction(this).IgnoreVectorization(m_no_vectorize);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall()
    {
        AppException.Raise("Not Implemented");
        return null;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa)
    {
        AssertFullApp(1);
        app<object, object>(out var r, aa);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb)
    {
        AssertFullApp(2);
        app<object, object, object>(out var r, aa, bb);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc)
    {
        AssertFullApp(3);
        app<object, object, object, object>(out var r, aa, bb, cc);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd)
    {
        AssertFullApp(4);
        app<object, object, object, object, object>(out var r, aa, bb, cc, dd);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee)
    {
        AssertFullApp(5);
        app<object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff)
    {
        AssertFullApp(6);
        app<object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg)
    {
        AssertFullApp(7);
        app<object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh)
    {
        AssertFullApp(8);
        app<object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii)
    {
        AssertFullApp(9);
        app<object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj)
    {
        AssertFullApp(10);
        app<object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk)
    {
        AssertFullApp(11);
        app<object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll)
    {
        AssertFullApp(12);
        app<object, object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm)
    {
        AssertFullApp(13);
        app<object, object, object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm, object nn)
    {
        AssertFullApp(14);
        app<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm, object nn, object oo)
    {
        AssertFullApp(15);
        app<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm, object nn, object oo, object pp)
    {
        AssertFullApp(16);
        app<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call()
    {
        AppException.Raise("Not Implemented");
        return null;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa)
    {
        app<object, object>(out var r, aa);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb)
    {
        app<object, object, object>(out var r, aa, bb);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc)
    {
        app<object, object, object, object>(out var r, aa, bb, cc);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd)
    {
        app<object, object, object, object, object>(out var r, aa, bb, cc, dd);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee)
    {
        app<object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee, object ff)
    {
        app<object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee, object ff, object gg)
    {
        app<object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh)
    {
        app<object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii)
    {
        app<object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj)
    {
        app<object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk)
    {
        app<object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll)
    {
        app<object, object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm)
    {
        app<object, object, object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm, object nn)
    {
        app<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm, object nn, object oo)
    {
        app<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Call(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm, object nn, object oo, object pp)
    {
        app<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_0<R>()
    {
        app<R>(out var r);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_1<R, A>(A aa)
    {
        app<R, A>(out var r, aa);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_2<R, A, B>(A aa, B bb)
    {
        app<R, A, B>(out var r, aa, bb);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_3<R, A, B, C>(A aa, B bb, C cc)
    {
        app<R, A, B, C>(out var r, aa, bb, cc);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_4<R, A, B, C, D>(A aa, B bb, C cc, D dd)
    {
        app<R, A, B, C, D>(out var r, aa, bb, cc, dd);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_5<R, A, B, C, D, E>(A aa, B bb, C cc, D dd, E ee)
    {
        app<R, A, B, C, D, E>(out var r, aa, bb, cc, dd, ee);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_6<R, A, B, C, D, E, F>(A aa, B bb, C cc, D dd, E ee, F ff)
    {
        app<R, A, B, C, D, E, F>(out var r, aa, bb, cc, dd, ee, ff);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_7<R, A, B, C, D, E, F, G>(A aa, B bb, C cc, D dd, E ee, F ff, G gg)
    {
        app<R, A, B, C, D, E, F, G>(out var r, aa, bb, cc, dd, ee, ff, gg);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_8<R, A, B, C, D, E, F, G, H>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh)
    {
        app<R, A, B, C, D, E, F, G, H>(out var r, aa, bb, cc, dd, ee, ff, gg, hh);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_9<R, A, B, C, D, E, F, G, H, I>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii)
    {
        app<R, A, B, C, D, E, F, G, H, I>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_10<R, A, B, C, D, E, F, G, H, I, J>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj)
    {
        app<R, A, B, C, D, E, F, G, H, I, J>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_11<R, A, B, C, D, E, F, G, H, I, J, K>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk)
    {
        app<R, A, B, C, D, E, F, G, H, I, J, K>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_12<R, A, B, C, D, E, F, G, H, I, J, K, L>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll)
    {
        app<R, A, B, C, D, E, F, G, H, I, J, K, L>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_13<R, A, B, C, D, E, F, G, H, I, J, K, L, M>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm)
    {
        app<R, A, B, C, D, E, F, G, H, I, J, K, L, M>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_14<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn)
    {
        app<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_15<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo)
    {
        app<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_16<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo, P pp)
    {
        app<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(out var r, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A>(A aa) { app(out object result, aa); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B>(A aa, B bb) { app(out object result, aa, bb); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C>(A aa, B bb, C cc) { app(out object result, aa, bb, cc); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D>(A aa, B bb, C cc, D dd) { app(out object result, aa, bb, cc, dd); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E>(A aa, B bb, C cc, D dd, E ee) { app(out object result, aa, bb, cc, dd, ee); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F>(A aa, B bb, C cc, D dd, E ee, F ff) { app(out object result, aa, bb, cc, dd, ee, ff); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G>(A aa, B bb, C cc, D dd, E ee, F ff, G gg) { app(out object result, aa, bb, cc, dd, ee, ff, gg); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K, L>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K, L, M>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K, L, M, N>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo); return result; }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public object apply<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo, P pp) { app(out object result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp); return result; }

    [MirrorVerified("FunctionClosureDirectDifferentialTests")]
    public static Function closure<CLO>(Delegate d, CLO clo, int purity)
    {
        switch (purity)
        {
            case 2:
                return Delegates.FunctionClosure_1(d, clo);
            case 3:
                return Delegates.FunctionClosure_2(d, clo);
            case 4:
                return Delegates.FunctionClosure_3(d, clo);
            case 5:
                return Delegates.FunctionClosure_4(d, clo);
            default:
                return new Fn_clo<CLO>(d, clo, purity);
        }
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    protected virtual Function clo<CLO>(CLO a) => new Fn_clo<CLO>(this, a);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    protected virtual Function clovec<CLO>(CLO[] args)
    {
        Function function = this;
        int num = args.Length;
        for (int i = 0; i < num; i++)
        {
            function = function.clo(args[i]);
        }
        return function;
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public bool IsFullyApplied(int n) => m_varargs || (m_minargs <= n && n <= m_purity);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void AssertFullApp(int n)
    {
        if (IsFullyApplied(n))
        {
            return;
        }
        BadCall(n < m_minargs ? m_minargs : m_purity, n);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void AssertFullApp(int n, uint errpos)
    {
        if (IsFullyApplied(n))
        {
            return;
        }
        BadCall(n < m_minargs ? m_minargs : m_purity, n, errpos);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void appVec<R>(out R r, object[] args)
    {
        int num = args.Length;
        if (m_varargs)
        {
            invVec(out r, args);
            return;
        }
        switch (num)
        {
            case 1: app(out r, args[0]); return;
            case 2: app(out r, args[0], args[1]); return;
            case 3: app(out r, args[0], args[1], args[2]); return;
            case 4: app(out r, args[0], args[1], args[2], args[3]); return;
            case 5: app(out r, args[0], args[1], args[2], args[3], args[4]); return;
            case 6: app(out r, args[0], args[1], args[2], args[3], args[4], args[5]); return;
            case 7: app(out r, args[0], args[1], args[2], args[3], args[4], args[5], args[6]); return;
            case 8: app(out r, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]); return;
            case 9: app(out r, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]); return;
            case 10: app(out r, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]); return;
            case 11: app(out r, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]); return;
            case 12: app(out r, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]); return;
            case 13: app(out r, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]); return;
            case 14: app(out r, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]); return;
            case 15: app(out r, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14]); return;
            case 16: app(out r, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14], args[15]); return;
        }
        if (m_purity == num)
        {
            invVec(out r, args);
        }
        else if (m_purity < num)
        {
            object[] args2 = FunctionUtilities.take(m_purity, args);
            invVec<object>(out var r2, args2);
            ((Function)r2).appVec(out r, FunctionUtilities.but(m_purity, args));
        }
        else if (m_minargs <= num)
        {
            object[] array = new object[m_purity];
            Array.Copy(args, array, num);
            invVec(out r, array);
        }
        else
        {
            r = (R)(object)clovec(args);
        }
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R>(out R result)
    {
        if (m_purity == 0 || m_varargs) inv(out result);
        else if (m_minargs == 0) app(out result, OptArg);
        AppException.Raise("null_application");
        result = default;
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void app<R, A>(out R result, A aa)
    {
        if (m_purity == 1 || m_varargs) inv(out result, aa);
        else if (m_minargs > 1) result = (R)(object)clo(aa);
        else app(out result, aa, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void app<R, A, B>(out R result, A aa, B bb)
    {
        if (m_purity == 2 || m_varargs) inv(out result, aa, bb);
        else if (m_purity < 2 || m_minargs > 2) ((Function)apply(aa)).app(out result, bb);
        else app(out result, aa, bb, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void app<R, A, B, C>(out R result, A aa, B bb, C cc)
    {
        if (m_purity == 3 || m_varargs) inv(out result, aa, bb, cc);
        else if (m_purity < 3 || m_minargs > 3) ((Function)apply(aa, bb)).app(out result, cc);
        else app(out result, aa, bb, cc, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void app<R, A, B, C, D>(out R result, A aa, B bb, C cc, D dd)
    {
        if (m_purity == 4 || m_varargs) inv(out result, aa, bb, cc, dd);
        else if (m_purity < 4 || m_minargs > 4) ((Function)apply(aa, bb, cc)).app(out result, dd);
        else app(out result, aa, bb, cc, dd, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E>(out R result, A aa, B bb, C cc, D dd, E ee)
    {
        if (m_purity == 5 || m_varargs) inv(out result, aa, bb, cc, dd, ee);
        else if (m_purity < 5 || m_minargs > 5) ((Function)apply(aa, bb, cc, dd)).app(out result, ee);
        else app(out result, aa, bb, cc, dd, ee, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F>(out R result, A aa, B bb, C cc, D dd, E ee, F ff)
    {
        if (m_purity == 6 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff);
        else if (m_purity < 6 || m_minargs > 6) ((Function)apply(aa, bb, cc, dd, ee)).app(out result, ff);
        else app(out result, aa, bb, cc, dd, ee, ff, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg)
    {
        if (m_purity == 7 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg);
        else if (m_purity < 7 || m_minargs > 7) ((Function)apply(aa, bb, cc, dd, ee, ff)).app(out result, gg);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh)
    {
        if (m_purity == 8 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh);
        else if (m_purity < 8 || m_minargs > 8) ((Function)apply(aa, bb, cc, dd, ee, ff, gg)).app(out result, hh);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii)
    {
        if (m_purity == 9 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii);
        else if (m_purity < 9 || m_minargs > 9) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh)).app(out result, ii);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj)
    {
        if (m_purity == 10 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj);
        else if (m_purity < 10 || m_minargs > 10) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii)).app(out result, jj);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk)
    {
        if (m_purity == 11 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk);
        else if (m_purity < 11 || m_minargs > 11) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj)).app(out result, kk);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K, L>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll)
    {
        if (m_purity == 12 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll);
        else if (m_purity < 12 || m_minargs > 12) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk)).app(out result, ll);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K, L, M>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm)
    {
        if (m_purity == 13 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm);
        else if (m_purity < 13 || m_minargs > 13) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll)).app(out result, mm);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn)
    {
        if (m_purity == 14 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn);
        else if (m_purity < 14 || m_minargs > 14) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm)).app(out result, nn);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo)
    {
        if (m_purity == 15 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo);
        else if (m_purity < 15 || m_minargs > 15) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn)).app(out result, oo);
        else app(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, OptArg);
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public void app<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo, P pp)
    {
        if (m_purity == 16 || m_varargs) inv(out result, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp);
        else if (m_purity < 16 || m_minargs > 16) ((Function)apply(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo)).app(out result, pp);
        else result = default;
    }

    /// <summary>Calls the function with <paramref name="args"/> and stores its result in <paramref name="r"/>.</summary>
    public abstract void invVec<R>(out R r, object[] args);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R>(out R result) => invVec(out result, empty_vec);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A>(out R result, A aa) => invVec(out result, [aa]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B>(out R result, A aa, B bb) => invVec(out result, [aa, bb]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C>(out R result, A aa, B bb, C cc) => invVec(out result, [aa, bb, cc]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D>(out R result, A aa, B bb, C cc, D dd) => invVec(out result, [aa, bb, cc, dd]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E>(out R result, A aa, B bb, C cc, D dd, E ee) => invVec(out result, [aa, bb, cc, dd, ee]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F>(out R result, A aa, B bb, C cc, D dd, E ee, F ff) => invVec(out result, [aa, bb, cc, dd, ee, ff]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg) => invVec(out result, [aa, bb, cc, dd, ee, ff, gg]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh) => invVec(out result, [aa, bb, cc, dd, ee, ff, gg, hh]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii) => invVec(out result, [aa, bb, cc, dd, ee, ff, gg, hh, ii]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj) => invVec(out result, [aa, bb, cc, dd, ee, ff, gg, hh, ii, jj]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk) => invVec(out result, [aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K, L>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll) => invVec(out result, [aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm) => invVec(out result, [aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn) => invVec(out result, [aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo) => invVec(out result, [aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo]);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(out R result, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo, P pp) => invVec(out result, [aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp]);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object VecCall(object b)
    {
        if (!(m_no_vectorize | (b as MatVal) == null))
        {
            return MatVal.vectorize(this, new object[1] { b });
        }
        return DynCall(b);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object VecCall(object b, object c)
    {
        if (!(m_no_vectorize | ((b as MatVal) == null & (c as MatVal) == null)))
        {
            return MatVal.vectorize(this, new object[2] { b, c });
        }
        return DynCall(b, c);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object VecCall(object b, object c, object d)
    {
        if (!(m_no_vectorize | ((b as MatVal) == null & (c as MatVal) == null & (d as MatVal) == null)))
        {
            return MatVal.vectorize(this, new object[3] { b, c, d });
        }
        return DynCall(b, c, d);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object VecApply(object[] args)
    {
        if (!m_no_vectorize) return MatVal.vectorize(this, args);
        return Apply(args);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynApply(object[] args)
    {
        AssertFullApp(args.Length);
        object r;
        try
        {
            appVec<object>(out r, args);
        }
        catch (TargetInvocationException ex)
        {
            if (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
            throw;
        }
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public R Call_vec<R>(object[] args)
    {
        appVec<R>(out var r, args);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override object Apply(object[] args) { appVec(out object result, args); return result; }

    [MirrorVerified("FunctionDifferentialTests")]
    public object InvokeVec(object[] args) => DynApply(args);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public Function CurryMatVal()
    {
        F<object, object> f = AsDelegate_1(1u);
        return new Fn_varargs(DelegateFunction.make((F<object, object>)(args => f(new MatVal((object[])args))), 1));
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public Function UnCurryMatVal() => DelegateFunction.make((F<object, object>)(m => Apply((m as DimVal).asMatrix(1u).PackObjects())), 1);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static Function ConstantFunction(object c) => new FnConstant(c);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static Function VectorFunction(object vv)
    {
        if (vv is not MatVal { Cols: 1 } matVal)
            return null;
        int r = matVal.Rows;
        bool anyFunction = false;
        Function[] fvec = new Function[r];
        for (int i = 0; i < r; i++)
        {
            object obj = matVal.get_1(i);
            if (obj is not Function function)
            {
                fvec[i] = ConstantFunction(obj);
                continue;
            }
            anyFunction = true;
            fvec[i] = function;
        }
        if (!anyFunction)
            return null;
        int num = 2;
        foreach (Function f in fvec)
        {
            if (!f.IsFullyApplied(2))
            {
                num = 1;
                break;
            }
        }
        if (num == 2)
        {
            return DelegateFunction.makeF((object x, object y) =>
            {
                MatVal matVal2 = MatVal.reserve(r);
                foreach (Function function2 in fvec)
                    matVal2.PushRaw(function2.DynCall(x, y));
                return matVal2;
            }, "vector function");
        }
        foreach (Function f in fvec)
        {
            if (!f.IsFullyApplied(1))
                return null;
        }
        return DelegateFunction.makeF((object x) =>
        {
            MatVal matVal2 = MatVal.reserve(r);
            foreach (Function function2 in fvec)
                matVal2.PushRaw(function2.DynCall(x));
            return matVal2;
        }, "vector function");
    }

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

    [MirrorAnalyzed("Mpl.Run.Function")]
    public virtual F<R, A, B, C> AsF<R, A, B, C>(uint errpos)
    {
        AssertFullApp(3, errpos);
        return (A x, B y, C z) =>
        {
            inv<R, A, B, C>(out var r, x, y, z);
            return r;
        };
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public virtual F<R, A, B, C, D> AsF<R, A, B, C, D>(uint errpos)
    {
        AssertFullApp(4, errpos);
        return (A x, B y, C z, D w) =>
        {
            inv<R, A, B, C, D>(out var r, x, y, z, w);
            return r;
        };
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public F<object, object> AsDelegate_1() => AsF<object, object>(0u);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public F<object, object, object> AsDelegate_2() => AsF<object, object, object>(0u);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public F<object, object, object, object> AsDelegate_3() => AsF<object, object, object, object>(0u);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public F<object, object, object, object, object> AsDelegate_4() => AsF<object, object, object, object, object>(0u);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public F<object, object> AsDelegate_1(uint errpos) => AsF<object, object>(errpos);

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public F<object, object, object> AsDelegate_2(uint errpos) => AsF<object, object, object>(errpos);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public F<object, object, object, object> AsDelegate_3(uint errpos) => AsF<object, object, object, object>(errpos);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public F<object, object, object, object, object> AsDelegate_4(uint errpos) => AsF<object, object, object, object, object>(errpos);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static Function Fixpoint(Function f) => Fn_clo<object>.Fixpoint(f);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static Function Compose(Function f, Function g) => new FnCompose(f, g);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Apply1(object arg) => Call_1<object, object>(arg);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Apply1(int arg) => Call_1<object, int>(arg);

    [MirrorAnalyzed("Mpl.Run.Function")]
    public object Apply1(double arg) => Call_1<object, double>(arg);
}
