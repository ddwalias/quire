#nullable disable
#pragma warning disable

namespace Quire.Native.PtcCore;

static partial class SnapToRootVisualUtils
{
    private static global::System.Windows.Media.ScaleTransform DpiScale
    {
        get
        {
            if (_dpiScale == null)
            {
                global::System.Windows.Size dpiScale = global::Quire.Wpf.ImageUtils.DpiScaleForApplication();
                _dpiScale = new global::System.Windows.Media.ScaleTransform(dpiScale.Width, dpiScale.Height);
                _dpiScale.Freeze();
            }
            return _dpiScale;
        }
    }
}
