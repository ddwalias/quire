namespace Quire.EquationEditor.Tree.Transform;

public static partial class ParsedSExpressionToComboBoxInputControlData
{
    private static bool a(ParsedSExpression parsed, int index, out string value)
    {
        ParsedSExpression item = parsed.SExpList[index];
        value = item.Value;
        return item.HasValue;
    }

    private static bool a(ParsedSExpression parsed, int index, out int value)
    {
        value = 0;
        ParsedSExpression item = parsed.SExpList[index];
        if (item.HasValue)
        {
            return int.TryParse(item.Value, out value);
        }

        return false;
    }
}
