using Quire.Services;

namespace Quire.Controls.Plots.Plot3D.Tools;

internal partial class TrackingHelperFactory : ITrackingHelperFactory, IService
{
    private static ITrackingHelperFactory _instance;

    public static ITrackingHelperFactory Instance => _instance ?? (_instance = new TrackingHelperFactory());

    public bool IsConnected { get; set; }

    private TrackingHelperFactory()
    {
    }

    public ITrackingHelper CreateHelper(IBaseTrackball trackball)
    {
        return new TrackingHelper(trackball);
    }
}
