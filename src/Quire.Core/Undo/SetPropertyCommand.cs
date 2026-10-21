#nullable disable
using System.Windows;
using System.Windows;
using Quire.Mirror;
using Quire.Mirror;
using Quire.Wpf;

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

    [MirrorVerified("Release IL")]
    public void Undo()
    {
        if (_hasLocalValue)
        {
            Value = _oldValue;
        }
        else
        {
            ClearValue();
        }
    }

    [MirrorVerified("PtcUndoSetPropertyCommandMechanicalDifferentialTests")]
    protected virtual object Value { get => _dependencyObject.GetValue(_dependencyProperty); set => _dependencyObject.SetValue(_dependencyProperty, value); }

    [MirrorVerified("Release IL")]
    protected SetPropertyCommand(DependencyObject dependencyObject, DependencyProperty dependencyProperty, object newValue)
    {
        _dependencyObject = dependencyObject;
        _dependencyProperty = dependencyProperty;
        _newValue = newValue;
    }

    [MirrorVerified("Release IL")]
    private bool _hasLocalValue;

    [MirrorVerified("Release IL")]
    private object _oldValue;

    [MirrorVerified("Release IL")]
    public bool Do()
    {
        _hasLocalValue = WpfUtils.HasLocalValue(_dependencyObject, _dependencyProperty);
        _oldValue = Value;
        Value = _newValue;
        return true;
    }

    [MirrorVerified("Release IL")]
    public static bool Execute(DependencyObject dependencyObject, DependencyProperty dependencyProperty, object newValue) => CommandBuffer.Execute(new SetPropertyCommand(dependencyObject, dependencyProperty, newValue), null);

    [MirrorVerified("Release IL")]
    public static bool Toggle(DependencyObject dependencyObject, DependencyProperty dependencyProperty) => CommandBuffer.Execute(new SetPropertyCommand(dependencyObject, dependencyProperty, !(bool)dependencyObject.GetValue(dependencyProperty)), null);

    [MirrorVerified("Release IL")]
    public string Name => string.Format("set {0}", _dependencyProperty.Name);
}
