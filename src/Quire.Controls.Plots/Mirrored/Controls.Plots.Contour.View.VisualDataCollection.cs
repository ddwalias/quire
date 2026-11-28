#nullable disable
#pragma warning disable

using Quire.Controls.Core;
using Quire.Controls.Plots.Contour.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.Traces;
using Quire.Controls.Plots.ViewModel;
using Quire.Wpf;
using Quire;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System;

namespace Quire.Controls.Plots.Contour.View;

public partial class VisualDataCollection
{
    public static readonly global::System.Windows.Input.RoutedCommand SetColorSchemeCommand;
    private global::Quire.Controls.Plots.Contour.View.ViewModelHelper _viewModelHelper;
    private global::Quire.Controls.Plots.Contour.Model.IContourPlotModel _model;
    private static readonly global::System.Type TypeOfThis;
    private static readonly global::System.Windows.Rect DefaultLogicalRect;
    private global::System.Windows.Media.Transform _plotRenderTransform = Transform.Identity;
    public VisualDataCollection(IContourPlotModel model)
        {
            _model = model;
            ModelPropertyChanged("LineStyle");
            ModelPropertyChanged("ContoursColor");
            _viewModelHelper = new ViewModelHelper();
            Action<object> execute = (object param) =>
            {
                if ((AssertionE)DebugTools.DevAssert(param is double, null) == AssertionE.Passed)
                {
                    SetLineWeight((double)param);
                }
            };
            Action<object> setContoursLineColorAction = (object param) =>
            {
                Color? color = null;
                if (param is SolidColorBrush)
                {
                    color = ((SolidColorBrush)param).Color;
                }
                else if (param is Color)
                {
                    color = (Color)param;
                }
                if (color.HasValue)
                {
                    SetContoursLineColor(color.Value);
                }
                else
                {
                    DebugTools.DevAssert(false, null);
                }
            };
            Action<object> setColorSchemeAction = (object param) =>
            {
                EnumHelper.TryCallAction<ColorScheme>(param, SetColorScheme);
            };
            _viewModelHelper.HelperCreateCommandBinding(SetColorSchemeCommand, setColorSchemeAction);
        _viewModelHelper.HelperCreateCommandBinding(global::Quire.Controls.Plots.View.PlotControlBase.ChangeTraceLineCommand, SetContoursLineStyle);
        _viewModelHelper.HelperCreateCommandBinding(global::Quire.Controls.Plots.View.PlotControlBase.ChangeTraceWeightCommand, execute);
        _viewModelHelper.HelperCreateCommandBinding(global::Quire.Controls.Plots.View.PlotControlBase.ChangeTraceColorCommand, setContoursLineColorAction);
            _model.PropertyChanged += (object? sender, PropertyChangedEventArgs eArgs) =>
            {
                ModelPropertyChanged(eArgs.PropertyName);
            };
        }

