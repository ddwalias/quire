#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class header
{
    [MirrorVerified("PtcPersistentDataheaderMechanicalDifferentialTests")]
    public header() { }

    [MirrorVerified("Release IL")]
    public IEnumerable<IRegionType> Regions { get => regions; set => regions = Enumerable.ToArray<headerFooterRegionType>(Enumerable.Cast<headerFooterRegionType>(value)); }

    [MirrorVerified("Release IL")]
    public bool HeaderConvertedFromMC14
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
}
