#nullable disable
using System;
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class DelegateFunction : Function
{
    [MirrorAnalyzed("Mpl.Run.Function")]
    protected Delegate m_f;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override string Name
    {
        get
        {
            if (m_name != null)
                return m_name;
            return m_name = Function.canonical_name(m_f?.Method?.Name);
        }
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static DelegateFunction makeF(F<object, object> f, string name)
    {
        return new UnaryDelegateFunction(f)
        {
            Name = Function.canonical_name(name)
        };
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static DelegateFunction makeF(F<object, object, object> f, string name)
    {
        return new BinaryDelegateFunction(f)
        {
            Name = Function.canonical_name(name)
        };
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static DelegateFunction makeF(F<object, object, object, object> f, string name)
    {
        return new TernaryDelegateFunction(f)
        {
            Name = Function.canonical_name(name)
        };
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static DelegateFunction makeF(F<object, object, object, object, object> f, string name)
    {
        return new QuartnaryDelegateFunction(f)
        {
            Name = Function.canonical_name(name)
        };
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public static DelegateFunction make(Delegate f, int function_purity)
    {
        switch (function_purity)
        {
            case 1:
            {
                F<object, object> f1 = (F<object, object>)Delegates.toFDelegate(typeof(F<object, object>), f);
                if (f1 != null)
                    return makeF(f1, null);
                break;
            }
            case 2:
            {
                F<object, object, object> f2 = (F<object, object, object>)Delegates.toFDelegate(typeof(F<object, object, object>), f);
                if (f2 != null)
                    return makeF(f2, null);
                break;
            }
            case 3:
            {
                F<object, object, object, object> f3 = (F<object, object, object, object>)Delegates.toFDelegate(typeof(F<object, object, object, object>), f);
                if (f3 != null)
                    return makeF(f3, null);
                break;
            }
            case 4:
            {
                F<object, object, object, object, object> f4 = (F<object, object, object, object, object>)Delegates.toFDelegate(typeof(F<object, object, object, object, object>), f);
                if (f4 != null)
                    return makeF(f4, null);
                break;
            }
        }
        return new DelegateFunction(f, function_purity);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public DelegateFunction(Delegate f, int function_purity)
        : base(function_purity, 0)
    {
        m_f = f;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    private object app0()
    {
        if (m_purity == 0)
            return m_f.DynamicInvoke(FunctionUtilities.empty_vec);
        return this;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    protected override Function clo<CLO>(CLO a)
    {
        return Function.closure(m_f, a, m_purity);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    private string remove_module_suffix(string name)
    {
        StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
        if (name.EndsWith("R.dll", comparisonType) || name.EndsWith("D.dll", comparisonType))
        {
            name = name.Substring(0, name.Length - 5);
        }
        return name;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void invVec<R>(out R r, object[] args)
    {
        r = (R)m_f.DynamicInvoke(args);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R>(out R r)
    {
        F<R> f = m_f as F<R>;
        r = (f == null) ? (R)m_f.DynamicInvoke() : f();
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A>(out R r, A aa)
    {
        F<R, A> f = m_f as F<R, A>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa) : f(aa);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B>(out R r, A aa, B bb)
    {
        F<R, A, B> f = m_f as F<R, A, B>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb) : f(aa, bb);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C>(out R r, A aa, B bb, C cc)
    {
        F<R, A, B, C> f = m_f as F<R, A, B, C>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc) : f(aa, bb, cc);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D>(out R r, A aa, B bb, C cc, D dd)
    {
        F<R, A, B, C, D> f = m_f as F<R, A, B, C, D>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd) : f(aa, bb, cc, dd);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E>(out R r, A aa, B bb, C cc, D dd, E ee)
    {
        F<R, A, B, C, D, E> f = m_f as F<R, A, B, C, D, E>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee) : f(aa, bb, cc, dd, ee);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E, F>(out R r, A aa, B bb, C cc, D dd, E ee, F ff)
    {
        F<R, A, B, C, D, E, F> f = m_f as F<R, A, B, C, D, E, F>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee, ff) : f(aa, bb, cc, dd, ee, ff);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E, F, G>(out R r, A aa, B bb, C cc, D dd, E ee, F ff, G gg)
    {
        F<R, A, B, C, D, E, F, G> f = m_f as F<R, A, B, C, D, E, F, G>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee, ff, gg) : f(aa, bb, cc, dd, ee, ff, gg);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E, F, G, H>(out R r, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh)
    {
        F<R, A, B, C, D, E, F, G, H> f = m_f as F<R, A, B, C, D, E, F, G, H>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee, ff, gg, hh) : f(aa, bb, cc, dd, ee, ff, gg, hh);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E, F, G, H, I>(out R r, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii)
    {
        F<R, A, B, C, D, E, F, G, H, I> f = m_f as F<R, A, B, C, D, E, F, G, H, I>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee, ff, gg, hh, ii) : f(aa, bb, cc, dd, ee, ff, gg, hh, ii);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J>(out R r, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj)
    {
        F<R, A, B, C, D, E, F, G, H, I, J> f = m_f as F<R, A, B, C, D, E, F, G, H, I, J>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj) : f(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K>(out R r, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk)
    {
        F<R, A, B, C, D, E, F, G, H, I, J, K> f = m_f as F<R, A, B, C, D, E, F, G, H, I, J, K>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk) : f(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L>(out R r, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll)
    {
        F<R, A, B, C, D, E, F, G, H, I, J, K, L> f = m_f as F<R, A, B, C, D, E, F, G, H, I, J, K, L>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll) : f(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M>(out R r, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm)
    {
        F<R, A, B, C, D, E, F, G, H, I, J, K, L, M> f = m_f as F<R, A, B, C, D, E, F, G, H, I, J, K, L, M>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm) : f(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>(out R r, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn)
    {
        F<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N> f = m_f as F<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn) : f(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(out R r, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo)
    {
        F<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O> f = m_f as F<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo) : f(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(out R r, A aa, B bb, C cc, D dd, E ee, F ff, G gg, H hh, I ii, J jj, K kk, L ll, M mm, N nn, O oo, P pp)
    {
        F<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P> f = m_f as F<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>;
        r = (f == null) ? (R)m_f.DynamicInvoke(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp) : f(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override F<A, B> AsF<A, B>(uint errpos)
    {
        if (m_f is F<A, B> result)
            return result;
        return base.AsF<A, B>(errpos);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override F<A, B, C> AsF<A, B, C>(uint errpos)
    {
        if (m_f is F<A, B, C> result)
            return result;
        return base.AsF<A, B, C>(errpos);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override F<A, B, C, D> AsF<A, B, C, D>(uint errpos)
    {
        if (m_f is F<A, B, C, D> result)
            return result;
        return base.AsF<A, B, C, D>(errpos);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override F<A, B, C, D, E> AsF<A, B, C, D, E>(uint errpos)
    {
        if (m_f is F<A, B, C, D, E> result)
            return result;
        return base.AsF<A, B, C, D, E>(errpos);
    }
}

public partial class UnaryDelegateFunction : DelegateFunction
{
    [MirrorAnalyzed("Mpl.Run.Function")]
    private F<object, object> m_F;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public UnaryDelegateFunction(F<object, object> f)
        : base(f, 1)
    {
        m_F = f;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A>(out R r, A aa)
    {
        r = (R)m_F(aa);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void app<R, A>(out R r, A aa)
    {
        r = (R)m_F(aa);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa)
    {
        return m_F(aa);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override Function Premium()
    {
        return new UnaryDelegateFunction(Delegates.Premium_1(m_F)).OptFunc(MinArgs).IgnoreVectorization(m_no_vectorize);
    }
}

public partial class BinaryDelegateFunction : DelegateFunction
{
    [MirrorAnalyzed("Mpl.Run.Function")]
    private F<object, object, object> m_F;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public BinaryDelegateFunction(F<object, object, object> f)
        : base(f, 2)
    {
        m_F = f;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B>(out R r, A aa, B bb)
    {
        r = (R)m_F(aa, bb);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void app<R, A, B>(out R r, A aa, B bb)
    {
        r = (R)m_F(aa, bb);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb)
    {
        return m_F(aa, bb);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override Function Premium()
    {
        return new BinaryDelegateFunction(Delegates.Premium_2(m_F)).OptFunc(MinArgs).IgnoreVectorization(m_no_vectorize);
    }
}

public partial class TernaryDelegateFunction : DelegateFunction
{
    [MirrorAnalyzed("Mpl.Run.Function")]
    private F<object, object, object, object> m_F;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public TernaryDelegateFunction(F<object, object, object, object> f)
        : base(f, 3)
    {
        m_F = f;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C>(out R r, A aa, B bb, C cc)
    {
        r = (R)m_F(aa, bb, cc);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void app<R, A, B, C>(out R r, A aa, B bb, C cc)
    {
        r = (R)m_F(aa, bb, cc);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc)
    {
        return m_F(aa, bb, cc);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override Function Premium()
    {
        return new TernaryDelegateFunction(Delegates.Premium_3(m_F)).OptFunc(MinArgs).IgnoreVectorization(m_no_vectorize);
    }
}

public partial class QuartnaryDelegateFunction : DelegateFunction
{
    [MirrorAnalyzed("Mpl.Run.Function")]
    private F<object, object, object, object, object> m_F;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public QuartnaryDelegateFunction(F<object, object, object, object, object> f)
        : base(f, 4)
    {
        m_F = f;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D>(out R r, A aa, B bb, C cc, D dd)
    {
        r = (R)m_F(aa, bb, cc, dd);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void app<R, A, B, C, D>(out R r, A aa, B bb, C cc, D dd)
    {
        r = (R)m_F(aa, bb, cc, dd);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd)
    {
        return m_F(aa, bb, cc, dd);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override Function Premium()
    {
        return new QuartnaryDelegateFunction(Delegates.Premium_4(m_F)).OptFunc(MinArgs).IgnoreVectorization(m_no_vectorize);
    }
}

public partial class PremiumFunction : Function
{
    [MirrorAnalyzed("Mpl.Run.Function")]
    private Function m_func;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override string Name => m_func.Name;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public PremiumFunction(Function f)
        : base(f.Purity, 0)
    {
        m_minargs = f.MinArgs;
        m_varargs = f.Varargs;
        m_func = f;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void invVec<R>(out R r, object[] args)
    {
        Function.CheckPremium();
        m_func.invVec<R>(out r, args);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee, ff);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee, ff, gg);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee, ff, gg, hh);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee, ff, gg, hh, ii);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm, object nn)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm, object nn, object oo)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object aa, object bb, object cc, object dd, object ee, object ff, object gg, object hh, object ii, object jj, object kk, object ll, object mm, object nn, object oo, object pp)
    {
        Function.CheckPremium();
        return m_func.DynCall(aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo, pp);
    }
}

public partial class FnCompose : Function
{
    [MirrorAnalyzed("Mpl.Run.Function")]
    private Function m_f;

    [MirrorAnalyzed("Mpl.Run.Function")]
    private Function m_g;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override string Name => $"compose({m_f.Name},{m_g.Name})";

    [MirrorAnalyzed("Mpl.Run.Function")]
    public FnCompose(Function f, Function g)
        : base(g.Purity, 0)
    {
        m_f = f;
        m_g = g;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void invVec<R>(out R r, object[] args)
    {
        m_g.invVec<R>(out var r2, args);
        m_f.app<R, R>(out r, r2);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A>(out R r, A aa)
    {
        m_g.app<R, A>(out var r2, aa);
        m_f.app<R, R>(out r, r2);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B>(out R r, A aa, B bb)
    {
        m_g.app<R, A, B>(out var r2, aa, bb);
        m_f.app<R, R>(out r, r2);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C>(out R r, A aa, B bb, C cc)
    {
        m_g.app<R, A, B, C>(out var r2, aa, bb, cc);
        m_f.app<R, R>(out r, r2);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D>(out R r, A aa, B bb, C cc, D dd)
    {
        m_g.app<R, A, B, C, D>(out var r2, aa, bb, cc, dd);
        m_f.app<R, R>(out r, r2);
    }
}

public partial class FnConstant : Function
{
    [MirrorAnalyzed("Mpl.Run.Function")]
    private object m_val;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override string Name => m_val?.ToString() ?? "null";

    [MirrorAnalyzed("Mpl.Run.Function")]
    public FnConstant(object val)
        : base(1024, 0)
    {
        m_varargs = true;
        m_val = val;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void invVec<R>(out R r, object[] args)
    {
        r = (R)m_val;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object Apply(object[] args)
    {
        invVec<object>(out var r, args);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynApply(object[] args)
    {
        invVec<object>(out var r, args);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A>(out R r, A aa)
    {
        r = (R)m_val;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B>(out R r, A aa, B bb)
    {
        r = (R)m_val;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C>(out R r, A aa, B bb, C cc)
    {
        r = (R)m_val;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void inv<R, A, B, C, D>(out R r, A aa, B bb, C cc, D dd)
    {
        r = (R)m_val;
    }
}

public partial class Fn_varargs : Function
{
    [MirrorAnalyzed("Mpl.Run.Function")]
    private Function m_f;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override string Name => m_f.Name;

    [MirrorAnalyzed("Mpl.Run.Function")]
    public Fn_varargs(Function f)
        : base(1024, 0)
    {
        m_varargs = true;
        m_f = f;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override void invVec<R>(out R r, object[] args)
    {
        m_f.inv<R, object[]>(out r, args);
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object Apply(object[] args)
    {
        invVec<object>(out var r, args);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynApply(object[] args)
    {
        invVec<object>(out var r, args);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object arg1)
    {
        m_f.inv<object, object[]>(out var r, new object[1] { arg1 });
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object arg1, object arg2)
    {
        m_f.inv<object, object[]>(out var r, new object[2] { arg1, arg2 });
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object arg1, object arg2, object arg3)
    {
        m_f.inv<object, object[]>(out var r, new object[3] { arg1, arg2, arg3 });
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function")]
    public override object DynCall(object arg1, object arg2, object arg3, object arg4)
    {
        m_f.inv<object, object[]>(out var r, new object[4] { arg1, arg2, arg3, arg4 });
        return r;
    }
}
