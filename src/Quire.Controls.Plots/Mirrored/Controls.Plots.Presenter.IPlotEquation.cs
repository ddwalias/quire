#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Xml;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.ViewModel;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Plots.Presenter;

public partial interface IPlotEquation : IDelegateFocus, IInputElement, IPlotEquationBase, ICurrentMathFormatProperty, INotifyPropertyChanged, IMathFormatOwner, ISupportCustomInitialization, IMathFormatForDeserialization
{
    string EquationSExpression { get; set; }

    bool AtLeftEdge { get; }

    bool IsPlotEquationEmpty { get; }

    [Obsolete("Usage of Exp property is deprecated. Use Expression instead.")]
    EquationControl Exp { get; }

    IEquationControl Expression { get; }

    IWhiteboardItem ExpressionItem { get; }

    [Obsolete("Scaling EquationControl should not be exposed directly.")]
    EquationControl Scaling { get; }

    IWhiteboardItem ScalingItem { get; }

    EquationControl ActiveEquation { get; }

    IEquationControl FocusedEquation { get; }

    double ActualWidth { get; }

    double ActualHeight { get; }

    int RegionErrorCount { get; }

    string RegionErrorCode { get; }

    string RegionErrorSExp { get; }

    string ScalingRl { get; }

    SynchronizationStatus SynchronizationStatus { get; }

    CalculatingStatus CalculatingStatus { get; }

    IEquationControl ScalingEquation { get; }

    IList<Unit> Units { set; }

    string EmptyPlotEquationSExp { get; }

    EngineError PlotCalculatableError { get; set; }

    IPlotEquationViewModelBase ViewModel { get; }

    event Action<IPlotEquation, Direction> RequestRemove;

    event Action RequestEvaluation;

    void NavigateRegionStart();

    void Commit();

    void Commit(bool fireEvent);

    plotEquation GetDataObject();

    void SetDataObject(plotEquation equationData);

    void SetRegionXmlNode(XmlNode xmlNode);

    XmlNode GetRegionXmlNode();
}
