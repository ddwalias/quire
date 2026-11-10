using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public abstract partial class InputControlEngine
{
    protected InputControlEngineTypes _engineType = InputControlEngineTypes.DefaultScript;

    protected InputControlEngine()
    {
    }

    public InputControlEngineTypes EngineType => _engineType;

    public abstract void Dispose();
}
