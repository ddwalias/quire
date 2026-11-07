namespace Quire.EquationEditor.View;

public partial interface ICanBlockFocus
{
    bool BlockFocus { get; }

    bool IsAdornerFocused { get; }
}
