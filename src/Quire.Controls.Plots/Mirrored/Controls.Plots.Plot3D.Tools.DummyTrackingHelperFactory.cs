using Quire.Services;

namespace Quire.Controls.Plots.Plot3D.Tools;

internal partial class DummyTrackingHelperFactory : ITrackingHelperFactory, IService
{
    private static ITrackingHelperFactory _instance;

    private static DummyTrackingHelper _helperInstance;

    public static ITrackingHelperFactory Instance => _instance ?? (_instance = new DummyTrackingHelperFactory());

    public bool IsConnected { get; set; }

    private DummyTrackingHelperFactory()
    {
    }

    public ITrackingHelper CreateHelper(IBaseTrackball trackball)
    {
        return _helperInstance ?? (_helperInstance = new DummyTrackingHelper());
    }
}
