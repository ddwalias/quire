using System.Windows.Documents;

namespace Quire.Wpf;

public static partial class TextPointerExtensions
{
    public static TextPointer GetReversedTextPointer(this TextPointer textPtr)
    {
        if (textPtr.LogicalDirection != LogicalDirection.Forward)
        {
            return textPtr.GetPositionAtOffset(0, LogicalDirection.Forward);
        }
        return textPtr.GetPositionAtOffset(0, LogicalDirection.Backward);
    }
}
