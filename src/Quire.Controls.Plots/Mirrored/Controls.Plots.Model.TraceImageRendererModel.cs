using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Quire.Core;

namespace Quire.Controls.Plots.Model;

public partial class TraceImageRendererModel
{
    public static partial class PropertyNames
    {
        public const string RenderingBitmap = "RenderingBitmap";
    }

    private static readonly BitmapSource _nullValueRenderingBitmap;
    private readonly Size _viewPortDpi;
    private bool _isInitializing;
    private bool _createRgbaNeeded;
    private Int32Rect _bitmapRect;
    private readonly ImageCreator _imageCreator;
    private BitmapSource _renderingBitmap;
    private PointCollection _dataPoints;
    private Rect _dataRange;
    private Rect _viewPortRect;
    private DispatcherOperation _renderingOperationState;
    private Size _targetDpi;

    public Size ViewPortDpi => _viewPortDpi;

    public IStyleOwner StyleOwner => _imageCreator;

    public BitmapSource RenderingBitmap
    {
        get => _renderingBitmap;
        private set
        {
            if (_renderingBitmap == value)
            {
                return;
            }
            DebugTools.DevAssert(!HasCachedBitmap);
            _renderingBitmap = value;
            FirePropertyChanged(PropertyNames.RenderingBitmap);
        }
    }

    public bool HasCachedBitmap { get; private set; }

    public PointCollection DataPoints
    {
        get => _dataPoints;
        set
        {
            if (_dataPoints == value)
            {
                return;
            }
            _dataPoints = value;
            _createRgbaNeeded = true;
        }
    }

    public Rect DataRange
    {
        get => _dataRange;
        set
        {
            if (_dataRange == value)
            {
                return;
            }
            _dataRange = value;
            _createRgbaNeeded = true;
        }
    }

    public Rect ViewPortRect
    {
        get => _viewPortRect;
        set
        {
            _viewPortRect = value;
            InitalizeBitmapRect();
        }
    }

    public Size TargetDpi
    {
        get => _targetDpi;
        set
        {
            if (_targetDpi == value)
            {
                return;
            }
            _targetDpi = value;
            InitalizeBitmapRect();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public TraceImageRendererModel(Size viewPortDpi)
    {
        _targetDpi = (_viewPortDpi = viewPortDpi);
        RenderingBitmap = _nullValueRenderingBitmap;
        _imageCreator = new ImageCreator();
        _imageCreator.StyleChanged += OnStyleChanged;
    }

    private void OnStyleChanged(string propertyName)
    {
        FirePropertyChanged(propertyName);
        if (!_isInitializing)
        {
            AsyncCreateBitmap();
        }
    }

    public void SetCachedBitmap(BitmapSource cachedBitmap)
    {
        HasCachedBitmap = false;
        if (cachedBitmap != null)
        {
            RenderingBitmap = cachedBitmap;
            HasCachedBitmap = true;
        }
    }

    private void InitalizeBitmapRect()
    {
        Int32Rect bitmapRect = GetBitmapRect(TargetDpi);
        if (_bitmapRect != bitmapRect)
        {
            _bitmapRect = bitmapRect;
            _createRgbaNeeded = true;
        }
    }

    private Int32Rect GetBitmapRect(Size targetDpi)
    {
        Int32Rect result = default;
        result.Width = (int)Math.Round(_viewPortRect.Width / ViewPortDpi.Width * targetDpi.Width);
        result.Height = (int)Math.Round(_viewPortRect.Height / ViewPortDpi.Height * targetDpi.Height);
        return result;
    }

    public BitmapSource GetBitmapForPrint(Size targetDpi)
    {
        ImageCreator imageCreator = new ImageCreator(_imageCreator);
        Int32Rect bitmapRect = GetBitmapRect(targetDpi);
        return CreateBitmap(imageCreator.CreateRgba(DataPoints, DataRange, bitmapRect), bitmapRect, targetDpi);
    }

    public BitmapSource CreateSinglePointBitmap()
    {
        return CreateBitmap(new uint[1] { 4278190080u }, new Int32Rect(0, 0, 1, 1), new Size(96.0, 96.0));
    }

    public void BeginInit()
    {
        if (DebugTools.DevAssert(!_isInitializing).Failed)
        {
            return;
        }
        _isInitializing = true;
    }

    public void EndInit()
    {
        if (DebugTools.DevAssert(_isInitializing).Failed)
        {
            return;
        }
        _isInitializing = false;
        AsyncCreateBitmap();
    }

    private void AsyncCreateBitmap()
    {
        if (!ViewPortRect.IsValidSizeRect() || DataPoints.IsNullOrEmpty() || !DataRange.IsValidSizeRect())
        {
            AbortRendering();
            _createRgbaNeeded = false;
            _imageCreator.Reset();
            if (!HasCachedBitmap)
            {
                RenderingBitmap = _nullValueRenderingBitmap;
            }
            return;
        }
        if (_renderingOperationState == null
            || _renderingOperationState.Status == DispatcherOperationStatus.Completed
            || _renderingOperationState.Status == DispatcherOperationStatus.Aborted)
        {
            _renderingOperationState = Dispatcher.CurrentDispatcher.BeginInvoke(new Action(GenerateBitmap), DispatcherPriority.DataBind);
            return;
        }
        if (_renderingOperationState.Status == DispatcherOperationStatus.Executing)
        {
            DebugTools.DevAssert(false);
        }
    }

    private void GenerateBitmap()
    {
        if (HasCachedBitmap)
        {
            return;
        }
        uint[] array = null;
        if (_createRgbaNeeded)
        {
            array = _imageCreator.CreateRgba(DataPoints, DataRange, _bitmapRect);
            _createRgbaNeeded = false;
        }
        else if (_imageCreator.UpdateNeeded)
        {
            array = _imageCreator.UpdateRgba();
        }
        if (array != null)
        {
            RenderingBitmap = CreateBitmap(array);
        }
    }

    private WriteableBitmap CreateBitmap(uint[] rgbaArr)
    {
        return CreateBitmap(rgbaArr, _bitmapRect, TargetDpi);
    }

    private static WriteableBitmap CreateBitmap(uint[] rgbaArr, Int32Rect bitmapRect, Size dpi)
    {
        if (DebugTools.DevAssert(rgbaArr != null && bitmapRect.Width * bitmapRect.Height == rgbaArr.Length).Failed)
        {
            return null;
        }
        WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapRect.Width, bitmapRect.Height, dpi.Width, dpi.Height, PixelFormats.Bgra32, null);
        int num = PixelFormats.Bgra32.BitsPerPixel / 8;
        Int32Rect sourceRect = new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight);
        writeableBitmap.WritePixels(sourceRect, rgbaArr, num * writeableBitmap.PixelWidth, 0);
        return writeableBitmap;
    }

    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public void Dispose()
    {
        AbortRendering();
    }

    private void AbortRendering()
    {
        if (_renderingOperationState != null
            && (_renderingOperationState.Status == DispatcherOperationStatus.Executing || _renderingOperationState.Status == DispatcherOperationStatus.Pending))
        {
            _renderingOperationState.Abort();
        }
    }

    public void StopRendering()
    {
        BeginInit();
        DataPoints = null;
        EndInit();
    }
}