    private void ModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case "LineStyle":
                DashStyle = TraceRenderer._TraceLineDictionary[_model.LineStyle];
                Stroke = ((LineStyle != LineStyle.None) ? new SolidColorBrush(ContoursColor) : null);
                break;
            case "ContoursColor":
                if (LineStyle != LineStyle.None)
                {
                    Stroke = new SolidColorBrush(ContoursColor);
                }
                break;
        }
        if (TypeDescriptor.GetProperties(this)[propertyName] != null)
        {
            OnPropertyChanged(propertyName);
        }
    }

    private void OnSizeChanged()
    {
        if (_isResizing)
        {
            PlotRenderTransform = new ScaleTransform(Width / _sizeBeforeResizing.Width, Height / _sizeBeforeResizing.Height);
        }
        else
        {
            WpfUtils.DoWork(UpdateVisualPoints);
        }
    }
    private global::System.Windows.Size _sizeBeforeResizing;
    private bool _isResizing;
    private static readonly global::Quire.Controls.Plots.Contour.Model.ContourLogicalDataCollection DefaultContourLogicalDataCollection;
    private global::Quire.Controls.Plots.Contour.Model.ContourLogicalDataCollection _contourLogicalData = DefaultContourLogicalDataCollection;
    public static readonly global::System.Windows.DependencyProperty HeightProperty;
    public static readonly global::System.Windows.DependencyProperty WidthProperty;
    private void UpdateVisualPoints() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static readonly global::System.Windows.DependencyProperty LabelVisibilityProperty;
    private void LogicalDataChanged(global::Quire.Controls.Plots.Contour.Model.ContourLogicalDataCollection newValue, global::Quire.Controls.Plots.Contour.Model.ContourLogicalDataCollection oldValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void InitializeContourVisualData(ContourLogicalDataCollection value)
    {
        int length = value.Length;
        if (length > 0)
        {
            _visualData = new VisualData[length + 1];
            _visualData[0] = CreateVisualDataRaw(value.BoundingContour);
            for (int num = 0; num < length; num++)
            {
                _visualData[num + 1] = CreateContourVisualData(value.Contours[num]);
            }
            PositionLabels();
        }
        else
        {
            _visualData = new VisualData[0];
        }
    }

    private static void HeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualDataCollection)(object)d).OnSizeChanged();
    }

    private static void WidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualDataCollection)(object)d).OnSizeChanged();
    }

    private static void OnLabelVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        VisualDataCollection obj = d as VisualDataCollection;
        DebugTools.DevAssert(d != null, null);
        obj.PositionLabels();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    internal void PositionLabels()
    {
        Cursor overrideCursor = Mouse.OverrideCursor;
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (Width <= 0.0 || Height <= 0.0 || _visualData.Length == 0)
            {
                return;
            }
            if (HorizontalValueToScreen == null || VerticalValueToScreen == null)
            {
                DebugTools.DevAssert(false, null);
                return;
            }
            Rect bounds = _visualData[0].GeometryData.Bounds;
            Point labelPosition = new Point(Width + 10.0, Height + 10.0);
            List<Rect> list = new List<Rect>();
            for (int num = _visualData.Length - 1; num >= 0; num--)
            {
                VisualData visualData = _visualData[num];
                if (!string.IsNullOrEmpty(visualData.Label))
                {
                    Size labelSize = visualData.GetLabelSize();
                    bool flag = false;
                    int pointIndex = 0;
                    while (!flag && pointIndex < visualData.PointsData.Length)
                    {
                        Point dataPoint = visualData.PointsData[pointIndex];
                        Point screenPoint = new Point(HorizontalValueToScreen.ValueToScreen(dataPoint.X), VerticalValueToScreen.ValueToScreen(dataPoint.Y));
                        Rect labelBounds = new Rect(screenPoint.X - labelSize.Width / 2.0, screenPoint.Y, labelSize.Width, labelSize.Height);
                        if (bounds.Contains(labelBounds) && !list.Exists(labelBounds.IntersectsWith))
                        {
                            visualData.LabelPosition = labelBounds.TopLeft;
                            list.Add(labelBounds);
                            flag = true;
                        }
                        pointIndex++;
                    }
                    if (!flag)
                    {
                        visualData.LabelPosition = labelPosition;
                    }
                }
            }
        }
        finally
        {
            Mouse.OverrideCursor = overrideCursor;
        }
    }
    public static readonly global::System.Windows.DependencyProperty LabelFormatterProperty;
    private bool _reversed;
    private global::Quire.Controls.Plots.Contour.View.VisualData[] _visualData = new VisualData[0];
    private bool _initialized;
    private global::System.Windows.Media.DashStyle _dashStyle;
    private global::System.Windows.Media.Brush _stroke;

    public bool IsResizing
    {
        get
        {
            return _isResizing;
        }
        set
        {
            if (value != _isResizing)
            {
                _isResizing = value;
                if (_isResizing)
                {
                    _sizeBeforeResizing = new Size(Width, Height);
                }
                else
                {
                    PlotRenderTransform = Transform.Identity;
                    UpdateVisualPoints();
                }
                OnPropertyChanged("IsResizing");
            }
        }
    }

    public Transform PlotRenderTransform
    {
        get
        {
            return _plotRenderTransform;
        }
        set
        {
            if (value != _plotRenderTransform)
            {
                _plotRenderTransform = value;
                OnPropertyChanged("PlotRenderTransform");
            }
        }
    }

    public ContourLogicalDataCollection ContourLogicalData
    {
        get
        {
            return _contourLogicalData;
        }
        set
        {
            LogicalDataChanged(value, _contourLogicalData);
            _contourLogicalData = value;
        }
    }

    public double Height
    {
        get
        {
            return (double)((DependencyObject)this).GetValue(HeightProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(HeightProperty, (object)value);
        }
    }

    public double Width
    {
        get
        {
            return (double)((DependencyObject)this).GetValue(WidthProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(WidthProperty, (object)value);
        }
    }

    public Visibility LabelVisibility
    {
        get
        {
            return (Visibility)((DependencyObject)this).GetValue(LabelVisibilityProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(LabelVisibilityProperty, (object)value);
        }
    }

    public TickmarkFormatter LabelFormatter
    {
        get
        {
            return (TickmarkFormatter)((DependencyObject)this).GetValue(LabelFormatterProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(LabelFormatterProperty, (object)value);
        }
    }
    public global::System.Collections.Generic.IEnumerator<global::Quire.Controls.Plots.Contour.View.VisualData> GetEnumerator() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private global::Quire.Controls.Plots.Contour.View.VisualData CreateContourVisualData(global::Quire.Controls.Plots.Contour.Model.ContourLogicalData contourLogicalData)
    {
        VisualData visualData = CreateVisualDataRaw(contourLogicalData);
        BindingOperations.SetBinding(binding: new Binding
        {
            Source = this,
            Path = new PropertyPath(LabelFormatterProperty)
        }, target: (DependencyObject)(object)visualData, dp: VisualData.LabelFormatterProperty);
        BindingOperations.SetBinding(binding: new Binding
        {
            Source = this,
            Path = new PropertyPath("Stroke")
        }, target: (DependencyObject)(object)visualData, dp: VisualData.StrokeProperty);
        BindingOperations.SetBinding((DependencyObject)(object)visualData, VisualData.DashStyleProperty, new Binding
        {
            Source = this,
            Path = new PropertyPath("DashStyle")
        });
        BindingOperations.SetBinding((DependencyObject)(object)visualData, VisualData.StrokeThicknessProperty, new Binding
        {
            Source = this,
            Path = new PropertyPath("LineWeight")
        });
        BindingOperations.SetBinding((DependencyObject)(object)visualData, VisualData.LabelVisibilityProperty, new Binding
        {
            Source = this,
            Mode = BindingMode.OneWay,
            Path = new PropertyPath(LabelVisibilityProperty)
        });
        visualData.LabelValue = contourLogicalData.ZLevel;
        return visualData;
    }

    private VisualData CreateVisualDataRaw(ContourLogicalData contourLogicalData)
    {
        return new VisualData(contourLogicalData.Points, HorizontalValueToScreen, VerticalValueToScreen);
    }

    private void FireCollectionReset()
    {
        if (CollectionChanged != null)
        {
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    public void Add(VisualData item)
    {
        throw new NotSupportedException();
    }

    public void Clear()
    {
        throw new NotSupportedException();
    }

    public bool Contains(VisualData item)
    {
        return _visualData.Contains(item, null);
    }

    public void CopyTo(VisualData[] array, int arrayIndex)
    {
        _visualData.CopyTo(array, arrayIndex);
    }

    public bool Remove(VisualData item)
    {
        throw new NotSupportedException();
    }

    public bool Reversed
    {
        get
        {
            return _reversed;
        }
        set
        {
            if (value != _reversed)
            {
                _reversed = value;
                FireCollectionReset();
            }
        }
    }

    public int Count => _visualData.Count();

    public bool IsReadOnly => true;

    public DashStyle DashStyle
    {
        get
        {
            return _dashStyle;
        }
        set
        {
            if (_dashStyle != value)
            {
                _dashStyle = value;
                OnPropertyChanged("DashStyle");
            }
        }
    }

    public Brush Stroke
    {
        get
        {
            return _stroke;
        }
        set
        {
            if (_stroke != value)
            {
                _stroke = value;
                OnPropertyChanged("Stroke");
            }
        }
    }

    public ColorScheme ColorScheme
    {
        get
        {
            return _model.ColorScheme;
        }
        set
        {
            _model.ColorScheme = value;
        }
    }

    public LineStyle LineStyle
    {
        get
        {
            return _model.LineStyle;
        }
        set
        {
            _model.LineStyle = value;
        }
    }

    public Color ContoursColor
    {
        get
        {
            return _model.ContoursColor;
        }
        set
        {
            _model.ContoursColor = value;
        }
    }
    public void Initialize(global::Quire.Controls.Plots.Presenter.IValueToScreen horizontalTransformer, global::Quire.Controls.Plots.Presenter.IValueToScreen verticalTransformer) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void UpdateBrushes(Brush[] brushes)
    {
        if (brushes != null && brushes.Length != 0)
        {
            ContourLogicalDataCollection contourLogicalData = ContourLogicalData;
            for (int num = 0; num < _visualData.Length; num++)
            {
                int extendedIntervalIndex = contourLogicalData.GetExtendedIntervalIndex(num);
                if (-1 < extendedIntervalIndex && extendedIntervalIndex < brushes.Length)
                {
                    _visualData[num].Fill = brushes[extendedIntervalIndex];
                }
                else
                {
                    _visualData[num].Fill = null;
                }
            }
        }
        else
        {
            VisualData[] visualData = _visualData;
            for (int visualDataIndex = 0; visualDataIndex < visualData.Length; visualDataIndex++)
            {
                visualData[visualDataIndex].Fill = null;
            }
        }
    }

    public bool ShowLabels
    {
        get
        {
            return _model.ShowLabels;
        }
        set
        {
            _model.ShowLabels = value;
        }
    }

    public double LineWeight
    {
        get
        {
            return _model.LineWeight;
        }
        set
        {
            _model.LineWeight = value;
        }
    }
    internal global::System.Windows.Input.CommandBindingCollection CommandBindings => _viewModelHelper.CommandBindings;
    public event global::System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;
    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    private void SetContoursLineStyle(object parameter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SetColorScheme(global::Quire.Controls.Plots.Contour.ColorScheme newColorScheme) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SetLineWeight(double newLineWeight) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SetContoursLineColor(global::System.Windows.Media.Color newLinesColor) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Plots.Contour.View.VisualDataCollection+PropertyNames, Ptc.Controls.Plots")]
    public static partial class PropertyNames
    {
        [global::Quire.Mirror.MirrorStub]
        public const string IsResizing = "IsResizing";
        [global::Quire.Mirror.MirrorStub]
        public const string PlotRenderTransform = "PlotRenderTransform";
    }

    private IValueToScreen VerticalValueToScreen { get; set; }

    private IValueToScreen HorizontalValueToScreen { get; set; }
}
