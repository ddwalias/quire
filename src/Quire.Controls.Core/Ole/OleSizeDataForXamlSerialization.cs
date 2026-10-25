#nullable disable
using System.Windows;
using Quire.PersistentData;

namespace Quire.Controls.Core.Ole;

public partial class OleSizeDataForXamlSerialization : IOleSizeData
{
    public bool AutoResize { get; set; }

    public Size OleViewportSize { get; set; }

    public bool HasFakeOleViewportSize { get; set; }

    public OleSizeDataForXamlSerialization()
    {
    }

    public OleSizeDataForXamlSerialization(IOleSizeData sizeData)
    {
        CopyFrom(sizeData);
    }

    private void CopyFrom(IOleSizeData data)
    {
        AutoResize = data.AutoResize;
        OleViewportSize = data.OleViewportSize;
        HasFakeOleViewportSize = data.HasFakeOleViewportSize;
    }
}
