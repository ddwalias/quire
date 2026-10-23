using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class LocalBuilder
{
    private readonly int m_localIndex;
    private readonly Type m_localType;
    private readonly MethodBase m_methodBuilder;
    private readonly bool m_isPinned;

    public override bool IsPinned => m_isPinned;
    public override Type LocalType => m_localType;
    public override int LocalIndex => m_localIndex;

    internal LocalBuilder(int localIndex, Type localType, MethodBase methodBuilder)
        : this(localIndex, localType, methodBuilder, isPinned: false)
    {
    }

    internal LocalBuilder(int localIndex, Type localType, MethodBase methodBuilder, bool isPinned)
    {
        m_isPinned = isPinned;
        m_localIndex = localIndex;
        m_localType = localType;
        m_methodBuilder = methodBuilder;
    }

    internal int GetLocalIndex()
    {
        return m_localIndex;
    }

    internal MethodBuilder GetMethodBuilder()
    {
        if (!IsMethod())
        {
            return null;
        }
        return m_methodBuilder as MethodBuilder;
    }

    internal ConstructorBuilder GetConstructorBuilder()
    {
        if (!IsConstructor())
        {
            return null;
        }
        return m_methodBuilder as ConstructorBuilder;
    }

    internal bool IsConstructor()
    {
        // Mathcad calls op_Inequality on the as-result; "is" drops that call.
        return m_methodBuilder as ConstructorBuilder != null;
    }

    internal bool IsMethod()
    {
        // Mathcad calls op_Inequality on the as-result; "is" drops that call.
        return m_methodBuilder as MethodBuilder != null;
    }
}
