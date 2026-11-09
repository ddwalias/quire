using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class InputCheckBoxEngineFactory
{
    public IInputControlEngine CreateEngine(InputControlEngineTypes engineType)
    {
        return engineType switch
        {
            InputControlEngineTypes.VBScript => new InputCheckBoxEngineVBScript(),
            InputControlEngineTypes.JScript => new InputCheckBoxEngineJScript(),
            InputControlEngineTypes.Python => new InputCheckBoxEnginePython(),
            _ => new InputCheckBoxEngineDefaultScript(),
        };
    }
}
