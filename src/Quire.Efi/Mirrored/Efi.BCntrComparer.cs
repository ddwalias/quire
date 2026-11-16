#nullable disable
#pragma warning disable

using System.Collections.Generic;

namespace Quire.Efi;

public partial class BCntrComparer
{
    public BCntrComparer()
    {
    }

    public int Compare(double[] c1, double[] c2)
    {
        if (c1[0] > c2[0])
        {
            return 1;
        }
        if (c1[0] == c2[0])
        {
            return 0;
        }
        return -1;
    }
}
