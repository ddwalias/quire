#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial class oleObjectRegionType
{
    global::System.Windows.Size global::Quire.PersistentData.IOleSizeData.OleViewportSize
    {
        get
        {
            global::System.Nullable<global::System.Windows.Size> fakeOleViewportSize = FakeOleViewportSize;
            if (!fakeOleViewportSize.HasValue)
            {
                if (OleViewportSize != null)
                {
                    return new global::System.Windows.Size(OleViewportSize.width, OleViewportSize.height);
                }
                return global::System.Windows.Size.Empty;
            }
            return fakeOleViewportSize.GetValueOrDefault();
        }
    }
    public bool HasFakeOleViewportSize { get => FakeOleViewportSize.HasValue; }
}
