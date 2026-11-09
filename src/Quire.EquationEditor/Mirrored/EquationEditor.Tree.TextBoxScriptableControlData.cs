using System.Windows;

namespace Quire.EquationEditor.Tree;

public partial class TextBoxScriptableControlData
{
    private bool c;

    private bool d;

    private bool n;

    private TextAlignment o;

    public TextBoxScriptableControlData(string mathNodeName, Quire.Controls.Core.InputControlTypes inputControlTypes)
        : base(mathNodeName, inputControlTypes)
    {
        Width = b;
        Height = a;
        Values = new System.Collections.ObjectModel.Collection<string> { string.Empty };
    }

    public bool AcceptsReturn
    {
        get => c;
        set => c = value;
    }

    public bool IsReadOnly
    {
        get => d;
        set => d = value;
    }

    public bool Border
    {
        get => n;
        set => n = value;
    }

    public TextAlignment TextAlignment
    {
        get => o;
        set => o = value;
    }
}
