using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Base3D.ViewModel;
using Quire.Controls.Plots.Contour.Model;
using Quire.Controls.Plots.Contour.ViewModel;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;
using Quire.Core;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Plots.Contour.View;

public sealed partial class ContourPlotViewModel : PlotViewModelBase<ContourPlotModel>
{
    internal sealed partial class PropertyNames : PlotViewModelBase<ContourPlotModel>.PropertyNames
    {
        internal const string Opacity = "Opacity";

        internal const string ContoursClippingRect = "ContoursClippingRect";

        private PropertyNames()
        {
        }
    }

    private readonly VisualDataCollection _contourVisualDataCollection;

    private bool _initialized;

    private IColorMap _colorMap = (IColorMap)ColorMapsDictionary.Instance[ColorScheme.Rainbow];

    private Brush[] _brushes;

    private Rect _clippingRect;

    private double _opacity = 1.0;

    public bool HasOriginPositioning
    {
        get => Model.HasOriginPositioning;
        set => Model.HasOriginPositioning = value;
    }

    public VisualDataCollection ContourVisualDataCollection => _contourVisualDataCollection;

    public Base3DAxisViewModel XAxisViewModel { get; private set; }

    public Base3DAxisViewModel YAxisViewModel { get; private set; }

    public Base3DAxisViewModel ZAxisViewModel { get; private set; }

    public ContourPlotViewModel()
    {
        XAxisViewModel = new ContourXYAxisViewModel(AxesType.X);
        XAxisViewModel.CalculatableAdded += OnCalculatableAdded;
        XAxisViewModel.ValueToScreenChanged += ValueToScreenChangedHandler;
        XAxisViewModel.ViewSizeChanged = XorYAxisSizeChanged;
        YAxisViewModel = new ContourXYAxisViewModel(AxesType.Y);
        YAxisViewModel.CalculatableAdded += OnCalculatableAdded;
        YAxisViewModel.ValueToScreenChanged += ValueToScreenChangedHandler;
        YAxisViewModel.ViewSizeChanged = XorYAxisSizeChanged;
        ZAxisViewModel = new ContourZAxisViewModel();
        ZAxisViewModel.CalculatableAdded += OnCalculatableAdded;
        ZAxisViewModel.ViewSizeChanged = ZAxisViewSizeChanged;
        zAxisColorSamples = new ObservableCollection<ColorSample>();
        Model = new ContourPlotModel(XAxisViewModel.PlotAxisModel, YAxisViewModel.PlotAxisModel, ZAxisViewModel.PlotAxisModel);
        BindCommandsAndEvents();
        Model.PlotCalculatableError += ModelPlotCalculatableError;
        _contourVisualDataCollection = new VisualDataCollection(Model);
        _contourVisualDataCollection.PropertyChanged += VisualDataCollectionPropertyChanged;
        CommandBindings.AddRange(ContourVisualDataCollection.CommandBindings);
        UpdateContoursClippingRect();
    }

    private void UpdateContoursClippingRect()
    {
        if (!_contourVisualDataCollection.IsResizing)
        {
            Interval clippingInterval = XAxisViewModel.ClippingInterval;
            Interval yClippingInterval = YAxisViewModel.ClippingInterval;
            ContoursClippingRect = new Rect(clippingInterval.Lower, yClippingInterval.Lower, clippingInterval.Width, yClippingInterval.Width);
        }
    }

