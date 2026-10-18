#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ProgressBarData
{
    [MirrorVerified("PtcWpfProgressBarDataMechanicalDifferentialTests")]
    public string MessageFormat { get; private set; }

    [MirrorVerified("PtcWpfProgressBarDataMechanicalDifferentialTests")]
    public INotifyProgress ProgressNotifier { get; private set; }
}
