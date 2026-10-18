#nullable disable
using System.ComponentModel;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ViewModelBase
{
    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    protected virtual void ModelPropertyChanged(string propertyName) { }

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    protected virtual void OnDispose() { }

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    protected void CreateCommandBinding(ICommand command, Action<object> execute) => CreateCommandBinding(command, execute, null);

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    public virtual string DisplayName { get; protected set; }

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    private readonly CommandBindingCollection _commandBindings;

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    public CommandBindingCollection CommandBindings => _commandBindings;

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;
}
