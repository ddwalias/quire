#nullable disable
using Quire.Mirror;

namespace Quire;

public partial struct TextStylePropertyInfoExt
{
    [MirrorVerified("PtcTextStylePropertyInfoExtMechanicalDifferentialTests")]
    public object Value;

    [MirrorVerified("PtcTextStylePropertyInfoExtMechanicalDifferentialTests")]
    public bool IsInherited;

    [MirrorVerified("PtcTextStylePropertyInfoExtMechanicalDifferentialTests")]
    public TextStylePropertyInfoExt(bool isInherited, object value)
    {
        Value = value;
        IsInherited = isInherited;
    }
}
