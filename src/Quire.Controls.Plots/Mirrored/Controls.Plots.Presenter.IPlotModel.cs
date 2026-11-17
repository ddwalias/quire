#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Xml;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Core;

namespace Quire.Controls.Plots.Presenter;

internal partial interface IPlotModel : INotifyPropertyChanged
{
    PlotType Type { get; }

    IAxisModel rangeAxis { get; }

    IAxisModel PrimaryResultAxis { get; }

    IPlotTitleModel PlotTitleModel { get; }

    IPlotLegendModel PlotLegendModel { get; }

    IEnumerable<ITraceModel> Traces { get; }

    string RegionSExpression { get; }

    IEnumerable<ICalculatable> AxesLimitsExpressions { get; }

    bool HasOriginPositioning { get; set; }

    bool ShowAxisExpressions { get; set; }

    BackgroundTypeE BackgroundType { get; set; }

    Size GraphSize { get; set; }

    XmlNode PlotXmlNode { get; set; }

    event Action DeserializationFinished;

    event Action DeserializationStarted;

    ITraceModel AddTrace(IAxisModel axis1, IEquationItem equation1, IAxisModel axis2, IEquationItem equation2);

    void AddTrace(ITraceModel trace);

    void RemoveTrace(ITraceModel trace);

    TraceType GetDefaultTraceType();

    ITraceModel FirstTraceInAppearanceOrder();

    void DeserializeEpilog(bool isConvertedFromMathcad14);
}
