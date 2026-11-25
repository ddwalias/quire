#nullable disable
#pragma warning disable

using System.ComponentModel;
using System.Linq;
using System.Windows;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Model;

internal partial class PlotModel
{
    private const string TitleXmlNodeName = "ws:title";

    private static readonly global::System.Windows.Size _defaultViewPortDpi;

    public PlotModel() { }

    public PlotModel(global::Quire.Controls.Plots.Presenter.PlotType type, global::System.Windows.Size viewPortDpi, global::Quire.Controls.Plots.IPlotTitleModel titleModel, global::Quire.Controls.Plots.IPlotTitleModel xAxisTitleModel, global::Quire.Controls.Plots.IPlotTitleModel yAxisTitleModel) { }

    private readonly global::System.Collections.Generic.List<global::Quire.Controls.Plots.Presenter.ITraceModel> _traces;

    private readonly global::Quire.Controls.Plots.Presenter.IAxisModel _rangeAxis;

    private readonly global::Quire.Controls.Plots.Presenter.IAxisModel _primaryResultAxis;

    private readonly global::Quire.Controls.Plots.IPlotTitleModel _plotTitleModel;

    private readonly global::Quire.Controls.Plots.Presenter.IPlotLegendModel _plotLegendModel;

    private readonly global::System.Windows.Size _viewPortDpi;

    private bool _showAxisExpressions;

    public bool ShowAxisExpressions
    {
        get
        {
            return _showAxisExpressions;
        }

        set
        {
            _showAxisExpressions = value;
            Notify(PropertyNames.ShowAxisExpressions);
        }
    }

    private void Notify(string propName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    public global::Quire.Controls.Plots.Presenter.PlotType Type { get; private set; }

    public global::Quire.Controls.Plots.Presenter.IPlotEquation ActiveEquation { get; set; }

    public global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Plots.Presenter.ITraceModel> Traces { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public string RegionSExpression { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public global::System.Xml.XmlNode PlotXmlNode { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private void createPlot(global::System.Xml.XmlNode typeNode) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DeserializeLegendItems(global::System.Xml.XmlNode typeNode, global::System.Xml.XmlNamespaceManager nmspmgr) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static global::System.Xml.XmlNode GetAxisXmlNode(global::System.Xml.XmlDocument xmlDoc, global::Quire.Controls.Plots.Presenter.IAxisModel axis) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Plots.Presenter.ITraceModel> SortedTraces { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public global::System.Xml.XmlNode GraphRegionXmlNode { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private global::System.Windows.Size _graphSize;

    public Size GraphSize
    {
        get => _graphSize;
        set => _graphSize = value;
    }

    public IAxisModel rangeAxis => _rangeAxis;

    public IAxisModel PrimaryResultAxis => _primaryResultAxis;

    public IPlotTitleModel PlotTitleModel => _plotTitleModel;

    public IPlotLegendModel PlotLegendModel => _plotLegendModel;

    public bool HasOriginPositioning { get; set; }

    public global::Quire.Controls.Core.BackgroundTypeE BackgroundType { get; set; }

    public event global::System.Action DeserializationFinished;

    public event global::System.Action DeserializationStarted;

    public void DeserializeEpilog(bool isConvertedFromMathcad14) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void FireDeserializationFinished()
    {
        if (DeserializationFinished != null)
        {
            DeserializationFinished();
        }
    }

    public void FireDeserializationStarted()
    {
        if (DeserializationStarted != null)
        {
            DeserializationStarted();
        }
    }

    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> AxesLimitsExpressions => _rangeAxis.AxisCalculatables.Union(_primaryResultAxis.AxisCalculatables);

    public ITraceModel AddTrace(IAxisModel axis1, IEquationItem equation1, IAxisModel axis2, IEquationItem equation2)
    {
        ITraceModel newTrace = new TraceModel(Type, equation1, equation2, axis1, axis2, _viewPortDpi);
        AddTrace(newTrace);
        return newTrace;
    }

    public void AddTrace(ITraceModel newTrace)
    {
        _traces.Add(newTrace);
    }

    public void RemoveTrace(ITraceModel trace)
    {
        _traces.Remove(trace);
    }

    public TraceType GetDefaultTraceType()
    {
        if (_traces.All(trace => trace.Type != TraceType.Effects))
        {
            return TraceType.Lines;
        }

        return TraceType.Effects;
    }

    public ITraceModel FirstTraceInAppearanceOrder()
    {
        ITraceModel result = null;
        IEquationItem equation = rangeAxis.Equations.FirstOrDefault();
        if (equation != null)
        {
            result = equation.Traces.FirstOrDefault();
        }
        return result;
    }
}
