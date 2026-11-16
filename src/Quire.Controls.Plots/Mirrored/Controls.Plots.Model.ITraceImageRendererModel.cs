using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Quire.Controls.Plots.Model;

public partial interface ITraceImageRendererModel : INotifyPropertyChanged, ISupportInitialize, IDisposable
{
    IStyleOwner StyleOwner { get; }

    BitmapSource RenderingBitmap { get; }

    bool HasCachedBitmap { get; }

    PointCollection DataPoints { get; set; }

    Rect DataRange { get; set; }

    Rect ViewPortRect { get; set; }

    Size ViewPortDpi { get; }

    Size TargetDpi { get; set; }

    void SetCachedBitmap(BitmapSource cachedBitmap);

    void StopRendering();

    BitmapSource GetBitmapForPrint(Size printerTargetDpiScale);

    BitmapSource CreateSinglePointBitmap();
}
