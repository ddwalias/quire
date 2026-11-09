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

public partial class InputSliderEngineFactory
{
    public IInputControlEngine CreateEngine(InputControlEngineTypes engineType) => engineType switch
    {
        InputControlEngineTypes.VBScript => new InputSliderEngineVBScript(),
        InputControlEngineTypes.JScript => new InputSliderEngineJScript(),
        InputControlEngineTypes.Python => new InputSliderEnginePython(),
        _ => new InputSliderEngineDefaultScript(),
    };
}

public partial class InputTextBoxEngineFactory
{
    public IInputControlEngine CreateEngine(InputControlEngineTypes engineType) => engineType switch
    {
        InputControlEngineTypes.VBScript => new InputTextBoxEngineVBScript(),
        InputControlEngineTypes.JScript => new InputTextBoxEngineJScript(),
        InputControlEngineTypes.Python => new InputTextBoxEnginePython(),
        _ => new InputTextBoxEngineDefaultScript(),
    };
}
