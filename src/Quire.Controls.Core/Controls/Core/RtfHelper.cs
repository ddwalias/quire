#nullable disable
using System;

namespace Quire.Controls.Core;

public static partial class RtfHelper
{
    private const double DnOffsetCoefficient = 1.5;
    private const double PixelsToTwips = 15.0;
    private const int DipsPerInch = 96;
    private const int RegionMinimumShowingSpace = 96;
    private const int WordWideMargin = 192;
    private const int WordPageMaxWidth = 2112;
    private const int RightPointRestriction = 27360;
    private const int TextboxSideFrameWidth = 50;
    private static readonly int TextboxFrameThicknessTwips = ConvertPixelsToTwips(0.5);
    public const string BaselineOffsetTagName = "baselineOffsetTag";

    private static int ConvertPixelsToTwips(double pixels)
    {
        return (int)Math.Round(pixels * 15.0);
    }

    private static int NormalizeHorizontalPosition(double leftPosition)
    {
        return Math.Min(ConvertPixelsToTwips(leftPosition), 27360);
    }
}
