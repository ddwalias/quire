#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Globalization;
using System.Xml;
using Quire.Calculation.Core;
using Quire.PersistentDataObjects;
using Quire.Controls.Core.Serialization;
using Quire.Xml;
using Quire.Controls.Core;
using Quire;
using Quire.Controls.Plots.Presenter;
using Quire.Serialization;

namespace Quire.Controls.Plots.Model;

internal partial class AxisModel
{
    public const double LogarithmicStartDefault = 0.01;
    private const double LogarithmicAxisStep = 1.0;
    private const string TickMarkNodeName = "positionticmark";
    private readonly IList<IEquationItem> _equations = new List<IEquationItem>();
    private readonly AxisMarkersCollection _markers = new AxisMarkersCollection();
    private IAxisLimitEditor _startExpression;
    private IAxisLimitEditor _secondTickExpression;
    private IAxisLimitEditor _endExpression;
    private AxisScaling _scaling;
    private double _start;
    private double _end;
    private bool _showNumbers = true;
    public static readonly string ErrorIndicator = '?'.ToString();
    private bool _showGridLines;
    private string _gridLineColorHex = PlotCommonConstants.GridlinesDefaultColorHex;
    private NumberFormat _displayNumberFormat;
    private bool? _isLinearizedRangeAssertEnabled;

    public event Func<IAxisModel, XmlNode, IPlotEquation> RequestAddEquation;
    public event Action Changed;
    public event Action RangeChanged;
    public event Action<AxisScaling> ScalingChanged;
    public event Action NumberFormatInitialized;
    public event PropertyChangedEventHandler PropertyChanged;
    public event Action TickMarkLabelsUpdated;

    public AxisModel(AxesType axisType, PlotType plotType, IPlotTitleModel axisTitleModel = null)
    {
        Type = axisType;
        PlotType = plotType;
        AxisModelDefinitionCollection.AxisModelDefinition axisModelDefinition = AxisModelDefinitionCollection.Instance[axisType, plotType];
        StartDefault = axisModelDefinition.StartDefault;
        EndDefault = axisModelDefinition.EndDefault;
        DefaultTickMarks = axisModelDefinition.TickMarksDefault;
        MaxTickmarks = axisModelDefinition.MaxTickmarks;
        Start = StartDefault;
        End = EndDefault;
        PositionTickMark = DefaultTickMarks / 2;
        MajorTickMarks = new TickScale
        {
            Show = true,
            MagnitudeThreashold = axisModelDefinition.DefaultTickMagnitudeThreashold,
            Count = DefaultTickMarks
        };
        MinorTickMarks = new TickScale
        {
            Show = false
        };
        AxisTitleModel = axisTitleModel;
        Position = Positions.Origin;
        ShowNumbers = true;
        Scaling = AxisScaling.Linear;
        IsDiscrete = false;
        UpdateTickMarkValues(true);
        UseBetterTickMarks = true;
    }

    public static partial class PropertyNames
    {
        public const string Start = "Start";
        public const string End = "End";
        public const string StartExpression = "StartExpression";
        public const string EndExpression = "EndExpression";
        public const string SecondTickExpression = "SecondTickExpression";
        public const string DisplayNumberFormat = "DisplayNumberFormat";
        public const string ShowTicks = "ShowTicks";
        public const string ShowNumbers = "ShowNumbers";
    }

    public IEnumerable<ICalculatable> AxisCalculatables
    {
        get
        {
            if (StartExpression != null)
            {
                yield return StartExpression;
            }
            if (SecondTickExpression != null)
            {
                yield return SecondTickExpression;
            }
            if (EndExpression != null)
            {
                yield return EndExpression;
            }
        }
    }

    internal void CalculateBetterTickmarks(ref double start, ref double end)
    {
        int numberOfIntervals = MajorTickMarks.Count - 1;
        // Pattern check materializes bool before brfalse.s to match Mathcad Release IL.
        if ((start < 0.0 && end > 0.0) is true)
        {
            CalculateBetterTickmarksWithZeroInRange(ref start, ref end, ref numberOfIntervals);
        }
        else
        {
            CalculateBetterTickmarksWhenZeroIsNotInRange(ref start, ref end, ref numberOfIntervals);
        }
        MajorTickMarks.Count = numberOfIntervals + 1;
    }

