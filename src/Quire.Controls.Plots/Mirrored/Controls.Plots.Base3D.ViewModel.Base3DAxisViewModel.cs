#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;

namespace Quire.Controls.Plots.Base3D.ViewModel;

public abstract partial class Base3DAxisViewModel
{
    private bool _isActive;

    private bool _isHighlighted;

    private Color _lineColor;

    private IPlotAxisView _plotAxisView;

    public Action<ICalculatable> CalculatableAdded;

    private static readonly Color _defaultLineColor = new Color
    {
        A = byte.MaxValue,
        R = 112,
        G = 112,
        B = 112
    };

    private static readonly Color _higlightedLineColor = Colors.Black;

    private static readonly Color _activeLineColor = Colors.DodgerBlue;

    private double _height;

    internal IAxisModel PlotAxisModel { get; private set; }

    public Interval ClippingInterval
    {
        get
        {
            // Match Mathcad IL: single return local across branches preserves IL layout.
            Interval result;
            if (_plotAxisView == null || PlotAxisModel == null)
            {
                result = new Interval
                {
                    Lower = double.NegativeInfinity,
                    Upper = double.PositiveInfinity
                };
            }
            else
            {
                Interval tickInterval = GetTickInterval(0);
                Interval tickInterval2 = GetTickInterval(_plotAxisView.TickMarks);
                // Match Mathcad IL: preserve float64 temporary local store/load before branch.
                if (tickInterval.Center < (tickInterval2.Center switch { var center2 => center2 }))
                {
                    result = new Interval
                    {
                        Lower = tickInterval.GetValueByNormalizedPosition(1.0),
                        Upper = tickInterval2.GetValueByNormalizedPosition(0.0)
                    };
                }
                else
                {
                    result = new Interval
                    {
                        Lower = tickInterval2.GetValueByNormalizedPosition(0.0),
                        Upper = tickInterval.GetValueByNormalizedPosition(1.0)
                    };
                }
            }
            return result;
        }
    }

    protected double? ResultStart { get; set; }

    protected double? ResultEnd { get; set; }

    public bool IsActive
    {
        get
        {
            return _isActive;
        }
        set
        {
            _isActive = value;
            UpdateLineColor();
        }
    }

    public bool IsHighlighted
    {
        get
        {
            return _isHighlighted;
        }
        set
        {
            _isHighlighted = value;
            UpdateLineColor();
        }
    }

    public Color LineColor
    {
        get
        {
            return _lineColor;
        }
        private set
        {
            if (!_lineColor.Equals(value))
            {
                _lineColor = value;
                FirePropertyChanged("LineColor");
            }
        }
    }

    internal SizeChangedEventHandler ViewSizeChanged { private get; set; }

    public AxisRangeForXamlSerialization AxisRangeForXamlSerialization
    {
        get
        {
            return new AxisRangeForXamlSerialization
            {
                Start = PlotAxisModel.Start,
                End = PlotAxisModel.End
            };
        }
        set
        {
            PlotAxisModel.SetRange(value.Start, value.End);
        }
    }

    public string[] Labels => PlotAxisModel.MajorTickMarks.Labels;

    internal string[] MajorTickMarkLabels => PlotAxisModel.MajorTickMarks.Labels;

    public string[] FilteredLabels => PlotAxisModel.FilteredLabels;

    public bool ShowTicks
    {
        get
        {
            return PlotAxisModel.ShowTicks;
        }
        set
        {
            PlotAxisModel.ShowTicks = value;
        }
    }

    public bool ShowNumbers
    {
        get
        {
            return PlotAxisModel.ShowNumbers;
        }
        set
        {
            PlotAxisModel.ShowNumbers = value;
        }
    }

    public double Height
    {
        get
        {
            return _height;
        }
        set
        {
            if (value != _height)
            {
                _height = value;
                FirePropertyChanged("Height");
            }
        }
    }

    public event Action RangeChanged;

    public event Action<object> ValueToScreenChanged;

    public event Func<FocusNavigationDirection, bool> CanTraverseEvent;

    public event Action<FocusNavigationDirection> HandleTraverseEvent;

    public event Action RequestFocus;

