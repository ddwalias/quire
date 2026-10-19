#nullable disable
using System.Linq;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class footer
{
    [MirrorVerified("PtcPersistentDataHeaderFooterMechanicalDifferentialTests")]
    public IEnumerable<IRegionType> Regions
    {
        get
        {
            return regions;
        }
        set
        {
            regions = value.Cast<headerFooterRegionType>().ToArray();
        }
    }

    [MirrorVerified("PtcPersistentDataHeaderFooterMechanicalDifferentialTests")]
    public bool FooterConvertedFromMC14
    {
        get
        {
            if (convertedfromSpecified)
            {
                return convertedfrom == applicationVersionType.Mathcad14;
            }
            return false;
        }
    }

    [MirrorVerified("PtcPersistentDatafooterMechanicalDifferentialTests")]
    public footer() { }
}