    private static void CalculateBetterTickmarksWithZeroInRange(ref double start, ref double end, ref int numberOfIntervals)
    {
        if (!(start < 0.0 && end > 0.0))
        {
            throw new ArgumentException("Zero is not in range, this function should not have been called.");
        }
        double ratio = -start / end;
        int numNeg = (int)Math.Round((double)numberOfIntervals * ratio / (1.0 + ratio));
        numNeg = Math.Max(numNeg, 1);
        numNeg = Math.Min(numNeg, numberOfIntervals - 1);
        int numPos = numberOfIntervals - numNeg;
        double negOrder = OrderFunction(-start / (double)numNeg);
        double posOrder = OrderFunction(end / (double)numPos);
        double minOrder = Math.Min(negOrder, posOrder);
        double negStep = RoundWithOrder(-start / (double)numNeg, 0.5 * negOrder);
        double posStep = RoundWithOrder(end / (double)numPos, 0.5 * posOrder);
        double step = Math.Max(negStep, posStep);
        if (step == 0.0)
        {
            step = 0.5 * minOrder;
        }
        if (SetDoubleValueIfValid(ref start, FloorWithOrder(start, 0.5 * minOrder)) && SetDoubleValueIfValid(ref end, CeilingWithOrder(end, 0.5 * minOrder)))
        {
            numNeg = (int)CeilingWithOrder(-start / step, 1.0);
            numPos = (int)CeilingWithOrder(end / step, 1.0);
            numberOfIntervals = numNeg + numPos;
            start = (double)(-numNeg) * step;
            end = (double)numPos * step;
        }
    }

    private void SetXmlNode(XmlNode value, XmlNamespaceManager nmspmgr) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    private double? LinearizeStartEnd(double value)
    {
        double num = Linearize(value);
        if (num.IsValid())
        {
            return num;
        }
        if (IsLinearizedRangeAssertEnabled.HasValue && IsLinearizedRangeAssertEnabled.Value && Scaling == AxisScaling.Logarithmic && !HasAxisError())
        {
            IsLinearizedRangeAssertEnabled = false;
            DebugTools.FailAssertion("Start or End values of axis model should not be negative when scaling is logarithmic.");
        }
        return null;
    }

    private void BuildTickMarksValues(bool useSecondTickValue)
    {
        int tickCount = MajorTickMarks.Count;
        if (DebugTools.DevAssert(tickCount > 1, null).Passed)
        {
            double spacing = (End - Start) / (double)(tickCount - 1);
            if (useSecondTickValue && SecondTickExpression != null && SecondTickExpression.IsValueValid && !SecondTickExpression.HasErrors)
            {
                spacing = SecondTickExpression.CalculatedValue - Start;
                tickCount = (int)Math.Ceiling((End - Start) / spacing);
                End = Start + spacing * (double)tickCount;
                tickCount++;
            }
            MajorTickMarks.Values = new double[tickCount];
            MajorTickMarks.Count = tickCount;
            for (int i = 0; i < tickCount - 1; i++)
            {
                MajorTickMarks.Values[i] = Start + spacing * (double)i;
            }
            MajorTickMarks.Values[tickCount - 1] = End;
        }
    }

    private static void CalculateBetterTickmarksWhenZeroIsNotInRange(ref double start, ref double end, ref int numberOfIntervals)
    {
        if (start < 0.0 && end > 0.0)
        {
            throw new ArgumentException("Zero is not in range, this function should not have been called");
        }
        if (end > start)
        {
            double step = (end - start) / (double)numberOfIntervals;
            double order = OrderFunction(step);
            step = RoundWithOrder(step, 0.5 * order);
            if (step == 0.0)
            {
                step = 0.5 * order;
            }
            double floor = FloorWithOrder(start, 0.5 * order);
            double ceiling = CeilingWithOrder(end, 0.5 * order);
            if (DebugTools.DevAssert(floor.IsValid() && ceiling.IsValid(), null).Passed)
            {
                start = floor;
                end = ceiling;
                int intervals = (int)CeilingWithOrder((end - start) / step, 1.0);
                numberOfIntervals = intervals;
                end = start + (double)numberOfIntervals * step;
            }
        }
    }

    public bool FormatTickMarkValues()
    {
        if (IsDiscrete)
        {
            return false;
        }
        NumberFormat numberFormat = (DisplayNumberFormat != null) ? DisplayNumberFormat.Clone() : new NumberFormat();
        numberFormat.ExponentialThreshold = 3;
        MajorTickMarks.FormatValues(numberFormat);
        UpdateFilteredLabels();
        FireTickMarkLabelsUpdated();
        return true;
    }

    public void AddEquation(IPlotEquation equation, int index)
    {
        EquationItem item = new EquationItem(equation);
        _equations.Insert(index, item);
    }

