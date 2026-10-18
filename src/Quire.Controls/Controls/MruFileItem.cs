#nullable disable
using Quire.Mirror;

namespace Quire.Controls;

public partial class MruFileItem
{
    [MirrorVerified("PtcControlsMruFileItemMechanicalDifferentialTests")]
    public MruFileItem() { }

    [MirrorVerified("PtcControlsMruFileItemMechanicalDifferentialTests")]
    public bool HasAccessKey => true;

    [MirrorVerified("PtcControlsMruFileItemMechanicalDifferentialTests")]
    public MruFileTypes MruFileType { get; set; }

    [MirrorVerified("PtcControlsMruFileItemMechanicalDifferentialTests")]
    public event Delegate2<MruFileItem, bool> onPinChangedEvent;
}
