using System.ComponentModel;
using System.Xml;
using Quire.Controls.Text;
using Quire.Serialization;

namespace Quire.Controls.Plots;

public partial interface IPlotTitleModel : INotifyPropertyChanged, IPersistentDataProvider
{
    LegendPositions.PlotTitlePosition Position { get; set; }

    RichTextTitle Title { get; set; }

    bool ShowTitle { get; set; }

    bool IsDirty { get; set; }

    XmlNode RegionXmlNode { get; set; }

    IS11NProvider S11NProvider { get; set; }

    IDeS11NProvider DeS11NProvider { get; set; }

    bool ShowBorder { get; set; }

    bool IsVertical { get; set; }

    PlotTitleType TitleType { get; }
}