    public void AddEquation(XmlNode xmlNode)
    {
        RequestAddEquation(this, xmlNode);
    }

    protected xyDomainType CreateXyDomainType(IResultManager resultManager)
    {
        return new xyDomainType
        {
            startValue = _startExpression.GetDataObject(resultManager),
            endValue = _endExpression.GetDataObject(resultManager),
            secondTickValue = _secondTickExpression.IsDefaultValue ? null : _secondTickExpression.GetDataObject(resultManager),
            scaletype = scaleType.linear,
            autoscale = true,
            allownegativevaluesSpecified = true
        };
    }

    public axisBaseType GetDataObject(IResultManager resultManager)
    {
        xyDomainType domainData = CreateXyDomainType(resultManager);
        axisBaseType axisData;
        switch (Type)
        {
            case AxesType.X:
                axisData = new xAxis
                {
                    xyDomain = domainData
                };
                break;
            case AxesType.Y:
                axisData = new yAxis
                {
                    xyDomain = domainData
                };
                break;
            case AxesType.Z:
                axisData = new zAxis
                {
                    zDomain = domainData
                };
                break;
            default:
                DebugTools.DevAssert(false, null);
                return new axisBaseType();
        }
        axisData.axisGrid = new axisGridType
        {
            gridFrequency = new axisGridTypeGridFrequency
            {
                Value = MajorTickMarks.Count.ToString(CultureInfo.InvariantCulture)
            },
            gridLabels = new axisGridTypeGridLabels
            {
                display = ShowNumbers
            },
            tickMarks = new axisGridTypeTickMarks
            {
                display = ShowTicks
            },
            gridLines = new axisGridTypeGridLines()
        };
        axisData.rank = "1";
        axisData.axisLabel = new plotTitleType();
        axisData.axisLine = new axisBaseTypeAxisLine
        {
            legendWidthSpecified = false
        };
        axisData.start = Start;
        axisData.startSpecified = true;
        axisData.end = End;
        axisData.endSpecified = true;
        if (IsDisplayNumberFormatLocal)
        {
            NumberFormatSerializer numberFormatSerializer = new NumberFormatSerializer(DisplayNumberFormat, null);
            axisData.numberFormat = numberFormatSerializer.GetDataObject(resultManager);
        }
        return axisData;
    }

    private void ReadAxisDomainData(xyDomainType axisDomain, IResultManager resultManager)
    {
        _startExpression.SetDataObject(axisDomain.startValue, resultManager);
        _endExpression.SetDataObject(axisDomain.endValue, resultManager);
        _secondTickExpression.SetDataObject(axisDomain.secondTickValue, resultManager);
    }

    public void SetDataObject(axisBaseType axisData, IResultManager resultManager)
    {
        xyDomainType axisDomain = null;
        switch (Type)
        {
            case AxesType.X:
                if (axisData is xAxis xAxisData)
                {
                    axisDomain = xAxisData.xyDomain;
                }
                else
                {
                    DebugTools.DevAssert(false, null);
                }
                break;
            case AxesType.Y:
                if (axisData is yAxis yAxisData)
                {
                    axisDomain = yAxisData.xyDomain;
                }
                else
                {
                    DebugTools.DevAssert(false, null);
                }
                break;
            case AxesType.Z:
                if (axisData is zAxis zAxisData)
                {
                    axisDomain = zAxisData.zDomain;
                }
                else
                {
                    DebugTools.DevAssert(false, null);
                }
                break;
            default:
                DebugTools.DevAssert(false, null);
                break;
        }
        ReadAxisDomainData(axisDomain, resultManager);
        MajorTickMarks.Count = int.Parse(axisData.axisGrid.gridFrequency.Value, CultureInfo.InvariantCulture);
        ShowNumbers = axisData.axisGrid.gridLabels.display;
        ShowTicks = axisData.axisGrid.tickMarks.display;
        resultFormatType numberFormat = axisData.numberFormat;
        if (numberFormat != null)
        {
            DisplayNumberFormat = new NumberFormat();
            IsDisplayNumberFormatLocal = true;
            // The IL calls the serializer through a stored reference, not on the fresh instance.
            NumberFormatSerializer numberFormatSerializer = new NumberFormatSerializer(DisplayNumberFormat, null);
            numberFormatSerializer.SetDataObject(numberFormat, resultManager);
            FireNumberFormatInitialized();
        }
    }

    public XmlNode RegionXmlNode
    {
        get
        {
            return GetXmlNode(XmlWorksheetData.McdxNameSpaceManager.LookupNamespace(string.Empty), "http://schemas.mathsoft.com/math50");
        }
        set
        {
            SetXmlNode(value, XmlWorksheetData.McdxNameSpaceManager);
        }
    }

