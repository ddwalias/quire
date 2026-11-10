namespace Quire.EquationEditor.Tree.Navigation;

public partial struct NavigatorState
{
    public bool IsEquivalentTo(NavigatorState another)
    {
        return IsEquivalentTo(another, false, false);
    }

    public bool IsEquivalentTo(NavigatorState another, bool enterSideConsidered, bool caretInsertionTypeConsidered)
    {
        bool equivalent = Current == another.Current &&
            CaretSide == another.CaretSide &&
            OperatorPartIndex == another.OperatorPartIndex &&
            ((!ValueRange.HasValue && !another.ValueRange.HasValue) ||
             (ValueRange.HasValue && ValueRange.HasValue && ValueRange.Value.IsEquivalentTo(another.ValueRange.Value)));

        if (enterSideConsidered)
        {
            equivalent &= EnterSide == another.EnterSide;
        }

        if (caretInsertionTypeConsidered)
        {
            equivalent &= CaretInsertionType == another.CaretInsertionType;
        }

        return equivalent;
    }
}
