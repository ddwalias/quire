using System;
using Quire.Mirror;

namespace Quire;

public partial struct TextRange
{
    [MirrorVerified("Release IL")]
    public TextRange(int startPosition, int endPosition)
    {
        if (startPosition < 0)
        {
            throw new ArgumentOutOfRangeException("startPosition", startPosition, "Negative values are not ok.  We would use uints but nobody else is doing it, so why should we");
        }
        if (endPosition < 0)
        {
            throw new ArgumentOutOfRangeException("endPosition", endPosition, "Negative values are not ok.  We would use uints but nobody else is doing it, so why should we");
        }
        CaretPosition = endPosition;
        if (startPosition < endPosition)
        {
            SelectionStart = startPosition;
            SelectionEnd = endPosition;
        }
        else
        {
            SelectionStart = endPosition;
            SelectionEnd = startPosition;
        }
    }

    [MirrorVerified("Release IL")]
    public bool IsEquivalentTo(TextRange another)
    {
        if (CaretPosition == another.CaretPosition && SelectionStart == another.SelectionStart)
        {
            return SelectionEnd == another.SelectionEnd;
        }
        return false;
    }
}
