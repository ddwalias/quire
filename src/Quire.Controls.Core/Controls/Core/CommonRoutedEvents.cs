#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class CommonRoutedEvents
{
    [MirrorVerified("PtcControlsCoreCommonRoutedEventsMechanicalDifferentialTests")]
    public CommonRoutedEvents() { }

    partial class CaretLeavesTextRegionEventArgs
    {
        [MirrorVerified("PtcControlsCoreCommonRoutedEventsMechanicalDifferentialTests")]
        public CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection LeavingDirection { get; set; }
    }
}
