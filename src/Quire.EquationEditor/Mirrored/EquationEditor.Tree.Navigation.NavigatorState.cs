namespace Quire.EquationEditor.Tree.Navigation;

public partial struct NavigatorState
{
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
