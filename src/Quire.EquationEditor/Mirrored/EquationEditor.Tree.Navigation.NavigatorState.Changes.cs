namespace Quire.EquationEditor.Tree.Navigation;

public partial struct NavigatorState
{
    public global::Quire.Direction WhatSideOfTheNodeIsTheCaret()
    {
        if (CaretSide != global::Quire.Direction.Indeterminate)
        {
            return CaretSide;
        }

        if (Current != null && CaretPosition.HasValue && Current.CarriesValue && !Current.DistinctNodeEdges)
        {
            if (CaretPosition.Value == 0)
            {
                return global::Quire.Direction.Left;
            }

            if (CaretPosition.Value == Current.NodeInfoValueLength)
            {
                return global::Quire.Direction.Right;
            }
        }

        if (Current != null && Current.DescriptorName == "RESULT")
        {
            if (EnterSide == global::Quire.Direction.Left)
            {
                return global::Quire.Direction.Left;
            }

            return global::Quire.Direction.Right;
        }

        return global::Quire.Direction.Indeterminate;
    }

    public NavigatorState ChangeCurrentAndMakeValid(MathNode node)
    {
        if (node == null)
        {
            return Empty;
        }

        global::Quire.Direction direction = global::Quire.Direction.Indeterminate;
        if ((CaretSide == global::Quire.Direction.Left && node.AllowCaretOnLeft) ||
            (CaretSide == global::Quire.Direction.Right && node.AllowCaretOnRight))
        {
            direction = CaretSide;
        }

        int? operatorPartIndex = OperatorPartIndex;
        if (operatorPartIndex.HasValue && operatorPartIndex >= node.NavigablePartCount)
        {
            operatorPartIndex = node.NavigablePartCount - 1;
        }

        CaretInsertionTypes insertionType = CaretInsertionTypes.Normal;
        TextRange? valueRange = null;
        if (node.CarriesValue && node.HasCaretPositions && (!node.DistinctNodeEdges || direction == global::Quire.Direction.Indeterminate))
        {
            if (node.NodeInfo is IdentifierNodeInfo)
            {
                insertionType = CaretInsertionType;
            }

            if (!ValueRange.HasValue)
            {
                valueRange = new TextRange(0);
            }
            else
            {
                int valueLength = node.NodeInfoValueLength;
                int selectionStart = ValueRange.Value.SelectionStart;
                int selectionEnd = ValueRange.Value.SelectionEnd;
                valueRange = new TextRange(
                    selectionStart <= valueLength ? selectionStart : valueLength,
                    selectionEnd <= valueLength ? selectionEnd : valueLength);
            }
        }

        return new NavigatorState(node, direction, operatorPartIndex, EnterSide, insertionType, valueRange);
    }

    public static bool UpdateCurrentIfNeeded(
        global::System.Collections.Generic.IList<int> path,
        ref NavigatorState nstate,
        MathNode eroot)
    {
        bool result = true;
        if (!nstate.Current.IsRooted)
        {
            result = false;
            MathNode node = eroot.NodeFromPath(path);
            if (node != null)
            {
                NavigatorState updated = nstate.ChangeCurrent(node);
                nstate = updated;
                result = true;
            }
        }

        return result;
    }

    public NavigatorState ChangeCurrent(MathNode node)
    {
        return new NavigatorState(node, CaretSide, OperatorPartIndex, EnterSide, CaretInsertionType, ValueRange);
    }

    public NavigatorState ChangeCaretSide(global::Quire.Direction direction)
    {
        if (direction == CaretSide)
        {
            return this;
        }

        if (direction != global::Quire.Direction.Indeterminate)
        {
            return new NavigatorState(Current, direction, null, EnterSide, CaretInsertionTypes.Normal, null);
        }

        return new NavigatorState(Current, direction, OperatorPartIndex, EnterSide, CaretInsertionTypes.Normal, ValueRange);
    }

    public NavigatorState ChangeNodeSide(global::Quire.Direction direction)
    {
        if (direction == NodeSide)
        {
            return this;
        }

        if (direction != global::Quire.Direction.Indeterminate)
        {
            return new NavigatorState(Current, global::Quire.Direction.Indeterminate,
                a(Current, global::Quire.Direction.Indeterminate, direction), EnterSide, CaretInsertionTypes.Normal, null);
        }

        return new NavigatorState(Current, CaretSide, a(Current, CaretSide, direction),
            EnterSide, CaretInsertionTypes.Normal, ValueRange);
    }

    public NavigatorState ChangeOperatorPartIndex(int? operatorPartIndex)
    {
        if (operatorPartIndex == OperatorPartIndex)
        {
            return this;
        }

        if (operatorPartIndex.HasValue)
        {
            return new NavigatorState(Current, global::Quire.Direction.Indeterminate, operatorPartIndex,
                EnterSide, CaretInsertionTypes.Normal, null);
        }

        return new NavigatorState(Current, CaretSide, null, EnterSide, CaretInsertionTypes.Normal, ValueRange);
    }

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
