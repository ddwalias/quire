#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Quire.Controls.Core;
using Quire;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Model;

internal partial class AxisModel
{
    public IPlotTitleModel AxisTitleModel { get; set; }

    public AxisMarkersCollection Markers => _markers;

    public IEnumerable<IEquationItem> Equations => _equations;

    public bool CanHaveMarkers => !IsDiscrete;

    public bool XIsString { get; set; }

    public bool YIsString { get; set; }

    public bool IsDiscrete { get; set; }

    public double LegendWidth { get; set; }

    public string AxisXmlNodeName
    {
        get
        {
            switch (Type)
            {
                case AxesType.X:
                    return "xAxis";
                case AxesType.Y:
                    return "yAxis";
                case AxesType.Z:
                    return "zAxis";
                case AxesType.Angular:
                    return "angularAxis";
                case AxesType.Radial:
                    return "radialAxis";
                default:
                    return "";
            }
        }
    }

    public string DomainXmlNodeName
    {
        get
        {
            switch (Type)
            {
                case AxesType.X:
                    return "xyDomain";
                case AxesType.Y:
                    return "xyDomain";
                case AxesType.Angular:
                    return "angularDomain";
                case AxesType.Radial:
                    return "radialDomain";
                case AxesType.Z:
                    return "zDomain";
                default:
                    return "";
            }
        }
    }

    public AxesType Type { get; private set; }

    public PlotType PlotType { get; private set; }

    public Positions Position { get; set; }

    public int PositionTickMark { get; set; }

    public int LegendPositionIndex { get; set; }

    public bool UseBetterTickMarks { get; set; }

    public double StartDefault { get; private set; }

    public double EndDefault { get; private set; }

    public int DefaultTickMarks { get; private set; }

    public int MaxTickmarks { get; private set; }

    public bool IsDisplayNumberFormatLocal { get; private set; }

    public TickScale MajorTickMarks { get; set; }

    public TickScale MinorTickMarks { get; set; }

    public string[] FilteredLabels { get; private set; }

    public bool ShowTicks
    {
        get => MajorTickMarks.Show;
        set
        {
            if (ShowTicks != value)
            {
                MajorTickMarks.Show = value;
                OnPropertyChanged("ShowTicks");
            }
        }
    }

    public bool ShowGridLines
    {
        get => _showGridLines;
        set
        {
            if (_showGridLines != value)
            {
                _showGridLines = value;
                OnPropertyChanged("ShowGridLines");
            }
        }
    }

    public bool ShowNumbers
    {
        get => _showNumbers;
        set
        {
            if (_showNumbers != value)
            {
                _showNumbers = value;
                OnPropertyChanged("ShowNumbers");
            }
        }
    }

    public double LinearizedStart => LinearizeStartEnd(Start) ?? double.MinValue;

    public double LinearizedEnd => LinearizeStartEnd(End) ?? double.MaxValue;

    private double StartDefaultByScaling
    {
        get
        {
            if (Scaling != AxisScaling.Logarithmic)
            {
                return StartDefault;
            }
            return 0.01;
        }
    }

    private bool ExpressionsAreReady => IsReady(StartExpression) && IsReady(EndExpression) && IsReady(SecondTickExpression);

    public bool Contains(double coordinate)
    {
        switch (Type)
        {
            case AxesType.X:
            case AxesType.Y:
            case AxesType.Z:
                return IsInStartEndInterval(coordinate);
            case AxesType.Radial:
                return coordinate <= End;
            default:
                return true;
        }
    }

    private bool IsInStartEndInterval(double coordinate)
    {
        if (Start <= coordinate)
        {
            return coordinate <= End;
        }
        return false;
    }

    private static bool LimitHasErrors(IAxisLimitEditor axisLimitEditor)
    {
        return axisLimitEditor != null && axisLimitEditor.HasErrors;
    }

    public bool HasAxisError()
    {
        return StartExpression.ErrorCount > 0 || EndExpression.ErrorCount > 0;
    }

    private static bool IsReady(IAxisLimitEditor axisLimitEditor)
    {
        if (axisLimitEditor != null)
        {
            return !axisLimitEditor.IsWaitingForEngineResponse;
        }
        return true;
    }

    private static double OrderFunction(double step)
    {
        return Math.Pow(10.0, Math.Floor(Math.Log10(step)));
    }

