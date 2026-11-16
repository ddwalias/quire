namespace Quire.Controls.Plots.Plot3D.Tools;

internal partial interface ITrackingHelperFactory
{
    ITrackingHelper CreateHelper(IBaseTrackball trackball);
}
