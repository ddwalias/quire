using System.Windows;
using System.Windows.Input;

namespace Quire.Controls.Plots.Plot3D.Tools;

public abstract partial class BaseTrackball : IBaseTrackball
{
    protected TrackingInfo CurrentTrackingInfo { get; private set; }

    protected partial class TrackingInfo
    {
        public MouseButton TrackingButton { get; set; }

        public bool HasSignificantMove { get; set; }

        public Point CaptureStartPosition { get; set; }

        public Point LastPosition { get; set; }

        public TrackingInfo()
        {
        }
    }
}
