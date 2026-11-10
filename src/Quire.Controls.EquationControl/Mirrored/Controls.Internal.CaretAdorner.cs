namespace Quire.Controls.Internal;

internal partial class CaretAdorner
{
    public void SetCaretType(bool isReadWrite)
    {
        if (isReadWrite)
        {
            _caret.Color = MathcadSettings.Instance.CaretColor;
        }
        else
        {
            _caret.Color = MathcadSettings.Instance.ReadOnlyCaretColor;
        }
    }
}
