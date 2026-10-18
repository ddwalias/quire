#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.DocumentAreaCommands;

public partial class DependencyPropertySetCommand
{
    [MirrorVerified("PtcControlsDocumentAreaCommandsDependencyPropertySetCommandMechanicalDifferentialTests")]
    private readonly DependencyProperty _property;

    [MirrorVerified("PtcControlsDocumentAreaCommandsDependencyPropertySetCommandMechanicalDifferentialTests")]
    private readonly DependencyObject _target;

    [MirrorVerified("PtcControlsDocumentAreaCommandsDependencyPropertySetCommandMechanicalDifferentialTests")]
    private object _old_value;

    [MirrorVerified("PtcControlsDocumentAreaCommandsDependencyPropertySetCommandMechanicalDifferentialTests")]
    public void Undo() => _target.SetValue(_property, _old_value);

    [MirrorVerified("PtcControlsDocumentAreaCommandsDependencyPropertySetCommandMechanicalDifferentialTests")]
    public void Redo() => Do();
}
