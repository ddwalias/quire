#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Plots.Contour.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.Traces;
using Quire.Controls.Plots.ViewModel;
using Quire.Wpf;

namespace Quire.Controls.Plots.Contour.View;

public partial class VisualDataCollection
{

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

    private IValueToScreen VerticalValueToScreen { get; set; }

    private IValueToScreen HorizontalValueToScreen { get; set; }

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
}
