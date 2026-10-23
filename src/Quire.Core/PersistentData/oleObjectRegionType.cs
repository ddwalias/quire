#nullable disable
using System.Windows;
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.PersistentData;

public partial class oleObjectRegionType
{
    public oleObjectRegionType(IOleSizeData sizeData)
    {
        if (sizeData.HasFakeOleViewportSize)
        {
            OleViewportSize = new SizeType
            {
                width = 0.0,
                height = 0.0
            };
            FakeOleViewportSize = sizeData.OleViewportSize;
        }
        else
        {
            CopyFrom(sizeData);
            FakeOleViewportSize = null;
        }
        AutoResize = sizeData.AutoResize;
    }

    [MirrorVerified("PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests")]
    public oleObjectRegionType() { }

    [MirrorVerified("PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests")]
    private bool autoResizeField;

    [MirrorVerified("PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests")]
    public bool AutoResize { get => autoResizeField; set => autoResizeField = value; }

    [MirrorVerified("PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests")]
    protected override ExternalObjectType ExternalObjectType => ExternalObjectType.Ole;

    [MirrorVerified("PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests")]
    public Nullable<Size> FakeOleViewportSize { get; set; }
}
