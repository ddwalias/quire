using Quire.Controls.Text;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Plots.Presenter;

public partial interface IPlotLegendItem
{
    IPlotTrace SourceTrace { get; set; }

    RichTextTitle LegendLabel { get; set; }

    textType GetDataObject(IS11NProvider s11NProvider);

    void SetDataObject(IDeS11NProvider d11NProvider, textType label);
}
