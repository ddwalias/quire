#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Quire.Controls.Core;

namespace Quire.Controls.Plots.Presenter;

public partial interface IPlotAxisView : IValueToScreen, INotifyPropertyChanged
{
    double Start { get; }

    double End { get; }

    Pair<double, double> ClipBounds { get; }

    int PositionTickMark { get; set; }

    Positions Position { get; set; }

    bool ShowNumbers { get; set; }

    bool ShowTicks { get; set; }

    bool ShowGridLines { get; set; }

    Color GridLineColor { get; set; }

    string[] Labels { get; set; }

    int TickMarks { get; set; }

    IPlotEquation this[int index] { get; }

    bool[] TickMarksVisabilityPattern { set; }

    TickScale MinorTickmarks { get; set; }

    PlotTitle AxisTitle { get; set; }

    int EquationCount { get; }

    double Offset { get; }

    AxisLimitEditor StartLimitEditor { get; set; }

    AxisLimitEditor SecondTickEditor { get; set; }

    AxisLimitEditor EndLimitEditor { get; set; }

    EquationControl StartExpressionEqn { get; }

    EquationControl SecondTickExpressionEqn { get; }

    EquationControl EndExpressionEqn { get; }

    AxisScaling Scaling { get; set; }

    bool IsDiscrete { get; set; }

    double[] TickMarksValues { get; set; }

    NumberFormat DisplayNumberFormat { get; set; }

    bool IsDisplayNumberFormatLocal { get; }

    PlotGraphAxis.Point2ScreenHelper Helper { get; }

    double LegendActualWidth { get; set; }

    GridSplitter Splitter { set; }

    event Action<int, int> RequestAddEquation;

    event Action<IPlotEquation, int, int, Direction> EquationRemoved;

    event Action Changed;

    event Action<IPlotEquation> ActiveEquationChanged;

    event Action<double> LegendWidthChanged;

    event SizeChangedEventHandler SizeChanged;

    event Action<NumberFormat, bool> NumberFormatChanged;

    void AddTraceToEquation(IPlotTrace trace, IPlotEquation equation);

    void AddTraceToEquation(IPlotTrace trace, IPlotEquation equation, int index);

    void RemoveTraceFromEquation(int traceIndex, IPlotEquation equation);

    IPlotEquation AddEquation(XmlNode xmlNode);

    IEquationListItem AddEquation(int index);

    void AddEquation(int index, IEquationListItem item);

    IEquationListItem RemoveEquation(int index, int previousIndex, Direction direction);

    IEnumerable<EquationControl> CollectElementsForTraversal();

    void AddEquation(string expression);

    double Point2Screen(double offset);

    void SetTickmarkPattern(double startGap, double[] pattern);

    void SetRange(double start, double end);

    void ShowStartExpression();

    void ShowSecondTickExpression();

    void ShowEndExpression();

    void HideStartExpression();

    void HideSecondTickExpression();

    void HideEndExpression();

    void OnNumberFormatInitialized(NumberFormat format);

    bool CanTraverse(FocusNavigationDirection direction);

    void HandleTraverse(FocusNavigationDirection direction);

    double GetPerpendicularAxisPositionRatio();
}
