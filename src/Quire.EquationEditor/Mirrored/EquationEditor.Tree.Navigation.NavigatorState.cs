namespace Quire.EquationEditor.Tree.Navigation;

public partial struct NavigatorState
{
    public readonly MathNode Current;
    public readonly global::Quire.Direction CaretSide;
    public readonly global::Quire.Direction NodeSide;
    public readonly int? OperatorPartIndex;
    public readonly global::Quire.Direction EnterSide;
    public readonly global::Quire.TextRange? ValueRange;
    public readonly CaretInsertionTypes CaretInsertionType;

    public NavigatorState(
        MathNode current,
        global::Quire.Direction caretSide,
        global::Quire.Direction nodeSide,
        global::Quire.Direction enterSide,
        CaretInsertionTypes insertionType,
        global::Quire.TextRange? valueRange)
        : this(current, caretSide, a(current, caretSide, nodeSide), enterSide, insertionType, valueRange)
    {
    }

    public NavigatorState(
        MathNode current,
        global::Quire.Direction caretSide,
        int? operatorPartIndex,
        global::Quire.Direction enterSide,
        CaretInsertionTypes insertionType,
        global::Quire.TextRange? valueRange)
    {
        if (current != null && !valueRange.HasValue && current.CarriesValue && current.HasCaretPositions && (!current.DistinctNodeEdges || caretSide == global::Quire.Direction.Indeterminate))
        {
            if (enterSide == global::Quire.Direction.Right)
            {
                valueRange = new global::Quire.TextRange(current.NodeInfoValueLength);
            }
            else
            {
                valueRange = new global::Quire.TextRange(0);
            }
        }

        Current = current;
        CaretSide = caretSide;
        OperatorPartIndex = operatorPartIndex;
        global::Quire.Direction nodeSide = global::Quire.Direction.Indeterminate;
        if (Current != null && Current.NavigablePartCount > 1)
        {
            if (OperatorPartIndex == 0)
            {
                nodeSide = global::Quire.Direction.Left;
            }
            else if (OperatorPartIndex == Current.NavigablePartCount - 1)
            {
                nodeSide = global::Quire.Direction.Right;
            }
        }

        NodeSide = nodeSide;
        EnterSide = enterSide;
        ValueRange = valueRange;
        CaretInsertionType = insertionType;
    }

    public int? CaretPosition
    {
        get
        {
            if (!ValueRange.HasValue)
            {
                return null;
            }
            return ValueRange.Value.CaretPosition;
        }
    }

    public int? SelectionStart
    {
        get
        {
            if (!ValueRange.HasValue)
            {
                return null;
            }
            return ValueRange.Value.SelectionStart;
        }
    }

    public int? SelectionEnd
    {
        get
        {
            if (!ValueRange.HasValue)
            {
                return null;
            }
            return ValueRange.Value.SelectionEnd;
        }
    }
}
