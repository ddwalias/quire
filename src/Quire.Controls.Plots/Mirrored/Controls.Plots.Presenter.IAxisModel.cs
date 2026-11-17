#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Plots.Presenter;

public partial interface IAxisModel : IMarkersContainer, INotifyPropertyChanged
{
    IEnumerable<IEquationItem> Equations { get; }

    double StartDefault { get; }

    double EndDefault { get; }

    double LinearizedStart { get; }

    double LinearizedEnd { get; }

    int MaxTickmarks { get; }

    AxesType Type { get; }

    AxisScaling Scaling { get; set; }

    bool IsDiscrete { get; set; }

    XmlNode RegionXmlNode { get; set; }

    string DomainXmlNodeName { get; }

    Positions Position { get; set; }

    int PositionTickMark { get; set; }

    bool ShowTicks { get; set; }

    bool ShowNumbers { get; set; }

    bool ShowGridLines { get; set; }

    Color GridLineColor { get; set; }

    TickScale MajorTickMarks { get; set; }

    TickScale MinorTickMarks { get; set; }

    IPlotTitleModel AxisTitleModel { get; set; }

    IAxisLimitEditor StartExpression { get; set; }

    IAxisLimitEditor SecondTickExpression { get; set; }

    IAxisLimitEditor EndExpression { get; set; }

    int LegendPositionIndex { get; set; }

    double LegendWidth { get; set; }

    IEnumerable<ICalculatable> AxisCalculatables { get; }

    bool UseBetterTickMarks { get; set; }

    int DefaultTickMarks { get; }

    bool IsDisplayNumberFormatLocal { get; }

    string[] FilteredLabels { get; }

    PlotType PlotType { get; }

    bool XIsString { get; set; }

    bool YIsString { get; set; }

    event Func<IAxisModel, XmlNode, IPlotEquation> RequestAddEquation;

    event Action Changed;

    event Action<AxisScaling> ScalingChanged;

    event Action NumberFormatInitialized;

    event Action TickMarkLabelsUpdated;

    void AddEquation(IPlotEquation equation, int index);

    void RemoveEquation(IEquationItem equation);

    void AddEquation(XmlNode xmlNode);

    void SetRange(double? start, double? end);

    int GetNumberOfTracesForEquation(IPlotEquation equation);

    void SetTickMarksPattern(double startGap, double[] pattern);

    void StartValueChanged();

    void SetLevels(double[] levels);

    void SetDisplayNumberFormat(NumberFormat numberFormat, bool isLocalValue);

    bool Contains(double coordinate);

    axisBaseType GetDataObject(IResultManager resultManager);

    void SetDataObject(axisBaseType axisData, IResultManager resultManager);

    bool FormatTickMarkValues();

    void UpdateTickMarkValues(bool useSecondTickValue);

    double GetRadialPointsStart();
}
