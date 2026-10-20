#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.FunctionalitiesLimitation;

public partial struct DisplayFreemiumAboutBoxCommandParameter
{
    [MirrorVerified("Release IL")]
    public readonly IPositionProvider LockedItem;

    [MirrorVerified("Release IL")]
    public readonly EnumFunctionalityKey FunctionalityKey;

    [MirrorVerified("Release IL")]
    public DisplayFreemiumAboutBoxCommandParameter(IPositionProvider lockedItem, EnumFunctionalityKey functionalityKey)
    {
        LockedItem = lockedItem;
        FunctionalityKey = functionalityKey;
    }

    [MirrorVerified("Release IL")]
    public DisplayFreemiumAboutBoxCommandParameter(FrameworkElement lockedItem, EnumFunctionalityKey functionalityKey)
    {
        LockedItem = new FrameworkElementPositionWrapper(lockedItem);
        FunctionalityKey = functionalityKey;
    }
}
