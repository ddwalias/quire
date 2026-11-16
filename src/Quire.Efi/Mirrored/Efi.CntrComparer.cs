#nullable disable
#pragma warning disable

using System.Collections.Generic;

namespace Quire.Efi;

public partial class CntrComparer
{
    public CntrComparer()
    {
    }

    public int Compare(List<shpPoint3D> c1, List<shpPoint3D> c2)
    {
        if (c1[0].Z() > c2[0].Z())
        {
            return 1;
        }
        if (c1[0].Z() == c2[0].Z())
        {
            return 0;
        }
        return -1;
    }
}
