#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Media.Imaging;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;

namespace Quire.Controls.Plots.ViewModel;

public partial class TraceImageRendererViewModel : ViewModelBase, ITraceImageRendererViewModel
{
    private readonly ITraceImageRendererModel _model;

    public BitmapSource RenderingBitmap => _model.RenderingBitmap;

    public bool HasCachedBitmap => _model.HasCachedBitmap;

    public bool UseTraceInvalidStyle
    {
        get
        {
            return _model.StyleOwner.UseTraceInvalidStyle;
        }
        set
        {
            _model.StyleOwner.UseTraceInvalidStyle = value;
        }
    }

    public Rect ViewPortRect
    {
        get
        {
            return _model.ViewPortRect;
        }
        set
        {
            _model.BeginInit();
            _model.ViewPortRect = value;
            _model.EndInit();
        }
    }

    public Size ViewPortDpi
    {
        get
        {
            return _model.ViewPortDpi;
        }
    }

    public Size TargetDpi
    {
        get
        {
            return _model.TargetDpi;
        }
        set
        {
            _model.BeginInit();
            _model.TargetDpi = value;
            _model.EndInit();
        }
    }

    public TraceImageRendererViewModel(ITraceImageRendererModel model)
    {
        _model = model;
        _model.PropertyChanged += ModelPropertyChangedInner;
    }

    public void SetCachedBitmap(BitmapSource cachedBitmap)
    {
        _model.SetCachedBitmap(cachedBitmap);
    }

    public BitmapSource GetBitmapForPrint(Size printerTargetDpiScale)
    {
        return _model.GetBitmapForPrint(printerTargetDpiScale);
    }

    public BitmapSource CreateSinglePointBitmap()
    {
        return _model.CreateSinglePointBitmap();
    }
}
