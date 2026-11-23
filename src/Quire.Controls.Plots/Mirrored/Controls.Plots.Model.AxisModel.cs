#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
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

    private bool SetRangeToAngularAxis()
    {
        return SetTickMarksByEffectiveValues(StartDefault, EndDefault, StartDefault, EndDefault);
    }

    private bool SetTickMarksByEffectiveValues(double effectiveStart, double effectiveEnd, double start, double end)
    {
        ClearCustomErrors();
        if (effectiveStart >= effectiveEnd)
        {
            SetRangeError();
            if (effectiveStart > effectiveEnd)
            {
                return false;
            }
        }
        start = effectiveStart;
        end = effectiveEnd;
        DealWithZeroLengthRange(ref start, ref end);
        // The IL joins both scale branches on one result local and a single return.
        bool isSet;
        if (Scaling == AxisScaling.Logarithmic)
        {
            isSet = SetLogarithmicScale(start, end);
        }
        else
        {
            isSet = SetLinearScale(start, end);
        }
        return isSet;
    }

    public void SetDisplayNumberFormat(NumberFormat numberFormat, bool isLocalValue)
    {
        DisplayNumberFormat = numberFormat;
        IsDisplayNumberFormatLocal = isLocalValue;
    }

    public void UpdateTickMarkValues(bool useSecondTickValue)
    {
        BuildTickMarksValues(useSecondTickValue);
        FormatTickMarkValues();
    }

    private bool SetLinearScale(double start, double end)
    {
        if (!IsDiscrete)
        {
            MajorTickMarks.Count = DefaultTickMarks;
            if (SecondTickExpression.IsDefaultValue && UseBetterTickMarks)
            {
                CalculateBetterTickmarks(ref start, ref end);
            }
        }
        bool useSecondTickValue = !IsDiscrete && ValidateSecondTickValue(start, end);
        MinorTickMarks.Show = false;
        Start = start;
        End = end;
        if (MajorTickMarks.Count > 1)
        {
            UpdateTickMarkValues(useSecondTickValue);
        }
        StartExpression.SetPreCalculatedValue(Start);
        SetSecondTickPreCalculatedValue();
        EndExpression.SetPreCalculatedValue(End);
        return true;
    }

    public void SetRange(double? min, double? max)
    {
        // The IL creates the closure only after the readiness test, so the body sits inside the if.
        if (ExpressionsAreReady)
        {
            bool flag;
            bool labelsUpdated = false;
            Action onLabelsUpdated = () =>
            {
                labelsUpdated = true;
            };
            try
            {
                TickMarkLabelsUpdated += onLabelsUpdated;
                double? num = (StartExpression.IsValueValid && !IsStringLabel()) ? new double?(Start) : min;
                double? num2 = (EndExpression.IsValueValid && !IsStringLabel()) ? new double?(End) : max;
                switch (Type)
                {
                    case AxesType.Angular:
                        flag = SetRangeToAngularAxis();
                        break;
                    case AxesType.Radial:
                        flag = SetRangeToRadialAxis(min, max);
                        break;
                    case AxesType.X:
                    case AxesType.Y:
                    case AxesType.Z:
                        flag = SetRangeToLinearAxis(num.HasValue ? num.Value : StartDefaultByScaling, num2.HasValue ? num2.Value : EndDefault);
                        break;
                    default:
                        flag = false;
                        break;
                }
            }
            finally
            {
                TickMarkLabelsUpdated -= onLabelsUpdated;
            }
            if (flag)
            {
                if (!labelsUpdated)
                {
                    FormatTickMarkValues();
                }
                FireRangeChanged();
            }
        }
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

    // Declared here, not at the end, so the lambda's display-class ordinal matches Mathcad's (compile-order device).
    public int GetNumberOfTracesForEquation(IPlotEquation equation)
    {
        return _equations.First((IEquationItem eq) => eq.Equation == equation).Traces.ToList().Count();
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

    public void SetTickMarksPattern(double startGap, double[] pattern)
    {
        MajorTickMarks.StartGap = startGap;
        MajorTickMarks.Pattern = pattern;
    }

    private int ConvertLegendPositionEnumToLegendPositionIndex(string enumValue)
    {
        if (AxesTypeToIndex.Index(Type) == AxesIndex.Range)
        {
            return Convert.ToInt32(Enum.Parse(typeof(LegendPositions.XLegendPosition), enumValue));
        }
        return Convert.ToInt32(Enum.Parse(typeof(LegendPositions.YLegendPosition), enumValue));
    }

    private string ConvertLegendPositionIndexToLegendPositionEnum()
    {
        if (AxesTypeToIndex.Index(Type) == AxesIndex.Range)
        {
            return ((LegendPositions.XLegendPosition)LegendPositionIndex).ToString();
        }
        return ((LegendPositions.YLegendPosition)LegendPositionIndex).ToString();
    }

    public Color GridLineColor
    {
        get
        {
            try
            {
                // The IL keeps the converted object in its own local before the type test.
                object gridLine = ColorConverter.ConvertFromString(_gridLineColorHex);
                if (gridLine is Color result)
                {
                    return result;
                }
            }
            catch
            {
            }
            try
            {
                object defaultGridLine = ColorConverter.ConvertFromString(PlotCommonConstants.GridlinesDefaultColorHex);
                if (defaultGridLine is Color result2)
                {
                    return result2;
                }
            }
            catch
            {
            }
            return Colors.Gray;
        }
        set
        {
            string text = value.ToString();
            if (_gridLineColorHex != text)
            {
                _gridLineColorHex = text;
                OnPropertyChanged("GridLineColor");
            }
        }
    }

    private void UpdateFilteredLabels()
    {
        string[] labels = MajorTickMarks.Labels;
        int num = labels.Length;
        bool flag = LimitHasErrors(StartExpression);
        bool flag2 = LimitHasErrors(SecondTickExpression);
        bool flag3 = LimitHasErrors(EndExpression);
        if (flag | flag2 | flag3)
        {
            FilteredLabels = new string[num];
            int num2 = num - 1;
            FilteredLabels[0] = (flag ? ErrorIndicator : labels[0]);
            if (num2 > 1)
            {
                FilteredLabels[1] = (flag2 ? ErrorIndicator : labels[1]);
            }
            else
            {
                DebugTools.DevAssert(num2 == 1, null);
            }
            for (int num3 = 2; num3 < num2; num3++)
            {
                FilteredLabels[num3] = string.Empty;
            }
            FilteredLabels[num2] = (flag3 ? ErrorIndicator : labels[num2]);
        }
        else
        {
            FilteredLabels = labels;
        }
    }

    private bool SetRangeToRadialAxis(double? min, double? max)
    {
        double effectiveStart;
        double effectiveEnd;
        double start;
        double num;
        if (min.HasValue && max.HasValue)
        {
            start = min.Value;
            num = max.Value;
            if (StartExpression.IsValueValid)
            {
                effectiveStart = StartExpression.CalculatedValue;
                effectiveEnd = EndExpression.IsValueValid ? EndExpression.CalculatedValue : num;
            }
            else
            {
                double num2 = Math.Max(Math.Abs(min.Value), Math.Abs(max.Value));
                effectiveStart = Math.Max(0.0, min.Value);
                effectiveEnd = EndExpression.IsValueValid ? Math.Max(Math.Abs(min.Value), EndExpression.CalculatedValue) : num2;
            }
        }
        else
        {
            effectiveStart = StartExpression.IsValueValid ? StartExpression.CalculatedValue : StartDefaultByScaling;
            effectiveEnd = EndExpression.IsValueValid ? EndExpression.CalculatedValue : EndDefault;
            start = Start;
            num = End;
        }
        return SetTickMarksByEffectiveValues(effectiveStart, effectiveEnd, start, num);
    }

    private bool SetRangeToLinearAxis(double start, double end)
    {
        double effectiveStart = (StartExpression.IsValueValid && !IsStringLabel()) ? StartExpression.CalculatedValue : start;
        double effectiveEnd = (EndExpression.IsValueValid && !IsStringLabel()) ? EndExpression.CalculatedValue : end;
        return SetTickMarksByEffectiveValues(effectiveStart, effectiveEnd, start, end);
    }

    private bool ValidateSecondTickValue(double start, double end)
    {
        if (SecondTickExpression != null && SecondTickExpression.IsValueValid)
        {
            if (SecondTickExpression.CalculatedValue <= start)
            {
                SetAxisLimitEditorError(SecondTickExpression, "Ptc.Plots.Axis.SECOND_TICK_TO_BE_GREATER_THAN_MIN");
                return false;
            }
            if (Type == AxesType.Angular && SecondTickExpression.CalculatedValue > 355.0)
            {
                SetAxisLimitEditorError(SecondTickExpression, "Ptc.Plots.Axis.ANGULAR_MAX_TO_BE_SMALLER_THAN_355");
                return false;
            }
            double num = SecondTickExpression.CalculatedValue - start;
            try
            {
                if (Convert.ToInt32(Math.Ceiling((end - start) / num)) <= MaxTickmarks)
                {
                    return true;
                }
            }
            catch (OverflowException)
            {
            }
            SetAxisLimitEditorError(SecondTickExpression, "Ptc.Plots.Axis.TOO_MANY_TICKMARKS");
            return false;
        }
        return true;
    }

    private bool SetLogarithmicScale(double start, double end)
    {
        bool isValid = true;
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(start <= end, null))
        {
            SetRangeError();
            isValid = false;
        }
        if (end <= 0.0)
        {
            SetAxisLimitEditorError(EndExpression, "Ptc.Plots.Axis.ERROR_ONE_LIMIT_MUST_BE_POSITIVE");
            isValid = false;
        }
        if (!isValid)
        {
            return false;
        }
        if (start <= 0.0)
        {
            start = Math.Min(end / (double)DefaultTickMarks, 0.01);
        }
        double num = Math.Floor(Math.Log10(start));
        double num2 = Math.Ceiling(Math.Log10(end));
        double num3 = num2 - num;
        if (num3 < 0.5)
        {
            num3 = 1.0;
            num = num2 - num3;
        }
        int num4;
        int count;
        try
        {
            num4 = Convert.ToInt32(Math.Ceiling(num3 / (double)(DefaultTickMarks - 1)));
            count = Convert.ToInt32(num3 / (double)num4 + 1.0);
        }
        catch
        {
            return false;
        }
        Start = Math.Pow(10.0, num);
        End = Math.Pow(10.0, num2);
        MajorTickMarks.Count = count;
        MajorTickMarks.Values = new double[MajorTickMarks.Count];
        MinorTickMarks.Count = (MajorTickMarks.Count - 1) * 8;
        MinorTickMarks.Values = new double[MinorTickMarks.Count];
        for (int i = 0; i < MajorTickMarks.Count - 1; i++)
        {
            double num5 = Math.Pow(10.0, num + (double)(num4 * i));
            MajorTickMarks.Values[i] = num5;
            for (int j = 0; j < 8; j++)
            {
                MinorTickMarks.Values[i * 8 + j] = num5 * (double)(j + 2);
            }
        }
        MajorTickMarks.Values[MajorTickMarks.Count - 1] = End;
        MinorTickMarks.Show = true;
        MinorTickMarks.StartGap = Math.Log10(2.0);
        MinorTickMarks.Pattern = CreateLogMinorTickmarksPattern();
        FormatTickMarkValues();
        StartExpression.SetPreCalculatedValue(Start);
        EndExpression.SetPreCalculatedValue(End);
        SetSecondTickPreCalculatedValue();
        return true;
    }}
