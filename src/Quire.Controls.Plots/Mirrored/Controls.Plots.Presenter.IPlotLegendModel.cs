using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Quire.Serialization;

namespace Quire.Controls.Plots.Presenter;

public partial interface IPlotLegendModel : INotifyPropertyChanged, IPersistentDataProvider
{
    LegendPositions.PlotLegendPosition Position { get; set; }

    PlotType PlotType { get; }

    bool IsDirty { get; set; }

    bool ShowPlotLegend { get; set; }

    bool ShowBorder { get; set; }

    IEnumerable<IPlotLegendItem> PlotLegendItems { get; }

    XmlNode RegionXml { get; set; }

    IDeS11NProvider DeS11NProvider { get; set; }

    IS11NProvider S11NProvider { get; set; }

    double LegendWidth { get; set; }

    void AddLegendItem(IPlotLegendItem legendItem, int index);

    void RemoveLegendItem(IPlotLegendItem legendItem);
}
