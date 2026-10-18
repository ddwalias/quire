#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class WindowLocatedRelativelyToPositionProvider
{
    [MirrorVerified("PtcWpfWindowLocatedRelativelyToPositionProviderMechanicalDifferentialTests")]
    public WindowLocatedRelativelyToPositionProvider() { }

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
