#nullable disable
using Quire.Core;
using Quire.Mirror;

namespace Quire;

public partial class TextStyleChangedHandler
{
    [MirrorVerified("PtcTextStyleChangedHandlerMechanicalDifferentialTests")]
    public TextStyleChangedHandler() { }

    [MirrorVerified("PtcTextStyleChangedHandlerMechanicalDifferentialTests")]
    public event Delegate1<ICollection<TextStyleData>> StyleChanged;

    [MirrorVerified("PtcTextStyleChangedHandlerMechanicalDifferentialTests")]
    public event Delegate1<TextFormat> StyleCreateRequest;

    public void UpdateStyle(ICollection<TextStyleData> textStyles)
    {
        StyleChanged?.Invoke(textStyles);
    }

    public void CreateNewStyle(TextFormat textFormat)
    {
        StyleCreateRequest?.Invoke(textFormat);
    }
}
