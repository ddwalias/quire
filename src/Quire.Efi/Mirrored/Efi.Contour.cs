#nullable disable
#pragma warning disable

using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

public partial class Contour
{
    private object m_levels;

    public Contour(int origin, object exp, object scale, object limits, object gridn, object levels, object[] param)
        : base(origin, exp, scale, limits, gridn, param)
    {
        m_levels = levels;
    }
}
