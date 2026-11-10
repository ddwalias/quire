using Quire.Wpf;

namespace Quire.Controls;

public partial class VisualOperatorDescriptor
{
    private string _canonicalName;
    private EditorGesture _multiKeyStrokesBinding;

    public VisualOperatorDescriptor()
    {
        _command = EquationCommands.InsertOperator;
    }

    public string CanonicalName
    {
        get => _canonicalName;
        set => _canonicalName = value;
    }

    public EditorGesture MultiKeyStrokesBinding
    {
        get => _multiKeyStrokesBinding;
        set => _multiKeyStrokesBinding = value;
    }

    public override object CommandParameter => CanonicalName;

    public override string GeneratedKeybindingToDisplay => base.GeneratedKeybindingToDisplay ?? (string)new EditorGestureConverter().ConvertTo(MultiKeyStrokesBinding, typeof(string));

    public int OperatorOrdinal { get; set; }
}
