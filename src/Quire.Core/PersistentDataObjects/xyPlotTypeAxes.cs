#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class xyPlotTypeAxes
{
    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeAxesMechanicalDifferentialTests")]
    public xyPlotTypeAxes() { }

    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeAxesMechanicalDifferentialTests")]
    private xAxis xAxisField;

    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeAxesMechanicalDifferentialTests")]
    public xAxis xAxis { get => xAxisField; set => xAxisField = value; }

    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeAxesMechanicalDifferentialTests")]
    private yAxis[] yAxisField;

    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeAxesMechanicalDifferentialTests")]
    public yAxis[] yAxis { get => yAxisField; set => yAxisField = value; }
}
