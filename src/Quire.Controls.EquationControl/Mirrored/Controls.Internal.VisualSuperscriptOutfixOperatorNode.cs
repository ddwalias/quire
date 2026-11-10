namespace Quire.Controls.Internal;

internal partial class VisualSuperscriptOutfixOperatorNode
{
    public static VisualSuperscriptOutfixOperatorNode CreateHorizontalBrackets()
    {
        return new VisualSuperscriptOutfixOperatorNode(true, VerticalFence.CreateLeftAngleBracket, VerticalFence.CreateRightAngleBracket);
    }

    public static VisualSuperscriptOutfixOperatorNode CreateVerticalBrackets()
    {
        return new VisualSuperscriptOutfixOperatorNode(false, HorizontalFence.CreateLowerAngleBracket, HorizontalFence.CreateUpperAngleBracket);
    }
}
