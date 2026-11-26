#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Quire.Controls.Core;
using Quire.Controls.Plots.Plot3D.Tools;
using Quire.Controls.Plots.Plot3D.ViewModel;
using Quire.Media3D;
using Quire.Wpf;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class PlotGraph3D
{
    private static readonly Type _typeOfThis = typeof(PlotGraph3D);

    private VisualTrackballAdorner _trackballAdorner;

    private readonly TemporaryStorageHelper _temporaryStorageHelper = new TemporaryStorageHelper();

    private readonly IList<PlotTrace3D> _traceViews = new List<PlotTrace3D>();

    private const double LineThicknessForPrint = 1.0;

    internal const double BaseLineThicknessValue = 1.0;

    public static readonly DependencyProperty BaseLineThicknessProperty = DependencyProperty.Register("BaseLineThickness", typeof(double), _typeOfThis, new PropertyMetadata(1.0, BaseLineThicknessChanged));

    private static void BaseLineThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        (sender as PlotGraph3D)._traceViews.Each(delegate(PlotTrace3D trace)
        {
            trace.BaseLineThickness = (double)args.NewValue;
        });
    }

    public double BaseLineThickness
    {
        get
        {
            return (double)GetValue(BaseLineThicknessProperty);
        }
        set
        {
            SetValue(BaseLineThicknessProperty, value);
        }
    }

    public static readonly DependencyProperty IsInPrintingProperty;

    static PlotGraph3D()
    {
        DocumentAttachedProperties.ZoomProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(OnZoomChanged));
        IsInPrintingProperty = DocumentAttachedProperties.IsInPrintingProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(OnIsPrintingChanged));
    }

    private static void OnZoomChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs eArgs)
    {
        (dObj as PlotGraph3D).BaseLineThickness = (double)BaseLineThicknessProperty.DefaultMetadata.DefaultValue / (double)eArgs.NewValue;
    }

    private static void OnIsPrintingChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs eArgs)
    {
        PlotGraph3D plotGraph3D = dObj as PlotGraph3D;
        if (DebugTools.AssertNotNull(plotGraph3D, null).Passed)
        {
            plotGraph3D._temporaryStorageHelper.CleanStorage(true);
        }
        foreach (PlotTrace3D traceView in plotGraph3D._traceViews)
        {
            traceView.ViewModel.IsInPrinting = (bool)eArgs.NewValue;
        }
    }

    public bool IsInPrinting => (bool)GetValue(IsInPrintingProperty);

    private bool IsCopyAsImageWithZoom
    {
        get
        {
            RegionImageSettings? copyAsImageSettings = DocumentAttachedProperties.GetCopyAsImageSettings(this);
            if (copyAsImageSettings.HasValue)
            {
                return copyAsImageSettings.Value.ApplyZoom;
            }
            return false;
        }
    }

    private ImageSource _imageSource;

    private ImageSource ImageSource
    {
        get
        {
            return _imageSource;
        }
        set
        {
            // Match Mathcad IL: == null ternary order emits brfalse.s to Visibility.Visible.
            Viewport.Visibility = ((value == null) ? Visibility.Visible : Visibility.Hidden);
            _imageSource = value;
        }
    }

    public string ImageFileNameForPrintXamlSerialization
    {
        get
        {
            if (!IsInPrinting || !CanPrint)
            {
                return null;
            }
            _temporaryStorageHelper.SaveIfFileNameNullOrEmpty(CreateBitmapForPrint);
            return _temporaryStorageHelper.StorageFileName;
        }
        set
        {
            if (value != null)
            {
                ImageSource = _temporaryStorageHelper.LoadBitmap(value);
            }
        }
    }

    public bool CanPrint => !AttachedProperties.GetContainedInCollapsedWhiteboard(this);

    public PlotGraph3D()
    {
        InitializeComponent();
        BoxVisibility = false;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        if (ImageSource != null)
        {
            DebugTools.DevAssert(Viewport.Visibility == Visibility.Hidden, null);
            drawingContext.DrawImage(ImageSource, new Rect(PointExtension.Zero, base.RenderSize));
        }
        else
        {
            base.OnRender(drawingContext);
        }
    }

    private BitmapSource CreateBitmapForPrint()
    {
        // Match Mathcad IL: declare result local before saving BaseLineThickness.
        BitmapSource result;
        double baseLineThickness = BaseLineThickness;
        try
        {
            if (!IsCopyAsImageWithZoom)
            {
                BaseLineThickness = 1.0;
            }
            result = ImageUtils.CreateBitmapFromVisual(300.0, 300.0, new Rect(PointExtension.Zero, base.RenderSize), this);
        }
        finally
        {
            BaseLineThickness = baseLineThickness;
        }
        return result;
    }

    internal void ToggleVisualTrackballCues()
    {
        if (_trackballAdorner == null)
        {
            _trackballAdorner = new VisualTrackballAdorner(this);
            return;
        }
        _trackballAdorner.Remove();
        _trackballAdorner = null;
    }

    private void AddToViewport(Visual3D visual3D)
    {
        if (visual3D != null && (AssertionE)DebugTools.DevAssert(!Viewport.Children.Contains(visual3D), null) == AssertionE.Passed)
        {
            int index = Math.Max(Viewport.Children.IndexOf(_labelsContainer) - 1, 0);
            Viewport.Children.Insert(index, visual3D);
        }
    }

    private void RemoveFromViewport(Visual3D visual3D)
    {
        if (visual3D != null)
        {
            DebugTools.DevAssert(Viewport.Children.Remove(visual3D), null);
        }
    }

    private PlotTrace3D GetTraceViewByViewModel(IPlotTraceViewModel3D traceViewModel)
    {
        return _traceViews.FirstOrDefault((PlotTrace3D vm) => vm.ViewModel == traceViewModel);
    }

    public void InsertTraceView(PlotTrace3D traceView, int index)
    {
        _traceViews.Insert(index, traceView);
        AddToViewport(traceView.Visual);
        traceView.BaseLineThickness = BaseLineThickness;
        traceView.VisualChanged += TraceViewVisualChanged;
    }

    public void RemoveTrace(IPlotTraceViewModel3D traceViewModel)
    {
        PlotTrace3D traceViewByViewModel = GetTraceViewByViewModel(traceViewModel);
        if ((AssertionE)DebugTools.DevAssert(traceViewByViewModel != null, null) == AssertionE.Failed)
        {
            return;
        }
        traceViewByViewModel.VisualChanged -= TraceViewVisualChanged;
        _traceViews.Remove(traceViewByViewModel);
        RemoveFromViewport(traceViewByViewModel.Visual);
    }

    public int TraceViewsCount => _traceViews.Count;

    private void TraceViewVisualChanged(PlotTrace3D sender, Visual3D oldValue, Visual3D newValue)
    {
        RemoveFromViewport(oldValue);
        AddToViewport(newValue);
    }

    internal bool BoxVisibility
    {
        get
        {
            return _boundingBoxVisual.Color.ScA > 0f;
        }
        set
        {
            Color color = _boundingBoxVisual.Color;
            color.ScA = (value ? 1 : 0);
            _boundingBoxVisual.Color = color;
        }
    }

    internal IEnumerable<TResult> SelectTraceViews<TResult>(Func<PlotTrace3D, TResult> selector)
    {
        return _traceViews.Select(selector);
    }

    internal IPlotAxis3D XAxis => _xAxis;

    internal IPlotAxis3D YAxis => _yAxis;

    internal IPlotAxis3D ZAxis => _zAxis;

    internal Viewport3D Viewport => _viewport;

    internal ProjectionCamera Camera => Viewport.Camera as ProjectionCamera;

    public void Dispose()
    {
        _viewport.Dispose();
        _traceViews.Each(delegate(PlotTrace3D trace)
        {
            trace.Dispose();
        });
        XAxis.Dispose();
        YAxis.Dispose();
        ZAxis.Dispose();
        _boundingBoxVisual.Dispose();
    }

    internal PlotGraph3D _thisPlotGraph3D;

    private PlotGraphViewport3D _viewport;

    private PlotAxis3D _xAxis;

    private PlotAxis3D _yAxis;

    private PlotAxis3D _zAxis;

    internal WireLines _boundingBoxVisual;

    private PlotLabelsContainer3D _labelsContainer;

    private bool _contentLoaded;

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plot3d/view/plotgraph3d.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
        case 1:
            _thisPlotGraph3D = (PlotGraph3D)target;
            break;
        case 2:
            _viewport = (PlotGraphViewport3D)target;
            break;
        case 3:
            _xAxis = (PlotAxis3D)target;
            break;
        case 4:
            _yAxis = (PlotAxis3D)target;
            break;
        case 5:
            _zAxis = (PlotAxis3D)target;
            break;
        case 6:
            _boundingBoxVisual = (WireLines)target;
            break;
        case 7:
            _labelsContainer = (PlotLabelsContainer3D)target;
            break;
        default:
            _contentLoaded = true;
            break;
        }
    }
}
