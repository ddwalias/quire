namespace Quire.Controls.Plots.Plot3D.Model;

internal partial class UserDefinedLimits
{
    private Interval _xInterval = CreateUndefinedInterval();

    private Interval _yInterval = CreateUndefinedInterval();

    private Interval _zInterval = CreateUndefinedInterval();

    internal string Rl => VectorFormatHelper.VectorRl(_xInterval.Lower, _xInterval.Upper, _yInterval.Lower, _yInterval.Upper, _zInterval.Lower, _zInterval.Upper);

    private static Interval CreateUndefinedInterval()
    {
        return new Interval
        {
            Lower = double.NaN,
            Upper = double.NaN
        };
    }

    internal void Set(double xStart, double xEnd, double yStart, double yEnd, double zStart, double zEnd, out bool xLimitsChanged, out bool yLimitsChanged, out bool zLimitsChanged)
    {
        xLimitsChanged = xStart.CompareTo(_xInterval.Lower) != 0 || xEnd.CompareTo(_xInterval.Upper) != 0;
        yLimitsChanged = yStart.CompareTo(_yInterval.Lower) != 0 || yEnd.CompareTo(_yInterval.Upper) != 0;
        zLimitsChanged = zStart.CompareTo(_zInterval.Lower) != 0 || zEnd.CompareTo(_zInterval.Upper) != 0;
        _xInterval.Lower = xStart;
        _xInterval.Upper = xEnd;
        _yInterval.Lower = yStart;
        _yInterval.Upper = yEnd;
        _zInterval.Lower = zStart;
        _zInterval.Upper = zEnd;
    }
}
