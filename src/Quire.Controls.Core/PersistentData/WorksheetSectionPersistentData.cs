#nullable disable
using System.Windows;
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.PersistentData;

public partial class WorksheetSectionPersistentData
{
    [MirrorVerified("PtcPersistentDataWorksheetSectionPersistentDataMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> SerializedRegions { get; private set; }

    [MirrorVerified("PtcPersistentDataWorksheetSectionPersistentDataMechanicalDifferentialTests")]
    public IDictionary<UIElement, Point> RegionsToSerialize { get; private set; }

    [MirrorVerified("PtcPersistentDataWorksheetSectionPersistentDataMechanicalDifferentialTests")]
    public bool ConvertedFromMC14 { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetSectionPersistentDataMechanicalDifferentialTests")]
    public worksheetMessageIdType MsgId { get; set; }
}
