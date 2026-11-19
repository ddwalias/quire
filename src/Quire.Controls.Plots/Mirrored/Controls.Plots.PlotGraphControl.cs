#nullable disable
#pragma warning disable

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

public partial class PlotGraphControl
{
    public PlotTrace selectedTrace
    {
        get
        {
            return (PlotTrace)GetValue(selectedTraceProperty);
        }
        set
        {
            SetValue(selectedTraceProperty, value);
        }
    }

    internal PlotGraphLinearAxisBase XAxis
    {
        get
        {
            return _xAxis;
        }
        set
        {
            if (_xAxis != null)
            {
                _xAxis.ShowGridLinesChanged -= OnAxisGridLinesChanged;
                _xAxis.TickMarksChangedEvent -= OnAxisGridLinesChanged;
            }
            _xAxis = value;
            if (_xAxis != null)
            {
                _xAxis.ShowGridLinesChanged += OnAxisGridLinesChanged;
                _xAxis.TickMarksChangedEvent += OnAxisGridLinesChanged;
            }
            InvalidateVisual();
        }
    }

    internal PlotGraphLinearAxisBase YAxis
    {
        get
        {
            return _yAxis;
        }
        set
        {
            if (_yAxis != null)
            {
                _yAxis.ShowGridLinesChanged -= OnAxisGridLinesChanged;
                _yAxis.TickMarksChangedEvent -= OnAxisGridLinesChanged;
            }
            _yAxis = value;
            if (_yAxis != null)
            {
                _yAxis.ShowGridLinesChanged += OnAxisGridLinesChanged;
                _yAxis.TickMarksChangedEvent += OnAxisGridLinesChanged;
            }
            InvalidateVisual();
        }
    }

    internal PlotGraphAxis AngularAxis
    {
        get
        {
            return _angularAxis;
        }
        set
        {
            if (_angularAxis != null)
            {
                _angularAxis.ShowGridLinesChanged -= OnAxisGridLinesChanged;
                _angularAxis.TickMarksChangedEvent -= OnAxisGridLinesChanged;
            }
            _angularAxis = value;
            if (_angularAxis != null)
            {
                _angularAxis.ShowGridLinesChanged += OnAxisGridLinesChanged;
                _angularAxis.TickMarksChangedEvent += OnAxisGridLinesChanged;
            }
            InvalidateVisual();
        }
    }

    internal PlotGraphAxis RadialAxis
    {
        get
        {
            return _radialAxis;
        }
        set
        {
            if (_radialAxis != null)
            {
                _radialAxis.ShowGridLinesChanged -= OnAxisGridLinesChanged;
                _radialAxis.TickMarksChangedEvent -= OnAxisGridLinesChanged;
            }
            _radialAxis = value;
            if (_radialAxis != null)
            {
                _radialAxis.ShowGridLinesChanged += OnAxisGridLinesChanged;
                _radialAxis.TickMarksChangedEvent += OnAxisGridLinesChanged;
            }
            InvalidateVisual();
        }
    }

    public IEnumerable<PlotTrace> TracesUnsorted => Children.OfType<PlotTrace>();

    internal Action<PlotTrace> OnTraceRemoved { private get; set; }

    internal Action<PlotTrace> OnTraceAdded { private get; set; }

    public int Count => TracesUnsorted.Count();
}
