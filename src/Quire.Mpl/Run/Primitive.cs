using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class Primitive
{
    [MirrorVerified("PrimitiveDifferentialTests")]
    public bool IsOpCodePrimitive()
    {
        if (m_code == null)
        {
            return m_opcodes != null;
        }
        return false;
    }

    [MirrorVerified("PrimitiveDifferentialTests")]
    public bool IsCodePrimitive()
    {
        return m_code != null;
    }

    [MirrorVerified("PrimitiveDifferentialTests")]
    public bool IsMethodPrimitive()
    {
        if (m_method != null && m_code == null)
        {
            return m_opcodes == null;
        }
        return false;
    }

    [MirrorVerified("PrimitiveDifferentialTests")]
    public bool IsForeignPrimitive()
    {
        return m_foreign != null;
    }
}
