#nullable disable
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Quire.Core;

namespace Quire.Controls.Core.Ole;

public abstract partial class OleContainerBase : Control
{
    private static readonly Type _typeOfThis;

    static OleContainerBase()
    {
        _typeOfThis = typeof(OleContainerBase);
        DocumentAttachedProperties.ZoomProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata((DependencyObject obj, DependencyPropertyChangedEventArgs args) =>
        {
            ((OleContainerBase)obj).OnZoomChanged();
        }));
    }

    protected BitmapSource _bitmap;

    protected internal BitmapSource Bitmap
    {
        get
        {
            AcquireBitmapSource();
            return _bitmap;
        }
        set
        {
            _bitmap = value;
        }
    }

    protected virtual void AcquireBitmapSource()
    {
        BitmapSource bitmapSource = null;
        HwndSource hwndSource;
        Int32Rect renderRectInPixels = GetRenderRectInPixels(out hwndSource);
        if (renderRectInPixels.IsEmpty)
        {
            DebugTools.DevAssert(!IsVisible, null);
        }
        else if ((_bitmap == null || renderRectInPixels.Width != _bitmap.PixelWidth || renderRectInPixels.Height != _bitmap.PixelHeight) && DebugTools.DevAssert(IsOleCreated, null).Passed)
        {
            _bitmap = bitmapSource = CreateBitmap(renderRectInPixels, hwndSource);
        }
    }

    protected abstract BitmapSource CreateBitmap(Int32Rect rectInPixels, HwndSource hwndSource);

    protected abstract Int32Rect GetRenderRectInPixels(out HwndSource hwndSource);

    protected abstract bool IsOleCreated { get; }

    protected virtual void OnZoomChanged()
    {
    }

    protected OleContainerBase()
    {
    }
}
