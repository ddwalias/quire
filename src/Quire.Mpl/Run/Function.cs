#nullable disable
using Quire.Mcd;
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>Helpers shared by callable values.</summary>
public abstract partial class FunctionUtilities : DimVal
{
    protected FunctionUtilities() { }
}

/// <summary>
/// A callable value. <see cref="MinArgs"/> arguments are required; a <see cref="Varargs"/> function accepts more.
/// Subclasses implement <see cref="invVec{R}"/>; typed call forms also support virtual overrides.
/// </summary>
public abstract partial class Function : FunctionUtilities
{
    protected int m_purity;
    protected int m_n;
    protected bool m_no_vectorize;
    protected int m_minargs;
    protected bool m_varargs;
    protected string m_name;

    /// <param name="function_purity">The original function purity/arity counter before captures.</param>
    /// <param name="n">The captured-argument count subtracted from that counter.</param>
    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    protected Function(int function_purity, int n)
    {
        m_n = n;
        m_purity = function_purity - n;
        m_minargs = m_purity;
        m_no_vectorize = false;
        m_dim = Dim.function_dim;
    }

    public int Purity => m_purity;
    public int MinArgs => m_minargs;
    public bool Varargs => m_varargs;

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public virtual string Name
    {
        get => m_name ?? "<anonymous>";
        set => m_name = value;
    }

    public override Function IsFunction => this;

    /// <summary>Calls the function with <paramref name="args"/> and stores its result in <paramref name="r"/>.</summary>
    public abstract void invVec<R>(out R r, object[] args);

    public object InvokeVec(object[] args)
    {
        invVec(out object r, args);
        return r;
    }

    [MirrorAnalyzed("Mpl.Run.Function.Matrix")]
    public override string ToString() => $"Function: {Name}";
}