    private static double RoundWithOrder(double exp, double order)
    {
        return Math.Round(exp / order) * order;
    }

    private static double CeilingWithOrder(double exp, double order)
    {
        return Math.Ceiling(exp / order) * order;
    }

    private static double FloorWithOrder(double exp, double order)
    {
        return Math.Floor(exp / order) * order;
    }

    private void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private void FireChanged()
    {
        if (Changed != null)
        {
            Changed();
        }
    }

    private void FireRangeChanged()
    {
        if (RangeChanged != null)
        {
            RangeChanged();
        }
    }

    private void FireScalingChanged()
    {
        if (ScalingChanged != null)
        {
            ScalingChanged(Scaling);
        }
    }

    private void FireNumberFormatInitialized()
    {
        if (NumberFormatInitialized != null)
        {
            NumberFormatInitialized();
        }
    }

    private void FireTickMarkLabelsUpdated()
    {
        Action tickMarkLabelsUpdated = TickMarkLabelsUpdated;
        if (tickMarkLabelsUpdated != null)
        {
            tickMarkLabelsUpdated();
        }
    }

    public double Start
    {
        get
        {
            return _start;
        }
        set
        {
            if (_start != value)
            {
                _start = value;
                if (IsLinearizedRangeAssertEnabled.HasValue)
                {
                    IsLinearizedRangeAssertEnabled = true;
                }
                OnPropertyChanged("Start");
            }
        }
    }

    public double End
    {
        get
        {
            return _end;
        }
        set
        {
            if (value != _end)
            {
                _end = value;
                if (IsLinearizedRangeAssertEnabled.HasValue)
                {
                    IsLinearizedRangeAssertEnabled = true;
                }
                OnPropertyChanged("End");
            }
        }
    }

    public AxisScaling Scaling
    {
        get
        {
            return _scaling;
        }
        set
        {
            if (_scaling == value)
            {
                return;
            }
            bool hasValue = IsLinearizedRangeAssertEnabled.HasValue;
            IsLinearizedRangeAssertEnabled = null;
            _scaling = value;
            MajorTickMarks.UseMagnitudeThreashold = _scaling == AxisScaling.Linear;
            FireScalingChanged();
            IsLinearizedRangeAssertEnabled = (hasValue ? new bool?(true) : ((bool?)null));
        }
    }

    public NumberFormat DisplayNumberFormat
    {
        get
        {
            return _displayNumberFormat;
        }
        private set
        {
            if (value != _displayNumberFormat)
            {
                _displayNumberFormat = value;
                OnPropertyChanged("DisplayNumberFormat");
            }
        }
    }

    public bool? IsLinearizedRangeAssertEnabled
    {
        get
        {
            return _isLinearizedRangeAssertEnabled;
        }
        set
        {
            bool? current = _isLinearizedRangeAssertEnabled;
            bool? newValue = value;
            // Non-short-circuit '&': Release IL evaluates both comparisons before one branch.
            if ((current.GetValueOrDefault() == newValue.GetValueOrDefault()) & (current.HasValue == newValue.HasValue))
            {
                return;
            }
            _isLinearizedRangeAssertEnabled = value;
            _ = LinearizedStart;
            _ = LinearizedEnd;
        }
    }

    public double GetRadialPointsStart()
    {
        if (DebugTools.DevAssert(AxesType.Radial == Type, null).Failed)
        {
            return double.NaN;
        }
        double num = Start;
        if (Scaling == AxisScaling.Logarithmic)
        {
            if (num <= 0.0)
            {
                num = 0.01;
            }
            num = Math.Log10(num);
        }
        return num;
    }

    private static void SetAxisLimitEditorError(IAxisLimitEditor axisLimitEditor, string errorCodeString)
    {
        if (!axisLimitEditor.IsDefaultValue)
        {
            axisLimitEditor.SetNonCalculationRelatedCustomError(errorCodeString);
        }
    }

    private bool IsStringLabel()
    {
        switch (AxisXmlNodeName)
        {
            case "xAxis":
                return XIsString;
            case "yAxis":
                return YIsString;
            default:
                return false;
        }
    }

    private static double[] CreateLogMinorTickmarksPattern()
    {
        double[] array = new double[8];
        for (int num = 3; num < 10; num++)
        {
            array[num - 3] = Math.Log10((double)num / ((double)num - 1.0));
        }
        array[7] = Math.Log10(2.2222222222222223);
        return array;
    }