    public event Action TickMarkLabelsUpdated
    {
        add
        {
            PlotAxisModel.TickMarkLabelsUpdated += value;
        }
        // Match Mathcad IL: remove accessor calls += instead of -=.
        remove
        {
            PlotAxisModel.TickMarkLabelsUpdated += value;
        }
    }

    internal Interval GetTickInterval(int tickNumber)
    {
        return new Interval
        {
            Lower = ((PlotGraphLinearAxis)_plotAxisView).GetTickPositionByTickNumber(tickNumber - 1),
            Upper = ((PlotGraphLinearAxis)_plotAxisView).GetTickPositionByTickNumber(tickNumber)
        };
    }

    protected Base3DAxisViewModel(AxesType axisType, PlotType plotType)
    {
        PlotAxisModel = new AxisModel(axisType, plotType, null);
        PlotAxisModel.RangeChanged += FireRangeChanged;
        PlotAxisModel.PropertyChanged += base.ModelPropertyChangedInner;
        UpdateLineColor();
    }

    private void FireRangeChanged()
    {
        if (RangeChanged != null)
        {
            RangeChanged();
        }
    }

    private void InitializeLimitExpression(IAxisLimitEditor limitExpression, Action limitValueChangedHandler)
    {
        if (limitExpression != null)
        {
            FireCalculatableAdded(limitExpression);
            limitExpression.ValueChanged += limitValueChangedHandler;
        }
    }

    private void StartExpressionValueChanged()
    {
        PlotAxisModel.StartValueChanged();
        LimitsExpressionValueChanged();
    }

    private void FireCalculatableAdded(ICalculatable calculatable)
    {
        if (CalculatableAdded != null)
        {
            CalculatableAdded(calculatable);
        }
    }

    private void UpdateLineColor()
    {
        LineColor = (IsActive ? _activeLineColor : (IsHighlighted ? _higlightedLineColor : _defaultLineColor));
    }

    public void HookupView(IPlotAxisView axisView)
    {
        DebugTools.DevAssert(_plotAxisView == null && axisView != null, null);
        if (_plotAxisView != null && ValueToScreenChanged != null)
        {
            _plotAxisView.ValueToScreenChanged -= ValueToScreenChanged;
            _plotAxisView.SizeChanged -= ViewSizeChanged;
        }
        _plotAxisView = axisView;
        new AxisPresenter(_plotAxisView, PlotAxisModel);
        InitializeLimitExpression(PlotAxisModel.StartExpression, StartExpressionValueChanged);
        InitializeLimitExpression(PlotAxisModel.SecondTickExpression, LimitsExpressionValueChanged);
        InitializeLimitExpression(PlotAxisModel.EndExpression, LimitsExpressionValueChanged);
        if (ViewSizeChanged != null)
        {
            _plotAxisView.SizeChanged += ViewSizeChanged;
        }
        if (ValueToScreenChanged != null)
        {
            _plotAxisView.ValueToScreenChanged += ValueToScreenChanged;
            ValueToScreenChanged(_plotAxisView);
        }
    }

    public void HandleTraverse(FocusNavigationDirection direction)
    {
        if (HandleTraverseEvent != null)
        {
            HandleTraverseEvent(direction);
        }
    }

    public bool CanTraverse(FocusNavigationDirection direction)
    {
        if (CanTraverseEvent != null)
        {
            return CanTraverseEvent(direction);
        }
        return false;
    }

    private void FirePropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }

    public void LimitEquationRequestFocus()
    {
        if (RequestFocus != null)
        {
            RequestFocus();
        }
    }

    internal virtual void SetResultRange(double? resultStart, double? resultEnd)
    {
    }

    protected virtual void LimitsExpressionValueChanged()
    {
        double start = (ResultStart.HasValue ? ResultStart.Value : PlotAxisModel.StartDefault);
        double end = (ResultEnd.HasValue ? ResultEnd.Value : PlotAxisModel.EndDefault);
        PlotAxisModel.SetRange(start, end);
    }

    public static partial class PropertyNames
    {
        public const string Height = "Height";

        public const string ShowTicks = "ShowTicks";

        public const string ShowNumbers = "ShowNumbers";

        public const string LineColor = "LineColor";
    }
}