    private void VisualDataCollectionPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (propertyChangedEventArgs.PropertyName.Equals("IsResizing"))
        {
            UpdateContoursClippingRect();
        }
    }

    private void ZAxisViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            UpdateZAxisColorSamples();
        }
    }

    private void XorYAxisSizeChanged(object sender, SizeChangedEventArgs e)
    {
        ValueToScreenChangedHandler(sender);
    }

    private void ValueToScreenChangedHandler(object obj)
    {
        UpdateContoursClippingRect();
    }

    protected override void ModelPropertyChanged(string propertyName)
    {
        base.ModelPropertyChanged(propertyName);
        switch (propertyName)
        {
            case "LogicalContoursDataCollection":
                ContourVisualDataCollection.ContourLogicalData = Model.LogicalContoursDataCollection;
                UpdateAxesLimitsFromResults();
                UpdateBrushes();
                break;
            case "ColorScheme":
                ColorMap = (IColorMap)ColorMapsDictionary.Instance[Model.ColorScheme];
                break;
            case "HasError":
                Opacity = Model.HasError ? 0.5 : 1.0;
                break;
            case "SynchronizationStatus":
                Opacity = ((Model.ContourGraphCalculatable as ICalculatableInfo)?.SynchronizationStatus == SynchronizationStatus.Pending) ? 0.5 : 1.0;
                break;
        }
    }

    private void UpdateAxesLimitsFromResults()
    {
        if (Model.LogicalContoursDataCollection != null && Model.LogicalContoursDataCollection.Levels != null)
        {
            IList<double> levels = Model.LogicalContoursDataCollection.Levels;
            if (levels.Count > 1)
            {
                ZAxisViewModel.SetResultRange(levels[0], levels[levels.Count - 1]);
                Rect boundingRectangleXy = Model.LogicalContoursDataCollection.BoundingRectangleXy;
                XAxisViewModel.SetResultRange(boundingRectangleXy.Left, boundingRectangleXy.Right);
                YAxisViewModel.SetResultRange(boundingRectangleXy.Top, boundingRectangleXy.Bottom);
            }
            else
            {
                ZAxisViewModel.SetResultRange(null, null);
                XAxisViewModel.SetResultRange(null, null);
                YAxisViewModel.SetResultRange(null, null);
            }
        }
    }

    public double Opacity
    {
        get => _opacity;
        private set
        {
            if (_opacity != value)
            {
                _opacity = value;
                OnPropertyChanged("Opacity");
            }
        }
    }

    public void Initialize(IPlotEquation zExpression, IValueToScreen horizontalTransformer, IValueToScreen verticalTransformer)
    {
        if (DebugTools.DevAssert(!_initialized, null).Failed)
        {
            return;
        }
        Model.InitializeZEquation(zExpression);
        ContourVisualDataCollection.Initialize(horizontalTransformer, verticalTransformer);
        _initialized = true;
    }

    public IColorMap ColorMap
    {
        set
        {
            _colorMap = value;
            UpdateBrushes();
        }
    }

    public contourPlotTypeContour ContourDataObjectForXamlSerialization
    {
        get => Model.GetContourDataObject(null);
        set => Model.SetContourDataObject(value, null);
    }

    internal SizeType PlotGraphSizeDataObjectForXamlSerialization
    {
        get => Model.GraphSizeDataObject;
        set => Model.GraphSizeDataObject = value;
    }

    private void UpdateBrushes()
    {
        ContourLogicalDataCollection contourLogicalData = ContourVisualDataCollection.ContourLogicalData;
        if (ContourVisualDataCollection.ContourLogicalData.Length > 0)
        {
            BuildBrushes(contourLogicalData);
        }
        else
        {
            _brushes = null;
        }
        ContourVisualDataCollection.UpdateBrushes(_brushes);
        UpdateZAxisColorSamples();
    }

    public ObservableCollection<ColorSample> zAxisColorSamples { get; private set; }

    private void UpdateZAxisColorSamples()
    {
        zAxisColorSamples.Clear();
        if (_brushes == null)
        {
            return;
        }
        zAxisColorSamples.Clear();
        int length = _brushes.Length;
        for (int i = 0; i < length; i++)
        {
            zAxisColorSamples.Add(new ColorSample
            {
                Color = _brushes[i],
                Width = ZAxisViewModel.GetTickInterval(i).Width
            });
        }
    }

    private void BuildBrushes(ContourLogicalDataCollection contourLogicalData)
    {
        int levelsCount = contourLogicalData.LevelsCount;
        if (levelsCount > -1 && _colorMap != null)
        {
            _brushes = new Brush[levelsCount + 1];
            Brush brush = _colorMap.GetBrush(0.0);
            _brushes[0] = brush;
            for (int i = 1; i < levelsCount; i++)
            {
                brush = _colorMap.GetBrush(contourLogicalData.NormalizedCenter(i));
                _brushes[i] = brush;
            }
            brush = _colorMap.GetBrush(1.0);
            _brushes[levelsCount] = brush;
        }
        else
        {
            _brushes = null;
        }
    }

    public Rect ContoursClippingRect
    {
        get => _clippingRect;
        private set
        {
            if (value != _clippingRect)
            {
                _clippingRect = value;
                OnPropertyChanged("ContoursClippingRect");
            }
        }
    }
}
