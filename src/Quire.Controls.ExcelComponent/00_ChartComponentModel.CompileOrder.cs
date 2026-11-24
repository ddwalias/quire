using System.Collections.Generic;

namespace Quire.Controls.ExcelComponent.Model;

// Compile-order device: this file sorts before Generated/, so the four fields and the constructor below precede
// CommitPre; Release IL numbers its closure as <CommitPre>b__5_0 (the member count before CommitPre is 5).
public partial class ChartComponentModel
{
    [global::Quire.Mirror.MirrorStub]
    private readonly Dictionary<ChartWrapperType, SimpleChartCalculatable> _defaultLimits;

    [global::Quire.Mirror.MirrorStub]
    private readonly ChartMinMaxCalculatable _chartMinMaxCalc;

    [global::Quire.Mirror.MirrorStub]
    private readonly ChartMinMaxPreCalculatable _chartMinMaxPreCalc;

    [global::Quire.Mirror.MirrorStub]
    private readonly SortedDictionary<uint, ChartTraceCalculatable> _traceCalculatables;

    [global::Quire.Mirror.MirrorStub]
    public ChartComponentModel() { }

    internal void CommitPre()
    {
        ConnectedDefaultLimits.Each(limit => limit.Commit());
    }
}
