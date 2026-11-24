using System.Collections.Generic;

namespace Quire.Controls.ExcelComponent.Model;

// Compile-order device: this file sorts before Generated/, so the four fields and the constructor below precede
// CommitPre; Release IL numbers its closure as <CommitPre>b__5_0 (the member count before CommitPre is 5).
public partial class ChartComponentModel
{
    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("ChartComponentModel compile order")]
    private readonly Dictionary<ChartWrapperType, SimpleChartCalculatable> _defaultLimits;

    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("ChartComponentModel compile order")]
    private readonly ChartMinMaxCalculatable _chartMinMaxCalc;

    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("ChartComponentModel compile order")]
    private readonly ChartMinMaxPreCalculatable _chartMinMaxPreCalc;

    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("ChartComponentModel compile order")]
    private readonly SortedDictionary<uint, ChartTraceCalculatable> _traceCalculatables;

    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("ChartComponentModel constructor order")]
    public ChartComponentModel() { }

    internal void CommitPre()
    {
        ConnectedDefaultLimits.Each(limit => limit.Commit());
    }

    internal void CommitPost()
    {
        _chartMinMaxCalc.Commit();
    }

    public override void Commit()
    {
        CalculatablesConnectedToTheEngine.Each(calc => calc.Commit());
        base.Commit();
    }
}
