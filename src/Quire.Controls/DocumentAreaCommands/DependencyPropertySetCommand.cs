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

    private readonly object _value;

    [MirrorVerified("PtcControlsDocumentAreaCommandsDependencyPropertySetCommandMechanicalDifferentialTests")]
    private object _old_value;

    public DependencyPropertySetCommand(DependencyObject target, DependencyProperty property, object value)
    {
        _property = property;
        _target = target;
        _value = value;
    }

    public bool Do()
    {
        _old_value = _target.GetValue(_property);
        _target.SetValue(_property, _value);
        return true;
    }

    [MirrorVerified("PtcControlsDocumentAreaCommandsDependencyPropertySetCommandMechanicalDifferentialTests")]
    public void Undo() => _target.SetValue(_property, _old_value);

    [MirrorVerified("PtcControlsDocumentAreaCommandsDependencyPropertySetCommandMechanicalDifferentialTests")]
    public void Redo() => Do();

    public string Name => string.Format("Set {0} Dependency Property Command", _property.Name);
}
