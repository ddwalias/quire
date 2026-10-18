#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class RelayCommand
{
    [MirrorVerified("PtcControlsCoreRelayCommandMechanicalDifferentialTests")]
    public RelayCommand(Action<object> execute) : this(execute, null) { }

    [MirrorVerified("PtcControlsCoreRelayCommandMechanicalDifferentialTests")]
    private readonly Action<object> _execute;

    [MirrorVerified("PtcControlsCoreRelayCommandMechanicalDifferentialTests")]
    public void Execute(object parameter) => _execute.Invoke(parameter);
}
