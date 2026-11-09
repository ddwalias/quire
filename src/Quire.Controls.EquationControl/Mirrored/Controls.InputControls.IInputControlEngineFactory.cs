using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial interface IInputControlEngineFactory
{
    IInputControlEngine CreateEngine(InputControlEngineTypes engineType);
}
