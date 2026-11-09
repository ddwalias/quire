using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial interface IInputControlEngine
{
    InputControlEngineTypes EngineType { get; }
}
