using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class InputButtonEngineFactory
{
    public IInputControlEngine CreateEngine(InputControlEngineTypes engineType) => engineType switch
    {
        InputControlEngineTypes.VBScript => new InputButtonEngineVBScript(),
        InputControlEngineTypes.JScript => new InputButtonEngineJScript(),
        InputControlEngineTypes.Python => new InputButtonEnginePython(),
        _ => new InputButtonEngineDefaultScript(),
    };
}

public partial class InputRadioButtonEngineFactory
{
    public IInputControlEngine CreateEngine(InputControlEngineTypes engineType) => engineType switch
    {
        InputControlEngineTypes.VBScript => new InputRadioButtonEngineVBScript(),
        InputControlEngineTypes.JScript => new InputRadioButtonEngineJScript(),
        InputControlEngineTypes.Python => new InputRadioButtonEnginePython(),
        _ => new InputRadioButtonEngineDefaultScript(),
    };
}

public partial class InputListBoxEngineFactory
{
    public IInputControlEngine CreateEngine(InputControlEngineTypes engineType) => engineType switch
    {
        InputControlEngineTypes.VBScript => new InputListBoxEngineVBScript(),
        InputControlEngineTypes.JScript => new InputListBoxEngineJScript(),
        InputControlEngineTypes.Python => new InputListBoxEnginePython(),
        _ => new InputListBoxEngineDefaultScript(),
    };
}
