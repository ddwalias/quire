using System.Windows;
using System.Windows.Media.Imaging;

namespace Quire.Controls.Plots.ViewModel;

public partial interface ITraceImageRendererViewModel
{
    BitmapSource RenderingBitmap { get; }

    bool UseTraceInvalidStyle { get; set; }

    Rect ViewPortRect { get; set; }

    bool HasCachedBitmap { get; }

    Size ViewPortDpi { get; }

    Size TargetDpi { get; set; }

    void SetCachedBitmap(BitmapSource cachedBitmap);

    BitmapSource GetBitmapForPrint(Size printerTargetDpiScale);

    BitmapSource CreateSinglePointBitmap();
}
