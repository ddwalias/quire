#nullable disable
using Quire.Mirror;

namespace Quire.Mpl;

/// <summary>A return type and parameter types, used as a key (docs/mirror/evidence/Mpl.MplSignature.md).</summary>
public partial class MplSignature
{
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    private System.Type return_type;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    private System.Type[] parameter_types;

    /// <summary>Keeps the array given, without copying.</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public MplSignature(System.Type r, System.Type[] p)
    {
        return_type = r;
        parameter_types = p;
    }

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public System.Type RetType => return_type;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public System.Type[] ArgTypes => parameter_types;

    /// <summary>The sum of the type hashes (wrapping), times 64, plus the parameter count.</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public override int GetHashCode()
    {
        int h = return_type.GetHashCode();
        foreach (System.Type t in parameter_types) h += t.GetHashCode();
        return h * 64 + parameter_types.Length;
    }

    /// <summary>
    /// Same runtime class, then this signature's parameter types position by position, then the return type. The
    /// other's extra parameters are ignored, and fewer throw <see cref="IndexOutOfRangeException"/>.
    /// </summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        var other = (MplSignature)obj;
        for (int i = 0; i < parameter_types.Length; i++)
            if (!parameter_types[i].Equals(other.parameter_types[i])) return false;
        return return_type.Equals(other.return_type);
    }
}
