namespace Quire.EquationEditor.Tree;

public partial class StringToComboBoxMathTransformerForConvertor
{
    public override MathNode TransformStringMathNode(string value)
    {
        if (ShouldWrapUnicodeQuotes)
        {
            if (value.IsWrappedWithQuotes(global::Quire.StringExtension.QuotesType.BothNonStraight))
            {
                value = "\"" + value + "\"";
            }
        }
        else
        {
            string text = value.Trim();
            if (text.IsWrappedWithQuotes(global::Quire.StringExtension.QuotesType.BothStraight))
            {
                value = text.Substring(1, text.Length - 2);
            }
        }

        return base.TransformStringMathNode(value);
    }
}
