#nullable disable
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using System.Windows;

namespace Quire.PersistentData;

public partial class oleBaseContainerType
{
    protected abstract ExternalObjectType ExternalObjectType { get; }

    [MirrorVerified("PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests")]
    protected oleBaseContainerType() { }

    [MirrorVerified("PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests")]
    private SizeType oleViewportSizeField;

    [MirrorVerified("PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests")]
    public SizeType OleViewportSize { get => oleViewportSizeField; set => oleViewportSizeField = value; }

    [MirrorVerified("PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests")]
    public string StorageFileName { private get; set; }

    [MirrorVerified("PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests")]
    public IS11NProvider S11NProvider { get; set; }

    [MirrorVerified("Release IL")]
    protected void CopyFrom(IOleSizeData data)
    {
        if (!data.OleViewportSize.IsEmpty)
        {
            OleViewportSize = new SizeType
            {
                width = data.OleViewportSize.Width,
                height = data.OleViewportSize.Height
            };
        }
    }
}
