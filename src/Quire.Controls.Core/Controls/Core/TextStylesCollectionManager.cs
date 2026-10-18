#nullable disable
using System.ComponentModel;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class TextStylesCollectionManager
{
    [MirrorVerified("PtcControlsCoreTextStylesCollectionManagerMechanicalDifferentialTests")]
    private readonly TextStyleChangedHandler _textStyleChangedHandler;

    [MirrorVerified("PtcControlsCoreTextStylesCollectionManagerMechanicalDifferentialTests")]
    public TextStyleChangedHandler TextStyleChangedHandler => _textStyleChangedHandler;

    [MirrorVerified("PtcControlsCoreTextStylesCollectionManagerMechanicalDifferentialTests")]
    public event Delegate0 StyleCollectionChanged;

    [MirrorVerified("PtcControlsCoreTextStylesCollectionManagerMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;
}
