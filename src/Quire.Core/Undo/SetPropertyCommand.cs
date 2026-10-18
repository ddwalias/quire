#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Undo;

public partial class SetPropertyCommand
{
    [MirrorVerified("PtcUndoSetPropertyCommandMechanicalDifferentialTests")]
    protected readonly DependencyObject _dependencyObject;

    [MirrorVerified("PtcUndoSetPropertyCommandMechanicalDifferentialTests")]
    private readonly DependencyProperty _dependencyProperty;

    [MirrorVerified("PtcUndoSetPropertyCommandMechanicalDifferentialTests")]
    protected virtual void ClearValue() => _dependencyObject.ClearValue(_dependencyProperty);

    [MirrorVerified("PtcUndoSetPropertyCommandMechanicalDifferentialTests")]
    private readonly object _newValue;

    [MirrorVerified("PtcUndoSetPropertyCommandMechanicalDifferentialTests")]
    public void Redo() => Value = _newValue;

    [MirrorVerified("PtcUndoSetPropertyCommandMechanicalDifferentialTests")]
    protected virtual object Value { get => _dependencyObject.GetValue(_dependencyProperty); set => _dependencyObject.SetValue(_dependencyProperty, value); }
}
