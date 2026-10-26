#nullable disable
using System;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class PageLayoutConstants
{
    public const double A1CmPageWidth = 59.4;

    public const double A1CmPageHeight = 84.1;

    public const double A2CmPageWidth = 42.0;

    public const double A2CmPageHeight = 59.4;

    public const double A3CmPageWidth = 29.7;

    public const double A3CmPageHeight = 42.0;

    public const double A4CmPageWidth = 21.0;

    public const double A4CmPageHeight = 29.7;

    public const double A5CmPageWidth = 14.8;

    public const double A5CmPageHeight = 21.0;

    public const double LetterInchPageWidth = 8.5;

    public const double LetterInchPageHeight = 11.0;

    public const double LegalInchPageWidth = 8.5;

    public const double LegalInchPageHeight = 14.0;

    public const double LedgerInchPageWidth = 11.0;

    public const double LedgerInchPageHeight = 17.0;

    public const double MetricCmMargin = 2.5;

    public const double MetricGridCellSizeInCm = 0.5;

    public const double MetricHeaderFooterLeftRightMarginInCm = 0.5;

    public const double MetricHeaderFooterTopBottomMarginInCm = 0.25;

    public const double MetricPageMarginLeftRightMinInCm = 0.5;

    public const double MetricPageMarginTopBottomMinInCm = 0.6;

    public const double USHeaderFooterLeftRightMarginInInches = 0.25;

    public const double USHeaderFooterTopBottomMarginInInches = 0.125;

    public const double USPageMarginLeftRightMinInInches = 0.25;

    public const double USPageMarginTopBottomMinInInches = 0.3;

    public const double USMarginInInches = 1.0;

    public const double USGridCellSizeInInches = 0.2;

    public const double LegacyMarginInInches = 1.2;

    public const double DefaultGridCellSizeInDips = 19.200000000000003;

    public const double NarrowMarginWidthMultiple = 0.5;

    public const double NarrowMarginHeightMultiple = 0.5;

    public const double WideMarginWidthMultiple = 2.0;

    public const double WideMarginHeightMultiple = 2.0;

    public const double FineGridSizeMultiple = 0.5;

    public const double HeaderFooterGridCellSizeMultiple = 0.5;

    public const double MetricMajorGridLinesInCm = 2.0;

    public const double USMajorGridLinesInInches = 1.0;

    static PageLayoutConstants()
    {
        DebugTools.DevAssert(0.5.IsEqualTo(0.5), null);
        DebugTools.DevAssert(0.25.IsEqualTo(0.25), null);
        DebugTools.DevAssert(0.5.IsGreaterThanOrEqual(0.5), null);
        DebugTools.DevAssert(0.25.IsGreaterThanOrEqual(0.2), null);
        DebugTools.DevAssert(0.6.IsGreaterThan(0.5), null);
        DebugTools.DevAssert(0.3.IsGreaterThan(0.25), null);
        DebugTools.DevAssert(0.5.IsEqualTo(0.5), null);
        DebugTools.DevAssert(0.25.IsEqualTo(0.25), null);
    }

    public static PageSizeUnits GetPageSizeUnitsByPageSize(PageSize pageSize)
    {
        switch (pageSize)
        {
            case PageSize.A1:
            case PageSize.A2:
            case PageSize.A3:
            case PageSize.A4:
            case PageSize.A5:
                return PageSizeUnits.Centimeters;
            case PageSize.Letter:
            case PageSize.Legal:
            case PageSize.Ledger:
                return PageSizeUnits.Inches;
            default:
                DebugTools.FailAssertion(new ArgumentOutOfRangeException("pageSize", pageSize, null));
                return PageSizeUnits.Centimeters;
        }
    }

    [MirrorVerified("PtcControlsCorePageLayoutConstantsMechanicalDifferentialTests")]
    public PageLayoutConstants() { }
}
