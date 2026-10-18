#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.IORegions;

public static partial class IOTag
{
    [MirrorVerified("PtcControlsCoreIORegionsIOTagMechanicalDifferentialTests")]
    public static readonly DependencyProperty IOTagProperty;

    [MirrorVerified("PtcControlsCoreIORegionsIOTagMechanicalDifferentialTests")]
    public static void ClearIOTag(DependencyObject element) => element.ClearValue(IOTagProperty);

    [MirrorVerified("PtcControlsCoreIORegionsIOTagMechanicalDifferentialTests")]
    public static bool HasInputTag(DependencyObject element) => IsRegionOfType(element, IOTagType.Input);

    [MirrorVerified("PtcControlsCoreIORegionsIOTagMechanicalDifferentialTests")]
    public static bool HasOutputTag(DependencyObject element) => IsRegionOfType(element, IOTagType.Output);

    [MirrorVerified("PtcControlsCoreIORegionsIOTagMechanicalDifferentialTests")]
    public static event Action<DependencyObject, bool> IOTagSet;

    [MirrorVerified("PtcControlsCoreIORegionsIOTagMechanicalDifferentialTests")]
    public static event Action<DependencyObject, bool> IOTagUnSet;
}
