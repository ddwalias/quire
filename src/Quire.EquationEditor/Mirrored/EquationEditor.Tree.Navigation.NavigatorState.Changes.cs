namespace Quire.EquationEditor.Tree.Navigation;

public partial struct NavigatorState
{
    public NavigatorState ChangeEnterSide(global::Quire.Direction enterSide)
    {
        return new NavigatorState(Current, CaretSide, OperatorPartIndex, enterSide, CaretInsertionTypes.Normal, ValueRange);
    }

    public NavigatorState ChangeValueRange(int caretPosition)
    {
        return ChangeValueRange(new TextRange(caretPosition));
    }

    public NavigatorState ChangeValueRange(int startPosition, int endPosition)
    {
        return ChangeValueRange(new TextRange(startPosition, endPosition));
    }

    public NavigatorState ChangeValueRange(TextRange? range)
    {
        if (range.Equals(ValueRange))
        {
            return this;
        }

        if (range.HasValue)
        {
            return new NavigatorState(Current, global::Quire.Direction.Indeterminate, null, EnterSide, CaretInsertionType, range);
        }

        return new NavigatorState(Current, CaretSide, OperatorPartIndex, EnterSide, CaretInsertionType, range);
    }

    public NavigatorState ChangeValueRangeAndInsertionType(int position, CaretInsertionTypes insertionType)
    {
        return new NavigatorState(Current, global::Quire.Direction.Indeterminate, null, EnterSide, insertionType, new TextRange(position));
    }

    public NavigatorState ToggleCaretInsertionType()
    {
        CaretInsertionTypes insertionType = CaretInsertionType == CaretInsertionTypes.Normal
            ? CaretInsertionTypes.Subscript
            : CaretInsertionTypes.Normal;
        return new NavigatorState(Current, CaretSide, OperatorPartIndex, EnterSide, insertionType, ValueRange);
    }
}
