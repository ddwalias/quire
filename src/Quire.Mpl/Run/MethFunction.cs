#nullable disable
using System;
using System.Reflection;
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class MethFunction : Function
{
    [MirrorVerified("MethFunctionDifferentialTests")]
    private MethodBase m_method;

    [MirrorVerified("MethFunctionDifferentialTests")]
    private object m_clo;

    [MirrorVerified("MethFunctionDifferentialTests")]
    private object[] m_args;

    [MirrorVerified("MethFunctionDifferentialTests")]
    public MethFunction(RuntimeMethodHandle mh, int purity)
        : base(purity, 0)
    {
        m_method = MethodBase.GetMethodFromHandle(mh);
        m_clo = null;
        m_args = FunctionUtilities.empty_vec;
    }

    [MirrorVerified("MethFunctionDifferentialTests")]
    public MethFunction(RuntimeMethodHandle mh, int purity, object clo)
        : this(mh, purity)
    {
        if (m_method.IsStatic)
        {
            m_args = new object[1] { clo };
        }
        else
        {
            m_clo = clo;
        }
    }

    [MirrorVerified("MethFunctionDifferentialTests")]
    private MethFunction(MethodBase mi, object clo, object[] args, int purity)
        : base(purity, 0)
    {
        m_method = mi;
        m_clo = clo;
        m_args = args;
    }

    [MirrorVerified("MethFunctionDifferentialTests")]
    public override void invVec<R>(out R r, object[] args)
    {
        if (m_method.IsStatic || m_clo != null)
        {
            r = (R)m_method.Invoke(m_clo, FunctionUtilities.concat(m_args, args));
        }
        else
        {
            r = (R)m_method.Invoke(args[0], FunctionUtilities.but(1, args));
        }
    }
}
