using System.Windows.Documents;

namespace Quire.Wpf;

public static partial class TextPointerExtensions
{
    public static Hyperlink GetHyperlinkAdjacentToTextPointer(this TextPointer textPtr, out LogicalDirection direction)
    {
        direction = textPtr.LogicalDirection;
        if (textPtr.GetAdjacentElement(direction) is Hyperlink hyperlink)
        {
            return hyperlink;
        }
        direction = direction == LogicalDirection.Forward ? LogicalDirection.Backward : LogicalDirection.Forward;
        return textPtr.GetAdjacentElement(direction) as Hyperlink;
    }

    public static TextPointer GetPositionAtCharsOffset(this TextPointer startPointer, int skipNChars)
    {
        TextPointer textPointer = startPointer?.GetPositionAtOffset(0);
        if (skipNChars == 0)
        {
            return textPointer;
        }
        int num = skipNChars > 0 ? skipNChars : -skipNChars;
        LogicalDirection logicalDirection = skipNChars > 0 ? LogicalDirection.Forward : LogicalDirection.Backward;
        int num2 = 0;
        while (num2 < num && textPointer != null)
        {
            TextPointer textPointer2 = textPointer.GetNextInsertionPosition(logicalDirection);
            if (textPointer2 == null && num2 < num && logicalDirection == LogicalDirection.Forward)
            {
                textPointer2 = textPointer.DocumentEnd;
            }
            num2 = textPointer2 == null ? num2 : num2 + new global::System.Windows.Documents.TextRange(textPointer, textPointer2).Text.Length;
            textPointer = textPointer2;
        }
        if (!global::Quire.DebugTools.DevAssert(new global::System.Windows.Documents.TextRange(startPointer, textPointer).Text.Length == skipNChars, null).Passed)
        {
            return null;
        }
        return textPointer;
    }

    public static TextPointer GetReversedTextPointer(this TextPointer textPtr)
    {
        if (textPtr.LogicalDirection != LogicalDirection.Forward)
        {
            return textPtr.GetPositionAtOffset(0, LogicalDirection.Forward);
        }
        return textPtr.GetPositionAtOffset(0, LogicalDirection.Backward);
    }
}
