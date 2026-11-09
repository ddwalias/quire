using Quire.Controls.Core;

namespace Quire.Controls;

public partial class EquationControlCreator
{
    public IEquationControl Create() => new EquationControl();

    public IEquationControl CreateWithTextInput(string text) => EquationControl.CreateWithTextInput(text);
}
