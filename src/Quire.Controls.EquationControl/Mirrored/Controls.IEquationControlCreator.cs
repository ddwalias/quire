using Quire.Controls.Core;

namespace Quire.Controls;

public partial interface IEquationControlCreator
{
    IEquationControl Create();

    IEquationControl CreateWithTextInput(string text);
}
