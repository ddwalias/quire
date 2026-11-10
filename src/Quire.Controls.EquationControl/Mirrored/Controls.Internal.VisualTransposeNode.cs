using Quire.Controls.Core;

namespace Quire.Controls.Internal;

internal partial class VisualTransposeNode
{
    private void SetOperatorElementFontSize(IMathFormat mathFormat)
    {
        OperatorElement.MathFormat.FontSize = FontManager.GetScaledFontSize(mathFormat, FontConstants.ScriptFontSizeRatio);
    }
}
