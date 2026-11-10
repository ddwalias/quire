using System.Windows.Threading;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class InputScriptableControlModelWithInputs
{
    public IInputScriptableControlDataWithInputs Data => (IInputScriptableControlDataWithInputs)_data;

    public Dispatcher Dispatcher => _dispatcher;

    public string InputFieldSExpression
    {
        get => Data.InputFieldSExpression;
        set => Data.InputFieldSExpression = value;
    }

    public int? InputFieldSize
    {
        get => Data.InputFieldSize;
        set => Data.InputFieldSize = value;
    }

    public bool ShouldCommitAtLeastOnce { get; }

    public bool IsBlockRunningScript { get; set; }

    public bool ShouldCommitIfDirty => true;
}
