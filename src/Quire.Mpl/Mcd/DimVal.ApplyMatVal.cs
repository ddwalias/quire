#nullable disable
using Quire.Mirror;

namespace Quire.Mcd;

public abstract partial class DimVal
{
    [MirrorVerified("DimValApplyMatValDifferentialTests")]
    public static object ApplyMatVal(DimVal fv, DimVal mv)
    {
        MatVal matVal = mv.asMatrix(2u);
        return fv.Apply(matVal.PackObjects());
    }
}
