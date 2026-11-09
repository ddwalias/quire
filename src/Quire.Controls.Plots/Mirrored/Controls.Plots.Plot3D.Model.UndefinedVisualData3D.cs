namespace Quire.Controls.Plots.Plot3D.Model;

internal partial class UndefinedVisualData3D
{
    internal UndefinedVisualData3D()
        : base(TraceType3D.Undefined, null, 0)
    {
    }

    public override bool HasNoData => false;
}
