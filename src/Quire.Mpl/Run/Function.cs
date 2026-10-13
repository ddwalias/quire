#nullable disable
using Quire.Mcd;

namespace Quire.Mpl.Run;

/// <summary>Helpers shared by callable values.</summary>
public abstract partial class FunctionUtilities : DimVal
{
    protected FunctionUtilities() { }
}

/// <summary>
/// A callable value. <see cref="MinArgs"/> arguments are required; a <see cref="Varargs"/> function accepts more.
/// Subclasses implement <see cref="invVec{R}"/>, which every call form ends in.
/// </summary>
public abstract partial class Function : FunctionUtilities
{
    protected int m_purity;
    protected int m_n;
    protected bool m_no_vectorize;
    protected int m_minargs;
    protected bool m_varargs;
    protected string m_name;

    /// <param name="function_purity">0 for a pure function; nonzero when calls have side effects.</param>
    /// <param name="n">The number of parameters.</param>
    protected Function(int function_purity, int n)
    {
        m_purity = function_purity;
        m_n = n;
        m_minargs = n;
        m_dim = Dim.function_dim;
    }

    public int Purity => m_purity;
    public int MinArgs => m_minargs;
    public bool Varargs => m_varargs;

    public virtual string Name
    {
        get => m_name;
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

    public override string ToString() => m_name ?? GetType().Name;
}

/// <summary>Runtime services for compiled and interpreted Mpl code.</summary>
public static partial class Rts
{
    [ThreadStatic]
    private static CancellationTokenSource ms_thread_cancellation_token;

    private static readonly CancellationTokenSource ms_dummy_cancellation_token = new();

    /// <summary>Makes <see cref="IsCancellationRequested"/> on the calling thread follow <paramref name="src"/>.</summary>
    public static void RegisterThreadForCancellationChecking(CancellationTokenSource src) =>
        ms_thread_cancellation_token = src;

    /// <summary>True when the calculation running on this thread should stop.</summary>
    public static bool IsCancellationRequested() =>
        (ms_thread_cancellation_token ?? ms_dummy_cancellation_token).IsCancellationRequested;
}
