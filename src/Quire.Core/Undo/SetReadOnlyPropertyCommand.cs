#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Undo;

public partial class SetReadOnlyPropertyCommand
{
    [MirrorVerified("PtcUndoSetReadOnlyPropertyCommandMechanicalDifferentialTests")]
    private readonly DependencyPropertyKey _dependencyPropertyKey;

    [MirrorVerified("PtcUndoSetReadOnlyPropertyCommandMechanicalDifferentialTests")]
    protected override void ClearValue() => _dependencyObject.ClearValue(_dependencyPropertyKey);

    [MirrorVerified("PtcUndoSetReadOnlyPropertyCommandMechanicalDifferentialTests")]
    protected override object Value { get => base.Value; set => _dependencyObject.SetValue(_dependencyPropertyKey, value); }
}
