#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public static partial class AttachedProperties
{
    [MirrorVerified("PtcControlsCoreAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty CanActivateProperty;

    [MirrorVerified("PtcControlsCoreAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearCanActivate(DependencyObject dObj) => dObj.ClearValue(CanActivateProperty);

    [MirrorVerified("PtcControlsCoreAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty HandlesContextMenuProperty;

    [MirrorVerified("PtcControlsCoreAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearHandlesContextMenu(DependencyObject dObj) => dObj.ClearValue(HandlesContextMenuProperty);
}