    private static void ValidateAxisLimitExpression(IAxisLimitEditor oldExpression)
    {
        if (oldExpression != null)
        {
            DebugTools.DevAssert(false, null);
        }
    }

    private void OnExpressionHasErrorsChanged()
    {
        FormatTickMarkValues();
    }

    private void ClearCustomErrors()
    {
        StartExpression.ClearNonCalculationRelatedCustomErrors();
        SecondTickExpression.ClearNonCalculationRelatedCustomErrors();
        EndExpression.ClearNonCalculationRelatedCustomErrors();
    }

    private void SetSecondTickPreCalculatedValue()
    {
        if (MajorTickMarks != null && MajorTickMarks.Values != null && MajorTickMarks.Values.Length > 1)
        {
            SecondTickExpression.SetPreCalculatedValue(MajorTickMarks.Values[1]);
        }
    }

    private static bool SetDoubleValueIfValid(ref double variableToChange, double newValue)
    {
        bool flag = newValue.IsValid();
        if (flag)
        {
            variableToChange = newValue;
        }
        return flag;
    }

    public void RemoveEquation(IEquationItem equation)
    {
        _equations.Remove(equation);
    }

    public void StartValueChanged()
    {
        if (StartExpression.IsValueValid && SecondTickExpression.IsValueValid && SecondTickExpression.CalculatedValue <= StartExpression.CalculatedValue)
        {
            SecondTickExpression.IsDefaultValue = true;
        }
    }

    public IAxisLimitEditor StartExpression
    {
        get
        {
            return _startExpression;
        }
        set
        {
            ValidateAxisLimitExpression(_startExpression);
            if (_startExpression != null)
            {
                _startExpression.HasErrorsChanged -= OnExpressionHasErrorsChanged;
            }
            _startExpression = value;
            _startExpression.SetPreCalculatedValue(Start);
            _startExpression.HasErrorsChanged += OnExpressionHasErrorsChanged;
            OnPropertyChanged("StartExpression");
        }
    }

    public IAxisLimitEditor SecondTickExpression
    {
        get
        {
            return _secondTickExpression;
        }
        set
        {
            ValidateAxisLimitExpression(_secondTickExpression);
            if (_secondTickExpression != null)
            {
                _secondTickExpression.HasErrorsChanged -= OnExpressionHasErrorsChanged;
            }
            _secondTickExpression = value;
            SetSecondTickPreCalculatedValue();
            _secondTickExpression.HasErrorsChanged += OnExpressionHasErrorsChanged;
            OnPropertyChanged("SecondTickExpression");
        }
    }

    public IAxisLimitEditor EndExpression
    {
        get
        {
            return _endExpression;
        }
        set
        {
            ValidateAxisLimitExpression(_endExpression);
            if (_endExpression != null)
            {
                _endExpression.HasErrorsChanged -= OnExpressionHasErrorsChanged;
            }
            _endExpression = value;
            _endExpression.SetPreCalculatedValue(End);
            _endExpression.HasErrorsChanged += OnExpressionHasErrorsChanged;
            OnPropertyChanged("EndExpression");
        }
    }

    private double Linearize(double value)
    {
        if (Scaling != AxisScaling.Linear)
        {
            if (!(value > 0.0))
            {
                return double.NegativeInfinity;
            }
            return Math.Log10(value);
        }
        return value;
    }

    private void DealWithZeroLengthRange(ref double min, ref double max)
    {
        if (min != max)
        {
            return;
        }
        if (min < 0.0)
        {
            max = 0.0;
            if (Type != AxesType.Radial)
            {
                min *= 2.0;
            }
        }
        else if (min > 0.0)
        {
            min = 0.0;
            if (Type != AxesType.Radial)
            {
                max *= 2.0;
            }
        }
        else
        {
            min = Type == AxesType.Radial ? 0 : -1;
            max = 1.0;
        }
    }

    private void SetRangeError()
    {
        SetAxisLimitEditorError(StartExpression, "Ptc.Plots.Axis.MIN_TO_BE_SMALLER_THAN_MAX");
        SetAxisLimitEditorError(EndExpression, "Ptc.Plots.Axis.MAX_TO_BE_GREATER_THAN_MIN");
    }
}
