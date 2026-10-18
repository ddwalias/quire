#nullable disable
using System.ComponentModel;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ImageControl
{
    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    private Uri _originalImageUri;

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public string GetImageExtension() => GetImageExtension(_originalImageUri);

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public void SimulateMouseClick(Point location, int numberOfClicks) => Activate(true);

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public string GetHelpContext() => "ImageControl";

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public byte[] OriginalStreamHash { get; private set; }

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public double VerticalAlignmentOffset => 0.0;

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public bool IsEmpty => false;

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public bool PreserveAspectRatio => true;

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public SubElementToResizeData SubElementToResizeData => null;

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public event Action<IWhiteboardItem> OnGotFocusEvent;

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public event Action<IWhiteboardItem, Direction> RequestRemove;

    [MirrorVerified("PtcControlsCoreImageControlMechanicalDifferentialTests")]
    public event Action<IResizeToFitWhiteboardItem> RequestResizeToFitOrClearRestriction;
}