    public void SetLevels(double[] levels)
    {
        int num = levels.Length;
        if (num > 1)
        {
            DefaultTickMarks = num;
            double start = levels[0];
            double end = levels[num - 1];
            if (SetRangeToLinearAxis(start, end))
            {
                FireRangeChanged();
            }
        }
        DefaultTickMarks = AxisModelDefinitionCollection.Instance[Type, PlotType].TickMarksDefault;
    }

    private XmlNode GetXmlNode(string wsNameSpace, string mathNameSpace)
    {
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement xmlElement = xmlDocument.CreateElement(AxisXmlNodeName, wsNameSpace);
        xmlElement.SetAttribute("rank", "1");
        XmlElement axisLineElement = xmlDocument.CreateElement("axisLine", wsNameSpace);
        xmlElement.AppendChild(axisLineElement);
        if (AxesType.Z != Type)
        {
            xmlElement.SetAttribute("legend-position", ConvertLegendPositionIndexToLegendPositionEnum());
        }
        axisLineElement.SetAttribute("position", Position.ToString().ToLower());
        axisLineElement.SetAttribute("positionticmark", PositionTickMark.ToString());
        axisLineElement.SetAttribute("legendWidth", LegendWidth.ToString(CultureInfo.InvariantCulture));
        XmlElement axisGridElement = xmlDocument.CreateElement("axisGrid", wsNameSpace);
        xmlElement.AppendChild(axisGridElement);
        XmlElement gridFrequencyElement = xmlDocument.CreateElement("gridFrequency", wsNameSpace);
        axisGridElement.AppendChild(gridFrequencyElement);
        gridFrequencyElement.AppendChild(xmlDocument.CreateTextNode(MajorTickMarks.Count.ToString()));
        XmlElement gridLabelsElement = xmlDocument.CreateElement("gridLabels", wsNameSpace);
        axisGridElement.AppendChild(gridLabelsElement);
        gridLabelsElement.SetAttribute("display", XmlConvert.ToString(ShowNumbers));
        XmlElement gridLinesElement = xmlDocument.CreateElement("gridLines", wsNameSpace);
        axisGridElement.AppendChild(gridLinesElement);
        gridLinesElement.SetAttribute("display", XmlConvert.ToString(ShowGridLines));
        gridLinesElement.SetAttribute("color", GridLineColor.ToString());
        XmlElement tickMarksElement = xmlDocument.CreateElement("tickMarks", wsNameSpace);
        axisGridElement.AppendChild(tickMarksElement);
        tickMarksElement.SetAttribute("display", XmlConvert.ToString(MajorTickMarks.Show));
        if (AxisTitleModel != null)
        {
            XmlNode newChild = xmlDocument.ImportNode(AxisTitleModel.RegionXmlNode, deep: true);
            xmlElement.AppendChild(newChild);
        }
        XmlElement markersElement = xmlDocument.CreateElement("markers", wsNameSpace);
        xmlElement.AppendChild(markersElement);
        foreach (AxisMarker marker in Markers)
        {
            using XmlNodeWriter xmlWriter = new XmlNodeWriter(markersElement);
            axisBaseTypeMarker.XmlSerializer.Serialize(xmlWriter, marker.GetDataObject(ResultManager.Instance));
        }
        if (IsDisplayNumberFormatLocal)
        {
            using XmlNodeWriter writer = new XmlNodeWriter(xmlElement);
            ResultFormat resultFormat = new ResultFormat(DisplayNumberFormat, null, wsNameSpace);
            resultFormat.RootNodeFormatName = "numberFormat";
            resultFormat.WriteXml(writer);
        }
        XmlElement domainElement = xmlDocument.CreateElement(DomainXmlNodeName, wsNameSpace);
        xmlElement.AppendChild(domainElement);
        domainElement.SetAttribute("scale-type", Scaling.ToString().ToLower());
        domainElement.SetAttribute("auto-scale", XmlConvert.ToString(value: true));
        XmlElement startValueElement = xmlDocument.CreateElement("startValue", wsNameSpace);
        domainElement.AppendChild(startValueElement);
        if (StartExpression.IsDefaultValue)
        {
            XmlElement startPlaceholder = XmlWorksheetData.CreateMLElement(xmlDocument, "placeholder", mathNameSpace);
            startValueElement.AppendChild(startPlaceholder);
        }
        else
        {
            startValueElement.AppendChild(xmlDocument.ImportNode(StartExpression.RegionXmlNode, deep: true));
            ResultManager.Instance.CreateResultRef(startValueElement, StartExpression);
        }
        if (!SecondTickExpression.IsDefaultValue)
        {
            XmlElement secondTickElement = xmlDocument.CreateElement("secondTickValue", wsNameSpace);
            domainElement.AppendChild(secondTickElement);
            secondTickElement.AppendChild(xmlDocument.ImportNode(SecondTickExpression.RegionXmlNode, deep: true));
            ResultManager.Instance.CreateResultRef(secondTickElement, SecondTickExpression);
        }
        XmlElement endValueElement = xmlDocument.CreateElement("endValue", wsNameSpace);
        domainElement.AppendChild(endValueElement);
        if (EndExpression.IsDefaultValue)
        {
            XmlElement endPlaceholder = XmlWorksheetData.CreateMLElement(xmlDocument, "placeholder", mathNameSpace);
            endValueElement.AppendChild(endPlaceholder);
        }
        else
        {
            endValueElement.AppendChild(xmlDocument.ImportNode(EndExpression.RegionXmlNode, deep: true));
            ResultManager.Instance.CreateResultRef(endValueElement, EndExpression);
        }
        xmlElement.SetAttribute("start", Start.ToString(CultureInfo.InvariantCulture));
        xmlElement.SetAttribute("end", End.ToString(CultureInfo.InvariantCulture));
        return xmlElement;
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
                double? endValue = (EndExpression.IsValueValid && !IsStringLabel()) ? new double?(End) : max;
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
                        flag = SetRangeToLinearAxis(num.HasValue ? num.Value : StartDefaultByScaling, endValue.HasValue ? endValue.Value : EndDefault);
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
                if (defaultGridLine is Color gridLineColor)
                {
                    return gridLineColor;
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
        bool hasStartLimitError = LimitHasErrors(StartExpression);
        bool hasSecondTickLimitError = LimitHasErrors(SecondTickExpression);
        bool hasEndLimitError = LimitHasErrors(EndExpression);
        if (hasStartLimitError | hasSecondTickLimitError | hasEndLimitError)
        {
            FilteredLabels = new string[num];
            int lastLabelIndex = num - 1;
            FilteredLabels[0] = (hasStartLimitError ? ErrorIndicator : labels[0]);
            if (lastLabelIndex > 1)
            {
                FilteredLabels[1] = (hasSecondTickLimitError ? ErrorIndicator : labels[1]);
            }
            else
            {
                DebugTools.DevAssert(lastLabelIndex == 1, null);
            }
            for (int labelIndex = 2; labelIndex < lastLabelIndex; labelIndex++)
            {
                FilteredLabels[labelIndex] = string.Empty;
            }
            FilteredLabels[lastLabelIndex] = (hasEndLimitError ? ErrorIndicator : labels[lastLabelIndex]);
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
                double maxRadius = Math.Max(Math.Abs(min.Value), Math.Abs(max.Value));
                effectiveStart = Math.Max(0.0, min.Value);
                effectiveEnd = EndExpression.IsValueValid ? Math.Max(Math.Abs(min.Value), EndExpression.CalculatedValue) : maxRadius;
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
        double startExponent = Math.Floor(Math.Log10(start));
        double endExponent = Math.Ceiling(Math.Log10(end));
        double exponentRange = endExponent - startExponent;
        if (exponentRange < 0.5)
        {
            exponentRange = 1.0;
            startExponent = endExponent - exponentRange;
        }
        int tickExponentStep;
        int count;
        try
        {
            tickExponentStep = Convert.ToInt32(Math.Ceiling(exponentRange / (double)(DefaultTickMarks - 1)));
            count = Convert.ToInt32(exponentRange / (double)tickExponentStep + 1.0);
        }
        catch
        {
            return false;
        }
        Start = Math.Pow(10.0, startExponent);
        End = Math.Pow(10.0, endExponent);
        MajorTickMarks.Count = count;
        MajorTickMarks.Values = new double[MajorTickMarks.Count];
        MinorTickMarks.Count = (MajorTickMarks.Count - 1) * 8;
        MinorTickMarks.Values = new double[MinorTickMarks.Count];
        for (int i = 0; i < MajorTickMarks.Count - 1; i++)
        {
            double tickValue = Math.Pow(10.0, startExponent + (double)(tickExponentStep * i));
            MajorTickMarks.Values[i] = tickValue;
            for (int j = 0; j < 8; j++)
            {
                MinorTickMarks.Values[i * 8 + j] = tickValue * (double)(j + 2);
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
