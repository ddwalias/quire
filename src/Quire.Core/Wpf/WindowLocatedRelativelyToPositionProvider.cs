#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class WindowLocatedRelativelyToPositionProvider
{
    private void SetWindowLocation()
    {
        try
        {
            if (PositionProvider.IsVisible)
            {
                global::System.Windows.Point popupLocation = GetPopupLocation();
                Left = popupLocation.X;
                Top = popupLocation.Y;
            }
            else
            {
                DebugTools.DevAssert(false, "positionProvider is not visible in the application.");
            }
        }
        catch (global::System.Exception ex)
        {
            DebugTools.DevAssert(false, "The following exception has been thrown when trying to set the window location:\n" + ex.Message);
        }
    }

    [MirrorVerified("PtcWpfWindowLocatedRelativelyToPositionProviderMechanicalDifferentialTests")]
    public WindowLocatedRelativelyToPositionProvider() { }

    private DpiFactors GetDpiFactor()
    {
        global::System.Windows.Media.Matrix transformToDevice = global::System.Windows.PresentationSource.FromVisual(Owner).CompositionTarget.TransformToDevice;
        double m = transformToDevice.M11;
        double m2 = transformToDevice.M22;
        return new DpiFactors(m, m2);
    }

    private global::System.Windows.Rect GetPositionProviderScreenBounds()
    {
        DpiFactors dpiFactor = GetDpiFactor();
        global::System.Windows.Rect val = new global::System.Windows.Rect(PositionProvider.PointToScreen(new global::System.Windows.Point(0.0, 0.0)), PositionProvider.PointToScreen(new global::System.Windows.Point(PositionProvider.ActualWidth, PositionProvider.ActualHeight)));
        return new global::System.Windows.Rect(val.X / dpiFactor.DpiFactorX, val.Y / dpiFactor.DpiFactorY, val.Width / dpiFactor.DpiFactorX, val.Height / dpiFactor.DpiFactorY);
    }

    private ItemQuarterLocationInOwnerWindow GetPositionProviderQuarterPosition(global::System.Windows.Point positionProviderRelativePointFromOwner)
    {
        bool flag = positionProviderRelativePointFromOwner.Y <= Owner.ActualHeight / 2.0;
        bool flag2 = positionProviderRelativePointFromOwner.X <= Owner.ActualWidth / 2.0;
        if (!flag)
        {
            if (!flag2)
            {
                return ItemQuarterLocationInOwnerWindow.BottomRight;
            }
            return ItemQuarterLocationInOwnerWindow.BottomLeft;
        }
        if (!flag2)
        {
            return ItemQuarterLocationInOwnerWindow.TopRight;
        }
        return ItemQuarterLocationInOwnerWindow.TopLeft;
    }

    [MirrorVerified("PtcWpfWindowLocatedRelativelyToPositionProviderMechanicalDifferentialTests")]
    protected IPositionProvider PositionProvider { get; set; }

    partial struct DpiFactors
    {
        [MirrorVerified("PtcWpfWindowLocatedRelativelyToPositionProviderMechanicalDifferentialTests")]
        public readonly double DpiFactorX;

        [MirrorVerified("PtcWpfWindowLocatedRelativelyToPositionProviderMechanicalDifferentialTests")]
        public readonly double DpiFactorY;

        [MirrorVerified("PtcWpfWindowLocatedRelativelyToPositionProviderMechanicalDifferentialTests")]
        public DpiFactors(double dpiFactorX, double dpiFactorY)
        {
            DpiFactorX = dpiFactorX;
            DpiFactorY = dpiFactorY;
        }
    }
}
