#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial interface IOleSizeData
{
    bool AutoResize { get; }
    global::System.Windows.Size OleViewportSize { get; }
    bool HasFakeOleViewportSize { get; }
}
