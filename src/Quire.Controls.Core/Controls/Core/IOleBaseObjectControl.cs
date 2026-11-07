#nullable disable
namespace Quire.Controls.Core;

public partial interface IOleBaseObjectControl
{
    bool IsOleOpen { get; }
    void CloseOle();
}
