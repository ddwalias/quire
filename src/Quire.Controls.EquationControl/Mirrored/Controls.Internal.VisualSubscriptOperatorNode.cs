using Quire.Controls.Core;

namespace Quire.Controls.Internal;

internal partial class VisualSubscriptOperatorNode
{
    private void SetOperatorSymbolFontSize(IMathFormat mathFormat)
    {
        _operator.MathFormat.FontSize = FontManager.GetScaledFontSize(mathFormat, FontConstants.ScriptFontSizeRatio);
    }
}
