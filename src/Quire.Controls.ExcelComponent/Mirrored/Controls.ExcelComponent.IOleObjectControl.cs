#nullable disable
#pragma warning disable
namespace Quire.Controls.ExcelComponent;

public partial interface IOleObjectControl
{
    bool OleObjectAutoResize { get; }
    bool CanResetSize { get; }
    bool CanUpdateLink { get; }
    bool CanToggleAutoResize { get; }
}
