namespace Quire.EquationEditor.Tree.Navigation;

public partial struct NavigatorState
{
    public NavigatorState(MathNode current, int caretPosition)
        : this(current, caretPosition, CaretInsertionTypes.Normal)
    {
    }

    public NavigatorState(MathNode current, int caretPosition, CaretInsertionTypes insertionType)
        : this(current, global::Quire.Direction.Indeterminate, null, global::Quire.Direction.Indeterminate, insertionType, new TextRange(caretPosition))
    {
    }

    public NavigatorState(MathNode current, TextRange range)
        : this(current, global::Quire.Direction.Indeterminate, null, global::Quire.Direction.Indeterminate, CaretInsertionTypes.Normal, range)
    {
    }

    public NavigatorState(MathNode current)
        : this(current, global::Quire.Direction.Indeterminate)
    {
    }

    public NavigatorState(MathNode current, global::Quire.Direction caretSide)
        : this(current, caretSide, global::Quire.Direction.Indeterminate)
    {
    }

    public NavigatorState(MathNode current, global::Quire.Direction caretSide, global::Quire.Direction nodeSide)
        : this(current, caretSide, nodeSide, global::Quire.Direction.Indeterminate)
    {
    }

    public NavigatorState(MathNode current, global::Quire.Direction caretSide, int? operatorPartIndex)
        : this(current, caretSide, operatorPartIndex, global::Quire.Direction.Indeterminate)
    {
    }

    public NavigatorState(MathNode current, global::Quire.Direction caretSide, global::Quire.Direction nodeSide, global::Quire.Direction enterSide)
        : this(current, caretSide, nodeSide, enterSide, CaretInsertionTypes.Normal, null)
    {
    }

    public NavigatorState(MathNode current, global::Quire.Direction caretSide, int? operatorPartIndex, global::Quire.Direction enterSide)
        : this(current, caretSide, operatorPartIndex, enterSide, CaretInsertionTypes.Normal, null)
    {
    }
}
